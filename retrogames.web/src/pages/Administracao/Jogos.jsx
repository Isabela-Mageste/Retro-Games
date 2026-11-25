import React, { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { jogoApi } from '../../api/servicos.js';
import { obterMensagemErro } from '../../api/erros.js';
import Paginacao from '../../components/Paginacao.jsx';
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx';
import { formatarPreco } from '../../lib/formato.js';

const TAMANHO_PAGINA = 10;

// Administração de jogos (/admin/jogos). O administrador envia incluirInativos=true (RN04),
// então os jogos desativados continuam na lista, marcados como Inativo.
export default function Jogos() {
  const location = useLocation();
  const navegar = useNavigate();
  const [parametros, definirParametros] = useSearchParams();
  const busca = parametros.get('busca') ?? '';
  const pagina = Math.max(1, Number.parseInt(parametros.get('pagina'), 10) || 1);

  const [textoBusca, definirTextoBusca] = useState(busca);
  const [resultado, definirResultado] = useState(null);
  const [carregando, definirCarregando] = useState(true);
  const [erro, definirErro] = useState(null);
  const [tentativa, definirTentativa] = useState(0);
  // Mensagem vinda do formulário depois de salvar ("Jogo incluído.").
  const [aviso, definirAviso] = useState(location.state?.mensagem ? { tipo: 'success', texto: location.state.mensagem } : null);
  const [desativandoId, definirDesativandoId] = useState(null);

  // Limpa o state da navegação para a mensagem não voltar num F5.
  useEffect(() => {
    if (location.state?.mensagem) navegar(location.pathname + location.search, { replace: true, state: null });
  }, [location, navegar]);

  useEffect(() => {
    definirTextoBusca(busca);
  }, [busca]);

  useEffect(() => {
    if (aviso?.tipo !== 'success') return undefined;
    const temporizador = setTimeout(() => definirAviso(null), 4000);
    return () => clearTimeout(temporizador);
  }, [aviso]);

  useEffect(() => {
    let ativo = true;
    definirCarregando(true);
    definirErro(null);
    jogoApi
      .listar({ busca: busca || undefined, pagina, tamanhoPagina: TAMANHO_PAGINA, incluirInativos: true })
      .then((dados) => ativo && definirResultado(dados))
      .catch((falha) => ativo && definirErro(obterMensagemErro(falha)))
      .finally(() => ativo && definirCarregando(false));
    return () => {
      ativo = false;
    };
  }, [busca, pagina, tentativa]);

  function aoBuscar(evento) {
    evento.preventDefault();
    const novos = new URLSearchParams();
    if (textoBusca.trim()) novos.set('busca', textoBusca.trim());
    definirParametros(novos);
  }

  function mudarPagina(novaPagina) {
    const novos = new URLSearchParams(parametros);
    novos.set('pagina', String(novaPagina));
    definirParametros(novos);
  }

  async function desativar(jogo) {
    const confirmou = window.confirm(
      `Desativar "${jogo.nome}"?\n\nO jogo sai do catálogo público, mas continua cadastrado e pode ser reativado na edição.`,
    );
    if (!confirmou) return;
    definirDesativandoId(jogo.id);
    definirAviso(null);
    try {
      await jogoApi.desativar(jogo.id);
      definirAviso({ tipo: 'success', texto: `"${jogo.nome}" foi desativado.` });
      definirTentativa((valor) => valor + 1);
    } catch (falha) {
      definirAviso({ tipo: 'danger', texto: obterMensagemErro(falha) });
    } finally {
      definirDesativandoId(null);
    }
  }

  return (
    <>
      <header className="d-flex flex-wrap align-items-end justify-content-between gap-3 mb-4">
        <div>
          <p className="plataforma-rotulo mb-1">Administração</p>
          <h1 className="h2 mb-0">Jogos</h1>
        </div>
        <Link to="/admin/jogos/novo" className="btn btn-primary">+ Novo jogo</Link>
      </header>

      {aviso && (
        <div className={`alert alert-${aviso.tipo} d-flex justify-content-between align-items-start gap-2`}
          role={aviso.tipo === 'danger' ? 'alert' : 'status'}>
          <span>{aviso.texto}</span>
          <button type="button" className="btn-close" aria-label="Fechar aviso" onClick={() => definirAviso(null)} />
        </div>
      )}

      <form className="painel d-flex flex-column flex-sm-row gap-2 p-3 mb-4" onSubmit={aoBuscar} role="search">
        <label htmlFor="buscaAdmin" className="visually-hidden">Buscar jogo pelo nome</label>
        <input id="buscaAdmin" type="search" className="form-control" placeholder="Buscar jogo pelo nome"
          value={textoBusca} onChange={(evento) => definirTextoBusca(evento.target.value)} />
        <button type="submit" className="btn btn-outline-info">Buscar</button>
      </form>

      {carregando && <Carregando texto="Carregando jogos..." />}
      {!carregando && erro && <MensagemErro mensagem={erro} aoTentarNovamente={() => definirTentativa((v) => v + 1)} />}
      {!carregando && !erro && resultado && resultado.itens.length === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado.">
          {busca ? <p className="mb-0">Tente outra busca.</p> : <p className="mb-0">Use o botão Novo jogo para incluir o primeiro.</p>}
        </EstadoVazio>
      )}
      {!carregando && !erro && resultado && resultado.itens.length > 0 && (
        <>
          <div className="painel p-2 p-md-3 mb-4">
            <div className="table-responsive">
              <table className="table table-hover align-middle">
                <caption className="visually-hidden">Jogos cadastrados, inclusive os inativos</caption>
                <thead>
                  <tr>
                    <th scope="col">Jogo</th>
                    <th scope="col" className="d-none d-md-table-cell">Plataforma</th>
                    <th scope="col" className="d-none d-sm-table-cell">Preço</th>
                    <th scope="col" className="d-none d-lg-table-cell">Estoque</th>
                    <th scope="col">Situação</th>
                    <th scope="col" className="text-end">Ações</th>
                  </tr>
                </thead>
                <tbody>
                  {resultado.itens.map((jogo) => (
                    <tr key={jogo.id}>
                      <td>
                        <span className="fw-semibold">{jogo.nome}</span>
                        <span className="d-block d-md-none small text-body-secondary">{jogo.nomePlataforma}</span>
                      </td>
                      <td className="d-none d-md-table-cell">{jogo.nomePlataforma}</td>
                      <td className="d-none d-sm-table-cell">{formatarPreco(jogo.preco)}</td>
                      <td className="d-none d-lg-table-cell">{jogo.estoque}</td>
                      <td>
                        <span className={`badge ${jogo.ativo ? 'badge-ativo' : 'badge-inativo'}`}>
                          {jogo.ativo ? 'Ativo' : 'Inativo'}
                        </span>
                      </td>
                      <td className="text-end">
                        <div className="acoes-tabela d-inline-flex gap-2">
                          <Link to={`/admin/jogos/${jogo.id}/editar`} className="btn btn-outline-info btn-sm">
                            Editar<span className="visually-hidden"> {jogo.nome}</span>
                          </Link>
                          {jogo.ativo && (
                            <button type="button" className="btn btn-outline-danger btn-sm"
                              onClick={() => desativar(jogo)} disabled={desativandoId === jogo.id}>
                              {desativandoId === jogo.id ? 'Desativando...' : 'Desativar'}
                              <span className="visually-hidden"> {jogo.nome}</span>
                            </button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
          <p className="text-body-secondary small text-center">
            {resultado.totalItens} {resultado.totalItens === 1 ? 'jogo' : 'jogos'} no total
          </p>
          <Paginacao pagina={resultado.pagina} totalPaginas={resultado.totalPaginas} aoMudar={mudarPagina} />
        </>
      )}
    </>
  );
}
