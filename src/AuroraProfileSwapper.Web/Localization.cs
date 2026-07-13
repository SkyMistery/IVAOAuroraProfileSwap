namespace AuroraProfileSwapper.Web;

/// <summary>UI languages supported by the app.</summary>
public enum AppLang { It, En }

/// <summary>
/// Tiny in-app localization table. Each entry holds the Italian and English text
/// for a key; some values contain HTML and are rendered as <c>MarkupString</c>.
/// String.Format placeholders (<c>{0}</c>) are filled in by the caller.
/// </summary>
public static class Loc
{
    private static readonly Dictionary<string, (string It, string En)> S = new(StringComparer.Ordinal)
    {
        ["intro"] = (
            "Copia intere sezioni (es. <strong>TRAFFICLISTS</strong>) da un profilo Aurora a <strong>uno o più</strong> profili di destinazione, senza copia-incolla manuale. Tutto avviene nel tuo browser: i file non vengono caricati da nessuna parte.",
            "Copy whole sections (e.g. <strong>TRAFFICLISTS</strong>) from one Aurora profile into <strong>one or more</strong> destination profiles, with no manual copy-paste. Everything happens in your browser: files are never uploaded anywhere."),

        ["sourceTitle"] = ("Profilo sorgente", "Source profile"),
        ["sourceHint"] = ("Il profilo <em>da cui</em> copiare.", "The profile to copy <em>from</em>."),
        ["destTitle"] = ("Profili destinazione", "Destination profiles"),
        ["destHint"] = (
            "I profili <em>in cui</em> incollare. Puoi caricarne <strong>più di uno</strong>.",
            "The profiles to paste <em>into</em>. You can load <strong>more than one</strong>."),

        ["secCount"] = ("{0} sezioni", "{0} sections"),
        ["change"] = ("Cambia", "Change"),
        ["remove"] = ("Rimuovi", "Remove"),
        ["removeTitle"] = ("Rimuovi questo profilo", "Remove this profile"),
        ["changeTitle"] = ("Carica un altro file", "Load another file"),

        ["dropSource"] = (
            "Trascina qui il file <strong>.cpr</strong><br />o <span class=\"btn-file\">scegli dal disco</span>",
            "Drop the <strong>.cpr</strong> file here<br />or <span class=\"btn-file\">pick from disk</span>"),
        ["dropDest"] = (
            "Trascina qui uno o più file <strong>.cpr</strong><br />o <span class=\"btn-file\">scegli dal disco</span>",
            "Drop one or more <strong>.cpr</strong> files here<br />or <span class=\"btn-file\">pick from disk</span>"),

        ["sectionsTitle"] = ("Sezioni da copiare", "Sections to copy"),
        ["selectAll"] = ("Seleziona tutto", "Select all"),
        ["clearAll"] = ("Deseleziona tutto", "Clear all"),
        ["selectedCount"] = ("{0} selezionate", "{0} selected"),
        ["newBadgeTitle"] = (
            "Assente in tutti i destinazione: verrà aggiunta in fondo",
            "Missing in every destination: it will be appended at the end"),

        ["previewTitle"] = ("Anteprima delle modifiche", "Preview of the changes"),
        ["previewHint"] = (
            "Testo che verrà copiato dal sorgente in <strong>tutti</strong> i profili destinazione ({0}).",
            "Text that will be copied from the source into <strong>all</strong> destination profiles ({0})."),
        ["previewFrom"] = ("Da copiare — {0}", "To copy — {0}"),
        ["previewMissing"] = (
            "Assente in: {0} — verrà aggiunta in fondo.",
            "Missing in: {0} — it will be appended at the end."),

        ["swapBtn"] = ("Copia e scarica (.zip, {0} profili)", "Copy and download (.zip, {0} profiles)"),
        ["outputInfo"] = (
            "Output: un unico .zip con {0} profili aggiornati.",
            "Output: a single .zip with {0} updated profiles."),

        ["footer"] = (
            "Le sezioni non selezionate restano identiche byte-per-byte. Colori e font seguono il",
            "Unselected sections stay byte-for-byte identical. Colours and fonts follow the"),
        ["footerBrand"] = ("brand IVAO", "IVAO brand"),

        // Status / errors
        ["errTooBig"] = (
            "Il file «{0}» supera il limite di {1} MB.",
            "File “{0}” exceeds the {1} MB limit."),
        ["errRead"] = (
            "Impossibile leggere «{0}»: {1}",
            "Could not read “{0}”: {1}"),
        ["errSectionMissing"] = (
            "La sezione [{0}] non è presente nel profilo sorgente.",
            "Section [{0}] is not present in the source profile."),
        ["errSwap"] = (
            "Errore durante la copia: {0}",
            "Error while copying: {0}"),
        ["detailLine"] = (
            "«{0}» — {1} sezioni ({2} sostituite, {3} aggiunte in fondo).",
            "“{0}” — {1} sections ({2} replaced, {3} appended)."),
        ["done"] = (
            "Fatto: {0} sezioni copiate in {1} profili, scaricati in «{2}».",
            "Done: {0} sections copied into {1} profiles, downloaded as “{2}”."),

        // Header controls
        ["themeToTitle"] = ("Passa al tema scuro", "Switch to dark theme"),
        ["themeToTitleLight"] = ("Passa al tema chiaro", "Switch to light theme"),

        // Section search + metadata
        ["searchPlaceholder"] = ("Filtra sezioni…", "Filter sections…"),
        ["secMeta"] = ("{0} righe", "{0} lines"),
        ["noMatch"] = ("Nessuna sezione corrisponde al filtro.", "No section matches the filter."),

        // Identical flag
        ["badgeSame"] = ("IDENTICA", "SAME"),
        ["badgeSameTitle"] = (
            "Già identica in tutti i destinazione: la copia non cambia nulla",
            "Already identical in every destination: copying changes nothing"),
        ["identicalNoChange"] = ("identica — nessuna modifica", "identical — no change"),

        // Destination list controls
        ["clearAllDests"] = ("Rimuovi tutti", "Remove all"),

        // Input warnings (non-blocking)
        ["warningsTitle"] = ("Attenzione", "Warning"),
        ["warnSameName"] = (
            "«{0}» ha lo stesso nome del sorgente: rischi di sovrascrivere il file di partenza.",
            "“{0}” has the same name as the source: you risk overwriting the starting file."),
        ["warnSameContent"] = (
            "«{0}» è identico al sorgente: copiare non produrrà alcuna modifica.",
            "“{0}” is identical to the source: copying will produce no change."),
        ["warnDuplicate"] = (
            "«{0}» è caricato più di una volta come destinazione.",
            "“{0}” is loaded more than once as a destination."),

        // Busy / progress
        ["busy"] = ("Creazione zip…", "Building zip…"),
    };

    /// <summary>Return the raw string for a key in the given language.</summary>
    public static string T(AppLang lang, string key)
    {
        if (!S.TryGetValue(key, out var pair)) return key;
        return lang == AppLang.It ? pair.It : pair.En;
    }

    /// <summary>Return the string for a key, formatted with the given arguments.</summary>
    public static string T(AppLang lang, string key, params object[] args)
        => string.Format(T(lang, key), args);
}
