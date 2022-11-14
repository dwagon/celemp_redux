using System;
namespace Celemp
{
    public partial class Player
    {
        public void Cmd_Ship_Attack_Ship(Command cmd)
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
            int fight = CheckShotsLeft(ship, cmd.numbers["amount"]);
            int shots = ship.Shots(fight);

            ship.FireShots(fight);
            results.Add($"Fired {shots} at {victim.DisplayNumber()}");
            galaxy.players[victim.owner].messages.Add($"{ship.DisplayNumber()} attacked your ship {victim.DisplayNumber()} with {shots} shots");
            victim.SufferShots(shots);
        }

        public void Cmd_Ship_Attack_PDU(Command cmd)
        {
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

        public void Cmd_Ship_Attack_Mine(Command cmd)
        {
            Ship ship = galaxy!.ships[cmd.numbers["ship"]];
            Planet plan = galaxy!.planets[ship.planet];
            int oreType = cmd.numbers["oretype"];

            if (!CheckShipOwnership(ship, cmd))
                return;
            int fight = CheckShotsLeft(ship, cmd.numbers["amount"]);
            int shots = ship.Shots(fight);

            ship.FireShots(fight);
            int destroyed = Math.Min(shots / 10, plan.mine[oreType]);
            int ore_destroyed = shots - (destroyed * 10);

            galaxy.players[plan.owner].messages.Add($"{ship.DisplayNumber()} fired on your Mines R{oreType} on {plan.DisplayNumber()} destroying {destroyed} and {ore_destroyed} ore");
            results.Add($"Fired {shots} at Mines R{oreType} destroying {destroyed} of them and {ore_destroyed} ore");
            plan.mine[oreType] -= destroyed;
            plan.ore[oreType] -= ore_destroyed;
        }

        public void Cmd_Ship_Attack_Spacmine(Command cmd)
        {
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

        public void Cmd_Planet_Attack_Ship(Command cmd)
        {
            Planet plan = galaxy!.planets[cmd.numbers["planet"]];
            Ship ship = galaxy.ships[cmd.numbers["victim"]];
            int amount = cmd.numbers["amount"];
            if (!CheckPlanetOwnership(plan, cmd))
                return;
            if (ship.planet != plan.number)
            {
                results.Add($"{ship.DisplayNumber()} not orbitting {plan.DisplayNumber()}");
                return;
            }
            if (amount > 0)
            {
                amount = Math.Min(plan.PduLeft(), amount);
                results.Add("Limited PDU left");
            }
            else
                amount = plan.PduLeft();
            int received = plan.PDUAttack(ship.number, amount);
            ship.SufferShots(received);

            results.Add($"Fired {amount} at {ship.DisplayNumber()}");
            galaxy.players[ship.owner].messages.Add($"{plan.DisplayNumber()} fired on your ship {ship.DisplayNumber()} with {amount} PDUs inflicting {received} damage  ");
        }
    }
}