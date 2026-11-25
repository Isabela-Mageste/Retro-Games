using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Validators;

// Validators de jogo: RN06 (valores) e a parte de formato da RN05 (ao menos um gênero).
public class JogoValidatorsTests
{
    private readonly InserirJogoRequestValidator _inserir = new();
    private readonly AlterarJogoRequestValidator _alterar = new();
    private readonly ListarJogoRequestValidator _listar = new();

    private static InserirJogoRequestApiModel JogoValido() => new()
    {
        Nome = "Contra", AnoLancamento = 1988, Preco = 79.90m, Estoque = 0,
        IdPlataforma = 1, IdCondicaoJogo = 2, IdsGeneros = [1],
    };

    [Fact]
    public void Inserir_DadosValidos_NaoTemErros()
    {
        Assert.True(_inserir.Validate(JogoValido()).IsValid);
    }

    [Fact]
    public void Inserir_PrecoZero_TemErroNoCampoPreco()
    {
        var request = JogoValido();
        request.Preco = 0;

        var erro = Assert.Single(_inserir.Validate(request).Errors);

        Assert.Equal("Preco", erro.PropertyName);
        Assert.Equal("O preço deve ser maior que zero.", erro.ErrorMessage);
    }

    [Fact]
    public void Inserir_EstoqueNegativo_TemErroNoCampoEstoque()
    {
        var request = JogoValido();
        request.Estoque = -1;

        var erro = Assert.Single(_inserir.Validate(request).Errors);

        Assert.Equal("Estoque", erro.PropertyName);
    }

    [Fact]
    public void Inserir_SemGeneros_TemErroNoCampoIdsGeneros()
    {
        var request = JogoValido();
        request.IdsGeneros = [];

        var erro = Assert.Single(_inserir.Validate(request).Errors);

        Assert.Equal("IdsGeneros", erro.PropertyName);
        Assert.Equal("Informe ao menos um gênero.", erro.ErrorMessage);
    }

    [Fact]
    public void Inserir_PrecoComTresCasasDecimais_TemErroNoCampoPreco()
    {
        var request = JogoValido();
        request.Preco = 10.999m;

        var erro = Assert.Single(_inserir.Validate(request).Errors);

        Assert.Equal("Preco", erro.PropertyName);
    }

    [Fact]
    public void Alterar_AnoForaDaFaixa_TemErroNoAno()
    {
        var resultado = _alterar.Validate(new AlterarJogoRequestApiModel
        {
            Nome = "Contra", AnoLancamento = 1800, Preco = 10, Estoque = 1,
            IdPlataforma = 1, IdCondicaoJogo = 2, IdsGeneros = [1],
        });

        var erro = Assert.Single(resultado.Errors);
        Assert.Equal("AnoLancamento", erro.PropertyName);
    }

    [Fact]
    public void Listar_PaginaZero_TemErroNoCampoPagina()
    {
        var erro = Assert.Single(_listar.Validate(new ListarJogoRequestApiModel { Pagina = 0 }).Errors);

        Assert.Equal("Pagina", erro.PropertyName);
    }
}
