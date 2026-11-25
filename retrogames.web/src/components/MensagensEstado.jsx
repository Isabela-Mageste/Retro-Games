import React from 'react';

// Os três estados de toda tela que busca dados: carregando, erro e vazio.

export function Carregando({ texto = 'Carregando...' }) {
  return (
    <div className="d-flex align-items-center gap-2 py-4 text-body-secondary" role="status">
      <div className="spinner-border spinner-border-sm text-primary" aria-hidden="true" />
      <span>{texto}</span>
    </div>
  );
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger d-flex flex-wrap align-items-center justify-content-between gap-2" role="alert">
      <span>{mensagem}</span>
      {aoTentarNovamente && (
        <button type="button" className="btn btn-outline-light btn-sm" onClick={aoTentarNovamente}>
          Tentar novamente
        </button>
      )}
    </div>
  );
}

export function EstadoVazio({ titulo, children }) {
  return (
    <div className="painel text-center text-body-secondary py-5 px-3">
      <p className="h5 text-body">{titulo}</p>
      {children}
    </div>
  );
}
