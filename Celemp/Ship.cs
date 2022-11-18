using System;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using static Celemp.Constants;

namespace Celemp
{
    [Serializable]
    public class Ship
    {
        public string name { get; set; }
        public int number { get; set; }
        public int owner { get; set; }
        public int fighter { get; set; }
        public int cargo { get; set; }
        public int shield { get; set; }
        public int tractor { get; set; }
        public Dictionary<String, int> carrying { get; set; }
        public int planet { get; set; }
        public int efficiency { get; set; }
        public String stndord { get; set; }

        private bool moved;     // Moved this turn
        private bool engaged;   // Engaged by a tractor beam
        private int engaging;   // Ship engaged by our tractor
        private Galaxy? galaxy;
        private int shotsleft;
        private int hits;

        public Ship()
        {
            name = "Unnamed";
            // number = -1;
            owner = -1;
            fighter = 0;
            cargo = 0;
            shield = 0;
            tractor = 0;
            moved = false;
            engaged = false;
            engaging = -1;
            carrying = new Dictionary<string, int>();
            for (int oreType = 0; oreType < numOreTypes; oreType++)
            {
                carrying.Add($"{oreType}", 0);
            }
            carrying.Add(cargo_industry, 0);
            carrying.Add(cargo_mine, 0);
            carrying.Add(cargo_pdu, 0);
            carrying.Add(cargo_spacemine, 0);

            efficiency = 0;
            planet = -1;
            shotsleft = fighter;
            stndord = "";
            hits = 0;
        }

        public Ship(Galaxy g, int n) : this()
        {
            galaxy = g;
            number = n;
            g.ships[n] = this;
        }

        public int CargoLeft()
        {
            // Return how much cargo capacity is left
            int cargoleft = cargo;
            for (int oreType = 0; oreType < numOreTypes; oreType++)
                cargoleft -= carrying[$"{oreType}"] * CargoScale($"{oreType}");
            cargoleft -= carrying[cargo_industry] * CargoScale(cargo_industry);
            cargoleft -= carrying[cargo_mine] * CargoScale(cargo_mine);
            cargoleft -= carrying[cargo_pdu] * CargoScale(cargo_pdu);
            cargoleft -= carrying[cargo_spacemine] * CargoScale(cargo_spacemine);
            return cargoleft;
        }

        public bool HasMoved()
        {
            return moved;
        }

        public bool IsEngaged()
        {
            return engaged;
        }

        public bool IsEngaging()
        {
            return engaging >= 0;
        }

        public void EngageShip(Ship target)
        {
            engaging = target.number;
            target.engaged = true;
        }

        public void EndTurn()
        {
            if (IsEmpty())
            {
                if (galaxy!.planets[planet].owner != owner)
                {
                    int newowner = galaxy!.planets[planet].owner;
                    galaxy.players[newowner].messages.Add($"You now control {DisplayNumber()}");
                    stndord = "";
                    galaxy.players[owner].messages.Add($"You lost control of {DisplayNumber()}");
                    owner = newowner;
                }
            }
        }

        public void ResolveDamage()
        {
            if (hits == 0)
                return;
            hits -= ShieldPower();
            if (hits < 0)
            {
                galaxy!.players[owner].messages.Add($"{DisplayNumber()} All hits absorbed on shields");
                return;
            }
            else
            {
                galaxy!.players[owner].messages.Add($"{DisplayNumber()} {ShieldPower()} hits absorbed on shields");
            }

            int shield_destroyed = Math.Min(shield, hits);
            shield -= shield_destroyed;
            if (shield_destroyed > 0)
                galaxy!.players[owner].messages.Add($"{DisplayNumber()} {shield_destroyed} Shields units destroyed");
            hits -= shield_destroyed;
            if (hits < 0)
                return;

            int fight_destroyed = Math.Min(fighter, hits);
            fighter -= fight_destroyed;
            if (fight_destroyed > 0)
                galaxy!.players[owner].messages.Add($"{DisplayNumber()} {fight_destroyed} Fighter units destroyed");
            hits -= fight_destroyed;
            if (hits < 0)
                return;

            int tractor_destroyed = Math.Min(tractor, hits);
            tractor -= tractor_destroyed;
            if (tractor_destroyed > 0)
                galaxy!.players[owner].messages.Add($"{DisplayNumber()} {tractor_destroyed} Tractor units destroyed");
            hits -= tractor_destroyed;
            if (hits < 0)
                return;

            int cargo_destroyed = Math.Min(cargo, hits);
            cargo -= cargo_destroyed;
            if (cargo_destroyed > 0)
                galaxy!.players[owner].messages.Add($"{DisplayNumber()} {cargo_destroyed} Cargo units destroyed");

            galaxy!.players[owner].messages.Add($"{DisplayNumber()} is now {CalcType()}");
            RemoveDestroyedCargo();
        }

