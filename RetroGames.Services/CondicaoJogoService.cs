using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

// Condições de jogo só são lidas e não têm regra.
public class CondicaoJogoService(IRepositorySet repositorio) : ICondicaoJogoService
{
    public Task<IReadOnlyList<CondicaoJogo>> ListarAsync()
    {
        return repositorio.CondicaoJogo.ListarAsync();
    }
}
