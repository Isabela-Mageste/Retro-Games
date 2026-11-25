using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public interface IPedidoService
{
    // C04: cada item informa só IdJogo e Quantidade; o preço vem do banco.
    Task<PedidoServiceModel> FinalizarAsync(IReadOnlyCollection<ItemPedido> itens);

    // C05: os pedidos do usuário logado, do mais recente para o mais antigo.
    Task<List<PedidoServiceModel>> ListarMeusAsync();
}
