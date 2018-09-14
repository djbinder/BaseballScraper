using System;
using BaseballScraper.Controllers;
using BaseballScraper.Models.Configuration;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using BaseballScraper.Infrastructure;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    [Route("api/yahoo")]
    [ApiController]
    public class YahooManagerController: Controller
    {
        private Helpers _h = new Helpers();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooHomeController _yahooHomeController;

        public YahooManagerController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooHomeController yahooHomeController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooHomeController = yahooHomeController;
        }

        /// <summary> Instantiate new instance of a yahoo manager </summary>
        /// <param name="managerId"> A number 0 - X; Where X is the total number of teams in the league; Basically every manager has their own single number Id; Select the Id of the Manager you would want to view </param>
        /// <example> https://127.0.0.1:5001/api/yahoo/manager/1 </example>
        /// <returns> A new YahooManager </returns>
        [Route("manager/{managerId}")]
        public YahooManager CreateYahooManagerModel (int managerId)
        {
            _h.StartMethod();

            // retrieve the league key from user secrets / yahoo league config
            string leagueKey = _theGameConfig.LeagueKey;

            // create the uri that will be used to generate the appropriate json; in this case, it's the League Standings endpoint (view YahooApiEndPoints.cs)
            var uriLeagueStandings = endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;

            JObject leagueStandings = _yahooHomeController.GenerateYahooResourceJObject(uriLeagueStandings);

            YahooManager yM = new YahooManager();
            // int          managerId = 0;

            // these pull from the yahoo response (xml or json) to set each item
            yM.ManagerId = endPoints.TeamItem(leagueStandings, managerId, "ManagerId");
            yM.NickName  = endPoints.TeamItem(leagueStandings, managerId, "Nickname");
            yM.Guid      = endPoints.TeamItem(leagueStandings, managerId, "Guid");
            try
            {
                yM.IsCommissioner = endPoints.TeamItem(leagueStandings, managerId, "IsCommissioner");
                yM.IsCurrentLogin = endPoints.TeamItem(leagueStandings, managerId, "IsCurrentLogin");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION MESSAGE: {ex.Message} --> because they are not the current login and/or they are not the league commissioner");
            }
            yM.Email    = endPoints.TeamItem(leagueStandings, managerId, "Email");
            yM.ImageUrl = endPoints.TeamItem(leagueStandings, managerId, "ImageUrl");

            return yM;
        }

        /// <summary> View the yahoo manager page</summary>
        /// <example> https://127.0.0.1:5001/api/yahoo/manager/view </example>
        /// <returns> A view with instantiated model </returns>
        [Route("manager/view")]
        public IActionResult ViewYahooManagerModel ()
        {
            _h.StartMethod();

            var yahooManager = CreateYahooManagerModel(1);

            return Content($"mgr: {yahooManager}");
        }
    }
}