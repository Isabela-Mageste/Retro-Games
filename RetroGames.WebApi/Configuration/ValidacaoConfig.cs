using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.Filters;

namespace RetroGames.WebApi.Configuration;

public static class ValidacaoConfig
{
    public const string MensagemFormatoInvalido = "Valor em formato inválido.";

    // Com [ApiController], erros de model binding (JSON malformado, texto num campo
    // numérico, corpo vazio) param antes da action. Esta fábrica troca a resposta padrão
    // do ASP.NET Core pelo envelope: 400 com o código 3 (ou 2, se o corpo veio vazio).
    public static void ConfigurarRespostaDeModeloInvalido(ApiBehaviorOptions opcoes)
    {
        opcoes.InvalidModelStateResponseFactory = contexto =>
        {
            var esperaCorpo = contexto.ActionDescriptor.Parameters
                .Any(parametro => parametro.BindingInfo?.BindingSource == BindingSource.Body);
            if (esperaCorpo && contexto.HttpContext.Request.ContentLength is null or 0)
                return RespostasDeValidacao.ModeloNulo();

            var nomesParametros = contexto.ActionDescriptor.Parameters.Select(parametro => parametro.Name).ToHashSet();

            // As mensagens técnicas do conversor JSON não vão para o cliente.
            var errosPorCampo = contexto.ModelState
                .Where(entrada => entrada.Value is { Errors.Count: > 0 })
                .Select(entrada =>
                {
                    var campo = RespostasDeValidacao.NomeDoCampo(entrada.Key);
                    return string.IsNullOrEmpty(campo) || nomesParametros.Contains(entrada.Key) ? "corpo" : campo;
                })
                .Distinct()
                .ToDictionary(campo => campo, _ => new[] { MensagemFormatoInvalido });

            return RespostasDeValidacao.ModeloInvalido(errosPorCampo);
        };
    }
}
