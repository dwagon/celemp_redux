using System.Text.Json;
using static Celemp.Constants;

namespace Celemp
{
    [Serializable]
    public class Galaxy
    {
        public int turn { get; set; }
        public String duedate { get; set; }
        public Dictionary<int, Planet> planets { get; set; }
        public Player[] players { get; set; }
        public Dictionary<string, int> earthBids { get; set; }
        public Dictionary<String, Tuple<bool, int>> winning_terms { get; set; }
        public int earthAmnesty { get; set; }
        public int earthMult { get; set; }
        public Dictionary<int, Ship> ships { get; set; }

        private Dictionary<int, Planet> home_planets;
        private int ship_num;

        public Galaxy()
        {
            duedate = "TODO";   // TODO - generate due date
            planets = new Dictionary<int, Planet>();
            ships = new Dictionary<int, Ship>();
            winning_terms = new();
            turn = 0;
            ship_num = 0;
            earthBids = new();
            earthMult = -1;
            earthAmnesty = -1;
            earthBids.Add("Cargo", 1);
            earthBids.Add("Fighter", 1);
            earthBids.Add("Shield", 1);
            players = new Player[numPlayers];
            home_planets = new Dictionary<int, Planet>();
        }

        public List<Command> ParseCommandStrings(List<string>[] cmdstrings)
        {
            List<Command> commands = new();
            for (int plrNum = 1; plrNum < numPlayers; plrNum++)
            {
                foreach (Command cmd in players[plrNum].ParseCommandStrings(cmdstrings[plrNum]))
                    commands.Add(cmd);
            }
            return commands;
        }

        public void ProcessCommands(List<Command> commands)
        {
            foreach (Command cmd in commands)
            {
                players[cmd.plrNum].ProcessCommand(cmd);
            }
        }

        public void InitialiseTurn()
        {
            for (int plrNum=0; plrNum<numPlayers; plrNum++)
            {
                players[plrNum].InitialiseTurn();
            }
            for (int planNum = 0; planNum < numPlanets; planNum++)
                planets[planNum].InitialiseTurn();
            foreach (var kvp in ships)
                kvp.Value.InitialiseTurn();
        }

        public void InitGalaxy(Config config)
        {
            InitConfig(config);
            InitPlanets(config);
            InitPlayers(config);
        }

        public void InitConfig(Config config)
        {
            winning_terms = config.winning_terms;
            earthMult = config.earthMult;
            earthAmnesty = config.earthAmnesty;
        }

