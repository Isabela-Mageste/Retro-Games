namespace RetroGames.WebApi.ApiModels;

// Uma página de resultados com os totais.
public class PaginaResponseApiModel<T>
{
    public List<T> Itens { get; set; } = [];
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public long TotalItens { get; set; }
    public int TotalPaginas { get; set; }
}
