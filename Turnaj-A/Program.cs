using System.Text;

// ============================================================
//  Část pro A: T1 přidání hráče, T2 výpis hráčů, T3 odebrání hráče,
//  T17 chybějící soubor, T19 ošetření vstupů z klávesnice.
//  Vše v jednom souboru. Logika (OperationResult, Player, PlayerRegistry,
//  PlayerService) nepoužívá Console, výpis a vstupy jsou dole (PlayerMenu, InputReader).
// ============================================================

Console.OutputEncoding = Encoding.UTF8;

string path = args.Length > 0 ? args[0] : "turnaj.dat";
Console.WriteLine(StartupCheck.Describe(path));

var service = new PlayerService(new PlayerRegistry());
try
{
    new PlayerMenu(service).Run();
}
catch (EndOfInputException)
{
    Console.WriteLine();
    Console.WriteLine("Konec vstupu, program končí.");
}

// ---------------------- MODEL ----------------------

/// <summary>Výsledek operace logiky. Logika nic nevypisuje, jen vrací tento objekt.</summary>
public class OperationResult
{
    public bool Ok { get; }
    public string Message { get; }

    private OperationResult(bool ok, string message)
    {
        Ok = ok;
        Message = message;
    }

    public static OperationResult Success(string message = "") => new(true, message);
    public static OperationResult Fail(string message) => new(false, message);
}

public class Player
{
    public string Name { get; }
    public string Nickname { get; }

    public Player(string name, string nickname)
    {
        Name = name;
        Nickname = nickname;
    }
}

// ---------------------- LOGIKA ----------------------

public class PlayerRegistry
{
    private readonly Dictionary<string, Player> _players = new(StringComparer.OrdinalIgnoreCase);

    public int Count => _players.Count;

    public IEnumerable<Player> All =>
        _players.Values.OrderBy(p => p.Nickname, StringComparer.OrdinalIgnoreCase);

    public Player? Find(string nick) =>
        _players.TryGetValue(nick.Trim(), out var p) ? p : null;

    public OperationResult Add(string name, string nick)
    {
        name = name.Trim();
        nick = nick.Trim();
        if (name.Length == 0) return OperationResult.Fail("Jméno nesmí být prázdné.");
        if (nick.Length == 0) return OperationResult.Fail("Přezdívka nesmí být prázdná.");
        if (_players.ContainsKey(nick))
            return OperationResult.Fail($"Přezdívka '{nick}' už existuje.");
        _players[nick] = new Player(name, nick);
        return OperationResult.Success($"Hráč '{nick}' byl přidán.");
    }

    public bool Remove(string nick) => _players.Remove(nick.Trim());
}

/// <summary>
/// Spojovací bod na část ostatních členů týmu (turnaj, zápasy).
/// TournamentService ho později implementuje, zatím se používá NoPlayerUsage.
/// </summary>
public interface IPlayerUsage
{
    bool HasPlayedMatch(string nick);
    bool IsEnrolledInOpenTournament(string nick);
}

public class NoPlayerUsage : IPlayerUsage
{
    public bool HasPlayedMatch(string nick) => false;
    public bool IsEnrolledInOpenTournament(string nick) => false;
}

/// <summary>T1, T2, T3: přidání, výpis a odebrání hráče.</summary>
public class PlayerService
{
    private readonly PlayerRegistry _players;
    private readonly IPlayerUsage _usage;

    public PlayerService(PlayerRegistry players, IPlayerUsage? usage = null)
    {
        _players = players;
        _usage = usage ?? new NoPlayerUsage();
    }

    public OperationResult Add(string name, string nick) => _players.Add(name, nick);

    public List<Player> List() => _players.All.ToList();

