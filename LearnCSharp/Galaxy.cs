using System;
using System.Text.Json;
using System.Xml.Linq;

namespace Celemp
{
    [Serializable]
    public class Galaxy
    {
        public uint turn { get; set; }
        public Protofile proto = null!;
        public Dictionary<int, Planet> planets { get; set; }
        public Player[] players { get; set; }
        public Dictionary<int, Ship> ships { get; set; }
        private Dictionary<int, Planet> home_planets;
        private int ship_num;

        public Galaxy(Protofile protofile)
        {
            proto = protofile;
            planets = new Dictionary<int, Planet>();
            ships = new Dictionary<int, Ship>();
            turn = 0;
            ship_num = 0;
            players = new Player[9];
            home_planets = new Dictionary<int, Planet>();

            InitPlanets();
            InitPlayers();
        }

        private void InitPlayers()
        {
            for (int plr_num = 0; plr_num < 9; plr_num++)
            {
                players[plr_num] = new Player(proto, plr_num);
                players[plr_num].home_planet = home_planets[plr_num].number;

                InitShip1(players[plr_num]);
                InitShip2(players[plr_num]);
            }
        }

        private void InitShip1(Player owner) {
            Ship newship;

            for (int num=0; num < proto.ship1_num; num++)
            {
                newship = InitShip(proto.ship1_fight, proto.ship1_cargo, proto.ship1_shield, proto.ship1_tractor, proto.ship1_eff);
                newship.owner = owner.number;
                newship.planet = owner.home_planet;
            }
        }

        private void InitShip2(Player owner) {
            Ship newship;

            for (int num = 0; num < proto.ship2_num; num++)
            {
                newship = InitShip(proto.ship2_fight, proto.ship2_cargo, proto.ship2_shield, proto.ship2_tractor, proto.ship2_eff);
                newship.owner = owner.number;
                newship.planet = owner.home_planet;
            }
        }

        private Ship InitShip(int fight, int cargo, int shield, int tractor, int eff)
        {
            Ship newship = new Ship();
            newship.fight = fight;
            newship.cargo = cargo;
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

                planets[trans[plan_num]] = new Planet(proto, trans[plan_num]);
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
            Random rnd = new Random();

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
            rp.name = $"{rp.name} RP";
            rp.research = true;
        }

        private void SetEarth(int plannum)
        {
            Planet earth = planets[plannum];
            earth.name = "**** EARTH ****";
            earth.industry = proto.earthInd;
            earth.pdu = proto.earthPDU;
            for (int ore_type = 0; ore_type < 10; ore_type++)
            {
                earth.ore[ore_type] = proto.earthOre[ore_type];
                earth.mine[ore_type] = proto.earthMines[ore_type];
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
                home.mine[ore_type] = proto.homeMines[ore_type];
                home.ore[ore_type] = proto.homeOre[ore_type];
            }
            home.pdu = proto.homePDU;
            home.industry = proto.homeIndustry;
            home.indleft = home.industry;
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