using System;
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
                case "load":
                    pc.TestLoad(game_number);
                    break;
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

        void TestLoad(int game_number)
        {
            string load_file = Path.Join(path, $"celemp_{game_number}.json");
            string save_file = Path.Join(path, $"celemp_{game_number}_a.json");

            Galaxy galaxy = new();
            galaxy.LoadGame(load_file);
            galaxy.SaveGame(save_file);
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

            Galaxy galaxy = new();
            galaxy.LoadGame(save_file);
            galaxy.GenerateTurnSheets();
        }

        void ProcessTurns(int game_number) {
            string save_file = Path.Join(path, $"celemp_{game_number}.json");
            Galaxy galaxy = new();
            galaxy.LoadGame(save_file);
            // TODO the processing of the turn
            galaxy.SaveGame(save_file);
        }
    }
}