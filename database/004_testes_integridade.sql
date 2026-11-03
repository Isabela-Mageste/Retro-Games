-- Execute UM BLOCO POR VEZ no Workbench, na mesma conexão.
-- Após cada erro esperado, selecione e execute ROLLBACK antes do próximo bloco.
-- Resultados abaixo são ESPERADOS; não executados no ambiente de preparação.
-- Não execute com dados de produção. Use o banco de exercício.

-- T1: esperado ERROR 1452. Observado: PENDENTE de execução em MySQL 8.
START TRANSACTION;
INSERT INTO tb_jogo (no_jogo,nu_ano_lancamento,vl_preco,qt_estoque,dt_cadastro,co_plataforma,co_condicao_jogo)
VALUES ('TESTE FK',1990,10.00,1,UTC_TIMESTAMP(),2147483647,2);
ROLLBACK;

-- T2: esperado ERROR 1062. Observado: PENDENTE de execução em MySQL 8.
START TRANSACTION;
INSERT INTO tb_plataforma (no_plataforma,no_fabricante,nu_ano_lancamento)
VALUES ('NES','Nintendo',1983);
ROLLBACK;

-- T3: esperado ERROR 1451. Observado: PENDENTE de execução em MySQL 8.
START TRANSACTION;
DELETE FROM tb_plataforma WHERE co_seq_plataforma = 1;
ROLLBACK;

-- T4: esperado ERROR 1048. Observado: PENDENTE de execução em MySQL 8.
START TRANSACTION;
INSERT INTO tb_jogo (no_jogo,nu_ano_lancamento,vl_preco,qt_estoque,dt_cadastro,co_plataforma,co_condicao_jogo)
VALUES (NULL,1990,10.00,1,UTC_TIMESTAMP(),1,2);
ROLLBACK;

-- T5: esperado ACEITO. Observado: PENDENTE de execução em MySQL 8.
-- Preço zero é rejeitado por RN06 no Service/Validator futuro, não pelo banco.
START TRANSACTION;
INSERT INTO tb_jogo (no_jogo,nu_ano_lancamento,vl_preco,qt_estoque,dt_cadastro,co_plataforma,co_condicao_jogo)
VALUES ('TESTE PRECO ZERO',1990,0.00,1,UTC_TIMESTAMP(),1,2);
SELECT co_seq_jogo, no_jogo, vl_preco FROM tb_jogo WHERE co_seq_jogo = LAST_INSERT_ID();
ROLLBACK;
-- Todos os blocos desfazem dados, inclusive se uma restrição falhar inesperadamente.
-- AUTO_INCREMENT pode avançar mesmo com rollback: lacunas são normais.
