-- Executar em schema NOVO e vazio. Não apaga nem altera bancos existentes.
-- Selecione o schema antes de executar. Datas de exemplo são UTC.
SET NAMES utf8mb4;
SET time_zone = '+00:00';
CREATE TABLE tb_condicao_jogo (
 co_condicao_jogo INT NOT NULL PRIMARY KEY,
 ds_condicao_jogo VARCHAR(30) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
CREATE TABLE tb_perfil (
 co_perfil INT NOT NULL PRIMARY KEY,
 ds_perfil VARCHAR(30) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
CREATE TABLE tb_plataforma (
 co_seq_plataforma INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
 no_plataforma VARCHAR(100) NOT NULL UNIQUE,
 no_fabricante VARCHAR(100) NOT NULL,
 nu_ano_lancamento INT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
CREATE TABLE tb_genero (
 co_seq_genero INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
 no_genero VARCHAR(100) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
CREATE TABLE tb_jogo (
 co_seq_jogo INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
 no_jogo VARCHAR(150) NOT NULL,
 ds_jogo TEXT NULL,
 nu_ano_lancamento INT NOT NULL,
 vl_preco DECIMAL(10,2) NOT NULL,
 qt_estoque INT NOT NULL,
 st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
 dt_cadastro DATETIME NOT NULL,
 co_plataforma INT NOT NULL,
 co_condicao_jogo INT NOT NULL,
 INDEX ix_jogo_nome (no_jogo),
 FOREIGN KEY (co_plataforma) REFERENCES tb_plataforma(co_seq_plataforma),
 FOREIGN KEY (co_condicao_jogo) REFERENCES tb_condicao_jogo(co_condicao_jogo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
CREATE TABLE tb_jogo_genero (
 co_jogo INT NOT NULL,
 co_genero INT NOT NULL,
 PRIMARY KEY (co_jogo, co_genero),
 FOREIGN KEY (co_jogo) REFERENCES tb_jogo(co_seq_jogo),
 FOREIGN KEY (co_genero) REFERENCES tb_genero(co_seq_genero)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
CREATE TABLE tb_usuario (
 co_seq_usuario INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
 no_usuario VARCHAR(150) NOT NULL,
 ds_email VARCHAR(254) NOT NULL UNIQUE,
 ds_senha_hash VARCHAR(512) NOT NULL,
 st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
 dt_cadastro DATETIME NOT NULL,
 co_perfil INT NOT NULL,
 FOREIGN KEY (co_perfil) REFERENCES tb_perfil(co_perfil)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
