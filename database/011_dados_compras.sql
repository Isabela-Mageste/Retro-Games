-- Desafio: Módulo de Compras. Dados fixos das situações de pedido.
-- Execute uma vez, depois de 010. Os ids se repetem em RetroGames.Model/Constantes/SituacaoPedidoIds.cs.
SET NAMES utf8mb4;
START TRANSACTION;
INSERT INTO tb_situacao_pedido (co_situacao_pedido, ds_situacao_pedido) VALUES
(1, 'Pendente'),
(2, 'Pago'),
(3, 'Enviado'),
(4, 'Cancelado');
COMMIT;
SELECT co_situacao_pedido, ds_situacao_pedido FROM tb_situacao_pedido ORDER BY co_situacao_pedido;
