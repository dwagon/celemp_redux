using System;
using System.IO;

namespace Celemp
{
    public class Protofile
    {
        public int earthMult;
        public int earthInd;
        public int earthPDU;
        public int earthSpacemine;
        public int earthDeployed;
        public int[] earthOre = new int[10];
        public int[] earthMines = new int[10];
        public int earthFlag;
        public int amnesty;
        public int winning_score;
        public int winning_turns;
        public int homeIndustry;
        public int homePDU;
        public int homeSpacemine;
        public int homeDeployed;
        public int[] homeOre = new int[10];
        public int[] homeMines = new int[10];
        public int galHasInd;
        public int galHasPDU;
        public int galNoMines;
        public int galExtraMines;
        public int galExtraOre;
        public int ship1_num, ship2_num;
        public int ship1_fight, ship2_fight;
        public int ship1_cargo, ship2_cargo;
        public int ship1_shield, ship2_shield;
        public int ship1_tractor, ship2_tractor;
        public int ship1_eff, ship2_eff;

        public Protofile()
        {
            earthMult = 1;
            earthInd = 60;
            earthPDU = 200;
            earthSpacemine = 0;
            earthDeployed = 0;
            earthFlag = 0;
            for (int oretype = 0; oretype < 10; oretype++)
            {
                earthOre[oretype] = 10;
                earthMines[oretype] = 0;
            }

            amnesty = 10;
            winning_score = 10000;
            winning_turns = 30;

            homeIndustry = 60;
            homePDU = 100;
            homeSpacemine = 0;
            homeDeployed = 0;
            homeOre = new int[10] { 100, 30, 30, 20, 25, 15, 15, 15, 50, 50 };
            homeMines = new int[10] { 5, 3, 3, 1, 2, 1, 1, 0, 0, 0 };

            galHasInd = 30;
            galHasPDU = 10;
            galNoMines = 1;
            galExtraMines = 1;
            galExtraOre = 20;

            ship1_num = 4;
            ship1_fight = 0;
            ship1_cargo = 10;
            ship1_shield = 5;
            ship1_tractor = 0;
            ship1_eff = 0;

            ship2_num = 1;
            ship2_fight = 20;
            ship2_cargo = 5;
            ship2_shield = 0;
            ship2_tractor = 0;
            ship2_eff = 1;
        }

        public void LoadProto(string filename)
        {
        string text = File.ReadAllText(filename);

        using (StringReader sr = new StringReader(text))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    string[] tokens = line.Split('=');
                    string[] bits = tokens[1].Split(' ');

                    switch (tokens[0].ToLower())
                    {
                        case "earth_mult":
                            earthMult = Convert.ToInt16(tokens[1]);
                            break;
                        case "earth_spcmine":
                            earthSpacemine = Convert.ToInt16(tokens[1]);
                            break;
                        case "earth_deployed":
                            earthDeployed = Convert.ToInt16(tokens[1]);
                            break;
                        case "earth_ind":
                            earthInd = Convert.ToInt16(tokens[1]);
                            break;
                        case "earth_ore":
                            for (int ore_type = 0; ore_type < 10; ore_type++)
                            {
                                earthOre[ore_type] = Convert.ToInt16(bits[ore_type]);
                            }
                            break;
                        case "earth_mine":
                            for (int ore_type = 0; ore_type < 10; ore_type++)
                            {
                                earthMines[ore_type] = Convert.ToInt16(bits[ore_type]);
                            }
                            break;
                        case "earth_pdu":
                            earthPDU = Convert.ToInt16(tokens[1]);
                            break;
                        case "amnesty":
                            amnesty = Convert.ToInt16(tokens[1]);
                            break;
                        case "winning_score":
                            winning_score = Convert.ToInt16(tokens[1]);
                            break;
                        case "winning_turns":
                            winning_turns = Convert.ToInt16(tokens[1]);
                            break;
                        case "home_ind":
                            homeIndustry = Convert.ToInt16(tokens[1]);
                            break;
                        case "home_pdu":
                            homePDU = Convert.ToInt16(tokens[1]);
                            break;
                        case "home_spcmine":
                            homeSpacemine = Convert.ToInt16(tokens[1]);
                            break;
                        case "home_deployed":
                            homeDeployed = Convert.ToInt16(tokens[1]);
                            break;
                        case "home_ore":
                            for (int ore_type = 0; ore_type < 10; ore_type++)
                            {
                                homeOre[ore_type] = Convert.ToInt16(bits[ore_type]);
                            }
                            break;
                        case "home_mine":
                            for (int ore_type = 0; ore_type < 10; ore_type++)
                            {
                                homeMines[ore_type] = Convert.ToInt16(bits[ore_type]);
                            }
                            break;
                        case "gal_no_mines":
                            galNoMines = Convert.ToInt16(tokens[1]);
                            break;
                        case "gal_extra_mines":
                            galExtraMines = Convert.ToInt16(tokens[1]);
                            break;
                        case "gal_has_ind":
                            galHasInd = Convert.ToInt16(tokens[1]);
                            break;
                        case "gal_has_pdu":
                            galHasPDU = Convert.ToInt16(tokens[1]);
                            break;
                        case "gal_extra_ore":
                            galExtraOre = Convert.ToInt16(tokens[1]);
                            break;
                        case "ship1_num":
                            ship1_num = Convert.ToInt16(tokens[1]);
                            break;
                        case "ship2_num":
                            ship2_num = Convert.ToInt16(tokens[1]);
                            break;
                        case "ship1_fight":
                            ship1_fight = Convert.ToInt16(tokens[1]);
                            break;
                        case "ship2_fight":
                            ship2_fight = Convert.ToInt16(tokens[1]);
                            break;
                        case "ship1_cargo":
                            ship1_cargo = Convert.ToInt16(tokens[1]);
                            break;
                        case "ship2_cargo":
                            ship2_cargo = Convert.ToInt16(tokens[1]);
                            break;
                        case "ship1_shield":
                            ship1_shield = Convert.ToInt16(tokens[1]);
                            break;
                        case "ship2_shield":
                            ship2_shield = Convert.ToInt16(tokens[1]);
                            break;
                        default:
                            Console.WriteLine($"LoadProto: Unknown token {tokens[0]}");
                            break;
                    }
                }
            }
        }
    }
}