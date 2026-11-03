# Validação e pendências

| Etapa | Material preparado | Verificação ainda necessária |
|---|---|---|
| 1 | .gitignore, AGENTS, quadro local, Git local | Instalar/conferir ferramentas na máquina; criar repositório e Projects no GitHub; push |
| 2 | .sln, seis projetos e front-end | Interação no navegador; restore, dotnet build, npm install/build/dev e HTTP local já verificados |
| 3 | DDL com sete tabelas | Executar no MySQL 8; SHOW TABLES e Table Inspector; recriar em outro schema vazio |
| 4 | 7 plataformas, 8 gêneros, 24 jogos | Executar seed e conferir totais e relacionamentos |
| 5 | 5 consultas e 5 testes | Executar e anotar resultados reais; confirmar rollback e ausência de resíduos |
| 6 | 7 modelos e 2 classes de constantes | Comparar com as tabelas efetivamente criadas; modelos já compilados |

Conferência estática: referências da solução, XML de projetos, JSON de configuração, colunas/propriedades, quantidades dos dados, 1–3 gêneros por jogo, plataforma vazia, inativo, estoque zero e busca com segunda página. Essas verificações não substituem execução em .NET/MySQL.

Resultados SQL esperados: catálogo 23; busca Super 7 (5 na primeira página e 2 na segunda); RPG 3; Super Metroid 3 gêneros. Integridade: 1452, 1062, 1451, 1048 e aceite de preço zero, respectivamente.

Execução confirmada: npm install (51 pacotes), npm run build (Vite 8.3.5, 17 módulos) e GET HTTP local na porta 5173 retornando o HTML do projeto. Executados com Node.js v24.22.1, não com a versão 22 de referência. package-lock.json incluído.

Validação final .NET: build da solução com SDK 10.0.403, zero avisos e zero erros. Comando: dotnet build RetroGames.sln -m:1 -p:UseSharedCompilation=false. OpenAPI ajustado para Microsoft.AspNetCore.OpenApi 10.0.14 dentro da linha 10.x; o restore final não reportou o aviso NU1903 da versão inicial. API iniciada e GET /openapi/v1.json retornou HTTP 200, com paths vazio como esperado nesta aula. dotnet test --no-build --no-restore -m:1 executou e informou ausência de testes, pois ainda não há classes de testes; isso não equivale a testes funcionais aprovados.
