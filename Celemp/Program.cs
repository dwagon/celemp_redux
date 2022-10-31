using System.Text.Json;

namespace Celemp
{
    class Program
    {        
        static void Main(string[] args)
        {
            int game_number;

            if (args.Length != 2) {
                PrintUsage();
                System.Environment.Exit(1);
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
                    pc.ProcessTurns(game_number, game_path);
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

        void NewGame(string celemp_path) {
            string save_file = Path.Join(celemp_path, "celemp.json");
            Config config = new(Path.Join(celemp_path, "protofile"));
            Galaxy galaxy = new();

            Console.WriteLine("New galaxy");
            galaxy.InitGalaxy(config);
            galaxy.SaveGame(save_file);
        }

        void GenerateTurnSheets(int game_number, string celemp_path) {
            string save_file = Path.Join(celemp_path, "celemp.json");

            Galaxy galaxy = LoadGame(save_file);
            galaxy.GenerateTurnSheets(celemp_path);
        }

        void ProcessTurns(int game_number, string celemp_path) {
            string save_file = Path.Join(celemp_path, "celemp.json");
            Galaxy galaxy = LoadGame(save_file);
            // TODO the processing of the turn
            galaxy.SaveGame(save_file);
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
                System.Environment.Exit(1);
            }
            for (int plrNum = 0; plrNum < 9; plrNum++)
                galaxy.players[plrNum].InitPlayer(galaxy);
            for (int planNum = 0; planNum < 256; planNum++)
                galaxy.planets[planNum].setGalaxy(galaxy);
            foreach (KeyValuePair<int, Ship> kvp in galaxy.ships)
            {
                kvp.Value.SetGalaxy(galaxy);
            }
            return galaxy;
        }
    }

}