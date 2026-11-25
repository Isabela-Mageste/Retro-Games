namespace RetroGames.WebApi.ApiModels;

// C04/C05: um pedido com situação, total e itens (jogo, quantidade e preço pago).
public class PedidoResponseApiModel
{
    public int Id { get; set; }
    public DateTime DataPedido { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<ItemPedidoResponseApiModel> Itens { get; set; } = [];
}

public class ItemPedidoResponseApiModel
{
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
