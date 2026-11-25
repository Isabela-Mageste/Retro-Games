import { useContext } from 'react';
import { CarrinhoContext } from '../contexts/carrinhoContext.js';

// Dá às telas acesso a itens, quantidadeTotal, total, adicionar, alterarQuantidade, remover e limpar.
export default function useCarrinho() {
  const contexto = useContext(CarrinhoContext);
  if (!contexto) {
    throw new Error('useCarrinho deve ser usado dentro do CarrinhoProvider.');
  }
  return contexto;
}
