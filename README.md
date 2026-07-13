# Aurora Profile Swapper

Strumento web per copiare **intere sezioni** (es. `[TRAFFICLISTS]`) da un profilo Aurora
IVAO (`.cpr`) a un altro, senza copia-incolla manuale e senza errori.

Tutta l'elaborazione avviene **nel browser**: i profili non vengono mai caricati su un server.

## Come provarlo (per i colleghi)

Una volta pubblicato su GitHub Pages, basta aprire il link del progetto — nessuna
installazione. Si scelgono il profilo **sorgente** e quello **destinazione**, si spuntano le
sezioni da copiare e si clicca **Copia e scarica**.

## Struttura del progetto

```
AuroraProfileSwapper.sln
src/
  AuroraProfileSwapper.Core/   Libreria C# pura: parsing + swap. Nessuna dipendenza dalla UI.
  AuroraProfileSwapper.Web/    App Blazor WebAssembly (UI standalone, brand IVAO).
tests/
  AuroraProfileSwapper.Core.Tests/   Unit test (xUnit) sui profili reali.
ProfilesTest/                  Profili .cpr reali usati come fixture dei test.
.github/workflows/deploy.yml   Build + test + publish su GitHub Pages.
```

Il **Core** è indipendente dalla UI: la stessa libreria potrà essere riusata in futuro
dentro un altro sito .NET (Blazor, MVC/Razor Pages, o dietro una Web API).

## Requisiti

[.NET SDK 10.0](https://dotnet.microsoft.com/download) (o superiore).

## Sviluppo in locale

```bash
# Eseguire i test
dotnet test

# Avviare l'app in locale (poi apri l'URL mostrato, di solito https://localhost:5xxx)
dotnet run --project src/AuroraProfileSwapper.Web
```

## Pubblicazione su GitHub Pages

1. Crea un repository su GitHub e fai push del codice sul branch `main`.
2. In **Settings → Pages**, imposta **Source = GitHub Actions**.
3. Il workflow `.github/workflows/deploy.yml` compila, esegue i test, pubblica la WASM e
   sistema automaticamente il `<base href>` per l'URL di progetto
   (`https://<utente>.github.io/<repo>/`).

Il workflow gestisce anche `.nojekyll` (necessario per la cartella `_framework` di Blazor)
e un fallback `404.html` per i link diretti.

## Come funziona (garanzie)

- Un `.cpr` è un file di testo in stile INI (`[SEZIONE]` + righe `Chiave=Valore`), con fine
  riga **CRLF** e codifica ANSI/Windows-1252.
- Il parser tratta il file come **lista ordinata di blocchi verbatim**: caricare e
  riscrivere un profilo non modificato produce un file **byte-identico** all'originale
  (verificato da un test su tutti i profili in `ProfilesTest/`).
- Lo swap sostituisce **solo** le sezioni scelte; se una sezione manca nel destinazione
  viene aggiunta in fondo; se manca nel sorgente l'operazione fallisce con errore esplicito.
- Tutto ciò che non è selezionato resta invariato byte-per-byte.

## Roadmap

- Granularità a livello di **singole chiavi** (oltre alle sezioni intere).
- Integrazione del Core dentro il sito principale (host .NET da decidere).
