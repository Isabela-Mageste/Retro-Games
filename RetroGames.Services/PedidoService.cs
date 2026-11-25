using System.Transactions;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

// Desafio, Módulo de Compras (C04 e C05). O usuário vem do IUsuarioAtualService (cookie de
// sessão), nunca do corpo da requisição.
public class PedidoService(IRepositorySet repositorio, IUsuarioAtualService usuarioAtual) : IPedidoService
{
    public async Task<PedidoServiceModel> FinalizarAsync(IReadOnlyCollection<ItemPedido> itens)
    {
        var idUsuario = ObterIdUsuario();

        // Só IdJogo e Quantidade vêm do cliente; o resto é preenchido aqui.
        var itensPedido = itens
            .Select(item => new ItemPedido { IdJogo = item.IdJogo, Quantidade = item.Quantidade })
            .ToList();

        var idsJogos = itensPedido.Select(item => item.IdJogo).Distinct().ToList();
        var jogos = await repositorio.Jogo.ListarPorIdsAsync(idsJogos);

        // Jogo inexistente ou inativo não pode ser vendido (110).
        var algumIndisponivel = idsJogos.Any(id => !jogos.Any(jogo => jogo.Id == id && jogo.Ativo));
        if (algumIndisponivel)
            throw new BusinessRuleException(BusinessErrors.JOGO_INDISPONIVEL);

        // Estoque conferido antes de gravar (111), com o nome do jogo na mensagem.
        foreach (var item in itensPedido)
        {
            var jogo = jogos.First(atual => atual.Id == item.IdJogo);
            if (jogo.Estoque < item.Quantidade)
                throw EstoqueInsuficiente(jogo.Nome);

            // O preço vem do banco, nunca do front.
            item.PrecoUnitario = jogo.Preco;
        }

        var pedido = new Pedido
        {
            DataPedido = DateTime.UtcNow,
            Total = itensPedido.Sum(item => item.PrecoUnitario * item.Quantidade),
            IdUsuario = idUsuario,
            IdSituacaoPedido = SituacaoPedidoIds.Pendente,
        };

        // Pedido, itens e baixa de estoque: tudo ou nada.
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            pedido.Id = await repositorio.Pedido.InserirAsync(pedido);
            itensPedido.ForEach(item => item.IdPedido = pedido.Id);
            await repositorio.ItemPedido.InserirAsync(itensPedido);

            foreach (var item in itensPedido)
            {
                // Outra compra pode ter levado o estoque entre a conferência e aqui: sem Complete(),
                // o pedido e os itens já gravados são desfeitos.
                if (!await repositorio.Jogo.BaixarEstoqueAsync(item.IdJogo, item.Quantidade))
                {
                    throw EstoqueInsuficiente(jogos.First(jogo => jogo.Id == item.IdJogo).Nome);
                }
            }

            transactionScope.Complete();
        }

        return (await MontarPedidosAsync([pedido]))[0];
    }

    public async Task<List<PedidoServiceModel>> ListarMeusAsync()
    {
        var pedidos = await repositorio.Pedido.ListarPorUsuarioAsync(ObterIdUsuario());
        return await MontarPedidosAsync(pedidos);
    }

    // "Estoque insuficiente para Tetris." (sem ponto duplo em nomes como "Super Mario Bros.").
    private static BusinessRuleException EstoqueInsuficiente(string nomeJogo) =>
        new(BusinessErrors.ESTOQUE_INSUFICIENTE, $"Estoque insuficiente para {nomeJogo.TrimEnd('.')}.");

    private int ObterIdUsuario() =>
        usuarioAtual.IdUsuario ?? throw new InvalidOperationException("O pedido exige um usuário autenticado.");

    // Monta os pedidos sempre com três consultas (itens, jogos e situações), qualquer que seja a
    // quantidade de pedidos, e combina tudo com LINQ, mantendo a ordem dos pedidos recebidos.
    private async Task<List<PedidoServiceModel>> MontarPedidosAsync(IReadOnlyList<Pedido> pedidos)
    {
        if (pedidos.Count == 0)
            return [];

        var itens = await repositorio.ItemPedido.ListarPorPedidosAsync(pedidos.Select(pedido => pedido.Id).ToList());
        var jogos = await repositorio.Jogo.ListarPorIdsAsync(itens.Select(item => item.IdJogo).Distinct().ToList());
        var situacoes = await repositorio.SituacaoPedido.ListarAsync();

        var resultado = from pedido in pedidos
                        join situacao in situacoes on pedido.IdSituacaoPedido equals situacao.Id
                        select new PedidoServiceModel
                        {
                            Pedido = pedido,
                            DescricaoSituacao = situacao.Descricao,
                            Itens = (from item in itens
                                     where item.IdPedido == pedido.Id
                                     join jogo in jogos on item.IdJogo equals jogo.Id
                                     select new ItemPedidoServiceModel { Item = item, NomeJogo = jogo.Nome }).ToList(),
                        };

        return resultado.ToList();
    }
}
