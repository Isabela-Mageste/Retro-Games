namespace RetroGames.WebApi.ApiModels;

// C04: o carrinho enviado na finalização. Só o id do jogo e a quantidade: o preço vem do banco
// e o usuário vem da sessão.
public class FinalizarPedidoRequestApiModel
{
    public List<ItemPedidoRequestApiModel> Itens { get; set; } = [];
}

public class ItemPedidoRequestApiModel
{
    public int IdJogo { get; set; }
    public int Quantidade { get; set; }
}
