namespace PragueParking2;

// Läser och håller priserna från prislista.txt.
// Filen ska vara lätt att ändra för personalen, därför är den en vanlig
// textfil med rader som "CAR=20" och kommentarer som börjar med #.
public class Prislista
{
    public Dictionary<string, int> PrisPerTimme { get; } = new Dictionary<string, int>();
    public int GratisMinuter { get; private set; }

    // Tar emot raderna från filen och returnerar ett felmeddelande, eller "" om allt gick bra
    public string LäsIn(string[] rader)
    {
        PrisPerTimme.Clear();
        GratisMinuter = 0;

        foreach (string rad in rader)
        {
            var text = rad.Trim();

            // Hoppa över tomma rader och kommentarer
            if (text == "" || text.StartsWith("#"))
            {
                continue;
            }

            // Ta bort kommentar i slutet av raden, t.ex. "CAR=20 # per timme"
            if (text.Contains('#'))
            {
                text = text.Substring(0, text.IndexOf('#')).Trim();
            }

            string[] delar = text.Split('=');
            if (delar.Length != 2 || !int.TryParse(delar[1].Trim(), out int värde) || värde < 0)
            {
                return $"Felaktig rad i prislistan: \"{rad}\"";
            }

            var namn = delar[0].Trim().ToUpper();
            if (namn == "GRATIS")
            {
                GratisMinuter = värde;
            }
            else
            {
                PrisPerTimme[namn] = värde;
            }
        }

        if (!PrisPerTimme.ContainsKey(Konstanter.Car) || !PrisPerTimme.ContainsKey(Konstanter.MC))
        {
            return $"Prislistan måste innehålla pris för {Konstanter.Car} och {Konstanter.MC}.";
        }

        return "";
    }

    // Pris per påbörjad timme. De första GratisMinuter minuterna är gratis.
    public int BeräknaPris(Fordon fordon, DateTime utcheckning)
    {
        TimeSpan tid = utcheckning - fordon.Ankomst;

        if (tid.TotalMinutes <= GratisMinuter)
        {
            return 0;
        }

        // Math.Ceiling avrundar uppåt: 61 minuter blir 2 påbörjade timmar
        var påbörjadeTimmar = (int)Math.Ceiling(tid.TotalMinutes / 60);
        return påbörjadeTimmar * PrisPerTimme[fordon.Typ];
    }
}