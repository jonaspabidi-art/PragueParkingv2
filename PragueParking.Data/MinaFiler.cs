using System.Text.Json;

namespace PragueParking.Data;

// All läsning och sparning av filer ligger i det här klassbiblioteket.
// Klassen vet ingenting om parkering. Den kan läsa och spara vilken typ
// som helst, därför används <T> (generisk metod, som DataLager<T> i lektion 12).
public static class MinaFiler
{
    // Filerna ligger i samma mapp som programmet (bin/Debug/...),
    // dit Visual Studio kopierar dem med "Copy if newer".
    private static string HelSökväg(string filnamn)
    {
        return Path.Combine(AppContext.BaseDirectory, filnamn);
    }

    public static bool FilFinns(string filnamn)
    {
        return File.Exists(HelSökväg(filnamn));
    }

    // Läser en textfil rad för rad
    public static string[] LäsRader(string filnamn)
    {
        return File.ReadAllLines(HelSökväg(filnamn));
    }

    // Läser en JSON-fil och gör om den till ett objekt av typen T.
    // Kastar ett fel om filen saknas eller är trasig, det fångas i Program.
    public static T LäsJson<T>(string filnamn)
    {
        string json = File.ReadAllText(HelSökväg(filnamn));
        T? data = JsonSerializer.Deserialize<T>(json);

        if (data == null)
        {
            throw new InvalidDataException($"{filnamn} är tom.");
        }

        return data;
    }

    // Gör om ett objekt till JSON och sparar det
    public static void SparaJson<T>(string filnamn, T data)
    {
        // WriteIndented gör filen lättare att läsa för en människa
        var inställningar = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(data, inställningar);
        File.WriteAllText(HelSökväg(filnamn), json);
    }
}