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

    // Desafio, C04/C05: vários jogos numa única consulta (inclusive inativos).
    Task<IReadOnlyList<Jogo>> ListarPorIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancelamento = default);

    // Desafio, C04: baixa o estoque só se houver quantidade suficiente. Devolve false se não baixou.
    Task<bool> BaixarEstoqueAsync(int id, int quantidade, CancellationToken cancelamento = default);
}
