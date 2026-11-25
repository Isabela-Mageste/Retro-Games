namespace RetroGames.Services.ServiceModels;

// Uma página de resultados e o total de itens encontrados com o mesmo filtro.
public class PaginaServiceModel<T>
{
    public List<T> Itens { get; set; } = [];
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public long TotalItens { get; set; }

    public int TotalPaginas =>
        TamanhoPagina <= 0 ? 0 : (int)((TotalItens + TamanhoPagina - 1) / TamanhoPagina);
}
