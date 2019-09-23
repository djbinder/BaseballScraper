using System.Collections.Generic;
using BaseballScraper.Models.Cbs;
using BaseballScraper.Models.Espn;
using BaseballScraper.Models.Yahoo;

#pragma warning disable MA0016
namespace BaseballScraper.Models.ViewModels
{
    public class TransactionTrendsViewModel
    {
        public List<CbsMostAddedOrDroppedPlayer> CbsPlayers { get; set; } 
        public List<YahooTransactionTrendsPlayer> YahooPlayers { get; set; }
        public List<EspnTransactionTrendPlayer> EspnPlayers { get; set; } 


        public TransactionTrendsViewModel()
        {
            CbsPlayers = new List<CbsMostAddedOrDroppedPlayer>();
            YahooPlayers  = new List<YahooTransactionTrendsPlayer>();
            EspnPlayers = new List<EspnTransactionTrendPlayer>();
        }
    }
}