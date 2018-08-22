using System;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    public class YahooPlayerController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        public void PlayerInfo()
        {
            Start.ThisMethod();
            // these are for paul goldschmidt
            string pKey = "378.p.8967";
            string pId  = "8967";
        }
    }
}