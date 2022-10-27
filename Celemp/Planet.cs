using System;
using System.Diagnostics.Metrics;
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
        public int spacemines { get; set; }
        public int deployed { get; set; }
        public int[] ore { get; set; } = new int[10];
        public int[] mine { get; set; } = new int[10];
        public int industry { get; set; }
        public int indleft { get; set; }
        public int pdu { get; set; }
        public int[] link { get; set; } = new int[4];
        public bool[] knows { get; set; } = new bool[9];
        public bool scanned { get; set; }
        public bool research { get; set; }
        public int income { get; set; }
        public String stndord { get; set; }
        private Galaxy? galaxy;

        public Planet()
        {
            galaxy = null;
            number = -1;
            name = "TODO";
            owner = -1;
            research = false;
            spacemines = 0;
            deployed = 0;
            for (int player=0;player<9;player++)
            {
                knows[player] = false;
            }
            scanned = false;
            stndord = "";
            Console.WriteLine("Planet");
        }

        public void setGalaxy(Galaxy? aGalaxy)
        {
            galaxy = aGalaxy;
        }

        public void InitPlanet(Config config)
        // Initialise planets for the first time to random values
        {
            int num_mines = 0;

            num_mines = NumMines(config.galNoMines, config.galExtraMines);
            SetMines(num_mines);
            SetOre(config.galExtraOre);
            SetLinks();
            SetPDU(config.galHasPDU);
            SetIndustry(config.galHasInd);
        }

        public String DisplayNumber(int num=-1)
        {
            if (num < 0) { num = number; }
            return (num + 100).ToString();
        }

        public void TurnPlanetDetails(StreamWriter outfh)
        {
            outfh.Write("\\subsection*{" + DisplayNumber() + " " + name+ "}\n");
            if (research)
                outfh.Write(" --- Research Planet");
            outfh.Write("\n");
            outfh.Write("\\begin{tabular}{r|llll}\n");
            outfh.Write("Owner & \\multicolumn{4}{l}{" + galaxy!.players[owner].name + "}\\\\\n");
            if (scanned)
            {
                outfh.Write("Scanned & \\multicolumn{4}{l}{Planet scanned this turn}\\\\\n");
            };
            outfh.Write("Nearby Planets");
            for (int count = 0; count < 4; count++)
                if (link[count] >= 0)
                    outfh.Write("& " + DisplayNumber(link[count]));
                else
                    outfh.Write(" & ");
            outfh.Write("\\\\\n");
            outfh.Write($"Industry & Industry={industry} & PDU={pdu}({PduValue()}) & Income={income} &\\\\\n");
            outfh.Write($"Spacemines & Stored={spacemines} & Deployed={deployed} & \\\\\n");
            outfh.Write("Standing Order & ");
            if (stndord.Length > 0)
                outfh.Write("\\multicolumn{4}{l}{None}\\\\\n");
            else
                outfh.Write("\\multicolumn{4}{l}{" + (number + 100) + stndord + "}\\\\\n");
            outfh.Write("\\end{tabular}\n\n");

            outfh.Write("\\begin{tabular}{r|cccccccccc}\n");
            outfh.Write("Mine Type");
            for (int oreType = 0; oreType < 10; oreType++)
            {
                outfh.Write("& " + oreType);
            }
            outfh.Write("\\\\ \\hline \n");
            outfh.Write("Amount stored");
            for (int oreType = 0; oreType < 10; oreType++)
            {
                outfh.Write($"& {ore[oreType]}");
            }
            outfh.Write("\\\\\n");
            outfh.Write("Production");
            for (int mineType = 0; mineType < 10; mineType++)
            {
                outfh.Write($"& {mine[mineType]}");
            }
            outfh.Write("\\\\\n");
            outfh.Write("\\end{tabular}\n");
        }

        int PduValue()
        {
            if (pdu > 500)
                return pdu * 4;
            if (pdu > 100)
                return (int)(pdu * (0.0025 * pdu + 2.75));
            if (pdu > 20)
                return (int)(pdu * (0.0125 * pdu + 1.75));
            return (int)(pdu * (0.05 * pdu + 1));
        }

        public bool IsEarth()
        {
            // TODO - Sometimes this should be true
            return false;
        }

        private void SetOre(int pct_extra_ore)
        {
            var rnd = new Random();

            for (int ore_type = 0; ore_type < 10; ore_type++)
            {
                if (mine[ore_type] != 0)
                {
                    if (rnd.Next(100) > (100 - pct_extra_ore))
                        ore[ore_type] = mine[ore_type] + rnd.Next(5);
                    else
                        ore[ore_type] = rnd.Next(3);
                }
                else
                {
                    ore[ore_type] = 0;
                }
            }
        }

        private void SetLinks()
        {
            for (int link_num = 0; link_num < 4; link_num++)
            {
                link[link_num] = -1;
            }
        }

        private int Normal() {
            var rnd = new Random();

            int val = rnd.Next(100);
            if (val > 93) { return 5; }
            if (val > 87) { return 4; }
            if (val > 75) { return 3; }
            if (val > 50) { return 2; }
            return 1;
        }

        private void SetIndustry(int pct_has_industry)
        // Set the industry for a planet
        {
            var rnd = new Random();
            if (rnd.Next(100) > (100 - pct_has_industry))
            {
                industry = Normal();
            }
            else
            {
                industry = 0;
            }
            indleft = industry;
        }

        private void SetPDU(int pct_has_pdu)
        {
            var rnd = new Random();

            if (rnd.Next(100) > (100 - pct_has_pdu))
            {
                pdu = Normal() * 2;
            }
        }

        private int NumMines(int pct_no_mines, int pct_extra_mines)
        // Return the number of mines a normal planet should have
        {
            int num_mines;
            var rnd = new Random();

            if (rnd.Next(100) > (100 - pct_no_mines))
                num_mines = 0;
            else if (rnd.Next(100) > (100 - pct_extra_mines))
                num_mines = Normal() * (rnd.Next(8) + 1);
            else
            {
                num_mines = Normal() * (rnd.Next(5) + 1);
            }
            return num_mines;
        }

        private void SetMines(int num_mines)
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
    }
}