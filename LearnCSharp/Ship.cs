using System;
namespace Celemp
{
    public class Ship
    {
        public string name { get; set; }
        public int number { get; set; }
        public int owner { get; set; }
        public int fight { get; set; }
        public int cargo { get; set; }
        public int shield { get; set; }
        public int tractor { get; set; }
        public int type { get; set; }
        public int[] ore { get; set; } = new int[10];
        public int industry { get; set; }
        public int mines { get; set; }
        public int pdu { get; set; }
        public int planet { get; set; }

        public Ship(int shipnumb)
        {
            name = "";
            number = shipnumb;
            owner = -1;
            fight = 0;
            cargo = 0;
            shield = 0;
            tractor = 0;
            type = 0;
            for (int i = 0;i<10; i++)
            {
                ore[i] = 0;
            }
            industry = 0;
            mines = 0;
            pdu = 0;
            planet = -1;
        }

        public bool IsEmpty()
        // Is this an empty ship - or a hull
        {
            if (cargo == 0 && fight == 0 && tractor == 0 && shield == 0) {
                return true;
            }
            return false;
        }

        public int ShieldPower()
        // Return the shield power
        {
            int totunits = 0;
            float ratio, shldrat;

            totunits = cargo + fight + tractor + shield;
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

        public bool loadSpacemines(int amount, int player)
        {
            if (owner != player)
            {
                Console.WriteLine($"loadSpacemines: Player {player} does not own ship {number}");
                return false;
            }

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

