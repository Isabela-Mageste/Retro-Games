using MySqlConnector;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    private const string Colunas = "co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil";

    public UsuarioRepository(RepositoryConfig configuracao) : base(configuracao) { }

    public async Task<Usuario?> BuscarPorEmailAsync(string email, CancellationToken cancelamento = default)
    {
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, "SELECT " + Colunas + " FROM tb_usuario WHERE ds_email = @email");
        AdicionarParametro(comando, "@email", email);
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        return await leitor.ReadAsync(cancelamento) ? Mapear(leitor) : null;
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id, CancellationToken cancelamento = default)
    {
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, "SELECT " + Colunas + " FROM tb_usuario WHERE co_seq_usuario = @id");
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        return await leitor.ReadAsync(cancelamento) ? Mapear(leitor) : null;
    }

    public async Task<bool> ExisteAdministradorAsync(CancellationToken cancelamento = default)
    {
        const string sql = "SELECT 1 FROM tb_usuario WHERE co_perfil = @idPerfil LIMIT 1";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@idPerfil", PerfilIds.Administrador);
        return await comando.ExecuteScalarAsync(cancelamento) is not null;
    }

    public async Task<int> InserirAsync(Usuario usuario, CancellationToken cancelamento = default)
    {
        const string sql = "INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil) VALUES (@nome, @email, @senhaHash, @ativo, @dataCadastro, @idPerfil)";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@senhaHash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@dataCadastro", usuario.DataCadastro);
        AdicionarParametro(comando, "@idPerfil", usuario.IdPerfil);
        // ds_email é UNIQUE: o erro 1062 vira RegistroDuplicadoException.
        await ExecutarGravacaoAsync(comando, cancelamento);
        return checked((int)comando.LastInsertedId);
    }

    private static Usuario Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_seq_usuario"),
        Nome = leitor.GetString("no_usuario"),
        Email = leitor.GetString("ds_email"),
        SenhaHash = leitor.GetString("ds_senha_hash"),
        Ativo = leitor.GetBoolean("st_ativo"),
        DataCadastro = DateTime.SpecifyKind(leitor.GetDateTime("dt_cadastro"), DateTimeKind.Utc),
        IdPerfil = leitor.GetInt32("co_perfil")
    };
}
