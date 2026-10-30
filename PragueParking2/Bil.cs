namespace PragueParking2;

// En bil är ett Fordon. Den skickar sina värden vidare till
// basklassens konstruktor med : base(...) (som Katt : Djur i lektion 13).
public class Bil : Fordon
{
    public Bil(string regnr, int storlek, DateTime ankomst)
        : base(regnr, Konstanter.Car, storlek, ankomst)
    {

    }
}