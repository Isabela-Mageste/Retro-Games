using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

// C04: formato do pedido (400, código 3). Existência, situação e estoque dos jogos são regras
// de negócio e ficam no PedidoService (110 e 111).
public class FinalizarPedidoRequestValidator : AbstractValidator<FinalizarPedidoRequestApiModel>
{
    public const int QuantidadeMinima = 1;
    public const int QuantidadeMaxima = 10;

    public FinalizarPedidoRequestValidator()
    {
        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("Adicione ao menos um jogo ao pedido.");

        // Um jogo aparece uma só vez em cada pedido (UNIQUE em tb_item_pedido).
        RuleFor(x => x.Itens)
            .Must(itens => itens.Select(item => item.IdJogo).Distinct().Count() == itens.Count)
            .When(x => x.Itens is { Count: > 0 })
            .WithMessage("Cada jogo pode aparecer uma só vez no pedido.");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(x => x.IdJogo)
                .GreaterThan(0).WithMessage("Jogo inválido.");
            item.RuleFor(x => x.Quantidade)
                .InclusiveBetween(QuantidadeMinima, QuantidadeMaxima)
                .WithMessage($"A quantidade deve ser de {QuantidadeMinima} a {QuantidadeMaxima}.");
        });
    }
}
