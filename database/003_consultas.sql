-- Exercícios de SQL da Aula 1. Não copiar JOINs para Repository.
-- 1. Catálogo: esperado 23 jogos ativos, inclusive Tetris esgotado.
SELECT j.*, p.no_plataforma, c.ds_condicao_jogo
FROM tb_jogo j
JOIN tb_plataforma p ON p.co_seq_plataforma = j.co_plataforma
JOIN tb_condicao_jogo c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo, j.co_seq_jogo;

-- 2. Gêneros de Super Metroid (7): Ação, Aventura, Plataforma.
SET @id_jogo = 7;
SELECT g.co_seq_genero, g.no_genero
FROM tb_genero g JOIN tb_jogo_genero jg ON jg.co_genero = g.co_seq_genero
WHERE jg.co_jogo = @id_jogo ORDER BY g.no_genero;

-- 3. Jogos de RPG (4): Chrono Trigger, Final Fantasy VII e Pokémon Red.
SET @id_genero = 4;
SELECT j.* FROM tb_jogo j
JOIN tb_jogo_genero jg ON jg.co_jogo = j.co_seq_jogo
WHERE jg.co_genero = @id_genero ORDER BY j.no_jogo, j.co_seq_jogo;

-- 4. Inclui inativos. Esperado: NES 5, SNES 6, Mega Drive 5,
-- Nintendo 64 3, PlayStation 3, Game Boy 2, Sega Saturn 0.
SELECT p.co_seq_plataforma, p.no_plataforma, COUNT(j.co_seq_jogo) AS quantidade
FROM tb_plataforma p LEFT JOIN tb_jogo j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma ORDER BY p.no_plataforma;

-- 5. Busca do catálogo. 'Super' encontra 7 jogos: página 1 tem 5, página 2 tem 2.
-- Variáveis de sessão são parâmetros deste exercício; ADO.NET usará parâmetros de comando.
SET @trecho = '%Super%';
SET @tamanho = 5;
SET @pagina = 1;
SET @deslocamento = (@pagina - 1) * @tamanho;
PREPARE busca FROM 'SELECT * FROM tb_jogo WHERE st_ativo = TRUE AND no_jogo LIKE ? ORDER BY no_jogo, co_seq_jogo LIMIT ? OFFSET ?';
EXECUTE busca USING @trecho, @tamanho, @deslocamento;
SET @pagina = 2;
SET @deslocamento = (@pagina - 1) * @tamanho;
EXECUTE busca USING @trecho, @tamanho, @deslocamento;
DEALLOCATE PREPARE busca;
