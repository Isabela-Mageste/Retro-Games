# Aulas 3 a 7 — Service, API, autenticação e front-end do catálogo

Este guia resume o que cada aula acrescentou, onde está o código e como conferir cada item dos roteiros.

## Aula 3 — Service (`RetroGames.Services`)

| Etapa | Arquivos |
|---|---|
| 1. Erros de negócio | `BusinessErrors.cs` (códigos 100–105, mensagem e `HttpStatusCode`), `BusinessRuleException.cs` |
| 2. Plataformas | `IPlataformaService.cs`, `PlataformaService.cs` — RN01 (nome único, inclusive `RegistroDuplicadoException`) e RN02 |
| 3. Gêneros e condições | `IGeneroService`/`GeneroService`, `ICondicaoJogoService`/`CondicaoJogoService` |
| 4. Jogos | `ServiceModels/` (`JogoDetalheServiceModel`, filtro e página), `IJogoService`, `JogoService` |

Pontos de atenção no `JogoService`:

- `MontarDetalhesAsync` faz sempre 4 consultas (plataformas, condições, gêneros e vínculos da página), qualquer que seja o tamanho da página, e combina tudo com `join` do LINQ.
- O filtro por gênero lê `tb_jogo_genero`, escolhe os ids com LINQ e entrega a lista ao SQL, que continua contando e paginando.
- Inserir e alterar gravam jogo e vínculos dentro de `TransactionScope`, com `Complete()` só depois da última gravação.
- Desativar faz só `UPDATE st_ativo` (RN03). Alterar alcança jogos inativos, o que permite reativá-los com `"ativo": true`.

## Aula 4 — API, parte 1 (leitura)

| Etapa | Arquivos |
|---|---|
| 1. API no ar | `Program.cs` (DotNetEnv, para com mensagem clara sem a connection string), `Configuration/InjecaoDependenciaConfig.cs` |
| 2. Envelope e plataformas | `ResponseCodes.cs`, `RetroGamesResponseApiModel.cs`, `ListarPlataformaResponseApiModel.cs`, `Mappers/PlataformaMapper.cs`, `Controllers/PlataformaController.cs` |
| 3. Erros globais | `Middlewares/GlobalExceptionHandler.cs` |
| 4. Gêneros e condições | `GeneroController`, `CondicaoJogoController`, ApiModels e mappers |
| 5. Jogos | `JogoController`, `JogoMapper`, `ListarJogoRequestApiModel`, `ListarJogoResponseApiModel`, `ListarPorIdJogoResponseApiModel`, `PaginaResponseApiModel<T>` |

## Aula 5 — API, parte 2 (gravação)

| Etapa | Arquivos |
|---|---|
| 1. Validação | `Filters/FluentValidationFilter.cs`, `Filters/RespostasDeValidacao.cs`, `Configuration/ValidacaoConfig.cs`, `Validators/ListarJogoRequestValidator.cs`, códigos 2 e 3 |
| 2. Plataformas | `Inserir/AlterarPlataformaRequestApiModel`, validators, POST (201 + `Location`), PUT e DELETE (200 com envelope) |
| 3. Jogos | `Inserir/AlterarJogoRequestApiModel`, validators (RN06 e formato da RN05), POST, PUT e DELETE lógico |

JSON malformado ou texto num campo numérico também responde 400 com código 3 (a mensagem técnica do conversor não vai para o cliente). Corpo vazio responde 400 com código 2.

## Aula 6 — Autenticação e administração

| Etapa | Arquivos |
|---|---|
| 1. Usuários e senhas | `IUsuarioRepository`/`UsuarioRepository`, `Model/Constantes/Perfis.cs`, `IAutenticacaoService`/`AutenticacaoService`, administrador inicial no `Program.cs`, `.env.example` |
| 2. Sessão e login | Códigos 4–6, `Configuration/AutenticacaoConfig.cs` (cookie `retrogames.sessao`), `Extensions/ClaimsPrincipalExtensions.cs`, `LoginRequestApiModel`, `UsuarioLogadoResponseApiModel`, `LoginRequestValidator`, `AutenticacaoController` |
| 3. Proteção | `Configuration/Politicas.cs`, `Configuration/AutorizacaoConfig.cs` (FallbackPolicy + SomenteAdministrador), `IUsuarioAtualService` (Services), `Services/UsuarioAtualService.cs` (WebApi), RN04 no `JogoService` |

