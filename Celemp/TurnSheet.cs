﻿using System;
using static Celemp.Constants;

namespace Celemp
{
    public class TurnSheet
    {
        private Galaxy galaxy;

        public TurnSheet(Galaxy aGalaxy)
        {
            galaxy = aGalaxy;
        }

        public void GenerateTurnSheets(string celemp_path)
        {
            for (int plrNum = 0; plrNum < numPlayers; plrNum++)
            {
                Console.WriteLine($"Generate turn sheet for player {plrNum}");
                GenerateTurnSheet(celemp_path, plrNum);
            }
        }

        private void GenerateTurnSheet(string celemp_path, int plrNum)
        // Generate the turn sheet output file
        {
            string output_filename = Path.Join(celemp_path, $"turn_{plrNum}_{galaxy!.turn}.tex");
            Console.WriteLine($"Generating {output_filename}");
            Player plr = galaxy.players[plrNum];
            using (StreamWriter sw = File.CreateText(output_filename))
            {
                TitlePage(sw);
                TurnSheetHeadings(sw, plr);
                TurnWinningConditions(sw, plr);
                TurnEarthBids(sw);
                TurnShipTypeSummary(sw);
                TurnOwnerSummary(sw);
                TurnPlanetSummary(sw);
                TurnShipSummary(sw);
                TurnPlanetDetails(sw, plr);
                TurnCommandHistory(sw, plr);
                TurnFooter(sw);
            }
        }

        private void TurnPlanetDetails(StreamWriter outfh, Player plr)
        {
            for (int planNum = 0; planNum < numPlanets; planNum++)
            {
                if (galaxy!.planets[planNum].Knows(plr.number))
                {
                    galaxy.planets[planNum].TurnPlanetDetails(outfh);
                    foreach (KeyValuePair<int, Ship> ship in galaxy.ships)
                    {
                        if (ship.Value.planet == planNum)
                            ship.Value.TurnFriendShip(outfh);
                    }
                }
            }
        }

        private void TurnCommandHistory(StreamWriter outfh, Player plr)
        {
            outfh.WriteLine("\\section*{Command history}");
            outfh.WriteLine("\\begin{itemize}");
            foreach (string cmd in plr.executed)
                outfh.WriteLine($"\\item {cmd}");
            if (plr.executed.Count == 0)
                outfh.WriteLine("\\item No commands entered");
            outfh.WriteLine("\\end{itemize}");
        }

        private void TurnOwnerSummary(StreamWriter outfh)
        {
            outfh.WriteLine("Owner Summary");   // TODO - Complete
        }

        private void TurnShipTypeSummary(StreamWriter outfh)
        {
            outfh.WriteLine("Ship Type Summary");   // TODO - Complete
        }

        private void TurnShipSummary(StreamWriter outfh)
        {
            outfh.WriteLine("Ship Summary");    // TODO - Complete
        }

        private void TurnPlanetSummary(StreamWriter outfh)
        {
            outfh.WriteLine("Planet Summary");  // TODO - Complete
        }

        private void TurnFooter(StreamWriter outfh)
        {
            outfh.Write("\\end{document}\n");
        }

        private void TitlePage(StreamWriter outfh)
        {
            outfh.Write("\\documentclass{article}\n");
            outfh.Write("\\usepackage{longtable, graphicx, epstopdf, changepage, a4wide}\n");
            outfh.Write("\\title{Celestial Empire Game}\n");
            outfh.Write("\\author{Dougal Scott $<$dougal.scott@gmail.com$>$}\n");
            outfh.Write("\\begin{document}\n");
            outfh.Write("\\maketitle\n");
        }

        private void TurnSheetHeadings(StreamWriter outfh, Player plr)
        {
            outfh.Write("\n");
            outfh.Write("\\section*{" + plr.name + "}\n");
            /* Print out score and gm and turn numbers */
            outfh.Write("\\subsection*{Turn " + galaxy!.turn + "}\n");
            outfh.Write("\\begin{itemize}\n");
            outfh.Write("\\item score=" + plr.score + "\n");
            /* Print out due date, and player scores */
            outfh.Write("\\item date due= before " + galaxy.duedate + "\n");
            outfh.Write("\\item your income=", plr.Income() + "\n");
            outfh.Write("\\item Earth credits=" + plr.earthCredit + "\n");
            outfh.Write("\\item Credits:Score=" + galaxy.earthMult + "\n");
            outfh.Write($"\\item You have {plr.NumScans()} scans this turn\n");
            outfh.Write("\\item \\begin{tabular}{c|c|c}\n");
            outfh.Write("\\multicolumn{3}{c}{Player Scores}\\\\ \\hline \n");
            outfh.Write($"{galaxy!.players[0].score} & {galaxy.players[1].score}& {galaxy.players[2].score}\\\\ \n");
            outfh.Write($"{galaxy!.players[3].score} & {galaxy.players[4].score} & {galaxy.players[5].score}\\\\ \n");
            outfh.Write($"{galaxy!.players[6].score} & {galaxy.players[7].score} & {galaxy.players[8].score}\\\\ \n");
            outfh.Write("\\end{tabular}\n\n");
            outfh.Write("\\end{itemize}\n\n");
        }

        private void TurnEarthBids(StreamWriter outfh)
        {
            outfh.Write("Minimum bids:\n");
            outfh.Write("\\begin{itemize}\n");
            outfh.Write($"\\item Cargo={galaxy!.earthBids["Cargo"]}\n");
            outfh.Write($"\\item Fighter={galaxy!.earthBids["Fighter"]}\n");
            outfh.Write($"\\item Shield={galaxy!.earthBids["Shield"]}\n");
            outfh.Write("\\end{itemize}\n");
        }

        private void TurnWinningConditions(StreamWriter outfh, Player plr)
        {
            /* Print winning conditions */
            outfh.Write("\\subsection*{Winning Conditions}\n");
            outfh.Write("\\begin{itemize}\n");
            if (galaxy!.winning_terms!["Earth Ownership"].Item1)
            {
                outfh.Write("\\item Earth\n");
            }
            if (galaxy!.winning_terms["Credit"].Item1)
            {
                outfh.Write("\\item Credits=" + galaxy.winning_terms["Credit"].Item2 + "\n");
            }
            if (galaxy!.winning_terms["Income"].Item1)
            {
                outfh.Write("\\item Income=" + galaxy.winning_terms["Income"].Item2 + "\n");
            }
            if (galaxy!.winning_terms["Score"].Item1)
            {
                outfh.Write("\\item Score=" + galaxy.winning_terms["Score"].Item2 + "\n");
            }
            if (galaxy!.winning_terms["Planets"].Item1)
            {
                outfh.Write("\\item Planets=" + galaxy.winning_terms["Planets"].Item2 + "\n");
            }
            if (galaxy!.winning_terms["Fixed Turn"].Item1)
            {
                outfh.Write("\\item Turn=" + galaxy.winning_terms["Fixed Turn"].Item2 + "\n");
            }
            if (galaxy!.winning_terms["Variable Turn"].Item1)
            {
                outfh.Write($"\\item Desired end turn={plr.desired_endturn}\n");
            }
            outfh.Write("\\end{itemize}\n");
        }
    }
}
