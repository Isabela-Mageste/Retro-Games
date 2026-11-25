import api from './api.js';

// Uma função por endpoint. Cada uma devolve só o campo data do envelope
// { responseCode, data, errorMessage }.
const dados = (resposta) => resposta.data.data;

export const autenticacaoApi = {
  entrar: (email, senha) => api.post('/Autenticacao/login', { email, senha }).then(dados),
  sair: () => api.post('/Autenticacao/logout').then(dados),
  usuarioAtual: () => api.get('/Autenticacao/eu').then(dados),
  // C01: cria a conta de cliente; a API já inicia a sessão.
  cadastrar: (nome, email, senha) => api.post('/Autenticacao/cadastrar', { nome, email, senha }).then(dados),
};

export const jogoApi = {
  // filtros: { busca, idPlataforma, idGenero, pagina, tamanhoPagina, incluirInativos }
  listar: (filtros) => api.get('/Jogo', { params: filtros }).then(dados),
  buscarPorId: (id) => api.get(`/Jogo/${id}`).then(dados),
  inserir: (jogo) => api.post('/Jogo', jogo).then(dados),
  alterar: (id, jogo) => api.put(`/Jogo/${id}`, jogo).then(dados),
  // Exclusão lógica (RN03): a API só desativa o jogo.
  desativar: (id) => api.delete(`/Jogo/${id}`).then(dados),
};

// Desafio, Módulo de Compras. itens: [{ idJogo, quantidade }]; o preço vem do banco.
export const pedidoApi = {
  finalizar: (itens) => api.post('/Pedido', { itens }).then(dados),
  meus: () => api.get('/Pedido/meus').then(dados),
};

export const plataformaApi = {
  listar: () => api.get('/Plataforma').then(dados),
  inserir: (plataforma) => api.post('/Plataforma', plataforma).then(dados),
  alterar: (id, plataforma) => api.put(`/Plataforma/${id}`, plataforma).then(dados),
  excluir: (id) => api.delete(`/Plataforma/${id}`).then(dados),
};

export const generoApi = {
  listar: () => api.get('/Genero').then(dados),
};

export const condicaoJogoApi = {
  listar: () => api.get('/CondicaoJogo').then(dados),
};
