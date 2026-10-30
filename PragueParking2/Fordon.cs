namespace PragueParking2;

// Basklassen för alla fordon. Allt som alla fordon har gemensamt ligger här.
// abstract betyder att man inte kan skapa ett "Fordon", bara en Bil eller en MC
// (samma som GeometriskForm i lektion 13).
public abstract class Fordon
{
    // Bara get, värdena sätts i konstruktorn och kan inte ändras efteråt.
    // Ett regnr ska inte kunna bytas medan fordonet står parkerat.
    public string Regnr { get; }
    public string Typ { get; }
    public int Storlek { get; }
    public DateTime Ankomst { get; }

    public Fordon(string regnr, string typ, int storlek, DateTime ankomst)
    {
        Regnr = regnr;
        Typ = typ;
        Storlek = storlek;
        Ankomst = ankomst;
    }

    // Används när fordonet skrivs ut, t.ex. "CAR ABC123"
    public string Beskrivning()
    {
        return $"{Typ} {Regnr}";
    }
}