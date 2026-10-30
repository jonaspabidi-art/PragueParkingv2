using PragueParking.Data;
using Spectre.Console;

namespace PragueParking2;

// Program sköter bara menyn och pratar med användaren.
// Allt som har med parkeringen att göra ligger i klasserna.
internal class Program
{
    // Deklarerade på klassnivå, så alla metoder kommer åt dem
    // utan att de skickas med som parametrar (Claes kommentar på v1).
    static ParkeringsHus parkeringsHus = null!;
    static Prislista prislista = new Prislista();

    static void Main(string[] args)
    {
        if (!LäsInKonfiguration() || !LäsInPrislista())
        {
            Console.WriteLine("\n\nTryck på en tangent för att stänga fönstret");
            Console.ReadKey();
            return;
        }

        LäsInParkering();

        var fortsätt = true;
        while (fortsätt)
        {
            VisaMeny();
            var val = LäsText("Välj ett alternativ: ");
            Console.WriteLine();

            switch (val)
            {
                case "1":
                    ParkeraFordon();
                    break;
                case "2":
                    HämtaUtFordon();
                    break;
                case "3":
                    FlyttaFordon();
                    break;
                case "4":
                    SökFordon();
                    break;
                case "5":
                    VisaKarta();
                    break;
                case "6":
                    VisaPrislista();
                    break;
                case "7":
                    if (LäsInPrislista())
                    {
                        AnsiConsole.MarkupLine("[green]Prislistan är inläst på nytt.[/]");
                        VisaPrislista();
                    }
                    break;
                case "0":
                    fortsätt = false;
                    break;
                default:
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    break;
            }
        }

        Console.WriteLine("Programmet avslutat. Hejdå!");
        Console.WriteLine("\n\nTryck på en tangent för att stänga fönstret");
        Console.ReadKey();
    }

    static void VisaMeny()
    {
        Console.WriteLine();
        AnsiConsole.Write(new Rule("[yellow]Prague Parking 2.0[/]"));
        Console.WriteLine("1. Parkera fordon");
        Console.WriteLine("2. Hämta ut fordon");
        Console.WriteLine("3. Flytta fordon");
        Console.WriteLine("4. Sök fordon");
        Console.WriteLine("5. Visa karta över parkeringen");
        Console.WriteLine("6. Visa prislista");
        Console.WriteLine("7. Läs in prislistan på nytt");
        Console.WriteLine("0. Avsluta programmet");
    }

    // ---- Menyval ----

    static void ParkeraFordon()
    {
        var typ = LäsText($"Ange fordonstyp ({Konstanter.Car}/{Konstanter.MC}): ").ToUpper();
        if (typ != Konstanter.Car && typ != Konstanter.MC)
        {
            Console.WriteLine("Ogiltig fordonstyp. Försök igen.");
            return;
        }

        var regnr = LäsRegnr();
        if (!ÄrGiltigtRegnr(regnr))
        {
            return;
        }

        if (parkeringsHus.HittaPlats(regnr) != null)
        {
            Console.WriteLine("Fordonet är redan parkerat.");
            return;
        }

        // Här skapas objektet, en Bil eller en MC
        Fordon? fordon = parkeringsHus.SkapaFordon(typ, regnr, DateTime.Now);
        if (fordon == null)
        {
            Console.WriteLine($"Fordonstypen {typ} finns inte i {Konstanter.ConfigFil}.");
            return;
        }

        ParkeringsPlats? plats = parkeringsHus.Parkera(fordon);
        if (plats == null)
        {
            Console.WriteLine("Tyvärr, parkeringen är full.");
            return;
        }

        SparaParkering();
        AnsiConsole.MarkupLine($"[green]{Markup.Escape(fordon.Beskrivning())} ska parkeras på plats {plats.Nummer}.[/]");
    }

