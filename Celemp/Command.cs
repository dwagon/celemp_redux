using System;
using System.Numerics;
using static Celemp.Constants;

namespace Celemp
{
    public class Command : IComparable<Command>
    {
        public CommandOrder priority = CommandOrder.NOOPERAT;
        public Dictionary<string, int> numbers = new();
        public Dictionary<string, string> strings = new();
        public string cmdstr = new("");
        public int plrNum = 0;

        public Command(string rawcmd, int aPlrNum, bool special = false)
        {
            cmdstr = rawcmd;
            plrNum = aPlrNum;
            if (rawcmd.Trim() == "")
            {
                priority = CommandOrder.NOOPERAT;
                return;
            }
            if (special)    // Internal instruction
            {
                switch (cmdstr)
                {
                    case resolve_attack:
                        priority = CommandOrder.RESOLVE_ATTACK;
                        break;
                    case end_contracting:
                        priority = CommandOrder.RESOLVE_CONTRACTS;
                        break;
                    default:
                        throw new CommandParseException($"Unknown special command {rawcmd}");
                }
                return;
            }

            char firstchar = rawcmd[0];

            switch (Char.ToLower(firstchar))
            {
                case ' ':
                    break;
                case ';':
                    break;
                case '+':
                case '-':
                    ChangeAlliance(rawcmd);
                    break;
                case '{':
                    Broadcast(rawcmd);
                    break;
                case '&':
                    PersonalMessage(rawcmd);
                    break;
                case '(':
                    AllMessage(rawcmd);
                    break;
                case 'o':
                    SetStandingOrder(rawcmd);
                    break;
                case 'x':
                    ClearStandingOrder(rawcmd);
                    break;
                case 's':
                    if (rawcmd[1] == '1' || rawcmd[1] == '2' || rawcmd[1] == '3')
                        Ship_Order(rawcmd);
                    else if (rawcmd.Substring(0, 4).ToLower() == "scan")
                        Scan(rawcmd);
                    else
                        throw new CommandParseException($"Unknown command {rawcmd}");
                    break;
                case 't':
                    GameLength(rawcmd);
                    break;
                case '1':
                case '2':
                case '3':
                    Planet_Order(rawcmd);
                    break;
                default:
                    throw new CommandParseException($"Unknown command {rawcmd}");
            }
        }

        int IComparable<Command>.CompareTo(Command? other)
        {
            if (other is null)
                return 1;
            return this.priority.CompareTo(other.priority);
        }

        private void ChangeAlliance(string cmd)
        {
            throw new CommandParseException($"Unimplemented order {cmd}");
        }

        private void Broadcast(string cmd)
        {
            throw new CommandParseException($"Unimplemented order {cmd}");
        }

        private void PersonalMessage(string cmd)
        {
            throw new CommandParseException($"Unimplemented order {cmd}");
        }

        private void AllMessage(string cmd)
        {
            throw new CommandParseException($"Unimplemented order {cmd}");
        }

        private void SetStandingOrder(string cmd)
        //OS123J123
        {
            priority = CommandOrder.STANDING_ORDER;
            strings.Add("command", cmd.Substring(1));
            if (Char.ToLower(cmd[1]) == 's')
            {
                numbers.Add("ship", ParseShip(cmd.Substring(1, 4)));
                strings.Add("order", "setship");
            }
            else
            {
                numbers.Add("planet", ParsePlanet(cmd.Substring(1, 3)));
                strings.Add("order", "setplanet");
            }
        }

        private void ClearStandingOrder(string cmd)
        //XS123
        {
            priority = CommandOrder.STANDING_ORDER;
            if (cmd.Length == 5)
            {
                int ship = ParseShip(cmd.Substring(1, 4));
                numbers.Add("ship", ship);
                strings.Add("order", "clearship");
            }
            else
            {
                int planet = ParsePlanet(cmd.Substring(1, 3));
                numbers.Add("planet", planet);
                strings.Add("order", "clearplanet");
            }
        }

