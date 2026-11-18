-- Selecione o schema do RetroGames no Workbench antes de executar.
SELECT @@hostname AS servidor, @@port AS porta, CURRENT_USER() AS usuario, DATABASE() AS banco;
