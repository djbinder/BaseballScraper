using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;




#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooLeagueResourceController : ControllerBase
    {
        private readonly Helpers _h = new Helpers();

        // Yahoo Doc Link: https://developer.yahoo.com/fantasysports/guide/#league-resource
        // League resource description (from Yahoo):
        //      * League API gets league related information
        //      * Examples: league name, the number of teams, the draft status
        //      * Leagues only exist in the context of a particular Game
        //      * You can request a League Resource as the uri base by using the global ````
        //      * A user can only get data for private leagues they are in, or public leagues
        //  URIs
        //      * https://fantasysports.yahooapis.com/fantasy/v2/league/
        //      * https://fantasysports.yahooapis.com/fantasy/v2/league//
        //      * https://fantasysports.yahooapis.com/fantasy/v2/league/;out=,{sub_resource_2}




        [Route("test")]
        public void TestYahooLeagueResourceController()
        {
            _h.StartMethod();

            // var tsOne = CreateYahooTeamStatsModel(1);
        }
    }
}
