# Ambiente observado

Verificação em 03/11/2026 no ambiente de preparação:
- Node.js: v24.22.1 (o roteiro pede Node.js 22).
- Git: 2.51.1.
- dotnet, mysql, mysqld, docker e gh: não encontrados no PATH.
- MySQL Workbench, VS Code/C# Dev Kit e conta GitHub do estudante: não verificados.

Instale os pré-requisitos do roteiro na sua máquina. Não há credenciais no pacote. O histórico usa uma identidade técnica, não se faz passar pelo estudante. A branch aula-1 conserva seis commits de preparação; não foi integrada à main; a execução dos testes SQL e a publicação remota ainda estão pendentes.

Atualização: SDK .NET 10.0.403 disponibilizado temporariamente, com compilação da solução executada. O build usou -m:1 e -p:UseSharedCompilation=false devido à limitação de comunicação entre processos deste ambiente. MySQL continua indisponível.
