-- Desafio: Módulo de Compras. Estrutura das tabelas de pedido.
-- Execute uma vez, depois de 001 e 002. Só estrutura: PK, FK com comportamento padrão,
-- tipos, NOT NULL e UNIQUE. Sem trigger, function, CHECK nem ação referencial (Espec. 4.3).
SET NAMES utf8mb4;
SET time_zone = '+00:00';

CREATE TABLE tb_situacao_pedido (
 co_situacao_pedido INT NOT NULL PRIMARY KEY,
 ds_situacao_pedido VARCHAR(30) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE tb_pedido (
 co_seq_pedido INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
 dt_pedido DATETIME NOT NULL,            -- UTC
 vl_total DECIMAL(10,2) NOT NULL,        -- calculado no Service
 co_usuario INT NOT NULL,
 co_situacao_pedido INT NOT NULL,
 INDEX ix_pedido_usuario (co_usuario, dt_pedido),
 FOREIGN KEY (co_usuario) REFERENCES tb_usuario(co_seq_usuario),
 FOREIGN KEY (co_situacao_pedido) REFERENCES tb_situacao_pedido(co_situacao_pedido)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE tb_item_pedido (
 co_seq_item_pedido INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
 qt_item INT NOT NULL,
 vl_unitario DECIMAL(10,2) NOT NULL,     -- preço no momento da compra
 co_pedido INT NOT NULL,
 co_jogo INT NOT NULL,
 UNIQUE KEY uk_item_pedido_jogo (co_pedido, co_jogo), -- um jogo aparece uma só vez em cada pedido
 FOREIGN KEY (co_pedido) REFERENCES tb_pedido(co_seq_pedido),
 FOREIGN KEY (co_jogo) REFERENCES tb_jogo(co_seq_jogo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
