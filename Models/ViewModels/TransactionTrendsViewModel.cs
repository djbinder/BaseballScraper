using System.Collections.Generic;
using BaseballScraper.Models.Cbs;
using BaseballScraper.Models.Espn;
using BaseballScraper.Models.Yahoo;

namespace BaseballScraper.Models.ViewModels
{
    public class TransactionTrendsViewModel
    {
        public List<CbsMostAddedOrDroppedPlayer> CbsPlayers { get; set; } = new List<CbsMostAddedOrDroppedPlayer>();
        public List<YahooTransactionTrendsPlayer> YahooPlayers { get; set; } = new List<YahooTransactionTrendsPlayer>();
        public List<EspnTransactionTrendPlayer> EspnPlayers { get; set; } = new List<EspnTransactionTrendPlayer>();
    }
}