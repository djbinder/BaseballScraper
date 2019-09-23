using System.Collections.Generic;

#pragma warning disable MA0016, MA0048
namespace BaseballScraper.Models.Yahoo
{
    public class YahooScoreboard
    {
        public string Week { get; set; }
        public List<YahooMatchups> YahooMatchups { get; set; }

        public YahooScoreboard()
        {
            YahooMatchups = new List<YahooMatchups>();
        }
    }
}