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
        public int cargoleft { get; set; }
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

        public Ship()
        {
            name = "Unnamed";
            // number = -1;
            owner = -1;
            fighter = 0;
            cargo = 0;
            cargoleft = 0;
            shield = 0;
            tractor = 0;
            moved = false;
            engaged = false;
            engaging = -1;
            carrying = new Dictionary<string, int>();
            for (int oreType = 0;oreType<numOreTypes; oreType++)
            {
                carrying.Add($"Ore {oreType}", 0);
            }
            carrying.Add("Industry", 0);
            carrying.Add("Mines", 0);
            carrying.Add("PDU", 0);
            carrying.Add("Spacemines", 0);
    
            efficiency = 0;
            planet = -1;
            stndord = "";
        }

        public bool HasMoved()
        {
            return moved;
        }

        public bool IsEngaged()
        {
            return engaged;
        }

        public void InitialiseTurn()
        {
            moved = false;
            engaged = false;
        }

        public void SetGalaxy(Galaxy aGalaxy)
        {
            galaxy = aGalaxy;
        }

        public bool IsEmpty()
        // Is this an empty ship - or a hull
        {
            if (cargo == 0 && fighter == 0 && tractor == 0 && shield == 0) {
                return true;
            }
            return false;
        }

        public int FuelRequired(int distance)
        {
            return driveEfficiency[distance-1,EffectiveEfficiency()+1];
        }

        public String DisplayNumber(int num = -1)
        {
            if (num < 0) { num = number; }
            return (num + 100).ToString();
        }

        public int LoadShip(string cargotype, int amount)
        {
            // Load cargo onto the ship - doesn't remove it from source
            // Return the amount actually loaded based on cargo left.
            int scale = CargoScale(cargotype);

            if (cargoleft < amount * scale)
                amount = cargoleft / scale;
            cargoleft -= amount * scale;
            carrying[cargotype] += amount;
            return amount;
        }

        public int UnloadShip(string cargotype, int amount)
        {
            // Unload cargo from the ship - doesn't add it to the destination
            // Return the amount actually unloaded based on cargo left.
            int scale = CargoScale(cargotype);
      
            if (carrying[cargotype] < amount)
                amount = carrying[cargotype];
            cargoleft += amount * scale;
            carrying[cargotype] -= amount;
            return amount;
        }

        public static int CargoScale(string cargotype)
        {
            int scale = 1;
            if (String.Equals(cargotype, "PDU"))
                scale = 2;
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
            if (carrying["Ore 0"] < fuel)
                return false;
            carrying["Ore 0"] -= fuel;
            return true;
        }

        public bool MoveTo(int dest, bool ongoing=false)
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
            return Math.Min(4, efficiency - total / 200);
        }

        public int Shots(int shts)
        // How many shots does this ship have?
        {
            int weight = CalcWeight();
            float tmpshots;
            float ratio;

            //if (galaxy!.turn < galaxy.earthAmnesty && galaxy.planets[planet].IsEarth())
            //    return 0; // Zero shots due to Earth amnesty

            ratio = (float) shts / (float) weight;
            if (ratio >= 10)  // Class three ratio
                tmpshots = 3 * shts;
            else if (ratio < 1) // Class one ratio
                tmpshots = ratio * shts;
            else // Class two ratio
                tmpshots = (0.222F * ratio + 0.777F) * shts;
            return (int)tmpshots;
        }

        public int CalcWeight()
        // Return the weight of the ships
        {
            int weight =1;

            weight += cargo + (cargo - cargoleft) / 2;
            weight += tractor / 2;
            weight += fighter / 10;
            weight += shield / 2;
            return weight;
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

        private bool ShipOwnerCheck(Player player)
        {
            if (owner != player.number)
            {
                Console.WriteLine($"loadSpacemines: Player {player.name} does not own ship {number}");
                return false;
            }
            return true;
        }

        private bool LoadSpacemines(int amount, Player player)
        {
            if (!ShipOwnerCheck(player)) { return false; }
            return true;
        }
    }
}