        public void RemoveDestroyedCargo()
        {
            Planet orbitting = galaxy!.planets[planet];

            if (CargoLeft() >= 0)
            {
                return;
            }
            int mine_to_dump = Math.Min(carrying[cargo_mine], Math.Abs(CargoLeft() / CargoScale(cargo_mine)));
            carrying[cargo_mine] -= mine_to_dump;
            DumpDamagedMine(mine_to_dump);

            int ind_to_dump = Math.Min(carrying[cargo_industry], Math.Abs(CargoLeft() / CargoScale(cargo_industry)));
            carrying[cargo_industry] -= ind_to_dump;
            orbitting.industry += ind_to_dump;

            int pdu_to_dump = Math.Min(carrying[cargo_pdu], Math.Abs(CargoLeft() / CargoScale(cargo_pdu)));
            carrying[cargo_pdu] -= pdu_to_dump;
            orbitting.pdu += pdu_to_dump;

            int spcm_to_dump = Math.Min(carrying[cargo_spacemine], Math.Abs(CargoLeft() / CargoScale(cargo_spacemine)));
            carrying[cargo_spacemine] -= spcm_to_dump;
            orbitting.deployed += spcm_to_dump;

            for (int oreType = numOreTypes - 1; oreType >= 0; oreType--)
            {
                int ore_to_dump = Math.Min(carrying[$"{oreType}"], Math.Abs(CargoLeft() / CargoScale($"{oreType}")));
                carrying[$"{oreType}"] -= ore_to_dump;
                orbitting.ore[oreType] += ore_to_dump;
            }
        }

        public void DumpDamagedMine(int num)
        {
            // TODO
            // These mines will be randomly distributed amongst the mines that exist on the planet below.If there are no mines on that planet, then these mines will be destroyed
            Console.WriteLine($"Dumping {num} mines");
        }

        public void InitialiseTurn()
        {
            moved = false;
            engaged = false;
            shotsleft = fighter;
            hits = 0;
        }

        public int ShotsLeft()
        {
            return shotsleft;
        }

        public void FireShots(int funits)
        {
            if (funits < 0)
                funits = fighter;
            shotsleft -= funits;
            moved = true;
        }

        public void SufferShots(int shots)
        {
            // Someone has attacked our ship
            // Store how many hits we have taken and process at end of turn
            hits += shots;
        }

        public void SetGalaxy(Galaxy aGalaxy)
        {
            galaxy = aGalaxy;
        }

        public bool IsEmpty()
        // Is this an empty ship - or a hull
        {
            if (cargo == 0 && fighter == 0 && tractor == 0 && shield == 0)
            {
                return true;
            }
            return false;
        }

        public int FuelRequired(int distance)
        {
            return driveEfficiency[distance - 1, EffectiveEfficiency() + 1];
        }

        public String DisplayNumber(int num = -1)
        {
            if (num < 0) { num = number; }
            return "S" + (num + 100).ToString();
        }

        public void LoadShip(string cargotype, int amount)
        {
            // Load cargo onto the ship - doesn't remove it from source
            carrying[cargotype] += amount;
        }

        public int UnloadShip(string cargotype, int amount)
        {
            // Unload cargo from the ship - doesn't add it to the destination
            int scale = CargoScale(cargotype);

            if (carrying[cargotype] < amount)
                amount = carrying[cargotype];
            carrying[cargotype] -= amount;
            return amount;
        }

        public static int CargoScale(string cargotype)
        {
            int scale = 1;
            if (String.Equals(cargotype, cargo_spacemine))
                scale = 1;
            if (String.Equals(cargotype, cargo_pdu))
                scale = 2;
            if (String.Equals(cargotype, cargo_industry))
                scale = 10;
            if (String.Equals(cargotype, cargo_mine))
                scale = 20;
            return scale;
        }

        public bool CheckDest(int dest)
        {
            Planet plan = galaxy!.planets[planet];
            bool found = false;
            foreach (int linknum in plan.link)
            {
                if (dest == linknum)
                {
                    planet = dest;
                    found = true;
                }
            }
            return found;
        }

        public bool UseFuel(int dist)
        // Use fuel for jumping - return false if not enough
        {
            int fuel = FuelRequired(dist);
            if (carrying["0"] < fuel)
                return false;
            carrying["0"] -= fuel;
            return true;
        }

