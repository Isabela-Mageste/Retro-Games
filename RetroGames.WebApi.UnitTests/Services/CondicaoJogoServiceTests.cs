using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    private readonly Mock<ICondicaoJogoRepository> _condicoes = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    public CondicaoJogoServiceTests()
    {
        _repositorio.Setup(r => r.CondicaoJogo).Returns(_condicoes.Object);
    }

    private CondicaoJogoService CriarService() => new(_repositorio.Object);

    [Fact]
    public async Task Listar_RepositorioComCondicoes_DevolveAsCondicoesDoRepositorio()
    {
        // Arrange
        IReadOnlyList<CondicaoJogo> condicoes =
        [
            new CondicaoJogo { Id = 1, Descricao = "Novo" },
            new CondicaoJogo { Id = 2, Descricao = "Usado" },
            new CondicaoJogo { Id = 3, Descricao = "Recondicionado" },
        ];
        _condicoes.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(condicoes);

        // Act
        var resultado = await CriarService().ListarAsync();

        // Assert
        Assert.Equal(3, resultado.Count);
        Assert.Equal("Recondicionado", resultado[2].Descricao);
    }
}
