import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import useAutenticacao from '../hooks/useAutenticacao.js';
import { Carregando } from './MensagensEstado.jsx';

// Proteger a rota no front-end só melhora a navegação: quem protege de verdade é a API (RS03),
// que responde 401 ou 403 mesmo que alguém burle a tela.
export default function RotaProtegida({ somenteAdministrador = false }) {
  const { carregando, estaAutenticado, ehAdministrador } = useAutenticacao();
  const location = useLocation();

  // No F5, espera o GET /api/Autenticacao/eu antes de decidir.
  if (carregando) return <Carregando texto="Verificando sessão..." />;

  if (!estaAutenticado) {
    const voltarPara = encodeURIComponent(location.pathname + location.search);
    return <Navigate to={`/login?voltarPara=${voltarPara}`} replace />;
  }

  if (somenteAdministrador && !ehAdministrador) return <Navigate to="/" replace />;

  return <Outlet />;
}
