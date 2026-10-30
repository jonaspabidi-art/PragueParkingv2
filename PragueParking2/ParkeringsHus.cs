namespace PragueParking2;

// Parkeringshuset håller alla rutor i en lista. All logik om VAR fordon
// ska stå ligger här. Klassen frågar aldrig användaren något, den tar emot
// parametrar och svarar (samma uppdelning som i v1).
public class ParkeringsHus
{
    public List<ParkeringsPlats> Platser { get; }
    public Konfiguration Konfiguration { get; }

    public ParkeringsHus(Konfiguration konfiguration)
    {
        Konfiguration = konfiguration;
        Platser = new List<ParkeringsPlats>();

        // Platserna numreras 1, 2, 3 ... så vi slipper "+ 1" överallt
        for (int i = 1; i <= konfiguration.AntalPlatser; i++)
        {
            Platser.Add(new ParkeringsPlats(i, konfiguration.PlatsStorlek));
        }
    }

    public int AntalTommaPlatser
    {
        get
        {
            var antal = 0;
            foreach (ParkeringsPlats plats in Platser)
            {
                if (plats.ÄrTom)
                {
                    antal++;
                }
            }
            return antal;
        }
    }

    // Skapar en Bil eller en MC med rätt storlek från config.json.
    // Resten av programmet behöver bara veta att det är ett Fordon.
    public Fordon? SkapaFordon(string typ, string regnr, DateTime ankomst)
    {
        if (!Konfiguration.Fordonstyper.ContainsKey(typ))
        {
            return null;
        }

        var storlek = Konfiguration.Fordonstyper[typ];

        if (typ == Konstanter.Car)
        {
            return new Bil(regnr, storlek, ankomst);
        }
        if (typ == Konstanter.MC)
        {
            return new MC(regnr, storlek, ankomst);
        }
        return null;
    }

    // Returnerar platsen där fordonet står, eller null
    public ParkeringsPlats? HittaPlats(string regnr)
    {
        foreach (ParkeringsPlats plats in Platser)
        {
            if (plats.HittaFordon(regnr) != null)
            {
                return plats;
            }
        }
        return null;
    }

    // Returnerar plats nummer (1-...) eller null om nummret inte finns
    public ParkeringsPlats? HämtaPlats(int nummer)
    {
        if (nummer < 1 || nummer > Platser.Count)
        {
            return null;
        }
        // Listan börjar på index 0, platsnummer på 1
        return Platser[nummer - 1];
    }

    // Returnerar en plats där fordonet får plats, eller null om det är fullt
    public ParkeringsPlats? HittaLedigPlats(Fordon fordon)
    {
        // Först en plats som redan är delvis upptagen men har utrymme kvar.
        // Då ställs en MC bredvid en ensam MC, som i v1.
        foreach (ParkeringsPlats plats in Platser)
        {
            if (!plats.ÄrTom && plats.FinnsPlatsFör(fordon))
            {
                return plats;
            }
        }

        // Annars första helt tomma platsen
        foreach (ParkeringsPlats plats in Platser)
        {
            if (plats.ÄrTom && plats.FinnsPlatsFör(fordon))
            {
                return plats;
            }
        }

        return null;
    }

    // Returnerar platsen fordonet parkerades på, eller null om det är fullt
    public ParkeringsPlats? Parkera(Fordon fordon)
    {
        ParkeringsPlats? plats = HittaLedigPlats(fordon);
        if (plats != null)
        {
            plats.Parkera(fordon);
        }
        return plats;
    }

    // Tar bort fordonet och returnerar det, eller null om det inte fanns
    public Fordon? HämtaUt(string regnr)
    {
        ParkeringsPlats? plats = HittaPlats(regnr);
        if (plats == null)
        {
            return null;
        }

        Fordon? fordon = plats.HittaFordon(regnr);
        if (fordon != null)
        {
            plats.TaBort(fordon);
        }
        return fordon;
    }

    // Returnerar true om flytten gick
    public bool Flytta(string regnr, int tillNummer)
    {
        ParkeringsPlats? frånPlats = HittaPlats(regnr);
        ParkeringsPlats? tillPlats = HämtaPlats(tillNummer);

        if (frånPlats == null || tillPlats == null || frånPlats == tillPlats)
        {
            return false;
        }

        Fordon? fordon = frånPlats.HittaFordon(regnr);

        // Objektet vet sin egen storlek, så vi behöver inte lyfta bort det
        // först och ställa tillbaka det som i v1.
        if (fordon == null || !tillPlats.FinnsPlatsFör(fordon))
        {
            return false;
        }

        frånPlats.TaBort(fordon);
        tillPlats.Parkera(fordon);
        return true;
    }

    // ---- Omvandling till och från filen parkering.json ----

    public List<SparatFordon> TillSparadeFordon()
    {
        var sparade = new List<SparatFordon>();

        foreach (ParkeringsPlats plats in Platser)
        {
            foreach (Fordon fordon in plats.Fordon)
            {
                var sparat = new SparatFordon();
                sparat.Typ = fordon.Typ;
                sparat.Regnr = fordon.Regnr;
                sparat.PlatsNummer = plats.Nummer;
                sparat.Ankomst = fordon.Ankomst;
                sparade.Add(sparat);
            }
        }

        return sparade;
    }

    // Ställer tillbaka sparade fordon på sina platser.
    // Returnerar hur många som inte kunde ställas tillbaka.
    public int LäggTillSparadeFordon(List<SparatFordon> sparade)
    {
        var antalFel = 0;

        foreach (SparatFordon sparat in sparade)
        {
            Fordon? fordon = SkapaFordon(sparat.Typ, sparat.Regnr, sparat.Ankomst);
            ParkeringsPlats? plats = HämtaPlats(sparat.PlatsNummer);

            // Hoppa över fordon som inte passar längre, t.ex. om config.json
            // ändrats till färre platser
            if (fordon == null || plats == null || !plats.FinnsPlatsFör(fordon) || HittaPlats(fordon.Regnr) != null)
            {
                antalFel++;
                continue;
            }

            plats.Parkera(fordon);
        }

        return antalFel;
    }
}