    static void HämtaUtFordon()
    {
        var regnr = LäsRegnr();
        ParkeringsPlats? plats = parkeringsHus.HittaPlats(regnr);
        Fordon? fordon = parkeringsHus.HämtaUt(regnr);

        if (plats == null || fordon == null)
        {
            Console.WriteLine($"Hittade inget fordon med registreringsnummer {regnr}.");
            return;
        }

        SparaParkering();

        // Kvittot visas när fordonet checkas ut
        DateTime utcheckning = DateTime.Now;
        TimeSpan tid = utcheckning - fordon.Ankomst;
        var pris = prislista.BeräknaPris(fordon, utcheckning);

        var kvitto = new Table();
        kvitto.Title = new TableTitle("Kvitto");
        kvitto.AddColumn("");
        kvitto.AddColumn("");
        kvitto.HideHeaders();
        kvitto.AddRow("Fordon", Markup.Escape(fordon.Beskrivning()));
        kvitto.AddRow("Plats", plats.Nummer.ToString());
        kvitto.AddRow("Ankomst", fordon.Ankomst.ToString("yyyy-MM-dd HH:mm"));
        kvitto.AddRow("Utcheckning", utcheckning.ToString("yyyy-MM-dd HH:mm"));
        kvitto.AddRow("Parkerad tid", $"{(int)tid.TotalHours} h {tid.Minutes} min");
        kvitto.AddRow("[bold]Att betala[/]", $"[bold]{pris} CZK[/]");
        AnsiConsole.Write(kvitto);
    }

    static void FlyttaFordon()
    {
        var regnr = LäsRegnr();
        ParkeringsPlats? frånPlats = parkeringsHus.HittaPlats(regnr);

        if (frånPlats == null)
        {
            Console.WriteLine($"Hittade inget fordon med registreringsnummer {regnr}.");
            return;
        }

        Console.WriteLine($"{regnr} står på plats {frånPlats.Nummer}.");
        var indata = LäsText($"Flytta till plats (1-{parkeringsHus.Platser.Count}): ");

        if (!int.TryParse(indata, out int tillNummer))
        {
            Console.WriteLine("Det där var inget tal.");
            return;
        }

        if (parkeringsHus.HämtaPlats(tillNummer) == null)
        {
            Console.WriteLine($"Platsnummer måste vara mellan 1 och {parkeringsHus.Platser.Count}.");
            return;
        }

        if (tillNummer == frånPlats.Nummer)
        {
            Console.WriteLine("Fordonet står redan på den platsen.");
            return;
        }

        if (!parkeringsHus.Flytta(regnr, tillNummer))
        {
            Console.WriteLine($"Plats {tillNummer} har inte plats för fordonet.");
            return;
        }

        SparaParkering();
        AnsiConsole.MarkupLine($"[green]Flytta {Markup.Escape(regnr)} från plats {frånPlats.Nummer} till plats {tillNummer}.[/]");
    }

    static void SökFordon()
    {
        var regnr = LäsRegnr();
        ParkeringsPlats? plats = parkeringsHus.HittaPlats(regnr);

        if (plats == null)
        {
            Console.WriteLine($"{regnr} finns inte på parkeringen.");
            return;
        }

        Console.WriteLine($"{regnr} står på plats {plats.Nummer}: {plats.Innehåll()}");
    }

    // Karta över alla platser. Grön = tom, gul = delvis upptagen, röd = full.
    static void VisaKarta()
    {
        const int PlatserPerRad = 10;

        var karta = new Grid();
        for (int i = 0; i < PlatserPerRad; i++)
        {
            karta.AddColumn();
        }

        var rad = new List<string>();
        foreach (ParkeringsPlats plats in parkeringsHus.Platser)
        {
            var färg = "green";
            if (plats.ÄrFull)
            {
                färg = "red";
            }
            else if (!plats.ÄrTom)
            {
                färg = "yellow";
            }

            rad.Add($"[{färg}]{plats.Nummer,3}[/]");

            if (rad.Count == PlatserPerRad)
            {
                karta.AddRow(rad.ToArray());
                rad.Clear();
            }
        }

        // Sista raden om antalet platser inte är jämnt delbart med 10
        if (rad.Count > 0)
        {
            while (rad.Count < PlatserPerRad)
            {
                rad.Add("");
            }
            karta.AddRow(rad.ToArray());
        }

        AnsiConsole.Write(new Panel(karta).Header("Karta"));
        AnsiConsole.MarkupLine("[green]Tom[/]   [yellow]Delvis upptagen[/]   [red]Full[/]");

        // Stor text så att personalen ser antalet lediga platser på avstånd
        AnsiConsole.Write(new FigletText($"{parkeringsHus.AntalTommaPlatser} lediga").Color(Color.Green));

        // Lista över vilka fordon som står var
        var tabell = new Table();
        tabell.AddColumn("Plats");
        tabell.AddColumn("Fordon");
        foreach (ParkeringsPlats plats in parkeringsHus.Platser)
        {
            if (!plats.ÄrTom)
            {
                tabell.AddRow(plats.Nummer.ToString(), Markup.Escape(plats.Innehåll()));
            }
        }
        AnsiConsole.Write(tabell);
    }

