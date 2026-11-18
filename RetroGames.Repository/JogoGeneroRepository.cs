using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig configuracao) : base(configuracao) { }

    public async Task<IReadOnlyList<JogoGenero>> ListarTodosAsync(CancellationToken cancelamento = default)
    {
        const string sql = "SELECT co_jogo, co_genero FROM tb_jogo_genero ORDER BY co_jogo, co_genero";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        return await LerVinculosAsync(comando, cancelamento);
    }

    public async Task<IReadOnlyList<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos, CancellationToken cancelamento = default)
    {
        cancelamento.ThrowIfCancellationRequested();
        var ids = idsJogos.ToArray();
        if (ids.Length == 0)
            return Array.Empty<JogoGenero>();
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, string.Empty);
        var parametros = new List<string>();
        for (var indice = 0; indice < ids.Length; indice++)
        {
            var parametro = "@id" + indice;
            parametros.Add(parametro);
            AdicionarParametro(comando, parametro, ids[indice]);
        }
        comando.CommandText = "SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN (" + string.Join(", ", parametros) + ") ORDER BY co_jogo, co_genero";
        return await LerVinculosAsync(comando, cancelamento);
    }

    public async Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros, CancellationToken cancelamento = default)
    {
        cancelamento.ThrowIfCancellationRequested();
        var ids = idsGeneros.ToArray();
        if (ids.Length == 0)
            return;
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, string.Empty);
        AdicionarParametro(comando, "@idJogo", idJogo);
        var valores = new List<string>();
        for (var indice = 0; indice < ids.Length; indice++)
        {
            var parametro = "@genero" + indice;
            valores.Add("(@idJogo, " + parametro + ")");
            AdicionarParametro(comando, parametro, ids[indice]);
        }
        comando.CommandText = "INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES " + string.Join(", ", valores);
        await ExecutarGravacaoAsync(comando, cancelamento);
    }

    public async Task RemoverPorJogoAsync(int idJogo, CancellationToken cancelamento = default)
    {
        const string sql = "DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idJogo", idJogo);
        await comando.ExecuteNonQueryAsync(cancelamento);
    }

    private static async Task<IReadOnlyList<JogoGenero>> LerVinculosAsync(MySqlCommand comando, CancellationToken cancelamento)
    {
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        var vinculos = new List<JogoGenero>();
        while (await leitor.ReadAsync(cancelamento))
        {
            vinculos.Add(new JogoGenero
            {
                IdJogo = leitor.GetInt32("co_jogo"),
                IdGenero = leitor.GetInt32("co_genero")
            });
        }
        return vinculos;
    }
}
