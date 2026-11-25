# Capas dos jogos

As imagens desta pasta aparecem no detalhe do jogo e no carrinho. Jogo sem imagem (ou com arquivo que não
carrega) mostra a capa ilustrada, desenhada pelo próprio site: nada quebra se uma imagem faltar.

## Regra do desafio

Use só **imagens de uso livre ou do grupo** (documento do desafio, "Capriche na estilização"):

- fotos tiradas por você dos seus próprios cartuchos ou caixas; ou
- imagens com licença livre (por exemplo, Wikimedia Commons em CC BY, CC BY-SA ou domínio público), com o
  crédito que a licença exige.

Capas oficiais copiadas de sites de lojas ou da Wikipédia em inglês costumam ter direitos autorais: não use.

## Como adicionar uma capa

1. Coloque o arquivo aqui, com o id do jogo no nome: `1.jpg`, `7.webp`, `22.png`
   (o id aparece no endereço do detalhe, por exemplo `/jogos/7`). Prefira imagens em pé, na proporção 3:4,
   com até 600 px de largura.
2. Registre o arquivo em `capas.json`:

```json
{
  "7": {
    "arquivo": "7.jpg",
    "credito": "Foto: Seu Nome",
    "link": "https://endereço-da-fonte-se-houver"
  }
}
```

`credito` aparece embaixo da capa no detalhe do jogo; `link` é opcional e precisa começar com `https://`.
Não é preciso reiniciar nada: recarregue a página.
