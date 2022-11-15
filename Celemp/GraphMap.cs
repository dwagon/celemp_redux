using System;
using System.Drawing;
using static Celemp.Constants;

namespace Celemp
{
    public class GraphMap
    {
        private Galaxy galaxy;

        public GraphMap(Galaxy galaxy)
        {
            this.galaxy = galaxy;
        }

        public void GenerateTurnSheets(string celemp_path)
        {
            for (int plrNum = 0; plrNum < numPlayers; plrNum++)
                TurnSheet(galaxy.players[plrNum], celemp_path);
        }

        private void TurnSheet(Player plr, string celemp_path)
        {
            string dotfile = Path.Join(celemp_path, $"turn_{plr.number}_{galaxy.turn}.dot");
            using (StreamWriter sw = File.CreateText(dotfile))
            {
                Header(sw);
                for (int plan = 0; plan < numPlanets; plan++)
                {
                    Planet planet = galaxy.planets[plan];
                    if (planet.HasVisited(plr.number) || planet.Knows(plr.number))
                        Planet(sw, planet, plr);
                }
                Footer(sw);
            }

        }
        private void Header(StreamWriter outfh)
        {
            outfh.WriteLine("strict graph G {");
        }

        private void Footer(StreamWriter outfh)
        {
            outfh.WriteLine("}");
        }

        private void Planet(StreamWriter outfh, Planet plan, Player plr)
        {
            string label = $"{plan.DisplayNumber()} {plan.name}";
            string shape = "rectangle";
            string colour = "black";

            if (plr.home_planet == plan.number)
                shape = "square";

            if (plan.IsResearch())
                label += " (RP)";

            if (plan.owner == plr.number)
                colour = "green";
            else if (plan.owner != 0)
                colour = "firebrick1";
            else if (plan.pdu != 0)
                colour = "darkorange";

            outfh.Write($"{plan.DisplayNumber()} [");
            outfh.Write($"label=\"{label}\"; ");
            outfh.Write($"shape=\"{shape}\"; ");
            outfh.Write($"color=\"{colour}\"; ");

            outfh.WriteLine("];");
            for (int linkNum = 0; linkNum < 4; linkNum++)
            {
                if (plan.link[linkNum] >= 0)
                    outfh.WriteLine($"{plan.DisplayNumber()} -- {plan.DisplayNumber(plan.link[linkNum])}");
            }

            if (plan.Knows(plr.number))
            {
                foreach (Ship shp in plan.ShipsOrbitting())
                {
                    outfh.Write($"{shp.DisplayNumber()} [");
                    outfh.Write($"label=\"{shp.DisplayNumber()}\";");
                    outfh.Write($"shape=\"hexagon\";");
                    if (shp.owner != plr.number)
                        outfh.Write("color=\"firebrick2\";");
                    else
                        outfh.Write("color=\"green\";");
                    outfh.WriteLine("];");
                    outfh.WriteLine($"{shp.DisplayNumber()} -- {plan.DisplayNumber()};");
                }
            }
        }
    }
}

