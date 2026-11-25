# RetroGames

Loja web de jogos retrô usados. Visitantes navegam pelo catálogo, clientes compram e acompanham seus pedidos, e
o administrador mantém plataformas e jogos. O sistema tem três partes: uma SPA em React, uma API REST em
ASP.NET Core e um banco MySQL.

## O que o sistema faz

**Visitante**
- Catálogo com busca por nome, filtros por plataforma e gênero e paginação. Os filtros ficam na URL, então o
  link pode ser compartilhado e o botão Voltar funciona.
- Detalhe do jogo: capa, plataforma, ano, gêneros, descrição, condição, estoque e preço.
- Só jogos ativos aparecem.

**Cliente**
- Cria a conta e já entra logado.
- Monta o carrinho a partir do detalhe do jogo (quantidade de 1 a 10, sem passar do estoque). Jogo esgotado não
  pode ser adicionado.
- Finaliza a compra: o pedido nasce **Pendente** e o estoque é baixado.
- Vê em **Meus pedidos** cada pedido com data, situação, total e itens.

**Administrador**
- Inclui, altera e exclui plataformas. Uma plataforma com jogos não pode ser excluída.
- Inclui e altera jogos com seus gêneros, e desativa jogos (o jogo sai do catálogo, mas continua cadastrado e
  pode ser reativado).
- Vê também os jogos inativos na administração.

## Tecnologias

| Parte | Tecnologia |
|---|---|
| API | .NET 10, ASP.NET Core (controllers), FluentValidation, OpenAPI + Swagger UI |
| Acesso a dados | ADO.NET com MySqlConnector, SQL parametrizado |
| Banco | MySQL 8 |
| Front-end | React 19, Vite, React Router, Bootstrap 5, Axios |
| Testes | xUnit v3 e Moq |

## Arquitetura

```
React (5173) ──/api──▶ Controller ─▶ Service ─▶ Repository ─▶ MySQL
                        (HTTP)       (regras)     (SQL)
```

- **Controller:** recebe a requisição, chama o Service e responde no envelope. Não tem regra nem SQL.
- **Validator:** confere o formato da entrada (campos obrigatórios, tamanhos, faixas).
- **Service:** aplica as regras de negócio, abre transações e combina dados de várias tabelas com LINQ.
- **Repository:** um por tabela; cada consulta lê uma única tabela, com parâmetros, filtros e paginação no SQL.

| Pasta | Conteúdo |
|---|---|
| `RetroGames.Model` | Uma classe por tabela e constantes (perfis, condições, situações de pedido) |
| `RetroGames.Repository` | Repositórios ADO.NET e o conjunto `IRepositorySet` |
| `RetroGames.Services` | Services, ServiceModels e o catálogo de erros de negócio |
| `RetroGames.WebApi.ApiModels` | Modelos de entrada e saída da API, envelope e códigos de resposta |
| `RetroGames.WebApi` | Controllers, validators, mappers, autenticação e tratamento global de erros |
| `RetroGames.WebApi.UnitTests` | Testes unitários |
| `retrogames.web` | Front-end React |
| `database` | Scripts SQL numerados |
| `docs` | Documentação complementar |

## Como executar

### Pré-requisitos

.NET SDK 10, Node.js 22 LTS e MySQL 8 em execução.

### 1. Banco de dados

Crie um schema vazio (por exemplo `retrogames`, charset utf8mb4) e execute, nesta ordem:

| Script | O que faz |
|---|---|
| `database/001_estrutura.sql` | Tabelas do catálogo e dos usuários |
| `database/002_dados_exemplo.sql` | 7 plataformas, 8 gêneros, 24 jogos, condições e perfis |
| `database/009_descricoes_jogos.sql` | Descrição de cada jogo de exemplo |
| `database/010_estrutura_compras.sql` | Tabelas de pedido, itens e situações |
| `database/011_dados_compras.sql` | Situações: Pendente, Pago, Enviado e Cancelado |

### 2. Configurar a conexão (`.env`)

