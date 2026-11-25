using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailAsync(string email, CancellationToken cancelamento = default);
    Task<Usuario?> BuscarPorIdAsync(int id, CancellationToken cancelamento = default);
    Task<bool> ExisteAdministradorAsync(CancellationToken cancelamento = default);
    Task<int> InserirAsync(Usuario usuario, CancellationToken cancelamento = default);
}
