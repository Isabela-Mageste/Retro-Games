using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

// Regras de jogo: RN03 (exclusão lógica), RN04 (catálogo público só com ativos) e
// RN05 (referências válidas). Os dados de várias tabelas são combinados com LINQ.
public class JogoService(IRepositorySet repositorio) : IJogoService
{
    public async Task<PaginaServiceModel<JogoDetalheServiceModel>> ListarAsync(ListarJogoFiltroServiceModel filtro)
    {
        // Filtro que depende de outra tabela: o LINQ escolhe os ids dos jogos do gênero e
        // o SQL continua fazendo a contagem e a paginação.
        List<int>? idsJogos = null;
        if (filtro.IdGenero.HasValue)
        {
            var associacoes = await repositorio.JogoGenero.ListarTodosAsync();
            idsJogos = associacoes
                .Where(associacao => associacao.IdGenero == filtro.IdGenero.Value)
                .Select(associacao => associacao.IdJogo)
                .Distinct()
                .ToList();
        }

        var busca = string.IsNullOrWhiteSpace(filtro.Busca) ? null : filtro.Busca.Trim();

        // RN04: a consulta pública devolve apenas jogos ativos.
        var (jogos, total) = await repositorio.Jogo.ListarAsync(
            busca, filtro.IdPlataforma, idsJogos, false, filtro.Pagina, filtro.TamanhoPagina);

        return new PaginaServiceModel<JogoDetalheServiceModel>
        {
            Itens = await MontarDetalhesAsync(jogos),
            Pagina = filtro.Pagina,
            TamanhoPagina = filtro.TamanhoPagina,
            TotalItens = total,
        };
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await repositorio.Jogo.BuscarPorIdAsync(id);

        // RN04: jogo inativo não aparece no catálogo público.
        if (jogo is null || !jogo.Ativo)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        return (await MontarDetalhesAsync([jogo]))[0];
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        var generos = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, generos);

        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.DataCadastro = DateTime.UtcNow;

        // Jogo e vínculos são gravados juntos: ou tudo é gravado, ou nada.
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await repositorio.Jogo.InserirAsync(jogo);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transactionScope.Complete();
        }

        return (await MontarDetalhesAsync([jogo]))[0];
    }

    public async Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        // A alteração alcança também jogos inativos, para permitir reativá-los.
        var existente = await repositorio.Jogo.BuscarPorIdAsync(jogo.Id);
        if (existente is null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        var generos = idsGeneros.Distinct().ToList();
        await ValidarReferenciasAsync(jogo, generos);

        jogo.Nome = jogo.Nome.Trim();
        jogo.DataCadastro = existente.DataCadastro;

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                   transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        {
            await repositorio.Jogo.AlterarAsync(jogo);
            await repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transactionScope.Complete();
        }

        return (await MontarDetalhesAsync([jogo]))[0];
    }

    public async Task DesativarAsync(int id)
    {
        if (await repositorio.Jogo.BuscarPorIdAsync(id) is null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        // RN03: o jogo nunca é apagado; só deixa de estar ativo.
        await repositorio.Jogo.DesativarAsync(id);
    }

    // RN05: plataforma, condição e ao menos um gênero precisam existir.
    private async Task ValidarReferenciasAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        if (await repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma) is null)
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);

        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(condicao => condicao.Id == jogo.IdCondicaoJogo))
            throw new BusinessRuleException(BusinessErrors.CONDICAO_NAO_ENCONTRADA);

        var generos = await repositorio.Genero.ListarAsync();
        var algumInexistente = idsGeneros.Any(idGenero => !generos.Any(genero => genero.Id == idGenero));
        if (idsGeneros.Count == 0 || algumInexistente)
            throw new BusinessRuleException(BusinessErrors.GENERO_NAO_ENCONTRADO);
    }

    // Monta o detalhe de uma lista de jogos sempre com quatro consultas, qualquer que
    // seja o tamanho da lista (nunca uma consulta por jogo).
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(IReadOnlyList<Jogo> jogos)
    {
        if (jogos.Count == 0)
            return [];

        var plataformas = await repositorio.Plataforma.ListarAsync();
        var condicoes = await repositorio.CondicaoJogo.ListarAsync();
        var generos = await repositorio.Genero.ListarAsync();
        var vinculos = await repositorio.JogoGenero.ListarPorJogosAsync(jogos.Select(jogo => jogo.Id).ToList());

        var detalhes = from jogo in jogos
                       join plataforma in plataformas on jogo.IdPlataforma equals plataforma.Id
                       join condicao in condicoes on jogo.IdCondicaoJogo equals condicao.Id
                       select new JogoDetalheServiceModel
                       {
                           Jogo = jogo,
                           NomePlataforma = plataforma.Nome,
                           DescricaoCondicao = condicao.Descricao,
                           Generos = (from vinculo in vinculos
                                      where vinculo.IdJogo == jogo.Id
                                      join genero in generos on vinculo.IdGenero equals genero.Id
                                      orderby genero.Nome
                                      select genero).ToList(),
                       };

        return detalhes.ToList();
    }
}
