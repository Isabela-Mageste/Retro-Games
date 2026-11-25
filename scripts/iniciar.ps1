# Inicia o ambiente local do RetroGames no Windows: MySQL, API (porta 5080) e front-end (porta 5173).
# Uso: dê dois cliques em iniciar.cmd, na raiz do repositório, ou rode:
#   powershell -ExecutionPolicy Bypass -File scripts\iniciar.ps1
#
# Cada peça abre na sua própria janela, com o log visível. Para parar, feche as janelas.
# O script procura as ferramentas no PATH e, se não achar, nas pastas de instalação por usuário.

param(
    # Pasta de dados de um MySQL sem serviço do Windows (instalação por zip). Ignorada se o MySQL já estiver rodando.
    [string]$DadosMySql = "$env:LOCALAPPDATA\Programs\mysql-data"
)

$ErrorActionPreference = 'Stop'
$raiz = Split-Path -Parent $PSScriptRoot

function Escrever($texto, $cor = 'Gray') { Write-Host $texto -ForegroundColor $cor }

function Encontrar([string]$comando, [string[]]$candidatos) {
    $noPath = Get-Command $comando -ErrorAction SilentlyContinue
    if ($noPath) { return $noPath.Source }
    foreach ($caminho in $candidatos) {
        if ($caminho -and (Test-Path $caminho)) { return $caminho }
    }
    return $null
}

function PortaEmUso([int]$porta) {
    return [bool](Get-NetTCPConnection -LocalPort $porta -State Listen -ErrorAction SilentlyContinue)
}

function EsperarPorta([int]$porta, [int]$segundos) {
    for ($i = 0; $i -lt $segundos; $i++) {
        if (PortaEmUso $porta) { return $true }
        Start-Sleep -Seconds 1
    }
    return $false
}

function UltimaPasta([string]$padrao) {
    $pasta = Get-ChildItem $padrao -Directory -ErrorAction SilentlyContinue | Sort-Object Name | Select-Object -Last 1
    if ($pasta) { return $pasta.FullName }
    return $null
}

function AbrirJanela([string]$titulo, [string]$comandos) {
    $script = "`$Host.UI.RawUI.WindowTitle = '$titulo'; $comandos"
    Start-Process powershell -ArgumentList '-NoExit', '-ExecutionPolicy', 'Bypass', '-Command', $script | Out-Null
}

Escrever "RetroGames - iniciando o ambiente local" 'Magenta'

# 0. Ferramentas e configuração
$pastaNode = UltimaPasta "$env:LOCALAPPDATA\Programs\node-v22*"
$pastaMySql = UltimaPasta "$env:LOCALAPPDATA\Programs\mysql-8*"
$dotnet = Encontrar 'dotnet' @("$env:LOCALAPPDATA\Microsoft\dotnet\dotnet.exe", "$env:ProgramFiles\dotnet\dotnet.exe")
$node = Encontrar 'node' @($(if ($pastaNode) { "$pastaNode\node.exe" }), "$env:ProgramFiles\nodejs\node.exe")
$mysqld = Encontrar 'mysqld' @($(if ($pastaMySql) { "$pastaMySql\bin\mysqld.exe" }))

if (-not $dotnet) { Escrever "ERRO: .NET SDK 10 não encontrado. Instale-o e rode o script de novo." 'Red'; exit 1 }
if (-not $node) { Escrever "ERRO: Node.js 22 não encontrado. Instale-o e rode o script de novo." 'Red'; exit 1 }
if (-not (Test-Path "$raiz\.env")) {
    Escrever "ERRO: falta o arquivo .env na raiz. Copie .env.example para .env e preencha a conexão e o AdminInicial." 'Red'
    exit 1
}
$pastaDotnet = Split-Path -Parent $dotnet
$pastaNodeUsada = Split-Path -Parent $node

# 1. MySQL: a API não sobe sem o banco.
if (PortaEmUso 3306) {
    Escrever "[ok] MySQL já está rodando na porta 3306." 'Green'
} elseif ($mysqld -and (Test-Path $DadosMySql)) {
    $baseMySql = Split-Path -Parent (Split-Path -Parent $mysqld)
    Escrever "[..] Iniciando o MySQL..."
    AbrirJanela 'RetroGames - MySQL' "& '$mysqld' --basedir='$baseMySql' --datadir='$DadosMySql' --port=3306 --bind-address=127.0.0.1 --console"
    if (-not (EsperarPorta 3306 40)) { Escrever "ERRO: o MySQL não respondeu na porta 3306. Veja a janela do MySQL." 'Red'; exit 1 }
    Escrever "[ok] MySQL no ar." 'Green'
} else {
    Escrever "ERRO: o MySQL não está rodando na porta 3306. Inicie o serviço do MySQL (ou o MySQL Workbench) e rode o script de novo." 'Red'
    exit 1
}

# 2. API
if (PortaEmUso 5080) {
    Escrever "[ok] A API já está rodando na porta 5080." 'Green'
} else {
    Escrever "[..] Iniciando a API (a primeira compilação pode levar um minuto)..."
    AbrirJanela 'RetroGames - API' "`$env:PATH = '$pastaDotnet;' + `$env:PATH; Set-Location '$raiz'; dotnet run --project RetroGames.WebApi --launch-profile http"
    if (-not (EsperarPorta 5080 180)) { Escrever "ERRO: a API não subiu na porta 5080. Veja a mensagem na janela da API." 'Red'; exit 1 }
    Escrever "[ok] API no ar: http://localhost:5080/swagger" 'Green'
}

# 3. Front-end
if (PortaEmUso 5173) {
    Escrever "[ok] O front-end já está rodando na porta 5173." 'Green'
} else {
    Escrever "[..] Iniciando o front-end..."
    AbrirJanela 'RetroGames - Front-end' "`$env:PATH = '$pastaNodeUsada;' + `$env:PATH; Set-Location '$raiz\retrogames.web'; if (-not (Test-Path node_modules)) { npm install }; npm run dev"
    if (-not (EsperarPorta 5173 180)) { Escrever "ERRO: o front-end não subiu na porta 5173. Veja a janela do front-end." 'Red'; exit 1 }
    Escrever "[ok] Front-end no ar." 'Green'
}

Escrever "Abrindo http://localhost:5173 ..." 'Cyan'
Start-Process 'http://localhost:5173'
Escrever "Pronto. Para parar, feche as janelas do MySQL, da API e do front-end." 'Magenta'
