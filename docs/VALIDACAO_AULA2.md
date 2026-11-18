# Validação da Aula 2

Verificação executada em 18/11/2026 sobre a versão validada da Aula 1 recuperada do ZIP. Ambiente com .NET SDK 10.0.404 e MySQL Community Server 8.0.41.

## Resultados

| Item | Resultado |
|---|---|
| Solução completa | Build com zero erros e zero avisos |
| Métodos Repository | 39 verificações de comportamento aprovadas contra MySQL real |
| SQL de conferência | Scripts 006 e 007 executados com sucesso |
| Dados após testes | Sete plataformas e 24 jogos, com vínculos originais preservados |
| Parametrização | Valores de usuário enviados em parâmetros, inclusive listas de IDs |
| Estrutura | Cinco repositórios, cinco interfaces, conjunto com cinco propriedades e as três classes base |
| Conexão | .env de auditoria criado localmente e ignorado pelo Git. ZIP contém .env.example sem credenciais |
| Dependência | Somente RetroGames.Repository referencia diretamente MySqlConnector nos projetos da solução |

Foram exercitados todos os métodos públicos dos cinco repositórios. As verificações cobriram busca existente e ausente, nome único com ID ignorado, INSERT e UPDATE duplicados, exclusão com dependentes, geração de ID, parâmetros com aspas e texto SQL, filtros isolados e combinados, páginas sem sobreposição, página além do fim, jogos inativos, lista de IDs vazia, descrição nula, preço decimal, data UTC e busca literal por %, _ e !.

Os vínculos foram inseridos em lote. Duplicidade e referência inválida no segundo item desfizeram o INSERT inteiro. A desativação preservou o jogo e seus vínculos. Uma transação ambiente com dois comandos foi desfeita pelo chamador, e outra foi confirmada com Complete. Após erros e cancelamento, as leituras continuaram funcionando com pool limitado a uma conexão.

O contador de 39 verificações foi obtido por um programa temporário de auditoria fora da solução, chamando as classes C# diretamente. A suíte xUnit do projeto ainda retorna zero testes descobertos. Os testes unitários de Service, validators e controllers serão acrescentados nas aulas correspondentes.

## Pendências do ambiente do estudante

- Preencher .env com a conexão do próprio MySQL, obtida pela consulta no Workbench.
- Executar a conferência na interface do Workbench. A entrega foi verificada diretamente no servidor MySQL.
- Conectar a conta GitHub para publicar a branch aula-2 e sincronizar o quadro remoto.

O quadro local foi atualizado com o avanço dos repositórios. Os commits das cinco etapas estão no histórico integrado.
