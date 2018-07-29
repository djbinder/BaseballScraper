using System;
using System.Collections.Generic;
using System.Text;

using BaseballScraper.Models;

namespace BaseballScraper.Client
{

    public class GameCollectionFilters
    {
        public bool? IsAvailable { get; set; }
        public int[] Seasons { get; set; }
        public GameCode[] GameCodes { get; set; }
        public GameType[] GameTypes { get; set; }
    }


    public class PlayerCollectionFilters
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
