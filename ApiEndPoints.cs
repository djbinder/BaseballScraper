using System;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Http;

namespace BaseballScraper
{
    #pragma warning disable CS0414, CS0219, CS0169
    public class ApiEndPoints
    {
        private readonly string baseUri  = "https://fantasysports.yahooapis.com/fantasy/v2";
        private const string LoginString = ";use_login=1";
        private string EndPointType      = "";


        public class EndPoint
        {
            public string BaseUri { get; set; }
            public string ResourceType { get; set; }
            public string EndPointUri { get { return BaseUri + ResourceType; }}
        }


        // https://developer.yahoo.com/fantasysports/guide/game-resource.html
        #region GAMES end points
            public EndPoint GameBaseEndPoint(string gamekey)
            {
                EndPointType = "game";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{gamekey}"
                };
            }

            public EndPoint GameLeaguesEndPoint(string gamekey, string leaguekey)
            {
                EndPointType = "game";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{gamekey}/leagues;league_keys={leaguekey}"
                };
            }

            public EndPoint GamePlayersEndPoint(string gamekey, string playerkey)
            {
                EndPointType = "game";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{gamekey}/players;player_keys={playerkey}"
                };
            }

            public EndPoint GameWeeksEndPoint(string gamekey)
            {
                EndPointType = "game";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{gamekey}/game_weeks"
                };
            }

            public EndPoint GameStatCategoriesEndPoint(string gamekey)
            {
                EndPointType = "game";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{gamekey}/stat_categories"
                };
            }

            public EndPoint GamePositionTypesEndPoint(string gamekey)
            {
                EndPointType = "game";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{gamekey}/position_types"
                };
            }

            public EndPoint GameRosterPositionsEndPoint(string gamekey)
            {
                EndPointType = "game";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{gamekey}/roster_positions"
                };
            }
        #endregion GAMES end points


        // https://developer.yahoo.com/fantasysports/guide/league-resource.html
        #region LEAGUE end points

            // var uriLeagueBase = endPoints.LeagueBaseEndPoint(leagueKey).EndPointUri;
            public EndPoint LeagueBaseEndPoint(string leagueKey)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}"
                };
            }

            // var uriLeagueSettings = endPoints.LeagueSettingsEndPoint(leagueKey).EndPointUri;
            public EndPoint LeagueSettingsEndPoint (string leagueKey)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/settings"
                };
            }

            // var uriLeagueStandings = endPoints.LeagueStandingsEndPoint(leagueKey).EndPointUri;
            public EndPoint LeagueStandingsEndPoint (string leagueKey)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/standings"
                };
            }

            // var uriLeagueSeasonScoreboard = endPoints.LeagueSeasonScoreboardEndPoint(leagueKey).EndPointUri;
            public EndPoint LeagueSeasonScoreboardEndPoint (string leagueKey)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/scoreboard"
                };
            }

            // var uriLeagueOneWeekScoreboard = endPoints.LeagueOneWeekScoreboardEndPoint(leagueKey, leagueWeekNumber).EndPointUri;
            public EndPoint LeagueOneWeekScoreboardEndPoint (string leagueKey, int weeknumber)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/scoreboard;week={weeknumber}"
                };
            }

            // var uriLeagueTeams = endPoints.LeagueTeamsEndPoint(leagueKey).EndPointUri;
            public EndPoint LeagueTeamsEndPoint (string leagueKey)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/teams"
                };
            }

            // var uriLeaguePlayers = endPoints.LeaguePlayersEndPoint(leagueKey).EndPointUri;
            public EndPoint LeaguePlayersEndPoint (string leagueKey)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/players"
                };
            }

            // var uriLeagueDraftResults = endPoints.LeagueDraftResultsEndPoint(leagueKey).EndPointUri;
            public EndPoint LeagueDraftResultsEndPoint (string leagueKey)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/draftresults"
                };
            }

            // var uriLeagueTransactions = endPoints.LeagueTransactionsEndPoint(leagueKey).EndPointUri;
            public EndPoint LeagueTransactionsEndPoint (string leagueKey)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/transactions"
                };
            }

        #endregion LEAGUE end points

        //https://developer.yahoo.com/fantasysports/guide/team-resource.html
        #region TEAM end points

            // var uriTeamBase = endPoints.TeamBaseEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamBaseEndPoint (string leaguekey, int teamnumber)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamnumber}"
                };
            }

            // var uriTeamStatsSeason = endPoints.TeamSeasonStatsEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamSeasonStatsEndPoint (string leaguekey, int teamnumber)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamnumber}/stats"
                };
            }

            // var uriTeamStatsWeek = endPoints.TeamWeeksStatsEndPoint(leagueKey, teamId, weekNumber).EndPointUri;
            public EndPoint TeamWeeksStatsEndPoint (string leaguekey, int teamnumber, int weeknumber)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamnumber}/stats;type=week;week={weeknumber}"
                };
            }

            // var uriTeamStandings = endPoints.TeamStandingsEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamStandingsEndPoint (string leaguekey, int teamnumber)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamnumber}/standings"
                };
            }

            // var uriTeamRoster = endPoints.TeamRosterEndPoint(leagueKey, teamId, weekNumber).EndPointUri;
            public EndPoint TeamRosterEndPoint (string leaguekey, int teamnumber, int weeknumber)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamnumber}/roster;week={weeknumber}"
                };
            }

            // var uriTeamDraftResults = endPoints.TeamDraftResultsEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamDraftResultsEndPoint (string leaguekey, int teamnumber)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamnumber}/draftresults"
                };
            }

            // var uriTeamAllMatchups = endPoints.TeamAllMatchupsEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamAllMatchupsEndPoint (string leaguekey, int teamnumber)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamnumber}/matchups"
                };
            }

            public EndPoint TeamSelectedMatchupsEndPoint (string leaguekey, int teamnumber, int[] weeknumbers)
            {
                EndPointType = "team";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{leaguekey}.t.{teamnumber}/matchups;weeks={weeknumbers}"
                };
            }
        #endregion TEAM end points


        // https://developer.yahoo.com/fantasysports/guide/roster-resource.html
        #region ROSTER end point
            public EndPoint RosterPlayersEndPoint (string leaguekey, int teamnumber)
            {
                EndPointType = "roster";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamnumber}/{EndPointType}/players"
                };
            }
        #endregion ROSTER end point


        // https://developer.yahoo.com/fantasysports/guide/player-resource.html
        #region PLAYER end points
            public EndPoint PlayerBaseEndPoint (string playerkey)
            {
                EndPointType = "player";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{playerkey}"
                };
            }

            public EndPoint PlayerSeasonStatsEndPoint (string playerkey)
            {
                EndPointType = "player";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{playerkey}/stats"
                };
            }

            public EndPoint PlayerWeeksStatsEndPoint (string playerkey, int weeknumber)
            {
                EndPointType = "player";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{playerkey}/stats;type=week;week={weeknumber}"
                };
            }

            public EndPoint PlayerOwnershipEndPoint (string leaguekey, string playerkey)
            {
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leaguekey}/players;player_keys={playerkey}/ownership"
                };
            }

            public EndPoint PlayerPercentOwnedEndPoint (string playerkey)
            {
                EndPointType = "player";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{playerkey}/percent_owned"
                };
            }

            public EndPoint PlayerDraftAnalysisEndPoint (string playerkey)
            {
                EndPointType = "player";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{playerkey}/draft_analysis"
                };
            }
        #endregion PLAYER end points


        // https://developer.yahoo.com/fantasysports/guide/transaction-resource.html
        #region TRANSACTION end points
            public EndPoint TransactionBaseEndPoint (string transactionkey)
            {
                EndPointType = "transaction";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{transactionkey}"
                };
            }

            public EndPoint TransactionPlayersEndPoint (string transactionkey)
            {
                EndPointType = "transaction";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{transactionkey}/players"
                };
            }
        #endregion TRANSACTION end points


        // https://developer.yahoo.com/fantasysports/guide/user-resource.html
        #region USER end points
            public EndPoint YahooUserBaseEndPoint ()
            {
                EndPointType = "users";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType};{LoginString}/games"
                };
            }

            public EndPoint YahooUsersLeaguesEndPoint (string gamekey)
            {
                EndPointType = "users";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType};{LoginString}/games;games_keys={gamekey}/leagues"
                };
            }

            public EndPoint YahooUsersTeamsEndPoint (string gamekey)
            {
                EndPointType = "users";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType};{LoginString}/games;games_keys={gamekey}/teams"
                };
            }
        #endregion USER end points
    }
}