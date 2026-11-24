using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

// PlataformaService com o repositório substituído por mock (RN01 e RN02).
public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    public PlataformaServiceTests()
    {
        _repositorio.Setup(r => r.Plataforma).Returns(_plataformas.Object);
    }

    private PlataformaService CriarService() => new(_repositorio.Object);

    private void ConfigurarExistente(int id) =>
        _plataformas.Setup(r => r.BuscarPorIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Plataforma { Id = id, Nome = "NES", Fabricante = "Nintendo", AnoLancamento = 1983 });

    [Fact]
    public async Task BuscarPorId_Inexistente_LancaPlataformaNaoEncontrada()
    {
        // Arrange
        _plataformas.Setup(r => r.BuscarPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Plataforma?)null);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => CriarService().BuscarPorIdAsync(99));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
    }

    [Fact]
    public async Task Inserir_NomeJaExiste_LancaNomeDuplicado()
    {
        _plataformas.Setup(r => r.ExisteComNomeAsync("NES", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().InserirAsync(new Plataforma { Nome = "NES" }));

        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, erro.Erro);
        _plataformas.Verify(r => r.InserirAsync(It.IsAny<Plataforma>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_BancoRecusaPorDuplicidade_LancaNomeDuplicado()
    {
        // Arrange: a conferência passa, mas o banco recusa o INSERT (erro 1062)
        _plataformas.Setup(r => r.ExisteComNomeAsync("SNES", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _plataformas.Setup(r => r.InserirAsync(It.IsAny<Plataforma>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RegistroDuplicadoException(new Exception("Duplicate entry")));

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().InserirAsync(new Plataforma { Nome = "SNES" }));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, erro.Erro);
    }

    [Fact]
    public async Task Inserir_NomeComEspacos_GravaSemEspacosEDevolveIdGerado()
    {
        _plataformas.Setup(r => r.ExisteComNomeAsync("Atari 2600", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _plataformas.Setup(r => r.InserirAsync(It.IsAny<Plataforma>(), It.IsAny<CancellationToken>())).ReturnsAsync(8);

        var criada = await CriarService().InserirAsync(new Plataforma { Nome = "  Atari 2600 " });

        Assert.Equal(8, criada.Id);
        Assert.Equal("Atari 2600", criada.Nome);
    }

    [Fact]
    public async Task Alterar_MantendoOProprioNome_NaoTrataComoDuplicidade()
    {
        ConfigurarExistente(1);
        _plataformas.Setup(r => r.ExisteComNomeAsync("NES", 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await CriarService().AlterarAsync(new Plataforma { Id = 1, Nome = "NES", Fabricante = "Nintendo", AnoLancamento = 1983 });

        _plataformas.Verify(r => r.AlterarAsync(It.Is<Plataforma>(p => p.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Alterar_Inexistente_LancaPlataformaNaoEncontrada()
    {
        _plataformas.Setup(r => r.BuscarPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Plataforma?)null);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().AlterarAsync(new Plataforma { Id = 99, Nome = "X" }));

        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
    }

    [Fact]
    public async Task Excluir_PlataformaComJogos_LancaPossuiJogos()
    {
        ConfigurarExistente(1);
        _plataformas.Setup(r => r.PossuiJogosAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => CriarService().ExcluirAsync(1));

        Assert.Equal(BusinessErrors.PLATAFORMA_POSSUI_JOGOS, erro.Erro);
        _plataformas.Verify(r => r.ExcluirAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Excluir_PlataformaSemJogos_Exclui()
    {
        ConfigurarExistente(7);
        _plataformas.Setup(r => r.PossuiJogosAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await CriarService().ExcluirAsync(7);

        _plataformas.Verify(r => r.ExcluirAsync(7, It.IsAny<CancellationToken>()), Times.Once);
    }
}
