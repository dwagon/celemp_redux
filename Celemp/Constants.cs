using System;
namespace Celemp
{
    public static class Constants
    {
        public const int numPlayers = 10;
        public const int numPlanets = 256;
        public const int numOreTypes = 10;
        public static readonly int[,] driveEfficiency = {
            { 2, 1, 1, 1, 1, 1
            },
            { 8, 4, 2, 2, 2, 2
            },
            { 18, 9, 6, 3, 3, 3
            },
            { 32, 16, 12, 8, 4, 4
            },
            { 50, 25, 20, 15, 10, 5
            }
        };
    }
}