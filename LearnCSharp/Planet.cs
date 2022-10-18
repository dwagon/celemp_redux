using System;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Celemp
{
    [Serializable]
    public class Planet
    {
        public string name { get; set; }
        public int number { get; set; }
        public int owner { get; set; }
        public int[] ore { get; set; } = new int[10];
        public int[] mine { get; set; } = new int[10];
        public int industry { get; set; }
        public int indleft { get; set; }
        public int pdu { get; set; }
        public int[] link { get; set; } = new int[4];
        public bool[] knows { get; set; } = new bool[9];
        private readonly Protofile proto;

        public Planet(Protofile protofile, int plannum)
        {
            int num_mines = 0;

            proto = protofile;
            number = plannum;
            name = "foo";
            owner = 0;
            setPDU();
            setIndustry();
            num_mines = numMines();
            setMines(num_mines);
            setOre();
            setLinks();
            for (int player=0;player<9;player++)
            {
                knows[player] = false;
            }
        }

        private void setOre()
        {
            var rnd = new Random();

            for (int ore_type = 0; ore_type < 10; ore_type++)
            {
                if (mine[ore_type] != 0)
                {
                    if (rnd.Next(100) > (100 - proto.galExtraOre))
                    {
                        ore[ore_type] = mine[ore_type] + rnd.Next(5);
                    }
                    else
                    {
                        ore[ore_type] = rnd.Next(3);

                    }
                }
                else
                {
                    ore[ore_type] = 0;
                }
            }
        }

        private void setLinks()
        {
            for (int link_num = 0; link_num < 4; link_num++)
            {
                link[link_num] = -1;
            }
        }

        private int normal() {
            var rnd = new Random();

            int val = rnd.Next(100);
            if (val > 93) { return 5; }
            if (val > 87) { return 4; }
            if (val > 75) { return 3; }
            if (val > 50) { return 2; }
            return 1;
        }

        private void setIndustry()
        // Set the industry for a planet
        {
            var rnd = new Random();
            if (rnd.Next(100) > (100 - proto.galHasInd))
            {
                industry = normal();
            }
            else
            {
                industry = 0;
            }
            indleft = industry;
        }

        private void setPDU()
        {
            var rnd = new Random();
            if (rnd.Next(100) > (100 - proto.galHasPDU))
            {
                pdu = normal() * 2;
            }
        }

        private int numMines()
        // Return the number of mines a normal planet should have
        {
            int num_mines;
            var rnd = new Random();

            if (rnd.Next(100) > (100 - proto.galNoMines))
            {
                num_mines = 0;
            }
            else if (rnd.Next(100) > (100 - proto.galExtraMines))
            {
                num_mines = normal() * (rnd.Next(8) + 1);
            }
            else {
                num_mines = normal() * (rnd.Next(5) + 1);
            }
            return num_mines;
        }

        private void setMines(int num_mines)
        // Allocate mines to the planet with a tendency to clump
        {
            var rnd = new Random();
            int mines = num_mines;
            while (mines != 0)
            {
                int ore_type = (int)rnd.Next(10);
                if (mine[ore_type] == 0)
                {
                    if (rnd.Next(100) > 85)
                    {
                        mine[ore_type]++;
                        mines--;
                    }
                }
                else
                {
                    mine[ore_type]++;
                    mines--;
                }
            }
        }

        private void SetEarth()
        {
            name = "**** EARTH ****";
            industry = proto.earthInd;
            pdu = proto.earthPDU;
            for (int ore_type=0;ore_type < 10; ore_type++)
            {
                ore[ore_type] = proto.earthOre[ore_type];
                mine[ore_type] = proto.earthMines[ore_type];
            }
        } 

        private void SetHome() { }

        private void SetResearch() { }
    }
}