using System;

namespace NBAScoringBelt.Models
{
    public class Team
    {
        public string TeamName
        {
            get;
            set;
        }

        public int BeltWinners
        {
            get;
            set;
        }

        public DateTime LastBeltWin
        {
            get;
            set;
        }
    }
}