        private void Ship_Order(string cmd)
        {
            int ship = ParseShip(cmd.Substring(0, 4));
            numbers.Add("ship", ship);
            if (cmd.Length <= 4)
                throw new CommandParseException($"Ship command not understood {cmd}");
            char cmdchar = Char.ToLower(cmd[4]);
            switch (cmdchar)
            {
                case 'a':
                    ShipAttack(cmd, ship);
                    break;
                case 'b':
                    ShipBuildContract(cmd);
                    break;
                case 'd':
                    ShipDeploy(cmd, ship);
                    break;
                case 'e':
                    ShipEngageTractor(cmd);
                    break;
                case 'g':
                    ShipGift(cmd, ship);
                    break;
                case 'j':
                    ShipJump(cmd, ship);
                    break;
                case 'l':
                    ShipLoad(cmd, ship);
                    break;
                case 'p':
                    ShipPurchaseOre(cmd);
                    break;
                case 'r':
                    ShipRetrieve(cmd, ship);
                    break;
                case 't':
                    ShipTend(cmd, ship);
                    break;
                case 'u':
                    ShipUnload(cmd);
                    break;
                case 'x':
                    ShipSellOre(cmd);
                    break;
                case 'z':
                    ShipUnbuild(cmd, ship);
                    break;
                case '=':
                    ShipName(cmd, ship);
                    break;
                default:
                    throw new CommandParseException($"Ship command not understood {cmd}");
            }
        }

        private void ShipAttack(string cmd, int ship)
        {
            // S123AS234 or S123A23S345
            (int amount, int offset) = ExtractAmount(cmd, 5);
            char cmdchar = Char.ToLower(cmd[5 + offset]);
            numbers.Add("amount", amount);

            switch (cmdchar)
            {
                case 's':
                    switch (Char.ToLower(cmd[6 + offset]))
                    {
                        case 'm':
                            ShipAttackSpacemines(cmd);
                            break;
                        default:
                            ShipAttackShip(cmd);
                            break;
                    }
                    break;
                case 'd':
                    ShipAttackPDU(cmd);
                    break;
                case 'm':
                    ShipAttackMines(cmd);
                    break;
                case 'i':
                    ShipAttackIndustry(cmd);
                    break;
                case 'r':
                    ShipAttackOre(cmd);
                    break;
                default:
                    throw new CommandParseException($"Ship attack command not understood {cmd}");
            }
        }

        private void ShipAttackShip(string cmd)
        {
            int victim = ParseShip(cmd.Substring(cmd.Length - 4, 4));
            priority = CommandOrder.ATTACK_SHIP;
            numbers.Add("victim", victim);
        }

        private void ShipAttackPDU(string cmd)
        {
            priority = CommandOrder.ATTACK_PDU;
        }

        private void ShipAttackMines(string cmd)
        {
            int oreType = ParseOretype(cmd[cmd.Length - 1]);

            priority = CommandOrder.ATTACK_MINE;
            numbers.Add("oretype", oreType);
        }

        private void ShipAttackIndustry(string cmd)
        {
            priority = CommandOrder.ATTACK_IND;
        }

        private void ShipAttackOre(string cmd)
        {
            int oreType = ParseOretype(cmd[cmd.Length - 1]);

            priority = CommandOrder.ATTACK_ORE;
            numbers.Add("oretype", oreType);
        }

        private void ShipAttackSpacemines(string cmd)
        {
            priority = CommandOrder.ATTACK_SPCM;
        }

        private void ShipBuildContract(string cmd)
        {
            // S123B10C for build or S123B10C5 for contracting
            (int amount, int offset) = ExtractAmount(cmd, 5);
            char cmdchar = Char.ToLower(cmd[5 + offset]);
            (int bid, int offset2) = ExtractAmount(cmd, 6 + offset);
            numbers.Add("amount", amount);

            if (bid > 0)
            {
                numbers.Add("bid", bid);
                switch (cmdchar)
                {
                    case 'c':
                        priority = CommandOrder.CONTRACT_CARGO;
                        break;
                    case 'f':
                        priority = CommandOrder.CONTRACT_FIGHTER;
                        break;
                    case 't':
                        priority = CommandOrder.CONTRACT_TRACTOR;
                        break;
                    case 's':
                        priority = CommandOrder.CONTRACT_SHIELD;
                        break;
                    default: throw new CommandParseException($"Ship contract command not understood {cmd}");
                }
                return;
            }

            switch (cmdchar)
            {
                case 'c':
                    priority = CommandOrder.BUILD_CARGO;
                    break;
                case 'f':
                    priority = CommandOrder.BUILD_FIGHTER;
                    break;
                case 't':
                    priority = CommandOrder.BUILD_TRACTOR;
                    break;
                case 's':
                    priority = CommandOrder.BUILD_SHIELD;
                    break;
                default: throw new CommandParseException($"Ship build command not understood {cmd}");
            }
        }

