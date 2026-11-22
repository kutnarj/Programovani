using Turnaj.Model;

namespace Turnaj.Logic;

public static class TournamentSaver
{
    public static void SaveTournament(Tournament t)
    {
        List<string> radky = new List<string>();
        radky.Add("Turnaj|" + t.Name);
        foreach(string player in t.Enrolled)
        {
            radky.Add("HRAC|" + player);
        }

        foreach(Round round in t.Rounds)
        {
            radky.Add("KOLO|" + round.Number);

            foreach (Match match in round.Matches)
            {
                string radekZapasu = "ZAPAS|" + match.First + "|" + match.Second + "|" + match.Result;
                radky.Add(radekZapasu);
            }
        }
        File.WriteAllLines("turnaj.txt", radky);
    }
}