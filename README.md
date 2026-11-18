# RetroGames — Aulas 1 e 2

Projeto integrado com a base da Aula 1 e a camada Repository da Aula 2. Comece por docs/AULA2.md para configurar a conexão local e conferir os contratos dos repositórios.

## Conteúdo

- Seis projetos .NET 10 e referências conforme a seção 3.2.
- React 19 + Vite 8 em JavaScript, com Bootstrap e proxy /api → localhost:5080.
- Sete tabelas, 7 plataformas (Sega Saturn sem jogos), 8 gêneros e 24 jogos, cada um com 1–3 gêneros.
- NBA Jam inativo; Tetris com estoque zero; usuários vazios.
- Cinco consultas solicitadas e cinco testes de integridade isolados com rollback.
- Sete modelos com comentário de coluna em cada propriedade; constantes de condições e perfis.
- Cinco repositórios com interfaces, RepositoryBase, RepositoryConfig, RegistroDuplicadoException e RepositorySet.
- Modelo .env.example, SQL de conferência da Aula 2 e relatório de validação.
- Seis cartões locais, decisões técnicas e histórico Git das duas aulas.

## 1. Verificar ambiente

Instale .NET SDK 10, Node.js 22 LTS (22.12 ou superior), MySQL 8 + Workbench, Git e VS Code com C# Dev Kit. No terminal:

```sh
dotnet --version
node -v
mysql --version
git --version
```

## 2. Compilar e abrir

Na raiz extraída:

```sh
dotnet restore RetroGames.sln
dotnet build RetroGames.sln
dotnet test RetroGames.sln
dotnet run --project RetroGames.WebApi
```

API local: http://localhost:5080; Swagger: http://localhost:5080/swagger. Nesta aula, nenhum controller de negócio foi criado, portanto a documentação ainda não tem operações. O projeto xUnit está preparado, mas não contém testes de regras futuras; `dotnet test` não comprova os testes SQL. Para o runner nativo v3: `dotnet run --project RetroGames.WebApi.UnitTests`.

Em outro terminal:

```sh
cd retrogames.web
npm install
npm run build
npm run dev
```

Abra http://localhost:5173 e clique no contador. O package-lock.json está incluído. npm install e npm run build passaram; o servidor Vite respondeu por HTTP na porta 5173. O teste no Chromium confirmou que o botão incrementa o contador de 0 para 1.

## 3. Criar e popular o banco

No Workbench, conecte ao seu MySQL com suas próprias credenciais. Crie um schema novo chamado retrogames_aula1 com charset utf8mb4 e defina-o como padrão. Não reutilize um banco com tabelas/dados existentes.

Execute, em ordem:

1. database/001_estrutura.sql
2. database/002_dados_exemplo.sql
3. database/003_consultas.sql
4. database/004_testes_integridade.sql — um bloco por vez; execute ROLLBACK após cada erro.
5. database/005_conferencia.sql

Os comentários de 004 registram os resultados observados no MySQL 8.0.41. Para testar recriação, use outro schema vazio e repita 001/002; isso comprova a reexecução em banco vazio sem apagar seu trabalho. Confira também tb_jogo no Table Inspector.

A Aula 2 prepara a conexão em .env local, conforme a orientação escolhida pelo estudante. Copie .env.example para .env e preencha os dados do seu MySQL. Confira docs/AULA2.md. AdminInicial será configurado na aula de autenticação.

## 4. Entender o código

Model/Model contém classes que espelham as colunas, sem consultas ou regras de negócio. `decimal` conserva valores monetários; `DateTime` representa DATETIME; `bool` representa st_ativo; apenas `Descricao` de Jogo é anulável. As constantes repetem as chaves fixas inseridas no SQL.

A cadeia futura será Controller → Service → Repository. Os projetos já possuem as referências corretas. Program.cs registra controllers/OpenAPI e só oferece Swagger em desenvolvimento. Os exercícios SQL usam JOIN diretamente no Workbench; os repositórios futuros deverão combinar dados no Service usando LINQ.

## 5. Histórico e GitHub

O ZIP inclui o repositório Git local na branch aula-2. O histórico conserva os seis commits da Aula 1, seu commit de validação e acrescenta um commit por etapa da Aula 2. O remote depende da sua conta GitHub. Não há autenticação GitHub armazenada. Confira:

```sh
git log --oneline
git status
```

Se o programa de extração ocultar .git, extraia todos os arquivos. Depois de executar as verificações, crie no GitHub um repositório vazio RetroGames, copie a URL fornecida pelo próprio GitHub e use `git remote add origin URL_COPIADA` (substitua o marcador). Envie com `git push -u origin aula-2`. Não renomeie/integre à main antes de compilar e verificar.

Crie o quadro remoto usando docs/QUADRO_TAREFAS.md. O quadro registra o avanço da camada Repository nas funcionalidades correspondentes.

## Situação da entrega

Solução compilada e front-end testado em navegador. Os cinco scripts SQL foram executados em dois schemas vazios no MySQL 8.0.41. Os resultados da Aula 1 estão em docs/VALIDACAO.md e os da Aula 2 em docs/VALIDACAO_AULA2.md. A publicação do repositório e do quadro no GitHub e a conferência no Workbench da máquina do estudante seguem pendentes.