        private void ShipDeploy(string cmd, int ship) { }

        private void ShipEngageTractor(string cmd)
        {
            // S123ES234
            int victim = ParseShip(cmd.Substring(cmd.Length - 4, 4));
            priority = CommandOrder.ENGAGE_TRACTOR;
            numbers.Add("victim", victim);
        }

        private void ShipGift(string cmd, int ship)
        {
            // S123GJohn
            priority = CommandOrder.GIFTSHIP;
            strings.Add("recipient", cmd.Substring(5));
        }

        private void ShipJump(string cmd, int ship)
        {
            // S123J120
            switch (cmd.Length)
            {
                case 8:
                    priority = CommandOrder.JUMP1;
                    numbers.Add("jump1", ParsePlanet(cmd.Substring(5)));
                    break;
                case 12:
                    priority = CommandOrder.JUMP2;
                    numbers.Add("jump1", ParsePlanet(cmd.Substring(5, 3)));
                    numbers.Add("jump2", ParsePlanet(cmd.Substring(9, 3)));
                    break;
                case 16:
                    priority = CommandOrder.JUMP3;
                    numbers.Add("jump1", ParsePlanet(cmd.Substring(5, 3)));
                    numbers.Add("jump2", ParsePlanet(cmd.Substring(9, 3)));
                    numbers.Add("jump3", ParsePlanet(cmd.Substring(13, 3)));
                    break;
                case 20:
                    priority = CommandOrder.JUMP4;
                    numbers.Add("jump1", ParsePlanet(cmd.Substring(5, 3)));
                    numbers.Add("jump2", ParsePlanet(cmd.Substring(9, 3)));
                    numbers.Add("jump3", ParsePlanet(cmd.Substring(13, 3)));
                    numbers.Add("jump4", ParsePlanet(cmd.Substring(17, 3)));
                    break;
                case 24:
                    priority = CommandOrder.JUMP5;
                    numbers.Add("jump1", ParsePlanet(cmd.Substring(5, 3)));
                    numbers.Add("jump2", ParsePlanet(cmd.Substring(9, 3)));
                    numbers.Add("jump3", ParsePlanet(cmd.Substring(13, 3)));
                    numbers.Add("jump4", ParsePlanet(cmd.Substring(17, 3)));
                    numbers.Add("jump5", ParsePlanet(cmd.Substring(21, 3)));
                    break;
                default:
                    throw new CommandParseException($"Ship jump command not understood {cmd}");
            }
        }

        private void ShipLoad(string cmd, int ship)
        {
            // S123L = Load all
            char cmdchar;
            if (cmd.Length == 5)
            {
                priority = CommandOrder.LOAD_ALL;
                return;
            }
            try
            {
                (int amount, int offset) = ExtractAmount(cmd, 5);
                numbers.Add("amount", amount);

                cmdchar = Char.ToLower(cmd[5 + offset]);
            }
            catch (System.IndexOutOfRangeException)
            {
                throw new CommandParseException($"Ship load command not understood {cmd}");
            }
            switch (cmdchar)
            {
                case 'm':
                    ShipLoadMine(cmd);
                    break;
                case 'd':
                    ShipLoadPDU(cmd);
                    break;
                case 'i':
                    ShipLoadIndustry(cmd);
                    break;
                case 'r':
                    ShipLoadOre(cmd);
                    break;
                case 's':
                    ShipLoadSpacemines(cmd);
                    break;
                default:
                    throw new CommandParseException($"Ship load command not understood {cmd}");
            }
        }

        public static (int, int) ExtractAmount(string str, int startidx)
        // Pull out the number from the str starting at index
        // Return the number and the number of characters it took
        {
            int amount = -1;
            var amountstr = "";
            for (int idx = startidx; idx < str.Length; idx++)
            {
                if (Char.IsDigit(str[idx]))
                    amountstr += str[idx];
                else
                    break;
            }
            try
            {
                amount = Int16.Parse(amountstr);
            }
            catch (System.FormatException)
            {
                amount = -1;
            }

            return (amount, amountstr.Length);
        }

        private void ShipPurchaseOre(string cmd)
        {
            // S123P5R3
            priority = CommandOrder.BUY_ORE;
            (int amount, int offset) = ExtractAmount(cmd, 5);
            int type = ParseOretype(cmd[cmd.Length - 1]);
            numbers.Add("amount", amount);
            numbers.Add("oretype", type);
        }

