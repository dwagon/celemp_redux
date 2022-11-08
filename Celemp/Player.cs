using System.Numerics;
using static Celemp.Constants;

namespace Celemp
{
    [Serializable]
    public class Player
    {
        public int score { get; set; }
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
            name = "Unknown";
            number = -1;
            earthCredit = 0;
            desired_endturn = 30;
            home_planet = -1;
            executed = new();
        }

        public Player(String name): this()
        {
            this.name = name;
        }

        public int NumScans()
            // Number of scans available to player
        {
            return scans;
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

        public void EndTurn()
        {
            score += Income();
        }

        public int Income()
        {
            int inc = 0;
            for (int planNum=0; planNum < numPlanets; planNum++)
            {
                if (galaxy!.planets[planNum].owner == number) 
                    inc += galaxy!.planets[planNum].Income();
            }
            int numrp = galaxy!.NumberResearchPlanetsOwned(number);
            if (numrp < 4)
                inc += numrp * numrp * numrp * numrp;
            else
                inc += 256;
            return inc;
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
                case CommandOrder.LOADPDU:
                    Cmd_LoadPDU(cmd);
                    break;
                case CommandOrder.UNLOADPDU:
                    Cmd_UnloadPDU(cmd);
                    break;
                case CommandOrder.JUMP1:
                    Cmd_Jump1(cmd);
                    break;
                case CommandOrder.JUMP2:
                    Cmd_Jump2(cmd);
                    break;
                case CommandOrder.JUMP3:
                    Cmd_Jump3(cmd);
                    break;
                case CommandOrder.JUMP4:
                    Cmd_Jump4(cmd);
                    break;
                case CommandOrder.JUMP5:
                    Cmd_Jump5(cmd);
                    break;
                case CommandOrder.GIFTSHIP:
                    Cmd_GiftShip(cmd);
                    break;
                case CommandOrder.GIFTPLAN:
                    Cmd_GiftPlan(cmd);
                    break;
                case CommandOrder.BUILDMIN:
                    Cmd_BuildMine(cmd);
                    break;
                default:
                    Console.WriteLine($"Command not implemented {cmd.cmdstr}");
                    break;
            }
        }

        public void Cmd_BuildMine(Command cmd)
        {
            Planet plan = galaxy!.planets[cmd.numbers["planet"]];
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            if (plan.indleft < amount * 10)
                amount = plan.indleft / 10;
            if (plan.ore[8] < amount * 5)
                amount = plan.ore[8] / 5;
            if (plan.ore[9] < amount * 5)
                amount = plan.ore[9] / 5;
            plan.indleft -= amount * 10;
            plan.ore[8] -= amount * 5;
            plan.ore[9] -= amount * 5;
            plan.mine[cmd.numbers["oretype"]] += amount;
            executed.Add($"{cmd.cmdstr} - Built {amount}");
        }