        public void SaveGame(string save_file)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);
            File.WriteAllText(save_file, jsonString);
        }

        public void EndTurn()
        // All the end of turn processing
        {
            for (int planNum = 0; planNum < numPlanets; planNum++)
                planets[planNum].EndTurn();
            for (int plrNum = 0; plrNum < numPlayers; plrNum++)
                players[plrNum].EndTurn();
        }

        private void InitPlayers(Config config)
        {
            for (int plrNum = 0; plrNum < numPlayers; plrNum++)
            {
                players[plrNum] = new Player();
                players[plrNum].home_planet = home_planets[plrNum].number;
                players[plrNum].InitPlayer(this, plrNum);
                players[plrNum].name = config.plrNames[plrNum];
                if (plrNum != 0)    // NEUTRAL
                {
                    InitShip1(players[plrNum], config);
                    InitShip2(players[plrNum], config);
                }
            }
        }

        private void InitShip1(Player owner, Config config) {
            Ship newship;

            for (int num=0; num < config.ship1_num; num++)
            {
                newship = InitShip(config.ship1_fight, config.ship1_cargo, config.ship1_shield, config.ship1_tractor, config.ship1_eff);
                newship.owner = owner.number;
                newship.planet = owner.home_planet;
            }
        }

        private void InitShip2(Player owner, Config config) {
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
            Ship newship = new Ship();
            newship.SetGalaxy(this);
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

        private void InitPlanets(Config config) {
            int[] trans = GeneratePlanetShuffle();
            Dictionary<string, List<int>>? linkmap;
        
            linkmap = LoadGalaxyLinks();
            if (linkmap is null)
            {
                Console.WriteLine($"Couldn't load linkmap properly");
                System.Environment.Exit(1);
            }
            for (int plan_num = 0; plan_num< numPlanets; plan_num++) {
                int linknum = 0;

                planets[trans[plan_num]] = new Planet();
                planets[trans[plan_num]].InitPlanet(config);
                planets[trans[plan_num]].setGalaxy(this);
                planets[trans[plan_num]].number = trans[plan_num];
                foreach (int link in linkmap[plan_num.ToString()])
                {
                    planets[trans[plan_num]].link[linknum++] = trans[link];
                }
            }
            NamePlanets();
            SetEarth(trans[228], config);   // 228 is planet Earth

            int[] research =
                { 25, 28, 31, 33, 36, 39, 66, 70, 74, 103, 106, 109, 111, 113, 115, 141, 145, 149,
      178, 181, 184, 186, 188, 190, 216, 220, 224}; // Research planets
            foreach (int rp in research)
            {
                SetResearchPlanet(trans[rp]);
            }

            int plrnum = 0;
            int[] home_planet_list = { 228, 214, 68, 222, 143, 139, 147, 64, 218, 72 };  // Home planets
            foreach (int hp in home_planet_list)
                SetHome(trans[hp], plrnum++, config);
        }

        public int GuessPlayerName(string name)
        // Return the player number from the name
        {
            for(int plrNum=0;plrNum< numPlayers;plrNum++)
            {
                if (players[plrNum] is null)
                {   // So we don't need to define everyone for testing
                    Console.WriteLine($"GuessPlayerName - Ignoring undefined player {plrNum}");
                    continue;
                }
                Console.WriteLine($"Comparing {name} with {players[plrNum].name}");
                if (String.Equals(players[plrNum].name, name, StringComparison.OrdinalIgnoreCase))
                    return plrNum;
            }
            return -1;
        }

        private static int[] GeneratePlanetShuffle() 
        // Shuffle planet numbers around so they aren't always the same
        {
            int[] trans = new int[numPlanets];
            List<int> used = new();
            int num;
            Random rnd = new Random();

            // Generate mapping
            for (int plan_num=0; plan_num < numPlanets; plan_num++)
            {
                num = -1;
                while (num < 0 || used.Contains(num))
                {
                    num = rnd.Next(numPlanets);
                }
                trans[plan_num] = num;
                used.Add(num);
            }
            return trans;
        }

        static private Dictionary<string, List<int>>? LoadGalaxyLinks()
        // Return the dictionary of links for planets
        {
            Dictionary<string, List<int>>? linkdict = new();
            using (StreamReader r = new ("/Users/dwagon/Projects/Celemp/Celemp/GalaxyLinks.json"))
            {
                string jsonString = r.ReadToEnd();
                linkdict = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(jsonString);
            }
            return linkdict;
        }

        void NamePlanets()
        // Give the planets names
        {
            Random rnd = new ();

            string[] planetNames = LoadPlanetNames();
            for (int plan_num=0; plan_num < numPlanets; plan_num++) {
                int idx = rnd.Next(planetNames.Length);
                planets[plan_num].name = planetNames[idx];
            }
        }

        static string[] LoadPlanetNames() {
            string[]? planetnames;
            string fname = "/Users/dwagon/Projects/Celemp/Celemp/PlanetNames.json";
            using (StreamReader r = new (fname))
            {
                string jsonString = r.ReadToEnd();
                planetnames = JsonSerializer.Deserialize<String[]>(jsonString);
            }
            if (planetnames is null)
            {
                Console.WriteLine($"Couldn't read planet names from {fname}");
                Environment.Exit(1);
            }
            return planetnames;
        }

        private void SetResearchPlanet(int plannum)
        // Set specified planet as a Research planet
        {
            Planet rp = planets[plannum];
            rp.research = true;
        }

        private void SetEarth(int plannum, Config config)
        {
            Planet earth = planets[plannum];
            earth.name = "**** EARTH ****";
            earth.industry = config.earthInd;
            earth.pdu = config.earthPDU;
            for (int ore_type = 0; ore_type < numOreTypes; ore_type++)
            {
                earth.ore[ore_type] = config.earthOre[ore_type];
                earth.mine[ore_type] = config.earthMines[ore_type];
            }
        }

        public int NumberResearchPlanetsOwned(int plr_num)
        // Return the number of research planets owned by player
        {
            int count = 0;
            for (int plan_num = 0; plan_num < numPlanets; plan_num++)
            {
                if (planets[plan_num].owner == plr_num && planets[plan_num].research)
                    count++;
            }
            return count;
        }

        private void SetHome(int plan_num, int plrnum, Config config)
        // Make planet {plan_num} the home planet of player {player_num}
        {
            Planet home = planets[plan_num];
            home.owner = plrnum;
            home_planets[plrnum] = home;

            if (plrnum == 0)   // NEUTRAL
                return;
            home.name = $"Home Planet {home.name}";
            for (int ore_type = 0; ore_type < numOreTypes; ore_type++)
            {
                home.mine[ore_type] = config.homeMines[ore_type];
                home.ore[ore_type] = config.homeOre[ore_type];
            }
            home.pdu = config.homePDU;
            home.industry = config.homeIndustry;
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