        private void ShipRetrieve(string cmd, int ship) { }
        private void ShipTend(string cmd, int ship) { }
        private void ShipUnload(string cmd)
        {
            // S123U
            if (cmd.Length == 5)
            {
                priority = CommandOrder.UNLOAD_ALL;
                return;
            }
            (int amount, int offset) = ExtractAmount(cmd, 5);
            numbers.Add("amount", amount);

            char cmdchar = Char.ToLower(cmd[5 + offset]);
            switch (cmdchar)
            {
                case 'm':
                    ShipUnloadMine(cmd);
                    break;
                case 'd':
                    ShipUnloadPDU(cmd);
                    break;
                case 'i':
                    ShipUnloadIndustry(cmd);
                    break;
                case 'r':
                    ShipUnloadOre(cmd);
                    break;
                case 's':
                    ShipUnloadSpacemines(cmd);
                    break;
                default:
                    throw new CommandParseException($"Ship unload command not understood {cmd}");
            }
        }

        private void ShipUnloadMine(string cmd)
        {
            // S123U23M2 - Unload 23 Mines of type 2
            int type = ParseOretype(cmd[cmd.Length - 1]);
            priority = CommandOrder.UNLOAD_MINE;
            numbers.Add("oretype", type);
        }

        private void ShipUnloadPDU(string cmd)
        {
            // S322U23D
            priority = CommandOrder.UNLOAD_PDU;
        }

        private void ShipUnloadIndustry(string cmd)
        {
            priority = CommandOrder.UNLOAD_IND;
        }

        private void ShipUnloadOre(string cmd)
        {
            int type = ParseOretype(cmd[cmd.Length - 1]);
            numbers.Add("oretype", type);
            priority = CommandOrder.UNLOAD_ORE;
        }
        private void ShipUnloadSpacemines(string cmd)
        {
            priority = CommandOrder.UNLOAD_SPCM;
        }

        private void ShipLoadMine(string cmd)
        {
            // S123L23M2 - Load 23 Mines of type 2
            int type = ParseOretype(cmd[cmd.Length - 1]);
            priority = CommandOrder.LOAD_MINE;
            numbers.Add("oretype", type);
        }

        private void ShipLoadPDU(string cmd)
        {
            // S123L34D
            priority = CommandOrder.LOAD_PDU;
        }

        private void ShipLoadIndustry(string cmd)
        {
            // S345L3I
            priority = CommandOrder.LOAD_IND;
        }

        private void ShipLoadOre(string cmd)
        {
            // S123L120R2
            int type = ParseOretype(cmd[cmd.Length - 1]);

            priority = CommandOrder.LOAD_ORE;
            numbers.Add("oretype", type);
        }

        private void ShipLoadSpacemines(string cmd)
        {
            // S234L34S
            priority = CommandOrder.LOAD_SPCM;
        }

        private void ShipSellOre(string cmd)
        {
            // S123X10R9
            // S123X
            if (cmd.Length == 5)
            {
                priority = CommandOrder.SELL_ALL;
                return;
            }

            int oreType = ParseOretype(cmd[cmd.Length - 1]);
            (int amount, int offset) = ExtractAmount(cmd, 5);
            priority = CommandOrder.SELL_ORE;
            numbers.Add("oretype", oreType);
            numbers.Add("amount", amount);
        }

        private void ShipUnbuild(string cmd, int ship) { }

        private void ShipName(string cmd, int ship)
        {
            // S123=BLACKGUARD
            priority = CommandOrder.NAME_SHIP;
            strings.Add("name", cmd.Substring(5));
        }

        private void Scan(string cmd)
        {
            // SCAN123
            priority = CommandOrder.SCAN;
            numbers.Add("planet", ParsePlanet(cmd.Substring(4)));
        }

        private void GameLength(string cmd)
        {
            Console.WriteLine($"Unimplemented order {cmd}");
        }

        private void Planet_Order(string cmd)
        {
            int planet = ParsePlanet(cmd.Substring(0, 3));
            numbers.Add("planet", planet);
            char cmdchar = Char.ToLower(cmd[3]);
            switch (cmdchar)
            {
                case 'g':
                    PlanetGift(cmd);
                    break;
                case '=':
                    PlanetName(cmd);
                    break;
                case 'a':
                    PlanetAttack(cmd);
                    break;
                case 'b':
                    PlanetBuild(cmd);
                    break;
                case 'x':
                    PlanetTrans(cmd);
                    break;
                case 'd':
                    PlanetDeploy(cmd);
                    break;
                default:
                    throw new CommandParseException($"Planet command not understood {cmd}");
            }
        }

