# Quadro de tarefas — escopo do módulo

Este quadro local contém os cartões da seção 1.2 da especificação e os itens técnicos da sprint. O quadro
remoto (GitHub Projects) deve espelhar estes estados; veja `docs/SPRINT.md` para montá-lo e compartilhá-lo com
o professor.

| ID | Cartão | Estado | Critérios de aceite |
|---|---|---|---|
| US01 | Catálogo público | Concluído | Busca textual, filtros por plataforma e gênero, paginação; apenas jogos ativos (RN04). API (Aula 4) e tela (Aula 7). |
| US02 | Detalhe de jogo | Concluído | Exibir jogo, plataforma, condição e gêneros; inativo dá 404 para o público (RN04). API (Aula 4) e tela (Aula 7). |
| US03 | Manutenção de plataformas | Concluído | API com RN01–02 e acesso só de administrador (Aulas 5 e 6); tela `/admin/plataformas` (Aula 8). |
| US04 | Manutenção de jogos | Concluído | API com gêneros N:N, RN03, RN05 e RN06 (Aulas 5 e 6); telas `/admin/jogos` e formulário (Aula 8). |
| US05 | Consulta de domínios | Concluído | GET de gêneros e condições de jogo (Aula 4), usados nos filtros e formulários. |
| US06 | Autenticação e autorização | Concluído | Login, logout, eu, cookie HttpOnly/SameSite=Strict, hash de senha, FallbackPolicy e SomenteAdministrador (Aula 6); login, menu e rotas protegidas no site (Aula 8). |
| T01 | Testes unitários dos Services | Concluído | PlataformaServiceTests, JogoServiceTests, GeneroServiceTests e CondicaoJogoServiceTests (Aula 9). |
| T02 | Identidade visual e responsividade | Concluído | Tema escuro rosa e azul, celular e computador, heurísticas de Nielsen (`docs/INTERFACE.md`). |

## Histórico

- Aula 2: camada Repository das cinco primeiras funcionalidades.
- Aula 3: Services com as regras de negócio (RN01–RN05) e erros padronizados.
- Aulas 4 e 5: API de leitura e gravação, validação e envelope.
- Aula 6: autenticação, autorização e RN04 por perfil.
- Aula 7: front-end público (catálogo e detalhe).
- Aula 8: login e administração no front-end; identidade visual.
- Aula 9: testes unitários de todos os Services.
