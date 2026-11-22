using Turnaj.Model;
namespace Turnaj.UI;
public static class BoardPrinter
{
    public static void PrintBoard(List<StandingRow> board)
    {
        if (board.Count == 0)
        {
            Console.WriteLine("V tabulce zatím nikdo není.");
        }
        else
        {
            foreach(StandingRow row in board)
            {
                Console.WriteLine($"{row.Rank}. {row.Nickname} | {row.Points} Bodů | {row.Played} Her");
            }
        }
    }
}