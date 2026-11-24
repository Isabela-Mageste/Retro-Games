using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

// Lê o usuário da requisição atual (claims do cookie de sessão).
public class UsuarioAtualService(IHttpContextAccessor httpContextAccessor) : IUsuarioAtualService
{
    public bool EstaAutenticado => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public int? IdUsuario => EstaAutenticado ? httpContextAccessor.HttpContext!.User.ObterIdUsuario() : null;

    public bool EhAdministrador => EstaAutenticado && httpContextAccessor.HttpContext!.User.IsInRole(Perfis.Administrador);
}
