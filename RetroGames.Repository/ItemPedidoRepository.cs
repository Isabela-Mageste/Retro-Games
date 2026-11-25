using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class ItemPedidoRepository : RepositoryBase, IItemPedidoRepository
{
    public ItemPedidoRepository(RepositoryConfig configuracao) : base(configuracao) { }

    // Grava todos os itens num único INSERT. A tabela tem UNIQUE (co_pedido, co_jogo):
    // o erro 1062 vira RegistroDuplicadoException no ExecutarGravacaoAsync.
    public async Task InserirAsync(IReadOnlyCollection<ItemPedido> itens, CancellationToken cancelamento = default)
    {
        cancelamento.ThrowIfCancellationRequested();
        var lista = itens.ToArray();
        if (lista.Length == 0)
            return;
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, string.Empty);
        var valores = new List<string>();
        for (var indice = 0; indice < lista.Length; indice++)
        {
            valores.Add($"(@quantidade{indice}, @preco{indice}, @idPedido{indice}, @idJogo{indice})");
            AdicionarParametro(comando, "@quantidade" + indice, lista[indice].Quantidade);
            AdicionarParametro(comando, "@preco" + indice, lista[indice].PrecoUnitario);
            AdicionarParametro(comando, "@idPedido" + indice, lista[indice].IdPedido);
            AdicionarParametro(comando, "@idJogo" + indice, lista[indice].IdJogo);
        }
        // Somente trechos fixos e nomes de parâmetros entram na montagem do SQL.
        comando.CommandText = "INSERT INTO tb_item_pedido (qt_item, vl_unitario, co_pedido, co_jogo) VALUES " + string.Join(", ", valores);
        await ExecutarGravacaoAsync(comando, cancelamento);
    }

    // Os itens de uma lista de pedidos, numa única consulta.
    public async Task<IReadOnlyList<ItemPedido>> ListarPorPedidosAsync(IReadOnlyCollection<int> idsPedidos, CancellationToken cancelamento = default)
    {
        cancelamento.ThrowIfCancellationRequested();
        var ids = idsPedidos.ToArray();
        if (ids.Length == 0)
            return Array.Empty<ItemPedido>();
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, string.Empty);
        var parametros = new List<string>();
        for (var indice = 0; indice < ids.Length; indice++)
        {
            parametros.Add("@id" + indice);
            AdicionarParametro(comando, "@id" + indice, ids[indice]);
        }
        comando.CommandText = "SELECT co_seq_item_pedido, qt_item, vl_unitario, co_pedido, co_jogo FROM tb_item_pedido WHERE co_pedido IN ("
            + string.Join(", ", parametros) + ") ORDER BY co_pedido, co_seq_item_pedido";
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        var itens = new List<ItemPedido>();
        while (await leitor.ReadAsync(cancelamento))
        {
            itens.Add(new ItemPedido
            {
                Id = leitor.GetInt32("co_seq_item_pedido"),
                Quantidade = leitor.GetInt32("qt_item"),
                PrecoUnitario = leitor.GetDecimal("vl_unitario"),
                IdPedido = leitor.GetInt32("co_pedido"),
                IdJogo = leitor.GetInt32("co_jogo")
            });
        }
        return itens;
    }
}
