using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.Configuration;

// Registro dos repositórios e dos Services: "quem pedir esta interface recebe esta classe".
public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(this IServiceCollection services, string connectionString)
    {
        // Singleton: uma única instância, que só guarda a connection string.
        services.AddSingleton(new RepositoryConfig { ConnectionString = connectionString });

        // Scoped: uma instância por requisição HTTP.
        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        services.AddScoped<IRepositorySet, RepositorySet>();
        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection services)
    {
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        services.AddScoped<IJogoService, JogoService>();
        return services;
    }
}
