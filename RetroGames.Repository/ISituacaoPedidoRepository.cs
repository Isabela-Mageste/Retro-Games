using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface ISituacaoPedidoRepository
{
    Task<IReadOnlyList<SituacaoPedido>> ListarAsync(CancellationToken cancelamento = default);
}
