namespace RetroGames.WebApi.ApiModels;

// C01: entrada do cadastro de cliente. O perfil não vem do cliente: o Service sempre usa Cliente.
public class CadastrarRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
