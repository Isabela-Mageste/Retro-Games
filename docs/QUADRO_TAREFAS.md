# Quadro de tarefas — escopo do módulo

Este quadro local contém os seis cartões da seção 1.2 da especificação. O quadro remoto (GitHub Projects ou Jira)
deve espelhar estes estados.

| ID | Cartão | Estado | Critérios de aceite |
|---|---|---|---|
| US01 | Catálogo público | Concluído | Busca textual, filtros por plataforma e gênero, paginação; apenas jogos ativos (RN04). API (Aula 4) e tela (Aula 7). |
| US02 | Detalhe de jogo | Concluído | Exibir jogo, plataforma, condição e gêneros; inativo dá 404 para o público (RN04). API (Aula 4) e tela (Aula 7). |
| US03 | Manutenção de plataformas | Em andamento | API completa com RN01–02 e acesso só de administrador (Aulas 5 e 6). Falta a tela de administração (Aula 8). |
| US04 | Manutenção de jogos | Em andamento | API completa com gêneros N:N, RN03, RN05 e RN06 (Aulas 5 e 6). Falta a tela de administração (Aula 8). |
| US05 | Consulta de domínios | Concluído | GET de gêneros e condições de jogo (Aula 4), usados nos filtros do catálogo. |
| US06 | Autenticação e autorização | Em andamento | Login, logout, eu, cookie HttpOnly/SameSite=Strict, hash de senha, FallbackPolicy e SomenteAdministrador (Aula 6). Falta o login no front-end (Aula 8). |

## Histórico

- Aula 2: camada Repository das cinco primeiras funcionalidades.
- Aula 3: Services com as regras de negócio (RN01–RN05) e erros padronizados.
- Aulas 4 e 5: API de leitura e gravação, validação e envelope.
- Aula 6: autenticação, autorização e RN04 por perfil.
- Aula 7: front-end público (catálogo e detalhe).
