# Modelo entidade-relacionamento

```mermaid
erDiagram
    tb_plataforma ||--o{ tb_jogo : possui
    tb_condicao_jogo ||--o{ tb_jogo : classifica
    tb_jogo ||--o{ tb_jogo_genero : associa
    tb_genero ||--o{ tb_jogo_genero : associa
    tb_perfil ||--o{ tb_usuario : define
```

A chave de tb_jogo_genero é (co_jogo, co_genero). O mínimo de um gênero por jogo é uma regra do Service, não uma restrição estrutural do banco. As chaves estrangeiras impedem referências inexistentes; não removem dependentes automaticamente. O SQL completo e o dicionário efetivamente adotado estão em database/001_estrutura.sql.
