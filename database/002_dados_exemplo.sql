SET NAMES utf8mb4;
SET time_zone = '+00:00';
START TRANSACTION;
INSERT INTO tb_condicao_jogo VALUES (1,'Novo'),(2,'Usado'),(3,'Recondicionado');
INSERT INTO tb_perfil VALUES (1,'Administrador'),(2,'Cliente');
INSERT INTO tb_plataforma (co_seq_plataforma,no_plataforma,no_fabricante,nu_ano_lancamento) VALUES
(1,'NES','Nintendo',1983),
(2,'Super Nintendo','Nintendo',1990),
(3,'Mega Drive','Sega',1988),
(4,'Nintendo 64','Nintendo',1996),
(5,'PlayStation','Sony',1994),
(6,'Game Boy','Nintendo',1989),
(7,'Sega Saturn','Sega',1994);
INSERT INTO tb_genero (co_seq_genero,no_genero) VALUES
(1,'Ação'),
(2,'Aventura'),
(3,'Plataforma'),
(4,'RPG'),
(5,'Corrida'),
(6,'Luta'),
(7,'Puzzle'),
(8,'Esporte');
-- Preços e estoques fictícios; anos referem-se à versão da plataforma, com lançamentos regionais distintos.
INSERT INTO tb_jogo (co_seq_jogo,no_jogo,ds_jogo,nu_ano_lancamento,vl_preco,qt_estoque,st_ativo,dt_cadastro,co_plataforma,co_condicao_jogo) VALUES
(1,'Super Mario Bros.',NULL,1985,54.90,3,TRUE,UTC_TIMESTAMP(),1,2),
(2,'Super Mario Bros. 2',NULL,1988,59.90,4,TRUE,UTC_TIMESTAMP(),1,2),
(3,'Super Mario Bros. 3',NULL,1988,64.90,5,TRUE,UTC_TIMESTAMP(),1,2),
(4,'The Legend of Zelda',NULL,1986,69.90,6,TRUE,UTC_TIMESTAMP(),1,2),
(5,'Mega Man 2',NULL,1988,74.90,2,TRUE,UTC_TIMESTAMP(),1,2),
(6,'Super Mario World',NULL,1990,79.90,3,TRUE,UTC_TIMESTAMP(),2,3),
(7,'Super Metroid',NULL,1994,84.90,4,TRUE,UTC_TIMESTAMP(),2,2),
(8,'Super Mario Kart',NULL,1992,89.90,5,TRUE,UTC_TIMESTAMP(),2,2),
(9,'Donkey Kong Country',NULL,1994,94.90,6,TRUE,UTC_TIMESTAMP(),2,2),
(10,'Chrono Trigger',NULL,1995,99.90,2,TRUE,UTC_TIMESTAMP(),2,2),
(11,'Street Fighter II',NULL,1992,104.90,3,TRUE,UTC_TIMESTAMP(),2,2),
(12,'Sonic the Hedgehog',NULL,1991,109.90,4,TRUE,UTC_TIMESTAMP(),3,3),
(13,'Sonic the Hedgehog 2',NULL,1992,114.90,5,TRUE,UTC_TIMESTAMP(),3,2),
(14,'Streets of Rage 2',NULL,1992,119.90,6,TRUE,UTC_TIMESTAMP(),3,2),
(15,'Golden Axe',NULL,1989,124.90,2,TRUE,UTC_TIMESTAMP(),3,2),
(16,'Super Mario 64',NULL,1996,129.90,3,TRUE,UTC_TIMESTAMP(),4,2),
(17,'The Legend of Zelda: Ocarina of Time',NULL,1998,134.90,4,TRUE,UTC_TIMESTAMP(),4,2),
(18,'Mario Kart 64',NULL,1996,139.90,5,TRUE,UTC_TIMESTAMP(),4,3),
(19,'Final Fantasy VII',NULL,1997,144.90,6,TRUE,UTC_TIMESTAMP(),5,2),
(20,'Crash Bandicoot',NULL,1996,149.90,2,TRUE,UTC_TIMESTAMP(),5,2),
(21,'Tekken 3',NULL,1998,154.90,3,TRUE,UTC_TIMESTAMP(),5,2),
(22,'Tetris',NULL,1989,159.90,0,TRUE,UTC_TIMESTAMP(),6,2),
(23,'Pokémon Red',NULL,1996,164.90,5,TRUE,UTC_TIMESTAMP(),6,2),
(24,'NBA Jam',NULL,1993,169.90,6,FALSE,UTC_TIMESTAMP(),3,3);
INSERT INTO tb_jogo_genero (co_jogo,co_genero) VALUES
(1,3),
(2,3),
(3,3),
(4,1),
(4,2),
(5,1),
(5,3),
(6,3),
(7,1),
(7,2),
(7,3),
(8,5),
(9,3),
(10,4),
(11,6),
(12,1),
(12,3),
(13,1),
(13,3),
(14,1),
(15,1),
(16,3),
(16,2),
(17,1),
(17,2),
(18,5),
(19,4),
(20,3),
(21,6),
(22,7),
(23,4),
(24,8);
COMMIT;
-- tb_usuario permanece vazia. Sega Saturn fica sem jogos para testar a contagem zero.
