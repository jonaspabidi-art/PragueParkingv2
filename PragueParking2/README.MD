# Prague Parking 2.0

Inlämningsuppgift 2 i kursen Objektorienterad programmering C# (DSO26, KYH).

Version 2 av konsolprogrammet för valet parking vid slottet i Prag. Fordonen
är nu objekt i stället för strängar, inställningar och priser läses från filer
och parkeringen sparas mellan körningarna.

## Köra programmet

1. Öppna `PragueParking2.slnx` i Visual Studio.
2. Kontrollera att **PragueParking2** är startprojekt (fetstilt).
3. Tryck F5. Spectre.Console hämtas automatiskt via NuGet.

Kräver .NET 10.

## Funktioner

- Parkera bil eller MC. Två MC delar på en ruta.
- Hämta ut fordon och få ett kvitto med parkerad tid och pris.
- Flytta fordon till en annan ruta.
- Söka efter fordon med registreringsnummer.
- Karta över parkeringen i färg: grön = tom, gul = delvis upptagen, röd = full.
- Visa prislistan och läsa in den på nytt utan att starta om programmet.

## Filer

| Fil | Innehåll |
|---|---|
| `config.json` | Antal platser, storlek på en ruta och storlek på varje fordonstyp |
| `prislista.txt` | Pris per påbörjad timme och antal gratisminuter. Rader med `#` är kommentarer |
| `parkering.json` | De fordon som står parkerade. Sparas efter varje ändring |

Storleken räknas i enheter. En ruta är 4, en bil 4 och en MC 2, så två MC får
plats på en ruta.

## Pris

Pris tas per påbörjad timme. De första 10 minuterna är gratis.
Priserna ändras i `prislista.txt` och läses in med val 7 i menyn.

## Projektstruktur

- **PragueParking2** – konsolprogrammet med klasserna `Fordon`, `Bil`, `MC`,
  `ParkeringsPlats`, `ParkeringsHus`, `Prislista`, `Konfiguration`,
  `SparatFordon`, `Konstanter` och `Program`.
- **PragueParking.Data** – klassbibliotek med `MinaFiler`, som läser och
  sparar alla filer.