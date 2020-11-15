using System.Collections.Generic;

namespace dice_roller.model
{
    class DiceToss
    {
        public string RequestedToss { get; set; }
        public List<int> Tosses { get; set; }
        public int Result { get; set; }

        public DiceToss()
        {
            Tosses = new List<int>();
        }

        public DiceToss(string dice)
        {
            RequestedToss = dice;
            Tosses = new List<int>();
        }
    }
}
