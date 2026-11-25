# Especificação — atualização com o Módulo de Compras (versão básica)

Complementa a especificação **RetroGames — Arquitetura e Stack** (v1.0) com o que foi construído no desafio,
como pede o documento do desafio. As seções seguem a numeração da especificação original.

## 1.2 Funcionalidades (acréscimos)

| Funcionalidade |
|---|
| Cadastro de cliente (C01) |
| Menu do cliente logado (C02) |
| Carrinho de compras no navegador (C03) |
| Finalização da compra (C04) |
| Consulta dos próprios pedidos — Meus pedidos (C05) |

## 4.2 Tabelas do módulo (acréscimos)

Scripts: `database/010_estrutura_compras.sql` (estrutura) e `database/011_dados_compras.sql` (situações fixas).
Sem trigger, function, CHECK nem ação referencial.

| Tabela | Tipo | Colunas | Relacionamentos |
|---|---|---|---|
| `tb_situacao_pedido` | Domínio | `co_situacao_pedido`, `ds_situacao_pedido` (UNIQUE). Dados fixos: 1 Pendente, 2 Pago, 3 Enviado, 4 Cancelado | 1:N com `tb_pedido` |
| `tb_pedido` | Principal | `co_seq_pedido`, `dt_pedido` (UTC), `vl_total` DECIMAL(10,2), `co_usuario`, `co_situacao_pedido` | N:1 com `tb_usuario` e `tb_situacao_pedido` |
| `tb_item_pedido` | Associativa | `co_seq_item_pedido`, `qt_item`, `vl_unitario` (preço no momento da compra), `co_pedido`, `co_jogo`; UNIQUE (`co_pedido`, `co_jogo`) | N:1 com `tb_pedido` e `tb_jogo` |

Todas as colunas são obrigatórias. Também foi acrescentado `database/009_descricoes_jogos.sql`, que só preenche a
coluna opcional `ds_jogo` dos jogos de exemplo (sem mudar a estrutura).

## 5.2 Endpoints (acréscimos)

| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/api/Autenticacao/cadastrar` | Público (`[AllowAnonymous]`) | Cria o cliente e inicia a sessão. Responde 201 com o usuário logado |
| POST | `/api/Pedido` | Usuário logado | Finaliza a compra. Corpo: `{ "itens": [{ "idJogo": 1, "quantidade": 2 }] }`. Responde 201 com o pedido |
| GET | `/api/Pedido/meus` | Usuário logado | Pedidos do usuário logado, do mais recente para o mais antigo |

As duas rotas de pedido não têm atributo: vale a política padrão (FallbackPolicy), que exige login (401, código 4).
O usuário do pedido vem da sessão (`IUsuarioAtualService`), nunca do corpo.

## 5.3 Códigos de erro (acréscimos em BusinessErrors)

| Código · Status | Constante | Mensagem padrão |
|---|---|---|
| 106 · 409 | `EMAIL_JA_CADASTRADO` | Já existe uma conta com este e-mail. |
| 110 · 400 | `JOGO_INDISPONIVEL` | Um ou mais jogos não estão disponíveis para venda. |
| 111 · 409 | `ESTOQUE_INSUFICIENTE` | Estoque insuficiente. |

No 111, a mensagem enviada cita o jogo ("Estoque insuficiente para Tetris."), com o mesmo código e status.
Os códigos 107, 108, 109, 112 e 113 pertencem à versão completa (C06 a C08) e não foram criados.

## 7. Regras de negócio (acréscimos)

| ID | Regra | Implementação |
|---|---|---|
| C01 | E-mail único entre os usuários: conferido no Service e garantido pelo UNIQUE de `ds_email` (o 1062 vira 106). Cliente nasce ativo, com perfil Cliente e senha em hash | `AutenticacaoService.CadastrarClienteAsync`, `CadastrarRequestValidator` |
| C04-a | Pedido com ao menos um item, jogo uma só vez e quantidade de 1 a 10 | `FinalizarPedidoRequestValidator` (400, código 3) |
| C04-b | Jogo inexistente ou inativo não pode ser vendido | `PedidoService` (110) |
| C04-c | Quantidade pedida não pode passar do estoque | `PedidoService` (111) |
| C04-d | Preço de cada item vem do banco; total calculado no Service; pedido nasce Pendente com data em UTC | `PedidoService` |
| C04-e | Pedido, itens e baixa de estoque numa só transação (`TransactionScope`). A baixa usa `qt_estoque >= @quantidade`: se outra compra levou o estoque depois da conferência, nenhuma linha muda, o Service lança 111 e tudo é desfeito | `PedidoService`, `JogoRepository.BaixarEstoqueAsync` |
| C05 | Só os pedidos do usuário logado, do mais recente para o mais antigo, compostos com LINQ (pedidos, itens, jogos e situações em número fixo de consultas) | `PedidoService.ListarMeusAsync`, `PedidoRepository` |

## 7.1 Rastreabilidade (acréscimos)

| Funcionalidade | Endpoints | Regras | Classes principais | Testes |
|---|---|---|---|---|
| Cadastro de cliente | POST /api/Autenticacao/cadastrar | C01 | AutenticacaoController, AutenticacaoService, UsuarioRepository, CadastrarRequestValidator | AutenticacaoServiceTests, AutenticacaoValidatorsTests |
| Menu do cliente e carrinho | — (front-end) | C02, C03 | Layout, CarrinhoProvider, useCarrinho, DetalheJogo, Carrinho | Conferido no navegador |
| Finalizar compra | POST /api/Pedido | C04-a a C04-e | PedidoController, PedidoService, PedidoRepository, ItemPedidoRepository, JogoRepository, FinalizarPedidoRequestValidator | PedidoServiceTests, PedidoValidatorsTests |
| Meus pedidos | GET /api/Pedido/meus | C05 | PedidoController, PedidoService, PedidoRepository, ItemPedidoRepository, SituacaoPedidoRepository | PedidoServiceTests |

## 8. Front-end (acréscimos)

| Arquivo | Responsabilidade |
|---|---|
| `contexts/carrinhoContext.js`, `contexts/CarrinhoProvider.jsx`, `hooks/useCarrinho.js` | Carrinho no localStorage: adicionar, alterar quantidade (1 a 10, sem passar do estoque), remover, desfazer e total |
| `pages/Login/Cadastro.jsx` (`/cadastro`) | Cadastro com regras da senha visíveis e confirmação de senha |
| `pages/Compras/Carrinho.jsx` (`/carrinho`, exige login) | Itens conferidos de novo com a API, resumo, confirmação e finalização |
| `pages/Compras/MeusPedidos.jsx` (`/meus-pedidos`, exige login) | Pedidos com data, situação, total e itens |
| `components/CapaJogo.jsx` | Capa ilustrada (SVG) do jogo, gerada a partir dos dados, sem imagem de terceiros |
| `api/servicos.js` | Ganha `autenticacaoApi.cadastrar` e `pedidoApi` (`finalizar`, `meus`) |
| `lib/formato.js` | Ganha `formatarDataHora` (data UTC da API mostrada como dd/mm/aaaa, hh:mm) |
