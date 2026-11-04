# RetroGames | Verificação da Aula 1

O código extraído do ZIP passou nas verificações locais da aula. O roteiro completo continua com itens externos pendentes.

| Etapa | Verificação | Resultado observado |
|---|---|---|
| 1. Ambiente e Git | Histórico, diretrizes e quadro | Seis commits de preparação, AGENTS.md com o conteúdo do PDF e seis cartões locais. Repositório sem remote. |
| 2. Solução | Compilação e referências | Seis projetos net10.0 com as referências da seção 3.2. Build com SDK 10.0.403, zero avisos e zero erros. |
| 2. API | Inicialização em desenvolvimento | Swagger carregado. GET /openapi/v1.json na porta 5080 devolveu HTTP 200. O documento contém zero endpoints de negócio nesta aula. |
| 2. Front-end | Instalação pelo lockfile, build e clique | Node.js 22.23.4, Vite 8.3.5. O Chromium abriu a página na porta 5173 e o contador passou de 0 para 1 sem exceção JavaScript. |
| 3. Estrutura | Script 001 em dois schemas vazios | MySQL Community Server 8.0.41 criou as sete tabelas em ambos. SHOW TABLES e SHOW CREATE TABLE tb_jogo executados. |
| 4. Carga | Scripts 002 e 005 | Sete plataformas, oito gêneros, 24 jogos e zero usuários. Todos os jogos têm de um a três gêneros. Um inativo e um sem estoque. |
| 5. Consultas | Script 003 | Catálogo com 23 jogos ativos. Super Metroid tem três gêneros. RPG retorna três jogos. As sete plataformas aparecem na contagem, incluindo Sega Saturn com zero. A busca por Super traz cinco registros e depois dois, sem repetição. |
| 6. Modelos | Comparação com information_schema.columns | As 29 propriedades dos sete modelos correspondem às colunas em nome de coluna comentado, tipo e nulabilidade. Somente ds_jogo aceita NULL. IDs fixos de condições e perfis conferidos. |

## Integridade no MySQL

| Operação | Resultado nos dois schemas |
|---|---|
| Inserir jogo com plataforma inexistente | Erro 1452 |
| Repetir o nome NES | Erro 1062 |
| Excluir NES com jogos vinculados | Erro 1451 |
| Inserir jogo com nome nulo | Erro 1048 |
| Inserir jogo com preço zero | Aceito com 0.00. O rollback removeu a linha. A regra RN06 cabe à camada de negócio. |

Depois dos testes, a contagem continuou em 24 jogos e a busca por registros de teste retornou zero. Os comentários de database/004_testes_integridade.sql registram cada resultado.

## Limites e pendências

O comando dotnet test executou a descoberta e encontrou zero testes xUnit. O roteiro desta aula exige os cinco testes de integridade SQL relatados acima. As funcionalidades de catálogo, manutenção e autenticação pertencem às próximas aulas.

O roteiro também pede publicação no GitHub, quadro remoto e conferência da estrutura no Table Inspector do Workbench. A auditoria validou a estrutura diretamente no servidor. A publicação, o quadro online, a interface do Workbench e as instalações no computador do estudante ficam por conferir.
