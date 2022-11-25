using System;
using System.Security.AccessControl;
using static Celemp.Constants;

namespace Celemp
{
    public partial class Player
    {

        public void Cmd_UnloadAll(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd))
                return;
            int amount;
            results.Add($"Unloaded");

            for (int oreType = 1; oreType < numOreTypes; oreType++)
            {
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
            if (amount < 0)
                amount = ship.carrying[cargo_pdu];
            if (ship.carrying[cargo_pdu] < amount)
                amount = ship.carrying[cargo_pdu];
            amount = ship.UnloadShip(cargo_pdu, amount);
            planet.pdu += amount;
            results.Add($"Unloaded {amount} PDU");
        }

        public void Cmd_UnloadMine(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];
            int oreType = cmd.numbers["oretype"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            if (amount < 0)
                amount = ship.carrying[cargo_mine];
            if (ship.carrying[cargo_mine] < amount)
                amount = ship.carrying[cargo_mine];
            amount = ship.UnloadShip(cargo_mine, amount);
            planet.mine[oreType] += amount;
            results.Add($"Unloaded {amount} Mine R{oreType}");
        }

        public void Cmd_UnloadIndustry(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            if (amount < 0)
                amount = ship.carrying[cargo_industry];
            if (ship.carrying[cargo_industry] < amount)
                amount = ship.carrying[cargo_industry];
            amount = ship.UnloadShip(cargo_industry, amount);
            planet.industry += amount;
            results.Add($"Unloaded {amount} Industry");
        }

        public void Cmd_UnloadOre(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];
            int oreType = cmd.numbers["oretype"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            if (amount < 0)
                amount = ship.carrying[$"{oreType}"];
            if (ship.carrying[$"{oreType}"] < amount)
                amount = ship.carrying[$"{oreType}"];
            amount = ship.UnloadShip($"{oreType}", amount);
            planet.ore[oreType] += amount;
            results.Add($"Unloaded {amount} R{oreType}");
        }

        public void Cmd_UnloadSpacemine(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            if (amount < 0)
                amount = ship.carrying[cargo_spacemine];
            if (ship.carrying[cargo_pdu] < amount)
                amount = ship.carrying[cargo_spacemine];
            amount = ship.UnloadShip(cargo_spacemine, amount);
            planet.pdu += amount;
            results.Add($"Unloaded {amount} Spacemines");
        }

        private void Cmd_LoadAll(Command cmd)
        {
            // Load all available ore (Except type 0) onto ship
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;

            results.Add("Loaded");
            for (int oretype = 1; oretype < numOreTypes; oretype++)
            {
                int amount = planet.ore[oretype];
                amount = CheckCargoLeft(ship, amount, $"{oretype}", notify: false);
                planet.ore[oretype] -= amount;
                ship.LoadShip($"{oretype}", amount);
                if (amount != 0)
                    results.Add($"{amount} x R{oretype}");
            }
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
            amount = CheckCargoLeft(ship, amount, $"{oretype}");
            ship.LoadShip($"{oretype}", amount);
            planet.ore[oretype] -= amount;
            results.Add($"Loaded {amount} R{oretype}");
        }

        private void Cmd_LoadIndustry(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            int amount = cmd.numbers["amount"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
            amount = CheckCargoLeft(ship, amount, cargo_industry);
            ship.LoadShip(cargo_industry, amount);
            planet.industry -= amount;
            // TODO
        }

        private void Cmd_LoadMine(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];


            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
            // TODO
        }

        private void Cmd_LoadSpacemine(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
            // TODO
        }

        public void Cmd_LoadPDU(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
            if (planet.pdu < amount)
            {
                results.Add("Insufficient PDU on planet");
                amount = planet.pdu;
            }
            amount = CheckCargoLeft(ship, amount, cargo_pdu);
            ship.LoadShip(cargo_pdu, amount);
            planet.pdu -= amount;
            results.Add($"Loaded {amount} PDU");
        }

        private void Cmd_TendAll(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            Ship target = galaxy!.ships[cmd.numbers["target"]];
            if (!CheckShipOwnership(target, cmd))
                return;
            results.Add("Tended ");
            for (int oreType = 1; oreType < numOreTypes; oreType++)
            {
                int amount = ship.carrying[$"{oreType}"];
                if (target.CargoLeft() < amount)
                    amount = target.CargoLeft();
                ship.UnloadShip($"{oreType}", amount);
                target.LoadShip($"{oreType}", amount);
                results.Add($"{amount} x R{oreType}");
            }
            results.Add("OK");
        }

        private void Cmd_TendOre(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            Ship target = galaxy!.ships[cmd.numbers["target"]];
            if (!CheckShipOwnership(target, cmd))
                return;
            int oreType = cmd.numbers["oretype"];
            results.Add("Tended ");
            int amount = cmd.numbers["amount"];
            if (amount < 0)
                amount = ship.carrying[$"{oreType}"];
            amount = CheckCargoLeft(target, amount, $"{oreType}");

            ship.UnloadShip($"{oreType}", amount);
            target.LoadShip($"{oreType}", amount);
            results.Add($"{amount} x R{oreType}");

            results.Add("OK");
        }

        private void Cmd_TendIndustry(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            Ship target = galaxy!.ships[cmd.numbers["target"]];
            if (!CheckShipOwnership(target, cmd))
                return;
            results.Add("Tended ");
            int amount = cmd.numbers["amount"];
            if (amount < 0)
                amount = ship.carrying[cargo_industry];
            amount = CheckCargoLeft(target, amount, cargo_industry);

            ship.UnloadShip(cargo_industry, amount);
            target.LoadShip(cargo_industry, amount);
            results.Add($"{amount} x Industry");

            results.Add("OK");
        }

        private void Cmd_TendMine(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            Ship target = galaxy!.ships[cmd.numbers["target"]];
            if (!CheckShipOwnership(target, cmd))
                return;
            results.Add("Tended ");
            int amount = cmd.numbers["amount"];
            if (amount < 0)
                amount = ship.carrying[cargo_mine];
            amount = CheckCargoLeft(target, amount, cargo_mine);

            ship.UnloadShip(cargo_mine, amount);
            target.LoadShip(cargo_mine, amount);
            results.Add($"{amount} x Mines");

            results.Add("OK");
        }

        private void Cmd_TendPDU(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            Ship target = galaxy!.ships[cmd.numbers["target"]];
            if (!CheckShipOwnership(target, cmd))
                return;
            results.Add("Tended ");
            int amount = cmd.numbers["amount"];
            if (amount < 0)
                amount = ship.carrying[cargo_pdu];
            amount = CheckCargoLeft(target, amount, cargo_pdu);

            ship.UnloadShip(cargo_pdu, amount);
            target.LoadShip(cargo_pdu, amount);
            results.Add($"{amount} x PDU");

            results.Add("OK");
        }

        private void Cmd_TendSpacemine(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            if (!CheckShipOwnership(ship, cmd))
                return;
            Ship target = galaxy!.ships[cmd.numbers["target"]];
            if (!CheckShipOwnership(target, cmd))
                return;
            results.Add("Tended ");
            int amount = cmd.numbers["amount"];
            if (amount < 0)
                amount = ship.carrying[cargo_spacemine];
            amount = CheckCargoLeft(target, amount, cargo_spacemine);

            ship.UnloadShip(cargo_spacemine, amount);
            target.LoadShip(cargo_spacemine, amount);
            results.Add($"{amount} x Spacemines");

            results.Add("OK");
        }
    }
}