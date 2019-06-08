using System;
using BaseballScraper.Models.Configuration;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using BaseballScraper.Infrastructure;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooTeamStatsController: ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static readonly YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController _yahooApiRequestController;

        public YahooTeamStatsController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooApiRequestController yahooApiRequestController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooApiRequestController = yahooApiRequestController;
        }

        /// <summary>
        // FILE NOTE: provides three options on how to instantiate new instance of model
            // OPTION 1 - endpoint parameters defined within the method
            // OPTION 2 - endpoint parameters passed as parameters to method
            // OPTION 3 - endpoint parameters are appended to the Url
        /// </summary>



        // OPTION 1: endpoint parameters defined within the method
        /// <summary> Create new instance of YahooTeamStats Model; Show full season stats for one team </summary>
        /// <remarks> Within Option 1, the 'teamId' is defined within the method itself; to change the team Id you're searching for, change the 'teamId' variable; </remarks>
        /// <example> https://127.0.0.1:5001/api/yahoo/teamstats/season </example>
        /// <returns> new YahooTeamStatsModel </returns>
        ///     <remarks> Includes: H/AB, R, HR, RBI, SB, BB, IP, W, SV, H, ERA, WHIP </remarks>
        [HttpGet]
        [Route("season")]
        public YahooTeamStatsList CreateYahooTeamStatsModel()
        {
            _h.Spotlight("executing create yahoo team stats method option 1");
            int teamId      = 1;
            var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject teamStatsJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);
            // Extensions.PrintJObjectItems(teamStatsJson);

            YahooTeamStatsList tS = new YahooTeamStatsList();

                var teamStatsPath = teamStatsJson["fantasy_content"]["team"]["team_stats"];

                // STAT PATH type: Newtonsoft.Json.Linq.JArray
                var statPath = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"];

                int statIdIndexLocation = 0;

                    tS.HitsDividedByAtBatsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.RunsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.HomeRunsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.RunsBattedInId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.StolenBasesId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WalksId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.BattingAverageId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.InningsPitchedId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WinsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.StrikeoutsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.SavesId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.HoldsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.EarnedRunAverageId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WhipId = statPath[statIdIndexLocation]["stat_id"].ToString();

                int statValueIndexLocation = 0;

                    tS.HitsDividedByAtBatsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.RunsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.HomeRunsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.RunsBattedInTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.StolenBasesTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WalksTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.BattingAverageTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.InningsPitchedTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WinsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.StrikeoutsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.SavesTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.HoldsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.EarnedRunAverageTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WhipTotal = statPath[statValueIndexLocation]["value"].ToString();

            // tS.Dig();

            return tS;
        }

        // OPTION 2: endpoint parameters passed as parameters to method (i.e., 'teamId')
        /// <summary> Create new instance of YahooTeamStats Mode; Show full season stats for one team </summary>
        /// <remarks> Within Option 2, there is one parameter passed into the method - the 'teamId'; This option must be called by another method. To change the team you want to view the stats for, change the id number you are passing when calling the method </remarks>
        /// <param name="teamId"> A number between 0 and the total number of teams in the league with each number in between representing one of the teams (i.e., each team has its own unique team id number) </param>
        /// <example> CreateYahooTeamStatsModel(1) </example>
        /// <returns> new YahooTeamStatsModel </returns>
        ///     <remarks> Includes: H/AB, R, HR, RBI, SB, BB, IP, W, SV, H, ERA, WHIP </remarks>
        public YahooTeamStatsList CreateYahooTeamStatsModel(int teamId)
        {
            _h.Spotlight("executing create yahoo team stats method option 2");
            var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject teamStatsJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);
            // Extensions.PrintJObjectItems(teamStatsJson);

            YahooTeamStatsList tS = new YahooTeamStatsList();

                var teamStatsPath = teamStatsJson["fantasy_content"]["team"]["team_stats"];

                // STAT PATH type: Newtonsoft.Json.Linq.JArray
                var statPath = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"];

                int statIdIndexLocation = 0;

                    tS.HitsDividedByAtBatsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.RunsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.HomeRunsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.RunsBattedInId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.StolenBasesId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WalksId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.BattingAverageId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.InningsPitchedId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WinsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.StrikeoutsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.SavesId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.HoldsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.EarnedRunAverageId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WhipId = statPath[statIdIndexLocation]["stat_id"].ToString();

                int statValueIndexLocation = 0;

                    tS.HitsDividedByAtBatsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.RunsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.HomeRunsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.RunsBattedInTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.StolenBasesTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WalksTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.BattingAverageTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.InningsPitchedTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WinsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.StrikeoutsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.SavesTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.HoldsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.EarnedRunAverageTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WhipTotal = statPath[statValueIndexLocation]["value"].ToString();

            // tS.Dig();

            return tS;
        }

        // OPTION 2B: View for Option 2
        /// <summary> This allows viewing / testing of Option 2; The method is called and a team id is passed as a parameter </summary>
        /// <example> https://127.0.0.1:5001/api/yahoo/teamstats </example>
        /// <returns> A view of team stats for a selected team </returns>
        [HttpGet]
        [Route("")]
        public IActionResult ViewTeamStats()
        {
            int teamId = 1;
            CreateYahooTeamStatsModel(teamId);

            string currently = $"SEARCHING FOR TEAM ID: {teamId}";
            return Content(currently);
        }

        // TODO: Figure out how to differentiate Option 2 and Option 3 so that 'blankString' is not needed to differentiate the two
        // OPTION 3: endpoint parameters are appended to the Url
        /// <summary> Create new instance of YahooTeamStats Mode; Show full season stats for one team </summary>
        /// <remarks> Within Option 3, there are two parameters. To change the team you are searching for, change the team id within the url itself </remarks>
        /// <param name="teamId"> A number between 0 and the total number of teams in the league with each number in between representing one of the teams (i.e., each team has its own unique team id number) </param>
        /// <param name="blankString"> This parameter doesn't actually do anything; needed to make this method different than the previous method; there is probably a better way to do this </param>
        /// <example> https://127.0.0.1:5001/api/yahoo/teamstats/season/2 </example>
        /// <returns> new YahooTeamStatsModel </returns>
        ///     <remarks> Includes: H/AB, R, HR, RBI, SB, BB, IP, W, SV, H, ERA, WHIP </remarks>
        [HttpGet]
        [Route("season/{teamId}")]
        public YahooTeamStatsList CreateYahooTeamStatsModel(int teamId, string blankString)
        {
            _h.Spotlight("executing create yahoo team stats method option 3");
            var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject teamStatsJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);
            // Extensions.PrintJObjectItems(teamStatsJson);

            YahooTeamStatsList tS = new YahooTeamStatsList();

                var teamStatsPath = teamStatsJson["fantasy_content"]["team"]["team_stats"];

                // STAT PATH type: Newtonsoft.Json.Linq.JArray
                var statPath = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"];

                int statIdIndexLocation = 0;

                    tS.HitsDividedByAtBatsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.RunsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.HomeRunsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.RunsBattedInId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.StolenBasesId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WalksId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.BattingAverageId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.InningsPitchedId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WinsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.StrikeoutsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.SavesId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.HoldsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.EarnedRunAverageId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WhipId = statPath[statIdIndexLocation]["stat_id"].ToString();

                int statValueIndexLocation = 0;

                    tS.HitsDividedByAtBatsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.RunsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.HomeRunsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.RunsBattedInTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.StolenBasesTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WalksTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.BattingAverageTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.InningsPitchedTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WinsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.StrikeoutsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.SavesTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.HoldsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.EarnedRunAverageTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WhipTotal = statPath[statValueIndexLocation]["value"].ToString();

            // tS.Dig();

            return tS;
        }
    }
}
