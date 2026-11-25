import React from 'react';

// Capa ilustrada do jogo, desenhada pelo próprio site em SVG a partir dos dados do jogo.
// Não usa imagens de terceiros (as capas oficiais têm direitos autorais) e não exige coluna
// nova no banco: o dicionário de dados da especificação não tem imagem em tb_jogo.

// Pares de cores por plataforma, dentro da identidade rosa/azul/roxa.
const PALETAS = [
  ['#ff4fa3', '#38bdf8'],
  ['#a855f7', '#ff4fa3'],
  ['#38bdf8', '#6366f1'],
  ['#f472b6', '#8b5cf6'],
  ['#22d3ee', '#ff4fa3'],
  ['#818cf8', '#38bdf8'],
  ['#fb7185', '#a855f7'],
];

// Quebra o nome em até 4 linhas de ~13 caracteres, sem cortar palavras.
function quebrarTitulo(nome) {
  const linhas = [];
  let atual = '';
  nome.split(' ').forEach((palavra) => {
    const tentativa = atual ? `${atual} ${palavra}` : palavra;
    if (tentativa.length > 13 && atual) {
      linhas.push(atual);
      atual = palavra;
    } else {
      atual = tentativa;
    }
  });
  if (atual) linhas.push(atual);
  if (linhas.length > 4) return [...linhas.slice(0, 3), `${linhas.slice(3).join(' ').slice(0, 12)}…`];
  return linhas;
}

export default function CapaJogo({ jogo, className = '' }) {
  const [corA, corB] = PALETAS[(jogo.idPlataforma ?? jogo.id ?? 0) % PALETAS.length];
  const idGradiente = `capa-${jogo.id}`;
  const linhas = quebrarTitulo(jogo.nome);
  const genero = jogo.generos?.[0]?.nome ?? jogo.generos?.[0] ?? '';
  const tamanhoTitulo = linhas.some((linha) => linha.length > 11) ? 26 : 30;

  return (
    <svg
      className={`capa-ilustrada ${className}`}
      viewBox="0 0 300 400"
      role="img"
      aria-label={`Capa ilustrada de ${jogo.nome}`}
    >
      <defs>
        <linearGradient id={`${idGradiente}-fundo`} x1="0" y1="0" x2="1" y2="1">
          <stop offset="0" stopColor="#1b1029" />
          <stop offset="1" stopColor="#0d0814" />
        </linearGradient>
        <linearGradient id={`${idGradiente}-faixa`} x1="0" y1="0" x2="1" y2="0">
          <stop offset="0" stopColor={corA} />
          <stop offset="1" stopColor={corB} />
        </linearGradient>
        <pattern id={`${idGradiente}-grade`} width="20" height="20" patternUnits="userSpaceOnUse">
          <path d="M20 0H0V20" fill="none" stroke={corB} strokeOpacity="0.12" strokeWidth="1" />
        </pattern>
      </defs>

      <rect width="300" height="400" rx="18" fill={`url(#${idGradiente}-fundo)`} />
      {/* Horizonte em grade, no estilo dos anos 80 */}
      <rect y="250" width="300" height="150" fill={`url(#${idGradiente}-grade)`} />
      <circle cx="150" cy="250" r="78" fill={corA} fillOpacity="0.22" />
      <circle cx="150" cy="250" r="52" fill={corA} fillOpacity="0.35" />
      <rect y="248" width="300" height="3" fill={`url(#${idGradiente}-faixa)`} />

      {/* Faixa superior com a plataforma, como a lombada de uma caixa */}
      <rect width="300" height="44" rx="18" fill={`url(#${idGradiente}-faixa)`} />
      <rect y="26" width="300" height="18" fill={`url(#${idGradiente}-faixa)`} />
      <text x="16" y="29" fill="#1a0b16" fontFamily="Rajdhani, system-ui, sans-serif" fontWeight="700"
        fontSize="16" letterSpacing="2">{(jogo.nomePlataforma ?? '').toUpperCase()}</text>

      {/* Título */}
      {linhas.map((linha, indice) => (
        <text key={linha + indice} x="150" y={(linhas.length > 3 ? 82 : 100) + indice * (tamanhoTitulo + 6)} textAnchor="middle"
          fill="#f6eef8" fontFamily="'Press Start 2P', monospace" fontSize={tamanhoTitulo * 0.62}
          style={{ paintOrder: 'stroke' }} stroke={corA} strokeWidth="3" strokeOpacity="0.55">
          {linha}
        </text>
      ))}

      {/* Rodapé: ano e gênero */}
      <rect x="16" y="346" width="76" height="30" rx="8" fill="#0d0814" stroke={corB} strokeWidth="2" />
      <text x="54" y="367" textAnchor="middle" fill={corB} fontFamily="Rajdhani, system-ui, sans-serif"
        fontWeight="700" fontSize="18">{jogo.anoLancamento}</text>
      {genero && (
        <text x="284" y="367" textAnchor="end" fill="#f6eef8" fillOpacity="0.85"
          fontFamily="Rajdhani, system-ui, sans-serif" fontWeight="600" fontSize="16">{genero}</text>
      )}
    </svg>
  );
}
