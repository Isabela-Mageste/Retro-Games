import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { autenticacaoApi } from '../api/servicos.js';
import { AutenticacaoContext } from './autenticacaoContext.js';

// O cookie de sessão é HttpOnly: o JavaScript não o lê. Para saber quem está logado, o
// provider pergunta à API (GET /api/Autenticacao/eu) ao abrir o site e a cada F5.
// Nada é guardado no localStorage nem no sessionStorage.
export default function AutenticacaoProvider({ children }) {
  const [usuario, definirUsuario] = useState(null);
  const [carregando, definirCarregando] = useState(true);

  useEffect(() => {
    let ativo = true;
    autenticacaoApi
      .usuarioAtual()
      .then((dados) => ativo && definirUsuario(dados))
      .catch(() => ativo && definirUsuario(null)) // 401: visitante, e isso é normal
      .finally(() => ativo && definirCarregando(false));
    return () => {
      ativo = false;
    };
  }, []);

  // O api.js dispara este evento quando uma rota protegida responde 401.
  useEffect(() => {
    const limpar = () => definirUsuario(null);
    window.addEventListener('sessao:expirada', limpar);
    return () => window.removeEventListener('sessao:expirada', limpar);
  }, []);

  const entrar = useCallback(async (email, senha) => {
    const dados = await autenticacaoApi.entrar(email, senha);
    definirUsuario(dados);
    return dados;
  }, []);

  const cadastrar = useCallback(async (nome, email, senha) => {
    const dados = await autenticacaoApi.cadastrar(nome, email, senha);
    definirUsuario(dados);
    return dados;
  }, []);

  const sair = useCallback(async () => {
    try {
      await autenticacaoApi.sair();
    } finally {
      definirUsuario(null);
    }
  }, []);

  const valor = useMemo(() => ({
    usuario,
    carregando,
    estaAutenticado: usuario !== null,
    ehAdministrador: usuario?.perfil === 'Administrador',
    entrar,
    cadastrar,
    sair,
  }), [usuario, carregando, entrar, cadastrar, sair]);

  return <AutenticacaoContext.Provider value={valor}>{children}</AutenticacaoContext.Provider>;
}
