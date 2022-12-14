using System;
using System.Numerics;
using System.Text.Unicode;
using Neo4j.Driver;
using static Celemp.Constants;

namespace Celemp
{
    public class NeoUpdate
    {
        private Galaxy galaxy;
        private readonly IDriver _driver;

        public NeoUpdate(Galaxy galaxy, string uri, string user, string password)
        {
            this.galaxy = galaxy;
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public void GenerateUpdate()
        {
            for (int plrNum = 0; plrNum < numPlayers; plrNum++)
                NeoUpdates(galaxy.players[plrNum]);
        }

        private void NeoUpdates(Player plr)
        {
            using (var session = _driver.Session())
            {
                for (int plan = 0; plan < numPlanets; plan++)
                {
                    Planet planet = galaxy.planets[plan];
                    if (plr.number == 0 || planet.owner == plr.number)
                        OwnedPlanet(planet, plr, session);
                    else
                        UnownedPlanet(planet, plr, session);
                }
                for (int plan = 0; plan < numPlanets; plan++)
                {
                    Planet planet = galaxy.planets[plan];
                    if (plr.number == 0 || planet.HasVisited(plr.number) || planet.Knows(plr.number))
                        KnownPlanetLinks(planet, plr, session);
                }

                foreach (var ship in galaxy.ships)
                {
                    if (plr.number == 0 || ship.Value.owner == plr.number)
                        KnownShip(ship.Value, plr, session);
                }
            }
        }

        private static void KnownPlanetLinks(Planet planet, Player plr, ISession session)
        {
            for (int linkNum = 0; linkNum < 4; linkNum++)
            {
                if (planet.link[linkNum] >= 0)
                {
                    string cmd = "MATCH";
                    cmd += $"(a: Planet {{number: {planet.number}, player: {plr.number}}}), ";
                    cmd += $"(b: Planet {{number: {planet.link[linkNum]}, player: {plr.number}}}) ";
                    cmd += "MERGE(a) -[r: links]->(b);\n";
                    // Console.Write(cmd);
                    session.Run(cmd);
                }
            }
        }

        public IEnumerable<Planet> RouteToPlanet(Planet start, Planet dest, Player plr)
        {
            using (var session = _driver.Session())
            {
                string cmd = $"MATCH ";
                cmd += $"(s: Planet {{number: {start.number}, player: {plr.number}}}), (p: Planet {{number: {dest.number}, player: {plr.number}}}),";
                cmd += "r = shortestPath((s) -[:links*..10]- (p)) return r";
                var result = session.Run(cmd);
                foreach (var record in result)
                {
                    var path = record["r"].As<IPath>();
                    foreach (var node in path.Nodes)
                        foreach (var label in node.Labels)
                            foreach (var prop in node.Properties)
                                if (prop.Key == "number" && (int)(long)prop.Value != start.number)
                                    yield return galaxy!.planets[(int)(long)prop.Value];
                    yield break;
                }
            }
        }

        public IEnumerable<Planet> RouteToFuel(Planet start, Player plr)
        {
            // match(s:Planet {player:8, number:43}),(d:Planet {player:8} where d.R0>0), r=shortestPath((s)-[*]-(d)) return r
            using (var session = _driver.Session())
            {
                string cmd = $"MATCH ";
                cmd += $"(s: Planet {{number: {start.number}, player: {plr.number}}}), ";
                cmd += $"(d: Planet {{player: {plr.number}}} WHERE d.R0>0), ";
                cmd += "r = shortestPath((s) -[*]- (d)) return r order by length(r)";
                var result = session.Run(cmd);
                foreach (var record in result)
                {
                    var path = record["r"].As<IPath>();
                    foreach (var node in path.Nodes)
                        foreach (var label in node.Labels)
                            foreach (var prop in node.Properties)
                                if (prop.Key == "number" && (int)(long)prop.Value != start.number)
                                    yield return galaxy!.planets[(int)(long)prop.Value];
                    yield break;
                }
            }
        }

        private static void UnownedPlanet(Planet planet, Player plr, ISession session)
        {
            string cmd = $"MERGE (P{plr.number}_{planet.DisplayNumber()}:Planet ";
            cmd += $"{{number: {planet.number}, ";
            cmd += $"name: \"{planet.name}\", ";
            cmd += $"owner: {planet.owner}, ";
            cmd += $"player: {plr.number}}})\n";
            // Console.Write(cmd);
            session.Run(cmd);
        }

        private static void OwnedPlanet(Planet planet, Player plr, ISession session)
        {
            // Remove Old Planet
            session.Run($"MATCH (p:Planet {{number: {planet.number}, player: {plr.number}}}) detach delete p\n");
            // Add new Planet
            string cmd = "";
            cmd += $"MERGE (P{plr.number}_{planet.DisplayNumber()}:Planet {{";
            cmd += $"number: {planet.number}, ";
            cmd += $"player: {plr.number}, ";
            cmd += $"name: \"{planet.name}\", ";
            cmd += $"owner: {planet.owner}, ";
            cmd += $"pdu: {planet.pdu}, ";
            cmd += $"industry: {planet.industry}, ";
            for (int oreType = 0; oreType < numOreTypes; oreType++)
            {
                cmd += $"R{oreType}: {planet.ore[oreType]}, ";
                cmd += $"M{oreType}: {planet.mine[oreType]}, ";
            }
            cmd += $"spacemines: {planet.spacemines}";
            cmd += $"}})\n";
            // Console.Write(cmd);
            session.Run(cmd);
        }

        private void KnownShip(Ship ship, Player plr, ISession session)
        {
            string cmd = "";
            // Delete existing ship
            cmd = $"MATCH (s:Ship {{number: {ship.number}}}) DETACH DELETE s";
            session.Run(cmd);

            // Add new ship;
            cmd = "";
            Planet planet = galaxy!.planets[ship.planet];
            cmd += $"MERGE ({ship.DisplayNumber()}:Ship {{";
            cmd += $"number: {ship.number},";
            cmd += $"player: {plr.number},";
            cmd += $"name: \"{ship.name}\"";
            cmd += "});\n";
            // Console.Write(cmd);
            session.Run(cmd);

            cmd = "MATCH(p: Planet), (s: Ship) ";
            cmd += $"WHERE p.number = {planet.number} AND p.player = {plr.number} AND s.number = {ship.number} ";
            cmd += "MERGE(s) -[r: orbits]->(p);\n";
            // Console.Write(cmd);
            session.Run(cmd);
        }
    }
}

