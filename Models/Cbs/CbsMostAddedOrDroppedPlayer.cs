// https://www.cbssports.com/fantasy/baseball/trends/added/all


namespace BaseballScraper.Models.Cbs
{
    public class CbsRosterTrendPlayer
    {
        public string CbsRosterTrendPlayerName { get; set; }
    }

    public class CbsMostAddedOrDroppedPlayer: CbsRosterTrendPlayer
    {
        // public string CbsMostAddedOrDroppedName { get; set; }
        public string CbsRankPreviousWeek { get; set; }
        public string CbsRankCurrentWeek { get; set; }
        public string CbsDifferenceBetweenCurrentWeekAndPreviousWeek { get; set; }
    }

    public class CbsMostViewedPlayer : CbsRosterTrendPlayer
    {
        public string CbsRecentViews { get; set; }
        public string CbsTodaysViews { get; set; }
    }

    public class CbsMostTradedPlayer : CbsRosterTrendPlayer
    {
        public string CbsNumberOfTrades { get; set; }
    }
}