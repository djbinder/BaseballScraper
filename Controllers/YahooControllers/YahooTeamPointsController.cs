using System;
using BaseballScraper.Models.Configuration;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    [Route("api/yahoo/teampoints")]
    [ApiController]
    public class YahooTeamPointsController: Controller
    {
        private Constants _c = new Constants();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooHomeController _yahooHomeController;

        public YahooTeamPointsController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooHomeController yahooController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooHomeController = yahooController;
        }

        /// <summary>
        // FILE NOTE: provides two options on how to instantiate new instance of model
            // OPTION 1 - endpoint parameters defined within the method
            // OPTION 2 - endpoint parameters passed as parameters to method
        /// </summary>



        // OPTION 1: endpoint parameters defined within the method
        /// <summary> Create new instance of YahooTeamPoints model </summary>
        /// <example> https://127.0.0.1:5001/api/yahoo/teampoints/season </example>
        /// <returns> Instance of YahooTeamPoints model; includes CoverageType, WeekOrYear, and Total points </returns>
        [HttpGet]
        [Route("season")]
        public YahooTeamPoints CreateYahooTeamPointsModel ()
        {
            _c.Start.ThisMethod();
            YahooTeamPoints yTP = new YahooTeamPoints();

            // TEAMID --> A number between 0 and the total number of teams in the league with each number in between representing one of the teams (i.e., each team has its own unique team id number)
            int teamId = 1;

            var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject teamStatsJson = _yahooHomeController.GenerateYahooResourceJObject(uriTeamBase);

            var teamPointsPath = teamStatsJson["fantasy_content"]["team"]["team_points"];
            // var teamPointsPath         = o["fantasy_content"]["team"]["team_points"];
            var teamPointsCoverageType = yTP.CoverageType = teamPointsPath["coverage_type"].ToString();

            if(teamPointsCoverageType == "season")
                yTP.WeekOrYear = (int?)teamPointsPath["season"];

            if(teamPointsCoverageType == "week")
                yTP.WeekOrYear = (int?)teamPointsPath["week"];

            yTP.Total = (int?)teamPointsPath["total"];

            return yTP;
        }


        // OPTION 2 : endpoint parameters passed as parameters to method
        /// <summary> Create new instance of YahooTeamPoints model </summary>
        /// <param name="teamId"> A number between 0 and the total number of teams in the league with each number in between representing one of the teams (i.e., each team has its own unique team id number) </param>
        /// <param name="o"> An object that contains the json needed to instantiate new instance of YahooTeamPoints model</param>
        /// <returns> Instance of YahooTeamPoints model; includes CoverageType, WeekOrYear, and Total points  </returns>
        public YahooTeamPoints CreateYahooTeamPointsModel (int teamId, JObject o)
        {
            _c.Start.ThisMethod();
            YahooTeamPoints yTP = new YahooTeamPoints();

            var teamPointsPath = o["fantasy_content"]["team"]["team_points"];

            var teamPointsCoverageType = yTP.CoverageType = teamPointsPath["coverage_type"].ToString();

            if(teamPointsCoverageType == "season")
                yTP.WeekOrYear = (int?)teamPointsPath["season"];

            if(teamPointsCoverageType == "week")
                yTP.WeekOrYear = (int?)teamPointsPath["week"];

            yTP.Total = (int?)teamPointsPath["total"];

            return yTP;
        }


        // OPTION 2B : View for Option 2
        /// <summary> This allows viewing / testing of Option 2; The method is called and a team id and a JObject are passed parameters </summary>
        /// <example> https://127.0.0.1:5001/api/yahoo/teampoints </example>
        /// <returns> A view of points for a selected team </returns>
        [HttpGet]
        [Route("")]
        public IActionResult ViewTeamPoints()
        {
            _c.Start.ThisMethod();

            int teamId      = 1;
            var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject teamStatsJson = _yahooHomeController.GenerateYahooResourceJObject(uriTeamBase);

            var newPointsModel = CreateYahooTeamPointsModel(teamId, teamStatsJson);

            return Content($"RETURN: {newPointsModel}");
        }
    }
}