O `PasswordHasher` vem do pacote `Microsoft.Extensions.Identity.Core`, referenciado só pelo projeto Services: ele traz o hash sem trazer nada de HTTP.

## Aula 7 — Front-end: catálogo (`retrogames.web`)

| Etapa | Arquivos |
|---|---|
| 1. Projeto | `vite.config.js` (porta 5173, proxy `/api` → 5080), `main.jsx`, `App.jsx` |
| 2. Acesso à API | `src/api/api.js` (único que importa o Axios), `servicos.js`, `erros.js`, `src/lib/formato.js` |
| 3. Layout e rotas | `components/Layout.jsx`, `components/MensagensEstado.jsx`, `pages/NaoEncontrada.jsx`, rotas no `App.jsx` |
| 4. Catálogo | `pages/Catalogo/Catalogo.jsx` (filtros na URL com `useSearchParams`), `components/CardJogo.jsx`, `components/Paginacao.jsx` |
| 5. Detalhe | `pages/Catalogo/DetalheJogo.jsx` |

## Verificação executada em 24/11/2026

Ambiente: .NET SDK 10.0.404, Node.js 22.23.5, MySQL Community Server 8.4.10 num schema novo com `001` e `002`.

| Verificação | Resultado |
|---|---|
| `dotnet build RetroGames.sln` | 0 avisos, 0 erros |
| `dotnet test RetroGames.sln` | 42 testes aprovados, 0 falhas (Services, validators, controller e handler global; veja `RetroGames.WebApi.UnitTests/README.md`) |
| `npm ci` e `npm run build` | Build concluído (os avisos `"use client"` vêm do pacote react-router) |
| Swagger e `/openapi/v1.json` sem login | 200 |
| GET plataformas, gêneros, condições, jogos | 200 com envelope; 23 jogos ativos, 2 páginas de 12 |
| Busca `zelda`, plataforma 2 + gênero 3, página 2 com tamanho 5 | Resultados corretos e páginas sem repetição |
| GET `/api/Plataforma/99`, `/api/Jogo/9999`, jogo inativo sem login | 404 com 100 / 103 / 103 |
| Banco parado | 500, código 1, mensagem genérica; exceção completa só no terminal |
| `pagina=0`, `pagina=abc`, POST inválido, JSON malformado, corpo vazio | 400 com 3, 3, 3, 3 e 2 |
| POST plataforma (admin) | 201 com `Location: /api/Plataforma/8` |
| Nome duplicado no POST e no PUT; manter o próprio nome | 409 (101); 200 |
| DELETE plataforma com jogos / nova / 99 | 409 (102) / 200 / 404 (100) |
| POST jogo com gênero, plataforma ou condição inexistente | 404 com 105, 100, 104 |
| POST jogo válido com gênero repetido | 201; vínculos sem duplicidade |
| PUT jogo trocando gêneros e reativando | 200; `tb_jogo_genero` com os novos vínculos |
| DELETE jogo | 200; `st_ativo = 0`, linha preservada |
| POST plataforma sem login / GET eu sem login | 401 com 4 |
| Login senha errada / e-mail mal formado / correto | 401 (6) / 400 (3) / 200 com cookie `HttpOnly; SameSite=Strict` |
| `incluirInativos=true` sem login / com admin | 23 / 24 jogos |
| Logout e depois GET eu | 200 e 401 (4) |
| Administrador inicial | Hash de 84 caracteres; reiniciar a API não cria outro; a senha não aparece no log |
| API sem `ConnectionStrings__Default` | Para na subida com mensagem clara (código de saída 1) |
| Navegador em `localhost:5173` | Catálogo, filtro de plataforma na URL, Voltar, página 2, `/jogos/22` (Tetris esgotado, sem "null"), `/jogos/9999` ("Jogo não encontrado."), `/qualquer-coisa` e busca vazia |
| Tela de 375 px | O menu vira botão e abre ao clicar; sem rolagem horizontal |
| Front-end com a API parada | Mensagem "Não foi possível conectar ao servidor." com Tentar novamente, sem quebrar a tela |

O jogo criado nos testes foi removido do banco ao final. O 403 (código 5) depende de um usuário sem perfil de administrador, que só existirá no Módulo de Compras (roteiro da Aula 6, etapa 4).
