using System;
using System.Text.Json;

namespace Celemp
{
    [Serializable]
    public class Galaxy
    {
        public uint turn { get; set; }
        public Protofile proto = null!;
        public Dictionary<int, Planet> planets { get; set; }
        public Player[] players { get; set; }
        public List<Ship> ships { get; set; }

        public Galaxy(Protofile protofile)
        {
            proto = protofile;
            planets = new Dictionary<int, Planet>();
            ships = new List<Ship>();
            turn = 0;
            players = new Player[9];

            InitPlanets();
            InitPlayers();
        }

        private void InitPlayers()
        {
            for (int plr_num = 0; plr_num < 9; plr_num++)
            {
                players[plr_num] = new Player(proto, plr_num);
            }
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
        // Link the planets together
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
    }
}