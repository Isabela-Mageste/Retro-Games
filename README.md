# RetroGames

Loja web de jogos retrô usados — Módulo de Catálogo e Administração. Projeto do curso, construído aula a aula
seguindo a especificação **RetroGames — Arquitetura e Stack** e os roteiros do estudante.

| Aula | Conteúdo | Guia |
|---|---|---|
| 1 | Banco de dados, solução .NET, modelos | `docs/VALIDACAO.md` |
| 2 | Repository (ADO.NET + MySqlConnector) | `docs/AULA2.md` |
| 3 | Service: regras de negócio, LINQ e transações | `docs/AULAS_3_A_7.md` |
| 4 | API de leitura, envelope e tratamento global de erros | `docs/AULAS_3_A_7.md` |
| 5 | Validação (FluentValidation) e gravação | `docs/AULAS_3_A_7.md` |
| 6 | Login com cookie, hash de senha e autorização | `docs/AULAS_3_A_7.md` |
| 7 | Front-end React: catálogo e detalhe do jogo | `docs/AULAS_3_A_7.md` |
| 8 | Front-end: login, menu, rotas protegidas e administração | `docs/AULAS_8_E_9.md` |
| 9 | Testes unitários dos Services (xUnit + Moq) | `docs/AULAS_8_E_9.md` |

Interface (identidade visual, celular e heurísticas de Nielsen): `docs/INTERFACE.md`. Planejamento da sprint:
`docs/SPRINT.md`.

## Pré-requisitos

.NET SDK 10, Node.js 22 LTS (22.12 ou superior) e MySQL 8 em execução.

```sh
dotnet --version
node -v
```

## 1. Banco de dados

No MySQL Workbench, crie um schema vazio (por exemplo `retrogames`, charset utf8mb4) e execute, nesta ordem,
`database/001_estrutura.sql` e `database/002_dados_exemplo.sql`.

## 2. Arquivo .env

Copie `.env.example` para `.env`, na raiz do repositório, e preencha:

- `ConnectionStrings__Default`: servidor, porta, schema, usuário e senha do seu MySQL (`database/006_consultar_conexao.sql` mostra esses dados);
- `AdminInicial__Email` e `AdminInicial__Senha`: o administrador criado na primeira subida da API.

O `.env` está no `.gitignore` e nunca vai para o GitHub (`git check-ignore .env` deve mostrar `.env`).

## 3. API

```sh
dotnet run --project RetroGames.WebApi
```

A API responde em http://localhost:5080 e o Swagger fica em http://localhost:5080/swagger. Se faltar alguma
linha no `.env`, a API para na subida explicando o que falta.

## 4. Front-end

Em outro terminal:

```sh
cd retrogames.web
npm install
npm run dev
```

Abra http://localhost:5173. Para administrar, clique em **Entrar** e use o e-mail e a senha de `AdminInicial`
do seu `.env`. O Vite repassa `/api` para a porta 5080, então front-end e API têm a mesma origem
(sem CORS, com o cookie `SameSite=Strict`).

## 5. Testes e build

```sh
dotnet build RetroGames.sln
dotnet test RetroGames.sln
cd retrogames.web && npm run build
```

Com o banco e a API no ar, `bash docs/conferencia_api.sh` (Git Bash) provoca cada resposta pedida nos
roteiros das Aulas 4 a 6 e mostra OK ou FALHA para o status e o `responseCode` de cada uma. No Workbench,
`database/007_verificar_repository.sql` (Aula 2) e `database/008_verificar_usuario.sql` (Aula 6) conferem
o SQL dos repositórios, desfazendo as gravações no final.

## Rotas da API

| Método | Rota | Acesso |
|---|---|---|
| POST | `/api/Autenticacao/login` | Público |
| POST | `/api/Autenticacao/logout`, GET `/api/Autenticacao/eu` | Autenticado |
| GET | `/api/Plataforma`, `/api/Plataforma/{id}`, `/api/Genero`, `/api/CondicaoJogo`, `/api/Jogo`, `/api/Jogo/{id}` | Público |
| POST, PUT, DELETE | `/api/Plataforma`, `/api/Jogo` | Administrador |

Toda resposta usa o envelope `{ "responseCode", "data", "errorMessage" }`. Os códigos 0–6 estão em
`RetroGames.WebApi.ApiModels/ResponseCodes.cs` e os erros de negócio 100–105 em `RetroGames.Services/BusinessErrors.cs`.
