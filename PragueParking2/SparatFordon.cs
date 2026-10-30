namespace PragueParking2;

// Det som sparas i parkering.json för varje fordon.
// Vi sparar inte Bil- och MC-objekten direkt, eftersom JSON-läsaren inte vet
// om den ska skapa en Bil eller en MC. Därför sparas typen som text
// och ParkeringsHus.SkapaFordon gör om den till rätt klass igen.
public class SparatFordon
{
    public string Typ { get; set; } = "";
    public string Regnr { get; set; } = "";
    public int PlatsNummer { get; set; }
    public DateTime Ankomst { get; set; }
}