-- Aula 2. Selecione o schema já criado e populado da Aula 1.
-- Execute em um banco de exercício. As gravações abaixo são desfeitas no final.
-- As variáveis @... do Workbench equivalem aos parâmetros dos comandos C#.
SET NAMES utf8mb4;
START TRANSACTION;

-- Plataforma: listar, buscar, verificar nome (incluindo o próprio ID) e dependentes.
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma ORDER BY no_plataforma, co_seq_plataforma;
SET @id = 1;
SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma WHERE co_seq_plataforma = @id;
SET @nome = 'NES';
SET @idIgnorar = NULL;
SELECT 1 FROM tb_plataforma WHERE no_plataforma = @nome AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar) LIMIT 1;
SET @idIgnorar = 1;
SELECT 1 FROM tb_plataforma WHERE no_plataforma = @nome AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar) LIMIT 1;
SET @idPlataforma = 1;
SELECT 1 FROM tb_jogo WHERE co_plataforma = @idPlataforma LIMIT 1;
SET @nome = 'Console temporário Aula 2';
SET @fabricante = 'Fabricante de teste';
SET @ano = 1990;
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES (@nome, @fabricante, @ano);
SET @plataformaTemporaria = LAST_INSERT_ID();
SET @id = @plataformaTemporaria;
SET @nome = 'Console alterado Aula 2';
UPDATE tb_plataforma SET no_plataforma = @nome, no_fabricante = @fabricante, nu_ano_lancamento = @ano WHERE co_seq_plataforma = @id;
DELETE FROM tb_plataforma WHERE co_seq_plataforma = @id;

-- Jogos: catálogo sem filtro, segunda página e total. Esperado total 23.
SET @incluirInativos = FALSE;
SELECT COUNT(*) FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE);
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 0;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 5;
SET @incluirInativos = TRUE;
SELECT COUNT(*) FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE);
SET @incluirInativos = FALSE;

-- Cada filtro isolado e os mesmos filtros na contagem.
SET @nome = '%Super%';
SELECT COUNT(*) FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @nome ESCAPE '!';
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @nome ESCAPE '!' ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 5;
SET @idPlataforma = 2;
SELECT COUNT(*) FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) AND co_plataforma = @idPlataforma;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) AND co_plataforma = @idPlataforma ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 0;
SET @id0 = 6;
SET @id1 = 7;
SET @id2 = 8;
SELECT COUNT(*) FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) AND co_seq_jogo IN (@id0, @id1, @id2);
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) AND co_seq_jogo IN (@id0, @id1, @id2) ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 0;
SELECT COUNT(*) FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @nome ESCAPE '!' AND co_plataforma = @idPlataforma AND co_seq_jogo IN (@id0, @id1, @id2);
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) AND no_jogo LIKE @nome ESCAPE '!' AND co_plataforma = @idPlataforma AND co_seq_jogo IN (@id0, @id1, @id2) ORDER BY no_jogo, co_seq_jogo LIMIT 5 OFFSET 0;
-- Lista de IDs vazia.
SELECT COUNT(*) FROM tb_jogo WHERE (@incluirInativos = TRUE OR st_ativo = TRUE) AND 1 = 0;
SET @id = 7;
SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE co_seq_jogo = @id;

-- Gravar e alterar jogo temporário, preservando data de cadastro.
SET @nome = 'Jogo temporário Aula 2';
SET @descricao = NULL;
SET @ano = 1994;
SET @preco = 59.90;
SET @estoque = 3;
SET @ativo = TRUE;
SET @dataCadastro = UTC_TIMESTAMP();
SET @idCondicaoJogo = 2;
INSERT INTO tb_jogo (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo) VALUES (@nome, @descricao, @ano, @preco, @estoque, @ativo, @dataCadastro, @idPlataforma, @idCondicaoJogo);
SET @idJogo = LAST_INSERT_ID();
SET @id = @idJogo;
SET @descricao = 'Descrição alterada';
UPDATE tb_jogo SET no_jogo = @nome, ds_jogo = @descricao, nu_ano_lancamento = @ano, vl_preco = @preco, qt_estoque = @estoque, st_ativo = @ativo, co_plataforma = @idPlataforma, co_condicao_jogo = @idCondicaoJogo WHERE co_seq_jogo = @id;
UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @id;

-- Vínculos, numa consulta por operação.
SELECT co_jogo, co_genero FROM tb_jogo_genero ORDER BY co_jogo, co_genero;
SET @id0 = 1;
SET @id1 = 7;
SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN (@id0, @id1) ORDER BY co_jogo, co_genero;
SET @genero0 = 1;
SET @genero1 = 2;
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES (@idJogo, @genero0), (@idJogo, @genero1);
DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo;
SELECT co_seq_genero, no_genero FROM tb_genero ORDER BY no_genero, co_seq_genero;
SELECT co_condicao_jogo, ds_condicao_jogo FROM tb_condicao_jogo ORDER BY co_condicao_jogo;
ROLLBACK;
-- A contagem permanece 7 plataformas e 24 jogos após o rollback.
SELECT COUNT(*) FROM tb_plataforma;
SELECT COUNT(*) FROM tb_jogo;
