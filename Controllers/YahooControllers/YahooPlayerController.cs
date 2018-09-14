using System;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    [Route("api/yahoo")]
    [ApiController]
    public class YahooPlayerController: Controller
    {
        private Helpers _h = new Helpers();

        [Route("player")]
        public void PlayerInfo()
        {
            _h.StartMethod();
            // these are for paul goldschmidt
            string pKey = "378.p.8967";
            string pId  = "8967";
        }
    }
}