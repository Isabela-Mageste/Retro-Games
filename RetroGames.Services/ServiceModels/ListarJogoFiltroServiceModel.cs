namespace RetroGames.Services.ServiceModels;

// Filtro da listagem de jogos, montado pela API a partir da query string.
public class ListarJogoFiltroServiceModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
}
