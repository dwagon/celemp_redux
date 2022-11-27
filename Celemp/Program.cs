using System.Text.Json;
using static Celemp.Constants;

namespace Celemp
{
    class Program
    {
        static void Main(string[] args)
        {
            int game_number;

            if (args.Length != 2)
            {
                PrintUsage();
                Environment.Exit(1);
            }

            Program pc = new Program();
            game_number = Int16.Parse(args[1]);
            string game_path = MakePath(game_number);

            switch (args[0])
            {
                case "new":
                    pc.NewGame(game_path);
                    break;
                case "turn":
                    pc.ProcessTurns(game_path);
                    break;
                case "sheet":
                    pc.GenerateTurnSheets(game_number, game_path);
                    break;
                default:
                    Console.WriteLine($"Unknown argument {args[0]}");
                    break;
            }
        }

        static string MakePath(int game_number)
        {
            string file_path;
            string? celemp_path;
            celemp_path = Environment.GetEnvironmentVariable("CELEMP_HOME");


            if (celemp_path == null)
            {
                Console.WriteLine("Need a value for CELEMP_HOME");
                Environment.Exit(1);
            }
            file_path = Path.Join(celemp_path, $"game_{game_number}");

            if (!Directory.Exists(file_path))
            {
                Console.WriteLine($"Creating directory {file_path}");
                Directory.CreateDirectory(file_path);
            }
            return file_path;
        }

        static void PrintUsage()
        {
            Console.WriteLine("celemp new <gamenum>");
            Console.WriteLine("celemp turn <gamenum>");
            Console.WriteLine("celemp sheet <gamenum>");
        }

        void NewGame(string celemp_path)
        {
            string save_file = Path.Join(celemp_path, "celemp.json");
            Config config = new(Path.Join(celemp_path, "protofile"));
            Galaxy galaxy = new();

            Console.WriteLine("New galaxy");
            galaxy.InitGalaxy(config);
            galaxy.SaveGame(save_file);
        }

        void GenerateTurnSheets(int game_number, string celemp_path)
        {
            string save_file = Path.Join(celemp_path, "celemp.json");

            Galaxy galaxy = LoadGame(save_file);

            NeoUpdate neo = new NeoUpdate(galaxy, "neo4j://localhost", "neo4j", "secret");
            neo.GenerateUpdate();

            TurnSheet ts = new TurnSheet(galaxy);
            ts.GenerateTurnSheets(celemp_path, neo);

            GraphMap gm = new GraphMap(galaxy);
            gm.GenerateTurnSheets(celemp_path);
        }

        void ProcessTurns(string celemp_path)
        {
            string save_file = Path.Join(celemp_path, "celemp.json");
            Galaxy galaxy = LoadGame(save_file);
            galaxy.InitialiseTurn();

            List<string>[] cmdstrings = LoadCommandStrings(celemp_path, galaxy);
            List<Command> commands = galaxy.ParseCommandStrings(cmdstrings);
            commands.Sort();
            galaxy.ProcessCommands(commands);
            galaxy.EndTurn();
            string cmd_fname;
            // Now that the turn has succeeded back up the cmd files
            for (int plrNum = 1; plrNum < numPlayers; plrNum++)
            {
                cmd_fname = Path.Join(celemp_path, $"cmd{plrNum}");
                try
                {
                    File.Move(cmd_fname, $"{cmd_fname}.{galaxy.turn}");
                }
                catch (FileNotFoundException exc)
                {
                    Console.WriteLine($"Could not back up {cmd_fname} - {exc.Message}");
                }
            }
            File.Move(save_file, $"{save_file}.{galaxy.turn}");
            galaxy.SaveGame(save_file);
        }

        List<string>[] LoadCommandStrings(string celemp_path, Galaxy galaxy)
        // Load the commands from the players
        {
            string cmd_fname;
            List<string>[] commands = new List<string>[numPlayers];

            for (int plrNum = 1; plrNum < numPlayers; plrNum++)
            {
                commands[plrNum] = new List<string>();
                cmd_fname = Path.Join(celemp_path, $"cmd{plrNum}");
                Console.WriteLine($"Loading commands from {cmd_fname}");
                try
                {
                    foreach (string line in File.ReadLines(cmd_fname))
                    {
                        commands[plrNum].Add(line.Trim());
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Couldn't read commands from {cmd_fname}: {exc.Message}");
                }
            }
            return commands;
        }

        public Galaxy LoadGame(string save_file)
        {
            Galaxy galaxy = new();
            string jsonString = "";
            try
            {
                jsonString = File.ReadAllText(save_file);
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Failure when reading save file {save_file}: {exc.Message}");
                Environment.Exit(1);
            }
            galaxy = JsonSerializer.Deserialize<Galaxy>(jsonString)!;
            if (galaxy is null)
            {
                Console.WriteLine($"Failed to load from {save_file}");
                Environment.Exit(1);
            }
            for (int plrNum = 0; plrNum < numPlayers; plrNum++)
                galaxy.players[plrNum].InitPlayer(galaxy);
            for (int planNum = 0; planNum < numPlanets; planNum++)
                galaxy.planets[planNum].setGalaxy(galaxy);
            foreach (KeyValuePair<int, Ship> kvp in galaxy.ships)
            {
                kvp.Value.SetGalaxy(galaxy);
            }
            return galaxy;
        }
    }

}