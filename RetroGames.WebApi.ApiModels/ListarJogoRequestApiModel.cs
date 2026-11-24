namespace RetroGames.WebApi.ApiModels;

// Parâmetros da listagem de jogos, lidos da query string:
// /api/Jogo?busca=zelda&idPlataforma=2&idGenero=1&pagina=1&tamanhoPagina=12&incluirInativos=true
public class ListarJogoRequestApiModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;

    // Somente o administrador vê jogos inativos; para os demais, o parâmetro é ignorado.
    public bool IncluirInativos { get; set; }
}
