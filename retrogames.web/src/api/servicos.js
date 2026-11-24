import api from './api.js';

// Uma função por endpoint. Cada uma devolve só o campo data do envelope
// { responseCode, data, errorMessage }. Gravação e login entram na aula 8.
const dados = (resposta) => resposta.data.data;

export const jogoApi = {
  // filtros: { busca, idPlataforma, idGenero, pagina, tamanhoPagina }
  listar: (filtros) => api.get('/Jogo', { params: filtros }).then(dados),
  buscarPorId: (id) => api.get(`/Jogo/${id}`).then(dados),
};

export const plataformaApi = {
  listar: () => api.get('/Plataforma').then(dados),
};

export const generoApi = {
  listar: () => api.get('/Genero').then(dados),
};

export const condicaoJogoApi = {
  listar: () => api.get('/CondicaoJogo').then(dados),
};
