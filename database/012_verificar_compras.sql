-- Desafio, Módulo de Compras. Conferência no Workbench dos comandos dos repositórios de pedido.
-- Selecione o schema do RetroGames (com 001, 002, 010 e 011 executados). Funciona também num banco novo,
-- sem usuários: o cliente usado no teste é criado aqui e desfeito no ROLLBACK.
-- As variáveis @... equivalem aos parâmetros dos comandos C#. As gravações são desfeitas no final.
SET NAMES utf8mb4;
START TRANSACTION;

-- SituacaoPedidoRepository.ListarAsync
SELECT co_situacao_pedido, ds_situacao_pedido FROM tb_situacao_pedido ORDER BY co_situacao_pedido;

-- JogoRepository.ListarPorIdsAsync (inclui inativos: quem decide é o PedidoService)
SET @id0 = 1; SET @id1 = 24;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
FROM tb_jogo WHERE co_seq_jogo IN (@id0, @id1);

-- Cliente temporário para o teste (desfeito no ROLLBACK).
INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
VALUES ('Cliente temporário 012', 'temporario.012@retrogames.local', 'HASH-DE-EXEMPLO', TRUE, UTC_TIMESTAMP(), 2);
SET @idUsuario = LAST_INSERT_ID();

-- PedidoRepository.InserirAsync
SET @dataPedido = UTC_TIMESTAMP(); SET @total = 69.90; SET @idSituacao = 1;
INSERT INTO tb_pedido (dt_pedido, vl_total, co_usuario, co_situacao_pedido) VALUES (@dataPedido, @total, @idUsuario, @idSituacao);
SET @idPedido = LAST_INSERT_ID();

-- ItemPedidoRepository.InserirAsync (um único INSERT com todos os itens)
SET @quantidade0 = 1; SET @preco0 = 69.90; SET @idJogo0 = 4;
INSERT INTO tb_item_pedido (qt_item, vl_unitario, co_pedido, co_jogo) VALUES (@quantidade0, @preco0, @idPedido, @idJogo0);

-- Repetir o mesmo jogo no pedido: o UNIQUE recusa com o erro 1062. Descomente para ver; depois, ROLLBACK.
-- INSERT INTO tb_item_pedido (qt_item, vl_unitario, co_pedido, co_jogo) VALUES (1, 69.90, @idPedido, @idJogo0);

-- JogoRepository.BaixarEstoqueAsync: 1 linha alterada quando há estoque; 0 quando não há.
SET @id = 4; SET @quantidade = 1;
UPDATE tb_jogo SET qt_estoque = qt_estoque - @quantidade WHERE co_seq_jogo = @id AND qt_estoque >= @quantidade;
SELECT ROW_COUNT() AS linhas_alteradas_com_estoque;
SET @quantidade = 999;
UPDATE tb_jogo SET qt_estoque = qt_estoque - @quantidade WHERE co_seq_jogo = @id AND qt_estoque >= @quantidade;
SELECT ROW_COUNT() AS linhas_alteradas_sem_estoque;

-- PedidoRepository.ListarPorUsuarioAsync (mais recente primeiro)
SELECT co_seq_pedido, dt_pedido, vl_total, co_usuario, co_situacao_pedido FROM tb_pedido
WHERE co_usuario = @idUsuario ORDER BY dt_pedido DESC, co_seq_pedido DESC;

-- ItemPedidoRepository.ListarPorPedidosAsync
SET @id0 = @idPedido;
SELECT co_seq_item_pedido, qt_item, vl_unitario, co_pedido, co_jogo FROM tb_item_pedido
WHERE co_pedido IN (@id0) ORDER BY co_pedido, co_seq_item_pedido;

ROLLBACK;
-- Depois do rollback, o pedido de teste não existe mais. Esperado: zero linhas.
SELECT co_seq_pedido FROM tb_pedido WHERE co_seq_pedido = @idPedido;
