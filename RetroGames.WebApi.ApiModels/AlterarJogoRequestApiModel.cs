namespace RetroGames.WebApi.ApiModels;

// Entrada da alteração de jogo (PUT envia o recurso completo). O id vem da rota.
// Ativo permite reativar um jogo desativado.
public class AlterarJogoRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; } = true;
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }
    public List<int> IdsGeneros { get; set; } = [];
}
