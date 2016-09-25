using System;
using System.Collections.Generic;

namespace NBAScoringBelt.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentBeltHolder = new PlayerGameStats()
            {
                PlayerName = "Russell Westbrook",
                PlayerTeam = "OKC"
            };

            // Load the schedule
            string resultsPath = "data/scoring-belt-2015-2016.csv";
            var lines = System.IO.File.ReadAllLines("data/schedule-2015-2016.csv");
            var schedule = new List<Schedule>();

            int i = 0;

            foreach (var line in lines)
            {
                if (i > 0)
                {
                    string[] values = line.Split(',');
                    DateTime gameDate = Convert.ToDateTime(values[0]);

                    if (gameDate <= DateTime.Today)
                    {
                        schedule.Add(new Schedule(line));
                    }
                    else
                    {
                        break;
                    }
                }

                i++;
            }

            var stats = new Stats();
            var beltHolderHistory = new List<PlayerGameStats>();

            foreach (var game in schedule)
            {
                if (game.HomeTeamAbbreviated == currentBeltHolder.PlayerTeam || game.AwayTeamAbbreviated == currentBeltHolder.PlayerTeam)
                {
                    if (game.HomeTeamPoints.HasValue && game.AwayTeamPoints.HasValue)
                    {
                        // This means we have found the game that will determine the next belt holder
                        string gameId = stats.GetGameId(game.GameDate, currentBeltHolder.PlayerTeam).Result;
                        var leadingScorer = stats.GetScoringLeader(gameId).Result;
                        leadingScorer.GameDate = game.GameDate;
                        leadingScorer.HomeTeam = game.HomeTeamAbbreviated;
                        leadingScorer.HomeTeamPoints = game.HomeTeamPoints.Value;
                        leadingScorer.AwayTeam = game.AwayTeamAbbreviated;
                        leadingScorer.AwayTeamPoints = game.AwayTeamPoints.Value;

                        beltHolderHistory.Add(leadingScorer);
                        currentBeltHolder = leadingScorer;

                        Console.WriteLine(game.GameDate.ToString("yyyy-MM-dd") + " - " + leadingScorer.PlayerName + " - " + leadingScorer.Points);
                    }
                    else
                    {
                        var nextBeltGame = new PlayerGameStats
                        {
                            GameDate = game.GameDate,
                            HomeTeam = game.HomeTeamAbbreviated,
                            AwayTeam = game.AwayTeamAbbreviated
                        };

                        beltHolderHistory.Add(nextBeltGame);
                        break;
                    }
                }
                //else if (game.GameDate > DateTime.Today)
                //{
                //    break;
                //}
            }

            Console.WriteLine("----------------------------------------------------");

            stats.WriteResultsToFile(resultsPath, beltHolderHistory).Wait();

            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("End of scoring belt analysis.");
            Console.ReadLine();
        }
    }

    public static class Converters
    {
        public static readonly Dictionary<string, string> TeamAbbreviationMap =
            new Dictionary<string, string>
            {
                { "Atlanta Hawks", "ATL" },
                { "Boston Celtics", "BOS" },
                { "Brooklyn Nets", "BKN" },
                { "Charlotte Hornets", "CHA" },
                { "Charlotte Bobcats", "CHA" },
                { "Chicago Bulls", "CHI" },
                { "Cleveland Cavaliers", "CLE" },
                { "Dallas Mavericks", "DAL" },
                { "Denver Nuggets", "DEN" },
                { "Detroit Pistons", "DET" },
                { "Golden State Warriors", "GSW" },
                { "Houston Rockets", "HOU" },
                { "Indiana Pacers", "IND" },
                { "Los Angeles Clippers", "LAC" },
                { "Los Angeles Lakers", "LAL" },
                { "Memphis Grizzlies", "MEM" },
                { "Miami Heat", "MIA" },
                { "Milwaukee Bucks", "MIL" },
                { "Minnesota Timberwolves", "MIN" },
                { "New Jersey Nets", "NJN" },
                { "New Orleans Hornets", "NOH" },
                { "New Orleans/Oklahoma City Hornets", "NOK" },
                { "New Orleans Pelicans", "NOP" },
                { "New York Knicks", "NYK" },
                { "Oklahoma City Thunder", "OKC" },
                { "Orlando Magic", "ORL" },
                { "Philadelphia 76ers", "PHI" },
                { "Phoenix Suns", "PHX" },
                { "Portland Trail Blazers", "POR" },
                { "Sacramento Kings", "SAC" },
                { "San Antonio Spurs", "SAS" },
                { "Seattle SuperSonics", "SEA" },
                { "Toronto Raptors", "TOR" },
                { "Utah Jazz", "UTA" },
                { "Vancouver Grizzlies", "VAN" },
                { "Washington Wizards", "WAS" },
            };
    }

    public class Schedule
    {
        public Schedule()
        {
        }

        public Schedule(string csv)
        {
            Bind(csv);
        }

        public DateTime GameDate { get; set; }

        public string HomeTeam { get; set; }

        public string HomeTeamAbbreviated
        {
            get
            {
                string value = null;

                if (Converters.TeamAbbreviationMap.TryGetValue(HomeTeam, out value))
                {
                    return value;
                }

                return value;
            }
        }

        public int? HomeTeamPoints { get; set; }

        public string AwayTeam { get; set; }

        public string AwayTeamAbbreviated
        {
            get
            {
                string value = null;

                if (Converters.TeamAbbreviationMap.TryGetValue(AwayTeam, out value))
                {
                    return value;
                }

                return value;
            }
        }

        public int? AwayTeamPoints { get; set; }

        private void Bind(string csv)
        {
            string[] values = csv.Split(',');
            GameDate = Convert.ToDateTime(values[0]);
            AwayTeam = values[2];

            if (!string.IsNullOrWhiteSpace(values[3]))
            {
                AwayTeamPoints = Convert.ToInt32(values[3]);
            }

            HomeTeam = values[4];

            if (!string.IsNullOrWhiteSpace(values[5]))
            {
                HomeTeamPoints = Convert.ToInt32(values[5]);
            }
        }
    }

    public class PlayerGameStats
    {
        public string PlayerName { get; set; }

        public string PlayerTeam { get; set; }

        public DateTime GameDate { get; set; }

        public string HomeTeam { get; set; }

        public int? HomeTeamPoints { get; set; }

        public string AwayTeam { get; set; }

        public int? AwayTeamPoints { get; set; }

        public int? Points { get; set; }

        public int? FieldGoalsMade { get; set; }

        public int? FieldGoalAttempts { get; set; }

        public int? ThreePointFieldGoalsMade { get; set; }

        public int? FreeThrowAttempts { get; set; }

        // eFG% = (FGM + .5*3PM)/FGA
        public decimal? EffectiveFieldGoalPercentage
        {
            get
            {
                if (FieldGoalAttempts.HasValue)
                {
                    if (FieldGoalAttempts > 0)
                    {
                        return (FieldGoalsMade.Value + 0.5M * ThreePointFieldGoalsMade.Value) / FieldGoalAttempts.Value;
                    }

                    return 0.00M;
                }
                else
                {
                    return null;
                }
            }
        }

        public string EffectiveFieldGoalPercentageFormatted
        {
            get
            {
                if (EffectiveFieldGoalPercentage.HasValue)
                {
                    return EffectiveFieldGoalPercentage.Value.ToString("#.###");
                }
                else
                {
                    return null;
                }
            }
        }

        // TS% = Pts/(2*(FGA + (.44*FTA)))
        public decimal TrueShootingPercentage
        {
            get
            {
                if (FieldGoalAttempts > 0 || FreeThrowAttempts > 0)
                {
                    return Points.Value / (2 * (FieldGoalAttempts.Value + (0.44M * FreeThrowAttempts.Value)));
                }

                return 0.00M;
            }
        }

        public string TrueShootingPercentageFormatted
        {
            get { return TrueShootingPercentage.ToString("#.###"); }
        }
    }
}
