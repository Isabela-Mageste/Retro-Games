using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PedidoMapper
{
    public static List<ItemPedido> ParaModel(this FinalizarPedidoRequestApiModel request) =>
        request.Itens.Select(item => new ItemPedido { IdJogo = item.IdJogo, Quantidade = item.Quantidade }).ToList();

    public static PedidoResponseApiModel ParaResponse(this PedidoServiceModel pedido) => new()
    {
        Id = pedido.Pedido.Id,
        DataPedido = pedido.Pedido.DataPedido,
        Situacao = pedido.DescricaoSituacao,
        Total = pedido.Pedido.Total,
        Itens = pedido.Itens.Select(item => new ItemPedidoResponseApiModel
        {
            IdJogo = item.Item.IdJogo,
            NomeJogo = item.NomeJogo,
            Quantidade = item.Item.Quantidade,
            PrecoUnitario = item.Item.PrecoUnitario,
            Subtotal = item.Item.PrecoUnitario * item.Item.Quantidade,
        }).ToList(),
    };
}