        private void PlanetGift(string cmd)
        {
            // 123GJohn
            priority = CommandOrder.GIFTPLAN;
            strings.Add("recipient", cmd.Substring(4));
        }

        private void PlanetName(string cmd)
        {
            // 123=Arakis
            priority = CommandOrder.NAMEPLAN;
            strings.Add("name", cmd.Substring(4));
        }

        private void PlanetAttack(string cmd)
        {
            // 123AS123
            priority = CommandOrder.PLANET_ATTACK_SHIP;
            (int amount, int offset) = ExtractAmount(cmd, 4);
            numbers.Add("amount", amount);
            int victim = ParseShip(cmd.Substring(cmd.Length - 4, 4));
            numbers.Add("victim", victim);
        }

        private void PlanetBuild(string cmd)
        {
            (int amount, int offset) = ExtractAmount(cmd, 4);
            numbers.Add("amount", amount);

            char cmdchar = Char.ToLower(cmd[4 + offset]);
            switch (cmdchar)
            {
                case 'd':
                    PlanetBuildPDU(cmd);
                    break;
                case 'm':
                    PlanetBuildMine(cmd);
                    break;
                case 'i':
                    PlanetBuildIndustry(cmd);
                    break;
                case 's':
                    PlanetBuildSpacemine(cmd);
                    break;
                case 'h':
                    PlanetBuildHyperdrive(cmd);
                    break;
                default:
                    throw new CommandParseException($"Planet build command not understood {cmd}");
            }
        }

        private void PlanetBuildPDU(string cmd)
        {
            // 123B23D
            priority = CommandOrder.BUILD_PDU;
        }

        private void PlanetBuildMine(string cmd)
        {
            // 235B5M8 - Build 5 mines of ore type 8 on planet 235
            priority = CommandOrder.BUILD_MINE;
            int type = ParseOretype(cmd[cmd.Length - 1]);
            numbers.Add("oretype", type);
        }

        private void PlanetBuildIndustry(string cmd)
        {
            priority = CommandOrder.BUILD_IND;
        }

        private void PlanetBuildSpacemine(string cmd)
        {
            priority = CommandOrder.BUILD_SPACEMINE;
            int type = ParseOretype(cmd[cmd.Length - 1]);
            numbers.Add("oretype", type);
        }

        private void PlanetBuildHyperdrive(string cmd)
        {
            // xxxByyHff/cc/tt/ss
            priority = CommandOrder.BUILD_HYPER;
            try
            {
                int idx = cmd.ToLower().IndexOf('h');
                string[] bits = cmd.Substring(idx + 1).Split('/');
                numbers.Add("fighter", Int16.Parse(bits[0]));
                numbers.Add("cargo", Int16.Parse(bits[1]));
                numbers.Add("tractor", Int16.Parse(bits[2]));
                numbers.Add("shield", Int16.Parse(bits[3]));
            }
            catch (Exception exc)
            {
                throw new CommandParseException($"Hyperdrive build command not understood {cmd}: {exc}");
            }
        }

        private void PlanetTrans(string cmd) { }
        private void PlanetDeploy(string cmd) { }


        private int ParseShip(string shp)
        // Return ship number from a string like "S123"
        {
            int num;
            bool good = Int32.TryParse(shp.Substring(1), out num);

            if (Char.ToLower(shp[0]) != 's')
            {
                throw new CommandParseException($"Ship {shp} doesn't begin with an 'S'");
            }
            if (!good)
            {
                throw new CommandParseException($"Ship {shp} not understandable");
            }

            if (num < 100 || num > 356)
            {
                throw new CommandParseException($"Ship {shp} not a good ship number");
            }
            return num - 100;
        }

        private int ParseOretype(char ot)
        {
            int type = (int)Char.GetNumericValue(ot);
            if (type < 0)
                throw new CommandParseException($"Oretype {ot} not understandable");
            return type;
        }

        private int ParsePlanet(string plan)
        // Return a planet number from a string like "123"
        {
            int num;
            bool good = Int32.TryParse(plan, out num);

            if (!good)
            {
                throw new CommandParseException($"Planet {plan} not understandable");
            }

            if (num < 100 || num > 356)
            {
                throw new CommandParseException($"Planet {plan} not a good planet number");
            }
            return num - 100;
        }
    }
}

