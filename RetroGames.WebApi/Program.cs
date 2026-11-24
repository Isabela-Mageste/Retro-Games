using RetroGames.WebApi.Configuration;
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
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

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
