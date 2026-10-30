namespace PragueParking2;

// En MC är också ett Fordon. Storleken kommer från config.json,
// så två MC får plats på en ruta om de tillsammans inte är större än rutan.
public class MC : Fordon
{
    public MC(string regnr, int storlek, DateTime ankomst)
        : base(regnr, Konstanter.MC, storlek, ankomst)
    {

    }
}