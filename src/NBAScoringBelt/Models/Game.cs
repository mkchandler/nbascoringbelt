using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBAScoringBelt.Models
{
    public class Game
    {
        public Game()
        {
        }

        public Game(string csv)
        {
            Bind(csv);
        }

        public DateTime GameDate
        {
            get;
            set;
        }

        public string AwayTeam
        {
            get;
            set;
        }

        public int? AwayTeamPoints
        {
            get;
            set;
        }

        public bool AwayTeamWinner
        {
            get
            {
                return AwayTeamPoints > HomeTeamPoints;
            }
        }

        public string HomeTeam
        {
            get;
            set;
        }

        public int? HomeTeamPoints
        {
            get;
            set;
        }

        public bool HomeTeamWinner
        {
            get
            {
                return HomeTeamPoints > AwayTeamPoints;
            }
        }

        public string LeadingScorer
        {
            get;
            set;
        }

        public string LeadingScorerTeam
        {
            get;
            set;
        }

        public int? LeadingScorerPoints
        {
            get;
            set;
        }

        public decimal? LeadingScorerEFGPercentage
        {
            get;
            set;
        }

        private void Bind(string csv)
        {
            string[] values = csv.Split(',');
            GameDate = Convert.ToDateTime(values[0]);
            AwayTeam = values[1];
            AwayTeamPoints = String.IsNullOrWhiteSpace(values[2]) ? default(int?) : Convert.ToInt32(values[2]);
            HomeTeam = values[3];
            HomeTeamPoints = String.IsNullOrWhiteSpace(values[4]) ? default(int?) : Convert.ToInt32(values[4]);
            LeadingScorer = values[5];
            LeadingScorerTeam = values[6];
            LeadingScorerPoints = String.IsNullOrWhiteSpace(values[7]) ? default(int?) : Convert.ToInt32(values[7]);
            LeadingScorerEFGPercentage = String.IsNullOrWhiteSpace(values[8]) ? default(decimal?) : Convert.ToDecimal(values[8]);
        }
    }
}