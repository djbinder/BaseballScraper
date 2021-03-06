namespace BaseballScraper.EndPoints
{
    public class MlbDataApiEndPoints
    {
        private readonly string baseUri         = $"http://lookup-service-prod.mlb.com/json/named.";
        private readonly string baseUriStatsApi = $"https://statsapi.mlb.com/api/";

        private string endPointType     = "";

        public class MlbDataEndPoint
        {
            public string BaseUri     { get; set; }
            public string EndPoint    { get; set; }
            public string EndPointUri { get { return BaseUri + EndPoint; }}
        }


        // * See: https://appac.github.io/mlb-data-api-docs/#player-data-player-search-get
        // * /json/named.proj_pecota_batting.bam?season={season}&player_id={player_id}
        public MlbDataEndPoint PlayerSearchEndPoint(string lastName)
        {
            endPointType = "search_player_all";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&active_sw='Y'&name_part='{lastName}%25'",
            };
        }


        // * See: https://appac.github.io/mlb-data-api-docs/#player-data-player-search-get
        // * Endpoint: /json/named.proj_pecota_batting.bam?season={season}&player_id={player_id}
        public MlbDataEndPoint PlayerSearchEndPoint(string lastName, bool currentlyActive)
        {
            endPointType = "search_player_all";

            string activeStatus = "";

            if(currentlyActive)
                activeStatus = "Y";

            if(!currentlyActive)
                activeStatus = "N";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&active_sw='{activeStatus}'&name_part='{lastName}%25'",
            };
        }


        // * See: https://appac.github.io/mlb-data-api-docs/#player-data-player-info-get
        // * Endpoint: /json/named.player_info.bam?sport_code='mlb'&player_id={player_id}
        public MlbDataEndPoint PlayerInfoEndPoint (int playerId)
        {
            endPointType = "player_info";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&player_id='{playerId}'",
            };
        }


        // * See: https://appac.github.io/mlb-data-api-docs/#stats-data-projected-pitching-stats-get
        // * Endpoint: /json/named.proj_pecota_pitching.bam?season={season}&player_id={player_id}
        public MlbDataEndPoint ProjectedPitchingStatsEndPoint(int year, int playerId)
        {
            endPointType = "proj_pecota_pitching";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?season={year}&player_id={playerId}",
            };
        }


        // * See: https://appac.github.io/mlb-data-api-docs/#stats-data-projected-hitting-stats-get
        // * Endpoint: /json/named.proj_pecota_batting.bam?season={season}&player_id={player_id}
        public MlbDataEndPoint ProjectedHittingStatsEndPoint(int year, int playerId)
        {
            endPointType = "proj_pecota_batting";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?season={year}&player_id={playerId}",
            };
        }


        // OPTION 1 - regular season stats only
        // * See: https://appac.github.io/mlb-data-api-docs/#stats-data-season-pitching-stats-get
        // * Endpoint: /json/named.sport_pitching_tm.bam?league_list_id='mlb'&game_type={game_type}&season={season}&player_id={player_id}
        /// <summary>
        ///     Creates end point for a selected regular season
        /// </summary>
        /// <param name="results">
        ///     The number of pitchers to include in the results
        /// </param>
        /// <param name="year">
        ///     The year of the season you want to query
        /// </param>
        /// <param name="sortColumn">
        ///     The stat that you would like to sort by (e.g. ERA)
        /// </param>
        /// <returns>
        ///     List of current season pitching leaders
        /// </returns>
        public MlbDataEndPoint PitchingLeadersEndPoint(int results, string year, string sortColumn)
        {
            endPointType = "leader_pitching_repeater";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&results={results}&game_type='R'&season='{year}'&sort_column='{sortColumn}'",
            };
        }


        // OPTION 2 - define part of season for stats you want to retrieve (e.g., Spring Training)
        // https://appac.github.io/mlb-data-api-docs/#stats-data-season-pitching-stats-get
        // Endpoint: /json/named.sport_pitching_tm.bam?league_list_id='mlb'&game_type={game_type}&season={season}&player_id={player_id}
        /// <summary>
        ///     Creates end point for a selected year and part of the season (e.g. Spring Training, Regular Season, etc. )
        /// </summary>
        /// <remarks>
        ///     This is a more specific version of the previous end point
        /// </remarks>
        /// <param name="results">
        ///     The number of pitchers to include in the results
        /// </param>
        /// <param name="gameType">
        ///     The part of the season
        ///     Game type options:
        ///     * R - Regular Season
        ///     * S - Spring Training
        ///     * E - Exhibition
        ///     * A - All Star Game
        ///     * D - Division Series
        ///     * F - First Round (Wild Card)
        ///     * L - League Championship
        ///     * W - World Series
        /// </param>
        /// <param name="year">
        ///     The year of the season you want to query
        /// </param>
        /// <param name="sortColumn">
        ///     The stat that you would like to sort by (e.g. ERA)
        /// </param>
        /// <returns>
        ///     List of pitching leaders for defined season and game type
        /// </returns>
        public MlbDataEndPoint PitchingLeadersEndPoint(int results, string gameType, string year, string sortColumn)
        {
            endPointType = "leader_pitching_repeater";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&results={results}&game_type='{gameType}'&season='{year}'&sort_column='{sortColumn}'",
            };
        }


        // OPTION 1 - regular season stats only
        // https://appac.github.io/mlb-data-api-docs/#reports-hitting-leaders-get
        // Endpoint: /json/named.leader_hitting_repeater.bam?sport_code='mlb'&results={results}&game_type={game_type}&season={season}&sort_column={sort_column}&leader_hitting_repeater.col_in={leader_hitting_repeater.col_in}
        /// <summary>
        ///     Creates end point for a selected regular season
        /// </summary>
        /// <param name="results">
        ///     The number of hitters to include in the results
        /// </param>
        /// <param name="year">
        ///     The year of the season you want to query
        /// </param>
        /// <param name="sortColumn">
        ///     The stat that you would like to sort by (e.g. ops)
        /// </param>
        /// <returns>
        ///     List of current season hitting leaders
        /// </returns>
        public MlbDataEndPoint HittingLeadersEndPoint(int results, string year, string sortColumn)
        {
            endPointType = "leader_hitting_repeater";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&results={results}&game_type='R'&season='{year}'&sort_column='{sortColumn}'",
            };
        }


        // OPTION 2 - define part of season for stats you want to retrieve (e.g., Spring Training)
        // https://appac.github.io/mlb-data-api-docs/#reports-hitting-leaders-get
        // Endpoint: /json/named.leader_hitting_repeater.bam?sport_code='mlb'&results={results}&game_type={game_type}&season={season}&sort_column={sort_column}&leader_hitting_repeater.col_in={leader_hitting_repeater.col_in}
        /// <summary>
        ///     Creates end point for a selected year and part of the season (e.g. Spring Training, Regular Season, etc. )
        /// </summary>
        /// <remarks>
        ///     This is a more specific version of the previous end point
        /// </remarks>
        /// <param name="results">
        ///     The number of hitters to include in the results
        /// </param>
        /// <param name="gameType">
        ///     The part of the season
        ///     Game type options:
        ///     * R - Regular Season
        ///     * S - Spring Training
        ///     * E - Exhibition
        ///     * A - All Star Game
        ///     * D - Division Series
        ///     * F - First Round (Wild Card)
        ///     * L - League Championship
        ///     * W - World Series
        /// </param>
        /// <param name="year">
        ///     The year of the season you want to query
        /// </param>
        /// <param name="sortColumn">
        ///     The stat that you would like to sort by (e.g. Rbi)
        /// </param>
        /// <returns>
        ///     List of hitting leaders for defined season and game type
        /// </returns>
        public MlbDataEndPoint HittingLeadersEndPoint(int results, string gameType, string year, string sortColumn)
        {
            endPointType = "leader_hitting_repeater";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?sport_code='mlb'&results={results}&game_type='{gameType}'&season='{year}'&sort_column='{sortColumn}'",
            };
        }


        // https://appac.github.io/mlb-data-api-docs/#stats-data-season-hitting-stats-get
        // /json/named.sport_hitting_tm.bam?league_list_id='mlb'&game_type={game_type}&season={season}&player_id={player_id}
        // GET http://lookup-service-prod.mlb.com/json/named.sport_hitting_tm.bam?league_list_id='mlb'&game_type='R'&season='2017'&player_id='493316'
        public MlbDataEndPoint HitterSeasonEndPoint(string gameType, string year, string playerId)
        {
            endPointType = "sport_hitting_tm";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?league_list_id='mlb'&game_type='{gameType}'&season='{year}'&player_id='{playerId}'",
            };
        }


        // Retrieve the teams a player has played for over the course of a season, or their career.
        // https://appac.github.io/mlb-data-api-docs/#player-data-player-teams-get
        // /json/named.player_teams.bam?season={season}&player_id={player_id}
        // Omitting the season parameter will retrieve all teams the player has played for since the start of their career.
        // season string (optional) Example: '2014'
        // player_id string (required) Example: '493316'
        // GET http://lookup-service-prod.mlb.com/json/named.player_teams.bam?season='2014'&player_id='493316'
        public MlbDataEndPoint PlayerTeamsEndPoint(string season, string playerId)
        {
            endPointType = "player_teams";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?season={season}&player_id={playerId}",
            };
        }


        public MlbDataEndPoint PlayerTeamsEndPoint(string playerId)
        {
            endPointType = "player_teams";

            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{endPointType}.bam?player_id={playerId}",
            };
        }


        private readonly int sportId = 1;
        private readonly string versionOne = "v1";


        public MlbDataEndPoint AllGamesForDateEndPoint(int monthNumber, int dayNumber, int year)
        {
            return new MlbDataEndPoint
            {
                BaseUri  = baseUriStatsApi,
                EndPoint = $"{versionOne}/schedule?sportId={sportId}&date={monthNumber}%2F{dayNumber}%2F{year}",
            };
        }


        // gameId aka gamePk
        public MlbDataEndPoint SingleGameEndPoint(string gameId)
        {
            return new MlbDataEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"{versionOne}/game/{gameId}/feed/live",
            };
        }
    }
}
