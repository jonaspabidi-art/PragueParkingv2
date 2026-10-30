namespace PragueParking2;

// En parkeringsruta. Den vet själv vilka fordon som står på den och hur
// mycket plats som är kvar. Ersätter strängarna "MC#AAA|MC#BBB" från v1.
public class ParkeringsPlats
{
    public int Nummer { get; }
    public int Storlek { get; }
    public List<Fordon> Fordon { get; }

    public ParkeringsPlats(int nummer, int storlek)
    {
        Nummer = nummer;
        Storlek = storlek;
        Fordon = new List<Fordon>();
    }

    // Beräknade egenskaper (som Rektangel i lektion 11).
    // De räknas ut varje gång man frågar, så de stämmer alltid.
    public int UpptagetUtrymme
    {
        get
        {
            var summa = 0;
            foreach (Fordon fordon in Fordon)
            {
                summa += fordon.Storlek;
            }
            return summa;
        }
    }

    public int LedigtUtrymme
    {
        get { return Storlek - UpptagetUtrymme; }
    }

    public bool ÄrTom
    {
        get { return Fordon.Count == 0; }
    }

    public bool ÄrFull
    {
        get { return LedigtUtrymme == 0; }
    }

    public bool FinnsPlatsFör(Fordon fordon)
    {
        return fordon.Storlek <= LedigtUtrymme;
    }

    public void Parkera(Fordon fordon)
    {
        Fordon.Add(fordon);
    }

    // Returnerar fordonet om det står här, annars null
    public Fordon? HittaFordon(string regnr)
    {
        foreach (Fordon fordon in Fordon)
        {
            // == istället för Contains, annars hittar "ABC" även "ABC123"
            if (fordon.Regnr == regnr)
            {
                return fordon;
            }
        }
        return null;
    }

    public void TaBort(Fordon fordon)
    {
        Fordon.Remove(fordon);
    }

    // T.ex. "CAR ABC123" eller "MC AAA, MC BBB"
    public string Innehåll()
    {
        if (ÄrTom)
        {
            return "(tom)";
        }

        var beskrivningar = new List<string>();
        foreach (Fordon fordon in Fordon)
        {
            beskrivningar.Add(fordon.Beskrivning());
        }
        return string.Join(", ", beskrivningar);
    }
}