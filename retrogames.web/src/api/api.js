import axios from 'axios';

// Única instância do Axios da aplicação. Só este arquivo importa o Axios.
// withCredentials: o cookie de sessão (HttpOnly) vai junto em toda requisição.
const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
});

export default api;
