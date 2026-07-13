# Aurora Profile Swapper — Piano & Regole

Stato: **bozza** · Data: 2026-07-13
Aperti: host .NET da decidere · UI: design system IVAO acquisito (§7)

---

## 1. Obiettivo

Strumento web che permette di:

1. Caricare un profilo **sorgente** (da cui copiare) e uno **destinazione** (in cui copiare).
2. Selezionare una o più **sezioni** intere (es. `[TRAFFICLISTS]`).
3. Copiare quelle sezioni dal sorgente al destinazione, senza toccare il resto e senza errori manuali.
4. Scaricare il profilo risultante.

Granularità v1: **solo sezioni intere**. Progettare per estendere a singole chiavi in futuro, senza riscrivere il Core.

---

## 2. Formato dei file `.cpr`

- File di testo in stile **INI**, estensione `.cpr` (non binario).
- **Sezioni**: righe `[NOME]`. **Voci**: righe `Chiave=Valore` sotto la sezione.
- Fine riga **CRLF** (`\r\n`). Codifica **ANSI/Windows-1252** (ASCII esteso).
- 34 sezioni presenti in tutti i profili analizzati; `[GC]` presente solo in alcuni.
- Ordine delle sezioni sostanzialmente stabile ma **non garantito identico** tra file.

Esempio sezione:

```
[TRAFFICLISTS]
LST0XPos=640
LST0Structure=DEPARTURE
...
```

---

## 3. Architettura

**Regola d'oro: il Core non conosce la UI.**

```
AuroraProfileSwapper.Core        ← libreria C# pura (parsing + swap). NIENTE web.
AuroraProfileSwapper.Core.Tests  ← unit test (xUnit), fixture = i profili reali di ProfilesTest
AuroraProfileSwapper.Ui          ← guscio sottile: Blazor / ViewComponent / API+JS (deciso dopo)
```

Vantaggio: la scelta dell'host (Blazor Server/WASM, MVC/Razor Pages, o Web API + JS)
si fa **dopo**, senza toccare la logica. Il Core è consegnabile e testabile da subito.

### Modello dati del Core

- Un file = **lista ordinata di blocchi**. Un blocco = riga header `[NOME]` + corpo (righe fino
  al prossimo header). Eventuale preambolo prima del primo header = blocco senza header.
- Ogni blocco conserva il testo **verbatim** (righe originali, byte per byte).

### Contratto dell'operazione di swap (v1)

Per ogni sezione selezionata `S`:

- Prendi il blocco `S` dal **sorgente** verbatim.
- Nel **destinazione**: se `S` esiste → **sostituisci sul posto** (mantieni la posizione originale).
  Se `S` non esiste → **appendi in fondo** al file.
- Non modificare nessun'altra sezione. Preserva CRLF, righe vuote, ordine, encoding.
- Se `S` è selezionata ma **assente nel sorgente** → errore esplicito (niente copia silenziosa).

---

## 4. Regole anti-vibecoding

1. **Contratto prima del codice.** Comportamenti sopra (sezione esiste/non esiste/assente nel
   sorgente, posizione, ordine) definiti e concordati prima di implementare. Niente ambiguità
   risolte "a sensazione" durante la scrittura.

2. **Fedeltà byte.** Il Core preserva CRLF, encoding ANSI/Windows-1252, righe vuote e ordine.
   Vietato "riformattare" o normalizzare ciò che non è stato esplicitamente selezionato.

3. **Test di round-trip.** Caricare un profilo e riscriverlo **senza modifiche** deve produrre
   un file **byte-identico** all'originale. È il test che smaschera ogni perdita di fedeltà.

4. **Fixture reali.** Gli unit test usano i profili veri in `ProfilesTest/` come casi. Almeno un
   test per: swap `[TRAFFICLISTS]`, sezione assente nel dest (append), sezione assente nel
   sorgente (errore), file con `[GC]` e senza.

5. **Niente librerie INI generiche.** I parser INI comuni perdono fedeltà (riordinano, gestiscono
   male duplicati/righe vuote/preambolo). Parser dedicato e minimale, scritto e testato da noi.

6. **Zero dipendenze non necessarie.** Il Core resta su .NET base. Ogni dipendenza va giustificata.

7. **Errori espliciti.** Sezione mancante, file malformato, encoding non riconosciuto → messaggi
   chiari, mai fallimento silenzioso o output corrotto.

8. **Passi piccoli e verificabili.** Prima il parser + round-trip test (verde), poi lo swap, poi la
   UI. Ogni pezzo verificato prima del successivo. Un commit per funzionalità.

9. **Validazione dell'output.** Oltre agli unit test: diff visivo tra input e output per confermare
   che cambia **solo** la sezione scelta, e verifica che il file resti apribile in Aurora.

---

## 5. Fasi

- **F0 — Core: parser + round-trip test.** Modello a blocchi, load/save byte-identico. Verde sui 26 profili.
- **F1 — Core: swap sezioni.** Operazione di swap + test su fixture reali (tutti i casi del §4.4).
- **F2 — UI (host da decidere).** Guscio sottile sul Core secondo le indicazioni UI: upload sorgente
  + destinazione, lista sezioni con selezione, copia, download.
- **F3 — Estensioni.** Granularità a singole chiavi; eventuale multi-profilo/batch.

---

## 6. Decisioni aperte

- **Host .NET**: Blazor (component in Razor Class Library) vs ASP.NET MVC/Razor Pages (ViewComponent)
  vs Web API + frontend JS. Da scegliere prima di F2. Il Core (F0–F1) non ne dipende.

---

## 7. Design system UI (brand IVAO)

Riferimenti: <https://brand.ivao.aero/font/> · <https://brand.ivao.aero/colors/>

### Font

- **Nunito Sans** → titoli/header. Taglie web Bootstrap 5 (1rem = 16px):
  h1 40px · h2 32px · h3 28px · h4 24px · h5 20px · h6 16px.
- **Poppins** → corpo e testi lunghi, 16px (1rem).
- Allineamento **a sinistra**; mai centrato, salvo header brevi su banner/bottoni (e in quel caso
  se il body è a sinistra, anche l'header segue).

### Colori

| Ruolo | Nome | HEX |
|---|---|---|
| Primario | Blue | `#0D2C99` |
| Secondario | Grey | `#D7D7DC` |
| Secondario | Light Blue | `#3C55AC` |
| Semantico – ok | Green | `#2EC662` |
| Semantico – warning | Yellow | `#F9CC2C` |
| Semantico – errore | Red | `#E93434` |
| Semantico – info | Info Blue | `#7EA2D6` |

- Titoli in **blu primario**; sotto-titoli in **light blue**. Su sfondo scuro: titoli bianchi,
  sotto-titoli grigio secondario.
- Colori semantici **solo** per stati/interazioni (successo copia, errore sezione mancante,
  avvisi), **non** come palette decorativa.
- Palette "universal" (Bronze/Silver/Gold/Platinum) = award IVAO, **non** usare nella UI.

### Applicazione allo swapper

- Header ("Aurora Profile Swapper") in Nunito Sans, blu primario, a sinistra.
- Corpo, etichette sezioni e liste in Poppins.
- Bottone "Copia" in blu primario; esito ok in Green, errore (sezione assente nel sorgente) in Red,
  avvisi (sezione verrà appesa perché assente nel dest) in Yellow/Info Blue.
- Definire i colori come **CSS custom properties** (`--ivao-blue`, `--ivao-light-blue`, …) in un
  unico foglio, per riuso e coerenza col sito ospitante.
