using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

// Converte o request no filtro do Service e o JogoDetalheServiceModel nos ApiModels de saída.
public static class JogoMapper
{
    public static ListarJogoFiltroServiceModel ParaFiltro(this ListarJogoRequestApiModel request) => new()
    {
        Busca = request.Busca,
        IdPlataforma = request.IdPlataforma,
        IdGenero = request.IdGenero,
        Pagina = request.Pagina,
        TamanhoPagina = request.TamanhoPagina,
    };

    public static ListarJogoResponseApiModel ParaListarResponse(this JogoDetalheServiceModel detalhe) => new()
    {
        Id = detalhe.Jogo.Id,
        Nome = detalhe.Jogo.Nome,
        AnoLancamento = detalhe.Jogo.AnoLancamento,
        Preco = detalhe.Jogo.Preco,
        Estoque = detalhe.Jogo.Estoque,
        Ativo = detalhe.Jogo.Ativo,
        NomePlataforma = detalhe.NomePlataforma,
        DescricaoCondicao = detalhe.DescricaoCondicao,
        Generos = detalhe.Generos.Select(genero => genero.Nome).ToList(),
    };

    public static ListarPorIdJogoResponseApiModel ParaDetalheResponse(this JogoDetalheServiceModel detalhe) => new()
    {
        Id = detalhe.Jogo.Id,
        Nome = detalhe.Jogo.Nome,
        Descricao = detalhe.Jogo.Descricao,
        AnoLancamento = detalhe.Jogo.AnoLancamento,
        Preco = detalhe.Jogo.Preco,
        Estoque = detalhe.Jogo.Estoque,
        Ativo = detalhe.Jogo.Ativo,
        IdPlataforma = detalhe.Jogo.IdPlataforma,
        NomePlataforma = detalhe.NomePlataforma,
        IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
        DescricaoCondicao = detalhe.DescricaoCondicao,
        Generos = detalhe.Generos.Select(genero => genero.ParaResponse()).ToList(),
    };

    public static PaginaResponseApiModel<ListarJogoResponseApiModel> ParaResponse(
        this PaginaServiceModel<JogoDetalheServiceModel> pagina) => new()
    {
        Itens = pagina.Itens.Select(detalhe => detalhe.ParaListarResponse()).ToList(),
        Pagina = pagina.Pagina,
        TamanhoPagina = pagina.TamanhoPagina,
        TotalItens = pagina.TotalItens,
        TotalPaginas = pagina.TotalPaginas,
    };
}
