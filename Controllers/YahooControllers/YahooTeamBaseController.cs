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

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    public class YahooTeamBaseController
    {
        private Constants _c = new Constants();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooHomeController _yahooHomeController;

        public YahooTeamBaseController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooHomeController yahooHomeController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooHomeController = yahooHomeController;
        }


        /// <summary> Create instance of yahoo team model; save it to the database</summary>
        /// <returns> new YahooTeamBase </returns>
        [HttpGet]
        [Route("/yahoo/teambase/create")]
        public YahooTeamBase CreateYahooTeamBaseModel ()
        {
            _c.Start.ThisMethod();

            YahooTeamBase tB = new YahooTeamBase();

            int countOfTeamsInLeague = 10;

            // for each team in the league, dig through their team json, find the required items to create the new YahooTeamBase and set those items
            for(var teamId = 1; teamId <= countOfTeamsInLeague; teamId++)
            {
                Console.WriteLine($"creating team base for team {teamId}");
                var uriTeamBase = endPoints.TeamBaseEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

                JObject resourceJson = _yahooHomeController.GenerateYahooResourceJObject(uriTeamBase);

                var teamBasePath             = resourceJson["fantasy_content"]["team"];
                    tB.TeamKey               = teamBasePath["team_key"].ToString();
                    tB.TeamName              = teamBasePath["name"].ToString();
                    tB.TeamId                = (int?)teamBasePath["team_id"];
                    tB.IsOwnedByCurrentLogin = (int?)teamBasePath["is_owned_by_current_login"];
                    tB.Url                   = teamBasePath["url"].ToString();
                    tB.WaiverPriority        = (int?)teamBasePath["waiver_priority"];
                    tB.NumberOfMoves         = (int?)teamBasePath["number_of_moves"];
                    tB.NumberOfTrades        = (int?)teamBasePath["number_of_trades"];
                    tB.LeagueScoringType     = teamBasePath["league_scoring_type"].ToString();
                    tB.HasDraftGrade         = teamBasePath["has_draft_grade"].ToString();

                // team logo
                var teamLogosPath    = resourceJson["fantasy_content"]["team"]["team_logos"]["team_logo"];
                    tB.TeamLogo.Size = teamLogosPath["size"].ToString();
                    tB.TeamLogo.Url  = teamLogosPath["url"].ToString();

                // roster adds
                var teamRosterAddsPath              = resourceJson["fantasy_content"]["team"]["roster_adds"];
                    tB.TeamRosterAdds.CoverageType  = teamRosterAddsPath["coverage_type"].ToString();
                    tB.TeamRosterAdds.CoverageValue = teamRosterAddsPath["coverage_value"].ToString();
                    tB.TeamRosterAdds.Value         = teamRosterAddsPath["value"].ToString();

                var managerPath = resourceJson["fantasy_content"]["team"]["managers"]["manager"];
                    // the type under 'manager' will be different if there are more than 1 managers (i.e., there is a co-commish)
                    var managerPathChildrenType = managerPath.GetType().ToString();
                        // if there is one manager, the type is "Newtonsoft.Json.Linq.JObject"
                        string jObjectType = "Newtonsoft.Json.Linq.JObject";
                        // if there are co-managers, the type is "Newtonsoft.Json.Linq.JArray"
                        string jArrayType = "Newtonsoft.Json.Linq.JArray";

                        // if the manager type is JObject, there is only one manager for the team; so you would go this path to finish creating the new YahooTeamBase
                        if(managerPathChildrenType == jObjectType)
                        {
                            tB.PrimaryTeamManager.ManagerId      = managerPath["manager_id"].ToString();
                            tB.PrimaryTeamManager.NickName       = managerPath["nickname"].ToString();
                            tB.PrimaryTeamManager.Guid           = managerPath["guid"].ToString();
                            tB.PrimaryTeamManager.IsCommissioner = managerPath["is_commissioner"].ToString();
                            tB.PrimaryTeamManager.IsCurrentLogin = managerPath["is_current_login"].ToString();

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
                        }

                        // if the manager type is JArray, then there are co-managers for the team; so go this path to finish creating the new YahooTeamBase
                        int  countCheck                     = 1;
                        List<YahooManager> teamManagersList = new List<YahooManager>();
                             tB.TeamManagersList            = new List<YahooManager>();
                        if(managerPathChildrenType == jArrayType)
                        {
                            Console.WriteLine($"{tB.TeamName} has multiple managers");
                            int countOfItemsInJArray = managerPath.Children().Count();

                            for(var i = 0; i <= countOfItemsInJArray - 1; i++)
                            {
                                tB.PrimaryTeamManager.ManagerId = managerPath[i]["manager_id"].ToString();

                                string nickname = "";

                                tB.PrimaryTeamManager.NickName = nickname = managerPath[i]["nickname"].ToString();
                                    Console.WriteLine($"generating managers for {nickname}");

                                tB.PrimaryTeamManager.Guid           = managerPath[i]["guid"].ToString();
                                tB.PrimaryTeamManager.IsCommissioner = managerPath[i]["is_commissioner"].ToString();
                                tB.PrimaryTeamManager.IsCurrentLogin = managerPath[i]["is_current_login"].ToString();

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
                                    ManagerId      = managerPath[i]["manager_id"].ToString(),
                                    NickName       = managerPath[i]["nickname"].ToString(),
                                    Guid           = managerPath[i]["guid"].ToString(),
                                    IsCommissioner = managerPath[i]["is_commissioner"].ToString(),
                                    IsCurrentLogin = managerPath[i]["is_current_login"].ToString(),
                                    Email          = tB.PrimaryTeamManager.Email,
                                    ImageUrl       = managerPath[i]["image_url"].ToString()
                                };

                                teamManagersList.Add(manager);
                                tB.TeamManagersList.Add(manager);
                            }

                            teamManagersList.Dig();
                        }
                tB.Dig();
            }
            // SaveObjectToDatabase(tB);

            _c.Complete.ThisMethod();

            return tB;
        }


        // optional; a different way to set a YahooTeamBase
        [Route("/yahoo/teambase/hashtable")]
        public Hashtable CreateYahooTeamBaseHashTable ()
        {
            _c.Start.ThisMethod();

            int teamId      = 1;
            var uriTeamBase = endPoints.TeamBaseEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            JObject resourceJson = _yahooHomeController.GenerateYahooResourceJObject(uriTeamBase);

            Hashtable teamHashTable = new Hashtable();
                teamHashTable.Add("Key", resourceJson["fantasy_content"]["team"]["team_key"].ToString());
                teamHashTable.Add("TeamId", (int?)resourceJson["fantasy_content"]["team"]["team_id"]);
                teamHashTable.Add("Name", resourceJson["fantasy_content"]["team"]["name"].ToString());
                teamHashTable.Add("Is Owned By Current Login?", (int?)resourceJson["fantasy_content"]["team"]["is_owned_by_current_login"]);
                teamHashTable.Add("Url", resourceJson["fantasy_content"]["team"]["url"].ToString());
                teamHashTable.Add("Team Logo", resourceJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["url"].ToString());
                teamHashTable.Add("Waiver Priority", (int?)resourceJson["fantasy_content"]["team"]["waiver_priority"]);
                teamHashTable.Add("Number of Moves", (int?)resourceJson["fantasy_content"]["team"]["number_of_moves"]);
                teamHashTable.Add("Number of Trades", (int?)resourceJson["fantasy_content"]["team"]["number_of_trades"]);
                teamHashTable.Add("Coverage Type", resourceJson["fantasy_content"]["team"]["roster_adds"]["coverage_type"].ToString());
                teamHashTable.Add("Coverage Value", resourceJson["fantasy_content"]["team"]["roster_adds"]["coverage_value"].ToString());
                teamHashTable.Add("Value", resourceJson["fantasy_content"]["team"]["roster_adds"]["value"].ToString());
                teamHashTable.Add("League Scoring Type", resourceJson["fantasy_content"]["team"]["league_scoring_type"].ToString());
                teamHashTable.Add("Has Draft Grade?", resourceJson["fantasy_content"]["team"]["has_draft_grade"].ToString());

                teamHashTable.Add("Manager Id", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["manager_id"].ToString());
                teamHashTable.Add("NickName", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["nickname"].ToString());
                teamHashTable.Add("Guid", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["guid"].ToString());
                teamHashTable.Add("Is Commish?", (int?)resourceJson["fantasy_content"]["team"]["managers"]["manager"]["is_commissioner"]);
                teamHashTable.Add("Is Current Login?", (int?)resourceJson["fantasy_content"]["team"]["managers"]["manager"]["is_current_login"]);
                teamHashTable.Add("Email", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["email"].ToString());
                teamHashTable.Add("Image Url", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["image_url"].ToString());

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

            _c.Complete.ThisMethod();
           return teamHashTable;
        }
    }
}