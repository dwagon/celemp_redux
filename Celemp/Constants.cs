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

        public const string resolve_attack = "RESOLVEATTACK";
        public const string end_contracting = "ENDCONTRACTING";
        public const string cargo_pdu = "PDU";
        public const string cargo_mine = "Mine";
        public const string cargo_industry = "Industry";
        public const string cargo_spacemine = "Spacemine";
    }
}