using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Filters;

// Monta as respostas 400 de entrada inválida, sempre no envelope.
public static class RespostasDeValidacao
{
    // HTTP 400, responseCode 3 e os erros indexados por campo em data.
    public static BadRequestObjectResult ModeloInvalido(Dictionary<string, string[]> errosPorCampo)
    {
        return new BadRequestObjectResult(new RetroGamesResponseApiModel<Dictionary<string, string[]>>(
            errosPorCampo,
            ResponseCodes.MODEL_STATE_INVALIDO.Code,
            ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage));
    }

    // HTTP 400, responseCode 2: a requisição chegou sem o corpo esperado.
    public static BadRequestObjectResult ModeloNulo()
    {
        return new BadRequestObjectResult(new RetroGamesResponseApiModel(
            ResponseCodes.MODEL_NULL.Code,
            ResponseCodes.MODEL_NULL.DefaultMessage));
    }

    // "AnoLancamento" e "$.anoLancamento" viram "anoLancamento", como no JSON.
    public static string NomeDoCampo(string nome)
    {
        var semPrefixo = nome.StartsWith("$.") ? nome[2..] : nome.TrimStart('$');
        return JsonNamingPolicy.CamelCase.ConvertName(semPrefixo);
    }
}
