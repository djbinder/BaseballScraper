using System;
using System.Collections.Generic;
using BaseballScraper.Controllers.CbsControllers;
using BaseballScraper.Controllers.EspnControllers;
using BaseballScraper.Controllers.YahooControllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Cbs;
using BaseballScraper.Models.Espn;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseballScraper.Pages
{
    #pragma warning disable CS0414
    public class TransactionTrends : PageModel
    {
        private readonly Helpers _h = new Helpers();
        private readonly CbsTransactionTrendsController _c = new CbsTransactionTrendsController();
        private readonly EspnTransactionTrendsController _e = new EspnTransactionTrendsController();
        private readonly YahooTransactionTrendsController _y = new YahooTransactionTrendsController();

        public IList<CbsMostAddedOrDroppedPlayer> CbsPlayers { get; set; }
        public IList<EspnTransactionTrendPlayer> EspnPlayers { get; set; }
        public IList<YahooTransactionTrendsPlayer> YahooPlayers { get; set; }

        private const string cbsUrlForMostAddedAllFootball = "https://www.cbssports.com/fantasy/football/trends/added/all";
        private const string cbsUrlForMostAddedAllBaseball = "https://www.cbssports.com/fantasy/baseball/trends/added/all";



        public TransactionTrends() {}


        public IActionResult OnGet()
        {
            _h.StartMethod();

            // List<CbsMostAddedOrDroppedPlayer> cbsPlayers = _c.GetListOfCbsMostAddedOrDropped(cbsUrlForMostAddedAllBaseball);
            // Console.WriteLine($"Cbs Count: {cbsPlayers.Count}");
            // CbsPlayers = cbsPlayers;

            // List<EspnTransactionTrendPlayer> espnPlayers = _e.GetListOfMostAddedPlayers();
            // Console.WriteLine($"Espn Count: {espnPlayers.Count}");
            // EspnPlayers = espnPlayers;


            List<YahooTransactionTrendsPlayer> yahooPlayers = _y.GetTrendsForTodayAllPositions();
            // Console.WriteLine($"Yahoo Count: {yahooPlayers.Count}");
            YahooPlayers = yahooPlayers;

            return Page();
        }
    }
}
