using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

// C01: cadastro de cliente, com o repositório simulado. O PasswordHasher é o real: ele não usa banco.
public class AutenticacaoServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    public AutenticacaoServiceTests()
    {
        _repositorio.Setup(r => r.Usuario).Returns(_usuarios.Object);
    }

    private AutenticacaoService CriarService() => new(_repositorio.Object, new PasswordHasher<Usuario>());

    [Fact]
    public async Task CadastrarCliente_EmailJaCadastrado_LancaEmailJaCadastradoSemGravar()
    {
        // Arrange
        _usuarios.Setup(r => r.BuscarPorEmailAsync("ana@exemplo.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = 5, Email = "ana@exemplo.com" });

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().CadastrarClienteAsync("Ana", "ana@exemplo.com", "senha1234"));

        // Assert
        Assert.Equal(BusinessErrors.EMAIL_JA_CADASTRADO, erro.Erro);
        _usuarios.Verify(r => r.InserirAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarCliente_BancoRecusaPorDuplicidade_LancaEmailJaCadastrado()
    {
        _usuarios.Setup(r => r.InserirAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RegistroDuplicadoException(new Exception("Duplicate entry")));

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(
            () => CriarService().CadastrarClienteAsync("Ana", "ana@exemplo.com", "senha1234"));

        Assert.Equal(BusinessErrors.EMAIL_JA_CADASTRADO, erro.Erro);
    }

    [Fact]
    public async Task CadastrarCliente_DadosNovos_CriaClienteAtivoComSenhaEmHash()
    {
        // Arrange
        Usuario? gravado = null;
        _usuarios.Setup(r => r.InserirAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .Callback<Usuario, CancellationToken>((usuario, _) => gravado = usuario)
            .ReturnsAsync(42);

        // Act
        var cliente = await CriarService().CadastrarClienteAsync("  Ana Souza ", " ana@exemplo.com ", "senha1234");

        // Assert
        Assert.Equal(42, cliente.Id);
        Assert.NotNull(gravado);
        Assert.Equal(PerfilIds.Cliente, gravado!.IdPerfil);
        Assert.True(gravado.Ativo);
        Assert.Equal("Ana Souza", gravado.Nome);
        Assert.Equal("ana@exemplo.com", gravado.Email);
        Assert.NotEqual("senha1234", gravado.SenhaHash);
        Assert.Equal(PasswordVerificationResult.Success,
            new PasswordHasher<Usuario>().VerifyHashedPassword(gravado, gravado.SenhaHash, "senha1234"));
    }
}
