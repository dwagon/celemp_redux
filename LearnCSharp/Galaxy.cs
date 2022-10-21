using System;
using System.Text.Json;
using System.Xml.Linq;

namespace Celemp
{
    [Serializable]
    public class Galaxy
    {
        public int game_number { get; set; }
        public int turn { get; set; }
        public String duedate { get; set; }
        public Protofile config = null!;
        public Dictionary<int, Planet> planets { get; set; }
        public Player[] players { get; set; }
        public Dictionary<int, Ship> ships { get; set; }
        public int earth_bids_cargo;
        public int earth_bids_fighter;
        public int earth_bids_shield;

        private Dictionary<int, Planet> home_planets;
        private int ship_num;

        public Galaxy(Protofile protofile)
        {
            config = protofile;
            duedate = "TODO";   // TODO - generate due date
            game_number = 0;    // TODO - make game number a parameter
            planets = new Dictionary<int, Planet>();
            ships = new Dictionary<int, Ship>();
            turn = 0;
            ship_num = 0;
            earth_bids_cargo = 1;
            earth_bids_fighter = 1;
            earth_bids_shield = 1;
            players = new Player[9];
            home_planets = new Dictionary<int, Planet>();

            InitPlanets();
            InitPlayers();
        }

        public void GenerateTurnSheets()
        {
            for (int plrNum = 0; plrNum < 9; plrNum ++)
            {
                players[plrNum].GenerateTurnSheet();
            }
        }

        private void InitPlayers()
        {
            for (int plrNum = 0; plrNum < 9; plrNum++)
            {
                players[plrNum] = new Player(this, plrNum);
                players[plrNum].home_planet = home_planets[plrNum].number;

                InitShip1(players[plrNum]);
                InitShip2(players[plrNum]);
            }
        }

        private void InitShip1(Player owner) {
            Ship newship;

            for (int num=0; num < config.ship1_num; num++)
            {
                newship = InitShip(config.ship1_fight, config.ship1_cargo, config.ship1_shield, config.ship1_tractor, config.ship1_eff);
                newship.owner = owner.number;
                newship.planet = owner.home_planet;
            }
        }

        private void InitShip2(Player owner) {
            Ship newship;

            for (int num = 0; num < config.ship2_num; num++)
            {
                newship = InitShip(config.ship2_fight, config.ship2_cargo, config.ship2_shield, config.ship2_tractor, config.ship2_eff);
                newship.owner = owner.number;
                newship.planet = owner.home_planet;
            }
        }

        private Ship InitShip(int fight, int cargo, int shield, int tractor, int eff)
        {
            Ship newship = new Ship(this);
            newship.fighter = fight;
            newship.cargo = cargo;
            newship.cargoleft = cargo;
            newship.shield = shield;
            newship.tractor = tractor;
            newship.efficiency = eff;
            newship.number = ship_num;
            ships[ship_num++] = newship;
            return newship;
        }

        private void InitPlanets() {
            int[] trans = new int[256];
            Dictionary<String, List<int>> linkmap;
        
            trans = GeneratePlanetShuffle();
            linkmap = LoadGalaxyLinks();
            for (int plan_num = 0; plan_num< 256; plan_num++) {
                int linknum = 0;

                planets[trans[plan_num]] = new Planet(this, trans[plan_num]);
                foreach (int link in linkmap[plan_num.ToString()])
                {
                    planets[trans[plan_num]].link[linknum++] = trans[link];
                }
            }
            NamePlanets();
            SetEarth(trans[228]);   // 228 is planet Earth

            int[] research =
                { 25, 28, 31, 33, 36, 39, 66, 70, 74, 103, 106, 109, 111, 113, 115, 141, 145, 149,
      178, 181, 184, 186, 188, 190, 216, 220, 224}; // Research planets
            foreach (int rp in research)
            {
                SetResearchPlanet(trans[rp]);
            }

            int plrnum = 0;
            int[] home_planets = { 214, 68, 222, 143, 139, 147, 64, 218, 72 };  // Home planets
            foreach (int hp in home_planets)
            {
                SetHome(trans[hp], plrnum++);
            }
    }

        private static int[] GeneratePlanetShuffle() 
        // Shuffle planet numbers around so they aren't always the same
        {
            int[] trans = new int[256];
            List<int> used = new();
            int num;
            Random rnd = new Random();

            // Generate mapping
            for (int plan_num=0; plan_num < 256; plan_num++)
            {
                num = -1;
                while (num < 0 || used.Contains(num))
                {
                    num = rnd.Next(256);
                }
                trans[plan_num] = num;
                used.Add(num);
            }
            return trans;
        }

        private Dictionary<String, List<int>> LoadGalaxyLinks()
        // Return the dictionary of links for planets
        {
            Dictionary<String, List<int>>? linkdict;
            using (StreamReader r = new ("/Users/dwagon/Projects/LearnCSharp/LearnCSharp/GalaxyLinks.json"))
            {
                string jsonString = r.ReadToEnd();
                linkdict = JsonSerializer.Deserialize<Dictionary<String, List<int>>>(jsonString);
            }
            return linkdict;
        }

        void NamePlanets()
        // Give the planets names
        {
            Random rnd = new ();

            string[] planetNames = LoadPlanetNames();
            for (int plan_num=0; plan_num < 256; plan_num++) {
                int idx = rnd.Next(planetNames.Length);
                planets[plan_num].name = planetNames[idx];
            }
        }

        static string[]? LoadPlanetNames() {
            string[]? planetnames = null!;
            using (StreamReader r = new StreamReader("/Users/dwagon/Projects/LearnCSharp/LearnCSharp/PlanetNames.json"))
            {
                string jsonString = r.ReadToEnd();
                planetnames = JsonSerializer.Deserialize<String[]>(jsonString);
            }
            return planetnames;
        }

        private void SetResearchPlanet(int plannum)
        // Set specified planet as a Research planet
        {
            Planet rp = planets[plannum];
            rp.research = true;
        }

        private void SetEarth(int plannum)
        {
            Planet earth = planets[plannum];
            earth.name = "**** EARTH ****";
            earth.industry = config.earthInd;
            earth.pdu = config.earthPDU;
            for (int ore_type = 0; ore_type < 10; ore_type++)
            {
                earth.ore[ore_type] = config.earthOre[ore_type];
                earth.mine[ore_type] = config.earthMines[ore_type];
            }
        }

        private void SetHome(int plan_num, int player_num)
        // Make planet {plan_num} the home planet of player {player_num}
        {
            Planet home = planets[plan_num];
            home.name = $"Home Planet {home.name}";
            home.owner = player_num;
            home_planets[player_num] = home;
            for (int ore_type = 0; ore_type < 10; ore_type++)
            {
                home.mine[ore_type] = config.homeMines[ore_type];
                home.ore[ore_type] = config.homeOre[ore_type];
            }
            home.pdu = config.homePDU;
            home.industry = config.homeIndustry;
            home.indleft = home.industry;
            home.knows[player_num] = true;
            // Ensure no A-ring planets are defended or industrial to make things more even
            for (int link=0;link<4; link++)
            {
                Planet neighbour = planets[home.link[link]];
                neighbour.industry = 0;
                neighbour.pdu = 0;
            }
          
        }
    }
}