using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class SituacaoPedidoRepository : RepositoryBase, ISituacaoPedidoRepository
{
    public SituacaoPedidoRepository(RepositoryConfig configuracao) : base(configuracao) { }

    public async Task<IReadOnlyList<SituacaoPedido>> ListarAsync(CancellationToken cancelamento = default)
    {
        const string sql = "SELECT co_situacao_pedido, ds_situacao_pedido FROM tb_situacao_pedido ORDER BY co_situacao_pedido";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        var situacoes = new List<SituacaoPedido>();
        while (await leitor.ReadAsync(cancelamento))
        {
            situacoes.Add(new SituacaoPedido
            {
                Id = leitor.GetInt32("co_situacao_pedido"),
                Descricao = leitor.GetString("ds_situacao_pedido")
            });
        }
        return situacoes;
    }
}
