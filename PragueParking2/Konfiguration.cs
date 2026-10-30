namespace PragueParking2;

// Motsvarar innehållet i config.json. Namnen på properties måste vara
// samma som i filen, annars vet JSON-läsaren inte var värdena ska hamna.
public class Konfiguration
{
    public int AntalPlatser { get; set; }
    public int PlatsStorlek { get; set; }

    // Fordonstyp -> storlek, t.ex. "CAR" -> 4 och "MC" -> 2
    public Dictionary<string, int> Fordonstyper { get; set; } = new Dictionary<string, int>();

    // Kontrollerar att filen innehåller rimliga värden
    public bool ÄrGiltig()
    {
        if (AntalPlatser < 1 || PlatsStorlek < 1)
        {
            return false;
        }

        if (!Fordonstyper.ContainsKey(Konstanter.Car) || !Fordonstyper.ContainsKey(Konstanter.MC))
        {
            return false;
        }

        foreach (int storlek in Fordonstyper.Values)
        {
            if (storlek < 1 || storlek > PlatsStorlek)
            {
                return false;
            }
        }

        return true;
    }
}