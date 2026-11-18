using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    Task<(IReadOnlyList<Jogo> Jogos, long Total)> ListarAsync(
        string? nome = null, int? idPlataforma = null, IReadOnlyCollection<int>? idsJogos = null,
        bool incluirInativos = false, int pagina = 1, int tamanhoPagina = 5,
        CancellationToken cancelamento = default);
    Task<Jogo?> BuscarPorIdAsync(int id, CancellationToken cancelamento = default);
    Task<int> InserirAsync(Jogo jogo, CancellationToken cancelamento = default);
    Task AlterarAsync(Jogo jogo, CancellationToken cancelamento = default);
    Task DesativarAsync(int id, CancellationToken cancelamento = default);
}
