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

        public void GenerateTurnSheets(string celemp_path, NeoUpdate neo)
        {
            for (int plrNum = 0; plrNum < numPlayers; plrNum++)
            {
                Console.WriteLine($"Generate turn sheet for player {plrNum}");
                GenerateTurnSheet(celemp_path, plrNum, neo);
            }
        }

        private void GenerateTurnSheet(string celemp_path, int plrNum, NeoUpdate neo)
        // Generate the turn sheet output file
        {
            string output_filename = Path.Join(celemp_path, $"turn_{plrNum}_{galaxy!.turn}.tex");
            Player plr = galaxy.players[plrNum];
            using (StreamWriter sw = File.CreateText(output_filename))
            {
                TitlePage(sw);
                Header(sw, plr);
                Winning_Conditions(sw, plr);
                Earth_Details(sw);
                Earth_Bids(sw);
                Ship_Type_Summary(sw);
                Owner_Summary(sw);
                Planet_Summary(sw, plr);
                Ship_Summary(sw, plr);
                Planet_Details(sw, plr, neo);
                Messages(sw, plr);
                Command_History(sw, plr);
                Footer(sw);
            }
        }

        private void Messages(StreamWriter outfh, Player plr)
        {
            if (plr.messages.Count() > 0)
            {
                outfh.WriteLine("\\section*{Messages}");
                outfh.WriteLine("\\begin{verbatim}");
                foreach (string msg in plr.messages)
                {
                    outfh.Write($"{msg}\n");
                }
                outfh.WriteLine("\\end{verbatim}");
            }
        }

        private void Earth_Details(StreamWriter outfh)
        {
            Planet earth = galaxy.planets[galaxy.earth_planet];
            outfh.WriteLine("\\section*{Earth}");
            outfh.WriteLine("\\begin{itemize}");
            /*
            if(gamedet.earth.flag & WBUYALLORE)
            fprintf(output, "\\item Unlimited Selling\n");
            if(gamedet.earth.flag & WBUY100ORE)
            fprintf(output, "\\item Limited Selling\n");
            */
            outfh.WriteLine("\\item Planet: " + earth.DisplayNumber());
            if (galaxy.turn < galaxy.earthAmnesty)
                outfh.WriteLine("\\item Earth amnesty in effect. No shots allowed.");
            outfh.WriteLine($"\\item Nearby Planets {earth.DisplayNumber(earth.link[0])} {earth.DisplayNumber(earth.link[1])} {earth.DisplayNumber(earth.link[2])}");
            outfh.WriteLine("\\end{itemize}");

            outfh.WriteLine("\\subsection*{Earth Trading Prices}");
            outfh.WriteLine("\\begin{tabular}{rllllllllll}");
            outfh.Write("Ore Type");
            for (int oreType = 0; oreType < numOreTypes; oreType++)
                outfh.Write($" & {oreType}");
            outfh.WriteLine("\\\\");
            outfh.Write("Price");
            for (int oreType = 0; oreType < numOreTypes; oreType++)
                outfh.Write($" & {galaxy.earth_price[oreType]}");
            outfh.WriteLine("\\\\");
            outfh.Write("Amount");
            for (int oreType = 0; oreType < numOreTypes; oreType++)
                outfh.Write($" & {earth.ore[oreType]}");
            outfh.WriteLine("\\\\");

            outfh.WriteLine("\\end{tabular}");
            outfh.WriteLine("");
        }

        private void Planet_Details(StreamWriter outfh, Player plr, NeoUpdate neo)
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
                            if (plr.number == ship.Value.owner || plr.number == 0)
                                Friend_Ship(outfh, ship.Value, neo);
                            else
                                Enemy_Ship(outfh, ship.Value);
                    }
                }
            }
        }

        private void Command_History(StreamWriter outfh, Player plr)
        {
            outfh.WriteLine("\\section*{Command history}");
            outfh.WriteLine("\\begin{itemize}");
            foreach (string cmd in plr.cmd_results)
                outfh.WriteLine($"\\item {cmd}");
            if (plr.cmd_results.Count == 0)
                outfh.WriteLine("\\item No commands entered");
            outfh.WriteLine("\\end{itemize}\n");
        }

        private void Owner_Summary(StreamWriter outfh)
        {
            // outfh.WriteLine("Owner Summary");   // TODO - Complete
        }

        private void Ship_Type_Summary(StreamWriter outfh)
        {
            Dictionary<ShipType, int> stypes = new();
            ShipType st;

            foreach (KeyValuePair<int, Ship> s in galaxy.ships)
            {
                st = s.Value.CalcType();
                if (!stypes.ContainsKey(st))
                    stypes[st] = 0;
                stypes[st]++;
            }

            outfh.WriteLine("\\section*{Summary of ship types}");
            outfh.WriteLine("\\begin{tabular}{lr|lr}");
            int count = 0;
            foreach (KeyValuePair<ShipType, int> styp in stypes)
            {
                outfh.Write($"{styp.Key} & {styp.Value}");
                if (count % 2 == 0)
                    outfh.Write(" & ");
                else
                    outfh.WriteLine("\\\\");
                count++;
            }
            if (count % 2 == 0)
                outfh.WriteLine("\\\\");
            outfh.WriteLine("\\end{tabular}\n");
        }

        private void Ship_Summary(StreamWriter outfh, Player plr)
        {
            outfh.WriteLine("\\section*{Summary of ships}");
            outfh.WriteLine("\\begin{longtable}{rllllllll}");
            outfh.WriteLine("Ship & Type & F & C & T & S & Planet & Cargo & Name\\\\ \\hline");
            foreach (KeyValuePair<int, Ship> shp in galaxy.ships)
            {
                Ship s = shp.Value;
                if (s.owner == plr.number || plr.number == 0)
                {
                    outfh.Write(s.DisplayNumber());
                    outfh.Write("& ");
                    outfh.Write(s.CalcType());
                    outfh.Write($" & {s.fighter} & {s.cargo} & {s.tractor} & {s.shield}");
                    outfh.Write($"& {galaxy!.planets[s.planet].DisplayNumber()} &");
                    if (s.carrying[cargo_mine] > 0)
                        outfh.Write($"M: {s.carrying[cargo_mine]}");
                    if (s.carrying[cargo_industry] > 0)
                        outfh.Write($" I: {s.carrying[cargo_industry]}");
                    if (s.carrying[cargo_pdu] > 0)
                        outfh.Write($" D: {s.carrying[cargo_pdu]}");
                    if (s.carrying[cargo_spacemine] > 0)
                        outfh.Write($" SM: {s.carrying[cargo_spacemine]}");
                    for (int oreType = 0; oreType < numOreTypes; oreType++)
                    {
                        if (s.carrying[$"{oreType}"] > 0)
                            outfh.Write($" R{oreType}:{s.carrying[$"{oreType}"]}");
                    }

                    outfh.WriteLine($"& {s.name}");
                    outfh.WriteLine("\\\\");
                }
            }
            outfh.WriteLine("\\end{longtable}");
            outfh.WriteLine("");

        }

        private void Planet_Summary(StreamWriter outfh, Player plr)
        {
            int owned = 0;
            outfh.WriteLine("\\section*{Summary of planets}");
            outfh.WriteLine("\\begin{longtable}{rc@{/}cc@{/}cc@{/}cc@{/}cc@{/}cc@{/}cc@{/}cc@{/}cc@{/}cc@{/}ccc}");
            outfh.Write("Planet &");
            for (int oreType = 0; oreType < numOreTypes; oreType++)
            {
                outfh.Write("\\multicolumn{2}{c}{");
                outfh.Write(oreType);
                outfh.Write("} & ");
            }
            outfh.WriteLine("PDU & Ind \\\\ \\hline");
            for (int planNum = 0; planNum < numPlanets; planNum++)
            {
                Planet p = galaxy.planets[planNum];
                if (p.owner != plr.number && plr.number != 0)
                    continue;
                owned++;
                outfh.Write(p.DisplayNumber());
                outfh.Write(" & ");
                for (int oreType = 0; oreType < numOreTypes; oreType++)
                {
                    if (p.mine[oreType] > 0)
                        outfh.Write(p.mine[oreType]);
                    else
                        outfh.Write("");
                    outfh.Write("&");
                    if (p.ore[oreType] > 0)
                        outfh.Write(p.ore[oreType]);
                    else
                        outfh.Write("");
                    outfh.Write("&");
                }
                outfh.Write(p.pdu);
                outfh.Write("&");
                outfh.Write(p.industry);
                outfh.WriteLine("\\\\");
            }
            outfh.WriteLine("\\end{longtable}");
            outfh.WriteLine($"Total number of planets owned = {owned}");
            outfh.WriteLine("");
        }

        private void Footer(StreamWriter outfh)
        {
            outfh.Write("\\end{document}\n");
        }

        public void Enemy_Ship(StreamWriter outfh, Ship s)
        {
            outfh.WriteLine("\n");
            outfh.WriteLine("\\frame{\\");
            outfh.WriteLine("\\begin{tabular}{rlll}");
            outfh.Write($"{s.DisplayNumber()} & ");
            outfh.Write(galaxy!.players[s.owner].name);
            outfh.Write($"& {s.name} & {s.CalcType()}");
            outfh.WriteLine("\\\\");
            outfh.WriteLine("\\end{tabular}");
            outfh.WriteLine("}\n");
        }

        public void Friend_Ship(StreamWriter outfh, Ship s, NeoUpdate neo)
        {
            bool hasCargo = false;
            String ownerName = galaxy!.players[s.owner].name;
            ShipType type = s.CalcType();

            outfh.WriteLine("\n");
            outfh.WriteLine("\\frame{\\");
            outfh.WriteLine("\\begin{tabular}{rlll}");
            outfh.WriteLine(s.DisplayNumber() + " & \\multicolumn{2}{l}{" + s.name + "} & " + type + "\\\\");
            outfh.WriteLine($"Owner {ownerName} & f={s.fighter} & t={s.tractor} & s={s.shield}({s.ShieldPower()})\\\\");
            outfh.WriteLine($" & cargo={s.cargo} & cargoleft={s.CargoLeft()} & \\\\");
            outfh.WriteLine($" & eff={s.efficiency}(" + s.EffectiveEfficiency() + ") & shots=" + s.Shots(s.fighter) + " & \\\\");

            outfh.Write("Standing & \\multicolumn{3}{l}{");
            if (s.stndord.Length == 0)
            {
                outfh.Write("None");
            }
            else
            {
                outfh.Write(s.stndord);
            };
            outfh.WriteLine("}\\\\");
            outfh.WriteLine("");

            /* Print out cargo details */
            outfh.Write("Cargo & \\multicolumn{3}{l}{");
            if (s.carrying[cargo_industry] != 0)
            {
                outfh.Write("Ind: {" + s.carrying[cargo_industry] + ";");
                hasCargo = true;
            }
            if (s.carrying[cargo_mine] != 0)
            {
                outfh.Write("Mine: " + s.carrying[cargo_mine] + ";");
                hasCargo = true;
            }
            if (s.carrying[cargo_pdu] != 0)
            {
                outfh.Write("PDU: " + s.carrying[cargo_pdu] + ";");
                hasCargo = true;
            }
            if (s.carrying[cargo_spacemine] != 0)
            {
                outfh.Write("SpcMines: " + s.carrying[cargo_spacemine] + ";");
                hasCargo = true;
            }
            for (int oreType = 0; oreType < numOreTypes; oreType++)
                if (s.carrying[$"{oreType}"] != 0)
                {
                    outfh.Write($"R{oreType}: " + s.carrying[$"{oreType}"] + ";");
                    hasCargo = true;
                }
            if (!hasCargo)
            {
                outfh.Write("None");
            };
            outfh.WriteLine("}\\\\");
            PathToHome(outfh, s, neo);
            PathToEarth(outfh, s, neo);
            PathToFuel(outfh, s, neo);
            outfh.WriteLine("\\end{tabular}\n");

            outfh.WriteLine("}");
            outfh.WriteLine("");
        }

        private void PathToEarth(StreamWriter outfh, Ship shp, NeoUpdate neo)
        {
            Player plr = galaxy.players[shp.owner];
            Planet earth = galaxy.planets[galaxy.earth_planet];
            if (shp.planet == earth.number)
                return;
            outfh.Write("Path to Earth & \\multicolumn{3}{l}{");
            foreach (Planet path in neo.RouteToPlanet(galaxy.planets[shp.planet], earth, plr))
            {
                outfh.Write(" $\\rightarrow$ ");
                outfh.Write($"{path.DisplayNumber()}");
            }
            outfh.WriteLine("}\\\\");
        }

        private void PathToHome(StreamWriter outfh, Ship shp, NeoUpdate neo)
        {
            Player plr = galaxy.players[shp.owner];
            Planet home = galaxy.planets[plr.home_planet];
            if (shp.planet == home.number)
                return;
            outfh.Write("Path to Home & \\multicolumn{3}{l}{");
            foreach (Planet path in neo.RouteToPlanet(galaxy.planets[shp.planet], home, plr))
            {
                outfh.Write(" $\\rightarrow$ ");
                outfh.Write($"{path.DisplayNumber()}");
            }
            outfh.WriteLine("}\\\\");
        }

        private void PathToFuel(StreamWriter outfh, Ship shp, NeoUpdate neo)
        {
            Player plr = galaxy.players[shp.owner];

            if (galaxy.planets[shp.planet].ore[0] > 0)
                return;
            outfh.Write("Path to Fuel & \\multicolumn{3}{l}{");
            foreach (Planet path in neo.RouteToFuel(galaxy.planets[shp.planet], plr))
            {
                outfh.Write(" $\\rightarrow$ ");
                outfh.Write($"{path.DisplayNumber()}");
            }
            outfh.WriteLine("}\\\\");
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

        private void Header(StreamWriter outfh, Player plr)
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
            outfh.WriteLine($"\\item You have {plr.InitScans()} scans this turn");
            outfh.WriteLine("\\item \\begin{tabular}{c|c|c}");
            outfh.WriteLine("\\multicolumn{3}{c}{Player Scores}\\\\ \\hline");
            outfh.WriteLine($"{galaxy.players[1].score} & {galaxy.players[2].score} & {galaxy.players[3].score}\\\\");
            outfh.WriteLine($"{galaxy.players[4].score} & {galaxy.players[5].score} & {galaxy.players[6].score}\\\\");
            outfh.WriteLine($"{galaxy.players[7].score} & {galaxy.players[8].score} & {galaxy.players[9].score}\\\\");
            outfh.WriteLine("\\end{tabular}");
            outfh.WriteLine("\\end{itemize}\n");
        }

        private void Earth_Bids(StreamWriter outfh)
        {
            outfh.WriteLine("\\subsection*{Minimum Bids}");
            outfh.WriteLine("\\begin{itemize}");
            outfh.WriteLine($"\\item Cargo={galaxy!.earthBids["Cargo"]}");
            outfh.WriteLine($"\\item Fighter={galaxy!.earthBids["Fighter"]}");
            outfh.WriteLine($"\\item Shield={galaxy!.earthBids["Shield"]}");
            outfh.WriteLine($"\\item Tractor={galaxy!.earthBids["Tractor"]}");
            outfh.WriteLine("\\end{itemize}");
            outfh.WriteLine("");
        }

        private void Winning_Conditions(StreamWriter outfh, Player plr)
        {
            /* Print winning conditions */
            outfh.WriteLine("\\subsection*{Winning Conditions}");
            outfh.WriteLine("\\begin{itemize}");
            if (galaxy!.winning_terms!["Earth Ownership"].Item1)
                outfh.WriteLine("\\item Earth");
            if (galaxy!.winning_terms["Credit"].Item1)
                outfh.WriteLine("\\item Credits=" + galaxy.winning_terms["Credit"].Item2);
            if (galaxy!.winning_terms["Income"].Item1)
                outfh.WriteLine("\\item Income=" + galaxy.winning_terms["Income"].Item2);
            if (galaxy!.winning_terms["Score"].Item1)
                outfh.WriteLine("\\item Score=" + galaxy.winning_terms["Score"].Item2);
            if (galaxy!.winning_terms["Planets"].Item1)
                outfh.WriteLine("\\item Planets=" + galaxy.winning_terms["Planets"].Item2);
            if (galaxy!.winning_terms["Fixed Turn"].Item1)
                outfh.WriteLine("\\item Turn=" + galaxy.winning_terms["Fixed Turn"].Item2);
            if (galaxy!.winning_terms["Variable Turn"].Item1)
                outfh.WriteLine($"\\item Desired end turn={plr.desired_endturn}");
            outfh.WriteLine("\\end{itemize}");
            outfh.WriteLine("\n");
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
                outfh.Write("\\multicolumn{4}{l}{" + p.stndord + "}\\\\\n");
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

