namespace RetroGames.WebApi.ApiModels;

// Códigos que nascem na própria API (0 a 99). Os erros de negócio, a partir de 100,
// ficam no BusinessErrors do projeto Services. Um código novo recebe o próximo número livre.
public static class ResponseCodes
{
    public const int SUCESSO = 0;
    public const int ERRO = 1;
}
