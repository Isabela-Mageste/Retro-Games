const moeda = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });

// 199.9 → "R$ 199,90"
export function formatarPreco(valor) {
  return moeda.format(valor);
}

const dataHora = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' });

// A API devolve a data em UTC ("2026-11-25T01:12:55Z"); a tela mostra no fuso do navegador:
// "24/11/2026, 22:12".
export function formatarDataHora(valor) {
  return dataHora.format(new Date(valor));
}
