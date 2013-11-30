using System.Collections.Generic;
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

            return View(new HomeViewModel(games));
        }
    }
}
