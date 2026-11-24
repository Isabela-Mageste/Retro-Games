using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

// Sessão em cookie (RS01): HttpOnly, SameSite=Strict e expiração deslizante de 2 horas.
public static class AutenticacaoConfig
{
    public const string NomeCookie = "retrogames.sessao";

    public static IServiceCollection AddAutenticacaoPorCookie(this IServiceCollection services, IWebHostEnvironment ambiente)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = NomeCookie;
                opcoes.Cookie.HttpOnly = true;                  // o JavaScript não lê o cookie
                opcoes.Cookie.SameSite = SameSiteMode.Strict;   // não vai em requisições de outros sites (CSRF)
                opcoes.Cookie.SecurePolicy = ambiente.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest          // em desenvolvimento a API roda em HTTP
                    : CookieSecurePolicy.Always;                // em produção, só por HTTPS
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;                // renova enquanto há uso

                // Uma API não redireciona para uma página de login: responde 401/403 no envelope.
                opcoes.Events.OnRedirectToLogin = contexto => EscreverEnvelopeAsync(
                    contexto.Response, StatusCodes.Status401Unauthorized, ResponseCodes.NAO_AUTENTICADO);
                opcoes.Events.OnRedirectToAccessDenied = contexto => EscreverEnvelopeAsync(
                    contexto.Response, StatusCodes.Status403Forbidden, ResponseCodes.ACESSO_NEGADO);
            });

        return services;
    }

    private static Task EscreverEnvelopeAsync(HttpResponse resposta, int status, (int Code, string DefaultMessage) codigo)
    {
        resposta.StatusCode = status;
        return resposta.WriteAsJsonAsync(new RetroGamesResponseApiModel(codigo.Code, codigo.DefaultMessage));
    }
}
