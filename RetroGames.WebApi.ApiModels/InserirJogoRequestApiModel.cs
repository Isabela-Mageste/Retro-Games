namespace RetroGames.WebApi.ApiModels;

// Entrada da inclusão de jogo. Sem Id, sem Ativo e sem data de cadastro: quem define
// esses valores é o Service (proteção contra overposting).
public class InserirJogoRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }
    public List<int> IdsGeneros { get; set; } = [];
}
