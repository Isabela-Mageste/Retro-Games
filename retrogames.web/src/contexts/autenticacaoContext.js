import { createContext } from 'react';

// Estado de autenticação compartilhado por todas as telas (preenchido pelo AutenticacaoProvider).
export const AutenticacaoContext = createContext(null);
