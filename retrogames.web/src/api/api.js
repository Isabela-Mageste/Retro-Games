import axios from 'axios';

// Única instância do Axios da aplicação. Só este arquivo importa o Axios.
// withCredentials: o cookie de sessão (HttpOnly) vai junto em toda requisição.
const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
});

// 401 nestas rotas é resposta esperada (visitante ou senha errada), não sessão expirada.
const ROTAS_401_ESPERADO = ['/Autenticacao/eu', '/Autenticacao/login'];

// Qualquer outro 401 significa que a sessão acabou: o AutenticacaoProvider ouve o evento
// e limpa o usuário da tela.
api.interceptors.response.use(
  (resposta) => resposta,
  (erro) => {
    const url = erro.config?.url ?? '';
    const esperado = ROTAS_401_ESPERADO.some((rota) => url.startsWith(rota));
    if (erro.response?.status === 401 && !esperado) {
      window.dispatchEvent(new Event('sessao:expirada'));
    }
    return Promise.reject(erro);
  },
);

export default api;
