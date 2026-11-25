# Sprint — RetroGames (trabalho individual)

O trabalho é individual: a mesma pessoa acumula os papéis de Product Owner e de desenvolvimento, e é a
responsável por todas as tarefas. Pelo tamanho do grupo (1 estudante), a versão do desafio é a **básica (C01 a C05)**.

O quadro oficial fica no **GitHub Projects** do repositório, compartilhado com o professor. Este arquivo é a cópia
versionada do mesmo conteúdo; `docs/cartoes.csv` tem os cartões prontos para cadastrar.

## Como montar o quadro no GitHub Projects

1. No repositório no GitHub, abra **Projects → New project → Board** e dê o nome *RetroGames — Sprint Módulo de Compras*.
2. Crie as colunas **A fazer**, **Em andamento** e **Concluído**.
3. Cadastre um cartão por história (C01 a C05) com o texto e os critérios de aceite abaixo, e um cartão por tarefa.
4. Em **Settings → Manage access**, convide o professor (leitura), para que ele abra o quadro sem pedir acesso.
5. Mova os cartões a cada avanço e cite o commit no comentário do cartão.

## Sprint: Módulo de Compras

- **Objetivo:** o cliente se cadastra, compra e acompanha os pedidos.
- **Datas:** início em 24/11/2026, fim em 29/11/2026 (entrega). Apresentação em 01/12/2026.
- **Versão:** básica (C01 a C05).

### Histórias de usuário

| Código | História | Critérios de aceite |
|---|---|---|
| C01 | Como **visitante**, quero **criar minha conta com nome, e-mail e senha**, para **comprar na loja**. | `POST /api/Autenticacao/cadastrar` é público. Nome obrigatório (até 150), e-mail válido, senha com 8+ caracteres, letra e número (400, código 3). E-mail já usado: 409, código 106. Cria usuário ativo com perfil Cliente e senha em hash (RS04). Após o cadastro, a sessão já começa (201). Tela `/cadastro` com link na tela de Login. |
| C02 | Como **cliente logado**, quero **ver Carrinho e Meus pedidos no menu**, para **chegar rápido às minhas compras**. | Logado: o menu mostra Carrinho (com contador) e Meus pedidos. O cliente não vê o menu Administração. |
| C03 | Como **cliente**, quero **montar um carrinho**, para **escolher o que vou comprar antes de pagar**. | Adicionar a partir do detalhe do jogo (não para jogo esgotado); alterar quantidade de 1 a 10; remover itens; ver o total. O carrinho fica no navegador (localStorage), sem token de sessão (RS01 continua valendo). `/carrinho` exige login: sem sessão, vai ao Login e volta depois. |
| C04 | Como **cliente**, quero **finalizar a compra**, para **garantir os jogos do carrinho**. | `POST /api/Pedido` só com sessão (401, código 4). Pedido com ao menos um item e quantidade de 1 a 10 (400, código 3). Jogo inexistente ou inativo: 400, código 110. Estoque insuficiente: 409, código 111. Preço vem do banco, nunca do front. Pedido nasce Pendente, com data em UTC e total calculado no Service. Pedido, itens e baixa de estoque numa só transação: uma compra recusada não grava nada. |
| C05 | Como **cliente**, quero **ver meus pedidos**, para **acompanhar o que comprei**. | `GET /api/Pedido/meus` exige login e lista só os pedidos do cliente logado, do mais recente para o mais antigo, com situação, total e itens (jogo, quantidade e preço pago). Tela `/meus-pedidos`. |

### Tarefas

Responsável por todas as tarefas: quem faz o trabalho individual (grupo de uma pessoa).

| História | Tarefa | Camada | Situação | Commit |
|---|---|---|---|---|
| C04 | Tabelas `tb_situacao_pedido`, `tb_pedido`, `tb_item_pedido` e situações fixas | Banco (`010`, `011`) | Concluído | `7f38117` |
| C04 | Modelos `SituacaoPedido`, `Pedido`, `ItemPedido` e `SituacaoPedidoIds` | Model | Concluído | `7f38117` |
| C01 | `CadastrarClienteAsync` e erro 106 | Service | Concluído | `3bdbc12` |
| C01 | `POST /api/Autenticacao/cadastrar`, ApiModel e validator | API | Concluído | `3bdbc12` |
| C01 | Testes do cadastro (Service e validator) | Testes | Concluído | `3bdbc12` |
| C01 | Tela de cadastro com regras da senha visíveis | Tela | Concluído | `3bdbc12` |
| C04 | Repositórios de pedido, itens e situações; `ListarPorIdsAsync` e `BaixarEstoqueAsync` | Repository | Concluído | `dabdb2c` |
| C04 | `PedidoService.FinalizarAsync` com transação e erros 110 e 111 | Service | Concluído | `dabdb2c` |
| C05 | `PedidoService.ListarMeusAsync` com LINQ | Service | Concluído | `dabdb2c` |
| C04/C05 | `PedidoController` (`POST /api/Pedido`, `GET /api/Pedido/meus`), validator e mapper | API | Concluído | `dabdb2c` |
| C04/C05 | `PedidoServiceTests` e `PedidoValidatorsTests` | Testes | Concluído | `dabdb2c` |
| C04 | Consultas guardadas para o Workbench | Banco (`012`) | Concluído | `dabdb2c` |
| C03 | `CarrinhoProvider` (localStorage) e botão no detalhe do jogo | Tela | Concluído | `d4a57a7` |
| C02 | Menu com Carrinho (contador) e Meus pedidos | Tela | Concluído | `9f0387e` |
| C03/C04 | Tela do carrinho com resumo e confirmação do pedido | Tela | Concluído | `ef28218` |
| C05 | Tela Meus pedidos | Tela | Concluído | `ef28218` |
| — | Capa ilustrada, ano em destaque e descrições dos jogos | Tela e banco (`009`) | Concluído | `49f8e40` |
| — | Conferência automática das regras (`docs/conferencia_api.sh`) e documentação | Docs | Concluído | commit da documentação |

### Revisão da sprint (planejado × entregue)

- **Entregue:** C01, C02, C03, C04 e C05, todas Concluídas, com testes unitários do PedidoService (e do cadastro)
  cobrindo cada regra e o caminho que dá certo. Conferido de ponta a ponta no navegador, a 360 px e no computador.
- **Verificado no banco:** a compra recusada por estoque insuficiente não gravou pedido, itens nem baixa de estoque;
  duas compras simultâneas da última unidade resultaram em um 201 e um 409, com o estoque em 0 (não negativo).
- **Não entregue, por escopo:** C06 a C08 são da versão completa (grupos de 3 ou mais) e não fazem parte desta
  versão. "Favoritos" não está no documento do desafio e não foi implementado.
- **Pendência fora do código:** criar o quadro no GitHub Projects e compartilhar com o professor (passos acima).

## Sprint anterior: Aulas 8 e 9

| ID | Item | Critério de aceite | Estado |
|---|---|---|---|
| US01–US05 | Catálogo, detalhe, plataformas, jogos e domínios | Aulas 2 a 8 | Concluído |
| US06 | Autenticação e autorização | Login, F5 e Sair; /admin leva ao login | Concluído |
| T01 | Testes unitários dos Services | Cada regra com teste; `dotnet test` sem falhas | Concluído |
| T02 | Identidade visual e responsividade | Tema escuro rosa e azul; heurísticas de Nielsen | Concluído |
