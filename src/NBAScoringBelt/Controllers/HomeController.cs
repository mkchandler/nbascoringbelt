using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBAScoringBelt.Models;
using NBAScoringBelt.ViewModels;

namespace NBAScoringBelt.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Read in all of the year's games from the data set
            var lines = System.IO.File.ReadAllLines(HttpContext.Server.MapPath("~/App_Data/2013.csv"));
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
            var playerStats = games.Where(g => !String.IsNullOrWhiteSpace(g.LeadingScorer))
                                   .GroupBy(g => g.LeadingScorer)
                                   .Select(p => new Player() { PlayerName = p.Key, TeamName = p.ToList().FirstOrDefault().LeadingScorerTeam, BeltsHeld = p.Count(), LastBeltWin = p.ToList().Max(lg => lg.GameDate) })
                                   .OrderByDescending(p => p.BeltsHeld)
                                   .ThenByDescending(p => p.LastBeltWin)
                                   .Take(5);

            // Calculate the team stats
            var teamStats = games.Where(g => !String.IsNullOrWhiteSpace(g.LeadingScorer))
                                 .GroupBy(t => t.LeadingScorerTeam)
                                 .Select(t => new Team() { TeamName = t.Key, BeltWinners = t.Count(), LastBeltWin = t.ToList().Max(lg => lg.GameDate) })
                                 .OrderByDescending(t => t.BeltWinners)
                                 .ThenByDescending(t => t.LastBeltWin)
                                 .Take(5);

            return View(new HomeViewModel(games, playerStats, teamStats));
        }
    }
}
