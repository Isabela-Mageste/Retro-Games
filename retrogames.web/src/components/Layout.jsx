import React from 'react';
import { Link, NavLink, Outlet } from 'react-router-dom';

// Moldura comum: menu, área da página (<Outlet />) e rodapé.
export default function Layout() {
  return (
    <div className="d-flex flex-column min-vh-100">
      <nav className="navbar navbar-expand-md navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand fw-bold" to="/">RetroGames</Link>
          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#menuPrincipal"
            aria-controls="menuPrincipal"
            aria-expanded="false"
            aria-label="Abrir menu"
          >
            <span className="navbar-toggler-icon" />
          </button>
          <div className="collapse navbar-collapse" id="menuPrincipal">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                <NavLink className="nav-link" to="/" end>Catálogo</NavLink>
              </li>
            </ul>
          </div>
        </div>
      </nav>

      <main className="container py-4 flex-grow-1">
        <Outlet />
      </main>

      <footer className="bg-light border-top py-3">
        <div className="container text-center text-muted small">
          RetroGames · loja de jogos retrô usados
        </div>
      </footer>
    </div>
  );
}
