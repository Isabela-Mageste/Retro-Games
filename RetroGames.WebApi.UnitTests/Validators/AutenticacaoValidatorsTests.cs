using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Validators;

public class AutenticacaoValidatorsTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Login_DadosValidos_NaoTemErros()
    {
        var resultado = _validator.Validate(new LoginRequestApiModel { Email = "admin@retrogames.local", Senha = "x" });

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public void Login_EmailMalFormado_TemErroNoCampoEmail()
    {
        var resultado = _validator.Validate(new LoginRequestApiModel { Email = "abc", Senha = "x" });

        var erro = Assert.Single(resultado.Errors);
        Assert.Equal("Email", erro.PropertyName);
        Assert.Equal("Informe um e-mail válido.", erro.ErrorMessage);
    }

    [Fact]
    public void Login_SenhaVazia_TemErroNoCampoSenha()
    {
        var resultado = _validator.Validate(new LoginRequestApiModel { Email = "admin@retrogames.local", Senha = "" });

        var erro = Assert.Single(resultado.Errors);
        Assert.Equal("Senha", erro.PropertyName);
    }
}
