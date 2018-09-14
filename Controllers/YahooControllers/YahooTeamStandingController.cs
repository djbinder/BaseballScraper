using System;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    [Route("api/yahoo")]
    [ApiController]
    public class YahooTeamStandingController: Controller
    {
        private Helpers _h = new Helpers();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooHomeController _yahooHomeController;

        public YahooTeamStandingController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooHomeController yahooHomeController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooHomeController = yahooHomeController;
        }


        // TODO: most of this has been broken out into other methods; needs to be cleaned up
        /// <summary> Return instantiated 'YahooTeamStanding' </summary>
        /// <example> https://127.0.0.1:5001/api/yahoo/teamstanding </example>
        /// <returns> rank, playoff seed, games back, wins, losses, ties, winning percentage </returns>
        [HttpGet]
        [Route("teamstanding")]
        public YahooTeamStanding CreateYahooTeamStandingModel ()
        {
            _h.StartMethod();

            // retrieve the league key from user secrets / yahoo league config
            string leagueKey = _theGameConfig.LeagueKey;

            // create the uri that will be used to generate the appropriate json; in this case, it's the League Standings endpoint (view YahooApiEndPoints.cs)
            var uriLeagueStandings = endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;

            JObject leagueStandings = _yahooHomeController.GenerateYahooResourceJObject(uriLeagueStandings);
            int     teamsInLeague   = 10;

            YahooTeamStanding yS = new YahooTeamStanding();

            for(var teamId = 0; teamId <= teamsInLeague - 1; teamId++)
            {
                yS.Rank                     = endPoints.TeamItem(leagueStandings, 0, "Rank");
                yS.PlayoffSeed              = endPoints.TeamItem(leagueStandings, 0, "PlayoffSeed");
                yS.GamesBack                = endPoints.TeamItem(leagueStandings, 0, "GamesBack");
                yS.OutcomeTotals.Wins       = endPoints.TeamItem(leagueStandings, 0, "Wins");
                yS.OutcomeTotals.Losses     = endPoints.TeamItem(leagueStandings, 0, "Losses");
                yS.OutcomeTotals.Ties       = endPoints.TeamItem(leagueStandings, 0, "Ties");
                yS.OutcomeTotals.Percentage = endPoints.TeamItem(leagueStandings, 0, "WinningPercentage");

                Console.WriteLine($"TEAM STANDINGS FOR TEAM ID {teamId}");
                Console.WriteLine(yS);
                Console.WriteLine();
            }
            // int X = 0;

            // var leagueStandingsTeamsTeamX = leagueStandings["fantasy_content"]["league"]["standings"]["teams"]["team"][X];

            // Console.WriteLine(leagueStandingsTeamsTeamX["team_stats"].GetType());

            return yS;
        }
    }
}