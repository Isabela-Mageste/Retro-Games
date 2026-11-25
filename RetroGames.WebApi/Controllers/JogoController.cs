using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController(IJogoService jogoService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var pagina = await jogoService.ListarAsync(request.ParaFiltro());
        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>(pagina.ParaResponse()));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> ListarPorId(int id)
    {
        var detalhe = await jogoService.BuscarPorIdAsync(id);
        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>(detalhe.ParaDetalheResponse()));
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Inserir(
        InserirJogoRequestApiModel request)
    {
        var criado = await jogoService.InserirAsync(request.ParaModel(), request.IdsGeneros);
        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Jogo.Id },
            new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>(criado.ParaDetalheResponse()));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Alterar(
        int id, AlterarJogoRequestApiModel request)
    {
        var alterado = await jogoService.AlterarAsync(request.ParaModel(id), request.IdsGeneros);
        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>(alterado.ParaDetalheResponse()));
    }

    // Exclusão lógica (RN03): o jogo só é desativado.
    [HttpDelete("{id:int}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Desativar(int id)
    {
        await jogoService.DesativarAsync(id);
        return Ok(new RetroGamesResponseApiModel());
    }
}
