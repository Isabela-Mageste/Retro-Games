using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using RetroGames.Services.ServiceModels;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

// JogoService com os repositórios e o usuário atual substituídos por mocks (RN03 a RN05).
public class JogoServiceTests
{
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IJogoGeneroRepository> _jogoGeneros = new();
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<ICondicaoJogoRepository> _condicoes = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IRepositorySet> _repositorio = new();
    private readonly Mock<IUsuarioAtualService> _usuarioAtual = new();

    public JogoServiceTests()
    {
        _repositorio.Setup(r => r.Jogo).Returns(_jogos.Object);
        _repositorio.Setup(r => r.JogoGenero).Returns(_jogoGeneros.Object);
        _repositorio.Setup(r => r.Plataforma).Returns(_plataformas.Object);
        _repositorio.Setup(r => r.CondicaoJogo).Returns(_condicoes.Object);
        _repositorio.Setup(r => r.Genero).Returns(_generos.Object);

        // Tabelas de apoio usadas na montagem do detalhe e na conferência das referências.
        _plataformas.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Plataforma { Id = 1, Nome = "NES" }, new Plataforma { Id = 2, Nome = "Super Nintendo" }]);
        _plataformas.Setup(r => r.BuscarPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Plataforma { Id = 1, Nome = "NES" });
        _condicoes.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new CondicaoJogo { Id = 2, Descricao = "Usado" }]);
        _generos.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Genero { Id = 1, Nome = "Ação" }, new Genero { Id = 3, Nome = "Plataforma" }]);
        _jogoGeneros.Setup(r => r.ListarPorJogosAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new JogoGenero { IdJogo = 1, IdGenero = 3 }, new JogoGenero { IdJogo = 1, IdGenero = 1 }]);
    }

    private JogoService CriarService() => new(_repositorio.Object, _usuarioAtual.Object);

    private static Jogo NovoJogo(int id = 1, bool ativo = true) => new()
    {
        Id = id, Nome = "Super Mario Bros.", AnoLancamento = 1985, Preco = 54.90m, Estoque = 3,
        Ativo = ativo, IdPlataforma = 1, IdCondicaoJogo = 2,
    };

    private void ConfigurarListagem() =>
        _jogos.Setup(r => r.ListarAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<IReadOnlyCollection<int>?>(),
                It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(([NovoJogo()], 1L));

    [Fact]
    public async Task Listar_ComGenero_EnviaAoSqlOsIdsDosJogosDoGenero()
    {
        _jogoGeneros.Setup(r => r.ListarTodosAsync(It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            new JogoGenero { IdJogo = 1, IdGenero = 3 },
            new JogoGenero { IdJogo = 2, IdGenero = 1 },
            new JogoGenero { IdJogo = 6, IdGenero = 3 },
        ]);
        ConfigurarListagem();

        await CriarService().ListarAsync(new ListarJogoFiltroServiceModel { IdGenero = 3 });

        _jogos.Verify(r => r.ListarAsync(null, null,
            It.Is<IReadOnlyCollection<int>?>(ids => ids != null && ids.SequenceEqual(new[] { 1, 6 })),
            false, 1, 12, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Listar_SemGenero_NaoConsultaVinculosNemRestringeIds()
    {
        // Arrange
        ConfigurarListagem();

        // Act
        await CriarService().ListarAsync(new ListarJogoFiltroServiceModel());

        // Assert: sem gênero, a lista de ids chega nula (filtro desligado) e os vínculos não são lidos
        _jogoGeneros.Verify(r => r.ListarTodosAsync(It.IsAny<CancellationToken>()), Times.Never);
        _jogos.Verify(r => r.ListarAsync(null, null, null, false, 1, 12, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Listar_IncluirInativosSemSerAdministrador_ListaSoAtivos()
    {
        _usuarioAtual.Setup(u => u.EhAdministrador).Returns(false);
        ConfigurarListagem();

        await CriarService().ListarAsync(new ListarJogoFiltroServiceModel { IncluirInativos = true });

        _jogos.Verify(r => r.ListarAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<IReadOnlyCollection<int>?>(),
            false, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Listar_IncluirInativosComoAdministrador_IncluiInativos()
    {
        _usuarioAtual.Setup(u => u.EhAdministrador).Returns(true);
        ConfigurarListagem();

        await CriarService().ListarAsync(new ListarJogoFiltroServiceModel { IncluirInativos = true });

        _jogos.Verify(r => r.ListarAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<IReadOnlyCollection<int>?>(),
            true, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Listar_MontaDetalheComPlataformaCondicaoEGenerosOrdenados()
    {
        ConfigurarListagem();

        var pagina = await CriarService().ListarAsync(new ListarJogoFiltroServiceModel());

        var detalhe = Assert.Single(pagina.Itens);
        Assert.Equal("NES", detalhe.NomePlataforma);
        Assert.Equal("Usado", detalhe.DescricaoCondicao);
        Assert.Equal(new[] { "Ação", "Plataforma" }, detalhe.Generos.Select(g => g.Nome));
        Assert.Equal(1, pagina.TotalPaginas);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaJogoNaoEncontrado()
    {
        // Arrange
        _jogos.Setup(r => r.BuscarPorIdAsync(9999, It.IsAny<CancellationToken>())).ReturnsAsync((Jogo?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => CriarService().BuscarPorIdAsync(9999));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
    }

    [Fact]
    public async Task BuscarPorId_InativoSemSerAdministrador_LancaJogoNaoEncontrado()
    {
        _jogos.Setup(r => r.BuscarPorIdAsync(24, It.IsAny<CancellationToken>())).ReturnsAsync(NovoJogo(24, ativo: false));

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => CriarService().BuscarPorIdAsync(24));

        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
    }

    [Fact]
    public async Task BuscarPorId_InativoComoAdministrador_DevolveDetalhe()
    {
        _usuarioAtual.Setup(u => u.EhAdministrador).Returns(true);
        _jogos.Setup(r => r.BuscarPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(NovoJogo(1, ativo: false));

        var detalhe = await CriarService().BuscarPorIdAsync(1);

        Assert.False(detalhe.Jogo.Ativo);
    }

    [Fact]
    public async Task Inserir_PlataformaInexistente_LancaPlataformaNaoEncontrada()
    {
        var jogo = NovoJogo();
        jogo.IdPlataforma = 99;

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => CriarService().InserirAsync(jogo, [1]));

        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
        _jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_CondicaoInexistente_LancaCondicaoNaoEncontrada()
    {
        var jogo = NovoJogo();
        jogo.IdCondicaoJogo = 9;

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => CriarService().InserirAsync(jogo, [1]));

        Assert.Equal(BusinessErrors.CONDICAO_NAO_ENCONTRADA, erro.Erro);
        _jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_GeneroInexistente_LancaGeneroNaoEncontradoSemGravar()
    {
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => CriarService().InserirAsync(NovoJogo(), [1, 99]));

        Assert.Equal(BusinessErrors.GENERO_NAO_ENCONTRADO, erro.Erro);
        _jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_GenerosRepetidos_GravaJogoAtivoEVinculosSemRepeticao()
    {
        _jogos.Setup(r => r.InserirAsync(It.IsAny<Jogo>(), It.IsAny<CancellationToken>())).ReturnsAsync(25);
        var jogo = NovoJogo(0, ativo: false);
        jogo.Nome = "  Contra ";

        var criado = await CriarService().InserirAsync(jogo, [1, 1, 3]);

        Assert.Equal(25, criado.Jogo.Id);
        Assert.True(criado.Jogo.Ativo);
        Assert.Equal("Contra", criado.Jogo.Nome);
        _jogoGeneros.Verify(r => r.InserirAsync(25,
            It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { 1, 3 })), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Desativar_Inexistente_LancaJogoNaoEncontrado()
    {
        _jogos.Setup(r => r.BuscarPorIdAsync(9999, It.IsAny<CancellationToken>())).ReturnsAsync((Jogo?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => CriarService().DesativarAsync(9999));

        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
        _jogos.Verify(r => r.DesativarAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Desativar_Existente_SoMarcaComoInativo()
    {
        _jogos.Setup(r => r.BuscarPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(NovoJogo());

        await CriarService().DesativarAsync(1);

        _jogos.Verify(r => r.DesativarAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _jogos.Verify(r => r.AlterarAsync(It.IsAny<Jogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
