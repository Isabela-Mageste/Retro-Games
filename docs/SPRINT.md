# Sprint — RetroGames (trabalho individual)

O trabalho é individual: a mesma pessoa acumula os papéis de Product Owner e de desenvolvimento.
O planejamento e o acompanhamento ficam num quadro do **GitHub Projects** ligado a este repositório,
compartilhado com o professor. Este arquivo é a cópia versionada do mesmo conteúdo; `docs/cartoes.csv` tem os
cartões no formato de colunas, pronto para cadastrar no quadro.

## Como montar o quadro no GitHub Projects

1. No repositório no GitHub, abra **Projects → New project → Board** e dê o nome *RetroGames — Sprint*.
2. Crie as colunas **A fazer**, **Em andamento** e **Concluído**.
3. Cadastre um cartão para cada linha do backlog abaixo (ou importe `docs/cartoes.csv`), com o critério de aceite
   na descrição.
4. Em **Settings → Manage access**, convide o professor com permissão de leitura.
5. Ao concluir um item, mova o cartão e cite o commit no comentário.

## Objetivo da sprint

Entregar a loja completa do Módulo de Catálogo e Administração: catálogo público, login e administração de
plataformas e jogos pelo site, com testes unitários dos Services, interface responsiva e identidade visual própria.

## Backlog

| ID | Item | Aula | Critério de aceite | Estado |
|---|---|---|---|---|
| US01 | Catálogo público | 2–7 | Busca, filtros por plataforma e gênero na URL, paginação, só jogos ativos | Concluído |
| US02 | Detalhe de jogo | 2–7 | Plataforma, condição, gêneros; inexistente ou inativo mostra "Jogo não encontrado." | Concluído |
| US03 | Manutenção de plataformas | 2–5, 8 | Incluir, alterar e excluir pela tela; erros 3, 101 e 102 aparecem | Concluído |
| US04 | Manutenção de jogos | 2–5, 8 | Incluir e alterar com gêneros; desativar mantém o jogo como Inativo | Concluído |
| US05 | Consulta de domínios | 4 | Gêneros e condições nas listas de seleção | Concluído |
| US06 | Autenticação e autorização | 6, 8 | Login, F5 mantém a sessão, Sair; /admin leva ao login; gravação só para administrador | Concluído |
| T01 | Testes unitários dos Services | 9 | Cada regra de negócio com teste; `dotnet test` sem falhas | Concluído |
| T02 | Identidade visual e responsividade | — | Tema escuro rosa e azul; sem rolagem horizontal a 375 px; heurísticas de Nielsen (`docs/INTERFACE.md`) | Concluído |
| T03 | Conferência de ponta a ponta | 4–9 | `docs/conferencia_api.sh` sem falhas; checklists das aulas conferidos no navegador | Concluído |

## Tarefas por item (decomposição usada nos commits)

- **US06 / Aula 8:** sessão no front-end (provider, hook, interceptor de 401) → tela de login e menu →
  rotas protegidas.
- **US03 / Aula 8:** lista com os três estados → formulário de inclusão e edição com erros por campo →
  exclusão com confirmação.
- **US04 / Aula 8:** lista com inativos e desativação → formulário com plataforma, condição e gêneros.
- **T01 / Aula 9:** PlataformaServiceTests → JogoServiceTests → GeneroServiceTests e CondicaoJogoServiceTests →
  teste da rede de segurança.

## Fora desta sprint

Cadastro de cliente, carrinho, pedidos e manutenção de gêneros ficam para o desafio (aulas 9 e 10), como
indica o roteiro da Aula 8. O erro 403 (código 5) só pode ser provocado quando existir usuário cliente.
