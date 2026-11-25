using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Middlewares;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Middlewares;

// Lança cada tipo de erro e confere o status HTTP e o envelope escrito na resposta.
public class GlobalExceptionHandlerTests
{
    private static async Task<(int Status, JsonElement Corpo)> TratarAsync(Exception excecao)
    {
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        var tratada = await handler.TryHandleAsync(contexto, excecao, CancellationToken.None);

        Assert.True(tratada);
        contexto.Response.Body.Position = 0;
        var corpo = await JsonSerializer.DeserializeAsync<JsonElement>(contexto.Response.Body);
        return (contexto.Response.StatusCode, corpo);
    }

    [Fact]
    public async Task NomeDuplicado_Responde409ComCodigo101()
    {
        var (status, corpo) = await TratarAsync(new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO));

        Assert.Equal(StatusCodes.Status409Conflict, status);
        Assert.Equal(101, corpo.GetProperty("responseCode").GetInt32());
        Assert.Equal("Já existe uma plataforma com este nome.", corpo.GetProperty("errorMessage").GetString());
    }

    [Fact]
    public async Task PlataformaNaoEncontrada_Responde404ComCodigo100()
    {
        var (status, corpo) = await TratarAsync(new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA));

        Assert.Equal(StatusCodes.Status404NotFound, status);
        Assert.Equal(100, corpo.GetProperty("responseCode").GetInt32());
    }

    [Fact]
    public async Task ExcecaoInesperada_Responde500SemDetalhesInternos()
    {
        var (status, corpo) = await TratarAsync(new InvalidOperationException("SELECT * FROM tb_jogo falhou"));

        Assert.Equal(StatusCodes.Status500InternalServerError, status);
        Assert.Equal(ResponseCodes.ERRO, corpo.GetProperty("responseCode").GetInt32());
        Assert.Equal(GlobalExceptionHandler.MensagemErroInesperado, corpo.GetProperty("errorMessage").GetString());
        Assert.DoesNotContain("tb_jogo", corpo.GetRawText());
    }
}
