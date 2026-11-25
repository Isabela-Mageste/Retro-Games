-- Descrições breves dos jogos de exemplo (coluna opcional ds_jogo).
-- Script só de dados: a estrutura não muda. Execute uma vez, depois de 001 e 002.
-- Textos escritos para o projeto, com um resumo de cada jogo.
SET NAMES utf8mb4;
START TRANSACTION;
UPDATE tb_jogo SET ds_jogo = 'Mario atravessa o Reino dos Cogumelos para resgatar a princesa, no clássico que definiu os jogos de plataforma.' WHERE co_seq_jogo = 1;
UPDATE tb_jogo SET ds_jogo = 'Quatro heróis com habilidades diferentes arrancam vegetais do chão e enfrentam Wart no mundo dos sonhos de Subcon.' WHERE co_seq_jogo = 2;
UPDATE tb_jogo SET ds_jogo = 'Oito mundos temáticos, mapas para explorar e trajes especiais, como a folha que faz Mario voar.' WHERE co_seq_jogo = 3;
UPDATE tb_jogo SET ds_jogo = 'Link explora Hyrule em busca dos fragmentos da Triforce para enfrentar Ganon, com liberdade para escolher o caminho.' WHERE co_seq_jogo = 4;
UPDATE tb_jogo SET ds_jogo = 'Mega Man enfrenta oito robôs mestres e ganha a arma de cada chefe derrotado, com uma trilha sonora marcante.' WHERE co_seq_jogo = 5;
UPDATE tb_jogo SET ds_jogo = 'Mario e Yoshi exploram a Terra dos Dinossauros, com fases secretas e caminhos alternativos no mapa.' WHERE co_seq_jogo = 6;
UPDATE tb_jogo SET ds_jogo = 'Samus Aran explora o planeta Zebes atrás do filhote de Metroid, num mapa interligado cheio de segredos.' WHERE co_seq_jogo = 7;
UPDATE tb_jogo SET ds_jogo = 'Corridas de kart com os personagens da série, itens espalhados pelas pistas e modo batalha para dois jogadores.' WHERE co_seq_jogo = 8;
UPDATE tb_jogo SET ds_jogo = 'Donkey e Diddy Kong recuperam o estoque de bananas, com gráficos pré-renderizados que impressionaram na época.' WHERE co_seq_jogo = 9;
UPDATE tb_jogo SET ds_jogo = 'Um grupo de heróis viaja no tempo para impedir o fim do mundo, numa aventura de RPG com vários finais possíveis.' WHERE co_seq_jogo = 10;
UPDATE tb_jogo SET ds_jogo = 'Lutadores do mundo todo se enfrentam com golpes especiais e combos, no jogo que popularizou a luta um contra um.' WHERE co_seq_jogo = 11;
UPDATE tb_jogo SET ds_jogo = 'O ouriço azul corre em alta velocidade pelas fases do Dr. Robotnik para libertar os animais presos em robôs.' WHERE co_seq_jogo = 12;
UPDATE tb_jogo SET ds_jogo = 'Sonic ganha a companhia de Tails e o spin dash, e corre atrás das Esmeraldas do Caos em fases especiais.' WHERE co_seq_jogo = 13;
UPDATE tb_jogo SET ds_jogo = 'Briga de rua para até dois jogadores, com quatro personagens e uma trilha sonora eletrônica que virou referência.' WHERE co_seq_jogo = 14;
UPDATE tb_jogo SET ds_jogo = 'Três guerreiros enfrentam Death Adder em lutas com espadas, magias e montarias.' WHERE co_seq_jogo = 15;
UPDATE tb_jogo SET ds_jogo = 'Mario salta para o 3D e explora o castelo da Peach, onde cada quadro leva a um mundo aberto.' WHERE co_seq_jogo = 16;
UPDATE tb_jogo SET ds_jogo = 'Link viaja entre a infância e a vida adulta para deter Ganondorf, com mira travada e templos memoráveis.' WHERE co_seq_jogo = 17;
UPDATE tb_jogo SET ds_jogo = 'Corridas em pistas 3D para até quatro jogadores em tela dividida, com cascos, bananas e cogumelos de turbo.' WHERE co_seq_jogo = 18;
UPDATE tb_jogo SET ds_jogo = 'Cloud e o grupo Avalanche enfrentam a corporação Shinra e Sephiroth numa longa e emocionante aventura de RPG.' WHERE co_seq_jogo = 19;
UPDATE tb_jogo SET ds_jogo = 'O marsupial corre, gira e salta por ilhas cheias de armadilhas para enfrentar o Dr. Neo Cortex.' WHERE co_seq_jogo = 20;
UPDATE tb_jogo SET ds_jogo = 'Luta em 3D com novos personagens, movimentação lateral e combates rápidos para dois jogadores.' WHERE co_seq_jogo = 21;
UPDATE tb_jogo SET ds_jogo = 'Encaixe as peças que caem para completar linhas, no quebra-cabeça portátil que conquistou o mundo.' WHERE co_seq_jogo = 22;
UPDATE tb_jogo SET ds_jogo = 'Capture, treine e troque monstrinhos enquanto enfrenta os líderes de ginásio da região de Kanto.' WHERE co_seq_jogo = 23;
UPDATE tb_jogo SET ds_jogo = 'Basquete dois contra dois com enterradas exageradas e jogadores que pegam fogo depois de três cestas seguidas.' WHERE co_seq_jogo = 24;
COMMIT;
-- Conferência: esperado 24 jogos com descrição.
SELECT COUNT(*) FROM tb_jogo WHERE ds_jogo IS NOT NULL;
