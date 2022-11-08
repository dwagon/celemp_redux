using static Celemp.Constants;

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
        public int[] ore { get; set; } = new int[numOreTypes];
        public int[] mine { get; set; } = new int[numOreTypes];
        public int industry { get; set; }
        public int indleft { get; set; }
        public int pdu { get; set; }
        public int[] link { get; set; } = new int[4];
        public bool[] scanned { get; set; } = new bool[numPlayers];
        public bool research { get; set; }
        public String stndord { get; set; }
        private Galaxy? galaxy;

        public Planet()
        {
            galaxy = null;
            number = -1;
            name = "TODO";
            owner = 0;
            research = false;
            spacemines = 0;
            deployed = 0;
            for (int player=1;player<numPlayers;player++)
            {
                scanned[player] = false;
            }
            stndord = "";
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

        public void InitialiseTurn()
        // Initialise a planet at start of turn
        {
            for (int plrNum = 0; plrNum < numPlayers; plrNum++)
                scanned[plrNum] = false;
            indleft = industry;
        }

        public void ShipArriving(int shipnum)
        {
            // A ship has arrived at the planet
            // TODO

        }

        public void ShipTransitting(int shipnum)
        {
            // A ship is going through the system, but not stopping
            // TODO

        }

        public int SpacemineAttack(int shipnum) {
            // Have the spacemines attack the ship
            // Return the number of hits
            // TODO
            return 0;
        }

        public int PDUAttack(int shipnum)
        {
            // Attack ship transitting
            // TODO
            return 0;
        }

        public void EndTurn()
        // All end of turn processing
        {
            for (int oreType = 0; oreType < numOreTypes; oreType++)
                ore[oreType] += mine[oreType];

            // Ownership check
            int newowner = OwnershipCheck();
            if (newowner != owner)
            {
                Console.WriteLine($"Planet {DisplayNumber()} changed owner from {owner} to {newowner}");
                owner = newowner;
                stndord = "";
            }
        }

        public int Income()
        {
            // Calc Planet Income
            int income = 20 + industry * 5;
            for (int oreType = 0; oreType < numOreTypes; oreType++)
                income += mine[oreType];
            return income;
        }

        private int OwnershipCheck()
        // Return new owner of planet
        {
            List<Ship> orbitting = ShipsOrbitting();
            HashSet<int> owners = new();
            if (pdu > 0)
                return owner;
            if (orbitting.Count == 0)
                return owner;
            foreach (Ship shp in orbitting)
            {
                owners.Add(shp.owner);
            }
            if (owners.Count == 1)
                foreach (int own in owners)
                    return own;

            return owner;
        }

        private List<Ship> ShipsOrbitting()
        // Return list of ships orbitting planet
        {
            List<Ship> orbitting = new();
            foreach (KeyValuePair<int, Ship> shp in galaxy!.ships) {
                if (shp.Value.planet == number)
                    orbitting.Add(shp.Value);
                    }
            return orbitting;
        }

        public String DisplayNumber(int num=-1)
        {
            if (num < 0) { num = number; }
            return (num + 100).ToString();
        }

        public void Scan(int plrNum)
        {
            scanned[plrNum] = true;
        }

        public bool Knows(int plrNum)
        // Does plrNum know about this planet this turn
        {
            if (owner == plrNum)
                return true;
            if (scanned[plrNum])
                return true;
            foreach(KeyValuePair<int, Ship> ship in galaxy!.ships)
            {
                if (ship.Value.owner == plrNum && ship.Value.planet == number)
                    return true;
            }
            return false;
        }
        public bool HasBeenScanned()
        {
            bool result = false;
            for (int plrNum=0;plrNum<numPlayers;plrNum++)
            {
                if (scanned[plrNum])
                    result = true;
            }
            return result;
        }

        public int PduValue()
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

            for (int ore_type = 0; ore_type < numOreTypes; ore_type++)
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
                int ore_type = (int)rnd.Next(numOreTypes);
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