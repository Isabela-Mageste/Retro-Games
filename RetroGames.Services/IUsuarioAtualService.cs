namespace RetroGames.Services;

// Quem está chamando o Service. O Service não conhece HTTP: a WebApi implementa esta
// interface lendo o cookie de sessão. O usuário nunca vem do corpo da requisição.
public interface IUsuarioAtualService
{
    bool EstaAutenticado { get; }
    int? IdUsuario { get; }
    bool EhAdministrador { get; }
}
