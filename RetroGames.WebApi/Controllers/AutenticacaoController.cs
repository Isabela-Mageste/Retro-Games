using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController(IAutenticacaoService autenticacaoService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> Login(
        LoginRequestApiModel request)
    {
        var usuario = await autenticacaoService.AutenticarAsync(request.Email, request.Senha);
        if (usuario is null)
        {
            return Unauthorized(new RetroGamesResponseApiModel(
                ResponseCodes.CREDENCIAIS_INVALIDAS.Code, ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage));
        }

        // Cria a sessão: a API devolve o cookie retrogames.sessao com as claims criptografadas.
        var principal = usuario.ParaClaimsPrincipal();
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>(principal.ParaUsuarioLogado()));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new RetroGamesResponseApiModel());
    }

    [HttpGet("eu")]
    [Authorize]
    public ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>> Eu()
    {
        return Ok(new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>(User.ParaUsuarioLogado()));
    }
}
