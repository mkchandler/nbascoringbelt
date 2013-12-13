using System;

namespace NBAScoringBelt.Models
{
    public class Player
    {
        public string PlayerName
        {
            get;
            set;
        }

        public string TeamName
        {
            get;
            set;
        }

        public int BeltsHeld
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
