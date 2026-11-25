import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { condicaoJogoApi, generoApi, jogoApi, plataformaApi } from '../../api/servicos.js';
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros.js';
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx';

const FORMULARIO_VAZIO = {
  nome: '',
  descricao: '',
  anoLancamento: '',
  preco: '',
  estoque: '0',
  idPlataforma: '',
  idCondicaoJogo: '',
  idsGeneros: [],
  ativo: true,
};

// Inclusão (/admin/jogos/novo) e alteração (/admin/jogos/:id/editar) de jogo.
// Os gêneros (relação N:N) seguem para a API como a lista idsGeneros.
export default function FormularioJogo() {
  const { id } = useParams();
  const ehEdicao = id !== undefined;
  const navegar = useNavigate();

  const [plataformas, definirPlataformas] = useState([]);
  const [condicoes, definirCondicoes] = useState([]);
  const [generos, definirGeneros] = useState([]);
  const [campos, definirCampos] = useState(FORMULARIO_VAZIO);
  const [carregando, definirCarregando] = useState(true);
  const [erroCarga, definirErroCarga] = useState(null);
  const [tentativa, definirTentativa] = useState(0);

  const [errosCampos, definirErrosCampos] = useState({});
  const [erroFormulario, definirErroFormulario] = useState(null);
  const [salvando, definirSalvando] = useState(false);

  // Carrega as listas de seleção e, na edição, o jogo.
  useEffect(() => {
    let ativo = true;
    definirCarregando(true);
    definirErroCarga(null);
    Promise.all([
      plataformaApi.listar(),
      condicaoJogoApi.listar(),
      generoApi.listar(),
      ehEdicao ? jogoApi.buscarPorId(id) : Promise.resolve(null),
    ])
      .then(([listaPlataformas, listaCondicoes, listaGeneros, jogo]) => {
        if (!ativo) return;
        definirPlataformas(listaPlataformas);
        definirCondicoes(listaCondicoes);
        definirGeneros(listaGeneros);
        definirCampos(jogo ? {
          nome: jogo.nome,
          descricao: jogo.descricao ?? '',
          anoLancamento: String(jogo.anoLancamento),
          preco: String(jogo.preco),
          estoque: String(jogo.estoque),
          idPlataforma: String(jogo.idPlataforma),
          idCondicaoJogo: String(jogo.idCondicaoJogo),
          idsGeneros: jogo.generos.map((genero) => genero.id),
          ativo: jogo.ativo,
        } : FORMULARIO_VAZIO);
      })
      .catch((falha) => ativo && definirErroCarga({
        mensagem: obterMensagemErro(falha),
        naoEncontrado: falha?.response?.status === 404,
      }))
      .finally(() => ativo && definirCarregando(false));
    return () => {
      ativo = false;
    };
  }, [id, ehEdicao, tentativa]);

  function alterarCampo(evento) {
    const { name, value, type, checked } = evento.target;
    definirCampos((atuais) => ({ ...atuais, [name]: type === 'checkbox' ? checked : value }));
  }

  function alternarGenero(idGenero) {
    definirCampos((atuais) => ({
      ...atuais,
      idsGeneros: atuais.idsGeneros.includes(idGenero)
        ? atuais.idsGeneros.filter((atual) => atual !== idGenero)
        : [...atuais.idsGeneros, idGenero],
    }));
  }

  async function salvar(evento) {
    evento.preventDefault();
    definirSalvando(true);
    definirErrosCampos({});
    definirErroFormulario(null);

    // Os campos do formulário devolvem texto: os números são convertidos antes do envio.
    const jogo = {
      nome: campos.nome,
      descricao: campos.descricao,
      anoLancamento: Number(campos.anoLancamento),
      preco: Number(campos.preco),
      estoque: Number(campos.estoque),
      idPlataforma: Number(campos.idPlataforma),
      idCondicaoJogo: Number(campos.idCondicaoJogo),
      idsGeneros: campos.idsGeneros,
    };

    try {
      if (ehEdicao) {
        await jogoApi.alterar(id, { ...jogo, ativo: campos.ativo });
      } else {
        await jogoApi.inserir(jogo);
      }
      const nome = campos.nome.trim();
      navegar('/admin/jogos', { state: { mensagem: ehEdicao ? `"${nome}" foi alterado.` : `"${nome}" foi incluído.` } });
    } catch (falha) {
      const porCampo = obterErrosDosCampos(falha);
      definirErrosCampos(porCampo);
      if (Object.keys(porCampo).length === 0) definirErroFormulario(obterMensagemErro(falha));
      else definirErroFormulario('Confira os campos destacados.');
      window.scrollTo({ top: 0, behavior: 'smooth' });
    } finally {
      definirSalvando(false);
    }
  }

  const voltar = <Link to="/admin/jogos" className="btn btn-link px-0 mb-3">&larr; Voltar à lista de jogos</Link>;

  if (carregando) return <>{voltar}<Carregando texto={ehEdicao ? 'Carregando jogo...' : 'Carregando formulário...'} /></>;
  if (erroCarga) {
    return (
      <>
        {voltar}
        <MensagemErro mensagem={erroCarga.mensagem}
          aoTentarNovamente={erroCarga.naoEncontrado ? undefined : () => definirTentativa((v) => v + 1)} />
      </>
    );
  }

  const classeCampo = (nome, base = 'form-control') => `${base} ${errosCampos[nome] ? 'is-invalid' : ''}`;
  const erro = (nome) => errosCampos[nome] && <div id={`erro-${nome}`} className="invalid-feedback">{errosCampos[nome]}</div>;
  const descrito = (nome, ajuda) => (errosCampos[nome] ? `erro-${nome}` : ajuda);

  return (
    <>
      {voltar}
      <header className="mb-4">
        <p className="plataforma-rotulo mb-1">Administração</p>
        <h1 className="h2 mb-0">{ehEdicao ? `Editar ${campos.nome || 'jogo'}` : 'Novo jogo'}</h1>
      </header>

      {erroFormulario && <div className="alert alert-danger" role="alert">{erroFormulario}</div>}

      <form className="painel p-4" onSubmit={salvar} noValidate>
        <div className="row g-3">
          <div className="col-12 col-md-8">
            <label htmlFor="nome" className="form-label">Nome</label>
            <input id="nome" name="nome" className={classeCampo('nome')} value={campos.nome}
              onChange={alterarCampo} maxLength={150} aria-describedby={descrito('nome')} />
            {erro('nome')}
          </div>

          <div className="col-6 col-md-4">
            <label htmlFor="anoLancamento" className="form-label">Ano de lançamento</label>
            <input id="anoLancamento" name="anoLancamento" type="number" inputMode="numeric"
              className={classeCampo('anoLancamento')} value={campos.anoLancamento} onChange={alterarCampo}
              aria-describedby={descrito('anoLancamento', 'ajuda-ano')} />
            {erro('anoLancamento') || <div id="ajuda-ano" className="form-text">Entre 1970 e 2100.</div>}
          </div>

          <div className="col-12">
            <label htmlFor="descricao" className="form-label">Descrição <span className="text-body-secondary">(opcional)</span></label>
            <textarea id="descricao" name="descricao" rows={3} className={classeCampo('descricao')}
              value={campos.descricao} onChange={alterarCampo} maxLength={4000} aria-describedby={descrito('descricao')} />
            {erro('descricao')}
          </div>

          <div className="col-6 col-md-3">
            <label htmlFor="preco" className="form-label">Preço (R$)</label>
            <input id="preco" name="preco" type="number" inputMode="decimal" min="0" step="0.01"
              className={classeCampo('preco')} value={campos.preco} onChange={alterarCampo}
              aria-describedby={descrito('preco', 'ajuda-preco')} />
            {erro('preco') || <div id="ajuda-preco" className="form-text">Maior que zero.</div>}
          </div>

          <div className="col-6 col-md-3">
            <label htmlFor="estoque" className="form-label">Estoque</label>
            <input id="estoque" name="estoque" type="number" inputMode="numeric" min="0" step="1"
              className={classeCampo('estoque')} value={campos.estoque} onChange={alterarCampo}
              aria-describedby={descrito('estoque')} />
            {erro('estoque')}
          </div>

          <div className="col-12 col-md-3">
            <label htmlFor="idPlataforma" className="form-label">Plataforma</label>
            <select id="idPlataforma" name="idPlataforma" className={classeCampo('idPlataforma', 'form-select')}
              value={campos.idPlataforma} onChange={alterarCampo} aria-describedby={descrito('idPlataforma')}>
              <option value="">Selecione...</option>
              {plataformas.map((plataforma) => (
                <option key={plataforma.id} value={plataforma.id}>{plataforma.nome}</option>
              ))}
            </select>
            {erro('idPlataforma')}
          </div>

          <div className="col-12 col-md-3">
            <label htmlFor="idCondicaoJogo" className="form-label">Condição</label>
            <select id="idCondicaoJogo" name="idCondicaoJogo" className={classeCampo('idCondicaoJogo', 'form-select')}
              value={campos.idCondicaoJogo} onChange={alterarCampo} aria-describedby={descrito('idCondicaoJogo')}>
              <option value="">Selecione...</option>
              {condicoes.map((condicao) => (
                <option key={condicao.id} value={condicao.id}>{condicao.descricao}</option>
              ))}
            </select>
            {erro('idCondicaoJogo')}
          </div>

          <fieldset className="col-12" aria-describedby={descrito('idsGeneros', 'ajuda-generos')}>
            <legend className="form-label fs-6 mb-2">Gêneros</legend>
            <div className="d-flex flex-wrap gap-3">
              {generos.map((genero) => (
                <div className="form-check" key={genero.id}>
                  <input id={`genero-${genero.id}`} type="checkbox"
                    className={classeCampo('idsGeneros', 'form-check-input')}
                    checked={campos.idsGeneros.includes(genero.id)} onChange={() => alternarGenero(genero.id)} />
                  <label className="form-check-label" htmlFor={`genero-${genero.id}`}>{genero.nome}</label>
                </div>
              ))}
            </div>
            {errosCampos.idsGeneros
              ? <div id="erro-idsGeneros" className="invalid-feedback d-block">{errosCampos.idsGeneros}</div>
              : <div id="ajuda-generos" className="form-text">Marque ao menos um.</div>}
          </fieldset>

          {ehEdicao && (
            <div className="col-12">
              <div className="form-check form-switch">
                <input id="ativo" name="ativo" type="checkbox" role="switch" className="form-check-input"
                  checked={campos.ativo} onChange={alterarCampo} />
                <label className="form-check-label" htmlFor="ativo">
                  Ativo <span className="text-body-secondary">(aparece no catálogo público)</span>
                </label>
              </div>
            </div>
          )}
        </div>

        <div className="d-flex flex-column flex-sm-row gap-2 mt-4">
          <button type="submit" className="btn btn-primary" disabled={salvando}>
            {salvando ? 'Salvando...' : ehEdicao ? 'Salvar alterações' : 'Incluir jogo'}
          </button>
          <Link to="/admin/jogos" className="btn btn-outline-info">Cancelar</Link>
        </div>
      </form>
    </>
  );
}
