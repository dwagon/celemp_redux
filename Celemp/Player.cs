using static Celemp.Constants;

namespace Celemp
{
    [Serializable]
    public class Player
    {
        public int score { get; set; }
        public int income { get; set; }
        public String name { get; set; }
        public int number { get; set; }
        public int earthCredit { get; set; }
        public int desired_endturn { get; set; }
        public int home_planet { get; set; }
        public List<string> executed { get; set; }

        private Galaxy? galaxy;
        private int scans;

        public Player()
        {
            score = 0;
            income = 0;
            name = "Unknown";
            number = -1;
            earthCredit = 0;
            desired_endturn = 30;
            home_planet = -1;
            scans = 0;
            executed = new();
        }

        public void InitialiseTurn()
        {
            scans = galaxy!.NumberResearchPlanetsOwned(number) + 1;
            executed = new();
        }

        public IEnumerable<Command> ParseCommandStrings(List<string> aCommands)
        {
            foreach (string command in aCommands)
            {
                Command cmd = new(command, number);
                yield return cmd;
            }
        }

        public void ProcessCommand(Command cmd) {
            Console.WriteLine($"Processing command {cmd.cmdstr}");
            switch (cmd.priority)
            {
                case CommandOrder.SCAN:
                    Cmd_Scan(cmd);
                    break;
                case CommandOrder.NAMESHIP:
                    Cmd_NameShip(cmd);
                    break;
                case CommandOrder.LOADALL:
                    Cmd_LoadAll(cmd);
                    break;
                case CommandOrder.LOADORE:
                    Cmd_LoadOre(cmd);
                    break;
                case CommandOrder.LOADIND:
                    Cmd_LoadIndustry(cmd);
                    break;
                case CommandOrder.LOADMIN:
                    Cmd_LoadMine(cmd);
                    break;
                case CommandOrder.LOADSPM:
                    Cmd_LoadSpacemine(cmd);
                    break;
                case CommandOrder.LOADDEF:
                    Cmd_LoadPDU(cmd);
                    break;
                default:
                    Console.WriteLine($"Command not implemented {cmd.cmdstr}");
                    break;
            }
        }

        private void Cmd_LoadAll(Command cmd) {
            // Load all available ore (Except type 0) onto ship
            int ship = cmd.numbers["ship"];
            int planet = galaxy!.ships[ship].planet;

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;

            for (int oretype=1;oretype<numOreTypes;oretype++)
            {
                int amount = galaxy.planets[planet].ore[oretype];
                if (galaxy.ships[ship].cargoleft < amount)
                    amount = galaxy.ships[ship].cargoleft;
                galaxy.planets[planet].ore[oretype] -= amount;
                galaxy.ships[ship].LoadShip($"Ore {oretype}", amount);
            }
            executed.Add($"{cmd.cmdstr} - OK");
        }

        private void Cmd_LoadOre(Command cmd)
        {
            int ship = cmd.numbers["ship"];
            int planet = galaxy!.ships[ship].planet;
            int amount = cmd.numbers["amount"];
            int oretype = cmd.numbers["oretype"];

            Console.WriteLine($"Cmd_LoadOre({cmd.cmdstr})");
            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
            if (galaxy.ships[ship].cargoleft < amount)
                amount = galaxy.ships[ship].cargoleft;
            if (galaxy.planets[planet].ore[oretype] < amount)
                amount = galaxy.planets[planet].ore[oretype];
            galaxy.ships[ship].LoadShip($"Ore {oretype}", amount);
            galaxy.planets[planet].ore[oretype] -= amount;
            executed.Add($"{cmd.cmdstr} - Loaded {amount}");
        }

        private void Cmd_LoadIndustry(Command cmd) {
            int ship = cmd.numbers["ship"];
            int planet = galaxy!.ships[ship].planet;
            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
        }

