# Loggbok – Prague Parking 2.0

**Kurs:** DSO26 – Objektorienterad programmering C#
**Period:** 2026-10-20 – 2026-10-30
**Inlämnad:** 2026-10-30

---

## 2026-10-20

**Gjorde:**
Läste uppgiften och jämförde med v1. Ritade klassdiagram över
`Fordon`, `Bil`, `MC`, `ParkeringsPlats` och `ParkeringsHus`. Skapade
projektet och skrev `Konstanter.cs` med filnamn (`config.json`,
`prislista.txt`, `parkering.json`), `CAR`/`MC` och `MaxLängdRegnr`.

**Problem:**
Oklart om storleken skulle vara en abstrakt property i `Fordon` eller
skickas in i konstruktorn. I v1 var storleken hårdkodad (bil 4, MC 2),
men nu ska den läsas från `config.json`.

**Lösning:**
Valde att skicka in storleken i konstruktorn och spara den som en
vanlig property. Då kan `ParkeringsHus.SkapaFordon` läsa värdet från
`Konfiguration.Fordonstyper` och skicka in det.

**Lärde mig:**
Att skilja på vad som ska vara hårdkodat (filnamn, typnycklar) och
vad som ska komma från fil (antal platser, storlekar, priser).

**Nästa steg:**
Skriva `Fordon`, `Bil` och `MC`.

---

## 2026-10-21

**Gjorde:**
Skrev `Fordon` som abstrakt basklass med `Regnr`, `Typ`, `Storlek`,
`Ankomst` och `Beskrivning()`. Skrev `Bil : Fordon` och `MC : Fordon`
med `: base(regnr, Konstanter.Car, storlek, ankomst)` respektive
`Konstanter.MC`. Började på `ParkeringsPlats` med `List<Fordon>`.

**Problem:**
Först råkade jag skriva `Storlek` som ett fält i `Fordon` och ville
sedan overrida det i `Bil`/`MC` som i v1-tänket. Det gick inte eftersom
storleken nu kommer från config.

**Lösning:**
Lät `Storlek` vara en vanlig get-only property i basklassen som sätts
i konstruktorn. `Bil` och `MC` skickar bara vidare storleken – ingen
overriding behövs.

**Lärde mig:**
- `abstract` på klassen betyder att man inte kan skapa ett `Fordon`,
bara en `Bil` eller `MC` – samma som `GeometriskForm` i lektion 13.
- `: base(...)` skickar värdena vidare till basklassens konstruktor.

**Nästa steg:**
Göra klart `ParkeringsPlats` med `LedigtUtrymme` och `FinnsPlatsFör`.

---

## 2026-10-22

**Gjorde:**
Skrev klart `ParkeringsPlats`: `UpptagetUtrymme`, `LedigtUtrymme`,
`ÄrTom`, `ÄrFull`, `FinnsPlatsFör`, `Parkera`, `HittaFordon`,
`TaBort` och `Innehåll`. Testade två MC i samma ruta.

**Problem:**
`HittaFordon` använde först `Contains` på en sträng, vilket gjorde att
`"ABC"` matchade `"ABC123"`. Samma sorts bugg som jag undvek i v1
genom att jämföra hela fordonsträngen.

**Lösning:**
Bytte till `==` i loopen och kommenterade i koden varför.

**Lärde mig:**
Beräknade egenskaper (`get`-block som räknar ut varje gång) håller
alltid rätt värde utan att man behöver uppdatera dem manuellt – som
`Rektangel` i lektion 11.

**Nästa steg:**
Skriva `ParkeringsHus` med `HittaLedigPlats`, `Parkera`, `HämtaUt`
och `Flytta`.

---

## 2026-10-23

**Gjorde:**
Skrev `ParkeringsHus`: konstruktor som bygger platser 1..N,
`AntalTommaPlatser`, `SkapaFordon`, `HittaPlats`, `HämtaPlats`,
`HittaLedigPlats`, `Parkera`, `HämtaUt` och `Flytta`.

