import React, { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { pedidoApi } from '../../api/servicos.js';
import { obterMensagemErro } from '../../api/erros.js';
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado.jsx';
import { formatarDataHora, formatarPreco } from '../../lib/formato.js';

// Cores da situação do pedido, com texto sempre visível (a cor não é o único sinal).
const CLASSES_SITUACAO = {
  Pendente: 'badge-pendente',
  Pago: 'badge-ativo',
  Enviado: 'badge-genero',
  Cancelado: 'badge-inativo',
};

// C05: Meus pedidos (/meus-pedidos). A API devolve só os pedidos do usuário logado (a sessão diz
// quem é), do mais recente para o mais antigo.
export default function MeusPedidos() {
  const location = useLocation();
  const navegar = useNavigate();
  const [pedidoCriado] = useState(location.state?.pedidoCriado ?? null);
  const [pedidos, definirPedidos] = useState([]);
  const [carregando, definirCarregando] = useState(true);
  const [erro, definirErro] = useState(null);
  const [tentativa, definirTentativa] = useState(0);

  // Limpa o state da navegação para o aviso não voltar num F5.
  useEffect(() => {
    if (location.state?.pedidoCriado) navegar(location.pathname, { replace: true, state: null });
  }, [location, navegar]);

  useEffect(() => {
    let ativo = true;
    definirCarregando(true);
    definirErro(null);
    pedidoApi
      .meus()
      .then((dados) => ativo && definirPedidos(dados))
      .catch((falha) => ativo && definirErro(obterMensagemErro(falha)))
      .finally(() => ativo && definirCarregando(false));
    return () => {
      ativo = false;
    };
  }, [tentativa]);

  return (
    <>
      <header className="mb-4">
        <p className="plataforma-rotulo mb-1">Sua conta</p>
        <h1 className="h2 mb-0">Meus pedidos</h1>
      </header>

      {pedidoCriado && (
        <div className="alert alert-success" role="status">
          <p className="fw-semibold mb-1">Pedido #{pedidoCriado} realizado!</p>
          <p className="mb-0">Ele está <strong>Pendente</strong> até a confirmação do pagamento. Obrigado pela compra!</p>
        </div>
      )}

      {carregando && <Carregando texto="Carregando seus pedidos..." />}
      {!carregando && erro && <MensagemErro mensagem={erro} aoTentarNovamente={() => definirTentativa((v) => v + 1)} />}
      {!carregando && !erro && pedidos.length === 0 && (
        <EstadoVazio titulo="Você ainda não fez nenhum pedido.">
          <p>Escolha seus clássicos no catálogo, adicione ao carrinho e finalize a compra.</p>
          <Link to="/" className="btn btn-primary">Ver o catálogo</Link>
        </EstadoVazio>
      )}

      {!carregando && !erro && pedidos.length > 0 && (
        <ol className="list-unstyled d-flex flex-column gap-3 mb-0" aria-label="Pedidos, do mais recente para o mais antigo">
          {pedidos.map((pedido) => (
            <li key={pedido.id} className={`painel p-3 p-md-4 ${pedido.id === pedidoCriado ? 'pedido-novo' : ''}`}>
              <div className="d-flex flex-wrap justify-content-between align-items-start gap-2 mb-3">
                <div>
                  <h2 className="h5 mb-0">Pedido #{pedido.id}</h2>
                  <p className="small text-body-secondary mb-0">{formatarDataHora(pedido.dataPedido)}</p>
                </div>
                <div className="text-end">
                  <span className={`badge ${CLASSES_SITUACAO[pedido.situacao] ?? 'badge-genero'}`}>{pedido.situacao}</span>
                  <p className="preco fs-5 mb-0 mt-1">{formatarPreco(pedido.total)}</p>
                </div>
              </div>

              <div className="table-responsive">
                <table className="table table-sm align-middle">
                  <caption className="visually-hidden">Itens do pedido {pedido.id}</caption>
                  <thead>
                    <tr>
                      <th scope="col">Jogo</th>
                      <th scope="col" className="text-center">Qtd.</th>
                      <th scope="col" className="text-end d-none d-sm-table-cell">Preço pago</th>
                      <th scope="col" className="text-end">Subtotal</th>
                    </tr>
                  </thead>
                  <tbody>
                    {pedido.itens.map((item) => (
                      <tr key={item.idJogo}>
                        <td>{item.nomeJogo}</td>
                        <td className="text-center">{item.quantidade}</td>
                        <td className="text-end d-none d-sm-table-cell">{formatarPreco(item.precoUnitario)}</td>
                        <td className="text-end">{formatarPreco(item.subtotal)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </li>
          ))}
        </ol>
      )}
    </>
  );
}