        public bool MoveTo(int dest, bool ongoing = false)
        // Move a ship to a planet
        {
            Planet plan = galaxy!.planets[dest];
            planet = dest;
            if (engaging >= 0)
                galaxy!.ships[engaging].planet = dest;

            if (ongoing)
            {
                plan.ShipTransitting(number);
                if (engaging >= 0)
                    plan.ShipTransitting(engaging);
            }
            plan.ShipArriving(number);
            if (engaging >= 0)
                plan.ShipArriving(engaging);

            return false;
        }

        public int EffectiveEfficiency()
        {
            int total;

            total = cargo + fighter + tractor + shield;
            return Math.Min(4, efficiency - (int)(total / 200.0D));
        }

        public int Shots(int shts = -1)
        // How many shots does this ship have?
        {
            int weight = CalcWeight();
            float tmpshots;
            float ratio;

            //if (galaxy!.turn < galaxy.earthAmnesty && galaxy.planets[planet].IsEarth())
            //    return 0; // Zero shots due to Earth amnesty
            if (shts < 0)
                shts = fighter;
            ratio = (float)fighter / (float)weight;
            if (ratio >= 10)  // Class three ratio - unavailable in reality
                tmpshots = 3.0F * shts;
            else if (ratio < 1) // Class one ratio
                tmpshots = ratio * shts;
            else // Class two ratio
                tmpshots = (0.222F * ratio + 0.777F) * shts;
            return (int)tmpshots;
        }

        public int CalcWeight()
        // Return the weight of the ships
        {
            float weight = 1;

            weight += cargo + (cargo - CargoLeft()) / 2.0F;
            weight += tractor / 2.0F;
            weight += fighter / 10.0F;
            weight += shield / 2.0F;
            return (int)weight;
        }

        public int ShieldPower()
        // Return the shield power
        {
            int totunits = 0;
            float ratio, shldrat;

            totunits = cargo + fighter + tractor + shield;
            if (totunits == 0 || shield == 0)
            {
                return 0;
            }

            ratio = 100 * shield / totunits;
            if (ratio > 90)
            {
                shldrat = 4.0F;
                return (int)(shield * shldrat);
            }
            if (ratio < 20)
            {
                shldrat = ratio / 20 + 1;
                return (int)(shield * shldrat);
            }
            shldrat = ratio * 0.0286F + 1.4286F;
            return (int)(shield * shldrat);
        }

        public ShipType CalcType()
        // Calculate the type of ship that it is
        {
            int ratio;

            if (cargo == 0)
            {
                if (fighter == 0)
                    return ShipType.Hull;
                return CalcDeathStarType();
            }
            ratio = (100 * fighter) / cargo;
            if (ratio > 900)
                return CalcDeathStarType();
            if (ratio > 200)
                return CalcBattleType();
            if (ratio > 50)
                return CalcShipType();
            return CalcCargoType();
        }

        private ShipType CalcDeathStarType()
        {
            if (fighter > 250)
                return ShipType.UltraDeathstar;
            if (fighter > 150)
                return ShipType.MegaDeathstar;
            if (fighter > 75)
                return ShipType.SuperDeathstar;
            if (fighter > 30)
                return ShipType.LargeDeathstar;
            if (fighter > 10)
                return ShipType.MediumDeathstar;
            return ShipType.SmallDeathstar;
        }

        private ShipType CalcBattleType()
        {
            if (fighter > 250)
                return ShipType.UltraBattle;
            if (fighter > 150)
                return ShipType.MegaBattle;
            if (fighter > 75)
                return ShipType.SuperBattle;
            if (fighter > 30)
                return ShipType.LargeBattle;
            if (fighter > 10)
                return ShipType.MediumBattle;
            return ShipType.SmallBattle;
        }

        ShipType CalcShipType()
        {
            if (fighter > 250)
                return ShipType.UltraCargo;
            if (fighter > 150)
                return ShipType.MegaShip;
            if (fighter > 75)
                return ShipType.SuperShip;
            if (fighter > 30)
                return ShipType.LargeShip;
            if (fighter > 10)
                return ShipType.MediumShip;
            return ShipType.SmallShip;
        }

        private ShipType CalcCargoType()
        {
            if (cargo > 250)
                return ShipType.UltraCargo;
            if (cargo > 150)
                return ShipType.MegaCargo;
            if (cargo > 75)
                return ShipType.SuperCargo;
            if (cargo > 30)
                return ShipType.LargeCargo;
            if (cargo > 10)
                return ShipType.MediumCargo;
            return ShipType.SmallCargo;
        }
    }
}

