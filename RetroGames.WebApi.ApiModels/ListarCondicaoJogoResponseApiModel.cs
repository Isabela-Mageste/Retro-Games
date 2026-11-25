namespace RetroGames.WebApi.ApiModels;

// Dados de uma condição de jogo que saem no JSON.
public class ListarCondicaoJogoResponseApiModel
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