**Problem:**
I v1 var man tvungen att lyfta bort ett fordon för att se vilken typ
det var, och ställa tillbaka det om flytten misslyckades. Jag ville
undvika samma krångel i v2.

**Lösning:**
Eftersom `Fordon`-objektet vet sin egen `Storlek` kontrollerar
`Flytta` först `tillPlats.FinnsPlatsFör(fordon)` och tar bara bort
från källplatsen om det finns plats. Inget behöver lyftas ur
provisoriskt.

Lät också `HittaLedigPlats` prioritera en **delvis upptagen** plats
med plats kvar före en tom plats – då hamnar en MC bredvid en ensam
MC, precis som i v1.

**Lärde mig:**
Att polymorfism gör att `ParkeringsHus` aldrig behöver veta om det är
en `Bil` eller `MC` – den frågar bara objektet om dess `Storlek`.

**Nästa steg:**
Skriva `Konfiguration`, `SparatFordon` och `Prislista`.

---

## 2026-10-24

**Gjorde:**
Skrev `Konfiguration` med `AntalPlatser`, `PlatsStorlek`,
`Fordonstyper` och `ÄrGiltig()`. Skrev `SparatFordon` med `Typ`,
`Regnr`, `PlatsNummer` och `Ankomst`. Skrev `Prislista` med `LäsIn`
och `BeräknaPris`.

**Problem 1:**
JSON-läsaren vet inte om den ska skapa en `Bil` eller en `MC`, så jag
kan inte spara `Fordon`-objekten direkt.

**Lösning:**
Sparar `SparatFordon` där `Typ` ligger som text. `ParkeringsHus.SkapaFordon`
gör om typen till rätt klass när filen läses in.

**Problem 2:**
Prislistan kunde innehålla rader som `CAR=20 # per timme`, och då
gick `int.TryParse` bet på `"20 # per timme"`.

**Lösning:**
Klippte bort allt efter `#` innan `Split('=')`, och hoppade över helt
tomma rader och rena kommentarsrader. Validerar varje rad och
returnerar ett felmeddelande i stället för att krascha.

**Lärde mig:**
Att spara "typ som text" är ett enkelt sätt att kringgå att JSON inte
kan hantera abstrakta basklasser.

**Nästa steg:**
Bygga klassbiblioteket `PragueParking.Data` med `MinaFiler`.

---

## 2026-10-25

**Gjorde:**
Skapade klassbiblioteket **PragueParking.Data** och skrev `MinaFiler`
som en statisk generisk klass med `FilFinns`, `LäsRader`, `LäsJson<T>`
och `SparaJson<T>`. Lade till projektreferens från `PragueParking2`
till `PragueParking.Data`. Skrev `TillSparadeFordon` och
`LäggTillSparadeFordon` i `ParkeringsHus`.

**Problem:**
`PragueParking2` hittade inte `MinaFiler` – klassen fanns i ett annat
projekt.

**Lösning:**
Solution Explorer → `PragueParking2` → Add → Project Reference →
kryssade i `PragueParking.Data`.

**Lärde mig:**
- En generisk metod `<T>` kan läsa och spara vilken typ som helst
utan att veta något om parkering (som `DataLager<T>` i lektion 12).
- Klassbiblioteket vet inget om `Fordon` – det bara serialiserar.

**Nästa steg:**
Få filvägarna att fungera.

---

## 2026-10-26

**Gjorde:**
Testade inläsning av `config.json` – programmet kraschade direkt med
`FileNotFoundException`.

**Problem:**
Programmet körs från `bin/Debug`, inte från projektmappen, så filerna
låg inte där programmet letade.

**Lösning:**
- Satte `Copy to Output Directory = Copy if newer` på `config.json`,
`prislista.txt` och `parkering.json` i Visual Studio.
- Bytte i `MinaFiler` till `Path.Combine(AppContext.BaseDirectory, filnamn)`
i stället för relativ sökväg.

