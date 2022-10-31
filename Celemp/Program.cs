using System.Text.Json;

namespace Celemp
{
    class Program
    {
        readonly string path = "/Users/dwagon";
        
        static void Main(string[] args)
        {
            int game_number;

            if (args.Length != 2) {
                PrintUsage();
                System.Environment.Exit(1);
            }
            Program pc = new Program();
            game_number = Int16.Parse(args[1]);

            switch (args[0])
            {
                case "new":
                    pc.NewGame(game_number);
                    break;
                case "turn":
                    pc.ProcessTurns(game_number);
                    break;
                case "sheet":
                    pc.GenerateTurnSheets(game_number);
                    break;
                default:
                    Console.WriteLine($"Unknown argument {args[0]}");
                    break;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("celemp new <gamenum>");
            Console.WriteLine("celemp turn <gamenum>");
            Console.WriteLine("celemp sheet <gamenum>");
        }

        void NewGame(int game_number) {
            string save_file = Path.Join(path, $"celemp_{game_number}.json");
            Config config = new(Path.Join(path, "protofile"));
            Galaxy galaxy = new();

            Console.WriteLine($"New galaxy {game_number}");
            galaxy.InitGalaxy(config);
            galaxy.SaveGame(save_file);
        }

        void GenerateTurnSheets(int game_number) {
            string save_file = Path.Join(path, $"celemp_{game_number}.json");

            Galaxy galaxy = LoadGame(save_file);
            galaxy.SaveGame($"/Users/dwagon/celemp_{game_number}_a.json");  // DEBUG
            galaxy.GenerateTurnSheets();
        }

        void ProcessTurns(int game_number) {
            string save_file = Path.Join(path, $"celemp_{game_number}.json");
            Galaxy galaxy = LoadGame(save_file);
            // TODO the processing of the turn
            galaxy.SaveGame(save_file);
        }

        public Galaxy LoadGame(string save_file)
        {
            Galaxy galaxy = new();
            string jsonString = File.ReadAllText(save_file);
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