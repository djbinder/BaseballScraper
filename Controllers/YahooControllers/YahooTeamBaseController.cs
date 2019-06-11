using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class YahooTeamBaseController : ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static readonly YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController _yahooApiRequestController;
        public static readonly YahooGameResourceConroller _yahooGameResourceController = new YahooGameResourceConroller();


        public YahooTeamBaseController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooApiRequestController yahooApiRequestController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooApiRequestController = yahooApiRequestController;
        }

        public YahooTeamBaseController() {}


        [Route("test")]
        public void TestYahooTeamBaseController()
        {
            _h.StartMethod();
            var listOfTeamBases = CreateListOfAllTeamBasesForLeague(10);
        }



        #region GET TEAM BASE DATA - PRIMARY METHODS ------------------------------------------------------------

            // PRIMARY METHOD: OPTION 1
            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     Create instance of yahoo team model; save it to the database
            /// </summary>
            /// <param name="managerId">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Select the Id of the Manager you would want to view
            /// </param>
            /// <example>
            ///     var teamBase = CreateYahooTeamBaseModel(7);
            /// </example>
            public YahooTeamBase CreateYahooTeamBaseModel (int managerId)
            {
                // _h.StartMethod();
                YahooTeamBase tB = new YahooTeamBase();

                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                var uriTeamBase = endPoints.TeamBaseEndPoint(leagueKey, managerId).EndPointUri;
                // Console.WriteLine($"MANAGER CONTROLLER > leagueKey: {leagueKey}");
                // Console.WriteLine($"uriTeamBase: {uriTeamBase}");

                // for each team in the league, dig through their team json, find the required items to create the new YahooTeamBase and set those items
                JObject resourceJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);

                PopulateInitialTeamBaseProperties(tB, resourceJson);

                // Managers is nested under 'teamBasePath'
                var managerPath = resourceJson["fantasy_content"]["team"]["managers"]["manager"];

                // The type under 'manager' will indicate if the team has one manager or co-managers
                // Type for one manager = "Newtonsoft.Json.Linq.JObject"
                // Type for co-managers = "Newtonsoft.Json.Linq.JArray"
                var managerPathChildrenType = managerPath.GetType().ToString();
                string jObjectType = "Newtonsoft.Json.Linq.JObject";
                string jArrayType = "Newtonsoft.Json.Linq.JArray";

                // One manager path
                if(managerPathChildrenType == jObjectType) { PopulateTeamBaseWithOneManager(tB, managerPath); }

                // Co-manager path
                List<YahooManager> teamManagersList = new List<YahooManager>();
                tB.TeamManagersList            = new List<YahooManager>();

                if(managerPathChildrenType == jArrayType) { PopulateTeamBaseWithCoManagers(tB, managerPath, teamManagersList); }

                _h.Dig(tB);
                // _h.CompleteMethod();
                return tB;
            }


            // PRIMARY METHOD: OPTION 2
            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     This is the same thing as 'CreateYahooTeamBaseModel()' method except mgr id passed in in url
            ///     See 'CreateYahooTeamBaseModel()' method for comments for code within method
            /// </summary>
            /// <param name="managerId">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Append team member to end of controller url
            /// </param>
            /// <example>
            ///    https://127.0.0.1:5001/api/yahoo/yahooteambase/1
            /// </example>
            [HttpGet("{managerId}")]
            public YahooTeamBase CreateYahooTeamBaseModelFromUrl (int managerId)
            {
                YahooTeamBase tB = new YahooTeamBase();

                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                var uriTeamBase = endPoints.TeamBaseEndPoint(leagueKey, managerId).EndPointUri;

                JObject resourceJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);

                PopulateInitialTeamBaseProperties(tB, resourceJson);

                var managerPath = resourceJson["fantasy_content"]["team"]["managers"]["manager"];

                var managerPathChildrenType = managerPath.GetType().ToString();
                string jObjectType = "Newtonsoft.Json.Linq.JObject";
                string jArrayType = "Newtonsoft.Json.Linq.JArray";

                if(managerPathChildrenType == jObjectType) { PopulateTeamBaseWithOneManager(tB, managerPath); }

                List<YahooManager> teamManagersList = new List<YahooManager>();
                tB.TeamManagersList            = new List<YahooManager>();

                if(managerPathChildrenType == jArrayType) { PopulateTeamBaseWithCoManagers(tB, managerPath, teamManagersList); }

                return tB;
            }


            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     Create a list of team bases for all teams in league
            ///     Relies on the 'CreateYahooTeamBaseModel()' method to instantiate each Yahoo TeamBase
            /// </summary>
            /// <param name="NumberOfTeams">
            ///     The total number of teams in the league
            ///     Assumes 1 manager per team; if a team is co-managed it just gives you one mgr for that team
            /// </param>
            /// <example>
            ///     var listOfTeamBases = CreateListOfAllTeamBasesForLeague(10);
            /// </example>
            public List<YahooTeamBase> CreateListOfAllTeamBasesForLeague(int NumberOfTeams)
            {
                _h.StartMethod();
                List<YahooTeamBase> yTeamBaseList = new List<YahooTeamBase>();

                for(var counter = 1; counter <= NumberOfTeams - 1; counter++)
                {
                    Console.WriteLine($"Counter: {counter}");
                    YahooTeamBase yTb = CreateYahooTeamBaseModel(counter);
                    yTeamBaseList.Add(yTb);
                }
                _h.Dig(yTeamBaseList);
                return yTeamBaseList;
            }


            // OPTIONAL ADDITIONAL METHOD (not required)
            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     Create hashtable of YahooTeamBase items for a given team=
            /// </summary>
            /// <param name="managerId">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Select the Id of the Manager you would want to view
            /// </param>
            /// <example>
            ///   var teamBase = CreateYahooTeamBaseHashTable(1);
            /// </example>
            public Hashtable CreateYahooTeamBaseHashTable (int managerId)
            {
                // _h.StartMethod();
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                var uriTeamBase = endPoints.TeamBaseEndPoint(leagueKey, managerId).EndPointUri;

                JObject resourceJson = _yahooApiRequestController.GenerateYahooResourceJObject(uriTeamBase);

                Hashtable teamHashTable = new Hashtable
                { { "Key", resourceJson["fantasy_content"]["team"]["team_key"].ToString() },
                    { "TeamId", (int?)resourceJson["fantasy_content"]["team"]["team_id"] },
                    { "Name", resourceJson["fantasy_content"]["team"]["name"].ToString() },
                    { "Is Owned By Current Login?", (int?)resourceJson["fantasy_content"]["team"]["is_owned_by_current_login"] },
                    { "Url", resourceJson["fantasy_content"]["team"]["url"].ToString() },
                    { "Team Logo", resourceJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["url"].ToString() },
                    { "Waiver Priority", (int?)resourceJson["fantasy_content"]["team"]["waiver_priority"] },
                    { "Number of Moves", (int?)resourceJson["fantasy_content"]["team"]["number_of_moves"] },
                    { "Number of Trades", (int?)resourceJson["fantasy_content"]["team"]["number_of_trades"] },
                    { "Coverage Type", resourceJson["fantasy_content"]["team"]["roster_adds"]["coverage_type"].ToString() },
                    { "Coverage Value", resourceJson["fantasy_content"]["team"]["roster_adds"]["coverage_value"].ToString() },
                    { "Value", resourceJson["fantasy_content"]["team"]["roster_adds"]["value"].ToString() },
                    { "League Scoring Type", resourceJson["fantasy_content"]["team"]["league_scoring_type"].ToString() },
                    { "Has Draft Grade?", resourceJson["fantasy_content"]["team"]["has_draft_grade"].ToString() },

                    { "Manager Id", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["manager_id"].ToString() },
                    { "NickName", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["nickname"].ToString() },
                    { "Guid", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["guid"].ToString() },
                    { "Is Commish?", (int?)resourceJson["fantasy_content"]["team"]["managers"]["manager"]["is_commissioner"] },
                    { "Is Current Login?", (int?)resourceJson["fantasy_content"]["team"]["managers"]["manager"]["is_current_login"] },
                    { "Email", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["email"].ToString() },
                    { "Image Url", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["image_url"].ToString() }
                };
                // PrintTeamBaseDictionaryKeysAndValues(teamHashTable);
                // _h.CompleteMethod();
                return teamHashTable;
            }


        #endregion GET TEAM BASE DATA - PRIMARY METHODS ------------------------------------------------------------





        #region GET TEAM BASE DATA - SUPPORT METHODS ------------------------------------------------------------


            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     Set initial properties that all Team Bases have in common
            ///     Other properties are set in either of
            ///     1) 'PopulateTeamBaseWithOneManager()' or
            ///     2) 'PopulateTeamBaseWithCoManagers()' methods
            ///     This is ultimately called in the 'CreateYahooTeamBaseModel()' method
            /// <remarks>
            ///     Note that the code below is formated / indented to give an idea how the json looks
            /// </remarks>
            /// </summary>
            /// <example>
            ///     PopulateInitialTeamBaseProperties(tB, resourceJson);
            /// </example>
            private YahooTeamBase PopulateInitialTeamBaseProperties([FromQuery] YahooTeamBase tB, JObject resourceJson)
            {
                var teamBasePath             = resourceJson["fantasy_content"]["team"];
                    tB.TeamKey               = teamBasePath["team_key"].ToString();
                    tB.TeamId                = (int?)teamBasePath["team_id"];
                    tB.TeamName              = teamBasePath["name"].ToString();
                    tB.IsOwnedByCurrentLogin = (int?)teamBasePath["is_owned_by_current_login"];
                    tB.Url                   = teamBasePath["url"].ToString();

                    // Team Logo is nested under 'teamBasePath'
                    var teamLogosPath    = resourceJson["fantasy_content"]["team"]["team_logos"]["team_logo"];
                        tB.TeamLogo.Size = teamLogosPath["size"].ToString();
                        tB.TeamLogo.Url  = teamLogosPath["url"].ToString();

                    tB.WaiverPriority        = (int?)teamBasePath["waiver_priority"];
                    tB.NumberOfMoves         = (int?)teamBasePath["number_of_moves"];
                    tB.NumberOfTrades        = (int?)teamBasePath["number_of_trades"];

                    // Roster adds is nested under 'teamBasePath'
                    var teamRosterAddsPath              = resourceJson["fantasy_content"]["team"]["roster_adds"];
                        tB.TeamRosterAdds.CoverageType  = teamRosterAddsPath["coverage_type"].ToString();
                        tB.TeamRosterAdds.CoverageValue = teamRosterAddsPath["coverage_value"].ToString();
                        tB.TeamRosterAdds.Value         = teamRosterAddsPath["value"].ToString();

                tB.LeagueScoringType     = teamBasePath["league_scoring_type"].ToString();
                tB.HasDraftGrade         = teamBasePath["has_draft_grade"].ToString();

                return tB;
            }


            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     If the team only has one manager, populate remaining properties with this code
            ///     If there are two managers, populate properties with 'PopulateTeamBaseWithCoManagers()'
            ///     This is ultimately called in the 'CreateYahooTeamBaseModel()' method
            /// </summary>
            /// <example>
            ///     PopulateTeamBaseWithOneManager(tB, managerPath);
            /// </example>
            private YahooTeamBase PopulateTeamBaseWithOneManager([FromQuery] YahooTeamBase tB, [FromQuery] JToken managerPath)
            {
                tB.PrimaryTeamManager.ManagerId = managerPath["manager_id"].ToString();
                tB.PrimaryTeamManager.NickName  = managerPath["nickname"].ToString();
                tB.PrimaryTeamManager.Guid      = managerPath["guid"].ToString();
                try
                {
                    tB.PrimaryTeamManager.IsCommissioner = managerPath["is_commissioner"].ToString();
                    tB.PrimaryTeamManager.IsCurrentLogin = managerPath["is_current_login"].ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EXCEPTION MESSAGE: {ex.Message} --> because they are not the current login and/or they are not the league commissioner");
                }

                // some managers may keep their email address private / hidden; if that's the cause, you'll get an error;
                // this checks if there is an email; if there isn't, it sets the email as 'hidden'
                try
                {
                    tB.PrimaryTeamManager.Email = managerPath["email"].ToString();
                }
                catch
                {
                    tB.PrimaryTeamManager.Email = "hidden";
                }

                tB.PrimaryTeamManager.ImageUrl = managerPath["image_url"].ToString();
                return tB;
            }


            // STATUS [ June 9, 2019 ] : this works
            /// <summary>
            ///     If the team has co-managers, populate remaining properties with this code
            ///     If the team only has one manager, populate remaining properties with 'PopulateTeamBaseWithOneManager()' method
            ///     This is ultimately called in the 'CreateYahooTeamBaseModel()' method
            /// </summary>
            /// <example>
            ///     PopulateTeamBaseWithCoManagers(tB, managerPath, teamManagersList);
            /// </example>
            /// <returns>
            ///     A List<YahooManager> with manager details for each of the co-managers
            /// </returns>
            private List<YahooManager> PopulateTeamBaseWithCoManagers([FromQuery] YahooTeamBase tB, [FromQuery] JToken managerPath, List<YahooManager> teamManagersList)
            {
                Console.WriteLine($"{tB.TeamName} has multiple managers");
                int countOfItemsInJArray = managerPath.Children().Count();

                for(var i = 0; i <= countOfItemsInJArray - 1; i++)
                {
                    tB.PrimaryTeamManager.ManagerId = managerPath[i]["manager_id"].ToString();

                    string nickname = "";

                    tB.PrimaryTeamManager.NickName = nickname = managerPath[i]["nickname"].ToString();
                        Console.WriteLine($"generating managers for {nickname}");

                    tB.PrimaryTeamManager.Guid = managerPath[i]["guid"].ToString();
                    // tB.PrimaryTeamManager.IsCommissioner = managerPath[i]["is_commissioner"].ToString();
                    // tB.PrimaryTeamManager.IsCurrentLogin = managerPath[i]["is_current_login"].ToString();

                    // some managers may keep their email address private / hidden; if that's the case, you'll get an error;
                    // this checks if there is an email; if there isn't, it sets the email as 'hidden' so you don't get an error
                    try
                    {
                        tB.PrimaryTeamManager.Email = managerPath[i]["email"].ToString();
                    }
                    catch
                    {
                        tB.PrimaryTeamManager.Email = "hidden";
                    }

                    tB.PrimaryTeamManager.ImageUrl = managerPath[i]["image_url"].ToString();

                    YahooManager manager = new YahooManager()
                    {
                        ManagerId = managerPath[i]["manager_id"].ToString(),
                        NickName  = managerPath[i]["nickname"].ToString(),
                        Guid      = managerPath[i]["guid"].ToString(),
                        // IsCommissioner = managerPath[i]["is_commissioner"].ToString(),
                        // IsCurrentLogin = managerPath[i]["is_current_login"].ToString(),
                        Email    = tB.PrimaryTeamManager.Email,
                        ImageUrl = managerPath[i]["image_url"].ToString()
                    };

                    teamManagersList.Add(manager);
                    tB.TeamManagersList.Add(manager);
                }

                // _h.Dig(teamManagersList);
                return teamManagersList;
            }


        #endregion GET TEAM BASE DATA - SUPPORT METHODS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------


            public void PrintTeamBaseDictionaryKeysAndValues(Hashtable teamHashTable)
            {
                IDictionaryEnumerator _enumerator = teamHashTable.GetEnumerator();

                int _enumeratorCount = 1;

                while (_enumerator.MoveNext())
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(_enumerator.Key.ToString());
                    Console.ResetColor();

                    if(_enumerator.Value == null)
                    {
                        Console.WriteLine("null");
                    }

                    else
                    {
                        Console.WriteLine(_enumerator.Value.ToString());
                    }
                    Console.WriteLine();
                    _enumeratorCount++;
                }
            }


        #endregion PRINTING PRESS ------------------------------------------------------------


    }
}
