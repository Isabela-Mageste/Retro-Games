using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig configuracao;

    protected RepositoryBase(RepositoryConfig configuracao)
    {
        this.configuracao = configuracao;
    }

    protected async Task<MySqlConnection> AbrirConexaoAsync(CancellationToken cancelamento = default)
    {
        var conexao = new MySqlConnection(configuracao.ConnectionString);
        try
        {
            await conexao.OpenAsync(cancelamento);
            return conexao;
        }
        catch
        {
            // O chamador só assume a posse depois que OpenAsync termina.
            await conexao.DisposeAsync();
            throw;
        }
    }

    protected MySqlCommand CriarComando(MySqlConnection conexao, string sql)
    {
        return new MySqlCommand(sql, conexao);
    }

    protected void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    protected async Task<int> ExecutarGravacaoAsync(MySqlCommand comando, CancellationToken cancelamento = default)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync(cancelamento);
        }
        catch (MySqlException excecao) when (excecao.Number == 1062)
        {
            throw new RegistroDuplicadoException(excecao);
        }
    }
}
