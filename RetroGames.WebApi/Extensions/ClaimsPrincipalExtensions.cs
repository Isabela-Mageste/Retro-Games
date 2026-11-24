using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Extensions;

// Claims que viajam, criptografadas, dentro do cookie de sessão: id, nome, e-mail e perfil.
// Nunca o hash da senha.
public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal ParaClaimsPrincipal(this Usuario usuario)
    {
        var perfil = usuario.IdPerfil == PerfilIds.Administrador ? Perfis.Administrador : Perfis.Cliente;
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, perfil),
        };
        var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identidade);
    }

    // Id do usuário logado, ou nulo se não houver sessão.
    public static int? ObterIdUsuario(this ClaimsPrincipal principal)
    {
        var valor = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var id) ? id : null;
    }

    public static UsuarioLogadoResponseApiModel ParaUsuarioLogado(this ClaimsPrincipal principal) => new()
    {
        Id = principal.ObterIdUsuario() ?? 0,
        Nome = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
        Email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
        Perfil = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
    };
}
