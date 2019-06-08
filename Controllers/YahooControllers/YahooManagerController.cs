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


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooManagerController: ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static readonly YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController _yahooApiRequestController;
        private readonly YahooAuthController _yahooAuthController = new YahooAuthController();
        public string _sessionIdKey = "sessionid";

        public YahooManagerController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooApiRequestController yahooApiRequestController, YahooAuthController yahooAuthController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooApiRequestController = yahooApiRequestController;
            _yahooAuthController = yahooAuthController;
        }


        [Route("test")]
        public void TestYahooManagerController()
        {
            _h.StartMethod();

            _yahooAuthController.GetYahooAccessTokenResponse();

            try
            {
                int x = _yahooAuthController.CheckSessionId();

                if(x == 0)
                {
                    Console.WriteLine("000000000");
                }
                if(x > 0)
                {
                    Console.WriteLine(">>>>>>>>>>>");
                }
                else{
                    Console.WriteLine(" NULLLLLLLLL ");
                }
            }

            catch
            {
                Console.WriteLine("SUPER BROKE");
            }



            // _yAuthController.CheckSession();
            // _yAuthController.CheckYahooSession();

            // HttpContext.Session.SetInt32(_sessionIdKey,1);
            // var test = HttpContext.Session.GetInt32(_sessionIdKey);
            // Console.WriteLine($"test: {test}");


            // var yahooManager = CreateYahooManagerModel(7);
            // Console.WriteLine(yahooManager.NickName);

            // _yahooAuthController.CheckSession();

            // GetAllYahooManagersJObject();
        }



        /// <summary>
        ///     Instantiate new instance of a yahoo manager
        /// </summary>
        /// <param name="managerId">
        ///     A number 0 - X; Where X is the total number of teams in the league;
        ///     Basically every manager has their own single number Id;
        ///     Select the Id of the Manager you would want to view </param>
        /// <example>
        ///     https://127.0.0.1:5001/api/yahoo/manager/1
        /// </example>
        /// <returns> A new YahooManager </returns>
        public YahooManager CreateYahooManagerModel (int managerId)
        {
            // _h.StartMethod();

            // retrieve the league key from user secrets / yahoo league config

            string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();

            Console.WriteLine($"MANAGER CONTROLLER > leagueKey: {leagueKey}");

            // create the uri that will be used to generate the appropriate json; in this case, it's the League Standings endpoint (view YahooApiEndPoints.cs)
            var uriLeagueStandings = endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;

            JObject leagueStandings = _yahooApiRequestController.GenerateYahooResourceJObject(uriLeagueStandings);
            // _h.Dig(leagueStandings);

            YahooManager yM = new YahooManager
            {
                // these pull from the yahoo response (xml or json) to set each item
                ManagerId = endPoints.TeamItem(leagueStandings, managerId, "ManagerId"),
                NickName = endPoints.TeamItem(leagueStandings, managerId, "Nickname"),
                Guid = endPoints.TeamItem(leagueStandings, managerId, "Guid")
            };
            Console.WriteLine(yM.NickName);
            try
            {
                yM.IsCommissioner = endPoints.TeamItem(leagueStandings, managerId, "IsCommissioner");
                yM.IsCurrentLogin = endPoints.TeamItem(leagueStandings, managerId, "IsCurrentLogin");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR IN: YahooManagerController.CreateYahooManagerModel()");
                // Console.WriteLine($"EXCEPTION MESSAGE: {ex.Message} --> because they are not the current login and/or they are not the league commissioner");
            }
            yM.Email    = endPoints.TeamItem(leagueStandings, managerId, "Email");
            yM.ImageUrl = endPoints.TeamItem(leagueStandings, managerId, "ImageUrl");

            PrintYahooManagerDetails(yM);

            return yM;
        }


            //   "managers": {
            //     "manager": {
            //       "manager_id": "7",
            //       "nickname": "Mike",
            //       "guid": "T2JIOUHEBZEM2IQ4VGS7CEYOJA",
            //       "email": "mjsmith04@comcast.net",
            //       "image_url": "https://ct.yimg.com/cy/1768/39361574426_98028a_64sq.jpg"
            //     }
            //   },




        public JObject GetAllYahooManagersJObject ()
        {
            // _h.StartMethod();

            // retrieve the league key from user secrets / yahoo league config
            // string leagueKey = _theGameConfig.LeagueKey;
            string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
            Console.WriteLine($"MANAGER CONTROLLER > leagueKey: {leagueKey}");

            // create the uri that will be used to generate the appropriate json; in this case, it's the League Standings endpoint (view YahooApiEndPoints.cs)
            var uriLeagueStandings = endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;
            Console.WriteLine($"MANAGER CONTROLLER > x : {uriLeagueStandings}");

            JObject leagueStandings = _yahooApiRequestController.GenerateYahooResourceJObject(uriLeagueStandings);
            _h.Dig(leagueStandings);

            return leagueStandings;
        }



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
