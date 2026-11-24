using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

// A senha nunca é guardada: o banco guarda só o hash gerado pelo PasswordHasher (RS04).
public class AutenticacaoService(IRepositorySet repositorio, IPasswordHasher<Usuario> passwordHasher)
    : IAutenticacaoService
{
    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await repositorio.Usuario.BuscarPorEmailAsync(email.Trim());

        // E-mail inexistente, senha errada e usuário inativo dão o mesmo resultado: nulo.
        // Assim a resposta não revela quais e-mails estão cadastrados.
        if (usuario is null || !usuario.Ativo)
            return null;

        var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string email, string senha)
    {
        // Subir a API de novo não cria um segundo administrador.
        if (await repositorio.Usuario.ExisteAdministradorAsync())
            return;

        var administrador = new Usuario
        {
            Nome = Perfis.Administrador,
            Email = email.Trim(),
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            IdPerfil = PerfilIds.Administrador,
        };
        administrador.SenhaHash = passwordHasher.HashPassword(administrador, senha);

        administrador.Id = await repositorio.Usuario.InserirAsync(administrador);
    }
}
