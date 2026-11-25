# Interface: identidade visual, responsividade e heurísticas de Nielsen

## Identidade visual

Tema escuro com o rosa como cor principal e o azul nos destaques, inspirado em telas de fliperama.
Todos os valores ficam em variáveis no topo de `retrogames.web/src/index.css` e sobrescrevem as variáveis do
Bootstrap 5.3 (`data-bs-theme="dark"` no `index.html`): os componentes do Bootstrap herdam as cores sem CSS
repetido em cada tela.

| Papel | Cor | Uso |
|---|---|---|
| Fundo | `#0d0814` | Página, com brilhos suaves rosa e azul nos cantos |
| Superfície | `#170f22` / `#21162f` | Painéis, cartões, campos de formulário |
| Rosa (principal) | `#ff4fa3` | Marca, botões principais, preços, item de menu ativo, caixas marcadas |
| Azul (destaque) | `#38bdf8` | Links, foco do teclado, nome da plataforma, gêneros, botões secundários |
| Texto | `#f6eef8` / `#b9a9c6` | Texto principal e texto de apoio |

- **Tipografia:** *Press Start 2P* (pixel, da era 8 bits) só na marca e em pequenos detalhes ("Press start",
  "Game over"); *Rajdhani* nos títulos; fonte do sistema no texto corrido, que precisa ser legível.
- **Elementos próprios:** faixa em degradê rosa→azul no topo dos cartões e sob a barra de navegação, brilho rosa
  ao passar o mouse nos cartões e um ícone de controle (`public/favicon.svg`).
- **Contraste (WCAG AA, mínimo 4,5:1):** texto claro sobre o fundo (~17:1), texto de apoio (~8:1), rosa sobre o
  fundo (~6:1) e texto escuro sobre os botões rosa (~6:1) e azuis (~8:1). Por isso os botões coloridos usam
  texto escuro, e não branco.

## Celular e computador

- Grade do Bootstrap: o catálogo vai de 1 coluna no celular a 4 no computador; a tela de plataformas põe o
  formulário acima da lista no celular e ao lado no computador.
- Menu recolhido em botão abaixo de 768 px; ele fecha sozinho ao escolher uma página.
- Nas tabelas da administração, colunas secundárias (fabricante, preço, estoque) somem em telas estreitas e a
  plataforma aparece embaixo do nome do jogo; os botões de ação têm pelo menos 44 px no celular.
- Conferido a 375 px: nenhuma página tem rolagem horizontal.

## Heurísticas de Nielsen

| Heurística | Como aparece no RetroGames |
|---|---|
| 1. Visibilidade do estado do sistema | Toda tela que busca dados mostra "Carregando...", erro ou lista vazia; botões mudam para "Entrando...", "Salvando...", "Excluindo..." e ficam desabilitados; avisos de sucesso após gravar ("Plataforma incluída."); item do menu da página atual em rosa; "Olá, nome" mostra quem está logado; contagem de jogos encontrados |
| 2. Correspondência com o mundo real | Textos em português, preço em reais (R$ 199,90), termos da loja (Esgotado, Condição, Ativo/Inativo) e "Desativar" no lugar de "Excluir" para jogos, porque o jogo não é apagado |
| 3. Controle e liberdade do usuário | Cancelar na edição de plataforma e no formulário de jogo; "Voltar ao catálogo" e "Voltar à lista"; Limpar filtros; filtros na URL, então o botão Voltar do navegador desfaz um filtro; jogo desativado pode ser reativado na edição |
| 4. Consistência e padrões | Mesmo envelope de erro em toda a API; rosa sempre para a ação principal e azul para as secundárias; mesmo layout de cabeçalho ("Administração" + título) nas telas de administração; componentes de estado reaproveitados |
| 5. Prevenção de erros | Confirmação antes de excluir plataforma e desativar jogo, explicando a consequência; campos numéricos com `type="number"`, `min` e `step`; listas de seleção para plataforma e condição (não há como digitar um id inexistente); caixas de marcação para gêneros; estoque começa em 0; botão desabilitado durante o envio, evitando gravação dupla |
| 6. Reconhecimento em vez de memorização | Todo campo tem rótulo visível; dicas embaixo dos campos ("Entre 1970 e 2100.", "Maior que zero.", "Marque ao menos um."); na edição, o formulário já vem preenchido e com os gêneros marcados; a linha em edição fica destacada na tabela |
| 7. Flexibilidade e eficiência | Busca com Enter; filtros e páginas em links compartilháveis; depois do login, o usuário volta à página onde estava (`voltarPara`); ao editar uma plataforma, o foco vai direto para o campo Nome |
| 8. Estética e design minimalista | Cartões mostram só o necessário para escolher (plataforma, nome, ano, condição, gêneros, preço); detalhes completos ficam na página do jogo; colunas secundárias escondidas no celular |
| 9. Ajudar a reconhecer, diagnosticar e recuperar erros | Erros de validação aparecem embaixo do campo certo, em vermelho, com a mensagem da API; erros de regra aparecem como frase clara ("Já existe uma plataforma com este nome."); falha de conexão mostra "Tentar novamente"; sessão expirada leva ao login e depois de volta à tela |
| 10. Ajuda e documentação | Textos de apoio nos formulários e no topo do catálogo; README com o passo a passo de execução |

## Acessibilidade

Link "Pular para o conteúdo" para quem navega pelo teclado; foco visível em azul; `role="status"` e
`role="alert"` nas mensagens; erros ligados aos campos com `aria-describedby`; textos escondidos para leitores
de tela nos botões repetidos ("Editar Super Mario World"); animações desligadas com `prefers-reduced-motion`.
