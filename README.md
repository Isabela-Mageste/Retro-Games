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

## Início rápido (Windows)

Com o banco criado (passo 1) e o `.env` preenchido (passo 2), dê dois cliques em **`iniciar.cmd`**, na raiz.
Ele sobe, nesta ordem, o MySQL (se ainda não estiver rodando), a API e o front-end, cada um na sua janela, e
abre http://localhost:5173. Para parar, feche as três janelas. Os passos 3 e 4 abaixo fazem o mesmo à mão.

### "Não foi possível conectar ao servidor." em todas as telas

Essa mensagem significa que o front-end não alcançou a API. Confira, na ordem:

1. **O MySQL está rodando?** A API não sobe sem o banco: ela para na subida ao criar o administrador inicial.
2. **A API está no ar?** Abra http://localhost:5080/swagger. Se não abrir, veja a mensagem na janela da API
   (falta de linha no `.env`, banco fora do ar, porta 5080 ocupada).
3. **O front-end foi aberto pelo Vite (`npm run dev`)?** Abrir os arquivos de `dist/` direto não funciona: o
   repasse de `/api` para a porta 5080 é feito pelo servidor do Vite.
4. **`dotnet` ou `npm` "não é reconhecido"?** As ferramentas não estão no PATH; use o `iniciar.cmd`, que as
   procura também nas pastas de instalação por usuário.
5. **No PowerShell, `npm` diz que "a execução de scripts foi desabilitada"?** É a política de scripts padrão do
   Windows bloqueando o `npm.ps1`. Use `npm.cmd` no lugar de `npm` (por exemplo, `npm.cmd run dev`), rode no
   Prompt de Comando ou no Git Bash, ou libere scripts locais só para o seu usuário com
   `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser`.

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
