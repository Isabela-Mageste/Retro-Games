var construtor = WebApplication.CreateBuilder(args);
construtor.Services.AddControllers();
construtor.Services.AddOpenApi();
var aplicacao = construtor.Build();
if (aplicacao.Environment.IsDevelopment())
{
    aplicacao.MapOpenApi();
    aplicacao.UseSwaggerUI(opcoes =>
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames — Aula 1"));
}
aplicacao.MapControllers();
aplicacao.Run();
