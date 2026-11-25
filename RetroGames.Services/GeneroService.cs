using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

// Gêneros só são lidos e não têm regra; o Service existe para o controller falar só com Services.
public class GeneroService(IRepositorySet repositorio) : IGeneroService
{
    public Task<IReadOnlyList<Genero>> ListarAsync()
    {
        return repositorio.Genero.ListarAsync();
    }
}
