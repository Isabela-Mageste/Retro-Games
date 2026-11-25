using RetroGames.Model.Model;

namespace RetroGames.Services.ServiceModels;

// Reúne dados de quatro tabelas (tb_jogo, tb_plataforma, tb_condicao_jogo e tb_genero),
// combinados com LINQ no JogoService, sem alterar as entidades.
public class JogoDetalheServiceModel
{
    public Jogo Jogo { get; set; } = new();
    public string NomePlataforma { get; set; } = string.Empty;
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<Genero> Generos { get; set; } = [];
}
