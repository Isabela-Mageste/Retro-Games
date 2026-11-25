using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PedidoRepository : RepositoryBase, IPedidoRepository
{
    private const string Colunas = "co_seq_pedido, dt_pedido, vl_total, co_usuario, co_situacao_pedido";

    public PedidoRepository(RepositoryConfig configuracao) : base(configuracao) { }

    public async Task<int> InserirAsync(Pedido pedido, CancellationToken cancelamento = default)
    {
        const string sql = "INSERT INTO tb_pedido (dt_pedido, vl_total, co_usuario, co_situacao_pedido) VALUES (@dataPedido, @total, @idUsuario, @idSituacao)";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@dataPedido", pedido.DataPedido);
        AdicionarParametro(comando, "@total", pedido.Total);
        AdicionarParametro(comando, "@idUsuario", pedido.IdUsuario);
        AdicionarParametro(comando, "@idSituacao", pedido.IdSituacaoPedido);
        await ExecutarGravacaoAsync(comando, cancelamento);
        return checked((int)comando.LastInsertedId);
    }

    // Do mais recente para o mais antigo (C05).
    public async Task<IReadOnlyList<Pedido>> ListarPorUsuarioAsync(int idUsuario, CancellationToken cancelamento = default)
    {
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao,
            "SELECT " + Colunas + " FROM tb_pedido WHERE co_usuario = @idUsuario ORDER BY dt_pedido DESC, co_seq_pedido DESC");
        AdicionarParametro(comando, "@idUsuario", idUsuario);
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        var pedidos = new List<Pedido>();
        while (await leitor.ReadAsync(cancelamento))
            pedidos.Add(Mapear(leitor));
        return pedidos;
    }

    private static Pedido Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_seq_pedido"),
        DataPedido = DateTime.SpecifyKind(leitor.GetDateTime("dt_pedido"), DateTimeKind.Utc),
        Total = leitor.GetDecimal("vl_total"),
        IdUsuario = leitor.GetInt32("co_usuario"),
        IdSituacaoPedido = leitor.GetInt32("co_situacao_pedido")
    };
}
