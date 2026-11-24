import React from 'react';

// Os três estados de toda tela que busca dados: carregando, erro e vazio.

export function Carregando({ texto = 'Carregando...' }) {
  return (
    <div className="d-flex align-items-center gap-2 py-4" role="status">
      <div className="spinner-border spinner-border-sm" aria-hidden="true" />
      <span>{texto}</span>
    </div>
  );
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger d-flex flex-wrap align-items-center justify-content-between gap-2" role="alert">
      <span>{mensagem}</span>
      {aoTentarNovamente && (
        <button type="button" className="btn btn-outline-danger btn-sm" onClick={aoTentarNovamente}>
          Tentar novamente
        </button>
      )}
    </div>
  );
}

export function EstadoVazio({ titulo, children }) {
  return (
    <div className="text-center text-muted py-5">
      <p className="h5">{titulo}</p>
      {children}
    </div>
  );
}
