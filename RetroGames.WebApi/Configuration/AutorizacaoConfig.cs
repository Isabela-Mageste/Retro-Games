using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AddPoliticasDeAutorizacao(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            // Segura por padrão (RS03): toda rota exige login, exceto as marcadas com [AllowAnonymous].
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build())
            // Rotas de manutenção: sem login, 401; logado sem o perfil, 403.
            .AddPolicy(Politicas.Administrador, politica => politica
                .RequireAuthenticatedUser()
                .RequireRole(Perfis.Administrador));

        return services;
    }
}