    public OperationResult Remove(string nick)
    {
        var p = _players.Find(nick);
        if (p == null)
            return OperationResult.Fail($"Hráč s přezdívkou '{nick.Trim()}' neexistuje.");
        if (_usage.HasPlayedMatch(p.Nickname))
            return OperationResult.Fail("Hráč má odehraný zápas, nejde odebrat (zkreslila by se tabulka).");
        if (_usage.IsEnrolledInOpenTournament(p.Nickname))
            return OperationResult.Fail("Hráč je přihlášen do otevřeného turnaje. Nejdřív ho odhlaste.");
        _players.Remove(p.Nickname);
        return OperationResult.Success($"Hráč '{p.Nickname}' byl odebrán.");
    }
}

// ---------------------- ULOŽENÍ (T17) ----------------------

/// <summary>T17: chybějící soubor není chyba, program startuje s prázdným stavem.</summary>
public static class StartupCheck
{
    public static bool FileMissing(string path) => !File.Exists(path);

    public static string Describe(string path) =>
        FileMissing(path)
            ? $"Soubor '{path}' neexistuje, začínáme s prázdným stavem."
            : $"Soubor '{path}' nalezen (jeho načtení řeší T16).";
}

// ---------------------- VSTUPY A MENU (T19) ----------------------

/// <summary>Vstup skončil (Ctrl+D / Ctrl+Z nebo konec přesměrovaného souboru).</summary>
public class EndOfInputException : Exception { }

/// <summary>T19: jediné místo, které čte z klávesnice. Čtení se opakuje, dokud není vstup platný.</summary>
public static class InputReader
{
    private static string ReadRaw()
    {
        string? s = Console.ReadLine();
        if (s == null) throw new EndOfInputException();
        return s.Trim();
    }

    public static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + ": ");
            string s = ReadRaw();
            if (s.Length > 0) return s;
            Console.WriteLine("Vstup nesmí být prázdný.");
        }
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write($"{prompt} [{min}-{max}]: ");
            string s = ReadRaw();
            if (s.Length == 0)
            {
                Console.WriteLine("Vstup nesmí být prázdný.");
                continue;
            }
            if (!int.TryParse(s, out int value))
            {
                Console.WriteLine("Zadejte celé číslo.");
                continue;
            }
            if (value < min || value > max)
            {
                Console.WriteLine($"Číslo musí být v rozsahu {min} až {max}.");
                continue;
            }
            return value;
        }
    }

    public static bool Confirm(string prompt)
    {
        while (true)
        {
            Console.Write(prompt + " (a/n): ");
            string s = ReadRaw().ToLowerInvariant();
            if (s == "a" || s == "ano") return true;
            if (s == "n" || s == "ne") return false;
            Console.WriteLine("Odpovězte 'a' nebo 'n'.");
        }
    }
}

/// <summary>Menu jen pro hráče (položky 1 až 3 hlavního menu).</summary>
public class PlayerMenu
{
    private readonly PlayerService _service;

    public PlayerMenu(PlayerService service)
    {
        _service = service;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1 Přidat hráče   2 Vypsat hráče   3 Odebrat hráče   0 Konec");
            int choice = InputReader.ReadInt("Volba", 0, 3);
            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    string name = InputReader.ReadText("Jméno");
                    string nick = InputReader.ReadText("Přezdívka");
                    Show(_service.Add(name, nick));
                    break;
                case 2:
                    PrintPlayers();
                    break;
                case 3:
                    Show(_service.Remove(InputReader.ReadText("Přezdívka")));
                    break;
            }
        }
    }

    private static void Show(OperationResult r)
    {
        if (r.Ok) Console.WriteLine(r.Message.Length > 0 ? "OK: " + r.Message : "OK.");
        else Console.WriteLine("Chyba: " + r.Message);
    }

    private void PrintPlayers()
    {
        var players = _service.List();
        if (players.Count == 0)
        {
            Console.WriteLine("Zatím nejsou žádní hráči.");
            return;
        }
        Console.WriteLine($"{"Přezdívka",-15} Jméno");
        foreach (var p in players)
            Console.WriteLine($"{p.Nickname,-15} {p.Name}");
    }
}
