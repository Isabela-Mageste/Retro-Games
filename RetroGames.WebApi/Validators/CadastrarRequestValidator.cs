using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

// C01: formato do cadastro. Se o e-mail já existe é regra de negócio (106), no AutenticacaoService.
public class CadastrarRequestValidator : AbstractValidator<CadastrarRequestApiModel>
{
    public const int TamanhoMinimoSenha = 8;

    public CadastrarRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Informe o e-mail.")
            .MaximumLength(254).WithMessage("O e-mail deve ter no máximo 254 caracteres.")
            .EmailAddress().WithMessage("Informe um e-mail válido.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Informe a senha.")
            .MinimumLength(TamanhoMinimoSenha).WithMessage($"A senha deve ter pelo menos {TamanhoMinimoSenha} caracteres.")
            .MaximumLength(100).WithMessage("A senha deve ter no máximo 100 caracteres.")
            .Matches("[A-Za-z]").WithMessage("A senha deve ter pelo menos uma letra.")
            .Matches("[0-9]").WithMessage("A senha deve ter pelo menos um número.");
    }
}
