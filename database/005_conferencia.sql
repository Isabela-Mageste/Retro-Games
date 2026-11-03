SHOW TABLES; -- 7 tabelas
SHOW CREATE TABLE tb_jogo;
SELECT COUNT(*) AS plataformas FROM tb_plataforma; -- 7
SELECT COUNT(*) AS generos FROM tb_genero; -- 8
SELECT COUNT(*) AS jogos FROM tb_jogo; -- 24
SELECT COUNT(*) AS usuarios FROM tb_usuario; -- 0
SELECT COUNT(*) AS inativos FROM tb_jogo WHERE st_ativo = FALSE; -- 1
SELECT COUNT(*) AS esgotados FROM tb_jogo WHERE qt_estoque = 0; -- 1
-- Esperado: nenhuma linha (todos têm 1 a 3 gêneros).
SELECT j.co_seq_jogo, COUNT(jg.co_genero) AS quantidade_generos
FROM tb_jogo j LEFT JOIN tb_jogo_genero jg ON jg.co_jogo = j.co_seq_jogo
GROUP BY j.co_seq_jogo HAVING COUNT(jg.co_genero) NOT BETWEEN 1 AND 3;
-- Esperado: zero após os testes.
SELECT COUNT(*) AS residuos_testes FROM tb_jogo WHERE no_jogo LIKE 'TESTE%';
