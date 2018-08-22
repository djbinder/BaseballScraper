using System;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    public class YahooTeamPointsController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static ApiEndPoints endPoints = new ApiEndPoints();
        private static YahooHomeController _yahooHomeController;

        public YahooTeamPointsController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooHomeController yahooController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooHomeController = yahooController;
        }


        [Route("yahoo/teampoints/create")]
        // need to confirm this works
        public YahooTeamPoints CreateYahooTeamPointsModel (JObject o, int teamId)
        {
            Start.ThisMethod();
            YahooTeamPoints yTP = new YahooTeamPoints();

            var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject teamStatsJson = _yahooHomeController.GenerateYahooResourceJObject(uriTeamBase);

            var teamPointsPath         = o["fantasy_content"]["team"]["team_points"];
            var teamPointsCoverageType = yTP.CoverageType = teamPointsPath["coverage_type"].ToString();

            if(teamPointsCoverageType == "season")
                yTP.WeekOrYear = (int?)teamPointsPath["season"];

            if(teamPointsCoverageType == "week")
                yTP.WeekOrYear = (int?)teamPointsPath["week"];

            yTP.Total = (int?)teamPointsPath["total"];

            return yTP;
        }
    }
}