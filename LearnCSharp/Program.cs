using System;
using System.Text.Json;

namespace Celemp
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            Protofile proto = new Protofile();
            proto.LoadProto("/Users/dwagon/protofile");

            Galaxy galaxy = new Galaxy(proto);
            Console.Write("Celemp\n");

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(galaxy, options);
            File.WriteAllText("/Users/dwagon/celemp.json", jsonString);
        }

    }
}