#!/usr/bin/env bash
# Conferência das respostas pedidas nos roteiros das Aulas 4, 5 e 6
# (Aula 5 etapa 4 e Aula 6 etapa 4: "todas as respostas da API").
#
# Pré-requisitos: MySQL com os scripts 001 e 002, .env preenchido e a API no ar
# (dotnet run --project RetroGames.WebApi). Execute na raiz, no Git Bash:
#   bash docs/conferencia_api.sh
#
# O script cria e exclui duas plataformas de teste. Ele também inclui o jogo "Contra"
# e o desativa no fim (RN03: jogo nunca é apagado); rodá-lo de novo cria outro.
RAIZ="$(cd "$(dirname "$0")/.." && pwd)"; B=http://localhost:5080/api
T="$(mktemp -d)"; C="$T/cookie.txt"; trap 'rm -rf "$T"' EXIT
SENHA=$(grep AdminInicial__Senha "$RAIZ/.env" | cut -d'"' -f2)
OK=0; FALHA=0

# chk "descrição" statusEsperado codigoEsperado [args do curl...]
chk() {
  local d="$1" se="$2" ce="$3"; shift 3
  local st; st=$(curl -s -b "$C" -c "$C" -o "$T/r.json" -w "%{http_code}" "$@")
  local cd; cd=$(grep -o '"responseCode":[0-9]*' "$T/r.json" | head -1 | cut -d: -f2)
  if [[ "$st" == "$se" && "$cd" == "$ce" ]]; then OK=$((OK+1)); echo "OK     $d ($st/$cd)";
  else FALHA=$((FALHA+1)); echo "FALHA  $d: esperado $se/$ce, veio $st/$cd -> $(head -c 200 "$T/r.json")"; fi
}
json() { printf '%s' "$1" > "$T/b.json"; }
J=(-H "Content-Type: application/json; charset=utf-8")

curl -s --retry 40 --retry-connrefused --retry-delay 1 -o /dev/null http://localhost:5080/api/Genero

