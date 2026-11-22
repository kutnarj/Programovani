# Správa turnaje

Konzolová aplikace v C# pro evidenci školního turnaje. Pořadatel přidává hráče, založí turnaj, přihlásí účastníky, ručně vytvoří dvojice, zadá výsledky a zobrazí tabulku. Program nehraje žádnou disciplínu; eviduje pouze výsledky.

## Požadavky

- .NET SDK 10.0
- Windows, macOS nebo Linux

## Spuštění

Z kořenového adresáře repozitáře:

```powershell
dotnet run --project Turnaj/Turnaj.csproj
```

Lze zadat vlastní cestu k souboru dat:

```powershell
dotnet run --project Turnaj/Turnaj.csproj -- cesta/k/souboru.dat
```

Výchozí soubor je `turnaj.dat` v aktuálním adresáři.

## Základní pracovní postup

1. Přidejte hráče se jménem a jedinečnou přezdívkou.
2. Založte turnaj s názvem, disciplínou a počtem kol.
3. Přihlaste hráče do turnaje; přihlášky lze měnit do vytvoření prvního kola.
4. Vytvořte kolo a ručně zadejte dvojice.
5. Hráči bez soupeře mohou získat volný los.
6. Zadejte nebo opravte výsledky zápasů do uzavření kola.
7. Uzavřete kolo po zadání všech výsledků.
8. Zobrazte tabulku a případně uložte stav turnaje.

## Projektová struktura

```text
Turnaj/
├── Turnaj.csproj
├── Program.cs
├── Model/       # datové modely a výsledek operace
├── Logic/       # registr hráčů, turnajová a tabulková logika
├── Storage/     # vlastní textový formát a ukládání
└── Ui/          # konzolové menu, vstup a výpis
```

Business logika nepracuje s konzolí. Konzolový vstup a výstup jsou odděleny ve vrstvách `Ui` a `Program.cs`.

## Ukládání

Program používá vlastní řádkový textový formát. Každý záznam obsahuje typ a pole oddělená znakem `|`; textová pole jsou escapována, aby se oddělovač nebo nový řádek neinterpretovali jako struktura souboru.

- `turnaj.dat` — běžný soubor dat
- `turnaj.dat.tmp` — dočasný soubor během bezpečné zápisu
- `turnaj.dat.bad` — kopie poškozeného souboru

Chybějící soubor se při spuštění vyhodnotí jako nový prázdný stav. Poškozený soubor se načte bez přepsání původního data.

## Dokumentace

- [SPECIFIKACE.md](SPECIFIKACE.md) — pojmy, pravidla a funkce
- [NAVRH.md](NAVRH.md) — návrh tříd a souborového formátu
- [User-stories.md](User-stories.md) — uživatelské příběhy
- [TASKY.md](TASKY.md) — tasky a kanban podklad

## Vývojová kontrola

```powershell
dotnet restore Turnaj/Turnaj.csproj
dotnet build Turnaj/Turnaj.csproj
dotnet run --project Turnaj/Turnaj.csproj
```
