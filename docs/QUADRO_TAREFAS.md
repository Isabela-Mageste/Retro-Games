# Quadro de tarefas — escopo do módulo

Este quadro local contém os seis cartões da seção 1.2. Na Aula 2, as cinco primeiras funcionalidades avançaram para **Em andamento**, com sua camada de acesso a dados implementada. A autenticação permanece **A fazer**. O quadro remoto ainda precisa ser criado no GitHub Projects ou Jira.

| ID | Cartão | Estado | Critérios de aceite |
|---|---|---|---|
| US01 | Catálogo público | Em andamento | Busca textual, filtros por plataforma e gênero, paginação; apenas jogos ativos (RN04). |
| US02 | Detalhe de jogo | Em andamento | Exibir jogo, plataforma, condição e gêneros; respeitar visibilidade (RN04). |
| US03 | Manutenção de plataformas | Em andamento | Incluir, consultar, alterar e excluir; nome único e bloqueio de exclusão com jogos (RN01–02). |
| US04 | Manutenção de jogos | Em andamento | Incluir e alterar, associar gêneros, validar referências/valores e excluir logicamente (RN03–06). |
| US05 | Consulta de domínios | Em andamento | Consultar gêneros e condições de jogo. |
| US06 | Autenticação e autorização | A fazer | Login/logout, sessão, CSRF, administrador, hash de senha e segredos (RS01–06). |

Para o quadro remoto: crie as colunas A fazer, Em andamento e Concluído; cadastre cada linha como cartão. Mova cartões somente após implementar e verificar os critérios. O CSV ao lado contém os mesmos cartões para auxiliar o cadastro; não pressupõe importação nativa pelo GitHub Projects.

## Avanço na Aula 2

- US01 e US02 recebem listagem paginada, busca, filtros, consulta por ID e leitura dos domínios e vínculos.
- US03 recebe os sete métodos de PlataformaRepository.
- US04 recebe gravação, alteração, desativação e manutenção de vínculos.
- US05 recebe GeneroRepository e CondicaoJogoRepository.
- US06 aguarda a aula de autenticação.

Os critérios completos ainda dependem de Service, API e telas nas próximas aulas. A sincronização do quadro remoto está pendente da conexão com GitHub.
