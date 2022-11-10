using System;
using System.Numerics;

namespace Celemp
{
    public class Command: IComparable<Command>
    {
        public CommandOrder priority = CommandOrder.NOOPERAT;
        public Dictionary<string, int> numbers = new();
        public Dictionary<string, string> strings = new();
        public string cmdstr = new("");
        public int plrNum = 0;

        public Command(string rawcmd, int aPlrNum)
        {
            cmdstr = rawcmd;
            plrNum = aPlrNum;
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
                    else if (rawcmd.Substring(0,4).ToLower() == "scan")
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

        private void ChangeAlliance(string cmd) {
            Console.WriteLine($"Unimplemented order {cmd}");

        }

        private void Broadcast(string cmd) {
            Console.WriteLine($"Unimplemented order {cmd}");
        }

        private void PersonalMessage(string cmd) {
            Console.WriteLine($"Unimplemented order {cmd}");
        }

        private void AllMessage(string cmd) {
            Console.WriteLine($"Unimplemented order {cmd}");
        }

        private void SetStandingOrder(string cmd) {
            Console.WriteLine($"Unimplemented order {cmd}");
        }

        private void ClearStandingOrder(string cmd) {
            Console.WriteLine($"Unimplemented order {cmd}");
        }

        private void Ship_Order(string cmd) {
            int ship = ParseShip(cmd.Substring(0, 4));
            char cmdchar = Char.ToLower(cmd[4]);
            switch (cmdchar)
            {
                case 'a':
                    ShipAttack(cmd, ship);
                    break;
                case 'b':
                    ShipBuild(cmd, ship);
                    break;
                case 'd':
                    ShipDeploy(cmd, ship);
                    break;
                case 'e':
                    ShipEngageTractor(cmd, ship);
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
                    ShipPurchaseOre(cmd, ship);
                    break;
                case 'r':
                    ShipRetrieve(cmd, ship);
                    break;
                case 't':
                    ShipTend(cmd, ship);
                    break;
                case 'u':
                    ShipUnload(cmd, ship);
                    break;
                case 'x':
                    ShipSellOre(cmd, ship);
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

        private void ShipAttack(string cmd, int ship) { }

        private void ShipBuild(string cmd, int ship) {
            // S123B10C
            (int amount, int offset) = ExtractAmount(cmd, 5);
            char cmdchar = Char.ToLower(cmd[5 + offset]);
            switch (cmdchar)
            {
                case 'c':
                    ShipBuildCargo(cmd, ship, amount);
                    break;
                case 'f':
                    ShipBuildFighter(cmd, ship, amount);
                    break;
                case 't':
                    ShipBuildTractor(cmd, ship, amount);
                    break;
                case 's':
                    ShipBuildShield(cmd, ship, amount);
                    break;
                default: throw new CommandParseException($"Ship build command not understood {cmd}");
            }
        }

        private void ShipBuildCargo(string cmd, int ship, int amount) {
            priority = CommandOrder.BUILD_CARGO;
            numbers.Add("ship", ship);
            numbers.Add("amount", amount);
        }

        private void ShipBuildFighter(string cmd, int ship, int amount) {
            priority = CommandOrder.BUILD_FIGHTER;
            numbers.Add("ship", ship);
            numbers.Add("amount", amount);
        }

        private void ShipBuildTractor(string cmd, int ship, int amount) {
            priority = CommandOrder.BUILD_TRACTOR;
            numbers.Add("ship", ship);
            numbers.Add("amount", amount);
        }

        private void ShipBuildShield(string cmd, int ship, int amount) {
            priority = CommandOrder.BUILD_SHIELD;
            numbers.Add("ship", ship);
            numbers.Add("amount", amount);
        }

        private void ShipDeploy(string cmd, int ship) { }
        private void ShipEngageTractor(string cmd, int ship) { }

        private void ShipGift(string cmd, int ship) {
            // S123GJohn
            priority = CommandOrder.GIFTSHIP;
            strings.Add("recipient", cmd.Substring(5));
            numbers.Add("ship", ship);
        }

        private void ShipJump(string cmd, int ship) {
            // S123J120
            numbers.Add("ship", ship);
            switch(cmd.Length)
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

        private void ShipLoad(string cmd, int ship) {
            // S123L = Load all
            if (cmd.Length == 5)
            {
                priority = CommandOrder.LOADALL;
                numbers.Add("ship", ship);
                return;
            }
            (int amount, int offset) = ExtractAmount(cmd, 5);

            char cmdchar = Char.ToLower(cmd[5 + offset]);
            switch (cmdchar)
            {
                case 'm':
                    ShipLoadMine(cmd, ship, amount);
                    break;
                case 'd':
                    ShipLoadPDU(cmd, ship, amount);
                    break;
                case 'i':
                    ShipLoadIndustry(cmd, ship, amount);
                    break;
                case 'r':
                    ShipLoadOre(cmd, ship, amount);
                    break;
                case 's':
                    ShipLoadSpacemines(cmd, ship, amount);
                    break;
                default:
                    throw new CommandParseException($"Ship load command not understood {cmd}");
            }

        }

        private (int, int) ExtractAmount(string str, int startidx)
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
            amount = Int16.Parse(amountstr);

            return (amount, amountstr.Length);
        }

        private void ShipPurchaseOre(string cmd, int ship) { }
        private void ShipRetrieve(string cmd, int ship) { }
        private void ShipTend(string cmd, int ship) { }
        private void ShipUnload(string cmd, int ship) {
            // S123U
            if (cmd.Length==5) {
                priority = CommandOrder.UNLODALL;
                numbers.Add("ship", ship);
                return;
            }
            (int amount, int offset) = ExtractAmount(cmd, 5);

            char cmdchar = Char.ToLower(cmd[5 + offset]);
            switch (cmdchar)
            {
                case 'm':
                    ShipUnloadMine(cmd, ship, amount);
                    break;
                case 'd':
                    ShipUnloadPDU(cmd, ship, amount);
                    break;
                case 'i':
                    ShipUnloadIndustry(cmd, ship, amount);
                    break;
                case 'r':
                    ShipUnloadOre(cmd, ship, amount);
                    break;
                case 's':
                    ShipUnloadSpacemines(cmd, ship, amount);
                    break;
                default:
                    throw new CommandParseException($"Ship unload command not understood {cmd}");
            }
        }

        private void ShipUnloadMine(string cmd, int ship, int amount) {
            // S123U23M2 - Unload 23 Mines of type 2
            int type = Convert.ToInt16(cmd[cmd.Length-1]);
            priority = CommandOrder.UNLODMIN;
            numbers.Add("ship", ship);
            numbers.Add("amount", amount);
            strings.Add("cargo", "mine");
            numbers.Add("oretype", type);
        }

        private void ShipUnloadPDU(string cmd, int ship, int amount) {
            // S322U23D
            priority = CommandOrder.UNLOADPDU;
            numbers.Add("amount", amount);
            numbers.Add("ship", ship);
        }

        private void ShipUnloadIndustry(string cmd, int ship, int amount) { }
        private void ShipUnloadOre(string cmd, int ship, int amount) { }
        private void ShipUnloadSpacemines(string cmd, int ship, int amount) { }

        private void ShipLoadMine(string cmd, int ship, int amount)
        {
            // S123L23M2 - Load 23 Mines of type 2
            int type = Convert.ToInt16(cmd[cmd.Length - 1]);
            priority = CommandOrder.LOADMIN;
            numbers.Add("ship", ship);
            numbers.Add("amount", amount);
            numbers.Add("oretype", type);
        }

        private void ShipLoadPDU(string cmd, int ship, int amount) {
            // S123L34D
            priority = CommandOrder.LOADPDU;
            numbers.Add("amount", amount);
            numbers.Add("ship", ship);
        }

        private void ShipLoadIndustry(string cmd, int ship, int amount) {
            // S345L3I
            priority = CommandOrder.LOADIND;
            numbers.Add("amount", amount);
            numbers.Add("ship", ship);
        }

        private void ShipLoadOre(string cmd, int ship, int amount) {
            // S123L120R2
            int type = (int) Char.GetNumericValue(cmd[cmd.Length - 1]);

            priority = CommandOrder.LOADORE;
            numbers.Add("ship", ship);
            numbers.Add("amount", amount);
            numbers.Add("oretype", type);
        }

        private void ShipLoadSpacemines(string cmd, int ship, int amount) {
            // S234L34S
            priority = CommandOrder.LOADSPM;
            numbers.Add("amount", amount);
            numbers.Add("ship", ship);
        }

        private void ShipSellOre(string cmd, int ship) { }
        private void ShipUnbuild(string cmd, int ship) { }

        private void ShipName(string cmd, int ship) {
            // S123=BLACKGUARD
            priority = CommandOrder.NAMESHIP;
            numbers.Add("ship", ParseShip(cmd.Substring(0, 4)));
            strings.Add("name", cmd.Substring(5));
        }

        private void Scan(string cmd) {
            // SCAN123
            priority = CommandOrder.SCAN;
            numbers.Add("planet", ParsePlanet(cmd.Substring(4)));
        }

        private void GameLength(string cmd) {
            Console.WriteLine($"Unimplemented order {cmd}");
        }

        private void Planet_Order(string cmd) {
            int planet = ParsePlanet(cmd.Substring(0, 3));
            char cmdchar = Char.ToLower(cmd[3]);
            switch(cmdchar)
            {
                case 'g':
                    PlanetGift(cmd, planet);
                    break;
                case '=':
                    PlanetName(cmd, planet);
                    break;
                case 'a':
                    PlanetAttack(cmd, planet);
                    break;
                case 'b':
                    PlanetBuild(cmd, planet);
                    break;
                case 'x':
                    PlanetTrans(cmd, planet);
                    break;
                case 'd':
                    PlanetDeploy(cmd, planet);
                    break;
                default:
                    throw new CommandParseException($"Planet command not understood {cmd}");

            }
        }

        private void PlanetGift(string cmd, int planet) {
            // 123GJohn
            priority = CommandOrder.GIFTPLAN;
            strings.Add("recipient", cmd.Substring(4));
            numbers.Add("planet", planet);
        }

        private void PlanetName(string cmd, int planet) {
            // 123=Arakis
            priority = CommandOrder.NAMEPLAN;
            numbers.Add("planet", planet);
            strings.Add("name", cmd.Substring(4));
        }

        private void PlanetAttack(string cmd, int planet) { }

        private void PlanetBuild(string cmd, int planet)
        {
            (int amount, int offset) = ExtractAmount(cmd, 4);
            char cmdchar = Char.ToLower(cmd[4 + offset]);
            switch (cmdchar)
            {
                case 'd':
                    PlanetBuildPDU(cmd, planet, amount);
                    break;
                case 'm':
                    PlanetBuildMine(cmd, planet, amount);
                    break;
                case 'i':
                    PlanetBuildIndustry(cmd, planet, amount);
                    break;
                case 's':
                    PlanetBuildShip(cmd, planet, amount);
                    break;
                case 'h':
                    PlanetBuildHyperdrive(cmd, planet, amount);
                    break;
                default:
                    throw new CommandParseException($"Planet build command not understood {cmd}");
            }
        }

        private void PlanetBuildPDU(string cmd, int planet, int amount) { }

        private void PlanetBuildMine(string cmd, int planet, int amount) {
            // 235B5M8 - Build 5 mines of ore type 8 on planet 235
            priority = CommandOrder.BUILD_MINE;
            int type = (int)Char.GetNumericValue(cmd[cmd.Length - 1]);
            numbers.Add("amount", amount);
            numbers.Add("planet", planet);
            numbers.Add("oretype", type);
        }

        private void PlanetBuildIndustry(string cmd, int planet, int amount) { }
        private void PlanetBuildShip(string cmd, int planet, int amount) { }
        private void PlanetBuildHyperdrive(string cmd, int planet, int amount) { }

        private void PlanetTrans(string cmd, int planet) { }
        private void PlanetDeploy(string cmd, int planet) { }


        private int ParseShip(string shp)
            // Return ship number from a string like "S123"
        {
            int num;
            bool good = Int32.TryParse(shp.Substring(1), out num);

            if (Char.ToLower(shp[0]) != 's') {
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
            return num-100;
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
            return num-100;
        }
    }
}

