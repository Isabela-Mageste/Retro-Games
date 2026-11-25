using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

// Desafio, Módulo de Compras: regras do PedidoService (C04 e C05), com os repositórios e o
// usuário atual simulados por Moq.
public class PedidoServiceTests
{
    private const int IdCliente = 7;

    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPedidoRepository> _pedidos = new();
    private readonly Mock<IItemPedidoRepository> _itens = new();
    private readonly Mock<ISituacaoPedidoRepository> _situacoes = new();
    private readonly Mock<IRepositorySet> _repositorio = new();
    private readonly Mock<IUsuarioAtualService> _usuarioAtual = new();

    public PedidoServiceTests()
    {
        _repositorio.Setup(r => r.Jogo).Returns(_jogos.Object);
        _repositorio.Setup(r => r.Pedido).Returns(_pedidos.Object);
        _repositorio.Setup(r => r.ItemPedido).Returns(_itens.Object);
        _repositorio.Setup(r => r.SituacaoPedido).Returns(_situacoes.Object);
        _usuarioAtual.Setup(u => u.IdUsuario).Returns(IdCliente);

        // Padrão: nenhum item gravado. Os testes que precisam de itens configuram os seus.
        _itens.Setup(r => r.ListarPorPedidosAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _situacoes.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new SituacaoPedido { Id = SituacaoPedidoIds.Pendente, Descricao = "Pendente" },
            new SituacaoPedido { Id = SituacaoPedidoIds.Pago, Descricao = "Pago" },
        ]);
    }

    private PedidoService CriarService() => new(_repositorio.Object, _usuarioAtual.Object);

    private static Jogo NovoJogo(int id, string nome, decimal preco, int estoque, bool ativo = true) =>
        new() { Id = id, Nome = nome, Preco = preco, Estoque = estoque, Ativo = ativo };

    private void ConfigurarJogos(params Jogo[] jogos) =>
        _jogos.Setup(r => r.ListarPorIdsAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(jogos);

    private void VerificarQueNadaFoiGravado()
    {
        _pedidos.Verify(r => r.InserirAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()), Times.Never);
        _itens.Verify(r => r.InserirAsync(It.IsAny<IReadOnlyCollection<ItemPedido>>(), It.IsAny<CancellationToken>()), Times.Never);
        _jogos.Verify(r => r.BaixarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Finalizar_JogoInexistente_LancaJogoIndisponivelSemGravar()
    {
        // Arrange: o repositório não encontra o jogo 99
        ConfigurarJogos(NovoJogo(1, "Tetris", 159.90m, 5));

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => CriarService().FinalizarAsync(
            [new ItemPedido { IdJogo = 1, Quantidade = 1 }, new ItemPedido { IdJogo = 99, Quantidade = 1 }]));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_INDISPONIVEL, erro.Erro);
        VerificarQueNadaFoiGravado();
    }

    [Fact]
    public async Task Finalizar_JogoInativo_LancaJogoIndisponivel()
    {
        ConfigurarJogos(NovoJogo(24, "NBA Jam", 169.90m, 6, ativo: false));

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().FinalizarAsync([new ItemPedido { IdJogo = 24, Quantidade = 1 }]));

        Assert.Equal(BusinessErrors.JOGO_INDISPONIVEL, erro.Erro);
        VerificarQueNadaFoiGravado();
    }

    [Fact]
    public async Task Finalizar_EstoqueInsuficiente_LancaEstoqueInsuficienteComONomeDoJogoSemGravar()
    {
        ConfigurarJogos(NovoJogo(4, "The Legend of Zelda", 69.90m, 2));

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().FinalizarAsync([new ItemPedido { IdJogo = 4, Quantidade = 3 }]));

        Assert.Equal(BusinessErrors.ESTOQUE_INSUFICIENTE, erro.Erro);
        Assert.Equal("Estoque insuficiente para The Legend of Zelda.", erro.Message);
        VerificarQueNadaFoiGravado();
    }

    [Fact]
    public async Task Finalizar_EstoqueAcabaDuranteATransacao_LancaEstoqueInsuficiente()
    {
        // Arrange: a conferência passa, mas outra compra levou o estoque antes da baixa
        ConfigurarJogos(NovoJogo(22, "Tetris", 159.90m, 1));
        _pedidos.Setup(r => r.InserirAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>())).ReturnsAsync(10);
        _jogos.Setup(r => r.BaixarEstoqueAsync(22, 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().FinalizarAsync([new ItemPedido { IdJogo = 22, Quantidade = 1 }]));

        // Assert: o erro sai antes do Complete(), então a transação é desfeita
        Assert.Equal(BusinessErrors.ESTOQUE_INSUFICIENTE, erro.Erro);
    }

    [Fact]
    public async Task Finalizar_PrecoEnviadoPeloFront_UsaOPrecoDoBancoNoTotal()
    {
        // Arrange: o item chega com um preço falso; o banco diz 54,90
        ConfigurarJogos(NovoJogo(1, "Super Mario Bros.", 54.90m, 3));
        Pedido? gravado = null;
        _pedidos.Setup(r => r.InserirAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()))
            .Callback<Pedido, CancellationToken>((pedido, _) => gravado = pedido)
            .ReturnsAsync(10);
        _jogos.Setup(r => r.BaixarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        await CriarService().FinalizarAsync([new ItemPedido { IdJogo = 1, Quantidade = 2, PrecoUnitario = 0.01m }]);

        // Assert
        Assert.Equal(109.80m, gravado!.Total);
        _itens.Verify(r => r.InserirAsync(
            It.Is<IReadOnlyCollection<ItemPedido>>(itens => itens.Single().PrecoUnitario == 54.90m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Finalizar_DadosValidos_GravaPedidoPendenteEmUtcComItensEBaixaDeEstoque()
    {
        // Arrange
        ConfigurarJogos(NovoJogo(1, "Super Mario Bros.", 54.90m, 3), NovoJogo(7, "Super Metroid", 84.90m, 4));
        Pedido? gravado = null;
        _pedidos.Setup(r => r.InserirAsync(It.IsAny<Pedido>(), It.IsAny<CancellationToken>()))
            .Callback<Pedido, CancellationToken>((pedido, _) => gravado = pedido)
            .ReturnsAsync(10);
        _jogos.Setup(r => r.BaixarEstoqueAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _itens.Setup(r => r.ListarPorPedidosAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new ItemPedido { Id = 1, IdPedido = 10, IdJogo = 1, Quantidade = 1, PrecoUnitario = 54.90m },
            new ItemPedido { Id = 2, IdPedido = 10, IdJogo = 7, Quantidade = 2, PrecoUnitario = 84.90m },
        ]);
        var antes = DateTime.UtcNow;

        // Act
        var pedido = await CriarService().FinalizarAsync(
            [new ItemPedido { IdJogo = 1, Quantidade = 1 }, new ItemPedido { IdJogo = 7, Quantidade = 2 }]);

        // Assert
        Assert.Equal(IdCliente, gravado!.IdUsuario);
        Assert.Equal(SituacaoPedidoIds.Pendente, gravado.IdSituacaoPedido);
        Assert.Equal(DateTimeKind.Utc, gravado.DataPedido.Kind);
        Assert.InRange(gravado.DataPedido, antes, DateTime.UtcNow);
        Assert.Equal(224.70m, gravado.Total);
        _itens.Verify(r => r.InserirAsync(
            It.Is<IReadOnlyCollection<ItemPedido>>(itens => itens.Count == 2 && itens.All(item => item.IdPedido == 10)),
            It.IsAny<CancellationToken>()), Times.Once);
        _jogos.Verify(r => r.BaixarEstoqueAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
        _jogos.Verify(r => r.BaixarEstoqueAsync(7, 2, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal("Pendente", pedido.DescricaoSituacao);
        Assert.Equal(new[] { "Super Mario Bros.", "Super Metroid" }, pedido.Itens.Select(item => item.NomeJogo));
    }

    [Fact]
    public async Task ListarMeus_ClienteLogado_ConsultaSoOsPedidosDeleEMontaOsItens()
    {
        // Arrange
        _pedidos.Setup(r => r.ListarPorUsuarioAsync(IdCliente, It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new Pedido { Id = 11, IdUsuario = IdCliente, IdSituacaoPedido = SituacaoPedidoIds.Pago, Total = 159.90m },
            new Pedido { Id = 10, IdUsuario = IdCliente, IdSituacaoPedido = SituacaoPedidoIds.Pendente, Total = 54.90m },
        ]);
        _itens.Setup(r => r.ListarPorPedidosAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new ItemPedido { IdPedido = 10, IdJogo = 1, Quantidade = 1, PrecoUnitario = 54.90m },
            new ItemPedido { IdPedido = 11, IdJogo = 22, Quantidade = 1, PrecoUnitario = 159.90m },
        ]);
        ConfigurarJogos(NovoJogo(1, "Super Mario Bros.", 54.90m, 3), NovoJogo(22, "Tetris", 159.90m, 0));

        // Act
        var pedidos = await CriarService().ListarMeusAsync();

        // Assert: a ordem do repositório (mais recente primeiro) é mantida
        Assert.Equal(new[] { 11, 10 }, pedidos.Select(pedido => pedido.Pedido.Id));
        Assert.Equal("Pago", pedidos[0].DescricaoSituacao);
        Assert.Equal("Tetris", pedidos[0].Itens.Single().NomeJogo);
        _pedidos.Verify(r => r.ListarPorUsuarioAsync(It.Is<int>(id => id != IdCliente), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ListarMeus_SemPedidos_DevolveListaVaziaSemConsultarItens()
    {
        _pedidos.Setup(r => r.ListarPorUsuarioAsync(IdCliente, It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var pedidos = await CriarService().ListarMeusAsync();

        Assert.Empty(pedidos);
        _itens.Verify(r => r.ListarPorPedidosAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
