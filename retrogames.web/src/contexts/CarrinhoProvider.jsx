import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { CarrinhoContext } from './carrinhoContext.js';

// C03: o carrinho fica só no navegador (localStorage). Não é token de sessão: a sessão continua
// no cookie HttpOnly (RS01). A API só fica sabendo da compra quando o pedido é finalizado, e o
// preço gravado é sempre o do banco; o preço guardado aqui serve só para mostrar na tela.
const CHAVE = 'retrogames.carrinho';
export const QUANTIDADE_MINIMA = 1;
export const QUANTIDADE_MAXIMA = 10;

function lerDoNavegador() {
  try {
    const salvo = JSON.parse(window.localStorage.getItem(CHAVE) ?? '[]');
    return Array.isArray(salvo) ? salvo.filter((item) => Number.isInteger(item?.idJogo)) : [];
  } catch {
    return [];
  }
}

// Quantidade sempre entre 1 e 10 e nunca acima do estoque conhecido.
export function limitarQuantidade(quantidade, estoque) {
  const teto = Math.min(QUANTIDADE_MAXIMA, Math.max(QUANTIDADE_MINIMA, estoque ?? QUANTIDADE_MAXIMA));
  return Math.min(teto, Math.max(QUANTIDADE_MINIMA, Math.trunc(Number(quantidade) || QUANTIDADE_MINIMA)));
}

export default function CarrinhoProvider({ children }) {
  const [itens, definirItens] = useState(lerDoNavegador);

  useEffect(() => {
    try {
      window.localStorage.setItem(CHAVE, JSON.stringify(itens));
    } catch {
      // Navegador sem armazenamento (modo privado restrito): o carrinho vale só nesta aba.
    }
  }, [itens]);

  // Mantém o carrinho igual entre abas abertas do site.
  useEffect(() => {
    const aoMudarEmOutraAba = (evento) => {
      if (evento.key === CHAVE) definirItens(lerDoNavegador());
    };
    window.addEventListener('storage', aoMudarEmOutraAba);
    return () => window.removeEventListener('storage', aoMudarEmOutraAba);
  }, []);

  // Adiciona o jogo (ou soma à quantidade, se já estiver no carrinho).
  const adicionar = useCallback((jogo, quantidade = 1) => {
    definirItens((atuais) => {
      const existente = atuais.find((item) => item.idJogo === jogo.id);
      const dados = {
        idJogo: jogo.id,
        nome: jogo.nome,
        nomePlataforma: jogo.nomePlataforma,
        idPlataforma: jogo.idPlataforma,
        anoLancamento: jogo.anoLancamento,
        generos: (jogo.generos ?? []).map((genero) => genero.nome ?? genero),
        preco: jogo.preco,
        estoque: jogo.estoque,
      };
      if (existente) {
        return atuais.map((item) => (item.idJogo === jogo.id
          ? { ...item, ...dados, quantidade: limitarQuantidade(item.quantidade + quantidade, jogo.estoque) }
          : item));
      }
      return [...atuais, { ...dados, quantidade: limitarQuantidade(quantidade, jogo.estoque) }];
    });
  }, []);

  const alterarQuantidade = useCallback((idJogo, quantidade) => {
    definirItens((atuais) => atuais.map((item) => (item.idJogo === idJogo
      ? { ...item, quantidade: limitarQuantidade(quantidade, item.estoque) }
      : item)));
  }, []);

  // Atualiza nome, preço e estoque com o que a API respondeu agora.
  const atualizarDados = useCallback((idJogo, jogo) => {
    definirItens((atuais) => atuais.map((item) => (item.idJogo === idJogo
      ? {
        ...item,
        nome: jogo.nome,
        nomePlataforma: jogo.nomePlataforma,
        preco: jogo.preco,
        estoque: jogo.estoque,
        quantidade: jogo.estoque > 0 ? limitarQuantidade(item.quantidade, jogo.estoque) : item.quantidade,
      }
      : item)));
  }, []);

  const remover = useCallback((idJogo) => {
    definirItens((atuais) => atuais.filter((item) => item.idJogo !== idJogo));
  }, []);

  // Devolve um item removido à mesma posição (botão Desfazer).
  const restaurar = useCallback((item, posicao) => {
    definirItens((atuais) => {
      if (atuais.some((atual) => atual.idJogo === item.idJogo)) return atuais;
      const copia = [...atuais];
      copia.splice(Math.min(posicao, copia.length), 0, item);
      return copia;
    });
  }, []);

  const limpar = useCallback(() => definirItens([]), []);

  const valor = useMemo(() => ({
    itens,
    quantidadeTotal: itens.reduce((soma, item) => soma + item.quantidade, 0),
    total: itens.reduce((soma, item) => soma + item.preco * item.quantidade, 0),
    adicionar,
    alterarQuantidade,
    atualizarDados,
    remover,
    restaurar,
    limpar,
  }), [itens, adicionar, alterarQuantidade, atualizarDados, remover, restaurar, limpar]);

  return <CarrinhoContext.Provider value={valor}>{children}</CarrinhoContext.Provider>;
}
