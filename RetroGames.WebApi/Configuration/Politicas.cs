namespace RetroGames.WebApi.Configuration;

// Nomes das políticas de autorização usadas em [Authorize(Policy = ...)].
public static class Politicas
{
    public const string Administrador = "SomenteAdministrador";
}
