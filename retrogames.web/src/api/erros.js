const SEM_CONEXAO = 'Não foi possível conectar ao servidor.';

// Transforma uma falha do Axios numa mensagem para o usuário.
export function obterMensagemErro(erro) {
  // Sem resposta: a rede falhou ou o servidor não respondeu.
  if (!erro?.response) return SEM_CONEXAO;

  const mensagem = erro.response.data?.errorMessage;
  if (typeof mensagem === 'string' && mensagem) return mensagem;

  // 5xx sem envelope: em desenvolvimento, é o proxy do Vite sem a API no ar.
  if (erro.response.status >= 500) return SEM_CONEXAO;

  return 'Ocorreu um erro inesperado. Tente novamente.';
}
