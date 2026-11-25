using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

// GeneroService só lê dados, mas também tem teste: assim, todo Service tem testes.
public class GeneroServiceTests
{
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    public GeneroServiceTests()
    {
        _repositorio.Setup(r => r.Genero).Returns(_generos.Object);
    }

    private GeneroService CriarService() => new(_repositorio.Object);

    [Fact]
    public async Task Listar_RepositorioComGeneros_DevolveOsGenerosDoRepositorio()
    {
        // Arrange
        IReadOnlyList<Genero> generos = [new Genero { Id = 1, Nome = "Ação" }, new Genero { Id = 4, Nome = "RPG" }];
        _generos.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>())).ReturnsAsync(generos);

        // Act
        var resultado = await CriarService().ListarAsync();

        // Assert
        Assert.Equal(new[] { "Ação", "RPG" }, resultado.Select(genero => genero.Nome));
    }
}
