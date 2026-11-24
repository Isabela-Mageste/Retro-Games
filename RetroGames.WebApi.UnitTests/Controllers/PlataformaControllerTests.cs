using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Controllers;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Controllers;

// PlataformaController com o service substituído por mock: confere status e envelope.
public class PlataformaControllerTests
{
    private readonly Mock<IPlataformaService> _service = new();

    private PlataformaController CriarController() => new(_service.Object);

    [Fact]
    public async Task Listar_DevolveOkComAsPlataformasNoEnvelope()
    {
        _service.Setup(s => s.ListarAsync()).ReturnsAsync(
            [new Plataforma { Id = 1, Nome = "NES", Fabricante = "Nintendo", AnoLancamento = 1983 }]);

        var resposta = await CriarController().Listar();

        var ok = Assert.IsType<OkObjectResult>(resposta.Result);
        var envelope = Assert.IsType<RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>>(ok.Value);
        Assert.Equal(ResponseCodes.SUCESSO, envelope.ResponseCode);
        Assert.Equal("NES", Assert.Single(envelope.Data!).Nome);
    }

    [Fact]
    public async Task Inserir_Devolve201ApontandoParaONovoRecurso()
    {
        _service.Setup(s => s.InserirAsync(It.IsAny<Plataforma>())).ReturnsAsync(
            new Plataforma { Id = 12, Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1990 });
        var request = new InserirPlataformaRequestApiModel { Nome = "Super Nintendo", Fabricante = "Nintendo", AnoLancamento = 1990 };

        var resposta = await CriarController().Inserir(request);

        var criado = Assert.IsType<CreatedAtActionResult>(resposta.Result);
        Assert.Equal(nameof(PlataformaController.ListarPorId), criado.ActionName);
        Assert.Equal(12, criado.RouteValues!["id"]);
        var envelope = Assert.IsType<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>(criado.Value);
        Assert.Equal(12, envelope.Data!.Id);
    }

    [Fact]
    public async Task Alterar_UsaOIdDaRota()
    {
        _service.Setup(s => s.AlterarAsync(It.IsAny<Plataforma>())).ReturnsAsync((Plataforma p) => p);
        var request = new AlterarPlataformaRequestApiModel { Nome = "Super Famicom", Fabricante = "Nintendo", AnoLancamento = 1990 };

        var resposta = await CriarController().Alterar(3, request);

        Assert.IsType<OkObjectResult>(resposta.Result);
        _service.Verify(s => s.AlterarAsync(It.Is<Plataforma>(p => p.Id == 3 && p.Nome == "Super Famicom")), Times.Once);
    }

    [Fact]
    public async Task Excluir_DevolveOkComEnvelopeDeSucesso()
    {
        var resposta = await CriarController().Excluir(7);

        var ok = Assert.IsType<OkObjectResult>(resposta.Result);
        var envelope = Assert.IsType<RetroGamesResponseApiModel>(ok.Value);
        Assert.Equal(ResponseCodes.SUCESSO, envelope.ResponseCode);
        _service.Verify(s => s.ExcluirAsync(7), Times.Once);
    }
}
