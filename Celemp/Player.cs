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
                    messages.Add($"{command} - Command not understood error: {exc.Message}");
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
                default:
                    Console.WriteLine($"Command not implemented {cmd.cmdstr}");
                    break;
            }
            messages.Add(String.Join(": ", results));
        }

        public void Earth_Build_Cargo(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            int amount = cmd.numbers["amount"];
            results = new()
            {
                cmd.cmdstr.ToUpper()
            };

            amount = CheckShipOre(amount, ship, 1, 1);
            amount = CheckIndustry(amount, plan, 1);

            plan.ind_left -= amount;
            ship.carrying["1"] -= amount;
            ship.cargo += amount;
            int cost = amount * cmd.numbers["bid"];
            SpendEarthCredit(cost);
            results.Add($"Built {amount} cargo units at {cmd.numbers["bid"]} for {cost}");
            messages.Add(String.Join(": ", results));
        }

        public void Earth_Build_Fighter(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            results = new()
            {
                cmd.cmdstr.ToUpper()
            };

            int amount = cmd.numbers["amount"];
            amount = CheckShipOre(amount, ship, 1, 2);
            amount = CheckShipOre(amount, ship, 1, 3);
            amount = CheckIndustry(amount, plan, 2);

            plan.ind_left -= amount * 2;
            ship.carrying["2"] -= amount;
            ship.carrying["3"] -= amount;
            ship.fighter += amount;
            int cost = amount * cmd.numbers["bid"];
            SpendEarthCredit(cost);
            results.Add($"Built {amount} fighter units at {cmd.numbers["bid"]} for {cost}");
            messages.Add(String.Join(": ", results));
        }

        public void SpendEarthCredit(int cost)
        {
            earthCredit -= cost;
            if (earthCredit < 0)
            {
                int debt = Math.Abs(earthCredit);
                score -= debt * galaxy!.earthMult;
                earthCredit += debt;
            }
        }

        public void Earth_Build_Shield(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            results = new()
            {
                cmd.cmdstr.ToUpper()
            };

            int amount = cmd.numbers["amount"];
            amount = CheckShipOre(amount, ship, 1, 5);
            amount = CheckShipOre(amount, ship, 1, 6);
            amount = CheckIndustry(amount, plan, 2);

            plan.ind_left -= amount * 2;
            ship.carrying["5"] -= amount;
            ship.carrying["6"] -= amount;

            ship.shield += amount;
            int cost = amount * cmd.numbers["bid"];
            SpendEarthCredit(cost);
            results.Add($"Built {amount} shield units at {cmd.numbers["bid"]} for {cost}");
            messages.Add(String.Join(": ", results));
        }

        public void Earth_Build_Tractor(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            results = new()
            {
                cmd.cmdstr.ToUpper()
            };

            int amount = cmd.numbers["amount"];
            amount = CheckShipOre(amount, ship, 2, 7);
            amount = CheckIndustry(amount, plan, 2);

            plan.ind_left -= amount * 2;
            ship.carrying["7"] -= amount;
            ship.tractor += amount;
            int cost = amount * cmd.numbers["bid"];
            SpendEarthCredit(cost);
            results.Add($"Built {amount} tractor units at {cmd.numbers["bid"]} for {cost}");
            messages.Add(String.Join(": ", results));
        }

        // Contracts are weird as they get resolved in contractor order
        // So we lodge them first, then resolve them later
        public void Cmd_Contract_Cargo(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            galaxy!.AddBid("cargo", cmd);
            results.Add("Bid accepted");
        }

        public void Cmd_Contract_Fighter(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            galaxy!.AddBid("fighter", cmd);
            results.Add("Bid accepted");
        }

        public void Cmd_Contract_Shield(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            galaxy!.AddBid("shield", cmd);
            results.Add("Bid accepted");
        }

        public void Cmd_Contract_Tractor(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            galaxy!.AddBid("tractor", cmd);
            results.Add("Bid accepted");
        }

        public void Cmd_EngageTractor(Command cmd)
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

        public void Standing_Order(Command cmd)
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

        public void System_ResolveContracts(Command cmd)
        {
            galaxy!.ResolveContracts("cargo");
            galaxy!.ResolveContracts("fighter");
            galaxy!.ResolveContracts("shield");
            galaxy!.ResolveContracts("tractor");
        }

        public void System_ResolveAttacks(Command cmd)
        // Resolve all the hits to ships
        {
            foreach (KeyValuePair<int, Ship> kvp in galaxy!.ships)
                kvp.Value.ResolveDamage();
        }

        public void Cmd_BuyOre(Command cmd)
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

        public void BuyOre(Ship ship, int oretype, int amount)
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

        public void Cmd_SellOre(Command cmd)
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

        public void Cmd_SellAll(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckEarth(galaxy!.planets[ship.planet]))
                return;
            for (int oreType = 1; oreType < numOreTypes; oreType++)
                SellOre(ship, oreType, ship.carrying[$"{oreType}"]);
        }

        public void SellOre(Ship ship, int oretype, int amount)
        {
            int value = (int)(0.666F * amount * galaxy!.earth_price[oretype]);
            results.Add($"Gained {value} Earth Credits for {amount} x R{oretype}");
            earthCredit += value;
            ship.UnloadShip($"{oretype}", amount);
            galaxy.planets[ship.planet].ore[oretype] += amount;
        }

        public void Cmd_BuildIndustry(Command cmd)
        {
            Planet plan = galaxy!.planets[cmd.numbers["planet"]];
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 10);
            amount = CheckOre(amount, plan, 5, 8);
            amount = CheckOre(amount, plan, 5, 9);

            plan.ind_left -= amount * 10;
            plan.ore[8] -= amount * 5;
            plan.ore[9] -= amount * 5;
            plan.industry += amount;
            results.Add($"Built {amount} industry");
        }

        public void Cmd_BuildSpacemine(Command cmd)
        {
            Planet plan = galaxy!.planets[cmd.numbers["planet"]];
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            int oretype = cmd.numbers["oretype"];
            amount = CheckIndustry(amount, plan, 1);
            amount = CheckOre(amount, plan, 1, oretype);

            plan.ind_left -= amount;
            plan.ore[oretype] -= amount;
            plan.spacemines += amount;
            results.Add($"Built {amount} spacemines");
        }

        public void Cmd_BuildPDU(Command cmd)
        {
            Planet plan = galaxy!.planets[cmd.numbers["planet"]];
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 1);
            amount = CheckOre(amount, plan, 1, 4);

            plan.ind_left -= amount;
            plan.ore[4] -= amount;
            plan.pdu += amount;
            results.Add($"Built {amount} PDU");
        }

        public void Cmd_BuildCargo(Command cmd)
        {
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

            plan.ind_left -= amount;
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

            plan.ind_left -= amount * 2;
            plan.ore[2] -= amount;
            plan.ore[3] -= amount;
            ship.fighter += amount;
            results.Add($"Built {amount} fighter");
        }

        public void Cmd_BuildTractor(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 2);
            amount = CheckOre(amount, plan, 2, 7);

            plan.ind_left -= amount * 2;
            plan.ore[7] -= amount * 2;
            ship.tractor += amount;
            results.Add($"Built {amount} tractor");
        }

        public void Cmd_BuildShield(Command cmd)
        {
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

            plan.ind_left -= amount * 2;
            plan.ore[5] -= amount;
            plan.ore[6] -= amount;

            ship.shield += amount;
            results.Add($"Built {amount} shields");
        }

        public int CheckIndustry(int amount, Planet plan, int scale)
        {
            if (plan.ind_left < amount * scale)
            {
                amount = plan.ind_left / scale;
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

        public int CheckShipOre(int amount, Ship ship, int scale, int oretype)
        // Check amount of ore on the ship (for Earth contractor)
        {
            if (ship.carrying[$"{oretype}"] < amount * scale)
            {
                amount = ship.carrying[$"{oretype}"] / scale;
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

            plan.ind_left -= amount * 10;
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
            int new_owner = galaxy.GuessPlayerName(recip);
            if (new_owner < 0)
            {
                results.Add($"Unknown player {recip}");
                return;
            }
            galaxy.players[new_owner].messages.Add($"{galaxy.players[ship.owner].name} gave you {ship.DisplayNumber()}");
            ship.owner = new_owner;
            ship.stndord = "";
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
            galaxy.players[new_owner].messages.Add($"{galaxy.players[plan.owner].name} gave you {plan.DisplayNumber()}");
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
            foreach (string str in messages)
            {
                Console.WriteLine(str);
            }
        }
    }
}