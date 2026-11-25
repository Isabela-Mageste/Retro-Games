import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { jogoApi, pedidoApi } from '../../api/servicos.js';
import { obterMensagemErro } from '../../api/erros.js';
import CapaJogo from '../../components/CapaJogo.jsx';
import { Carregando, EstadoVazio } from '../../components/MensagensEstado.jsx';
import { QUANTIDADE_MAXIMA } from '../../contexts/CarrinhoProvider.jsx';
import useCarrinho from '../../hooks/useCarrinho.js';
import { formatarPreco } from '../../lib/formato.js';

const MODEL_STATE_INVALIDO = 3;

// C03 (carrinho) e C04 (finalizar compra). O carrinho vive no navegador; ao abrir a tela, cada
// item é conferido de novo com a API (preço, estoque e se ainda está à venda). Na finalização,
// só vão para a API o id do jogo e a quantidade: o preço e o total são calculados no servidor.
export default function Carrinho() {
  const navegar = useNavigate();
  const { itens, quantidadeTotal, total, alterarQuantidade, atualizarDados, remover, restaurar, limpar } = useCarrinho();

  const [verificando, definirVerificando] = useState(itens.length > 0);
  const [indisponiveis, definirIndisponiveis] = useState([]);
  const [verificacao, definirVerificacao] = useState(0);
  const [removido, definirRemovido] = useState(null); // { item, posicao }
  const [confirmando, definirConfirmando] = useState(false);
  const [finalizando, definirFinalizando] = useState(false);
  const [erroCompra, definirErroCompra] = useState(null); // lista de mensagens

  // Confere os itens com a API ao abrir a tela e depois de uma recusa por estoque.
  useEffect(() => {
    const ids = itens.map((item) => item.idJogo);
    if (ids.length === 0) {
      definirVerificando(false);
      return undefined;
    }
    let ativo = true;
    definirVerificando(true);
    Promise.allSettled(ids.map((id) => jogoApi.buscarPorId(id))).then((resultados) => {
      if (!ativo) return;
      const fora = [];
      resultados.forEach((resultado, indice) => {
        if (resultado.status === 'fulfilled') {
          atualizarDados(ids[indice], resultado.value);
          if (resultado.value.estoque === 0) fora.push(ids[indice]);
        } else if (resultado.reason?.response?.status === 404) {
          fora.push(ids[indice]); // inexistente ou desativado
        }
      });
      definirIndisponiveis(fora);
      definirVerificando(false);
    });
    return () => {
      ativo = false;
    };
    // Só ao abrir e a cada nova verificação; mudar a quantidade não precisa ir à API.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [verificacao]);

  // O aviso "Desfazer" some sozinho.
  useEffect(() => {
    if (!removido) return undefined;
    const temporizador = setTimeout(() => definirRemovido(null), 6000);
    return () => clearTimeout(temporizador);
  }, [removido]);

  function aoRemover(item) {
    definirRemovido({ item, posicao: itens.findIndex((atual) => atual.idJogo === item.idJogo) });
    remover(item.idJogo);
    definirIndisponiveis((atuais) => atuais.filter((id) => id !== item.idJogo));
    definirConfirmando(false);
  }

  function aoDesfazer() {
    restaurar(removido.item, removido.posicao);
    definirRemovido(null);
  }

  async function aoConfirmar() {
    definirFinalizando(true);
    definirErroCompra(null);
    try {
      const pedido = await pedidoApi.finalizar(itens.map((item) => ({ idJogo: item.idJogo, quantidade: item.quantidade })));
      limpar();
      navegar('/meus-pedidos', { state: { pedidoCriado: pedido.id } });
    } catch (falha) {
      const envelope = falha?.response?.data;
      // Erros de formato (código 3) chegam por campo; mostra cada mensagem.
      const mensagens = envelope?.responseCode === MODEL_STATE_INVALIDO && envelope.data
        ? [...new Set(Object.values(envelope.data).flat())]
        : [obterMensagemErro(falha)];
      definirErroCompra(mensagens);
      definirConfirmando(false);
      // Estoque ou disponibilidade mudaram: confere de novo para mostrar o que ajustar.
      if ([409, 400].includes(falha?.response?.status)) definirVerificacao((valor) => valor + 1);
    } finally {
      definirFinalizando(false);
    }
  }

  const cabecalho = (
    <header className="mb-4">
      <p className="plataforma-rotulo mb-1">Sua compra</p>
      <h1 className="h2 mb-0">Carrinho</h1>
    </header>
  );

  const avisoRemovido = removido && (
    <div className="alert alert-info d-flex flex-wrap justify-content-between align-items-center gap-2" role="status">
      <span>"{removido.item.nome}" saiu do carrinho.</span>
      <button type="button" className="btn btn-sm btn-outline-light" onClick={aoDesfazer}>Desfazer</button>
    </div>
  );

  if (itens.length === 0) {
    return (
      <>
        {cabecalho}
        {avisoRemovido}
        <EstadoVazio titulo="Seu carrinho está vazio.">
          <p>Escolha um jogo no catálogo e clique em <strong>Adicionar ao carrinho</strong> na página dele.</p>
          <Link to="/" className="btn btn-primary">Ver o catálogo</Link>
        </EstadoVazio>
      </>
    );
  }

  const temIndisponivel = itens.some((item) => indisponiveis.includes(item.idJogo));

  return (
    <>
      {cabecalho}
      {avisoRemovido}
      {erroCompra && (
        <div className="alert alert-danger" role="alert">
          <p className="fw-semibold mb-1">Não foi possível finalizar o pedido.</p>
          <ul className="mb-0 ps-3">{erroCompra.map((mensagem) => <li key={mensagem}>{mensagem}</li>)}</ul>
        </div>
      )}

      <div className="row g-4">
        <section className="col-12 col-lg-8" aria-label="Itens do carrinho">
          {verificando && <Carregando texto="Conferindo preços e estoque..." />}
          <ul className="list-unstyled d-flex flex-column gap-3 mb-0">
            {itens.map((item) => {
              const indisponivel = indisponiveis.includes(item.idJogo);
              const maximo = Math.max(1, Math.min(QUANTIDADE_MAXIMA, item.estoque || 1));
              return (
                <li key={item.idJogo} className={`painel p-3 ${indisponivel ? 'border-danger' : ''}`}>
                  <div className="d-flex gap-3">
                    <CapaJogo jogo={{ ...item, id: item.idJogo }} className="capa-miniatura" mostrarCredito={false} />
                    <div className="flex-grow-1 min-w-0">
                      <div className="d-flex flex-wrap justify-content-between gap-2">
                        <div>
                          <p className="plataforma-rotulo mb-0">{item.nomePlataforma}</p>
                          <Link to={`/jogos/${item.idJogo}`} className="fw-semibold text-body">{item.nome}</Link>
                          <p className="small text-body-secondary mb-0">{formatarPreco(item.preco)} cada</p>
                        </div>
                        <p className="preco fs-5 mb-0">{formatarPreco(item.preco * item.quantidade)}</p>
                      </div>

                      {indisponivel ? (
                        <p className="text-danger small mt-2 mb-2" role="alert">
                          Este jogo não está mais disponível. Remova-o para finalizar a compra.
                        </p>
                      ) : null}

                      <div className="d-flex flex-wrap align-items-center gap-2 mt-2">
                        <label htmlFor={`quantidade-${item.idJogo}`} className="small text-body-secondary">Quantidade</label>
                        <select id={`quantidade-${item.idJogo}`} className="form-select form-select-sm w-auto"
                          value={Math.min(item.quantidade, maximo)} disabled={indisponivel || finalizando}
                          onChange={(evento) => {
                            alterarQuantidade(item.idJogo, Number(evento.target.value));
                            definirConfirmando(false);
                          }}>
                          {Array.from({ length: maximo }, (_, indice) => indice + 1).map((valor) => (
                            <option key={valor} value={valor}>{valor}</option>
                          ))}
                        </select>
                        <button type="button" className="btn btn-sm btn-outline-danger ms-auto"
                          onClick={() => aoRemover(item)} disabled={finalizando}>
                          Remover<span className="visually-hidden"> {item.nome}</span>
                        </button>
                      </div>
                    </div>
                  </div>
                </li>
              );
            })}
          </ul>
          <Link to="/" className="btn btn-link px-0 mt-3">&larr; Continuar comprando</Link>
        </section>

        <aside className="col-12 col-lg-4" aria-labelledby="tituloResumo">
          <div className="painel p-4 resumo-pedido">
            <h2 id="tituloResumo" className="h5 mb-3">Resumo</h2>
            <dl className="d-flex justify-content-between mb-2">
              <dt className="fw-normal text-body-secondary">Itens</dt>
              <dd className="mb-0">{quantidadeTotal}</dd>
            </dl>
            <dl className="d-flex justify-content-between align-items-baseline mb-3">
              <dt className="fw-normal text-body-secondary">Total</dt>
              <dd className="preco fs-3 mb-0">{formatarPreco(total)}</dd>
            </dl>

            {temIndisponivel && (
              <p className="small text-danger" role="alert">Remova os jogos indisponíveis para continuar.</p>
            )}

            {!confirmando ? (
              <button type="button" className="btn btn-primary w-100" onClick={() => definirConfirmando(true)}
                disabled={verificando || temIndisponivel}>
                Finalizar compra
              </button>
            ) : (
              <div className="border-top pt-3" role="group" aria-labelledby="tituloConfirmacao">
                <p id="tituloConfirmacao" className="fw-semibold mb-1">Confirme seu pedido</p>
                <ul className="small ps-3 mb-2">
                  {itens.map((item) => <li key={item.idJogo}>{item.quantidade}× {item.nome}</li>)}
                </ul>
                <p className="small text-body-secondary">
                  Total de {formatarPreco(total)}. O pedido fica como <strong>Pendente</strong> até o pagamento, e o
                  preço confirmado é o da loja no momento da compra.
                </p>
                <div className="d-flex flex-column gap-2">
                  <button type="button" className="btn btn-primary" onClick={aoConfirmar} disabled={finalizando}>
                    {finalizando ? (
                      <><span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />Finalizando...</>
                    ) : 'Confirmar pedido'}
                  </button>
                  <button type="button" className="btn btn-outline-info" onClick={() => definirConfirmando(false)}
                    disabled={finalizando}>
                    Voltar ao carrinho
                  </button>
                </div>
              </div>
            )}
          </div>
        </aside>
      </div>
    </>
  );
}
