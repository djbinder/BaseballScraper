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


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Pages
{

    public class TransactionTrendsPage : PageModel
    {
        private readonly Helpers                          _helpers;
        private readonly CbsTransactionTrendsController   _cbsTrendsController;
        private readonly EspnTransactionTrendsController  _espnTrendsController;
        private readonly YahooTransactionTrendsController _yahooTrendsController;


        public IList<CbsMostAddedOrDroppedPlayer> CbsPlayers    { get; set; }
        public IList<EspnTransactionTrendPlayer> EspnPlayers    { get; set; }
        public IList<YahooTransactionTrendsPlayer> YahooPlayers { get; set; }


        private const string cbsUrlForMostAddedAllFootball = "https://www.cbssports.com/fantasy/football/trends/added/all";
        private const string cbsUrlForMostAddedAllBaseball = "https://www.cbssports.com/fantasy/baseball/trends/added/all";


        public TransactionTrendsPage(Helpers helpers, CbsTransactionTrendsController cbsTrendsController, EspnTransactionTrendsController espnTrendsController, YahooTransactionTrendsController yahooTrendsController)
        {
            _helpers = helpers;
            _cbsTrendsController = cbsTrendsController;
            _espnTrendsController = espnTrendsController;
            _yahooTrendsController = yahooTrendsController;
        }

        // https://127.0.0.1:5001/transaction_trends
        public IActionResult OnGet()
        {
            _helpers.StartMethod();

            List<CbsMostAddedOrDroppedPlayer> cbsPlayers    = _cbsTrendsController.GetListOfCbsMostAddedOrDropped(cbsUrlForMostAddedAllBaseball);
            List<EspnTransactionTrendPlayer> espnPlayers    = _espnTrendsController.GetListOfMostAddedPlayers();
            List<YahooTransactionTrendsPlayer> yahooPlayers = _yahooTrendsController.GetTrendsForTodayAllPositions();

            CbsPlayers   = cbsPlayers;
            EspnPlayers  = espnPlayers;
            YahooPlayers = yahooPlayers;

            // Console.WriteLine($"Cbs Count: {cbsPlayers.Count}");
            // Console.WriteLine($"Espn Count: {espnPlayers.Count}");
            // Console.WriteLine($"Yahoo Count: {yahooPlayers.Count}");
            return Page();
        }
    }
}
