# Aula 2 — Repository

## Configuração

A orientação específica da Aula 2 para .env foi escolhida pelo estudante diante da diferença em relação ao AGENTS.md anterior. Execute database/006_consultar_conexao.sql no Workbench com seu schema selecionado. Copie .env.example para .env, na raiz, e preencha o usuário, porta, schema e senha locais. Use localhost e somente a parte do usuário antes de @. O arquivo .env está ignorado no Git.

A conexão real do computador do estudante depende desses dados locais. O ZIP inclui o modelo sem credenciais. A API lerá .env na Aula 4, conforme o roteiro. RepositoryConfig recebe o texto pelo chamador. O módulo de Repository já pode ser usado por um programa C# de verificação.

## Contratos

Todos os métodos de I/O são assíncronos e aceitam CancellationToken opcional. ListarAsync de jogos retorna a tupla (Jogos, Total), com Total do tipo long. A página inicial é 1 e o tamanho padrão é 5. Página ou tamanho menores que 1 geram ArgumentOutOfRangeException como proteção técnica da paginação.

O filtro idsJogos nulo desativa o filtro. Uma lista vazia seleciona zero jogos. O filtro de nome busca um trecho literal, inclusive quando contém %, _ ou !. As listas de IDs usam nomes de parâmetros gerados internamente. Valores nunca entram no texto SQL. A ordenação dos jogos usa nome e ID para estabilizar as páginas.

BuscarPorIdAsync retorna inclusive jogos inativos para que o Service decida a visibilidade. AlterarAsync de Jogo preserva DataCadastro. DesativarAsync altera apenas Ativo. ListarPorJogosAsync retorna vazio para lista vazia. InserirAsync de JogoGenero insere todos os vínculos num único INSERT, permitindo que o banco reverta o comando completo se algum vínculo falhar. Repetições de gênero são duplicidades e seguem a tradução central de 1062.

O Repository participa da transação ambiente do chamador. O Service futuro deve abrir TransactionScope para operações que envolvam múltiplos comandos. A camada Repository não inicia nem confirma transações. Contagem e página usam o mesmo filtro em dois SELECTs. Para consistência de snapshot sob gravações simultâneas, cabe ao chamador definir a transação adequada.

## Conferência local

1. Na raiz, execute `dotnet build RetroGames.sln`.
2. No Workbench, selecione o schema da Aula 1 e execute `database/006_consultar_conexao.sql`.
3. Copie `.env.example` para `.env` e preencha os dados locais. `git check-ignore .env` deve mostrar `.env`, e `git status` deve omiti-lo.
4. Execute `database/007_verificar_repository.sql` no banco de exercício. Ele usa variáveis de sessão, consulta cada tabela isoladamente e desfaz as gravações com `ROLLBACK`.
5. Consulte `docs/VALIDACAO_AULA2.md` para os resultados dos testes executados nesta entrega.

O script 007 pressupõe os dados da Aula 1. O SQL foi executado em MySQL 8.0.41 por um cliente automatizado. A conferência na interface do Workbench do estudante permanece pendente.

## Instanciação para as próximas aulas

O construtor de cada repositório recebe RepositoryConfig. RepositorySet recebe as cinco interfaces pelo construtor, o que permite usar mocks na camada Service. A configuração e a injeção dessas classes pela API estão previstas para a Aula 4.

```csharp
var configuracao = new RepositoryConfig { ConnectionString = textoDaConexao };
IRepositorySet repositorios = new RepositorySet(
    new PlataformaRepository(configuracao),
    new JogoRepository(configuracao),
    new JogoGeneroRepository(configuracao),
    new GeneroRepository(configuracao),
    new CondicaoJogoRepository(configuracao));
var (jogos, total) = await repositorios.Jogo.ListarAsync(nome: "Super", pagina: 2);
```

O trecho é um exemplo de uso. `textoDaConexao` deve vir da configuração local do chamador, sem credenciais no código.
