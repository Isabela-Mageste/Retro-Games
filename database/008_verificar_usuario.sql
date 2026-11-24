-- Aula 6. Conferência no Workbench dos comandos do UsuarioRepository.
-- Selecione o schema do RetroGames. As gravações abaixo são desfeitas no final.
-- As variáveis @... do Workbench equivalem aos parâmetros dos comandos C#.
-- O hash abaixo é um texto de exemplo: o hash real é gerado pelo PasswordHasher da API.
SET NAMES utf8mb4;
START TRANSACTION;

-- Existe administrador? (antes da primeira subida da API: nenhuma linha)
SET @idPerfil = 1;
SELECT 1 FROM tb_usuario WHERE co_perfil = @idPerfil LIMIT 1;

-- Inserir.
SET @nome = 'Usuário temporário Aula 6';
SET @email = 'temporario.aula6@retrogames.local';
SET @senhaHash = 'HASH-DE-EXEMPLO-NAO-E-UMA-SENHA';
SET @ativo = TRUE;
SET @dataCadastro = UTC_TIMESTAMP();
INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
VALUES (@nome, @email, @senhaHash, @ativo, @dataCadastro, @idPerfil);
SET @id = LAST_INSERT_ID();

-- Buscar por e-mail e por id. Esperado: uma linha em cada.
SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil FROM tb_usuario WHERE ds_email = @email;
SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil FROM tb_usuario WHERE co_seq_usuario = @id;

-- Existe administrador? Agora deve devolver 1.
SELECT 1 FROM tb_usuario WHERE co_perfil = @idPerfil LIMIT 1;

-- E-mail repetido: o UNIQUE recusa com o erro 1062 (no C#, RegistroDuplicadoException).
-- Descomente para ver o erro; depois execute o ROLLBACK abaixo.
-- INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
-- VALUES (@nome, @email, @senhaHash, @ativo, @dataCadastro, @idPerfil);

ROLLBACK;

-- Depois do rollback, o usuário temporário não existe mais. Esperado: zero linhas.
SELECT co_seq_usuario FROM tb_usuario WHERE ds_email = 'temporario.aula6@retrogames.local';
