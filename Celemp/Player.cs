using System.Linq;
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
        public List<string> messages { get; set; }

        private Galaxy? galaxy;
        private int scans;
        private List<string> results;

        public Player()
        {
            score = 0;
            name = "Unknown";
            number = -1;
            earthCredit = 0;
            desired_endturn = 30;
            home_planet = -1;
            messages = new();
            results = new();
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

        public int InitScans()
        {
            scans = galaxy!.NumberResearchPlanetsOwned(number) + 1;
            return scans;
        }

        public void InitialiseTurn()
        {
            messages = new();
            InitScans();
        }

        public IEnumerable<Command> ParseCommandStrings(List<string> aCommands)
        {
            foreach (string command in aCommands)
            {
                Command cmd;
                try
                {
                    cmd = new(command, number);
                }
                catch
                {
                    messages.Add($"{command} - Command not understood error");
                    continue;
                }
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
            results = new();
            results.Add(cmd.cmdstr.ToUpper());
            switch (cmd.priority)
            {
                case CommandOrder.SCAN:
                    Cmd_Scan(cmd);
                    break;
                case CommandOrder.NAME_SHIP:
                    Cmd_NameShip(cmd);
                    break;
                case CommandOrder.LOAD_ALL:
                    Cmd_LoadAll(cmd);
                    break;
                case CommandOrder.LOAD_ORE:
                    Cmd_LoadOre(cmd);
                    break;
                case CommandOrder.LOAD_IND:
                    Cmd_LoadIndustry(cmd);
                    break;
                case CommandOrder.LOAD_MINE:
                    Cmd_LoadMine(cmd);
                    break;
                case CommandOrder.LOAD_SPCM:
                    Cmd_LoadSpacemine(cmd);
                    break;
                case CommandOrder.LOAD_PDU:
                    Cmd_LoadPDU(cmd);
                    break;
                case CommandOrder.UNLOAD_PDU:
                    Cmd_UnloadPDU(cmd);
                    break;
                case CommandOrder.UNLOAD_ALL:
                    Cmd_UnloadAll(cmd);
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
                case CommandOrder.BUILD_MINE:
                    Cmd_BuildMine(cmd);
                    break;
                case CommandOrder.BUILD_CARGO:
                    Cmd_BuildCargo(cmd);
                    break;
                case CommandOrder.BUILD_FIGHTER:
                    Cmd_BuildFighter(cmd);
                    break;
                case CommandOrder.BUILD_TRACTOR:
                    Cmd_BuildTractor(cmd);
                    break;
                case CommandOrder.BUILD_SHIELD:
                    Cmd_BuildShield(cmd);
                    break;
                case CommandOrder.ATTACK_SHIP:
                    Cmd_Ship_Attack_Ship(cmd);
                    break;
                case CommandOrder.ATTACK_PDU:
                    Cmd_Ship_Attack_PDU(cmd);
                    break;
                case CommandOrder.ATTACK_IND:
                    Cmd_Ship_Attack_Industry(cmd);
                    break;
                case CommandOrder.ATTACK_MINE:
                    Cmd_Ship_Attack_Mine(cmd);
                    break;
                case CommandOrder.ATTACK_SPCM:
                    Cmd_Ship_Attack_Spacmine(cmd);
                    break;
                case CommandOrder.ATTACK_ORE:
                    Cmd_Ship_Attack_Ore(cmd);
                    break;
                default:
                    Console.WriteLine($"Command not implemented {cmd.cmdstr}");
                    break;
            }
            messages.Add(String.Join(": ", results));
        }

        public void Cmd_Ship_Attack_Ship(Command cmd) {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Ship victim = galaxy!.ships[cmd.numbers["victim"]];

            if (!CheckShipOwnership(ship, cmd))
                return;
            if (ship.planet != victim.planet)
            {
                results.Add($"Not over same planet as {victim.DisplayNumber()}");
                return;
            }
            int fight = CheckShotsLeft(ship, cmd.numbers["amount"]);
            int shots = ship.Shots(fight);

            ship.FireShots(fight);
            results.Add($"Fired {shots} at {victim.DisplayNumber()}");
            galaxy.players[victim.owner].messages.Add($"{ship.DisplayNumber()} attacked your ship {victim.DisplayNumber()} with {shots} shots");
            victim.SufferShots(shots);
        }

        public void Cmd_Ship_Attack_PDU(Command cmd) {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy!.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            int fight = CheckShotsLeft(ship, cmd.numbers["amount"]);
            int shots = ship.Shots(fight);

            ship.FireShots(fight);
            int received = plan.PDUAttack(ship.number);
            int destroyed = Math.Min(shots / 3, plan.pdu);
            plan.pdu -= destroyed;
            results.Add($"Fired {shots} at PDU, destroying {destroyed}; receiving {received} hits in return");
            galaxy.players[plan.owner].messages.Add($"{ship.DisplayNumber()} fired on your PDUs around Planet {plan.DisplayNumber()} destroying {destroyed}; receiving {received} hits in return");
        }

        public void Cmd_Ship_Attack_Industry(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd))
                return;
            int fight = CheckShotsLeft(ship, cmd.numbers["amount"]);
            int shots = ship.Shots(fight);

            ship.FireShots(fight);
            int destroyed = Math.Min(shots / 10, plan.industry);
            galaxy.players[plan.owner].messages.Add($"{ship.DisplayNumber()} fired on your Industry on {plan.DisplayNumber()} destroying {destroyed}");
            results.Add($"Fired {shots} at Industry destoying {destroyed} of them");
            plan.industry -= destroyed;
        }

        public void Cmd_Ship_Attack_Mine(Command cmd) {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy!.planets[ship.planet];
            int oreType = cmd.numbers["oretype"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            int fight = CheckShotsLeft(ship, cmd.numbers["amount"]);
            int shots = ship.Shots(fight);

            ship.FireShots(fight);
            int destroyed = Math.Min(shots / 10, plan.mine[oreType]);
            int ore_destroyed = shots - (destroyed*10);

      galaxy.players[plan.owner].messages.Add($"{ship.DisplayNumber()} fired on your Mines R{oreType} on {plan.DisplayNumber()} destroying {destroyed} and {ore_destroyed} ore");
            results.Add($"Fired {shots} at Mines R{oreType} destroying {destroyed} of them and {ore_destroyed} ore");
            plan.mine[oreType] -= destroyed;
            plan.ore[oreType] -= ore_destroyed;
        }

        public void Cmd_Ship_Attack_Spacmine(Command cmd) {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd))
                return;
            int fight = CheckShotsLeft(ship, cmd.numbers["amount"]);
            int shots = ship.Shots(fight);

            ship.FireShots(fight);
            int destroyed = Math.Min(shots, plan.deployed);
            results.Add($"Fired {shots} at Spacemines destroyed {destroyed} of them");
            galaxy.players[plan.owner].messages.Add($"{ship.DisplayNumber()} fired on your spacemines on {plan.DisplayNumber()} destroying {destroyed}");
            plan.deployed -= destroyed;
        }

        public void Cmd_Ship_Attack_Ore(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy!.planets[ship.planet];
            int oreType = cmd.numbers["oretype"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            int fight = CheckShotsLeft(ship, cmd.numbers["amount"]);
            int shots = ship.Shots(fight);

            ship.FireShots(fight);
            int destroyed = Math.Min(shots, plan.ore[oreType]);

            galaxy.players[plan.owner].messages.Add($"{ship.DisplayNumber()} fired on your Ore R{oreType} on {plan.DisplayNumber()} destroying {destroyed} ore");
            results.Add($"Fired {shots} at Ore R{oreType} destroying {destroyed} of them");

            plan.ore[oreType] -= destroyed;
        }

        public void Cmd_BuildCargo(Command cmd) {
            // Build Cargo units on a ship
            // A cargo takes 1 industry and one ore type 1
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 1);
            amount = CheckOre(amount, plan, 1, 1);

            plan.indleft -= amount;
            plan.ore[1] -= amount;
            ship.cargo += amount;
            results.Add($"Built {amount} cargo");
        }

        public void Cmd_BuildFighter(Command cmd)
            // Build Fighter units on a ship
            // A fighter takes 2 industry and one each of ore 2 and 3
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 2);
            amount = CheckOre(amount, plan, 1, 2);
            amount = CheckOre(amount, plan, 1, 3);

            plan.indleft -= amount * 2;
            plan.ore[2] -= amount;
            plan.ore[3] -= amount;
            ship.fighter += amount;
            results.Add($"Built {amount} fighter");
        }

        public void Cmd_BuildTractor(Command cmd) {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 2);
            amount = CheckOre(amount, plan, 2, 7);

            plan.indleft -= amount * 2;
            plan.ore[7] -= amount * 2;
            ship.tractor += amount;
            results.Add($"Built {amount} tractor");
        }

        public void Cmd_BuildShield(Command cmd) {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 2);
            amount = CheckOre(amount, plan, 1, 5);
            amount = CheckOre(amount, plan, 1, 6);

            plan.indleft -= amount * 2;
            plan.ore[5] -= amount;
            plan.ore[6] -= amount;

            ship.shield += amount;
            results.Add($"Built {amount} shields");
        }

        public int CheckIndustry(int amount, Planet plan, int scale)
        {
            if (plan.indleft < amount * scale)
            {
                amount = plan.indleft / scale;
                results.Add("Insufficient Industry");
            }
            return amount;
        }

        public int CheckOre(int amount, Planet plan, int scale, int oretype)
        {
            if (plan.ore[oretype] < amount * scale)
            {
                amount = plan.ore[oretype] / scale;
                results.Add($"Insufficient Ore {oretype}");
            }
            return amount;
        }

        public void Cmd_BuildMine(Command cmd)
        {
            Planet plan = galaxy!.planets[cmd.numbers["planet"]];
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            int oretype = cmd.numbers["oretype"];
            amount = CheckIndustry(amount, plan, 10);
            amount = CheckOre(amount, plan, 5, 8);
            amount = CheckOre(amount, plan, 5, 9);
    
            plan.indleft -= amount * 10;
            plan.ore[8] -= amount * 5;
            plan.ore[9] -= amount * 5;
            plan.mine[oretype] += amount;
            results.Add($"Built {amount} mine type {oretype}");
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
                results.Add($"Unknown player {recip}");
                return;
            }
            ship.owner = new_owner;
            results.Add("OK");
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
                results.Add($"Unknown player {recip}");
                return;
            }
            plan.owner = new_owner;
            results.Add("OK");
        }

        private void Cmd_Jump1(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            int dest1 = cmd.numbers["jump1"];
            int distance = 1;

            if (!JumpChecks(ship, cmd, distance))
                return;
            ship.UseFuel(distance);

            if (!CheckDest(ship, dest1, cmd))
                return;
            ship.MoveTo(dest1);
            results.Add($"OK - Used {ship.FuelRequired(distance)} Fuel");
        }

        private void Cmd_Jump2(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            int distance = 2;

            if (!JumpChecks(ship, cmd, distance))
                return;
            ship.UseFuel(distance);

            int dest1 = cmd.numbers["jump1"];
            if (!CheckDest(ship, dest1, cmd))
                return;
            ship.MoveTo(dest1, true);
            int dest2 = cmd.numbers["jump2"];
            if (!CheckDest(ship, dest2, cmd))
                return;
            ship.MoveTo(dest2);
            results.Add($"OK - Used {ship.FuelRequired(distance)} Fuel");
        }

        private void Cmd_Jump3(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            int distance = 3;

            if (!JumpChecks(ship, cmd, distance))
                return;
            ship.UseFuel(distance);

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
            results.Add($"OK - Used {ship.FuelRequired(distance)} Fuel");
        }

        private void Cmd_Jump4(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            int distance = 4;

            if (!JumpChecks(ship, cmd, distance))
                return;
            ship.UseFuel(distance);

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
            results.Add($"OK - Used {ship.FuelRequired(distance)} Fuel");
        }

        private void Cmd_Jump5(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            int distance = 5;

            if (!JumpChecks(ship, cmd, distance))
                return;
            ship.UseFuel(distance);

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
            results.Add($"OK - Used {ship.FuelRequired(distance)} Fuel");
        }

        private bool JumpChecks(Ship ship, Command cmd, int jumplength)
        // Standard checks for every jump
        {
            if (!CheckShipOwnership(ship, cmd))
                return false;
            if (!CheckShipMoved(ship, cmd))
                return false;
            if (!CheckFuel(ship, jumplength))
                return false;
            return true;
        }

        private bool CheckFuel(Ship aShip, int distance) {
            int required = aShip.FuelRequired(distance);
            if (aShip.carrying["0"] < required)
            {
                results.Add($"Insufficient fuel ({required} required)");
                return false;
            }
            return true;
        }

        private bool CheckDest(Ship aShip, int dest, Command cmd)
        {
            if (!aShip.CheckDest(dest))
            {
                results.Add("Invalid destination");
                return false;
            }
            return true;
        }

        private bool CheckShipMoved(Ship aShip, Command cmd)
        {
            // Check for the ship having moved this turn
            if (!aShip.HasMoved())
                return true;
            results.Add("Already moved this turn");
            return false;
        }

        private bool CheckShipEngaged(Ship aShip, Command cmd)
        {
            if (aShip.IsEngaged())
                return true;
            results.Add("Engaged by a tractor beam");
            return false;
        }

        private void Cmd_LoadAll(Command cmd) {
            // Load all available ore (Except type 0) onto ship
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;

            results.Add("Loaded");
            for (int oretype=1;oretype<numOreTypes;oretype++)
            {
                int amount = planet.ore[oretype];
                if (ship.CargoLeft() < amount)
                    amount = ship.CargoLeft();
                planet.ore[oretype] -= amount;
                ship.LoadShip($"{oretype}", amount);
                if(amount != 0) 
                    results.Add($"{amount} x R{oretype}");
            }
            results.Add("OK");
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
            amount = ship.LoadShip($"{oretype}", amount);
            planet.ore[oretype] -= amount;
            results.Add($"Loaded {amount} R{oretype}");
        }

        private void Cmd_LoadIndustry(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
            // TODO
        }

        private void Cmd_LoadMine(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];


            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
            // TODO
        }
        private void Cmd_LoadSpacemine(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship,cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
            // TODO
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
            results.Add($"Loaded {amount} PDU");
        }

        public void Cmd_UnloadAll(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd))
                return;
            int amount;
            results.Add($"Unloaded");

            for (int oreType = 1; oreType < numOreTypes; oreType++) {
                amount = ship.carrying[$"{oreType}"];
                if (amount == 0)
                    continue;
                amount = ship.UnloadShip($"{oreType}", amount);
                planet.ore[oreType] += amount;
                results.Add($"{amount} x R{oreType}");
            }
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
            results.Add($"Unloaded {amount} PDU");
        }

        private void Cmd_NameShip(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            if (!CheckShipOwnership(ship, cmd))
                return;
            ship.name = cmd.strings["name"];
            results.Add("OK");
        }

        private void Cmd_Scan(Command cmd)
        {
            if (scans > 0)
            {
                Planet plan = galaxy!.planets[cmd.numbers["planet"]];
                plan.Scan(cmd.plrNum);
                results.Add("OK");
                scans--;
            }
            else
            {
                results.Add("No more scans");
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
            results.Add($"You do not own ship {aShip.DisplayNumber()}");
            return false;
        }

        private int CheckShotsLeft(Ship aShip, int amount)
        {
            if (amount>aShip.ShotsLeft())
            {
                results.Add("Insufficient fighter units");
                amount = aShip.ShotsLeft();
            }
            return amount;
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
            results.Add($"You do not own planet {aPlanet.DisplayNumber()}");
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
            foreach (string str in messages)
            {
                Console.WriteLine(str);
            }
        }
    }
}