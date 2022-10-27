using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Celemp
{
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
        public ShipType type { get; set; }
        public Dictionary<String, int> carrying { get; set; }
        public int[] ore { get; set; } = new int[10];
        public int industry { get; set; }
        public int mines { get; set; }
        public int pdu { get; set; }
        public int planet { get; set; }
        public int efficiency { get; set; }
        public String stndord { get; set; }
        private Galaxy? galaxy;

        public Ship()
        {
            name = "Unnamed";
            number = -1;
            owner = -1;
            fighter = 0;
            cargo = 0;
            cargoleft = 0;
            shield = 0;
            tractor = 0;
            type = 0;
            carrying = new Dictionary<string, int>();
            for (int oreType = 0;oreType<10; oreType++)
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

        public String DisplayNumber(int num = -1)
        {
            if (num < 0) { num = number; }
            return (num + 100).ToString();
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

            if (galaxy!.turn < galaxy.earthAmnesty && galaxy.planets[planet].IsEarth())
                return 0; // Zero shots due to Earth amnesty

            ratio = shts / weight;
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
            int weight;

            weight = 1 + cargo + tractor / 2 + shield + (cargo - cargoleft) / 2;
            weight += fighter / 10 - shield / 2;
            return weight;
        }

        public void TurnFriendShip(StreamWriter outfh)
        {
            bool hasCargo = false;
            String ownerName = galaxy!.players[owner].name;
            type = CalcType();

            outfh.WriteLine("\n");
            outfh.WriteLine("\\frame{\\");
            outfh.WriteLine("\\begin{tabular}{rlll}");
            outfh.WriteLine("S" + DisplayNumber() + " & \\multicolumn{2}{l}{" + name + "} & " + type + "\\\\");
            outfh.WriteLine($"Owner {ownerName} & f={fighter} & t={tractor} & s={shield}({ShieldPower()})\\\\");
            outfh.WriteLine($" & cargo={cargo} & cargoleft={cargoleft} & \\\\");
            outfh.WriteLine($" & eff={efficiency}(" + EffectiveEfficiency() + ") & shots=" + Shots(fighter) + " & \\\\");

            outfh.Write("Standing & \\multicolumn{3}{l}{");
            if (stndord.Length == 0)
            {
                outfh.Write("None");
            }
            else
            {
                outfh.Write($"S{DisplayNumber()}{stndord}");
            };
            outfh.WriteLine("}\\\\");

            /* Print out cargo details */
            outfh.Write("Cargo & \\multicolumn{3}{l}{");
            if (carrying["Industry"] != 0)
            {
                outfh.Write("Ind {" + carrying["Industry"] + ";");
                hasCargo = true;
            }
            if (carrying["Mines"] != 0)
            {
                outfh.Write("Mine " + carrying["Mines"] + ";");
                hasCargo = true;
            }
            if (carrying["PDU"] != 0)
            {
                outfh.Write("PDU " + carrying["PDU"] + ";");
                hasCargo = true;
            }
            if (carrying["Spacemines"] != 0)
            {
                outfh.Write("SpcMines " + carrying["Spacemines"]  + ";");
                hasCargo = true;
            }
            for (int oreType = 0; oreType < 10; oreType++)
                if (carrying[$"Ore {oreType}"] != 0)
                {
                    outfh.Write($"R{oreType}" + carrying[$"Ore {oreType}"] + ";");
                    hasCargo = true;
                }
            if (!hasCargo)
            {
                outfh.Write("None");
            };
            outfh.WriteLine("}\\\\");
            outfh.WriteLine("\\end{tabular}");
            outfh.WriteLine("}");
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

        ShipType CalcType()
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

        ShipType CalcDeathStarType()
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

        ShipType CalcBattleType()
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
        
        ShipType CalcCargoType()        
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
                        /*
             * Planet num;

TRLOAD(printf("load:LoadSpcmine(shp:%d,amt:%d)\n",shp,amt));

fprintf(trns[plr],"S%dL%dS\t",shp+100,amt);
if(fleet[shp].owner!=plr) {
    fprintf(trns[plr],"You do not own ship %d\n",shp+100);
    fprintf(stderr,"LoadSpcmine:Plr %d does not own ship %d\n",plr,shp+100);
    return;
    }
    
num=fleet[shp].planet;
if(galaxy[num].owner!=plr && alliance[galaxy[num].owner][plr]!=ALLY) {
    fprintf(trns[plr],"You do not own planet %d\n",num+100);
    fprintf(stderr,"LoadSpcmine:Plr %d does not own planet %d\n",plr,num+100);
    return;
    }
if(fleet[shp].cargleft<amt) {
    amt=fleet[shp].cargleft;
    fprintf(trns[plr],"C ");
    }
if(galaxy[num].spacemine<amt) {
    amt=galaxy[num].spacemine;
    fprintf(trns[plr],"SM ");
    }
galaxy[num].spacemine-=amt;
fleet[shp].spacemines+=amt;
fleet[shp].cargleft-=amt;
fprintf(trns[plr],"S%dL%dS\n",shp+100,amt);
return;
             */
        }
    }
}

