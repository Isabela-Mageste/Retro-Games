import React, { useEffect, useRef, useState } from 'react';
import { plataformaApi } from '../../api/servicos.js';
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros.js';
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx';

const FORMULARIO_VAZIO = { nome: '', fabricante: '', anoLancamento: '' };

// Manutenção de plataformas (/admin/plataformas): lista, inclusão, alteração e exclusão.
// A tela não repete as regras de negócio: mostra o que a API respondeu.
export default function Plataforma() {
  const [plataformas, definirPlataformas] = useState([]);
  const [carregando, definirCarregando] = useState(true);
  const [erroLista, definirErroLista] = useState(null);
  const [tentativa, definirTentativa] = useState(0);

  const [campos, definirCampos] = useState(FORMULARIO_VAZIO);
  const [emEdicao, definirEmEdicao] = useState(null);
  const [errosCampos, definirErrosCampos] = useState({});
  const [erroFormulario, definirErroFormulario] = useState(null);
  const [salvando, definirSalvando] = useState(false);

  const [aviso, definirAviso] = useState(null); // { tipo: 'success' | 'danger', texto }
  const [excluindoId, definirExcluindoId] = useState(null);
  const campoNome = useRef(null);

  useEffect(() => {
    let ativo = true;
    definirCarregando(true);
    definirErroLista(null);
    plataformaApi
      .listar()
      .then((dados) => ativo && definirPlataformas(dados))
      .catch((falha) => ativo && definirErroLista(obterMensagemErro(falha)))
      .finally(() => ativo && definirCarregando(false));
    return () => {
      ativo = false;
    };
  }, [tentativa]);

  // A mensagem de sucesso some sozinha depois de alguns segundos.
  useEffect(() => {
    if (aviso?.tipo !== 'success') return undefined;
    const temporizador = setTimeout(() => definirAviso(null), 4000);
    return () => clearTimeout(temporizador);
  }, [aviso]);

  const recarregar = () => definirTentativa((valor) => valor + 1);

  function alterarCampo(evento) {
    const { name, value } = evento.target;
    definirCampos((atuais) => ({ ...atuais, [name]: value }));
  }

  function limparFormulario() {
    definirCampos(FORMULARIO_VAZIO);
    definirEmEdicao(null);
    definirErrosCampos({});
    definirErroFormulario(null);
  }

  function editar(plataforma) {
    definirEmEdicao(plataforma);
    definirCampos({
      nome: plataforma.nome,
      fabricante: plataforma.fabricante,
      anoLancamento: String(plataforma.anoLancamento),
    });
    definirErrosCampos({});
    definirErroFormulario(null);
    campoNome.current?.focus();
  }

  async function salvar(evento) {
    evento.preventDefault();
    definirSalvando(true);
    definirErrosCampos({});
    definirErroFormulario(null);
    const plataforma = {
      nome: campos.nome,
      fabricante: campos.fabricante,
      anoLancamento: Number(campos.anoLancamento),
    };
    try {
      if (emEdicao) {
        await plataformaApi.alterar(emEdicao.id, plataforma);
        definirAviso({ tipo: 'success', texto: `Plataforma "${plataforma.nome.trim()}" alterada.` });
      } else {
        await plataformaApi.inserir(plataforma);
        definirAviso({ tipo: 'success', texto: `Plataforma "${plataforma.nome.trim()}" incluída.` });
      }
      limparFormulario();
      recarregar();
    } catch (falha) {
      const porCampo = obterErrosDosCampos(falha);
      definirErrosCampos(porCampo);
      if (Object.keys(porCampo).length === 0) definirErroFormulario(obterMensagemErro(falha));
    } finally {
      definirSalvando(false);
    }
  }

  async function excluir(plataforma) {
    const confirmou = window.confirm(
      `Excluir a plataforma "${plataforma.nome}"?\n\nEssa ação não pode ser desfeita.`,
    );
    if (!confirmou) return;
    definirExcluindoId(plataforma.id);
    definirAviso(null);
    try {
      await plataformaApi.excluir(plataforma.id);
      if (emEdicao?.id === plataforma.id) limparFormulario();
      definirAviso({ tipo: 'success', texto: `Plataforma "${plataforma.nome}" excluída.` });
      recarregar();
    } catch (falha) {
      definirAviso({ tipo: 'danger', texto: obterMensagemErro(falha) });
    } finally {
      definirExcluindoId(null);
    }
  }

  const classeCampo = (nome) => `form-control ${errosCampos[nome] ? 'is-invalid' : ''}`;

  return (
    <>
      <header className="mb-4">
        <p className="plataforma-rotulo mb-1">Administração</p>
        <h1 className="h2 mb-0">Plataformas</h1>
      </header>

      {aviso && (
        <div className={`alert alert-${aviso.tipo} d-flex justify-content-between align-items-start gap-2`}
          role={aviso.tipo === 'danger' ? 'alert' : 'status'}>
          <span>{aviso.texto}</span>
          <button type="button" className="btn-close" aria-label="Fechar aviso" onClick={() => definirAviso(null)} />
        </div>
      )}

      <div className="row g-4">
        <section className="col-12 col-lg-4" aria-labelledby="tituloFormulario">
          <form className="painel p-4" onSubmit={salvar} noValidate>
            <h2 id="tituloFormulario" className="h5 mb-3">
              {emEdicao ? `Editar ${emEdicao.nome}` : 'Nova plataforma'}
            </h2>

            {erroFormulario && <div className="alert alert-danger py-2" role="alert">{erroFormulario}</div>}

            <div className="mb-3">
              <label htmlFor="nome" className="form-label">Nome</label>
              <input id="nome" name="nome" ref={campoNome} className={classeCampo('nome')}
                value={campos.nome} onChange={alterarCampo} maxLength={80}
                aria-describedby={errosCampos.nome ? 'erroNome' : undefined} />
              {errosCampos.nome && <div id="erroNome" className="invalid-feedback">{errosCampos.nome}</div>}
            </div>

            <div className="mb-3">
              <label htmlFor="fabricante" className="form-label">Fabricante</label>
              <input id="fabricante" name="fabricante" className={classeCampo('fabricante')}
                value={campos.fabricante} onChange={alterarCampo} maxLength={80}
                aria-describedby={errosCampos.fabricante ? 'erroFabricante' : undefined} />
              {errosCampos.fabricante && <div id="erroFabricante" className="invalid-feedback">{errosCampos.fabricante}</div>}
            </div>

            <div className="mb-4">
              <label htmlFor="anoLancamento" className="form-label">Ano de lançamento</label>
              <input id="anoLancamento" name="anoLancamento" type="number" inputMode="numeric"
                className={classeCampo('anoLancamento')} value={campos.anoLancamento} onChange={alterarCampo}
                aria-describedby={errosCampos.anoLancamento ? 'erroAno' : 'ajudaAno'} />
              {errosCampos.anoLancamento
                ? <div id="erroAno" className="invalid-feedback">{errosCampos.anoLancamento}</div>
                : <div id="ajudaAno" className="form-text">Entre 1970 e 2100.</div>}
            </div>

            <div className="d-flex gap-2">
              <button type="submit" className="btn btn-primary flex-grow-1" disabled={salvando}>
                {salvando ? 'Salvando...' : emEdicao ? 'Salvar alterações' : 'Incluir plataforma'}
              </button>
              {emEdicao && (
                <button type="button" className="btn btn-outline-info" onClick={limparFormulario} disabled={salvando}>
                  Cancelar
                </button>
              )}
            </div>
          </form>
        </section>

        <section className="col-12 col-lg-8" aria-label="Plataformas cadastradas">
          {carregando && <Carregando texto="Carregando plataformas..." />}
          {!carregando && erroLista && <MensagemErro mensagem={erroLista} aoTentarNovamente={recarregar} />}
          {!carregando && !erroLista && plataformas.length === 0 && (
            <EstadoVazio titulo="Nenhuma plataforma cadastrada.">
              <p className="mb-0">Use o formulário para incluir a primeira.</p>
            </EstadoVazio>
          )}
          {!carregando && !erroLista && plataformas.length > 0 && (
            <div className="painel p-2 p-md-3">
              <div className="table-responsive">
                <table className="table table-hover align-middle">
                  <caption className="visually-hidden">Plataformas cadastradas</caption>
                  <thead>
                    <tr>
                      <th scope="col">Nome</th>
                      <th scope="col" className="d-none d-sm-table-cell">Fabricante</th>
                      <th scope="col">Ano</th>
                      <th scope="col" className="text-end">Ações</th>
                    </tr>
                  </thead>
                  <tbody>
                    {plataformas.map((plataforma) => (
                      <tr key={plataforma.id} className={emEdicao?.id === plataforma.id ? 'table-active' : undefined}>
                        <td className="fw-semibold">{plataforma.nome}</td>
                        <td className="d-none d-sm-table-cell">{plataforma.fabricante}</td>
                        <td>{plataforma.anoLancamento}</td>
                        <td className="text-end">
                          <div className="acoes-tabela d-inline-flex gap-2">
                            <button type="button" className="btn btn-outline-info btn-sm" onClick={() => editar(plataforma)}>
                              Editar<span className="visually-hidden"> {plataforma.nome}</span>
                            </button>
                            <button type="button" className="btn btn-outline-danger btn-sm"
                              onClick={() => excluir(plataforma)} disabled={excluindoId === plataforma.id}>
                              {excluindoId === plataforma.id ? 'Excluindo...' : 'Excluir'}
                              <span className="visually-hidden"> {plataforma.nome}</span>
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </section>
      </div>
    </>
  );
}
