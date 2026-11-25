// Capas por imagem, lidas de public/capas/capas.json (servido pelo Vite em /capas/capas.json).
// O arquivo é baixado uma vez e reaproveitado por todas as telas. Se ele não existir ou estiver
// com defeito, nenhum jogo tem imagem e todas as telas usam a capa ilustrada.
//
// Formato: { "<id do jogo>": { "arquivo": "1.jpg", "credito": "Foto: Autor (CC BY 4.0)", "link": "https://..." } }

const PASTA = '/capas/';
let carregamento = null;

function carregarCapas() {
  if (!carregamento) {
    carregamento = fetch(`${PASTA}capas.json`)
      .then((resposta) => (resposta.ok ? resposta.json() : {}))
      .then((dados) => (dados && typeof dados === 'object' && !Array.isArray(dados) ? dados : {}))
      .catch(() => ({}));
  }
  return carregamento;
}

// O servidor do Vite responde arquivos que não existem com a página do site (HTML, status 200).
// Por isso, antes de usar a imagem, confere se a resposta é mesmo uma imagem.
const conferidas = new Map();
function existeImagem(url) {
  if (!conferidas.has(url)) {
    conferidas.set(url, fetch(url, { method: 'HEAD' })
      .then((resposta) => resposta.ok && (resposta.headers.get('content-type') ?? '').startsWith('image/'))
      .catch(() => false));
  }
  return conferidas.get(url);
}

// Devolve { url, credito, link } para o jogo, ou null se ele não tem imagem que carregue.
export async function obterCapa(idJogo) {
  const capas = await carregarCapas();
  const capa = capas[String(idJogo)];
  if (!capa?.arquivo || typeof capa.arquivo !== 'string') return null;
  // Só arquivos da própria pasta: nada de caminhos com "/" ou endereços de outros sites.
  if (!/^[\w.-]+$/.test(capa.arquivo)) return null;
  const url = `${PASTA}${encodeURIComponent(capa.arquivo)}`;
  if (!(await existeImagem(url))) return null;
  return {
    url,
    credito: typeof capa.credito === 'string' ? capa.credito : '',
    link: typeof capa.link === 'string' && /^https:\/\//.test(capa.link) ? capa.link : null,
  };
}
