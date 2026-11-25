using RetroGames.Model.Model;

namespace RetroGames.Services.ServiceModels;

// Um pedido com a descrição da situação e os itens, combinados com LINQ a partir de
// tb_pedido, tb_situacao_pedido, tb_item_pedido e tb_jogo.
public class PedidoServiceModel
{
    public Pedido Pedido { get; set; } = new();
    public string DescricaoSituacao { get; set; } = string.Empty;
    public List<ItemPedidoServiceModel> Itens { get; set; } = [];
}

public class ItemPedidoServiceModel
{
    public ItemPedido Item { get; set; } = new();
    public string NomeJogo { get; set; } = string.Empty;
}
