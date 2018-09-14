using System;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BaseballScraper.Models;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.Configuration;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219, CS0169
    [Route("api/yahoo")]
    [ApiController]
    public class YahooScoreboardController: Controller
    {
        private Helpers _h                         = new Helpers();
        private static YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static YahooHomeController _yahooHomeController;

        public YahooScoreboardController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooHomeController yahooHomeController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooHomeController = yahooHomeController;
        }

        // TODO: add XML summary comments
        // Step 1
        public void CreateYahooLeagueScoreboard ()
        {
            _h.StartMethod();

            // retrieve the league key from user secrets / yahoo league config
            string leagueKey = _theGameConfig.LeagueKey;

            var uriLeagueScoreboard = endPoints.LeagueSeasonScoreboardEndPoint(leagueKey).EndPointUri;

            JObject leagueScoreboard = _yahooHomeController.GenerateYahooResourceJObject(uriLeagueScoreboard);

            _h.PrintJObjectItems(leagueScoreboard);
        }

        // Step 2
        public async Task GetYahooLeagueScoreboardAsync()
        {
            await Task.Run(() => { CreateYahooLeagueScoreboard(); });
        }

        // TODO: this is current void; need to make it so it instantiates a new instance
        // Step 3
        /// <example> https://127.0.0.1:5001/api/yahoo/scoreboard </example>
        [HttpGet]
        [Route("scoreboard")]
        public async Task<IActionResult> ViewLeagueScoreboardAsync()
        {
            await GetYahooLeagueScoreboardAsync();

            string currently = "retrieving league scoreboard";

            return Content($"CURRENT TASK: {currently}");
        }
    }
}