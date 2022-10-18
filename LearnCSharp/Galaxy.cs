using System;
using System.Text.Json;

namespace Celemp
{
    [Serializable]
    public class Galaxy
    {
        public uint turn { get; set; }
        public Protofile proto = null!;
        public Planet[] planets { get; set; }
        public Player[] players { get; set; }
        public Ship[] ships { get; set; }

        public Galaxy(Protofile protofile)
        {
            proto = protofile;
            planets = new Planet[256];
            ships = new Ship[256];
            turn = 0;

            players = new Player[9];

            InitPlanets();

            for (int plr_num=0; plr_num<9;plr_num++)
            {
                players[plr_num] = new Player(proto, plr_num);
            }
        }

        private void InitPlanets() {
            int[] trans = new int[256];
            for (int plan_num = 0; plan_num < 256; plan_num++)
            {
                planets[plan_num] = new Planet(proto, plan_num);
            }
            NamePlanets();
            trans = GeneratePlanetShuffle();
            LinkPlanets(trans);
        }

        private static int[] GeneratePlanetShuffle() 
        // Shuffle planet numbers around so they aren't always the same
        {
            int[] trans = new int[256];
            Dictionary<int, bool> used = new Dictionary<int, bool>();
            int num;
            Random rnd = new Random();

            // Generate mapping
            for (int plan_num=0; plan_num < 256; plan_num++)
            {
                num = -1;
                while (num < 0 || used.ContainsKey(num))
                {
                    num = rnd.Next(256);
                }
                trans[plan_num] = num;
            }
            return trans;
        }

        private void LinkPlanets(int[] trans)
        // Link the planets together
        { 
            Dictionary<String, List<int>>? linkdict;
            using (StreamReader r = new ("/Users/dwagon/Projects/LearnCSharp/LearnCSharp/GalaxyLinks.json"))
            {
                string jsonString = r.ReadToEnd();
                linkdict = JsonSerializer.Deserialize<Dictionary<String, List<int>>>(jsonString);
            }
            for (int plan_num = 0; plan_num < 256; plan_num++)
            {
                int linknum = 0;
                foreach (int link in linkdict[plan_num.ToString()])
                {
                    planets[plan_num].link[linknum++] = trans[link];
                }
            }
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