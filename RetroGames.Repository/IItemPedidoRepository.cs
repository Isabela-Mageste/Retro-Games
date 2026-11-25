using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IItemPedidoRepository
{
    Task InserirAsync(IReadOnlyCollection<ItemPedido> itens, CancellationToken cancelamento = default);
    Task<IReadOnlyList<ItemPedido>> ListarPorPedidosAsync(IReadOnlyCollection<int> idsPedidos, CancellationToken cancelamento = default);
}
