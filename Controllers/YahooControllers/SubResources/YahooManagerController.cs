using System;
using BaseballScraper.Controllers;
using BaseballScraper.Models.Configuration;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;


#pragma warning disable CS0168, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class YahooManagerController: ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private static readonly YahooApiEndPoints _endPoints = new YahooApiEndPoints();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static YahooApiRequestController _yahooApiRequestController;
        private readonly YahooAuthController _yahooAuthController = new YahooAuthController();


        public YahooManagerController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooApiRequestController yahooApiRequestController, YahooAuthController yahooAuthController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooApiRequestController = yahooApiRequestController;
            _yahooAuthController = yahooAuthController;
        }


        public YahooManagerController(){}


        [Route("test")]
        public void TestYahooManagerController()
        {
            var listOfManagers = GetListOfAllManagersInLeague(10);
        }



        // 1 : DJB
        // 2 : DSch
        // 3 : CSt
        // 4 : SpMc
        // 5 : PJB
        // 6 : JB
        // 7 : Pants
        // 8 : JCuz
        // 9 : DBra
        // 10: MC

        #region YAHOO MANAGER - PRIMARY METHODS ------------------------------------------------------------


            // STATUS [ June 8, 2019 ] : this works
            /// <summary>
            ///     Instantiate new instance of a yahoo manager
            ///     The manager data in the requested json is found nested under league standings
            /// </summary>
            /// <param name="managerId">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Select the Id of the Manager you would want to view
            /// </param>
            /// <example>
            ///     var yahooManager = CreateYahooManagerModel(1);
            /// </example>
            /// <returns>
            ///     A new YahooManager
            /// </returns>
            public YahooManager CreateYahooManagerModel (int managerId)
            {
                // _h.StartMethod();
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                // Console.WriteLine($"MANAGER CONTROLLER > leagueKey: {leagueKey}");

                // Create the uri that will be used to generate the appropriate json
                // Use Lg. Standings endpoint because mgr. details are nested in league standings json
                var uriLeagueStandings = _endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;

                JObject leagueStandings = _yahooApiRequestController.GenerateYahooResourceJObject(uriLeagueStandings);

                YahooManager yM = new YahooManager
                {
                    // these pull from the yahoo response (xml or json) to set each item
                    ManagerId = _endPoints.TeamItem(leagueStandings, managerId, "ManagerId"),
                    NickName = _endPoints.TeamItem(leagueStandings, managerId, "Nickname"),
                    Guid = _endPoints.TeamItem(leagueStandings, managerId, "Guid")
                };

                // Only the commish of the league will have  the "IsCommissioner" field
                try { yM.IsCommissioner = _endPoints.TeamItem(leagueStandings, managerId, "IsCommissioner"); }
                catch (Exception ex) { Console.WriteLine("ERROR IN: YahooManagerController.CreateYahooManagerModel() --> user not the commish");}


                // "IsCurrentLogin" will only return data of current user is fetching their league's data
                try { yM.IsCurrentLogin = _endPoints.TeamItem(leagueStandings, managerId, "IsCurrentLogin"); }
                catch { Console.WriteLine("ERROR IN: YahooManagerController.CreateYahooManagerModel() --> user not looged in "); }


                // Yahoo managers can hide their email; if hidden, you'll get an error
                try { yM.Email    = _endPoints.TeamItem(leagueStandings, managerId, "Email"); }
                catch { Console.WriteLine("ERROR IN: YahooManagerController.CreateYahooManagerModel() --> User has no email"); }

                yM.ImageUrl = _endPoints.TeamItem(leagueStandings, managerId, "ImageUrl");
                // PrintYahooManagerDetails(yM);
                return yM;
            }


            // STATUS [ June 8, 2019 ] : this works
            /// <summary>
            ///     This is the same thing as 'GetYahooManagerModel()' method except managerId is passed in through url
            /// </summary>
            /// <example>
            ///     https://127.0.0.1:5001/api/yahoo/yahoomanager/1
            /// </example>
            [HttpGet("{managerId}")]
            public IActionResult CreateYahooManagerModelFromUrl (int managerId)
            {
                // _h.StartMethod();
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                // Console.WriteLine($"MANAGER CONTROLLER > leagueKey: {leagueKey}");

                // Create the uri that will be used to generate the appropriate json
                // Use Lg. Standings endpoint because mgr. details are nested in league standings json
                var uriLeagueStandings = _endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;

                JObject leagueStandings = _yahooApiRequestController.GenerateYahooResourceJObject(uriLeagueStandings);

                YahooManager yM = new YahooManager
                {
                    // these pull from the yahoo response (xml or json) to set each item
                    ManagerId = _endPoints.TeamItem(leagueStandings, managerId, "ManagerId"),
                    NickName = _endPoints.TeamItem(leagueStandings, managerId, "Nickname"),
                    Guid = _endPoints.TeamItem(leagueStandings, managerId, "Guid")
                };

                // Only the commish of the league will have  the "IsCommissioner" field
                try { yM.IsCommissioner = _endPoints.TeamItem(leagueStandings, managerId, "IsCommissioner"); }
                catch (Exception ex) { Console.WriteLine("ERROR IN: YahooManagerController.CreateYahooManagerModel() --> user not the commish");}


                // "IsCurrentLogin" will only return data of current user is fetching their league's data
                try { yM.IsCurrentLogin = _endPoints.TeamItem(leagueStandings, managerId, "IsCurrentLogin"); }
                catch { Console.WriteLine("ERROR IN: YahooManagerController.CreateYahooManagerModel() --> user not looged in "); }


                // Yahoo managers can hide their email; if hidden, you'll get an error
                try { yM.Email    = _endPoints.TeamItem(leagueStandings, managerId, "Email"); }
                catch { Console.WriteLine("ERROR IN: YahooManagerController.CreateYahooManagerModel() --> User has no email"); }

                yM.ImageUrl = _endPoints.TeamItem(leagueStandings, managerId, "ImageUrl");
                // PrintYahooManagerDetails(yM);
                return Content(yM.ToString());
            }


            // STATUS [ June 8, 2019 ] : this works
            /// <summary>
            ///     Create a list of all yahoo managers in the league
            ///     The manager data in the requested json is found nested under league standings
            ///     Relies on the 'CreateYahooManagerModel()' method to instantiate each Yahoo Managers
            /// </summary>
            /// <param name="NumberOfTeams">
            ///     The total number of teams in the league
            ///     Assumes 1 manager per team; if a team is co-managed it just gives you one mgr for that team
            /// </param>
            /// <example>
            ///     var listOfManagers = GetListOfAllManagersInLeague(10);
            /// </example>
            public List<YahooManager> GetListOfAllManagersInLeague(int NumberOfTeams)
            {
                List<YahooManager> yManagerList = new List<YahooManager>();

                for(var counter = 0; counter <= NumberOfTeams - 1; counter++)
                {
                    YahooManager yM = CreateYahooManagerModel(counter);
                    yManagerList.Add(yM);
                }
                return yManagerList;
            }


            // STATUS [ June 8, 2019 ] : this kind of works but not sure if it's needed
            // Right now the JObject is the LeagueStandings json; would need to dig to get all managers
            // public JObject GetAllYahooManagersJObject ()
            // {
            //     string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();

            //     // Create the uri that will be used to generate the appropriate json
            //     // Use Lg. Standings endpoint because mgr. details are nested in league standings json
            //     var uriLeagueStandings = endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;

            //     JObject leagueStandings = _yahooApiRequestController.GenerateYahooResourceJObject(uriLeagueStandings);
            //     // _h.Dig(leagueStandings);

            //     return leagueStandings;
            // }


        #endregion YAHOO MANAGER - PRIMARY METHODS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------


            public void PrintYahooManagerDetails(YahooManager yM)
            {
                    Console.WriteLine();
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine("### YAHOO MANAGER DETAILS ###");
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine($"Y! MGR RECORD ID    | {yM.YahooManagerRecordId}");
                    Console.WriteLine($"MANAGER ID          | {yM.ManagerId}");
                    Console.WriteLine($"NICKNAME            | {yM.NickName}");
                    Console.WriteLine($"GUID                | {yM.Guid}");
                    Console.WriteLine($"IS COMMISH?         | {yM.IsCommissioner}");
                    Console.WriteLine($"IS CURRENT LOGIN?   | {yM.IsCurrentLogin}");
                    Console.WriteLine($"EMAIL               | {yM.Email}");
                    Console.WriteLine($"IMAGE               | {yM.ImageUrl}");
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine();
            }


        #endregion PRINTING PRESS ------------------------------------------------------------

    }
}
