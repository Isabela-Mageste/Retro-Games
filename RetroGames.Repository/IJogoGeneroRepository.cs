using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoGeneroRepository
{
    Task<IReadOnlyList<JogoGenero>> ListarTodosAsync(CancellationToken cancelamento = default);
    Task<IReadOnlyList<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos, CancellationToken cancelamento = default);
    Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros, CancellationToken cancelamento = default);
    Task RemoverPorJogoAsync(int idJogo, CancellationToken cancelamento = default);
}
