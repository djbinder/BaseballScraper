namespace BaseballScraper.EndPoints
{
    public class MlbDataApiEndPoints
    {
        private Constants _c = new Constants();

        private readonly string baseUri = $"http://lookup-service-prod.mlb.com/json/named.";
        public  string endPointType     = "";

        public class MlbDataEndPoint
        {
            public string EndPointType { get; set; }
            public string BaseUri { get; set; }
            public string EndPoint { get; set; }
            public string EndPointUri { get { return BaseUri + EndPoint; }}
        }

        // http://lookup-service-prod.mlb.com/json/named.search_player_all.bam?sport_code='mlb'&active_sw='Y'&name_part='hernandez%25'
        public MlbDataEndPoint PlayerSearchEndPoint(string lastName)
        {
            // _c.Start.ThisMethod();

            endPointType = "search_player_all";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&active_sw='Y'&name_part='{lastName}%25'"
            };
        }
        public MlbDataEndPoint PlayerSearchEndPoint(string lastName, bool currentlyActive)
        {
            endPointType = "search_player_all";

            string activeStatus = "";

            if(currentlyActive == true)
            {
                activeStatus = "Y";
            }

            if(currentlyActive == false)
            {
                activeStatus = "N";
            }

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&active_sw='{activeStatus}'&name_part='{lastName}%25'"
            };
        }

        /// <summary> Creates end point for a selected regular season </summary>
        /// <param name="results"> The number of pitchers to include in the results </param>
        /// <param name="year"> The year of the season you want to query </param>
        /// <param name="sortColumn"> The stat that you would like to sort by (e.g. ERA) </param>
        /// <returns> List of current season pitching leaders </returns>
        /// <example> http://lookup-service-prod.mlb.com/json/named.leader_pitching_repeater.bam?sport_code='mlb'&results=5&game_type='R'&season='2017'&sort_column='era'&leader_pitching_repeater.col_in=era </example>
        public MlbDataEndPoint PitchingLeadersEndPoint(int results, string year, string sortColumn)
        {
            endPointType = "leader_pitching_repeater";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"http://lookup-service-prod.mlb.com/json/named.leader_pitching_repeater.bam?sport_code='mlb'&results={results}&game_type='R'&season='{year}'&sort_column='{sortColumn}'"
            };
        }

        /// <summary> Creates end point for a selected year and part of the season (e.g. Spring Training, Regular Season, etc. ) </summary>
        /// <remarks> This is a more specific version of the previous end point </remarks>
        /// <param name="results"> The number of pitchers to include in the results </param>
        /// <param name="gameType"> The part of the season </param>
        ///     <list type="gameTypeOptions">
        ///         <item> R - Regular Season
        ///         <item> S - Spring Training
        ///         <item> E - Exhibition
        ///         <item> A - All Star Game
        ///         <item> D - Division Series
        ///         <item> F - First Round (Wild Card)
        ///         <item> L - League Championship
        ///         <item> W - World Series
        /// <param name="year"> The year of the season you want to query </param>
        /// <param name="sortColumn"> The stat that you would like to sort by (e.g. ERA) </param>
        /// <returns> List of pitching leaders for defined season and game type</returns>
        public MlbDataEndPoint PitchingLeadersEndPoint(int results, string gameType, string year, string sortColumn)
        {
            endPointType = "leader_pitching_repeater";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"http://lookup-service-prod.mlb.com/json/named.leader_pitching_repeater.bam?sport_code='mlb'&results={results}&game_type='{gameType}'&season='{year}'&sort_column='{sortColumn}'"
            };
        }
    }
}