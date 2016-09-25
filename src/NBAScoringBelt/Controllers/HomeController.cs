using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NBAScoringBelt.Models;
using NBAScoringBelt.ViewModels;

namespace NBAScoringBelt.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(GenerateViewForYear(2016));
        }

        public ActionResult Year(int year)
        {
            return View("Index", GenerateViewForYear(year));
        }

        private HomeViewModel GenerateViewForYear(int year)
        {
            // Read in all of the year's games from the data set
            string statsFile = DetermineStatsFile(year);
            var lines = System.IO.File.ReadAllLines(HttpContext.Server.MapPath(string.Format("~/App_Data/{0}", statsFile)));
            var games = new List<Game>();

            int i = 0;

            foreach (var line in lines)
            {
                if (i > 0)
                {
                    games.Add(new Game(line));
                }

                i++;
            }

            // Calculate the player stats
            var playerStats = CalculatePlayerStats(games);

            // Calculate the team stats
            var teamStats = CalculateTeamStats(games);

            return new HomeViewModel(year, games, playerStats, teamStats);
        }

        private IEnumerable<Player> CalculatePlayerStats(IEnumerable<Game> games)
        {
            var playerStats = games.Where(g => !string.IsNullOrWhiteSpace(g.LeadingScorer))
                                   .GroupBy(g => g.LeadingScorer)
                                   .Select(p => new Player() { PlayerName = p.Key, TeamName = p.ToList().FirstOrDefault().LeadingScorerTeam, BeltsHeld = p.Count(), LastBeltWin = p.ToList().Max(lg => lg.GameDate) })
                                   .OrderByDescending(p => p.BeltsHeld)
                                   .ThenByDescending(p => p.LastBeltWin)
                                   .Take(5);

            return playerStats;
        }

        private IEnumerable<Team> CalculateTeamStats(IEnumerable<Game> games)
        {
            var teamStats = games.Where(g => !string.IsNullOrWhiteSpace(g.LeadingScorer))
                                 .GroupBy(t => t.LeadingScorerTeam)
                                 .Select(t => new Team() { TeamName = t.Key, BeltWinners = t.Count(), LastBeltWin = t.ToList().Max(lg => lg.GameDate) })
                                 .OrderByDescending(t => t.BeltWinners)
                                 .ThenByDescending(t => t.LastBeltWin)
                                 .Take(5);

            return teamStats;
        }

        private string DetermineStatsFile(int year)
        {
            switch (year)
            {
                case 2012:
                    return "scoring-belt-2011-2012.csv";
                case 2013:
                    return "scoring-belt-2012-2013.csv";
                case 2014:
                    return "scoring-belt-2013-2014.csv";
                case 2015:
                    return "scoring-belt-2014-2015.csv";
                case 2016:
                    return "scoring-belt-2015-2016.csv";
                case 2017:
                    return "scoring-belt-2016-2017.csv";
                default:
                    return null;
            }
        }
    }
}
