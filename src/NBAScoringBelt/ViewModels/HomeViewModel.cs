using NBAScoringBelt.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NBAScoringBelt.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel(int year, IEnumerable<Game> games, IEnumerable<Player> beltHolderStats, IEnumerable<Team> teamStats)
        {
            Year = year;
            _games = games;
            _beltHolderStats = beltHolderStats;
            _teamStats = teamStats;
        }

        public int Year
        {
            get;
            set;
        }

        public Game CurrentHolder
        {
            get
            {
                return _games.Where(g => !String.IsNullOrWhiteSpace(g.LeadingScorer)).OrderByDescending(g => g.GameDate).FirstOrDefault();
            }
        }

        public bool IsEndOfSeason
        {
            get
            {
                return NextBeltGame == null;
            }
        }

        public Game NextBeltGame
        {
            get
            {
                return _games.Where(g => String.IsNullOrWhiteSpace(g.LeadingScorer)).FirstOrDefault();
            }
        }

        private IEnumerable<Player> _beltHolderStats;

        public IEnumerable<Player> BeltHolderStats
        {
            get
            {
                return _beltHolderStats;
            }
        }

        private IEnumerable<Team> _teamStats;

        public IEnumerable<Team> TeamStats
        {
            get
            {
                return _teamStats;
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
