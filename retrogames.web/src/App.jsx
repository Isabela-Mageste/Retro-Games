import React from 'react';
import { Route, Routes } from 'react-router-dom';
import Layout from './components/Layout.jsx';
import Catalogo from './pages/Catalogo/Catalogo.jsx';
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx';
import NaoEncontrada from './pages/NaoEncontrada.jsx';

// Rotas públicas, todas dentro do Layout. As rotas do administrador entram na aula 8.
export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<Catalogo />} />
        <Route path="/jogos/:id" element={<DetalheJogo />} />
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  );
}
