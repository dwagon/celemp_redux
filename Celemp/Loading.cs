using System;
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
            if (ship.carrying["PDU"] < amount)
                amount = ship.carrying["PDU"];
            amount = ship.UnloadShip("PDU", amount);
            planet.pdu += amount;
            results.Add($"Unloaded {amount} PDU");
        }

        public void Cmd_UnloadMine(Command cmd)
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

        public void Cmd_UnloadIndustry(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            if (ship.carrying["Industry"] < amount)
                amount = ship.carrying["Industry"];
            amount = ship.UnloadShip("Industry", amount);
            planet.industry += amount;
            results.Add($"Unloaded {amount} Industry");
        }
        
        public void Cmd_UnloadOre(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];
            int oreType = cmd.numbers["oretype"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            if (ship.carrying[$"{oreType}"] < amount)
                amount = ship.carrying[$"{oreType}"];
            amount = ship.UnloadShip($"{oreType}", amount);
            planet.ore[oreType] += amount;
            results.Add($"Unloaded {amount} R{oreType}");
        }

        public void Cmd_UnloadSpacemine(Command cmd) {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];

            Planet planet = galaxy!.planets[ship.planet];
            int amount = cmd.numbers["amount"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            if (ship.carrying["PDU"] < amount)
                amount = ship.carrying["Spacemines"];
            amount = ship.UnloadShip("Spacemines", amount);
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
                if (ship.CargoLeft() < amount)
                    amount = ship.CargoLeft();
                planet.ore[oretype] -= amount;
                ship.LoadShip($"{oretype}", amount);
                if (amount != 0)
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

        private void Cmd_LoadIndustry(Command cmd)
        {
            int shipNum = cmd.numbers["ship"];
            Ship ship = galaxy!.ships[shipNum];
            Planet planet = galaxy!.planets[ship.planet];

            if (!CheckShipOwnership(ship, cmd) || !CheckPlanetOwnership(planet, cmd))
                return;
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
                amount = planet.pdu;
            amount = ship.LoadShip("PDU", amount);
            planet.pdu -= amount;
            results.Add($"Loaded {amount} PDU");
        }
    }
}