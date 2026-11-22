using Turnaj.Model;

namespace Turnaj.Logic;
public static class RoundPrinter
{
    public static string RoundInfo(Round inputRound)
    {
        string roundValue = "";
        roundValue += $"Round:{inputRound.Number}\n";
        roundValue += $"Matches:\n";
        if (inputRound.Matches.Count == 0)
        {
            roundValue += "No Matches were played.\n";
        }
        else
        {
            foreach(Match match in inputRound.Matches)
            {
                roundValue += $"Players: {match.First} against {match.Second}\n";
                roundValue += $"Winner:";
                switch (match.Result)
                {
                    case MatchResult.None:
                    roundValue += $"No winner\n";
                        break;
                    case MatchResult.FirstWins:
                    roundValue += $"{match.First}\n";
                        break;
                    case MatchResult.SecondWins:
                    roundValue += $"{match.Second}\n";
                        break;
                    case MatchResult.Draw:
                    roundValue += $"Draw\n";
                        break;
                }
            }
        }
        return(roundValue);
    }
}
