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

const MODEL_STATE_INVALIDO = 3;

// Com responseCode 3 (400 de validação), devolve { campo: mensagem }, com a primeira mensagem
// de cada campo. Nos demais casos, devolve {}.
// "idsGeneros[0]" vira "idsGeneros", para o erro aparecer junto da lista inteira.
export function obterErrosDosCampos(erro) {
  const envelope = erro?.response?.data;
  if (envelope?.responseCode !== MODEL_STATE_INVALIDO || !envelope.data) return {};

  const erros = {};
  Object.entries(envelope.data).forEach(([campo, mensagens]) => {
    const nome = campo.replace(/\[\d+\]$/, '');
    if (!erros[nome]) erros[nome] = Array.isArray(mensagens) ? mensagens[0] : String(mensagens);
  });
  return erros;
}
