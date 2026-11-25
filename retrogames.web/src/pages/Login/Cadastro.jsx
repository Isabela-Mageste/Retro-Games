import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros.js';
import useAutenticacao from '../../hooks/useAutenticacao.js';

// Só aceita destinos internos ("/carrinho"), nunca outro site ("//exemplo.com").
function destinoSeguro(voltarPara) {
  return voltarPara && voltarPara.startsWith('/') && !voltarPara.startsWith('//') ? voltarPara : '/';
}

const SENHA_MINIMA = 8;

// C01: cadastro de cliente (/cadastro). Após criar a conta, a sessão já começa.
export default function Cadastro() {
  const { cadastrar, estaAutenticado, carregando } = useAutenticacao();
  const [parametros] = useSearchParams();
  const navegar = useNavigate();
  const destino = destinoSeguro(parametros.get('voltarPara'));

  const [campos, definirCampos] = useState({ nome: '', email: '', senha: '', confirmacao: '' });
  const [errosCampos, definirErrosCampos] = useState({});
  const [erro, definirErro] = useState(null);
  const [enviando, definirEnviando] = useState(false);
  const [mostrarSenha, definirMostrarSenha] = useState(false);

  useEffect(() => {
    if (!carregando && estaAutenticado) navegar(destino, { replace: true });
  }, [carregando, estaAutenticado, destino, navegar]);

  function alterarCampo(evento) {
    const { name, value } = evento.target;
    definirCampos((atuais) => ({ ...atuais, [name]: value }));
  }

  // Indicadores das regras da senha, atualizados enquanto a pessoa digita.
  const regras = [
    { texto: `Pelo menos ${SENHA_MINIMA} caracteres`, ok: campos.senha.length >= SENHA_MINIMA },
    { texto: 'Pelo menos uma letra', ok: /[A-Za-z]/.test(campos.senha) },
    { texto: 'Pelo menos um número', ok: /[0-9]/.test(campos.senha) },
  ];

  async function aoEnviar(evento) {
    evento.preventDefault();
    definirErro(null);
    definirErrosCampos({});

    // A confirmação só existe na tela: evita um erro de digitação antes de chegar à API.
    if (campos.senha !== campos.confirmacao) {
      definirErrosCampos({ confirmacao: 'As senhas não conferem.' });
      return;
    }

    definirEnviando(true);
    try {
      await cadastrar(campos.nome, campos.email.trim(), campos.senha);
      navegar(destino, { replace: true, state: { boasVindas: true } });
    } catch (falha) {
      const porCampo = obterErrosDosCampos(falha);
      definirErrosCampos(porCampo);
      if (Object.keys(porCampo).length === 0) definirErro(obterMensagemErro(falha));
    } finally {
      definirEnviando(false);
    }
  }

  const classeCampo = (nome) => `form-control ${errosCampos[nome] ? 'is-invalid' : ''}`;
  const erroCampo = (nome) => errosCampos[nome] && (
    <div id={`erro-${nome}`} className="invalid-feedback">{errosCampos[nome]}</div>
  );

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-sm-10 col-md-8 col-lg-6">
        <div className="painel p-4 p-md-5">
          <p className="marca-retro mb-2">New player</p>
          <h1 className="h3 mb-1">Criar conta</h1>
          <p className="text-body-secondary mb-4">Cadastre-se para montar o carrinho e acompanhar seus pedidos.</p>

          {erro && <div className="alert alert-danger" role="alert">{erro}</div>}

          <form onSubmit={aoEnviar} noValidate>
            <div className="mb-3">
              <label htmlFor="nome" className="form-label">Nome</label>
              <input id="nome" name="nome" className={classeCampo('nome')} autoComplete="name"
                value={campos.nome} onChange={alterarCampo} maxLength={150} autoFocus
                aria-describedby={errosCampos.nome ? 'erro-nome' : undefined} />
              {erroCampo('nome')}
            </div>

            <div className="mb-3">
              <label htmlFor="email" className="form-label">E-mail</label>
              <input id="email" name="email" type="email" className={classeCampo('email')} autoComplete="email"
                value={campos.email} onChange={alterarCampo} maxLength={254}
                aria-describedby={errosCampos.email ? 'erro-email' : undefined} />
              {erroCampo('email')}
            </div>

            <div className="mb-3">
              <label htmlFor="senha" className="form-label">Senha</label>
              <div className="input-group has-validation">
                <input id="senha" name="senha" type={mostrarSenha ? 'text' : 'password'} className={classeCampo('senha')}
                  autoComplete="new-password" value={campos.senha} onChange={alterarCampo} maxLength={100}
                  aria-describedby={errosCampos.senha ? 'erro-senha regrasSenha' : 'regrasSenha'} />
                <button type="button" className="btn btn-outline-info" onClick={() => definirMostrarSenha((v) => !v)}
                  aria-pressed={mostrarSenha}>
                  {mostrarSenha ? 'Ocultar' : 'Mostrar'}
                </button>
                {erroCampo('senha')}
              </div>
              <ul id="regrasSenha" className="list-unstyled small mt-2 mb-0">
                {regras.map((regra) => (
                  <li key={regra.texto} className={regra.ok ? 'text-success' : 'text-body-secondary'}>
                    <span aria-hidden="true">{regra.ok ? '✓' : '•'}</span> {regra.texto}
                    <span className="visually-hidden">{regra.ok ? ' (atendida)' : ' (pendente)'}</span>
                  </li>
                ))}
              </ul>
            </div>

            <div className="mb-4">
              <label htmlFor="confirmacao" className="form-label">Confirme a senha</label>
              <input id="confirmacao" name="confirmacao" type={mostrarSenha ? 'text' : 'password'}
                className={classeCampo('confirmacao')} autoComplete="new-password"
                value={campos.confirmacao} onChange={alterarCampo} maxLength={100}
                aria-describedby={errosCampos.confirmacao ? 'erro-confirmacao' : undefined} />
              {erroCampo('confirmacao')}
            </div>

            <button type="submit" className="btn btn-primary w-100" disabled={enviando}>
              {enviando ? (
                <><span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />Criando conta...</>
              ) : 'Criar conta'}
            </button>
          </form>

          <p className="text-center mt-4 mb-0">
            Já tem conta?{' '}
            <Link to={`/login${destino !== '/' ? `?voltarPara=${encodeURIComponent(destino)}` : ''}`}>Entrar</Link>
          </p>
        </div>
      </div>
    </div>
  );
}
