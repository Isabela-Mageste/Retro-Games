using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

// Desafio, Módulo de Compras. Sem [AllowAnonymous]: vale a política padrão, que exige login
// (sem sessão, 401 com código 4). O usuário do pedido vem da sessão, no Service.
[ApiController]
[Route("api/[controller]")]
public class PedidoController(IPedidoService pedidoService) : ControllerBase
{
    // C04: finalizar a compra.
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<PedidoResponseApiModel>>> Finalizar(
        FinalizarPedidoRequestApiModel request)
    {
        var pedido = await pedidoService.FinalizarAsync(request.ParaModel());
        return StatusCode(StatusCodes.Status201Created,
            new RetroGamesResponseApiModel<PedidoResponseApiModel>(pedido.ParaResponse()));
    }

    // C05: meus pedidos.
    [HttpGet("meus")]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<PedidoResponseApiModel>>>> ListarMeus()
    {
        var pedidos = await pedidoService.ListarMeusAsync();
        return Ok(new RetroGamesResponseApiModel<List<PedidoResponseApiModel>>(
            pedidos.Select(pedido => pedido.ParaResponse()).ToList()));
    }
}
