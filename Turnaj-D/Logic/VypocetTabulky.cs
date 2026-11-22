using Turnaj.Model;

namespace Turnaj.Logic;
public static class BoardCalculator
{
    public static List<StandingRow> CalculateBoard(Tournament tournament)
    {
        List<StandingRow> board = new List<StandingRow>();
        List<string> winners = new List<string>();
        List<string> losers = new List<string>();
        List<string> draws = new List<string>();
        foreach(Round round in tournament.Rounds)
        {
            foreach(Match match in round.Matches)
            {
                if (match.HasResult)
                {
                    switch (match.Result)
                    {             
                        case MatchResult.FirstWins:
                        winners.Add(match.First);
                        losers.Add(match.Second);
                            break;

                        case MatchResult.SecondWins:
                        winners.Add(match.Second);
                        losers.Add(match.First);
                            break;
                        case MatchResult.Draw:
                        draws.Add(match.Second);
                        draws.Add(match.First);
                            break;
                    }
                }
                
            }
        }
        foreach(var player in tournament.Enrolled)
        {
            int points = 0;
            int played = 0;
            foreach(string nick in winners)
            {
                if (Nick.Eq(nick, player))
                {
                    points += 2;
                    played++;
                }
            }
            foreach(string nick in losers)
            {
                if (Nick.Eq(nick, player))
                {
                    played++;
                }
            }
            foreach(string nick in draws)
            {
                if (Nick.Eq(nick, player))
                {
                    points++;
                    played++;
                }
            }
            board.Add(new StandingRow(0, player, points, played));
        }
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = i + 1; j < board.Count; j++)
            {
                StandingRow prvni = board[i];
                StandingRow druhy = board[j];

                // Když má ten druhý víc bodů než ten první, prohodíme je
                if (druhy.Points > prvni.Points)
                {
                    board[i] = druhy;
                    board[j] = prvni;
                }
            }
        }
        for (int i = 0; i < board.Count; i++)
        {
            StandingRow radek = board[i];
            board[i] = new StandingRow(i + 1, radek.Nickname, radek.Points, radek.Played);
        }
        return board;
    }
}
