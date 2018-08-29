using System;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.EndPoints
{
    public class MlbDataApiEndPoints
    {
        private Constants _c            = new Constants();
        private readonly string baseUri = $"http://lookup-service-prod.mlb.com/json/named.";
        public  string endPointType     = "";

        public class MlbDataEndPoint
        {
            // public string EndPointType { get; set; }
            public string BaseUri { get; set; }
            public string EndPoint { get; set; }
            public string EndPointUri { get { return BaseUri + EndPoint; }}
        }

        // https://appac.github.io/mlb-data-api-docs/#player-data-player-search-get
        // /json/named.proj_pecota_batting.bam?season={season}&player_id={player_id}
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
        // https://appac.github.io/mlb-data-api-docs/#player-data-player-search-get
        // Endpoint: /json/named.proj_pecota_batting.bam?season={season}&player_id={player_id}
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

        // https://appac.github.io/mlb-data-api-docs/#player-data-player-info-get
        // Endpoint: /json/named.player_info.bam?sport_code='mlb'&player_id={player_id}
        public MlbDataEndPoint PlayerInfoEndPoint (int playerId)
        {
            endPointType = "player_info";
            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&player_id='{playerId}'"
            };
        }

        // https://appac.github.io/mlb-data-api-docs/#stats-data-projected-pitching-stats-get
        // Endpoint: /json/named.proj_pecota_pitching.bam?season={season}&player_id={player_id}
        public MlbDataEndPoint ProjectedPitchingStatsEndPoint(int year, int playerId)
        {
            endPointType = "proj_pecota_pitching";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?season={year}&player_id={playerId}"
            };
        }

        // https://appac.github.io/mlb-data-api-docs/#stats-data-projected-hitting-stats-get
        // Endpoint: /json/named.proj_pecota_batting.bam?season={season}&player_id={player_id}
        public MlbDataEndPoint ProjectedHittingStatsEndPoint(int year, int playerId)
        {
            endPointType = "proj_pecota_batting";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?season={year}&player_id={playerId}"
            };
        }

        // OPTION 1 - regular season stats only
        // https://appac.github.io/mlb-data-api-docs/#stats-data-season-pitching-stats-get
        // Endpoint: /json/named.sport_pitching_tm.bam?league_list_id='mlb'&game_type={game_type}&season={season}&player_id={player_id}
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
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&results={results}&game_type='R'&season='{year}'&sort_column='{sortColumn}'"
            };
        }

        // OPTION 2 - define part of season for stats you want to retrieve (e.g., Spring Training)
        // https://appac.github.io/mlb-data-api-docs/#stats-data-season-pitching-stats-get
        // Endpoint: /json/named.sport_pitching_tm.bam?league_list_id='mlb'&game_type={game_type}&season={season}&player_id={player_id}
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
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&results={results}&game_type='{gameType}'&season='{year}'&sort_column='{sortColumn}'"
            };
        }

        // OPTION 1 - regular season stats only
        // https://appac.github.io/mlb-data-api-docs/#reports-hitting-leaders-get
        // Endpoint: /json/named.leader_hitting_repeater.bam?sport_code='mlb'&results={results}&game_type={game_type}&season={season}&sort_column={sort_column}&leader_hitting_repeater.col_in={leader_hitting_repeater.col_in}
        /// <summary> Creates end point for a selected regular season </summary>
        /// <param name="results"> The number of hitters to include in the results </param>
        /// <param name="year"> The year of the season you want to query </param>
        /// <param name="sortColumn"> The stat that you would like to sort by (e.g. ops) </param>
        /// <returns> List of current season hitting leaders </returns>
        /// <example> http://lookup-service-prod.mlb.com/json/named.leader_hitting_repeater.bam?sport_code='mlb'&results=5&game_type='R'&season='2017'&sort_column='ab'&leader_hitting_repeater.col_in=ab </example>
        public MlbDataEndPoint HittingLeadersEndPoint(int results, string year, string sortColumn)
        {
            endPointType = "leader_hitting_repeater";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&results={results}&game_type='R'&season='{year}'&sort_column='{sortColumn}'"
            };
        }

        // OPTION 2 - define part of season for stats you want to retrieve (e.g., Spring Training)
        // https://appac.github.io/mlb-data-api-docs/#reports-hitting-leaders-get
        // Endpoint: /json/named.leader_hitting_repeater.bam?sport_code='mlb'&results={results}&game_type={game_type}&season={season}&sort_column={sort_column}&leader_hitting_repeater.col_in={leader_hitting_repeater.col_in}
        /// <summary> Creates end point for a selected year and part of the season (e.g. Spring Training, Regular Season, etc. ) </summary>
        /// <remarks> This is a more specific version of the previous end point </remarks>
        /// <param name="results"> The number of hitters to include in the results </param>
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
        /// <param name="sortColumn"> The stat that you would like to sort by (e.g. Rbi) </param>
        /// <returns> List of hitting leaders for defined season and game type</returns>
        public MlbDataEndPoint HittingLeadersEndPoint(int results, string gameType, string year, string sortColumn)
        {
            endPointType = "leader_hitting_repeater";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&results={results}&game_type='{gameType}'&season='{year}'&sort_column='{sortColumn}'"
            };
        }



    }
}