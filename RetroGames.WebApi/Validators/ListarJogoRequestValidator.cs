using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

// Formato dos parâmetros da listagem. Não consulta o banco.
public class ListarJogoRequestValidator : AbstractValidator<ListarJogoRequestApiModel>
{
    public ListarJogoRequestValidator()
    {
        RuleFor(x => x.Busca)
            .MaximumLength(150).WithMessage("A busca deve ter no máximo 150 caracteres.");

        RuleFor(x => x.IdPlataforma)
            .GreaterThan(0).When(x => x.IdPlataforma.HasValue).WithMessage("Plataforma inválida.");

        RuleFor(x => x.IdGenero)
            .GreaterThan(0).When(x => x.IdGenero.HasValue).WithMessage("Gênero inválido.");

        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("Página inválida.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}
