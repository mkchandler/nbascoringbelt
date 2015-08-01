using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NBAScoringBelt.Cmd
{
    public class Stats
    {
        public async Task<string> GetGameId(DateTime date, string teamAbbreviation)
        {
            var gamesData = await GetGamesByDate(date);

            JObject jObject = JObject.Parse(gamesData);

            foreach (var game in jObject["resultSets"][0]["rowSet"].ToList())
            {
                var values = game.Values().ToArray();

                if (values[5].ToString().IndexOf(teamAbbreviation, 0, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return values[2].ToString();
                }
            }

            return null;
        }

        public async Task<PlayerGameStats> GetScoringLeader(string gameId)
        {
            var gameData = await GetGameById(gameId);

            JObject jObject = JObject.Parse(gameData);
            var allPlayers = new List<PlayerGameStats>();

            foreach (var playerStatsJson in jObject["resultSets"][4]["rowSet"].ToList())
            {
                var values = playerStatsJson.Values().ToArray();
                var playerStats = new PlayerGameStats();
                playerStats.PlayerName = values[5].ToString();
                playerStats.PlayerTeam = values[2].ToString();
                playerStats.Points = string.IsNullOrWhiteSpace(values[26].ToString()) ? 0 : Convert.ToInt32(values[26]);
                playerStats.FieldGoalsMade = string.IsNullOrWhiteSpace(values[9].ToString()) ? 0 : Convert.ToInt32(values[9]);
                playerStats.FieldGoalAttempts = string.IsNullOrWhiteSpace(values[10].ToString()) ? 0 : Convert.ToInt32(values[10]);
                playerStats.ThreePointFieldGoalsMade = string.IsNullOrWhiteSpace(values[12].ToString()) ? 0 : Convert.ToInt32(values[12]);
                playerStats.FreeThrowAttempts = string.IsNullOrWhiteSpace(values[16].ToString()) ? 0 : Convert.ToInt32(values[16]);

                allPlayers.Add(playerStats);
            }

            var topScorer = allPlayers.OrderByDescending(p => p.Points)
                                      .ThenByDescending(p => p.EffectiveFieldGoalPercentage)
                                      .ThenByDescending(p => p.TrueShootingPercentage)
                                      .FirstOrDefault();

            return topScorer;
        }

        private async Task<string> GetGameById(string gameId)
        {
            string filePath = string.Format("data/game-{0}.json", gameId);

            if (File.Exists(filePath))
            {
                // If we have already downloaded the file, just read it from the file system
                return await ReadFileAsync(filePath);
            }
            else
            {
                // Download the file if we don't already have it
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://stats.nba.com/stats/");

                var response = await httpClient.GetAsync(string.Format("boxscore?GameID={0}&RangeType=0&StartPeriod=0&EndPeriod=0&StartRange=0&EndRange=0", gameId)).ConfigureAwait(false);
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                await WriteFileAsync(filePath, responseString);

                return responseString;
            }
        }

        private async Task<string> GetGamesByDate(DateTime date)
        {
            string filePath = string.Format("data/games-{0}.json", date.ToString("yyyy-MM-dd"));

            if (File.Exists(filePath))
            {
                // If we have already downloaded the file, just read it from the file system
                return await ReadFileAsync(filePath);
            }
            else
            {
                // Download the file if we don't already have it
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://stats.nba.com/stats/");

                var response = await httpClient.GetAsync(string.Format("scoreboard?LeagueID=00&gameDate={0}&DayOffset=0", date.ToString("M/d/yyyy"))).ConfigureAwait(false);
                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                await WriteFileAsync(filePath, responseString);

                return responseString;
            }
        }

        public async Task WriteResultsToFile(string filePath, IEnumerable<PlayerGameStats> scoringBeltHistory)
        {
            var fileContents = new StringBuilder();
            fileContents.AppendLine("Game_Date,Away_Team,Away_Team_Points,Home_Team,Home_Team_Points,Leading_Scorer,Leading_Scorer_Team,Leading_Scorer_Points,Leading_Scorer_EFG");

            foreach (var scoringBeltStat in scoringBeltHistory)
            {
                fileContents.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", scoringBeltStat.GameDate.ToString("yyyy-MM-dd"),
                                                                                             scoringBeltStat.AwayTeam,
                                                                                             scoringBeltStat.AwayTeamPoints,
                                                                                             scoringBeltStat.HomeTeam,
                                                                                             scoringBeltStat.HomeTeamPoints,
                                                                                             scoringBeltStat.PlayerName,
                                                                                             scoringBeltStat.PlayerTeam,
                                                                                             scoringBeltStat.Points,
                                                                                             scoringBeltStat.EffectiveFieldGoalPercentageFormatted));
            }

            Console.WriteLine("Writing results to " + filePath);

            await WriteFileAsync(filePath, fileContents.ToString());
        }

        private async Task WriteFileAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.UTF8.GetBytes(text);

            using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await fileStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }

        private async Task<string> ReadFileAsync(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            {
                var contents = new StringBuilder();

                byte[] buffer = new byte[0x1000];
                int bytesRead;

                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    contents.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                }

                return contents.ToString();
            }
        }
    }
}
