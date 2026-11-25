using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IAutenticacaoService
{
    // Devolve o usuário, ou nulo se o e-mail não existe, a senha não confere ou o usuário está inativo.
    Task<Usuario?> AutenticarAsync(string email, string senha);

    // Cria o administrador inicial se ainda não houver nenhum administrador.
    Task GarantirAdministradorInicialAsync(string email, string senha);

    // C01: cria um usuário ativo com perfil Cliente e a senha em hash. E-mail já usado: 106.
    Task<Usuario> CadastrarClienteAsync(string nome, string email, string senha);
}