echo "--- Aula 4"
[[ $(curl -s -o /dev/null -w "%{http_code}" http://localhost:5080/swagger/index.html) == 200 ]] && { OK=$((OK+1)); echo "OK     Swagger abre sem login"; } || { FALHA=$((FALHA+1)); echo "FALHA  Swagger"; }
[[ $(curl -s -o /dev/null -w "%{http_code}" http://localhost:5080/openapi/v1.json) == 200 ]] && { OK=$((OK+1)); echo "OK     /openapi/v1.json sem login"; } || { FALHA=$((FALHA+1)); echo "FALHA  openapi"; }
chk "GET /api/Plataforma" 200 0 $B/Plataforma
chk "GET /api/Plataforma/99" 404 100 $B/Plataforma/99
chk "GET /api/Genero" 200 0 $B/Genero
chk "GET /api/CondicaoJogo" 200 0 $B/CondicaoJogo
chk "GET /api/Jogo" 200 0 $B/Jogo
grep -q '"itens":\[.*"pagina":1,"tamanhoPagina":12,"totalItens":[0-9]*,"totalPaginas":[0-9]*' "$T/r.json" && { OK=$((OK+1)); echo "OK     página com itens, pagina, tamanhoPagina, totalItens e totalPaginas"; } || { FALHA=$((FALHA+1)); echo "FALHA  campos da página: $(grep -o '"pagina".*' "$T/r.json")"; }
grep -q '"nome":"NBA Jam"' "$T/r.json" && { FALHA=$((FALHA+1)); echo "FALHA  inativo apareceu no catálogo"; } || { OK=$((OK+1)); echo "OK     listagem só com ativos"; }
chk "GET /api/Jogo?busca=zelda" 200 0 "$B/Jogo?busca=zelda"
grep -q '"totalItens":2' "$T/r.json" && { OK=$((OK+1)); echo "OK     busca zelda = 2 jogos"; } || { FALHA=$((FALHA+1)); echo "FALHA  busca zelda"; }
chk "GET /api/Jogo?idPlataforma=2" 200 0 "$B/Jogo?idPlataforma=2"
grep -q '"totalItens":6' "$T/r.json" && { OK=$((OK+1)); echo "OK     plataforma 2 = 6 jogos"; } || { FALHA=$((FALHA+1)); echo "FALHA  filtro plataforma"; }
chk "GET /api/Jogo?idGenero=4" 200 0 "$B/Jogo?idGenero=4"
grep -q '"totalItens":3' "$T/r.json" && { OK=$((OK+1)); echo "OK     gênero RPG = 3 jogos"; } || { FALHA=$((FALHA+1)); echo "FALHA  filtro gênero"; }
P1=$(curl -s "$B/Jogo?tamanhoPagina=5" | grep -o '"id":[0-9]*,"nome"' | tr '\n' ' ')
P2=$(curl -s "$B/Jogo?tamanhoPagina=5&pagina=2" | grep -o '"id":[0-9]*,"nome"' | tr '\n' ' ')
[[ -n "$P2" && "$P1" != "$P2" ]] && { OK=$((OK+1)); echo "OK     página 2 traz jogos diferentes da 1"; } || { FALHA=$((FALHA+1)); echo "FALHA  paginação"; }
chk "GET /api/Jogo/9999" 404 103 $B/Jogo/9999
chk "GET /api/Jogo/1 (detalhe)" 200 0 $B/Jogo/1

echo "--- Aula 5 (formato) e Aula 6 (acesso anônimo)"
json '{"nome":"Super Nintendo","fabricante":"Nintendo","anoLancamento":1990}'
chk "POST /api/Plataforma sem login" 401 4 "${J[@]}" -X POST $B/Plataforma --data-binary @"$T/b.json"
chk "GET /api/Autenticacao/eu sem login" 401 4 $B/Autenticacao/eu
json '{"email":"admin@retrogames.local","senha":"errada"}'
chk "login com senha errada" 401 6 "${J[@]}" -X POST $B/Autenticacao/login --data-binary @"$T/b.json"
json '{"email":"abc","senha":"x"}'
chk "login com e-mail mal formado" 400 3 "${J[@]}" -X POST $B/Autenticacao/login --data-binary @"$T/b.json"
chk "GET /api/Jogo?incluirInativos=true sem login" 200 0 "$B/Jogo?incluirInativos=true"
ANONIMO=$(grep -o '"totalItens":[0-9]*' "$T/r.json" | cut -d: -f2)
ATIVOS=$(curl -s "$B/Jogo" | grep -o '"totalItens":[0-9]*' | cut -d: -f2)
[[ "$ANONIMO" == "$ATIVOS" ]] && { OK=$((OK+1)); echo "OK     sem login: parâmetro ignorado, só ativos ($ANONIMO)"; } || { FALHA=$((FALHA+1)); echo "FALHA  incluirInativos sem login"; }
chk "GET /api/Jogo/24 (inativo) sem login" 404 103 $B/Jogo/24
chk "GET /api/Jogo?pagina=0" 400 3 "$B/Jogo?pagina=0"

echo "--- Aula 6 (administrador)"
json "{\"email\":\"admin@retrogames.local\",\"senha\":\"$SENHA\"}"
chk "login do administrador" 200 0 "${J[@]}" -X POST $B/Autenticacao/login --data-binary @"$T/b.json"
grep -q "retrogames.sessao" "$C" && { OK=$((OK+1)); echo "OK     cookie retrogames.sessao gravado"; } || { FALHA=$((FALHA+1)); echo "FALHA  cookie"; }
grep -qi "hash\|senha" "$T/r.json" && { FALHA=$((FALHA+1)); echo "FALHA  resposta de login expõe senha/hash"; } || { OK=$((OK+1)); echo "OK     login não expõe o hash"; }
chk "GET eu logado" 200 0 $B/Autenticacao/eu
chk "incluirInativos=true com admin" 200 0 "$B/Jogo?incluirInativos=true"
ADMIN=$(grep -o '"totalItens":[0-9]*' "$T/r.json" | cut -d: -f2)
[[ "$ADMIN" -gt "$ANONIMO" ]] && { OK=$((OK+1)); echo "OK     admin vê também os inativos ($ADMIN > $ANONIMO)"; } || { FALHA=$((FALHA+1)); echo "FALHA  incluirInativos admin"; }

echo "--- Aula 5 (gravação, logado como admin)"
json '{"nome":"  Atari 2600 ","fabricante":"Atari","anoLancamento":1977}'
chk "POST plataforma" 201 0 "${J[@]}" -X POST $B/Plataforma --data-binary @"$T/b.json"
ID=$(grep -o '"id":[0-9]*' "$T/r.json" | head -1 | cut -d: -f2)
grep -q '"nome":"Atari 2600"' "$T/r.json" && { OK=$((OK+1)); echo "OK     nome gravado sem espaços"; } || { FALHA=$((FALHA+1)); echo "FALHA  trim"; }
LOC=$(curl -s -b "$C" -D - -o /dev/null "${J[@]}" -X POST $B/Plataforma --data-binary '{"nome":"Neo Geo","fabricante":"SNK","anoLancamento":1990}' | grep -i '^location:' | tr -d '\r')
ID2=${LOC##*/}; [[ "$LOC" == *"/api/Plataforma/"* ]] && { OK=$((OK+1)); echo "OK     201 traz cabeçalho Location ($LOC)"; } || { FALHA=$((FALHA+1)); echo "FALHA  Location"; }
json '{"nome":"NES","fabricante":"Nintendo","anoLancamento":1983}'
chk "POST nome repetido" 409 101 "${J[@]}" -X POST $B/Plataforma --data-binary @"$T/b.json"
json '{"nome":"","fabricante":"Nintendo","anoLancamento":1800}'
chk "POST campos inválidos" 400 3 "${J[@]}" -X POST $B/Plataforma --data-binary @"$T/b.json"
grep -q '"nome":\["Informe o nome."\]' "$T/r.json" && grep -q '"anoLancamento"' "$T/r.json" && { OK=$((OK+1)); echo "OK     erros indexados por campo em data"; } || { FALHA=$((FALHA+1)); echo "FALHA  erros por campo"; }
json '{"nome":"X,"fabricante":1}'
chk "POST JSON malformado" 400 3 "${J[@]}" -X POST $B/Plataforma --data-binary @"$T/b.json"
json '{"nome":"Atari 2600","fabricante":"Atari Inc.","anoLancamento":1977}'
chk "PUT mantendo o próprio nome" 200 0 "${J[@]}" -X PUT $B/Plataforma/$ID --data-binary @"$T/b.json"
json '{"nome":"NES","fabricante":"Atari","anoLancamento":1977}'
chk "PUT com nome de outra" 409 101 "${J[@]}" -X PUT $B/Plataforma/$ID --data-binary @"$T/b.json"
chk "PUT plataforma 99" 404 100 "${J[@]}" -X PUT $B/Plataforma/99 --data-binary @"$T/b.json"
chk "DELETE plataforma com jogos" 409 102 -X DELETE $B/Plataforma/1
chk "DELETE plataforma sem jogos" 200 0 -X DELETE $B/Plataforma/$ID
chk "DELETE plataforma Neo Geo" 200 0 -X DELETE $B/Plataforma/$ID2
chk "DELETE plataforma 99" 404 100 -X DELETE $B/Plataforma/99
json '{"nome":"Teste","anoLancamento":1990,"preco":10,"estoque":1,"idPlataforma":99,"idCondicaoJogo":2,"idsGeneros":[1]}'
chk "POST jogo com plataforma inexistente" 404 100 "${J[@]}" -X POST $B/Jogo --data-binary @"$T/b.json"
json '{"nome":"Teste","anoLancamento":1990,"preco":10,"estoque":1,"idPlataforma":1,"idCondicaoJogo":9,"idsGeneros":[1]}'
chk "POST jogo com condição inexistente" 404 104 "${J[@]}" -X POST $B/Jogo --data-binary @"$T/b.json"
json '{"nome":"Teste","anoLancamento":1990,"preco":10,"estoque":1,"idPlataforma":1,"idCondicaoJogo":2,"idsGeneros":[99]}'
chk "POST jogo com gênero inexistente" 404 105 "${J[@]}" -X POST $B/Jogo --data-binary @"$T/b.json"
json '{"nome":"","anoLancamento":1990,"preco":0,"estoque":-1,"idPlataforma":1,"idCondicaoJogo":2,"idsGeneros":[]}'
chk "POST jogo inválido (RN06)" 400 3 "${J[@]}" -X POST $B/Jogo --data-binary @"$T/b.json"
json '{"nome":"Contra","anoLancamento":1988,"preco":79.9,"estoque":2,"idPlataforma":1,"idCondicaoJogo":2,"idsGeneros":[1,3]}'
chk "POST jogo com gêneros" 201 0 "${J[@]}" -X POST $B/Jogo --data-binary @"$T/b.json"
JID=$(grep -o '"id":[0-9]*' "$T/r.json" | head -1 | cut -d: -f2)
printf '{"nome":"Contra","descricao":"Run and gun cl\xc3\xa1ssico.","anoLancamento":1988,"preco":89.9,"estoque":3,"ativo":true,"idPlataforma":1,"idCondicaoJogo":3,"idsGeneros":[2]}' > "$T/b.json"
chk "PUT jogo trocando gêneros" 200 0 "${J[@]}" -X PUT $B/Jogo/$JID --data-binary @"$T/b.json"
grep -q '"generos":\[{"id":2,"nome":"Aventura"}\]' "$T/r.json" && { OK=$((OK+1)); echo "OK     vínculos regravados na transação"; } || { FALHA=$((FALHA+1)); echo "FALHA  vínculos"; }
chk "PUT jogo 9999" 404 103 "${J[@]}" -X PUT $B/Jogo/9999 --data-binary @"$T/b.json"
chk "DELETE jogo (exclusão lógica)" 200 0 -X DELETE $B/Jogo/$JID
chk "jogo desativado continua existindo (admin)" 200 0 $B/Jogo/$JID
grep -q '"ativo":false' "$T/r.json" && { OK=$((OK+1)); echo "OK     DELETE só desativou (RN03)"; } || { FALHA=$((FALHA+1)); echo "FALHA  RN03"; }
chk "DELETE jogo 9999" 404 103 -X DELETE $B/Jogo/9999

echo "--- Aula 6 (logout)"
chk "POST logout" 200 0 -X POST $B/Autenticacao/logout
chk "GET eu depois do logout" 401 4 $B/Autenticacao/eu
chk "jogo desativado sem login" 404 103 $B/Jogo/$JID

echo "=== RESULTADO: $OK OK, $FALHA FALHA"
[[ $FALHA -eq 0 ]]