        private void Cmd_LoadMine(Command cmd) {
            int ship = cmd.numbers["ship"];
            int planet = galaxy!.ships[ship].planet;
            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
        }
        private void Cmd_LoadSpacemine(Command cmd) {
            int ship = cmd.numbers["ship"];
            int planet = galaxy!.ships[ship].planet;
            if (!CheckShipOwnership(ship,cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
        }

        private void Cmd_LoadPDU(Command cmd) {
            int ship = cmd.numbers["ship"];
            int planet = galaxy!.ships[ship].planet;
            if (!CheckShipOwnership(ship,cmd) || !CheckPlanetOwnership(planet,cmd))
                return;
        }

        private void Cmd_NameShip(Command cmd)
        {
            int ship = cmd.numbers["ship"];
            if (!CheckShipOwnership(ship, cmd))
                return;
            galaxy!.ships[ship].name = cmd.strings["name"];
            executed.Add($"{cmd.cmdstr} - OK");
        }

        private void Cmd_Scan(Command cmd)
        {
            if (scans > 0)
            {
                galaxy!.planets[cmd.numbers["planet"]].Scan(cmd.plrNum);
                executed.Add($"{cmd.cmdstr} - OK");
                scans--;
            }
            else
            {
                executed.Add($"{cmd.cmdstr} - Failed: no more scans");
            }
        }

        private bool CheckShipOwnership(int shipnum)
        {
            if (galaxy!.ships[shipnum].owner == number)
                return true;
            return false;
        }

        private bool CheckShipOwnership(int shipnum, Command cmd)
        {
            if (galaxy!.ships[shipnum].owner == number)
                return true;
            executed.Add($"{cmd.cmdstr} - Failed: You do not own ship");
            return false;
        }

        private bool CheckPlanetOwnership(int plannum)
        {
            if (galaxy!.planets[plannum].owner == number)
                return true;
            return false;
        }

        private bool CheckPlanetOwnership(int plannum, Command cmd)
        {
            if (galaxy!.planets[plannum].owner == number)
                return true;
            executed.Add($"{cmd.cmdstr} - Failed: You do not own planet {plannum}");
            return false;
        }

        public void InitPlayer(Galaxy aGalaxy, int aPlrNum)
        {
            galaxy = aGalaxy;
            number = aPlrNum;
        }

        public void InitPlayer(Galaxy aGalaxy)
        {
            galaxy = aGalaxy;
        }

        public void GenerateTurnSheet(string celemp_path)
        // Generate the turn sheet output file
        {
            string output_filename = Path.Join(celemp_path, $"turn_{number}_{galaxy!.turn}.tex");
            Console.WriteLine($"Generating {output_filename}");
            using (StreamWriter sw = File.CreateText(output_filename))
            {
                TitlePage(sw);
                TurnSheetHeadings(sw);
                TurnWinningConditions(sw);
                TurnEarthBids(sw);
                TurnShipTypeSummary(sw);
                TurnOwnerSummary(sw);
                TurnPlanetSummary(sw);
                TurnShipSummary(sw);
                TurnPlanetDetails(sw);
                TurnCommandHistory(sw);
                TurnFooter(sw);
            }
        }

        private void TurnPlanetDetails(StreamWriter outfh)
        {
            for(int planNum=0;planNum<numPlanets;planNum++)
            {
                if (galaxy!.planets[planNum].Knows(number))
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

        private void TurnCommandHistory(StreamWriter outfh)
        {
            outfh.WriteLine("\\section*{Command history}");
            outfh.WriteLine("\\begin{itemize}");
            foreach(string cmd in executed)
                outfh.WriteLine($"\\item {cmd}");
            if (executed.Count == 0)
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

        private void TurnSheetHeadings(StreamWriter outfh)
        {
            outfh.Write("\n");
            outfh.Write("\\section*{" + name + "}\n");
            /* Print out score and gm and turn numbers */
            outfh.Write("\\subsection*{Turn " + galaxy!.turn + "}\n");
            outfh.Write("\\begin{itemize}\n");
            outfh.Write("\\item score=" + score + "\n");
            /* Print out due date, and player scores */
            outfh.Write("\\item date due= before " + galaxy.duedate + "\n");
            outfh.Write("\\item your income=", income + "\n");
            outfh.Write("\\item Earth credits=" + earthCredit + "\n");
            outfh.Write("\\item Credits:Score=" + galaxy.earthMult + "\n");
            outfh.Write($"\\item You have {scans} scans this turn\n");
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

        private void TurnWinningConditions(StreamWriter outfh)
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
                outfh.Write($"\\item Desired end turn={desired_endturn}\n");
            }
            outfh.Write("\\end{itemize}\n");
        }
    }
}