**Lärde mig:**
`AppContext.BaseDirectory` pekar alltid på mappen där programmet körs,
oavsett om man startar från Visual Studio eller dubbelklickar på exe-filen.

**Nästa steg:**
Skriva `Program.cs` med meny och Spectre.Console.

---

## 2026-10-27

**Gjorde:**
Skrev `Program.cs`: meny med `Rule`, `ParkeraFordon`, `HämtaUtFordon`
(kvitto som `Table`), `FlyttaFordon`, `SökFordon`, `VisaKarta`
(`Grid` + `Panel` + `FigletText` + tabell) och `VisaPrislista`. Lade
in `try/catch` i `LäsInKonfiguration`, `LäsInPrislista`,
`LäsInParkering` och `SparaParkering`.

**Problem:**
Spectre.Console kraschade när ett regnr innehöll `[` – det tolkades
som markup.

**Lösning:**
Lade in `Markup.Escape()` runt all användardata som skrivs ut i
markup-strängar (regnr, `Beskrivning()`, `Innehåll()`).

**Lärde mig:**
Att `Markup.Escape` behövs så fort man stoppar in data från
användaren i en Spectre-sträng, annars blir det konstiga fel.

**Nästa steg:**
Få `BeräknaPris` att räkna rätt på påbörjad timme.

---

## 2026-10-28

**Gjorde:**
Implementerade `Prislista.BeräknaPris` med `Math.Ceiling` och
`GratisMinuter` från `prislista.txt`. Testade 5 min (0 kr),
10 min (0 kr), 61 min (2 h × pris). Lade till menyval 7 för att läsa
in prislistan på nytt utan omstart.

**Problem:**
61 minuter gav 1 timme i stället för 2, eftersom `TotalHours` blir
1.016 och trunkeras vid cast till `int`.

**Lösning:**
Räknade på minuter i stället:
`(int)Math.Ceiling(tid.TotalMinutes / 60)`.

**Lärde mig:**
`Math.Ceiling` avrundar alltid uppåt, vilket är precis vad "påbörjad
timme" betyder. `(int)` på en double trunkar, det avrundar inte.

**Nästa steg:**
Testa hela flödet manuellt och rätta småbuggar.

---

## 2026-10-29

**Testade:**
- Parkera bil och två MC. MC:na hamnade på samma plats.
- Flytta en MC från en plats med två MC. Den andra stod kvar, inget
tappades.
- Flytta en bil till en plats med en MC. Nekades, bilen stod kvar.
- Hämta ut fordon med kvitto. Parkerad tid och pris stämde.
- Stänga programmet och starta igen. `parkering.json` lästes tillbaka
och fordonen stod på samma platser.
- Starta utan `config.json`. Programmet skrev ut fel och avslutade
snyggt i stället för att krascha.

**Problem:**
Sista raden i kartan blev fel när antalet platser inte var jämnt
delbart med 10.

**Lösning:**
Fyllde på raden med tomma celler tills den var full innan
`karta.AddRow`.

**Status:**
Alla tester fungerade som de skulle.

---

## 2026-10-30

**Gjorde:**
Skrev `README.md` med körinstruktioner, filbeskrivning och
projektstruktur. Slutgenomgång av alla filer. Uppdaterade loggboken
och lämnade in.

**Lärde mig (sammanfattning av hela uppgiften):**
- Att gå från strängar till objekt gjorde koden mycket enklare –
`ParkeringsHus` behöver aldrig veta om det är en bil eller MC.
- Två projekt (konsol + klassbibliotek) tvingar fram en tydlig
ansvarsfördelning: `MinaFiler` vet inget om parkering.
- `Copy to Output Directory` + `AppContext.BaseDirectory` är lösningen
på "filen hittades inte" – inte att lägga filer i `bin/Debug` manuellt.
- `SparatFordon` + `SkapaFordon` är ett enkelt sätt att spara
polymorfa objekt som JSON.
- `Math.Ceiling` är rätt verktyg för "påbörjad timme".

**Status:**
Prague Parking 2.0 är klar och inlämnad