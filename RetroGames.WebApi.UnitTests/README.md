# Testes unitários

xUnit v3 + Moq, sem banco e sem servidor (Espec. 9; roteiro da Aula 9). Os repositórios são sempre
simulados com `Mock<...>`; nenhum teste usa connection string. Cada teste segue Arrange, Act, Assert, verifica
um único comportamento e tem o nome `Metodo_Cenario_Resultado`. Nenhum teste depende de outro: cada classe
cria mocks novos no construtor, que o xUnit executa para cada teste.

```sh
dotnet test RetroGames.sln
```

## Services (Aula 9)

| Classe | Cenários |
|---|---|
| `PlataformaServiceTests` | Id inexistente (100); nome já cadastrado (101, sem gravar); banco recusa por duplicidade (101); nome livre devolve o id gerado; alterar mantendo o próprio nome; alterar inexistente; excluir com jogos (102, sem excluir); excluir sem jogos |
| `JogoServiceTests` | Plataforma, condição e gênero inexistentes (100, 104, 105, sem gravar); filtro por gênero envia só os ids daquele gênero; sem gênero não consulta os vínculos nem restringe os ids; montagem com LINQ (plataforma, condição e gêneros); RN04 por perfil; id inexistente (103); desativar inexistente (103, sem desativar); desativar só marca como inativo; gêneros repetidos gravados uma vez |
| `GeneroServiceTests` | Listar devolve os gêneros do Repository |
| `CondicaoJogoServiceTests` | Listar devolve as condições do Repository |

## Desafio: Módulo de Compras

| Classe | Cenários |
|---|---|
| `PedidoServiceTests` | Jogo inexistente e jogo inativo (110, sem gravar); estoque insuficiente (111, com o nome do jogo, sem gravar); estoque que acaba durante a transação (111, sem `Complete()`); preço enviado pelo front é ignorado e o total usa o preço do banco; caminho feliz (Pendente, data UTC, usuário da sessão, itens e baixa de estoque); Meus pedidos só do usuário logado, na ordem do repositório; sem pedidos não consulta itens |
| `AutenticacaoServiceTests` | Cadastro com e-mail já usado (106, sem gravar); duplicidade recusada pelo banco (106); cadastro novo cria Cliente ativo com senha em hash |
| `PedidoValidatorsTests`, `AutenticacaoValidatorsTests` | Pedido sem itens, quantidade fora de 1 a 10, jogo repetido; regras da senha no cadastro |

## Demais camadas

| Pasta | Classe | O que verifica |
|---|---|---|
| `Validators` | `PlataformaValidatorsTests`, `JogoValidatorsTests`, `AutenticacaoValidatorsTests` | Casos válidos e inválidos, campo e mensagem de cada erro |
| `Controllers` | `PlataformaControllerTests` | 200/201 com envelope, `CreatedAtAction` apontando para o novo recurso e id vindo da rota |
| `Middlewares` | `GlobalExceptionHandlerTests` | 409/404 com o código do catálogo e 500 sem detalhes internos |

## Rede de segurança (Aula 9, etapa 4)

Executado em 25/11/2026: com a conferência de nome duplicado comentada no `PlataformaService.InserirAsync`,
o `dotnet test` mostrou **1 falha em 46**:

```
PlataformaServiceTests.Inserir_NomeJaExiste_LancaNomeDuplicado [FAIL]
  Assert.Throws() Failure: No exception was thrown
  Expected: typeof(RetroGames.Services.BusinessRuleException)
```

O nome do teste apontou direto a regra quebrada (RN01 na inclusão). A alteração foi desfeita, o arquivo voltou a
ser idêntico ao versionado e os 46 testes voltaram a passar.

O SQL dos repositórios não é coberto aqui: ele é conferido no MySQL (`database/007` e `008`) e cada rota, com
`docs/conferencia_api.sh`.
