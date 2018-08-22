using System;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    public class YahooTeamStatsController: Controller
    {

        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static ApiEndPoints endPoints = new ApiEndPoints();
        private static YahooHomeController _yahooController;

        public YahooTeamStatsController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooHomeController yahooHomeController)
        {
            _theGameConfig   = theGameConfig.Value;
            _yahooController = yahooHomeController;
        }



        /// <summary> Create new instance of YahooTeamStats Model</summary>
        /// <returns>YahooTeamStatsModel</returns>
        [HttpGet]
        [Route("yahoo/teamstats/create")]
        public YahooTeamStatsList CreateYahooTeamStatsModel()
        {
            int teamId      = 1;
            var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject teamStatsJson = _yahooController.GenerateYahooResourceJObject(uriTeamBase);
            // Extensions.PrintJObjectItems(teamStatsJson);

            YahooTeamStatsList tS = new YahooTeamStatsList();

                var teamStatsPath = teamStatsJson["fantasy_content"]["team"]["team_stats"];

                // the search can return season or week stats; the coverage type will differ based on which one you want
                // tS.StatCoverageType = teamStatsPath["coverage_type"].ToString();
                //     if(tS.StatCoverageType == "season")
                //         tS.Season = teamStatsPath["season"].ToString();
                //     if(tS.StatCoverageType == "week")
                //         tS.WeekNumber = teamStatsPath["week"].ToString();

                // STAT PATH type: Newtonsoft.Json.Linq.JArray
                var statPath = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"];

                int statIdIndexLocation = 0;

                    tS.HitsAtBatsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.RunsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.HomeRunsId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.RbiId = statPath[statIdIndexLocation]["stat_id"].ToString();
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

                    tS.EraId = statPath[statIdIndexLocation]["stat_id"].ToString();
                    statIdIndexLocation++;

                    tS.WhipId = statPath[statIdIndexLocation]["stat_id"].ToString();

                int statValueIndexLocation = 0;

                    tS.HitsAtBatsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.RunsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.HomeRunsTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.RbiTotal = statPath[statValueIndexLocation]["value"].ToString();
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

                    tS.EraTotal = statPath[statValueIndexLocation]["value"].ToString();
                    statValueIndexLocation++;

                    tS.WhipTotal = statPath[statValueIndexLocation]["value"].ToString();

            // tS.Dig();

            return tS;
        }
    }
}