using System;
using System.Xml.Linq;
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
                    Planet planet = galaxy.planets[planNum];
                    TurnPlanetDetails(outfh, planet);
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
                outfh.WriteLine($"\\item {cmd.ToUpper()}");
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
            outfh.WriteLine("\\section*{" + plr.name + "}");
            /* Print out score and gm and turn numbers */
            outfh.WriteLine("\\subsection*{Turn " + galaxy!.turn + "}");
            outfh.WriteLine("\\begin{itemize}");
            outfh.WriteLine("\\item Score=" + plr.score);
            /* Print out due date, and player scores */
            outfh.WriteLine("\\item Date due= before " + galaxy.duedate);
            outfh.WriteLine("\\item Your income=" + plr.Income());
            outfh.WriteLine("\\item Earth credits=" + plr.earthCredit);
            outfh.WriteLine("\\item Credits:Score=" + galaxy.earthMult);
            outfh.WriteLine($"\\item You have {plr.NumScans()} scans this turn");
            outfh.WriteLine("\\item \\begin{tabular}{c|c|c}");
            outfh.WriteLine("\\multicolumn{3}{c}{Player Scores}\\\\ \\hline");
            outfh.WriteLine($"{galaxy.players[1].score} & {galaxy.players[2].score} & {galaxy.players[3].score}\\\\");
            outfh.WriteLine($"{galaxy.players[4].score} & {galaxy.players[5].score} & {galaxy.players[6].score}\\\\");
            outfh.WriteLine($"{galaxy.players[7].score} & {galaxy.players[8].score} & {galaxy.players[9].score}\\\\");
            outfh.WriteLine("\\end{tabular}");
            outfh.WriteLine("\\end{itemize}\n");
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

        public void TurnPlanetDetails(StreamWriter outfh, Planet p)
        {
            outfh.Write("\\subsection*{" + p.DisplayNumber() + " " + p.name);
            if (p.research)
                outfh.Write(" --- Research Planet");
            outfh.Write("}\n");
            outfh.Write("\\begin{tabular}{r|llll}\n");
            outfh.Write("Owner & \\multicolumn{4}{l}{" + galaxy!.players[p.owner].name + "}\\\\\n");
            if (p.HasBeenScanned())
            {
                outfh.Write("Scanned & \\multicolumn{4}{l}{Planet scanned this turn}\\\\\n");
            };
            outfh.Write("Nearby Planets");
            for (int count = 0; count < 4; count++)
                if (p.link[count] >= 0)
                    outfh.Write("& " + p.DisplayNumber(p.link[count]));
                else
                    outfh.Write(" & ");
            outfh.Write("\\\\\n");
            outfh.Write($"Industry & Industry={p.industry} & PDU={p.pdu}({p.PduValue()}) & Income={p.Income()} &\\\\\n");
            outfh.Write($"Spacemines & Stored={p.spacemines} & Deployed={p.deployed} & \\\\\n");
            outfh.Write("Standing Order & ");
            if (p.stndord.Length == 0)
                outfh.Write("\\multicolumn{4}{l}{None}\\\\\n");
            else
                outfh.Write("\\multicolumn{4}{l}{" + (p.number + 100) + p.stndord + "}\\\\\n");
            outfh.Write("\\end{tabular}\n\n");

            outfh.Write("\\begin{tabular}{r|cccccccccc}\n");
            outfh.Write("Mine Type");
            for (int oreType = 0; oreType < numOreTypes; oreType++)
            {
                outfh.Write("& " + oreType);
            }
            outfh.Write("\\\\ \\hline \n");
            outfh.Write("Amount stored");
            for (int oreType = 0; oreType < numOreTypes; oreType++)
            {
                outfh.Write($"& {p.ore[oreType]}");
            }
            outfh.Write("\\\\\n");
            outfh.Write("Production");
            for (int mineType = 0; mineType < numOreTypes; mineType++)
            {
                outfh.Write($"& {p.mine[mineType]}");
            }
            outfh.Write("\\\\\n");
            outfh.Write("\\end{tabular}\n");
        }
    }
}

