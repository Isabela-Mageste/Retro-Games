namespace RetroGames.WebApi.ApiModels;

// Entrada da alteração (PUT envia o recurso completo). O id vem da rota, nunca do corpo.
public class AlterarPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
}
