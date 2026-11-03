import React, { useState } from 'react';
export default function App() {
  const [contador, definirContador] = useState(0);
  return <main className="container py-5">
    <h1>Vite + React · RetroGames</h1>
    <p>Base da Aula 1: ambiente, banco de dados e modelos.</p>
    <button className="btn btn-primary" onClick={() => definirContador(contador + 1)}>Contagem: {contador}</button>
    <p className="mt-3">Edite src/App.jsx para conferir a atualização da página.</p>
  </main>;
}
