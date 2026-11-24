using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

// Recebe a requisição, chama o Service, converte o resultado e responde com o envelope.
// Sem SQL, sem regra de negócio e sem try/catch: os erros vão para o GlobalExceptionHandler.
[ApiController]
[Route("api/[controller]")]
public class PlataformaController(IPlataformaService plataformaService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>>> Listar()
    {
        var plataformas = await plataformaService.ListarAsync();
        return Ok(new RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>(
            plataformas.Select(plataforma => plataforma.ParaResponse()).ToList()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> ListarPorId(int id)
    {
        var plataforma = await plataformaService.BuscarPorIdAsync(id);
        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>(plataforma.ParaResponse()));
    }

    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Inserir(
        InserirPlataformaRequestApiModel request)
    {
        var criada = await plataformaService.InserirAsync(request.ParaModel());

        // 201 Created, com o cabeçalho Location apontando para GET /api/Plataforma/{id}.
        return CreatedAtAction(nameof(ListarPorId), new { id = criada.Id },
            new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>(criada.ParaResponse()));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Alterar(
        int id, AlterarPlataformaRequestApiModel request)
    {
        var alterada = await plataformaService.AlterarAsync(request.ParaModel(id));
        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>(alterada.ParaResponse()));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await plataformaService.ExcluirAsync(id);
        return Ok(new RetroGamesResponseApiModel());
    }
}
