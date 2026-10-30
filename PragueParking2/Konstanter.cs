namespace PragueParking2;

// Fasta värden samlade på ett ställe (samma idé som Constants.cs i Claes DRY-demo).
// Antal platser, storlekar och priser ligger INTE här, de läses från filerna
// så att de kan ändras utan att man ändrar i koden.
public static class Konstanter
{
    public const string ConfigFil = "config.json";
    public const string PrislistaFil = "prislista.txt";
    public const string ParkeringFil = "parkering.json";

    public const string Car = "CAR";
    public const string MC = "MC";

    public const int MaxLängdRegnr = 10;
}