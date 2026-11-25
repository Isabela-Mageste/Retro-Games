using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

// Regras de plataforma: RN01 (nome único) e RN02 (não excluir plataforma com jogos).
public class PlataformaService(IRepositorySet repositorio) : IPlataformaService
{
    public Task<IReadOnlyList<Plataforma>> ListarAsync()
    {
        return repositorio.Plataforma.ListarAsync();
    }

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await repositorio.Plataforma.BuscarPorIdAsync(id);
        if (plataforma is null)
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);
        return plataforma;
    }

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        // RN01: a conferência dá a mensagem clara na situação comum.
        if (await repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome))
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);

        try
        {
            plataforma.Id = await repositorio.Plataforma.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            // Duas gravações simultâneas passaram pela conferência: o UNIQUE recusou a segunda.
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }
        return plataforma;
    }

    public async Task<Plataforma> AlterarAsync(Plataforma plataforma)
    {
        await BuscarPorIdAsync(plataforma.Id);
        plataforma.Nome = plataforma.Nome.Trim();

        // Manter o próprio nome não é duplicidade: a plataforma alterada é ignorada na busca.
        if (await repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome, plataforma.Id))
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);

        try
        {
            await repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }
        return plataforma;
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id);

        // RN02: plataforma com jogos vinculados não pode ser excluída.
        if (await repositorio.Plataforma.PossuiJogosAsync(id))
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_POSSUI_JOGOS);

        await repositorio.Plataforma.ExcluirAsync(id);
    }
}
