using BaseballScraper.Controllers.YahooControllers.Resources;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.ConfigurationModels;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class YahooTeamStatsController: ControllerBase
    {
        private readonly Helpers                                                       _helpers;
        private readonly TheGameIsTheGameConfiguration                                 _theGameConfig;
        private static readonly YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController                                       _yahooApiRequestController;

        public static readonly YahooGameResourceConroller                              _yahooGameResourceController = new YahooGameResourceConroller();

        public YahooTeamStatsController(Helpers helpers, IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooApiRequestController yahooApiRequestController)
        {
            _helpers = helpers;
            _theGameConfig       = theGameConfig.Value;
            _yahooApiRequestController = yahooApiRequestController;
        }

        public YahooTeamStatsController() {}



        [Route("test")]
        public void TestYahooTeamStatsController()
        {
            _helpers.StartMethod();

            // var tsOne = CreateYahooTeamStatsModel(1);
        }



        #region GET TEAM STATS DATA - PRIMARY METHODS ------------------------------------------------------------


            // PRIMARY METHODS offers 3 options to instantiate a new yahoo stats model
                // OPTION 1 - endpoint parameters defined within the method
                // OPTION 2 - endpoint parameters passed as parameters to method
                // OPTION 3 - endpoint parameters are appended to the Url


            // OPTION 1: endpoint parameters defined within the method
            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     Create new instance of YahooTeamStats Model
            ///     Show full season stats for one team
            /// </summary>
            /// <remarks>
            ///     This is the least helpful option of the method
            ///     Within Option 1, the 'teamId' is defined within the method itself
            ///     To change the team Id you're searching for, change the 'teamId' variable;
            ///     Includes: H/AB, R, HR, RBI, SB, BB, IP, W, SV, H, ERA, WHIP
            ///     Relies on 'PopulateTeamStatsProperties()' to populate model properties
            /// </remarks>
            /// <example>
            ///     https://127.0.0.1:5001/api/yahoo/teamstats
            /// </example>
            /// <returns>
            ///     New YahooTeamStatsModel
            /// </returns>
            public YahooTeamStatsList CreateYahooTeamStatsModel()
            {
                // _h.Spotlight("executing create yahoo team stats method option 1");
                int teamId      = 1;
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(leagueKey, teamId).EndPointUri;

                JObject teamStatsJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);

                YahooTeamStatsList tS = PopulateTeamStatsProperties(teamStatsJson);

                _helpers.Dig(tS);
                return tS;
            }


            // OPTION 2: endpoint parameters passed as parameters to method (i.e., 'teamId')
            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     Create new instance of YahooTeamStats Model
            ///     Show full season stats for one team
            /// </summary>
            /// <remarks>
            ///     Within Option 2, 'teamId' is passed in method
            ///     This option must be called by another method
            ///     To change the team you want to view the stats for, change the id number you are passing when calling the method
            ///     Includes: H/AB, R, HR, RBI, SB, BB, IP, W, SV, H, ERA, WHIP
            ///     Relies on 'PopulateTeamStatsProperties()' to populate model properties
            /// </remarks>
            /// <param name="teamId">
            ///     A number between 0 and the total number of teams in the league (i.e., each team has its own unique team id number)
            /// </param>
            /// <example>
            ///     var tsModel = CreateYahooTeamStatsModel(1)
            /// </example>
            public YahooTeamStatsList CreateYahooTeamStatsModel(int teamId)
            {
                // _h.Spotlight("executing create yahoo team stats method option 2");
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                // Console.WriteLine($"leagueKey: {leagueKey}");
                var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(leagueKey, teamId).EndPointUri;
                Console.WriteLine($"CreateYahooTeamStatsModel > uriTeamBase: {uriTeamBase}");


                JObject teamStatsJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);
                // _h.Dig(teamStatsJson);

                YahooTeamStatsList tS = PopulateTeamStatsProperties(teamStatsJson);

                _helpers.Dig(tS);
                return tS;
            }


            // OPTION 3: endpoint parameters are appended to the Url
            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     Create new instance of YahooTeamStats Model
            ///     Show full season stats for one team
            /// </summary>
            /// <remarks>
            ///     Within Option 3, there are two parameters.
            ///     To change the team you are searching for, change the team id within the url itself
            ///     Includes: H/AB, R, HR, RBI, SB, BB, IP, W, SV, H, ERA, WHIP
            ///     Relies on 'PopulateTeamStatsProperties()' to populate model properties
            /// </remarks>
            /// <param name="teamId">
            ///      A number between 0 and the total number of teams in the league (i.e., each team has its own unique team id number)
            /// </param>
            /// <example>
            ///     https://127.0.0.1:5001/api/yahoo/yahooteamstats/2
            /// </example>
            [HttpGet("{teamId}")]
            public YahooTeamStatsList CreateYahooTeamStatsModelFromUrl(int teamId)
            {
                // _h.Spotlight("executing create yahoo team stats method option 3");
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                var uriTeamBase = endPoints.TeamSeasonStatsEndPoint(leagueKey, teamId).EndPointUri;

                JObject teamStatsJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);

                YahooTeamStatsList tS = PopulateTeamStatsProperties(teamStatsJson);

                // _helpers.Dig(tS);
                return tS;
            }


            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     Create a list of team total stats for all teams in league for the season
            ///     Relies on the 'CreateYahooTeamStatsModel()' method to instantiate each Yahoo Team Stats List
            /// </summary>
            /// <param name="NumberOfTeams">
            ///     The total number of teams in the league
            ///     Assumes 1 manager per team; if a team is co-managed it just gives you one mgr for that team
            /// </param>
            /// <example>
            ///     var listOfTeamStats = CreateListOfAllYahooTeamStatsListsForLeague(10);
            /// </example>
            public List<YahooTeamStatsList> CreateListOfAllYahooTeamStatsListsForLeague(int NumberOfTeams)
            {
                List<YahooTeamStatsList> yStatsList = new List<YahooTeamStatsList>();

                for(var counter = 1; counter <= NumberOfTeams - 1; counter++)
                {
                    YahooTeamStatsList ySL = CreateYahooTeamStatsModel(counter);
                    yStatsList.Add(ySL);
                }
                _helpers.Dig(yStatsList);
                return yStatsList;
            }


        #endregion GET TEAM STATS DATA - PRIMARY METHODS ------------------------------------------------------------





        #region GET TEAM STATS DATA - SUPPORT METHODS ------------------------------------------------------------


            public YahooTeamStatsList PopulateTeamStatsProperties(JObject teamStatsJson)
            {
                YahooTeamStatsList tS = new YahooTeamStatsList();

                var yTs = tS.YahooTeamStats = new YahooTeamStats();

                var teamPath = teamStatsJson["fantasy_content"]["team"];
                    tS.TeamKey = teamPath["team_key"].ToString();
                    tS.TeamId = teamPath["team_id"].ToString();
                    tS.TeamName = teamPath["name"].ToString();

                yTs.WeekNumber = teamPath["roster_adds"]["coverage_value"].ToString();

                var managerPath = teamStatsJson["fantasy_content"]["team"]["managers"]["manager"];
                    tS.ManagerId = managerPath["manager_id"].ToString();
                    tS.ManagerNickName = managerPath["nickname"].ToString();
                    tS.ManagerGuid = managerPath["guid"].ToString();

                var teamStatsPath = teamStatsJson["fantasy_content"]["team"]["team_stats"];
                    yTs.StatCoverageType = teamStatsPath["coverage_type"].ToString();
                    yTs.Season = teamStatsPath["season"].ToString();

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

                    return tS;
            }


        #endregion GET TEAM STATS DATA - SUPPORT METHODS ------------------------------------------------------------




    }
}
