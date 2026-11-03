# Decisões e limites

Fontes: especificação técnica v1.0, roteiro da Aula 1 e AGENTS.md fornecidos. Não foi fornecido o Guia rápido de ferramentas nem o material Fundamentos teóricos.

O dicionário lista colunas e nulabilidade, mas não fixa tamanhos de textos nem todos os tipos SQL. Os tamanhos abaixo são escolhas de implementação, não exigências atribuídas ao professor: nomes de plataforma/fabricante/gênero 100; nome de jogo/usuário 150; condições/perfis 30; e-mail 254; hash 512; descrição de jogo TEXT. Identificadores, ano e estoque usam INT com sinal. Isso mantém a validação de faixas no Service. Ajuste somente se o professor fornecer dicionário mais detalhado.

Os demais tipos explicitamente exigidos são DECIMAL(10,2), DATETIME e BOOLEAN DEFAULT TRUE. Somente ds_jogo aceita NULL. Datas são UTC. Não há CHECK, trigger, function nem ação ON DELETE. SenhaHash é string; nenhum usuário ou senha é semeado.

As consultas com JOIN em database/003_consultas.sql são exercícios SQL executados diretamente, não código Repository. Nos repositórios futuros, cada consulta lerá uma única tabela e a composição será feita no Service com LINQ, conforme AGENTS.md.

O front-end preserva uma tela inicial simples Vite/React com contador para verificar montagem e interação. A implementação completa das telas e autenticação está fora da Aula 1. Os pacotes usam as versões de referência fornecidas; restore executado. Não foram adicionados ORM, TypeScript, Tailwind, JWT ou AutoMapper.

Um script numerado altera estrutura; os scripts de dados/consultas são separados. 001 e 002 devem rodar uma vez em schema vazio; não usam IF NOT EXISTS ou UPSERT para esconder uma execução repetida. Nenhum script apaga automaticamente o banco.

O projeto xUnit inclui Microsoft.NET.Test.Sdk 17.14.1 e xunit.runner.visualstudio 3.0.0 como infraestrutura de descoberta para o comando dotnet test pedido no roteiro; não são camadas adicionais nem ferramentas alternativas à stack. Restore e compilação verificados com SDK 10.0.403.
