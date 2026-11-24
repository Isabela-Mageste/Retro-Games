import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { generoApi, jogoApi, plataformaApi } from '../../api/servicos.js';
import { obterMensagemErro } from '../../api/erros.js';
import CardJogo from '../../components/CardJogo.jsx';
import Paginacao from '../../components/Paginacao.jsx';
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx';

const TAMANHO_PAGINA = 12;

// Catálogo público. Os filtros ficam na URL (?busca=mario&idPlataforma=2&pagina=2):
// o link pode ser compartilhado e o botão Voltar do navegador restaura o filtro anterior.
export default function Catalogo() {
  const [parametros, definirParametros] = useSearchParams();
  const busca = parametros.get('busca') ?? '';
  const idPlataforma = parametros.get('idPlataforma') ?? '';
  const idGenero = parametros.get('idGenero') ?? '';
  const pagina = Math.max(1, Number.parseInt(parametros.get('pagina'), 10) || 1);

  const [textoBusca, definirTextoBusca] = useState(busca);
  const [plataformas, definirPlataformas] = useState([]);
  const [generos, definirGeneros] = useState([]);
  const [erroFiltros, definirErroFiltros] = useState(null);

  const [resultado, definirResultado] = useState(null);
  const [carregando, definirCarregando] = useState(true);
  const [erro, definirErro] = useState(null);
  const [tentativa, definirTentativa] = useState(0);

  // Mantém o campo de busca igual à URL (por exemplo, depois do botão Voltar).
  useEffect(() => {
    definirTextoBusca(busca);
  }, [busca]);

  // As listas de plataformas e gêneros carregam uma vez.
  useEffect(() => {
    let ativo = true;
    Promise.all([plataformaApi.listar(), generoApi.listar()])
      .then(([listaPlataformas, listaGeneros]) => {
        if (!ativo) return;
        definirPlataformas(listaPlataformas);
        definirGeneros(listaGeneros);
      })
      .catch((falha) => ativo && definirErroFiltros(obterMensagemErro(falha)));
    return () => {
      ativo = false;
    };
  }, []);

  // Os jogos recarregam sempre que um filtro da URL muda.
  useEffect(() => {
    let ativo = true;
    definirCarregando(true);
    definirErro(null);
    jogoApi
      .listar({
        busca: busca || undefined,
        idPlataforma: idPlataforma || undefined,
        idGenero: idGenero || undefined,
        pagina,
        tamanhoPagina: TAMANHO_PAGINA,
      })
      .then((dados) => ativo && definirResultado(dados))
      .catch((falha) => ativo && definirErro(obterMensagemErro(falha)))
      .finally(() => ativo && definirCarregando(false));
    return () => {
      ativo = false;
    };
  }, [busca, idPlataforma, idGenero, pagina, tentativa]);

  // Trocar um filtro grava na URL e volta à página 1.
  function alterarFiltro(nome, valor) {
    const novos = new URLSearchParams(parametros);
    if (valor) novos.set(nome, valor);
    else novos.delete(nome);
    novos.delete('pagina');
    definirParametros(novos);
  }

  function aoBuscar(evento) {
    evento.preventDefault();
    alterarFiltro('busca', textoBusca.trim());
  }

  function mudarPagina(novaPagina) {
    const novos = new URLSearchParams(parametros);
    novos.set('pagina', String(novaPagina));
    definirParametros(novos);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  function limparFiltros() {
    definirParametros(new URLSearchParams());
  }

  const temFiltro = Boolean(busca || idPlataforma || idGenero);

  return (
    <>
      <h1 className="h3 mb-3">Catálogo</h1>

      <form className="row g-2 align-items-end mb-4" onSubmit={aoBuscar} role="search">
        <div className="col-12 col-md-5">
          <label htmlFor="busca" className="form-label">Buscar por nome</label>
          <div className="input-group">
            <input
              id="busca"
              type="search"
              className="form-control"
              placeholder="Ex.: Mario"
              value={textoBusca}
              onChange={(evento) => definirTextoBusca(evento.target.value)}
            />
            <button type="submit" className="btn btn-primary">Buscar</button>
          </div>
        </div>
        <div className="col-6 col-md-3">
          <label htmlFor="plataforma" className="form-label">Plataforma</label>
          <select
            id="plataforma"
            className="form-select"
            value={idPlataforma}
            onChange={(evento) => alterarFiltro('idPlataforma', evento.target.value)}
          >
            <option value="">Todas</option>
            {plataformas.map((plataforma) => (
              <option key={plataforma.id} value={plataforma.id}>{plataforma.nome}</option>
            ))}
          </select>
        </div>
        <div className="col-6 col-md-3">
          <label htmlFor="genero" className="form-label">Gênero</label>
          <select
            id="genero"
            className="form-select"
            value={idGenero}
            onChange={(evento) => alterarFiltro('idGenero', evento.target.value)}
          >
            <option value="">Todos</option>
            {generos.map((genero) => (
              <option key={genero.id} value={genero.id}>{genero.nome}</option>
            ))}
          </select>
        </div>
        <div className="col-12 col-md-1 d-grid">
          <button type="button" className="btn btn-outline-secondary" onClick={limparFiltros} disabled={!temFiltro}>
            Limpar
          </button>
        </div>
      </form>

      {erroFiltros && (
        <div className="alert alert-warning" role="alert">
          Não foi possível carregar os filtros: {erroFiltros}
        </div>
      )}

      {carregando && <Carregando texto="Carregando jogos..." />}

      {!carregando && erro && (
        <MensagemErro mensagem={erro} aoTentarNovamente={() => definirTentativa((valor) => valor + 1)} />
      )}

      {!carregando && !erro && resultado && resultado.itens.length === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado.">
          {temFiltro && <p>Tente outra busca ou limpe os filtros.</p>}
        </EstadoVazio>
      )}

      {!carregando && !erro && resultado && resultado.itens.length > 0 && (
        <>
          <p className="text-muted small">
            {resultado.totalItens} {resultado.totalItens === 1 ? 'jogo encontrado' : 'jogos encontrados'}
          </p>
          <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4 g-3 mb-4">
            {resultado.itens.map((jogo) => (
              <div className="col" key={jogo.id}>
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>
          <Paginacao pagina={resultado.pagina} totalPaginas={resultado.totalPaginas} aoMudar={mudarPagina} />
        </>
      )}
    </>
  );
}
