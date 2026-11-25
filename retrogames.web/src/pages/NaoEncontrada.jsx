import React from 'react';
import { Link } from 'react-router-dom';

export default function NaoEncontrada() {
  return (
    <div className="painel text-center py-5 px-3">
      <p className="marca-retro mb-3">Game over</p>
      <h1 className="h3">Página não encontrada</h1>
      <p className="text-body-secondary">O endereço acessado não existe.</p>
      <Link className="btn btn-primary" to="/">Ir para o catálogo</Link>
    </div>
  );
}
