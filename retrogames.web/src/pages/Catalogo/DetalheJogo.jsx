import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { jogoApi } from '../../api/servicos.js';
import { obterMensagemErro } from '../../api/erros.js';
import CapaJogo from '../../components/CapaJogo.jsx';
import { QUANTIDADE_MAXIMA } from '../../contexts/CarrinhoProvider.jsx';
import useCarrinho from '../../hooks/useCarrinho.js';
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx';
import { formatarPreco } from '../../lib/formato.js';

const MENSAGEM_NAO_ENCONTRADO = 'Jogo não encontrado.';

// Detalhe de um jogo em /jogos/:id. Jogo inexistente ou inativo: a API responde 404
// com o código 103, e a tela mostra a mensagem do envelope.
export default function DetalheJogo() {
  const { id } = useParams();
  const idValido = /^[1-9]\d*$/.test(id ?? '');

  const [jogo, definirJogo] = useState(null);
  const [carregando, definirCarregando] = useState(idValido);
  const [erro, definirErro] = useState(idValido ? null : { mensagem: MENSAGEM_NAO_ENCONTRADO, naoEncontrado: true });
  const [tentativa, definirTentativa] = useState(0);
  const { itens, adicionar } = useCarrinho();
  const [quantidade, definirQuantidade] = useState(1);
  const [adicionado, definirAdicionado] = useState(false);

  useEffect(() => {
    if (!idValido) {
      definirCarregando(false);
      definirErro({ mensagem: MENSAGEM_NAO_ENCONTRADO, naoEncontrado: true });
      return undefined;
    }
    let ativo = true;
    definirCarregando(true);
    definirErro(null);
    jogoApi
      .buscarPorId(id)
      .then((dados) => ativo && definirJogo(dados))
      .catch((falha) => ativo && definirErro({
        mensagem: obterMensagemErro(falha),
        naoEncontrado: falha?.response?.status === 404,
      }))
      .finally(() => ativo && definirCarregando(false));
    return () => {
      ativo = false;
    };
  }, [id, idValido, tentativa]);

  const voltar = <Link to="/" className="btn btn-link px-0 mb-3">&larr; Voltar ao catálogo</Link>;

  if (carregando) return <>{voltar}<Carregando texto="Carregando jogo..." /></>;

  if (erro) {
    return (
      <>
        {voltar}
        <MensagemErro
          mensagem={erro.mensagem}
          aoTentarNovamente={erro.naoEncontrado ? undefined : () => definirTentativa((valor) => valor + 1)}
        />
      </>
    );
  }

  if (!jogo) return null;

  const esgotado = jogo.estoque === 0;
  const noCarrinho = itens.find((item) => item.idJogo === jogo.id)?.quantidade ?? 0;
  // Quantidade de 1 a 10, sem passar do estoque nem do que já está no carrinho.
  const maximo = Math.max(0, Math.min(QUANTIDADE_MAXIMA, jogo.estoque) - noCarrinho);

  function aoAdicionar() {
    adicionar(jogo, Math.min(quantidade, maximo));
    definirAdicionado(true);
    definirQuantidade(1);
  }

  return (
    <>
      {voltar}
      <article className="painel overflow-hidden">
        <div className="row g-0">
          <div className="col-12 col-md-5 col-lg-4 p-3 p-md-4 d-flex justify-content-center">
            <CapaJogo jogo={jogo} className="capa-detalhe" />
          </div>

          <div className="col-12 col-md-7 col-lg-8 p-4 ps-md-2">
            <p className="plataforma-rotulo mb-1">{jogo.nomePlataforma}</p>
            <h1 className="h2 mb-2">{jogo.nome}</h1>
            <p className="ano-lancamento mb-3">Lançado em <strong>{jogo.anoLancamento}</strong></p>

            <div className="d-flex flex-wrap gap-1 mb-4">
              {jogo.generos.map((genero) => (
                <span key={genero.id} className="badge rounded-pill badge-genero">{genero.nome}</span>
              ))}
            </div>

            {jogo.descricao && (
              <section className="mb-4" aria-labelledby="sobreJogo">
                <h2 id="sobreJogo" className="h6 text-body-secondary text-uppercase mb-2">Sobre o jogo</h2>
                <p className="mb-0">{jogo.descricao}</p>
              </section>
            )}

            <dl className="row mb-4">
              <dt className="col-5 col-sm-4">Condição</dt>
              <dd className="col-7 col-sm-8">{jogo.descricaoCondicao}</dd>
              <dt className="col-5 col-sm-4">Estoque</dt>
              <dd className="col-7 col-sm-8 mb-0">
                {esgotado ? <span className="badge badge-esgotado">Esgotado</span> : `${jogo.estoque} unidade(s)`}
              </dd>
            </dl>

            <p className="display-6 preco mb-3">{formatarPreco(jogo.preco)}</p>

            {esgotado ? (
              <p className="text-body-secondary mb-0">Este jogo está esgotado no momento.</p>
            ) : (
              <div className="d-flex flex-wrap align-items-end gap-2">
                {maximo > 0 && (
                  <div>
                    <label htmlFor="quantidade" className="form-label small mb-1">Quantidade</label>
                    <select id="quantidade" className="form-select" value={quantidade}
                      onChange={(evento) => definirQuantidade(Number(evento.target.value))}>
                      {Array.from({ length: maximo }, (_, indice) => indice + 1).map((valor) => (
                        <option key={valor} value={valor}>{valor}</option>
                      ))}
                    </select>
                  </div>
                )}
                <button type="button" className="btn btn-primary btn-lg" onClick={aoAdicionar} disabled={maximo === 0}>
                  {maximo === 0 ? 'Limite no carrinho' : 'Adicionar ao carrinho'}
                </button>
              </div>
            )}

            {noCarrinho > 0 && (
              <div className={`alert ${adicionado ? 'alert-success' : 'alert-info'} d-flex flex-wrap justify-content-between align-items-center gap-2 mt-3 mb-0`}
                role="status">
                <span>
                  {adicionado ? 'Adicionado ao carrinho! ' : ''}
                  Você tem {noCarrinho} {noCarrinho === 1 ? 'unidade' : 'unidades'} deste jogo no carrinho.
                </span>
                <Link to="/carrinho" className="btn btn-sm btn-outline-light">Ver carrinho</Link>
              </div>
            )}
          </div>
        </div>
      </article>
    </>
  );
}
