using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPedidoRepository
{
    Task<int> InserirAsync(Pedido pedido, CancellationToken cancelamento = default);
    Task<IReadOnlyList<Pedido>> ListarPorUsuarioAsync(int idUsuario, CancellationToken cancelamento = default);
}
