using System;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    public class YahooPlayerController: Controller
    {
        private Constants _c = new Constants();
        public void PlayerInfo()
        {
            _c.Start.ThisMethod();
            // these are for paul goldschmidt
            string pKey = "378.p.8967";
            string pId  = "8967";
        }
    }
}