Copie `.env.example` para `.env`, na raiz do repositório, e preencha:

| Variável | Valor |
|---|---|
| `ConnectionStrings__Default` | Servidor, porta, schema, usuário e senha do seu MySQL. `database/006_consultar_conexao.sql` mostra servidor, porta, usuário e schema |
| `AdminInicial__Email` | E-mail do administrador, criado na primeira vez que a API sobe |
| `AdminInicial__Senha` | Senha desse administrador |

O `.env` fica fora do Git (`.gitignore`); o repositório guarda só o modelo, sem senhas. Se faltar uma dessas
linhas, a API não sobe e diz qual falta.

### 3. API

```bash
dotnet run --project RetroGames.WebApi
```

Responde em http://localhost:5080. O Swagger fica em http://localhost:5080/swagger.

### 4. Front-end

```bash
cd retrogames.web
npm install
npm run dev
```

Abra http://localhost:5173. O Vite repassa `/api` para a porta 5080, então site e API ficam na mesma origem.

- **Comprar:** Entrar → Criar conta.
- **Administrar:** Entrar com o e-mail e a senha de `AdminInicial`.

### 5. Testes

```bash
dotnet test RetroGames.sln
```

No Windows, depois dos passos 1 e 2, o `iniciar.cmd` faz os passos 3 e 4 de uma vez (veja "Funcionalidades extras").

## API

Toda resposta usa o mesmo envelope:

```json
{ "responseCode": 0, "data": { }, "errorMessage": null }
```

| Método | Rota | Acesso |
|---|---|---|
| POST | `/api/Autenticacao/login`, `/api/Autenticacao/cadastrar` | Público |
| POST | `/api/Autenticacao/logout` | Logado |
| GET | `/api/Autenticacao/eu` | Logado |
| GET | `/api/Plataforma`, `/api/Plataforma/{id}` | Público |
| GET | `/api/Genero`, `/api/CondicaoJogo` | Público |
| GET | `/api/Jogo` (busca, idPlataforma, idGenero, pagina, tamanhoPagina, incluirInativos), `/api/Jogo/{id}` | Público |
| POST, PUT, DELETE | `/api/Plataforma`, `/api/Jogo` | Administrador |
| POST | `/api/Pedido` | Logado |
| GET | `/api/Pedido/meus` | Logado |

### Códigos de resposta

| Código | Status | Significado |
|---|---|---|
| 0 | 200 / 201 | Sucesso |
| 1 | 500 | Erro inesperado (detalhes só no log da API) |
| 2 / 3 | 400 | Corpo vazio / dados inválidos (com os erros por campo em `data`) |
| 4 / 5 | 401 / 403 | Sem login / sem permissão |
| 6 | 401 | E-mail ou senha inválidos |
| 100 / 103 / 104 / 105 | 404 | Plataforma / jogo / condição / gênero não encontrado |
| 101 | 409 | Nome de plataforma repetido |
| 102 | 409 | Plataforma com jogos não pode ser excluída |
| 106 | 409 | E-mail já cadastrado |
| 110 | 400 | Jogo indisponível para venda |
| 111 | 409 | Estoque insuficiente |

## Regras de negócio

- Nome de plataforma é único; plataforma com jogos não pode ser excluída.
- Jogo nunca é apagado: a exclusão só o desativa. Visitantes e clientes veem apenas jogos ativos.
- Todo jogo tem plataforma, condição e ao menos um gênero existentes. O preço é maior que zero, o estoque não é
  negativo e o ano fica entre 1970 e 2100.
- E-mail de usuário é único; a senha tem pelo menos 8 caracteres, com letra e número.
- Pedido: de 1 a 10 unidades por jogo, só jogos ativos e com estoque. O preço e o total vêm do banco, nunca do
  navegador.
- Pedido, itens e baixa de estoque são gravados juntos: se algo falha, nada é gravado. Duas compras simultâneas
  da última unidade não deixam o estoque negativo.

## Segurança

