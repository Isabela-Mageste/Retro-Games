import React, { useEffect, useState } from 'react';
import { Link, NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom';
import useAutenticacao from '../hooks/useAutenticacao.js';

// Moldura comum: menu, área da página (<Outlet />) e rodapé.
// O menu muda conforme quem está logado: visitante vê Entrar; logado vê "Olá, nome" e Sair;
// administrador vê também o menu Administração.
export default function Layout() {
  const location = useLocation();
  const navegar = useNavigate();
  const { usuario, carregando, estaAutenticado, ehAdministrador, sair } = useAutenticacao();
  const [menuAberto, definirMenuAberto] = useState(false);
  const [adminAberto, definirAdminAberto] = useState(false);
  const [saindo, definirSaindo] = useState(false);

  // No celular, o menu recolhe sozinho depois de escolher uma página.
  useEffect(() => {
    definirMenuAberto(false);
    definirAdminAberto(false);
  }, [location.pathname]);

  async function aoSair() {
    definirSaindo(true);
    try {
      await sair();
    } finally {
      definirSaindo(false);
      navegar('/');
    }
  }

  const emAdministracao = location.pathname.startsWith('/admin');

  return (
    <div className="d-flex flex-column min-vh-100">
      <a href="#conteudo" className="visually-hidden-focusable btn btn-primary position-absolute m-2">
        Pular para o conteúdo
      </a>
      <nav className="navbar navbar-expand-md navbar-retro sticky-top">
        <div className="container">
          <Link className="navbar-brand marca-retro" to="/">RetroGames</Link>
          <button
            className="navbar-toggler"
            type="button"
            aria-controls="menuPrincipal"
            aria-expanded={menuAberto}
            aria-label={menuAberto ? 'Fechar menu' : 'Abrir menu'}
            onClick={() => definirMenuAberto((aberto) => !aberto)}
          >
            <span className="navbar-toggler-icon" />
          </button>
          <div className={`collapse navbar-collapse ${menuAberto ? 'show' : ''}`} id="menuPrincipal">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                <NavLink className="nav-link" to="/" end>Catálogo</NavLink>
              </li>
              {ehAdministrador && (
                <li className="nav-item dropdown">
                  <button
                    type="button"
                    className={`nav-link dropdown-toggle ${emAdministracao ? 'active' : ''}`}
                    aria-expanded={adminAberto}
                    onClick={() => definirAdminAberto((aberto) => !aberto)}
                  >
                    Administração
                  </button>
                  <ul className={`dropdown-menu ${adminAberto ? 'show' : ''}`}>
                    <li><NavLink className="dropdown-item" to="/admin/jogos">Jogos</NavLink></li>
                    <li><NavLink className="dropdown-item" to="/admin/plataformas">Plataformas</NavLink></li>
                  </ul>
                </li>
              )}
            </ul>

            {!carregando && (
              <div className="d-flex flex-column flex-md-row align-items-md-center gap-2 py-2 py-md-0">
                {estaAutenticado ? (
                  <>
                    <span className="saudacao">Olá, {usuario.nome}</span>
                    <button type="button" className="btn btn-outline-info btn-sm" onClick={aoSair} disabled={saindo}>
                      {saindo ? 'Saindo...' : 'Sair'}
                    </button>
                  </>
                ) : (
                  <NavLink className="btn btn-primary btn-sm" to="/login">Entrar</NavLink>
                )}
              </div>
            )}
          </div>
        </div>
      </nav>

      <main id="conteudo" className="container py-4 flex-grow-1">
        <Outlet />
      </main>

      <footer className="rodape-retro py-3">
        <div className="container d-flex flex-column flex-sm-row justify-content-between gap-1 small">
          <span><span className="marca-retro me-2" style={{ fontSize: '0.6rem' }}>RetroGames</span>loja de jogos retrô usados</span>
          <span>Clássicos testados, embalados com carinho.</span>
        </div>
      </footer>
    </div>
  );
}
