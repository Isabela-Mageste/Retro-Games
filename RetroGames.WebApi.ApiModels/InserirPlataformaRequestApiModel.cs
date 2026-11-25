namespace RetroGames.WebApi.ApiModels;

// Entrada da inclusão: só o que o cliente pode enviar (sem Id).
public class InserirPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
}
