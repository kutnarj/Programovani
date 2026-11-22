using System;
using System.IO;
using Turnaj.Model;

public static class TournamentReader
{
    public static Tournament ReadTournament(string filePath)
    {
        string[] radky = File.ReadAllLines(filePath);

        string nazev = "";
        foreach (string radek in radky)
        {
            if (string.IsNullOrWhiteSpace(radek))
            {
                continue;
            }

            string[] casti = radek.Split('|');
            if (casti[0] == "NAZEV")
            {
                nazev = casti[1];
                break;
            }
        }

        // Doplň druhý string a int podle toho, co chce konstruktor Tournament(string, string, int)
        Tournament t = new Tournament(nazev, "", 0);
        Round aktualniKolo = null;

        foreach (string radek in radky)
        {
            if (string.IsNullOrWhiteSpace(radek))
            {
                continue;
            }

            string[] casti = radek.Split('|');
            string stitek = casti[0];

            if (stitek == "HRAC")
            {
                string hrac = casti[1];
                bool uzTamJe = false;

                foreach (string existujici in t.Enrolled)
                {
                    if (Nick.Eq(existujici, hrac))
                    {
                        uzTamJe = true;
                        break;
                    }
                }

                if (!uzTamJe)
                {
                    t.Enrolled.Add(hrac);
                }
            }
            else if (stitek == "KOLO")
            {
                int cisloKola = int.Parse(casti[1]);
                aktualniKolo = new Round(cisloKola);
                t.Rounds.Add(aktualniKolo);
            }
            else if (stitek == "ZAPAS")
            {
                if (aktualniKolo != null)
                {
                    string prvni = casti[1];
                    string druhy = casti[2];
                    MatchResult vysledek = Enum.Parse<MatchResult>(casti[3]);

                    Match zapas = new Match(prvni, druhy);
                    zapas.Result = vysledek;
                    aktualniKolo.Matches.Add(zapas);
                }
            }
        }

        return t;
    }
}