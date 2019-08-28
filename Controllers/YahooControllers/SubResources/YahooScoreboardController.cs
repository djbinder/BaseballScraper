using BaseballScraper.Controllers.YahooControllers.Resources;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.ConfigurationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class YahooScoreboardController: ControllerBase
    {
        private readonly Helpers _h                         = new Helpers();
        private static readonly YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private static YahooApiRequestController _yahooApiRequestController;

        public static readonly YahooGameResourceConroller _yahooGameResourceController = new YahooGameResourceConroller();


        public YahooScoreboardController(IOptions<TheGameIsTheGameConfiguration> theGameConfig, YahooApiRequestController yahooApiRequestController)
        {
            _theGameConfig       = theGameConfig.Value;
            _yahooApiRequestController = yahooApiRequestController;
        }


        [Route("test")]
        public void TestYahooManagerController()
        {
            CreateYahooLeagueScoreboard();
        }


        // TODO: add XML summary comments
        // Step 1
        public void CreateYahooLeagueScoreboard ()
        {
            _h.StartMethod();

            // retrieve the league key from user secrets / yahoo league config
            string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();

            var uriLeagueScoreboard = endPoints.LeagueSeasonScoreboardEndPoint(leagueKey).EndPointUri;

            JObject leagueScoreboard = _yahooApiRequestController.GenerateYahooResourceJObject(uriLeagueScoreboard);

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
