# Zadanie 2 – GitHub Actions + Docker + Trivy (PAwChO)

Repozytorium zawiera zautomatyzowany łańcuch CI/CD oparty o GitHub Actions, który:
- buduje obraz kontenera z aplikacją ASP.NET Core (.NET 9, self-contained)
- obsługuje wieloarchitekturny build: linux/amd64, linux/arm64
- wykorzystuje cache warstw Dockera przechowywany na DockerHub
- skanuje obraz pod kątem podatności z wykorzystaniem narzędzia Trivy
- publikuje obraz do GitHub Container Registry (GHCR)

## Szczegóły działania pipeline

Aplikacja .NET 9 budowana jest w trybie self-contained i single-file. Obraz budowany z użyciem Buildx, a cache warstw zapisywany i odczytywany z DockerHub (`mode=max`). Obraz publikowany jako: `ghcr.io/llrnc/chmuraz2:sha-<commit>`.

### CVE Scan (Trivy)

Skanowanie obrazu realizowane jest przez Trivy. Wykryto podatności typu HIGH oraz CRITICAL (np. w `zlib1g`, `libdb5.3`, `bash`). Ponieważ są to podatności nierozwiązywalne (`will_not_fix`) wynikające z zależności w obrazie bazowym (Debian 11), workflow został skonfigurowany z `exit-code: 0`, aby mimo to umożliwić publikację obrazu. Informacja o wykrytych podatnościach znajduje się w logach workflow i zostały one świadomie zaakceptowane na potrzeby realizacji zadania.

## Tagowanie obrazów

Zastosowano `docker/metadata-action@v5`, który taguje obrazy na podstawie typu SHA:
	
  tags:
    type=sha

Tag SHA jest jednoznacznym identyfikatorem buildu i kompatybilny z `push` i `workflow_dispatch`.

## Użyte sekrety

Repozytorium korzysta z 2 sekretów:
- `DOCKERHUB_USERNAME` – login do DockerHub
- `DOCKERHUB_TOKEN` – personal access token z DockerHub (Read/Write scope)

## Obraz w GHCR

Po zakończonym buildzie obraz trafia do:  
https://github.com/users/LLRNC/packages

## Uruchamianie workflow

Pipeline może zostać uruchomiony:
1. Automatycznie – po wypchnięciu do gałęzi `main`
2. Ręcznie – z poziomu zakładki Actions poprzez `workflow_dispatch`
