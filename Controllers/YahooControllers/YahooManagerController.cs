using System;
using BaseballScraper.Controllers;
using BaseballScraper.Models.Configuration;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    public class YahooManagerController: Controller
    {
        private Constants _c = new Constants();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private static YahooHomeController _yahooHomeController;

        public YahooManagerController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooHomeController yahooHomeController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooHomeController = yahooHomeController;
        }

        /// <summary> Instantiate new instance of a yahoo manager </summary>
        /// <param name="managerId"> A number 0 - X; Where X is the total number of teams in the league; Basically every manager has their own single number Id </param>
        /// <returns> A new YahooManager </returns>
        [Route("yahoo/manager/create")]
        public YahooManager CreateYahooManagerModel (int managerId)
        {
            _c.Start.ThisMethod();

            // retrieve the league key from user secrets / yahoo league config
            string leagueKey = _theGameConfig.LeagueKey;

            // create the uri that will be used to generate the appropriate json; in this case, it's the League Standings endpoint (view YahooApiEndPoints.cs)
            var uriLeagueStandings = endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;

            JObject leagueStandings = _yahooHomeController.GenerateYahooResourceJObject(uriLeagueStandings);

            YahooManager yM = new YahooManager();
            // int          managerId = 0;

            // these pull from the yahoo response (xml or json) to set each item
            yM.ManagerId      = endPoints.TeamItem(leagueStandings, managerId, "ManagerId");
            yM.NickName       = endPoints.TeamItem(leagueStandings, managerId, "Nickname");
            yM.Guid           = endPoints.TeamItem(leagueStandings, managerId, "Guid");
            yM.IsCommissioner = endPoints.TeamItem(leagueStandings, managerId, "IsCommissioner");
            yM.IsCurrentLogin = endPoints.TeamItem(leagueStandings, managerId, "IsCurrentLogin");
            yM.Email          = endPoints.TeamItem(leagueStandings, managerId, "Email");
            yM.ImageUrl       = endPoints.TeamItem(leagueStandings, managerId, "ImageUrl");

            return yM;
        }

        // View the yahoo manager
        [Route("yahoo/manager/view")]
        public IActionResult ViewYahooManagerModel ()
        {
            _c.Start.ThisMethod();

            var yahooManager = CreateYahooManagerModel(1);

            return Content($"mgr: {yahooManager}");
        }
    }
}