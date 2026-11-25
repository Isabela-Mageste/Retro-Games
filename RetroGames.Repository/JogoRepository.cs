using MySqlConnector;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas = "co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo";

    public JogoRepository(RepositoryConfig configuracao) : base(configuracao) { }

    public async Task<(IReadOnlyList<Jogo> Jogos, long Total)> ListarAsync(
        string? nome = null, int? idPlataforma = null, IReadOnlyCollection<int>? idsJogos = null,
        bool incluirInativos = false, int pagina = 1, int tamanhoPagina = 5,
        CancellationToken cancelamento = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pagina, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(tamanhoPagina, 1);
        var ids = idsJogos?.ToArray();
        var filtros = new List<string> { "(@incluirInativos = TRUE OR st_ativo = TRUE)" };
        var parametros = new Dictionary<string, object?> { ["@incluirInativos"] = incluirInativos };
        if (!string.IsNullOrEmpty(nome))
        {
            filtros.Add("no_jogo LIKE @nome ESCAPE '!' ");
            parametros["@nome"] = "%" + nome.Replace("!", "!!").Replace("%", "!%").Replace("_", "!_") + "%";
        }
        if (idPlataforma.HasValue)
        {
            filtros.Add("co_plataforma = @idPlataforma");
            parametros["@idPlataforma"] = idPlataforma.Value;
        }
        if (ids is not null)
        {
            if (ids.Length == 0)
                filtros.Add("1 = 0");
            else
            {
                var nomes = new List<string>();
                for (var indice = 0; indice < ids.Length; indice++)
                {
                    var parametro = "@id" + indice;
                    nomes.Add(parametro);
                    parametros[parametro] = ids[indice];
                }
                filtros.Add("co_seq_jogo IN (" + string.Join(", ", nomes) + ")");
            }
        }
        // Somente trechos fixos e nomes de parâmetros entram na montagem do SQL.
        var filtro = " WHERE " + string.Join(" AND ", filtros);
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var contagem = CriarComando(conexao, "SELECT COUNT(*) FROM tb_jogo" + filtro);
        await using var comando = CriarComando(conexao, "SELECT " + Colunas + " FROM tb_jogo" + filtro + " ORDER BY no_jogo, co_seq_jogo LIMIT @limite OFFSET @deslocamento");
        foreach (var parametro in parametros)
        {
            AdicionarParametro(contagem, parametro.Key, parametro.Value);
            AdicionarParametro(comando, parametro.Key, parametro.Value);
        }
        AdicionarParametro(comando, "@limite", tamanhoPagina);
        AdicionarParametro(comando, "@deslocamento", ((long)pagina - 1) * tamanhoPagina);
        var total = Convert.ToInt64(await contagem.ExecuteScalarAsync(cancelamento));
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        var jogos = new List<Jogo>();
        while (await leitor.ReadAsync(cancelamento))
            jogos.Add(Mapear(leitor));
        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id, CancellationToken cancelamento = default)
    {
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, "SELECT " + Colunas + " FROM tb_jogo WHERE co_seq_jogo = @id");
        AdicionarParametro(comando, "@id", id);
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        return await leitor.ReadAsync(cancelamento) ? Mapear(leitor) : null;
    }

    public async Task<int> InserirAsync(Jogo jogo, CancellationToken cancelamento = default)
    {
        const string sql = "INSERT INTO tb_jogo (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo) VALUES (@nome, @descricao, @ano, @preco, @estoque, @ativo, @dataCadastro, @idPlataforma, @idCondicaoJogo)";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        PreencherParametros(comando, jogo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        await ExecutarGravacaoAsync(comando, cancelamento);
        return checked((int)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo, CancellationToken cancelamento = default)
    {
        const string sql = "UPDATE tb_jogo SET no_jogo = @nome, ds_jogo = @descricao, nu_ano_lancamento = @ano, vl_preco = @preco, qt_estoque = @estoque, st_ativo = @ativo, co_plataforma = @idPlataforma, co_condicao_jogo = @idCondicaoJogo WHERE co_seq_jogo = @id";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        PreencherParametros(comando, jogo);
        AdicionarParametro(comando, "@id", jogo.Id);
        await ExecutarGravacaoAsync(comando, cancelamento);
    }

    public async Task DesativarAsync(int id, CancellationToken cancelamento = default)
    {
        const string sql = "UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @id";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id", id);
        await ExecutarGravacaoAsync(comando, cancelamento);
    }

    public async Task<IReadOnlyList<Jogo>> ListarPorIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancelamento = default)
    {
        cancelamento.ThrowIfCancellationRequested();
        var lista = ids.ToArray();
        if (lista.Length == 0)
            return Array.Empty<Jogo>();
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, string.Empty);
        var nomes = new List<string>();
        for (var indice = 0; indice < lista.Length; indice++)
        {
            nomes.Add("@id" + indice);
            AdicionarParametro(comando, "@id" + indice, lista[indice]);
        }
        // Somente trechos fixos e nomes de parâmetros entram na montagem do SQL.
        comando.CommandText = "SELECT " + Colunas + " FROM tb_jogo WHERE co_seq_jogo IN (" + string.Join(", ", nomes) + ")";
        await using var leitor = await comando.ExecuteReaderAsync(cancelamento);
        var jogos = new List<Jogo>();
        while (await leitor.ReadAsync(cancelamento))
            jogos.Add(Mapear(leitor));
        return jogos;
    }

    // O "qt_estoque >= @quantidade" protege contra duas compras simultâneas do último item:
    // se outra transação já baixou o estoque, nenhuma linha é alterada e o Service decide o erro.
    public async Task<bool> BaixarEstoqueAsync(int id, int quantidade, CancellationToken cancelamento = default)
    {
        const string sql = "UPDATE tb_jogo SET qt_estoque = qt_estoque - @quantidade WHERE co_seq_jogo = @id AND qt_estoque >= @quantidade";
        await using var conexao = await AbrirConexaoAsync(cancelamento);
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@quantidade", quantidade);
        AdicionarParametro(comando, "@id", id);
        return await ExecutarGravacaoAsync(comando, cancelamento) > 0;
    }

    private void PreencherParametros(MySqlCommand comando, Jogo jogo)
    {
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicaoJogo", jogo.IdCondicaoJogo);
    }

    private static Jogo Mapear(MySqlDataReader leitor) => new()
    {
        Id = leitor.GetInt32("co_seq_jogo"),
        Nome = leitor.GetString("no_jogo"),
        Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo")) ? null : leitor.GetString("ds_jogo"),
        AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
        Preco = leitor.GetDecimal("vl_preco"),
        Estoque = leitor.GetInt32("qt_estoque"),
        Ativo = leitor.GetBoolean("st_ativo"),
        DataCadastro = DateTime.SpecifyKind(leitor.GetDateTime("dt_cadastro"), DateTimeKind.Utc),
        IdPlataforma = leitor.GetInt32("co_plataforma"),
        IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
    };
}
