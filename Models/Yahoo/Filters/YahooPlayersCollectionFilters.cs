using System;

namespace BaseballScraper.Models.Yahoo.Filters
{
    public partial class YahooPlayersCollectionFilters
    {
        public string[] Positions { get; set; }
        public string[] Statuses { get; set; }
        public string Search { get; set; }
        public string Sort { get; set; }
        public string SortType { get; set; }
        public string SortSeason { get; set; }
        public string SortWeek { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartIndex { get; set; }
        public string Count { get; set; }
    }
}
