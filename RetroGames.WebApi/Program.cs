using FluentValidation;
using RetroGames.Services;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Filters;
using RetroGames.WebApi.Middlewares;

// Lê o .env da raiz do repositório (procurando nas pastas acima) e publica cada linha
// como variável de ambiente: ConnectionStrings__Default vira ConnectionStrings:Default.
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine(
        "Connection string não configurada: falta a linha ConnectionStrings__Default no arquivo .env. " +
        "Copie .env.example para .env, na raiz do repositório, e preencha os dados do seu MySQL.");
    return 1;
}

// 1. Serviços (injeção de dependência)
builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers(opcoes =>
    {
        // O FluentValidation é o único responsável pelo formato da entrada.
        opcoes.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        opcoes.Filters.Add<FluentValidationFilter>();
    })
    .ConfigureApiBehaviorOptions(ValidacaoConfig.ConfigurarRespostaDeModeloInvalido);
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var adminEmail = builder.Configuration["AdminInicial:Email"];
var adminSenha = builder.Configuration["AdminInicial:Senha"];
if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminSenha))
{
    Console.Error.WriteLine(
        "Administrador inicial não configurado: faltam as linhas AdminInicial__Email e AdminInicial__Senha no arquivo .env. " +
        "Confira o modelo .env.example.");
    return 1;
}

var app = builder.Build();

// Cria o primeiro administrador, se ainda não existir nenhum. A senha nunca vai para o log.
using (var escopo = app.Services.CreateScope())
{
    var autenticacaoService = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    try
    {
        await autenticacaoService.GarantirAdministradorInicialAsync(adminEmail, adminSenha);
    }
    catch (Exception excecao)
    {
        app.Logger.LogCritical(excecao,
            "Não foi possível garantir o administrador inicial. Confira se o MySQL está em execução " +
            "e se a connection string do .env aponta para o banco do RetroGames.");
        return 1;
    }
}

// 2. Pipeline (a ordem importa)
app.UseExceptionHandler(); // primeira etapa: exceções → envelope de erro

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes => opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames"));
}

app.MapControllers();

await app.RunAsync();
return 0;