    static void VisaPrislista()
    {
        var tabell = new Table();
        tabell.Title = new TableTitle("Prislista");
        tabell.AddColumn("Fordonstyp");
        tabell.AddColumn("Pris per påbörjad timme");

        foreach (KeyValuePair<string, int> pris in prislista.PrisPerTimme)
        {
            tabell.AddRow(pris.Key, $"{pris.Value} CZK");
        }

        AnsiConsole.Write(tabell);
        Console.WriteLine($"De första {prislista.GratisMinuter} minuterna är gratis.");
    }

    // ---- Filer ----

    // Returnerar false om programmet inte kan starta
    static bool LäsInKonfiguration()
    {
        try
        {
            Konfiguration konfiguration = MinaFiler.LäsJson<Konfiguration>(Konstanter.ConfigFil);

            if (!konfiguration.ÄrGiltig())
            {
                Console.WriteLine($"{Konstanter.ConfigFil} innehåller felaktiga värden.");
                return false;
            }

            parkeringsHus = new ParkeringsHus(konfiguration);
            return true;
        }
        catch (Exception fel)
        {
            Console.WriteLine($"Kunde inte läsa {Konstanter.ConfigFil}: {fel.Message}");
            return false;
        }
    }

    static bool LäsInPrislista()
    {
        try
        {
            string[] rader = MinaFiler.LäsRader(Konstanter.PrislistaFil);
            var felmeddelande = prislista.LäsIn(rader);

            if (felmeddelande != "")
            {
                Console.WriteLine(felmeddelande);
                return false;
            }
            return true;
        }
        catch (Exception fel)
        {
            Console.WriteLine($"Kunde inte läsa {Konstanter.PrislistaFil}: {fel.Message}");
            return false;
        }
    }

    static void LäsInParkering()
    {
        // Första gången finns ingen fil, då börjar vi med en tom parkering
        if (!MinaFiler.FilFinns(Konstanter.ParkeringFil))
        {
            return;
        }

        try
        {
            List<SparatFordon> sparade = MinaFiler.LäsJson<List<SparatFordon>>(Konstanter.ParkeringFil);
            var antalFel = parkeringsHus.LäggTillSparadeFordon(sparade);

            if (antalFel > 0)
            {
                Console.WriteLine($"{antalFel} fordon i {Konstanter.ParkeringFil} kunde inte placeras.");
            }
        }
        catch (Exception fel)
        {
            Console.WriteLine($"Kunde inte läsa {Konstanter.ParkeringFil}: {fel.Message}");
            Console.WriteLine("Programmet startar med en tom parkering.");
        }
    }

    // Anropas efter varje ändring, så att inget försvinner om programmet stängs
    static void SparaParkering()
    {
        try
        {
            MinaFiler.SparaJson(Konstanter.ParkeringFil, parkeringsHus.TillSparadeFordon());
        }
        catch (Exception fel)
        {
            Console.WriteLine($"Kunde inte spara {Konstanter.ParkeringFil}: {fel.Message}");
        }
    }

    // ---- Inläsning ----

    static string LäsText(string ledtext)
    {
        Console.Write(ledtext);
        string indata = Console.ReadLine() ?? "";
        return indata.Trim();
    }

    // Versaler så att abc123 och ABC123 räknas som samma fordon
    static string LäsRegnr()
    {
        return LäsText("Registreringsnummer: ").ToUpper();
    }

    static bool ÄrGiltigtRegnr(string regnr)
    {
        if (regnr.Length == 0 || regnr.Length > Konstanter.MaxLängdRegnr)
        {
            Console.WriteLine($"Registreringsnumret ska vara 1-{Konstanter.MaxLängdRegnr} tecken.");
            return false;
        }

        if (regnr.Contains(' '))
        {
            Console.WriteLine("Registreringsnumret får inte innehålla mellanslag.");
            return false;
        }

        return true;
    }
}