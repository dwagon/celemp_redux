using System;
namespace Celemp
{
    public partial class Player
    {
        public void Cmd_Build_Hyperdrive(Command cmd)
        {
            Planet plan = galaxy!.planets[cmd.numbers["planet"]];
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            if (amount < 0)
                amount = 1;
            int cargo = cmd.numbers["cargo"];
            int fighter = cmd.numbers["fighter"];
            int shield = cmd.numbers["shield"];
            int tractor = cmd.numbers["tractor"];
            amount = CheckIndustry(amount, plan, 40 + cargo + fighter * 2 + shield * 2 + tractor * 2);
            amount = CheckOre(amount, plan, 10, 4);
            amount = CheckOre(amount, plan, 10 + shield, 5);
            amount = CheckOre(amount, plan, 10 + shield, 6);
            amount = CheckOre(amount, plan, 10 + tractor * 2, 7);
            amount = CheckOre(amount, plan, 1 * cargo, 1);
            amount = CheckOre(amount, plan, 1 * fighter, 2);
            amount = CheckOre(amount, plan, 1 * fighter, 3);

            plan.ind_left -= 40 + cargo + fighter * 2 + shield * 2 + tractor * 2;
            plan.ore[1] -= amount * cargo;
            plan.ore[2] -= amount * fighter;
            plan.ore[3] -= amount * fighter;
            plan.ore[4] -= amount * 10;
            plan.ore[5] -= amount * (10 + shield);
            plan.ore[6] -= amount * (10 + shield);
            plan.ore[7] -= amount * (10 + tractor * 2);

            for (int snum = 0; snum < amount; snum++)
            {
                Ship s = galaxy.InitShip(fighter, cargo, shield, tractor, 1);
                s.owner = number;
                s.planet = plan.number;
            }

            results.Add($"Built {amount} hyperdrives");
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
            cmd_results.Add(String.Join(": ", results));
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
            cmd_results.Add(String.Join(": ", results));
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
            cmd_results.Add(String.Join(": ", results));
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
            cmd_results.Add(String.Join(": ", results));
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

        public void Cmd_Unbuild_Cargo(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 1);
            plan.ind_left -= amount;
            int recovered = (int)amount / 2;
            plan.ore[1] += recovered;
            ship.cargo -= amount;
            ship.RemoveDestroyedCargo();
            results.Add($"Recovered {recovered} x R1");
        }

        public void Cmd_Unbuild_Fighter(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 2);
            plan.ind_left -= amount;
            int recovered = (int)amount / 2;
            plan.ore[2] += recovered;
            plan.ore[3] += recovered;
            ship.fighter -= amount;
            results.Add($"Recovered {recovered} x R2 + R3");
        }

        public void Cmd_Unbuild_Shield(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 2);
            plan.ind_left -= amount;
            int recovered = (int)amount / 2;
            plan.ore[5] += recovered;
            plan.ore[6] += recovered;
            ship.shield -= amount;
            results.Add($"Recovered {recovered} x R5 + R6");
        }

        public void Cmd_Unbuild_Tractor(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy.planets[ship.planet];
            if (!CheckShipOwnership(ship, cmd))
                return;
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            int amount = cmd.numbers["amount"];
            amount = CheckIndustry(amount, plan, 2);
            plan.ind_left -= amount;
            plan.ore[7] += amount;
            ship.tractor -= amount;
            results.Add($"Recovered {amount} x R7");
        }
    }
}