        public void Cmd_GiftShip(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            String recip = cmd.strings["recipient"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            int new_owner =galaxy.GuessPlayerName(recip);
            if (new_owner < 0)
            {
                executed.Add($"{cmd.cmdstr} - Unknown player {recip} ");
                return;
            }
            ship.owner = new_owner;
            executed.Add($"{cmd.cmdstr} - OK");
        }

        public void Cmd_GiftPlan(Command cmd)
            // Gift a planet
        {
            Planet plan = galaxy!.planets[cmd.numbers["planet"]];
            String recip = cmd.strings["recipient"];
            int new_owner = galaxy.GuessPlayerName(recip);

            if (!CheckPlanetOwnership(plan, cmd))
                return;
            if (new_owner < 0)
            {
                executed.Add($"{cmd.cmdstr} - Unknown player {recip}");
                return;
            }
            plan.owner = new_owner;
            executed.Add($"{cmd.cmdstr} - OK");
        }

        private void Cmd_Jump1(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            int dest1 = cmd.numbers["jump1"];

            if (!JumpChecks(ship, cmd, 2))
                return;
            if (!CheckDest(ship, dest1, cmd))
                return;
            ship.MoveTo(dest1);
            executed.Add($"{cmd.cmdstr} - OK");
        }

        private void Cmd_Jump2(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            if (!JumpChecks(ship, cmd, 2))
                return;

            int dest1 = cmd.numbers["jump1"];
            if (!CheckDest(ship, dest1, cmd))
                return;
            ship.MoveTo(dest1, true);
            int dest2 = cmd.numbers["jump2"];
            if (!CheckDest(ship, dest2, cmd))
                return;
            ship.MoveTo(dest2);
            executed.Add($"{cmd.cmdstr} - OK");
        }

        private void Cmd_Jump3(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            if (!JumpChecks(ship, cmd, 3))
                return;

            int dest1 = cmd.numbers["jump1"];
            if (!CheckDest(ship, dest1, cmd))
                return;
            ship.MoveTo(dest1, true);
            int dest2 = cmd.numbers["jump2"];
            if (!CheckDest(ship, dest2, cmd))
                return;
            ship.MoveTo(dest2, true);
            int dest3 = cmd.numbers["jump3"];
            if (!CheckDest(ship, dest3, cmd))
                return;
            ship.MoveTo(dest3);
            executed.Add($"{cmd.cmdstr} - OK");
        }

        private void Cmd_Jump4(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            if (!JumpChecks(ship, cmd, 4))
                return;

            int dest1 = cmd.numbers["jump1"];
            if (!CheckDest(ship, dest1, cmd))
                return;
            ship.MoveTo(dest1, true);
            int dest2 = cmd.numbers["jump2"];
            if (!CheckDest(ship, dest2, cmd))
                return;
            ship.MoveTo(dest2, true);
            int dest3 = cmd.numbers["jump3"];
            if (!CheckDest(ship, dest3, cmd))
                return;
            ship.MoveTo(dest3, true);
            int dest4 = cmd.numbers["jump4"];
            if (!CheckDest(ship, dest4, cmd))
                return;
            ship.MoveTo(dest4);
            executed.Add($"{cmd.cmdstr} - OK");
        }

        private void Cmd_Jump5(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            if (!JumpChecks(ship, cmd, 5))
                return;

            int dest1 = cmd.numbers["jump1"];
            if (!CheckDest(ship, dest1, cmd))
                return;
            ship.MoveTo(dest1, true);
            int dest2 = cmd.numbers["jump2"];
            if (!CheckDest(ship, dest2, cmd))
                return;
            ship.MoveTo(dest2, true);
            int dest3 = cmd.numbers["jump3"];
            if (!CheckDest(ship, dest3, cmd))
                return;
            ship.MoveTo(dest3, true);
            int dest4 = cmd.numbers["jump4"];
            if (!CheckDest(ship, dest4, cmd))
                return;
            ship.MoveTo(dest4, true);
            int dest5 = cmd.numbers["jump5"];
            if (!CheckDest(ship, dest5, cmd))
                return;
            ship.MoveTo(dest5);
            executed.Add($"{cmd.cmdstr} - OK");
        }

        private bool JumpChecks(Ship ship, Command cmd, int jumplength)
        // Standard checks for every jump
        {
            if (!CheckShipOwnership(ship, cmd))
                return false;
            if (!CheckShipMoved(ship, cmd))
                return false;
            if (!CheckFuel(ship, cmd, jumplength))
                return false;
            return true;
        }

        private bool CheckFuel(Ship aShip, Command cmd, int distance) {
            if (!aShip.UseFuel(distance))
            {
                executed.Add($"{cmd.cmdstr} - Failed to jump, insufficient fuel");
                return false;
            }
            return true;
        }

        private bool CheckDest(Ship aShip, int dest, Command cmd)
        {
            if (!aShip.CheckDest(dest))
            {
                executed.Add($"{cmd.cmdstr} - Failed to jump, invalid destination");
            }
            return true;
        }

        private bool CheckShipMoved(Ship aShip, Command cmd)
        {
            // Check for the ship having moved this turn
            if (!aShip.HasMoved())
                return true;
            executed.Add($"{cmd.cmdstr} - Failed: Ship has already moved this turn");
            return false;
        }

        private bool CheckShipEngaged(Ship aShip, Command cmd)
        {
            if (aShip.IsEngaged())
                return true;
            executed.Add($"{cmd.cmdstr} - Failed: Ship is engaged by a tractor beam");
            return false;
        }

        private void Cmd_LoadAll(Command cmd) {
            // Load all available ore (Except type 0) onto ship
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;

            for (int oretype=1;oretype<numOreTypes;oretype++)
            {
                int amount = planet.ore[oretype];
                if (ship.cargoleft < amount)
                    amount = ship.cargoleft;
                planet.ore[oretype] -= amount;
                ship.LoadShip($"Ore {oretype}", amount);
            }
            executed.Add($"{cmd.cmdstr} - OK");
        }

        private void Cmd_LoadOre(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            int amount = cmd.numbers["amount"];
            int oretype = cmd.numbers["oretype"];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
            if (planet.ore[oretype] < amount)
                amount = planet.ore[oretype];
            amount = ship.LoadShip($"Ore {oretype}", amount);
            planet.ore[oretype] -= amount;
            executed.Add($"{cmd.cmdstr} - Loaded {amount}");
        }

        private void Cmd_LoadIndustry(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
        }

        private void Cmd_LoadMine(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];


            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
        }
        private void Cmd_LoadSpacemine(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship,cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
        }

        public void Cmd_LoadPDU(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];

            if (!CheckShipOwnership(ship,cmd) || !CheckPlanetOwnership(planet,cmd))
                return;
            if (planet.pdu < amount)
                amount = planet.pdu;
            amount = ship.LoadShip("PDU", amount);
            planet.pdu -= amount;
            executed.Add($"{cmd.cmdstr} - Loaded {amount}");
        }

        public void Cmd_UnloadPDU(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            if (ship.carrying["PDU"] < amount)
                amount = ship.carrying["PDU"];
            amount = ship.UnloadShip("PDU", amount);
            planet.pdu += amount;
            executed.Add($"{cmd.cmdstr} - Unloaded {amount}");
        }

        private void Cmd_NameShip(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            if (!CheckShipOwnership(ship, cmd))
                return;
            ship.name = cmd.strings["name"];
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

        private bool CheckShipOwnership(Ship aShip)
        {
            if (aShip.owner == number)
                return true;
            return false;
        }

        private bool CheckShipOwnership(Ship aShip, Command cmd)
        {
            if (aShip.owner == number)
                return true;
            executed.Add($"{cmd.cmdstr} - Failed: You do not own ship {aShip.number+100}");
            return false;
        }

        private bool CheckPlanetOwnership(Planet aPlanet)
        {
            if (aPlanet.owner == number)
                return true;
            return false;
        }

        private bool CheckPlanetOwnership(Planet aPlanet, Command cmd)
        {
            if (aPlanet.owner == number)
                return true;
            executed.Add($"{cmd.cmdstr} - Failed: You do not own planet {aPlanet.DisplayNumber()}");
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

        public void OutputLog()
            // Dump executed output for debugging purposes
        {
            foreach (string str in executed)
            {
                Console.WriteLine(str);
            }
        }
    }
}