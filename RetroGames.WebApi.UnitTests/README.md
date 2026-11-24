# Testes unitários

xUnit v3 + Moq, sem banco e sem servidor (Espec. 9). Cada teste segue o padrão Arrange, Act, Assert e verifica
um único comportamento; o nome diz o cenário e o resultado esperado.

| Pasta | Classe | O que verifica |
|---|---|---|
| `Services` | `PlataformaServiceTests` | RN01 (inclusive a duplicidade recusada pelo banco), RN02, 404 e o próprio nome na alteração |
| `Services` | `JogoServiceTests` | Filtro por gênero com LINQ, RN04 por perfil, montagem do detalhe, RN05 (100, 104, 105) e RN03 |
| `Validators` | `PlataformaValidatorsTests`, `JogoValidatorsTests`, `AutenticacaoValidatorsTests` | Casos válidos e inválidos, campo e mensagem de cada erro |
| `Controllers` | `PlataformaControllerTests` | 200/201 com envelope, `CreatedAtAction` apontando para o novo recurso e id vindo da rota |
| `Middlewares` | `GlobalExceptionHandlerTests` | 409/404 com o código do catálogo e 500 sem detalhes internos |

Execute na raiz da solução:

```sh
dotnet test RetroGames.sln
```

O SQL dos repositórios não é coberto aqui: ele é conferido no MySQL (`database/007_verificar_repository.sql`) e
cada rota, no Swagger.
