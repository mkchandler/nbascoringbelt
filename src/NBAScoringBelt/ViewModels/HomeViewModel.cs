using NBAScoringBelt.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NBAScoringBelt.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel(IEnumerable<Game> games)
        {
            _games = games;
        }

        public Game CurrentHolder
        {
            get
            {
                return _games.Where(g => !String.IsNullOrWhiteSpace(g.LeadingScorer)).OrderByDescending(g => g.GameDate).FirstOrDefault();
            }
        }

        public Game NextBeltGame
        {
            get
            {
                return _games.Where(g => String.IsNullOrWhiteSpace(g.LeadingScorer)).FirstOrDefault();
            }
        }

        private IEnumerable<Game> _games;

        public IEnumerable<Game> Games
        {
            get
            {
                return _games.Where(g => !String.IsNullOrWhiteSpace(g.LeadingScorer)).OrderByDescending(g => g.GameDate);
            }
        }
    }
}
