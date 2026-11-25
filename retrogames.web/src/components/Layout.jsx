import React, { useEffect, useState } from 'react';
import { Link, NavLink, Outlet, useLocation } from 'react-router-dom';

// Moldura comum: menu, área da página (<Outlet />) e rodapé.
export default function Layout() {
  const location = useLocation();
  const [menuAberto, definirMenuAberto] = useState(false);

  // No celular, o menu recolhe sozinho depois de escolher uma página.
  useEffect(() => {
    definirMenuAberto(false);
  }, [location.pathname]);

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
            </ul>
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
