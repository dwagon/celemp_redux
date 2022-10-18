using System;
namespace Celemp
{
    [Serializable]
    public class Player
    {
        public int score { get; set; }
        public int number { get; set; }
        public int earthCredit { get; set; }
        public int desturn { get; set; }

        public Player(Protofile proto, int plr_num)
        {
            score = 0;
            number = plr_num;
            earthCredit = 0;
            desturn = 30;
        }
    }
}