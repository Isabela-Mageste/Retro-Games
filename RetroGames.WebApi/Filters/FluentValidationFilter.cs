using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RetroGames.WebApi.Filters;

// Filtro de ação: roda depois do model binding e antes da action. Para cada argumento
// que tem um IValidator<T> registrado, valida o formato. Com erro, responde 400 com o
// código 3 e os erros por campo, e a action não roda.
public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext contexto, ActionExecutionDelegate proximo)
    {
        foreach (var parametro in contexto.ActionDescriptor.Parameters)
        {
            contexto.ActionArguments.TryGetValue(parametro.Name, out var argumento);

            if (argumento is null)
            {
                // Corpo JSON igual a null: não há modelo para validar.
                if (parametro.BindingInfo?.BindingSource == BindingSource.Body)
                {
                    contexto.Result = RespostasDeValidacao.ModeloNulo();
                    return;
                }
                continue;
            }

            var tipoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (contexto.HttpContext.RequestServices.GetService(tipoValidator) is not IValidator validator)
                continue;

            var resultado = await validator.ValidateAsync(
                new ValidationContext<object>(argumento), contexto.HttpContext.RequestAborted);
            if (resultado.IsValid)
                continue;

            var errosPorCampo = resultado.Errors
                .GroupBy(erro => RespostasDeValidacao.NomeDoCampo(erro.PropertyName))
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Select(erro => erro.ErrorMessage).Distinct().ToArray());

            contexto.Result = RespostasDeValidacao.ModeloInvalido(errosPorCampo);
            return;
        }

        await proximo();
    }
}
