using System.Net;

namespace RetroGames.Services;

// Lançada pelo Service quando uma regra de negócio é violada. Guarda o erro do
// catálogo BusinessErrors; o GlobalExceptionHandler da API responde com o status,
// o código e a mensagem desse erro.
public class BusinessRuleException : Exception
{
    public (int Code, string DefaultMessage, HttpStatusCode Status) Erro { get; }

    public BusinessRuleException((int Code, string DefaultMessage, HttpStatusCode Status) erro)
        : base(erro.DefaultMessage)
    {
        Erro = erro;
    }

    // Mesmo erro do catálogo (código e status), com uma mensagem mais específica para a tela,
    // por exemplo "Estoque insuficiente para Tetris.".
    public BusinessRuleException((int Code, string DefaultMessage, HttpStatusCode Status) erro, string mensagem)
        : base(mensagem)
    {
        Erro = erro;
    }
}
