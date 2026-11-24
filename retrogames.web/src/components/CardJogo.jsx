import React from 'react';
import { Link } from 'react-router-dom';
import { formatarPreco } from '../lib/formato.js';

// Cartão do catálogo. Recebe tudo por props.
export default function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0;

  return (
    <div className="card h-100 card-jogo">
      <div className="card-body d-flex flex-column">
        <p className="text-uppercase text-muted small mb-1">{jogo.nomePlataforma}</p>
        <h2 className="card-title h5">{jogo.nome}</h2>
        <p className="card-text small text-muted mb-2">
          {jogo.anoLancamento} · {jogo.descricaoCondicao}
        </p>
        <div className="d-flex flex-wrap gap-1 mb-3">
          {jogo.generos.map((genero) => (
            <span key={genero} className="badge text-bg-secondary">{genero}</span>
          ))}
        </div>
        <div className="mt-auto d-flex align-items-center justify-content-between">
          <span className="fs-5 fw-semibold">{formatarPreco(jogo.preco)}</span>
          {esgotado && <span className="badge text-bg-warning">Esgotado</span>}
        </div>
      </div>
      <div className="card-footer bg-transparent border-top-0">
        <Link className="btn btn-outline-primary btn-sm w-100 stretched-link" to={`/jogos/${jogo.id}`}>
          Ver detalhes
        </Link>
      </div>
    </div>
  );
}
