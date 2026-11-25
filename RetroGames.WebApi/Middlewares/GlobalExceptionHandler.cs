using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

// Ponto único que transforma exceções não tratadas em respostas com o envelope.
// BusinessRuleException → status, código e mensagem do erro do catálogo (400, 404 ou 409).
// Qualquer outra → 500 com mensagem genérica; os detalhes vão só para o log.
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public const string MensagemErroInesperado = "Ocorreu um erro inesperado. Tente novamente mais tarde.";

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is BusinessRuleException erroNegocio)
        {
            httpContext.Response.StatusCode = (int)erroNegocio.Erro.Status;
            await httpContext.Response.WriteAsJsonAsync(
                new RetroGamesResponseApiModel(erroNegocio.Erro.Code, erroNegocio.Message),
                cancellationToken);
            return true;
        }

        logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}",
            httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            new RetroGamesResponseApiModel(ResponseCodes.ERRO, MensagemErroInesperado),
            cancellationToken);
        return true;
    }
}
