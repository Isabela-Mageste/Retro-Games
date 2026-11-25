import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros.js';
import useAutenticacao from '../../hooks/useAutenticacao.js';

// Só aceita destinos internos ("/admin/jogos"), nunca outro site ("//exemplo.com").
function destinoSeguro(voltarPara) {
  return voltarPara && voltarPara.startsWith('/') && !voltarPara.startsWith('//') ? voltarPara : '/';
}

export default function Login() {
  const { entrar, estaAutenticado, carregando } = useAutenticacao();
  const [parametros] = useSearchParams();
  const navegar = useNavigate();
  const destino = destinoSeguro(parametros.get('voltarPara'));

  const [email, definirEmail] = useState('');
  const [senha, definirSenha] = useState('');
  const [enviando, definirEnviando] = useState(false);
  const [erro, definirErro] = useState(null);
  const [errosCampos, definirErrosCampos] = useState({});

  // Quem já está logado não precisa ver o formulário.
  useEffect(() => {
    if (!carregando && estaAutenticado) navegar(destino, { replace: true });
  }, [carregando, estaAutenticado, destino, navegar]);

  async function aoEnviar(evento) {
    evento.preventDefault();
    definirEnviando(true);
    definirErro(null);
    definirErrosCampos({});
    try {
      await entrar(email.trim(), senha);
      navegar(destino, { replace: true });
    } catch (falha) {
      const porCampo = obterErrosDosCampos(falha);
      definirErrosCampos(porCampo);
      if (Object.keys(porCampo).length === 0) definirErro(obterMensagemErro(falha));
      definirSenha('');
    } finally {
      definirEnviando(false);
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-12 col-sm-10 col-md-7 col-lg-5">
        <div className="painel p-4 p-md-5">
          <p className="marca-retro mb-2">Player 1</p>
          <h1 className="h3 mb-1">Entrar</h1>
          <p className="text-body-secondary mb-4">Entre para comprar e acompanhar seus pedidos.</p>

          {destino !== '/' && (
            <div className="alert alert-info py-2 small" role="status">
              Faça login para continuar.
            </div>
          )}

          {erro && <div className="alert alert-danger" role="alert">{erro}</div>}

          <form onSubmit={aoEnviar} noValidate>
            <div className="mb-3">
              <label htmlFor="email" className="form-label">E-mail</label>
              <input
                id="email"
                type="email"
                className={`form-control ${errosCampos.email ? 'is-invalid' : ''}`}
                aria-describedby={errosCampos.email ? 'erroEmail' : undefined}
                autoComplete="username"
                value={email}
                onChange={(evento) => definirEmail(evento.target.value)}
                required
                autoFocus
              />
              {errosCampos.email && <div id="erroEmail" className="invalid-feedback">{errosCampos.email}</div>}
            </div>
            <div className="mb-4">
              <label htmlFor="senha" className="form-label">Senha</label>
              <input
                id="senha"
                type="password"
                className={`form-control ${errosCampos.senha ? 'is-invalid' : ''}`}
                aria-describedby={errosCampos.senha ? 'erroSenha' : undefined}
                autoComplete="current-password"
                value={senha}
                onChange={(evento) => definirSenha(evento.target.value)}
                required
              />
              {errosCampos.senha && <div id="erroSenha" className="invalid-feedback">{errosCampos.senha}</div>}
            </div>
            <button type="submit" className="btn btn-primary w-100" disabled={enviando}>
              {enviando ? (
                <><span className="spinner-border spinner-border-sm me-2" aria-hidden="true" />Entrando...</>
              ) : 'Entrar'}
            </button>
          </form>

          <p className="text-center mt-4 mb-2">
            Ainda não tem conta?{' '}
            <Link to={`/cadastro${destino !== '/' ? `?voltarPara=${encodeURIComponent(destino)}` : ''}`}>Criar conta</Link>
          </p>
          <p className="text-center small mb-0">
            <Link to="/">Voltar ao catálogo</Link>
          </p>
        </div>
      </div>
    </div>
  );
}
