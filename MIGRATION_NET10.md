# Migrazione a .NET 10 (Opzione B)

Piano operativo da eseguire in Visual Studio (con Claude). Nessuna riscrittura di logica:
si cambiano solo target framework, versioni pacchetti e la versione SDK nel workflow.

Prima di iniziare, verifica la versione esatta installata:

```bash
dotnet --version
dotnet --list-sdks
```

Annota la versione (es. `10.0.100`): serve a pinnare i pacchetti del runtime Blazor.

---

## 1. Target framework — 3 file `.csproj`

Cambia `net8.0` → `net10.0` in tutti e tre:

- `src/AuroraProfileSwapper.Core/AuroraProfileSwapper.Core.csproj`
- `src/AuroraProfileSwapper.Web/AuroraProfileSwapper.Web.csproj`
- `tests/AuroraProfileSwapper.Core.Tests/AuroraProfileSwapper.Core.Tests.csproj`

```xml
<TargetFramework>net10.0</TargetFramework>
```

Il **Core** non ha altre dipendenze: per lui basta questo.

---

## 2. Pacchetti Blazor — `AuroraProfileSwapper.Web.csproj`

Porta i due pacchetti alla **stessa minor del runtime .NET 10** (allinea alla tua `10.0.x`):

```xml
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="10.0.*" />
<PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="10.0.*" PrivateAssets="all" />
```

Nota: se è una **preview/RC**, il wildcard `10.0.*` potrebbe non pescare i preview.
In quel caso pinna la versione precisa (es. `10.0.0-rc.1.*`) uguale al runtime installato.
Il modo sicuro: in VS → tasto destro sul progetto → *Manage NuGet Packages* → scegli la
versione 10.x che compare, identica per entrambi i pacchetti.

---

## 3. Pacchetti di test — `AuroraProfileSwapper.Core.Tests.csproj`

Sono indipendenti dal target framework; vanno bene le versioni attuali, ma è il momento
buono per aggiornarle all'ultima stabile (in VS → NuGet → Update):

- `Microsoft.NET.Test.Sdk`
- `xunit`
- `xunit.runner.visualstudio`

Nessuna modifica al codice dei test.

---

## 4. Workflow CI — `.github/workflows/deploy.yml`

Aggiorna la versione dell'SDK:

```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: 10.0.x
```

Se usi una preview, aggiungi anche `dotnet-quality: preview` sotto `with:`.

Il resto del workflow (test → publish → fix base href → .nojekyll → 404 → deploy) resta
invariato.

---

## 5. README

Aggiorna il requisito da ".NET SDK 8.0" a ".NET SDK 10.0" nella sezione *Requisiti*.

---

## 6. Verifica (ordine consigliato)

```bash
dotnet restore
dotnet build -c Release          # deve compilare i 3 progetti
dotnet test                      # tutti i test verdi (round-trip byte-identico + swap)
dotnet run --project src/AuroraProfileSwapper.Web   # apri l'URL locale e prova uno swap reale
```

Checklist di verifica manuale nell'app:
1. Carica un sorgente e un destinazione.
2. Spunta solo `TRAFFICLISTS`, "Copia e scarica".
3. Apri il file scaricato in un editor e confronta: solo `[TRAFFICLISTS]` è cambiata.
4. Prova una sezione col badge `NEW` (assente nel dest) → deve finire in fondo al file.

---

## Note

- Nessun workload aggiuntivo: Blazor WASM standard si pubblica col solo SDK
  (`wasm-tools` serve solo per AOT, non usato qui).
- Se qualche pacchetto 10.x non esiste ancora nella tua build (preview molto recente),
  l'unico punto delicato sono i due pacchetti Blazor del §2: tienili sempre alla stessa
  versione del runtime installato.
