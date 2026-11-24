using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Validators;

// Validators de plataforma: entra um objeto, sai a lista de erros (sem mock).
public class PlataformaValidatorsTests
{
    private readonly InserirPlataformaRequestValidator _inserir = new();
    private readonly AlterarPlataformaRequestValidator _alterar = new();

    [Fact]
    public void Inserir_DadosValidos_NaoTemErros()
    {
        var resultado = _inserir.Validate(new InserirPlataformaRequestApiModel
        {
            Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1990,
        });

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public void Inserir_NomeVazio_TemErroNoCampoNome()
    {
        var resultado = _inserir.Validate(new InserirPlataformaRequestApiModel
        {
            Nome = "", Fabricante = "Nintendo", AnoLancamento = 1990,
        });

        var erro = Assert.Single(resultado.Errors);
        Assert.Equal("Nome", erro.PropertyName);
        Assert.Equal("Informe o nome.", erro.ErrorMessage);
    }

    [Theory]
    [InlineData(1969)]
    [InlineData(2101)]
    public void Inserir_AnoForaDaFaixa_TemErroNoAno(int ano)
    {
        var resultado = _inserir.Validate(new InserirPlataformaRequestApiModel
        {
            Nome = "X", Fabricante = "Y", AnoLancamento = ano,
        });

        var erro = Assert.Single(resultado.Errors);
        Assert.Equal("AnoLancamento", erro.PropertyName);
        Assert.Equal("Ano de lançamento inválido.", erro.ErrorMessage);
    }

    [Fact]
    public void Alterar_FabricanteMaiorQue80_TemErroNoFabricante()
    {
        var resultado = _alterar.Validate(new AlterarPlataformaRequestApiModel
        {
            Nome = "X", Fabricante = new string('a', 81), AnoLancamento = 1990,
        });

        var erro = Assert.Single(resultado.Errors);
        Assert.Equal("Fabricante", erro.PropertyName);
    }
}
