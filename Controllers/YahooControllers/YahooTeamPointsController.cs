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
    public class YahooTeamPointsController: ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static readonly YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController _yahooApiRequestController;

        public YahooTeamPointsController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooApiRequestController yahooApiRequestController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooApiRequestController = yahooApiRequestController;
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
            _h.StartMethod();
            YahooTeamPoints yTP = new YahooTeamPoints();

            // TEAMID --> A number between 0 and the total number of teams in the league with each number in between representing one of the teams (i.e., each team has its own unique team id number)
            int teamId = 1;

            var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject teamStatsJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);

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
            _h.StartMethod();
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
            _h.StartMethod();

            int teamId      = 1;
            var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject teamStatsJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);

            var newPointsModel = CreateYahooTeamPointsModel(teamId, teamStatsJson);

            return Content($"RETURN: {newPointsModel}");
        }
    }
}