- Sessão em cookie `HttpOnly` e `SameSite=Strict`, com expiração de 2 horas renovada pelo uso. Nenhum token fica
  no navegador.
- Toda rota exige login, exceto as públicas; as rotas de manutenção exigem o perfil Administrador.
- Senhas guardadas só como hash (PasswordHasher).
- Todo valor vai ao banco como parâmetro, nunca concatenado no SQL.
- Credenciais só no `.env`, fora do Git.

## Testes

66 testes unitários, sem banco: os repositórios são simulados com Moq. Eles cobrem as regras de todos os Services
(plataformas, jogos, gêneros, condições, cadastro e pedidos), os validators, um controller e o tratamento global
de erros. Detalhes em `RetroGames.WebApi.UnitTests/README.md`.

## Problemas comuns

| Sintoma | Causa provável |
|---|---|
| "Não foi possível conectar ao servidor." em todas as telas | A API não está no ar. Confira se o MySQL está rodando e veja a mensagem na janela da API |
| A API fecha logo ao iniciar | Falta uma linha no `.env` ou o banco está fora do ar |
| `npm` bloqueado no PowerShell ("execução de scripts foi desabilitada") | Use `npm.cmd run dev` ou rode `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser` |
| `dotnet` ou `npm` "não é reconhecido" | A ferramenta não está no PATH; use o `iniciar.cmd` |

## Funcionalidades extras

Além do escopo da loja, o projeto tem:

**Na loja**
- **Capa dos jogos:** cada jogo mostra uma capa desenhada pelo próprio site (SVG com nome, plataforma, ano e
  gênero), sem usar imagens de terceiros. Para trocar por uma imagem real, basta colocar o arquivo em
  `retrogames.web/public/capas/` e registrá-lo em `capas.json`, com o crédito exigido pela licença (instruções
  em `public/capas/LEIA-ME.md`). Se a imagem faltar ou não carregar, a capa desenhada volta automaticamente.
- **Descrições e ano em destaque** no detalhe de cada jogo de exemplo.
- **Carrinho mais seguro de usar:** botão **Desfazer** ao remover um item; resumo e confirmação antes de gravar o
  pedido; preço e estoque conferidos de novo com a API ao abrir o carrinho; carrinho igual entre abas abertas.
- **Mensagens específicas:** "Estoque insuficiente para Tetris." em vez de uma mensagem genérica, com a
  quantidade já ajustada ao estoque real.
- **Cadastro guiado:** as regras da senha são marcadas enquanto se digita, com botão para mostrar a senha e campo
  de confirmação.
- **Identidade visual própria:** tema escuro rosa e azul, fonte pixel no logotipo e ícone próprio. Funciona do
  celular (360 px) ao computador. Detalhes em `docs/INTERFACE.md`.
- **Acessibilidade:** link "Pular para o conteúdo", foco visível ao navegar pelo teclado, mensagens anunciadas a
  leitores de tela e contraste conferido.

**Para executar e conferir**
- **`iniciar.cmd` (Windows):** sobe o MySQL (se necessário), a API e o front-end, cada um na sua janela, e abre
  o site. Ele encontra as ferramentas mesmo fora do PATH.
- **`docs/conferencia_api.sh`:** com a API no ar, executa 78 verificações de status e código de resposta
  (catálogo, gravação, login, permissões, cadastro e compras) e mostra OK ou FALHA para cada uma.
- **Scripts de conferência SQL** (`database/007`, `008` e `012`): executam no Workbench os mesmos comandos dos
  repositórios e desfazem tudo no final.

## Documentação

| Arquivo | Conteúdo |
|---|---|
| `docs/ESPECIFICACAO_COMPRAS.md` | Tabelas, rotas, regras e rastreabilidade do módulo de compras |
| `docs/INTERFACE.md` | Identidade visual, responsividade e heurísticas de Nielsen |
| `docs/SPRINT.md` | Planejamento da sprint, histórias de usuário e revisão |
| `docs/MER.md` | Modelo entidade-relacionamento |
| `docs/DECISOES.md` | Decisões de modelagem |
