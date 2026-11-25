import { useContext } from 'react';
import { AutenticacaoContext } from '../contexts/autenticacaoContext.js';

// Dá às telas acesso a usuario, carregando, estaAutenticado, ehAdministrador, entrar e sair.
export default function useAutenticacao() {
  const contexto = useContext(AutenticacaoContext);
  if (!contexto) {
    throw new Error('useAutenticacao deve ser usado dentro do AutenticacaoProvider.');
  }
  return contexto;
}
