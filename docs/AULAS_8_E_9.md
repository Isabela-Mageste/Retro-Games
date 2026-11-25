# Aulas 8 e 9 — Login, administração e testes unitários

## Aula 8 — Front-end: login e administração (`retrogames.web/src`)

| Etapa | Arquivos |
|---|---|
| 1. Sessão no front-end | `api/servicos.js` (`autenticacaoApi`), `api/api.js` (interceptor: 401 fora de `/eu` e `/login` dispara `sessao:expirada`), `contexts/autenticacaoContext.js`, `contexts/AutenticacaoProvider.jsx`, `hooks/useAutenticacao.js`, `main.jsx` |
| 2. Login e menu | `pages/Login/Login.jsx` (rota `/login`, `voltarPara` só aceita caminhos internos), `components/Layout.jsx` (Entrar; "Olá, nome" e Sair; menu Administração só para o administrador) |
| 3. Rotas protegidas | `components/RotaProtegida.jsx` ("Verificando sessão..." no F5), grupo `/admin` no `App.jsx`, `obterErrosDosCampos` em `api/erros.js`, gravações em `api/servicos.js` |
| 4. Plataformas | `pages/Auxiliares/Plataforma.jsx` (`/admin/plataformas`) |
| 5. Jogos | `pages/Administracao/Jogos.jsx` (`/admin/jogos`, com `incluirInativos: true`) e `pages/Administracao/FormularioJogo.jsx` (`/admin/jogos/novo` e `/admin/jogos/:id/editar`) |

A identidade visual (tema escuro rosa e azul) e as heurísticas de Nielsen estão em `docs/INTERFACE.md`.

## Aula 9 — Testes unitários (`RetroGames.WebApi.UnitTests/Services`)

`PlataformaServiceTests`, `JogoServiceTests`, `GeneroServiceTests` e `CondicaoJogoServiceTests`, com os
repositórios simulados por Moq. A lista de cenários e o resultado da "rede de segurança" (etapa 4) estão em
`RetroGames.WebApi.UnitTests/README.md`.

## Verificação executada em 25/11/2026

| Verificação | Resultado |
|---|---|
| `dotnet build RetroGames.sln` | 0 avisos, 0 erros |
| `dotnet test RetroGames.sln` | 46 aprovados, 0 falhas |
| Rede de segurança: conferência de nome duplicado comentada | 1 falha (`Inserir_NomeJaExiste_LancaNomeDuplicado`); restaurada, 46 aprovados |
| `npm run build` | Concluído sem erros |
| `bash docs/conferencia_api.sh` | 61 OK, 0 FALHA |
| Sem login | `GET /api/Autenticacao/eu` responde 401 e o catálogo funciona; menu sem Administração; nada no localStorage nem no sessionStorage; o cookie não é visível ao JavaScript |
| Login | Senha errada mostra "E-mail ou senha inválidos." e fica na tela; e-mail mal formado mostra o erro embaixo do campo; login certo leva ao catálogo com "Olá, Administrador" e Sair; o F5 mantém o login |
| Sair | Volta ao catálogo como visitante; `GET eu` volta a responder 401 |
| `/admin/jogos` sem login | Vai para `/login?voltarPara=%2Fadmin%2Fjogos`; depois de entrar, volta para `/admin/jogos`; o F5 logado continua na página |
| Sessão encerrada com a tela aberta | A próxima gravação recebe 401 e o site leva ao login, com `voltarPara` |
| Plataformas | Nome vazio e ano 1800 mostram o erro embaixo de cada campo; "NES" repetido mostra a mensagem do 101; excluir NES mostra a mensagem do 102 e ela continua na lista; incluir, editar (linha atualizada sem recarregar a página) e excluir funcionam |
| Jogos | Preço zero e nenhum gênero marcado mostram o erro no campo certo; inclusão volta à lista com aviso; a edição mostra os dados atuais com os gêneros marcados; desativar deixa o jogo como Inativo na lista e o tira do catálogo público |
| Celular (375 px) | Menu em botão; tabelas sem rolagem horizontal da página; formulário em uma coluna |

Os jogos e plataformas criados nos testes foram removidos do banco local ao final.
