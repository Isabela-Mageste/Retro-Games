import React, { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { jogoApi } from '../../api/servicos.js';
import { obterMensagemErro } from '../../api/erros.js';
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

  return (
    <>
      {voltar}
      <article className="card overflow-hidden">
        <div className="capa-jogo" aria-hidden="true" />
        <div className="card-body p-4">
          <p className="plataforma-rotulo mb-1">{jogo.nomePlataforma}</p>
          <h1 className="h2 card-title">{jogo.nome}</h1>

          <div className="d-flex flex-wrap gap-1 mb-3">
            {jogo.generos.map((genero) => (
              <span key={genero.id} className="badge rounded-pill badge-genero">{genero.nome}</span>
            ))}
          </div>

          <dl className="row mb-3">
            <dt className="col-sm-3">Ano de lançamento</dt>
            <dd className="col-sm-9">{jogo.anoLancamento}</dd>
            <dt className="col-sm-3">Condição</dt>
            <dd className="col-sm-9">{jogo.descricaoCondicao}</dd>
            <dt className="col-sm-3">Estoque</dt>
            <dd className="col-sm-9">
              {esgotado ? <span className="badge badge-esgotado">Esgotado</span> : `${jogo.estoque} unidade(s)`}
            </dd>
          </dl>

          {jogo.descricao && <p className="card-text">{jogo.descricao}</p>}

          <p className="display-6 preco mb-0">{formatarPreco(jogo.preco)}</p>
        </div>
      </article>
    </>
  );
}
