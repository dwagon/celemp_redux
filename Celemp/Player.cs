using System.Linq;
using System.Numerics;
using static System.Formats.Asn1.AsnWriter;
using static Celemp.Constants;

namespace Celemp
{
    [Serializable]
    public partial class Player
    {
        public int score { get; set; }
        public String name { get; set; }
        public int number { get; set; }
        public int earthCredit { get; set; }
        public int desired_endturn { get; set; }
        public int home_planet { get; set; }
        public List<string> cmd_results { get; set; }
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
            cmd_results = new();
            messages = new();
            results = new();
        }

        public Player(String name) : this()
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
            cmd_results = new();
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
                catch (CommandParseException exc)
                {
                    cmd_results.Add($"{command} - Command not understood error: {exc.Message}");
                    continue;
                }
                catch
                {
                    Console.WriteLine($"Exception parsing {command} for player {number}");
                    throw;
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
            for (int planNum = 0; planNum < numPlanets; planNum++)
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

        public void ProcessCommand(Command cmd)
        {
            results = new()
            {
                cmd.cmdstr.ToUpper()
            };
            switch (cmd.priority)
            {
                case CommandOrder.NOOPERAT:
                    break;
                case CommandOrder.SCAN:
                    Cmd_Scan(cmd);
                    break;
                case CommandOrder.NAME_SHIP:
                    Cmd_NameShip(cmd);
                    break;
                case CommandOrder.NAME_PLANET:
                    Cmd_NamePlanet(cmd);
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
                case CommandOrder.UNLOAD_ORE:
                    Cmd_UnloadOre(cmd);
                    break;
                case CommandOrder.UNLOAD_SPCM:
                    Cmd_UnloadSpacemine(cmd);
                    break;
                case CommandOrder.UNLOAD_IND:
                    Cmd_UnloadIndustry(cmd);
                    break;
                case CommandOrder.UNLOAD_MINE:
                    Cmd_UnloadMine(cmd);
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
                case CommandOrder.PLANET_ATTACK_SHIP:
                    Cmd_Planet_Attack_Ship(cmd);
                    break;
                case CommandOrder.BUILD_SPACEMINE:
                    Cmd_BuildSpacemine(cmd);
                    break;
                case CommandOrder.BUILD_IND:
                    Cmd_BuildIndustry(cmd);
                    break;
                case CommandOrder.BUILD_PDU:
                    Cmd_BuildPDU(cmd);
                    break;
                case CommandOrder.SELL_ORE:
                    Cmd_SellOre(cmd);
                    break;
                case CommandOrder.SELL_ALL:
                    Cmd_SellAll(cmd);
                    break;
                case CommandOrder.BUY_ORE:
                    Cmd_BuyOre(cmd);
                    break;
                case CommandOrder.RESOLVE_ATTACK:
                    System_ResolveAttacks(cmd);
                    break;
                case CommandOrder.RESOLVE_CONTRACTS:
                    System_ResolveContracts(cmd);
                    break;
                case CommandOrder.STANDING_ORDER:
                    Standing_Order(cmd);
                    break;
                case CommandOrder.ENGAGE_TRACTOR:
                    Cmd_EngageTractor(cmd);
                    break;
                case CommandOrder.CONTRACT_CARGO:
                    Cmd_Contract_Cargo(cmd);
                    break;
                case CommandOrder.CONTRACT_FIGHTER:
                    Cmd_Contract_Fighter(cmd);
                    break;
                case CommandOrder.CONTRACT_SHIELD:
                    Cmd_Contract_Shield(cmd);
                    break;
                case CommandOrder.CONTRACT_TRACTOR:
                    Cmd_Contract_Tractor(cmd);
                    break;
                case CommandOrder.BUILD_HYPER:
                    Cmd_Build_Hyperdrive(cmd);
                    break;
                case CommandOrder.UNBUILD_CARGO:
                    Cmd_Unbuild_Cargo(cmd);
                    break;
                case CommandOrder.UNBUILD_FIGHTER:
                    Cmd_Unbuild_Fighter(cmd);
                    break;
                case CommandOrder.UNBUILD_SHIELD:
                    Cmd_Unbuild_Shield(cmd);
                    break;
                case CommandOrder.UNBUILD_TRACTOR:
                    Cmd_Unbuild_Tractor(cmd);
                    break;
                case CommandOrder.TEND_ALL:
                    Cmd_TendAll(cmd);
                    break;
                case CommandOrder.TEND_INDUSTRY:
                    Cmd_TendIndustry(cmd);
                    break;
                case CommandOrder.TEND_MINE:
                    Cmd_TendMine(cmd);
                    break;
                case CommandOrder.TEND_ORE:
                    Cmd_TendOre(cmd);
                    break;
                case CommandOrder.TEND_PDU:
                    Cmd_TendPDU(cmd);
                    break;
                case CommandOrder.TEND_SPACEMINE:
                    Cmd_TendSpacemine(cmd);
                    break;
                case CommandOrder.BROADCAST:
                    Cmd_Broadcast(cmd);
                    break;
                case CommandOrder.MESSAGE:
                    Cmd_Message(cmd);
                    break;
                default:
                    Console.WriteLine($"Command not implemented {cmd.cmdstr}");
                    break;
            }
            cmd_results.Add(String.Join(": ", results));
        }

        private void SpendEarthCredit(int cost)
        {
            earthCredit -= cost;
            if (earthCredit < 0)
            {
                int debt = Math.Abs(earthCredit);
                score -= debt * galaxy!.earthMult;
                earthCredit += debt;
            }
        }

        private void Cmd_Broadcast(Command cmd)
        {
            for (int plrNum = 0; plrNum < numPlayers; plrNum++)
                try
                {
                    galaxy!.players[plrNum].messages.Add("Broadcast: " + cmd.strings["message"]);
                }
                catch (NullReferenceException)  // Player array not full - in testing
                {
                    Console.WriteLine($"Cmd_Broadcast: Unconstructed player {plrNum}");
                    continue;
                }
            results.Add("OK");
        }

        private void Cmd_Message(Command cmd)
        {
            string recipient = cmd.strings["recipient"];
            int recip = galaxy!.GuessPlayerName(recipient);
            if (recip < 0)
            {
                results.Add($"Unknown player {recipient}");
                return;
            }
            galaxy!.players[recip].messages.Add($"Msg from {name}: " + cmd.strings["message"]);
            results.Add("OK");
        }

        // Contracts are weird as they get resolved in contractor order
        // So we lodge them first, then resolve them later
        private void Cmd_Contract_Cargo(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            galaxy!.AddBid("cargo", cmd);
            results.Add("Bid accepted");
        }

        private void Cmd_Contract_Fighter(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            galaxy!.AddBid("fighter", cmd);
            results.Add("Bid accepted");
        }

        private void Cmd_Contract_Shield(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            galaxy!.AddBid("shield", cmd);
            results.Add("Bid accepted");
        }

        private void Cmd_Contract_Tractor(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            galaxy!.AddBid("tractor", cmd);
            results.Add("Bid accepted");
        }

        private void Cmd_EngageTractor(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Ship victim = galaxy!.ships[cmd.numbers["victim"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (ship.planet != victim.planet)
            {
                results.Add($"Not over same planet as {victim.DisplayNumber()}");
                return;
            }
            if (victim.CalcWeight() > ship.tractor)
            {
                results.Add("Target ship is too heavy");
                return;
            }
            if (CheckShipEngaged(ship, cmd))
                return;
            if (ship.IsEngaging())
            {
                results.Add("Ship is already engaging another");
                return;
            }
            ship.EngageShip(victim);
            results.Add("OK");
        }

        private void Standing_Order(Command cmd)
        {
            Ship ship; Planet plan;
            switch (cmd.strings["order"])
            {
                case "setship":
                    ship = galaxy!.ships[cmd.numbers["ship"]];
                    if (!CheckShipOwnership(ship, cmd))
                        return;
                    ship.stndord = cmd.strings["command"];
                    break;
                case "setplanet":
                    plan = galaxy!.planets[cmd.numbers["planet"]];
                    if (!CheckPlanetOwnership(plan, cmd))
                        return;
                    plan.stndord = cmd.strings["command"];
                    break;
                case "clearship":
                    ship = galaxy!.ships[cmd.numbers["ship"]];
                    if (!CheckShipOwnership(ship, cmd))
                        return;
                    ship.stndord = "";
                    break;
                case "clearplanet":
                    plan = galaxy!.planets[cmd.numbers["planet"]];
                    if (!CheckPlanetOwnership(plan, cmd))
                        return;
                    plan.stndord = "";
                    break;
                default:
                    Console.WriteLine($"Unhandled standing order direction {cmd.strings["direction"]}");
                    break;
            }
            results.Add("OK");
        }

        private void System_ResolveContracts(Command cmd)
        {
            galaxy!.ResolveContracts();
        }

        private void System_ResolveAttacks(Command cmd)
        // Resolve all the hits to ships
        {
            foreach (KeyValuePair<int, Ship> kvp in galaxy!.ships)
                kvp.Value.ResolveDamage();
        }

        private void Cmd_BuyOre(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            int amount = cmd.numbers["amount"];
            int oretype = cmd.numbers["oretype"];
            BuyOre(ship, oretype, amount);
        }

        private void BuyOre(Ship ship, int oretype, int amount)
        {
            int price = galaxy!.earth_price[oretype];

            if (amount > galaxy.planets[ship.planet].ore[oretype])
            {
                amount = galaxy.planets[ship.planet].ore[oretype];
                results.Add("Insufficient Ore");
            }
            amount = CheckCargoLeft(ship, amount, $"{oretype}");
            int value = amount * price;
            results.Add($"Spent {value} Earth Credits for {amount} x R{oretype}");
            SpendEarthCredit(value);
            ship.LoadShip($"{oretype}", amount);
            galaxy.planets[ship.planet].ore[oretype] -= amount;
        }

        private void Cmd_SellOre(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            int amount = cmd.numbers["amount"];
            int oretype = cmd.numbers["oretype"];
            if (amount > ship.carrying[$"{oretype}"])
            {
                amount = ship.carrying[$"{oretype}"];
                results.Add($"Insufficient R{oretype}");
            }
            SellOre(ship, oretype, amount);
        }

        private void Cmd_SellAll(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            for (int oreType = 1; oreType < numOreTypes; oreType++)
                SellOre(ship, oreType, ship.carrying[$"{oreType}"]);
        }

        private void SellOre(Ship ship, int oretype, int amount)
        {
            int value = (int)(0.666F * amount * galaxy!.earth_price[oretype]);
            results.Add($"Gained {value} Earth Credits for {amount} x R{oretype}");
            earthCredit += value;
            ship.UnloadShip($"{oretype}", amount);
            galaxy.planets[ship.planet].ore[oretype] += amount;
        }

        private int CheckIndustry(int amount, Planet plan, int scale)
        {
            if (plan.ind_left < amount * scale)
            {
                amount = plan.ind_left / scale;
                results.Add("Insufficient Industry");
            }
            return amount;
        }

        private int CheckOre(int amount, Planet plan, int scale, int oretype)
        {
            if (plan.ore[oretype] < amount * scale)
            {
                amount = plan.ore[oretype] / scale;
                results.Add($"Insufficient Ore {oretype}");
            }
            return amount;
        }

        private int CheckShipOre(int amount, Ship ship, int scale, int oretype)
        // Check amount of ore on the ship (for Earth contractor)
        {
            if (ship.carrying[$"{oretype}"] < amount * scale)
            {
                amount = ship.carrying[$"{oretype}"] / scale;
                results.Add($"Insufficient Ore {oretype}");
            }
            return amount;
        }

        private void Cmd_GiftShip(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            String recip = cmd.strings["recipient"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            int new_owner = galaxy.GuessPlayerName(recip);
            if (new_owner < 0)
            {
                results.Add($"Unknown player {recip}");
                return;
            }
            galaxy.players[new_owner].cmd_results.Add($"{galaxy.players[ship.owner].name} gave you {ship.DisplayNumber()}");
            ship.owner = new_owner;
            ship.stndord = "";
            results.Add("OK");
        }

        private void Cmd_GiftPlan(Command cmd)
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
            galaxy.players[new_owner].cmd_results.Add($"{galaxy.players[plan.owner].name} gave you {plan.DisplayNumber()}");
            plan.owner = new_owner;
            plan.stndord = "";
            results.Add("OK");
        }

        private void Cmd_Jump1(Command cmd)
        {
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

        private void Cmd_Jump2(Command cmd)
        {
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

        private void Cmd_Jump3(Command cmd)
        {
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

        private void Cmd_Jump4(Command cmd)
        {
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

        private void Cmd_Jump5(Command cmd)
        {
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
            if (CheckShipEngaged(ship, cmd))
                return false;
            return true;
        }

        private bool CheckFuel(Ship aShip, int distance)
        {
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
            {
                results.Add("Engaged by a tractor beam");
                return true;
            }
            return false;
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

        private void Cmd_NamePlanet(Command cmd)
        {
            Planet plan = galaxy!.planets[cmd.numbers["planet"]];

            if (!CheckPlanetOwnership(plan, cmd))
                return;
            plan.name = cmd.strings["name"];
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

        private bool CheckShipOwnership(Ship aShip, Command cmd)
        {
            if (aShip.owner == number)
                return true;
            results.Add($"You do not own ship {aShip.DisplayNumber()}");
            return false;
        }

        private int CheckShotsLeft(Ship aShip, int amount)
        {
            if (amount > aShip.ShotsLeft())
            {
                results.Add("Insufficient fighter units");
                amount = aShip.ShotsLeft();
            }
            return amount;
        }

        private int CheckCargoLeft(Ship aShip, int amount, string cargo_type, bool notify = true)
        {
            if (aShip.CargoLeft() < amount * Ship.CargoScale(cargo_type))
            {
                if (notify)
                    results.Add("Insufficient free cargo units");
                amount = aShip.CargoLeft() / Ship.CargoScale(cargo_type);
            }
            return amount;
        }

        private bool CheckPlanetOwnership(Planet aPlanet, Command cmd)
        {
            if (aPlanet.owner == number)
                return true;
            results.Add($"You do not own planet {aPlanet.DisplayNumber()}");
            return false;
        }

        private bool CheckEarth(Planet aPlanet)
        {
            if (!aPlanet.IsEarth())
            {
                results.Add("You are not orbitting Earth");
                return false;
            }
            return true;
        }

        public void InitPlayer(Galaxy aGalaxy, int aPlrNum)
        {
            galaxy = aGalaxy;
            number = aPlrNum;
            galaxy.players[aPlrNum] = this;
        }

        public void InitPlayer(Galaxy aGalaxy)
        {
            galaxy = aGalaxy;
        }

        public void OutputLog()
        // Dump executed output for debugging purposes
        {
            foreach (string str in cmd_results)
            {
                Console.WriteLine(str);
            }
        }
    }
}