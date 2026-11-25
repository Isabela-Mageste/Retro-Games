using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Validators;

// C04: formato do pedido (400, código 3).
public class PedidoValidatorsTests
{
    private readonly FinalizarPedidoRequestValidator _validator = new();

    private static FinalizarPedidoRequestApiModel Pedido(params (int IdJogo, int Quantidade)[] itens) => new()
    {
        Itens = itens.Select(item => new ItemPedidoRequestApiModel { IdJogo = item.IdJogo, Quantidade = item.Quantidade }).ToList(),
    };

    [Fact]
    public void Finalizar_ItensValidos_NaoTemErros()
    {
        Assert.True(_validator.Validate(Pedido((1, 1), (7, 10))).IsValid);
    }

    [Fact]
    public void Finalizar_SemItens_TemErroNoCampoItens()
    {
        var erro = Assert.Single(_validator.Validate(Pedido()).Errors);

        Assert.Equal("Itens", erro.PropertyName);
        Assert.Equal("Adicione ao menos um jogo ao pedido.", erro.ErrorMessage);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Finalizar_QuantidadeForaDe1A10_TemErroNaQuantidade(int quantidade)
    {
        var erro = Assert.Single(_validator.Validate(Pedido((1, quantidade))).Errors);

        Assert.Equal("Itens[0].Quantidade", erro.PropertyName);
    }

    [Fact]
    public void Finalizar_JogoRepetido_TemErroNoCampoItens()
    {
        var erro = Assert.Single(_validator.Validate(Pedido((1, 1), (1, 2))).Errors);

        Assert.Equal("Itens", erro.PropertyName);
    }
}
