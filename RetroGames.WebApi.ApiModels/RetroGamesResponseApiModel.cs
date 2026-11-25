using System.Text.Json.Serialization;

namespace RetroGames.WebApi.ApiModels;

// Envelope de toda resposta da API (Espec. 5.3): { responseCode, data, errorMessage }.
// Esta versão, sem Data, é usada nas respostas de erro e nas que não devolvem registro.
public class RetroGamesResponseApiModel
{
    [JsonPropertyOrder(0)]
    public int ResponseCode { get; set; } = ResponseCodes.SUCESSO;

    [JsonPropertyOrder(2)]
    public string? ErrorMessage { get; set; }

    public RetroGamesResponseApiModel()
    {
    }

    public RetroGamesResponseApiModel(int responseCode, string? errorMessage)
    {
        ResponseCode = responseCode;
        ErrorMessage = errorMessage;
    }
}

// Envelope com o conteúdo da resposta em Data.
public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    [JsonPropertyOrder(1)]
    public T? Data { get; set; }

    public RetroGamesResponseApiModel()
    {
    }

    public RetroGamesResponseApiModel(T data)
    {
        Data = data;
    }

    public RetroGamesResponseApiModel(T data, int responseCode, string? errorMessage)
        : base(responseCode, errorMessage)
    {
        Data = data;
    }
}
