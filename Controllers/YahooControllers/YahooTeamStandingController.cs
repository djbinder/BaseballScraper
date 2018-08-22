using System;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    public class YahooTeamStandingController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static ApiEndPoints endPoints = new ApiEndPoints();
        private static YahooHomeController _yahooHomeController;

        public YahooTeamStandingController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooHomeController yahooHomeController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooHomeController = yahooHomeController;
        }



        // standings model returns: rank, playoff seed, games back, wins, losses, ties, winning percentage
        [HttpGet]
        [Route("yahoo/teamstanding/create")]
        public YahooTeamStanding CreateYahooTeamStandingModel ()
        {
            Start.ThisMethod();

            // retrieve the league key from user secrets / yahoo league config
            string leagueKey = _theGameConfig.LeagueKey;

            // create the uri that will be used to generate the appropriate json; in this case, it's the League Standings endpoint (view ApiEndPoints.cs)
            var uriLeagueStandings = endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;

            JObject           leagueStandings = _yahooHomeController.GenerateYahooResourceJObject(uriLeagueStandings);
            YahooTeamStanding yS              = new YahooTeamStanding();

            yS.Rank                     = endPoints.TeamItem(leagueStandings, 0, "Rank");
            yS.PlayoffSeed              = endPoints.TeamItem(leagueStandings, 0, "PlayoffSeed");
            yS.GamesBack                = endPoints.TeamItem(leagueStandings, 0, "GamesBack");
            yS.OutcomeTotals.Wins       = endPoints.TeamItem(leagueStandings, 0, "Wins");
            yS.OutcomeTotals.Losses     = endPoints.TeamItem(leagueStandings, 0, "Losses");
            yS.OutcomeTotals.Ties       = endPoints.TeamItem(leagueStandings, 0, "Ties");
            yS.OutcomeTotals.Percentage = endPoints.TeamItem(leagueStandings, 0, "WinningPercentage");

            // STANDINGS PATH type ---> Newtonsoft.Json.Linq.JObject
                // starts with "teams"
            // var standingsPath = leagueStandings["fantasy_content"]["league"]["standings"];
            // standingsPath.Intro("standings path");
                // STANDINGS PATH CHILDREN type ---> Newtonsoft.Json.Linq.JEnumerable`1[Newtonsoft.Json.Linq.JToken]
                    // Count is 1 for children
                    // var standingsPathChildren = standingsPath.Children();

            // TEAMS PATH type ---> Newtonsoft.Json.Linq.JObject
                // 2 Children
                    // @count, 'team'
            // var leagueStandingsTeams = leagueStandings["fantasy_content"]["league"]["standings"]["teams"];

            // TEAM PATH type ---> Newtonsoft.Json.Linq.JArray
                // 10 Children
                    // I believe this is for each team
            var leagueStandingsTeamsTeam = leagueStandings["fantasy_content"]["league"]["standings"]["teams"]["team"];

            int X = 0;

            // TEAM PATH X type ---> Newtonsoft.Json.Linq.JObject
                // 16 Children

            var leagueStandingsTeamsTeamX = leagueStandings["fantasy_content"]["league"]["standings"]["teams"]["team"][X];

                // foreach(var y in leagueStandingsTeamsTeamX)
                // {
                //     Console.WriteLine(y.GetType());
                // }

                // JObjects
                // Console.WriteLine(leagueStandingsTeamsTeamX["team_logos"].GetType());
                // Console.WriteLine(leagueStandingsTeamsTeamX["roster_adds"].GetType());
                // Console.WriteLine(leagueStandingsTeamsTeamX["managers"].GetType());
                Console.WriteLine(leagueStandingsTeamsTeamX["team_stats"].GetType());
                // Console.WriteLine(leagueStandingsTeamsTeamX["team_points"].GetType());
                // Console.WriteLine(leagueStandingsTeamsTeamX["team_standings"].GetType());

            // TEAM POINTS PATH type ---> Newtonsoft.Json.Linq.JObject
                // 3 Children
                    // coverage type, season, total
            // var teamPointsPath = leagueStandings["fantasy_content"]["league"]["standings"]["teams"]["team"][0]["team_points"];
                // CreateYahooTeamPointsModel(teamPointsPath);
            return yS;
        }
    }
}