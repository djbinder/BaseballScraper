using System;
using BaseballScraper.Controllers.YahooControllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Yahoo;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.EndPoints
{
    public class YahooApiEndPoints
    {
        private readonly Helpers _h = new Helpers();
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
        #region GAMES END POINTS ------------------------------------------------------------

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
        #endregion GAMES END POINTS ------------------------------------------------------------





        // https://developer.yahoo.com/fantasysports/guide/league-resource.html
        #region LEAGUE END POINTS ------------------------------------------------------------

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

            // team_key, team_id, name, is_owned_by_current_login, url, team_logos, waiver_priority, number_of_moves, number_of_trades, roster_adds, league_scoring_type, has_draft_grade, managers, team_stats, team_points, team_standings
            internal string TeamItem(JObject obj, int teamNumber, string itemName)
            {
                var LeagueTeam    = obj["fantasy_content"]["league"]["standings"]["teams"]["team"][teamNumber];
                var TeamLogos     = LeagueTeam["team_logos"];
                var TeamLogo      = TeamLogos["team_logo"];
                var RosterAdds    = LeagueTeam["roster_adds"];
                var TeamStandings = obj["fantasy_content"]["league"]["standings"]["teams"]["team"][teamNumber]["team_standings"];
                var TeamOutcomes  = TeamStandings["outcome_totals"];
                var TeamPoints    = obj["fantasy_content"]["league"]["standings"]["teams"]["team"][teamNumber]["team_points"];
                var Managers      = obj["fantasy_content"]["league"]["standings"]["teams"]["team"][teamNumber]["managers"];
                var Manager       = obj["fantasy_content"]["league"]["standings"]["teams"]["team"][teamNumber]["managers"]["manager"];

                // before being converted to strings, all return types is Newtonsoft.Json.Linq.JValue
                switch (itemName)
                {
                    #region ### LeagueTeam

                        // string teamKey = endPoints.LeagueTeamItem(leagueStandings, 0, "TeamKey");
                        case "TeamKey":
                            return LeagueTeam["team_key"].ToString();

                        case "TeamId":
                            return LeagueTeam["team_id"].ToString();

                        case "TeamName":
                            return LeagueTeam["team_name"].ToString();

                        case "IsOwnedByCurrentLogin":
                            return LeagueTeam["is_owned_by_current_login"].ToString();

                        case "Url":
                            return LeagueTeam["url"].ToString();

                        case "WaiverPriority":
                            return LeagueTeam["waiver_priority"].ToString();

                        case "NumberOfMoves":
                            return LeagueTeam["number_of_moves"].ToString();

                        case "NumberOfTrades":
                            return LeagueTeam["number_of_trades"].ToString();

                        case "LeagueScoringType":
                            return LeagueTeam["league_scoring_type"].ToString();

                        case "HasDraftGrade":
                            return LeagueTeam["has_draft_grade"].ToString();

                    #endregion ### LeagueTeam


                    #region ### RosterAdds ###

                        case "RosterCoverageType":
                            return RosterAdds["coverage_type"].ToString();

                        case "CoverageValue":
                            return RosterAdds["coverage_value"].ToString();

                        case "Value":
                            return RosterAdds["value"].ToString();

                    #endregion ### RosterAdds ###


                    #region ### TeamStandings ###

                        // string teamRank = endPoints.LeagueTeamItem(leagueStandings, 0, "Rank");
                        case "Rank":
                            return TeamStandings["rank"].ToString();

                        case "PlayoffSeed":
                            return TeamStandings["playoff_seed"].ToString();

                        case "GamesBack":
                            return TeamStandings["games_back"].ToString();

                    #endregion ### TeamStandings ###


                    #region ### TeamLogos ###

                        case "TeamLogos":
                            return TeamLogos["team_logo"].ToString();

                    #endregion ### TeamLogos ###


                    // 'team_logo' is nested under 'team_logos'
                    #region ### TeamLogo ###

                        case "TeamLogoSize":
                            return TeamLogo["size"].ToString();

                        case "TeamLogoUrl":
                            return TeamLogo["url"].ToString();

                    #endregion ### TeamLogo ###


                    // 'outcome_totals' is nested under TeamStandings
                    #region ### TeamOutcomes ###

                        case "Wins":
                            return TeamOutcomes["wins"].ToString();

                        case "Losses":
                            return TeamOutcomes["losses"].ToString();

                        case "Ties":
                            return TeamOutcomes["ties"].ToString();

                        case "WinningPercentage":
                            return TeamOutcomes["percentage"].ToString();

                    #endregion ### TeamOutcomes ###


                    #region ### TeamPoints ###

                        case "PointsCoverageType":
                            return TeamPoints["coverage_type"].ToString();

                        case "Season":
                            return TeamPoints["season"].ToString();

                        case "Total":
                            return TeamPoints["total"].ToString();

                    #endregion ### TeamPoints ###


                    #region ### Manager ###

                        case "Manager":
                            return Managers["manager"].ToString();

                    #endregion ### Manager ###


                    #region ### Manager ###

                        case "ManagerId":
                            return Manager["manager_id"].ToString();

                        case "Nickname":
                            return Manager["nickname"].ToString();

                        case "Guid":
                            return Manager["guid"].ToString();

                        case "IsCommissioner":
                            return Manager["is_commissioner"].ToString();

                        case "IsCurrentLogin":
                            return Manager["is_current_login"].ToString();

                        case "Email":
                            return Manager["email"].ToString();

                        case "ImageUrl":
                            return Manager["image_url"].ToString();

                    #endregion ### Manager ###
                }

                string test = "test";
                return test;
            }

        #endregion END POINTS ------------------------------------------------------------





        //https://developer.yahoo.com/fantasysports/guide/team-resource.html
        #region TEAM END POINTS ------------------------------------------------------------

            // var uriTeamBase = endPoints.TeamResourceEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamResourceEndPoint (string leaguekey, int teamnumber)
            {
                EndPointType = "team";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{leaguekey}.t.{teamnumber}"
                };
            }

            // var uriTeamStatsSeason = endPoints.TeamSeasonStatsEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamSeasonStatsEndPoint (string leaguekey, int teamnumber)
            {
                EndPointType = "team";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{leaguekey}.t.{teamnumber}/stats"
                };
            }

            // var uriTeamStatsWeek = endPoints.TeamWeeksStatsEndPoint(leagueKey, teamId, weekNumber).EndPointUri;
            public EndPoint TeamWeeksStatsEndPoint (string leaguekey, int teamnumber, int weeknumber)
            {
                EndPointType = "team";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{leaguekey}.t.{teamnumber}/stats;type=week;week={weeknumber}"
                };
            }

            // var uriTeamStandings = endPoints.TeamStandingsEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamStandingsEndPoint (string leaguekey, int teamnumber)
            {
                EndPointType = "team";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{leaguekey}.t.{teamnumber}/standings"
                };
            }

            // var uriTeamRoster = endPoints.TeamRosterEndPoint(leagueKey, teamId, weekNumber).EndPointUri;
            public EndPoint TeamRosterEndPoint (string leaguekey, int teamnumber, int weeknumber)
            {
                EndPointType = "team";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{leaguekey}.t.{teamnumber}/roster;week={weeknumber}"
                };
            }

            // var uriTeamDraftResults = endPoints.TeamDraftResultsEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamDraftResultsEndPoint (string leaguekey, int teamnumber)
            {
                EndPointType = "team";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{leaguekey}.t.{teamnumber}/draftresults"
                };
            }

            // var uriTeamAllMatchups = endPoints.TeamAllMatchupsEndPoint(leagueKey, teamId).EndPointUri;
            public EndPoint TeamAllMatchupsEndPoint (string leaguekey, int teamnumber)
            {
                EndPointType = "team";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/{EndPointType}/{leaguekey}.t.{teamnumber}/matchups"
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

        #endregion TEAM END POINTS ------------------------------------------------------------





        // https://developer.yahoo.com/fantasysports/guide/roster-resource.html
        // https://fantasysports.yahooapis.com/fantasy/v2/team/253.l.102614.t.10/roster/players
        #region ROSTER RESOURCE END POINTS ------------------------------------------------------------

            public EndPoint RosterResourcePlayersEndPoint(string leaguekey, int teamNumber)
            {
                EndPointType = "roster";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamNumber}/{EndPointType}/players"
                };
            }

            public EndPoint RosterResourceEndPoint(string leaguekey, int teamNumber)
            {
                EndPointType = "roster";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/team/{leaguekey}.t.{teamNumber}/{EndPointType}"
                };
            }




            // public EndPoint RosterPlayersEndPoint (string leaguekey, int teamnumber)
            // {
            //     EndPointType = "roster";
            //     return new EndPoint
            //     {
            //         BaseUri      = baseUri,
            //         ResourceType = $"/team/{leaguekey}.t.{teamnumber}/{EndPointType}/players"
            //     };
            // }

        #endregion ROSTER END POINTS ------------------------------------------------------------





        // https://developer.yahoo.com/fantasysports/guide/#players-collection
        #region PLAYERS COLLECTION END POINTS ------------------------------------------------------------

            // FILTER OPTIONS:
                // position
                    // Valid player positions (e.g., "3B", "SP")
                // status
                    // A (all available players)
                    // FA (free agents only)
                    // W (waivers only)
                    // T (all taken players)
                    // K (keepers only)
                // search
                    // player name
                // sort
                    // {stat_id}
                    // NAME (last, first)
                    // OR (overall rank)
                    // AR (actual rank)
                    // PTS (fantasy points)
                // sort_type
                    // season
                    // date (baseball, basketball, and hockey only)
                    // week (football only)
                    // lastweek (baseball, basketball, and hockey only)
                    // lastmonth
                // sort_season
                    // year
                // sort_date (baseball only)
                    // YYYY-MM-DD
                // sort_week (football only)
                    // week number
                // start
                    // any integer 0 or greater
                // count
                    // // any integer greater than 0




            public EndPoint PlayersCollectionForPlayerName (string leagueKey, string playerName)
            {
                EndPointType = "players";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};search={playerName}"
                };
            }


            public EndPoint PlayersCollectionForPlayerNameAndPosition (string leagueKey, string playerName, string position)
            {
                EndPointType = "players";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};search={playerName};position={position}"
                };
            }


            public EndPoint PlayersCollectionForPosition (string leagueKey, string position)
            {
                EndPointType = "players";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};position={position}"
                };
            }


            public EndPoint PlayersCollectionForPositionWithSort (string leagueKey, string position, string sortType)
            {
                EndPointType = "players";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};position={position};sort={sortType}"
                };
            }


            public EndPoint PlayersCollectionForPositionAndStatusWithSort (string leagueKey, string position, string status, string sortType)
            {
                EndPointType = "players";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};position={position};sort={sortType};status={status}"
                };
            }


            public EndPoint PlayersCollectionForPositionAndStatusWithSortForLastWeek (string leagueKey, string position, string status)
            {
                EndPointType = "players";
                string sortType = "AR";
                string sortTypeType = "lastweek";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};position={position};status={status};sort={sortType};sort_type={sortTypeType}"
                };
            }


            public EndPoint PlayersCollectionForPositionAndStatusWithSortForLastMonth (string leagueKey, string position, string status)
            {
                EndPointType = "players";
                string sortType = "AR";
                string sortTypeType = "lastmonth";

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};position={position};status={status};sort={sortType};sort_type={sortTypeType}"
                };
            }


            public EndPoint PlayersCollectionForPositionAndStatusWithSortForToday (string leagueKey, string position, string status)
            {
                EndPointType = "players";
                string sortType = "AR";
                string sortTypeType = "date";
                string todaysDateString = DateTime.Now.ToString("YYYY-MM-DD");

                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};position={position};status={status};sort={sortType};sort_type={sortTypeType};sort_date={todaysDateString}"
                };
            }


            public EndPoint PlayersCollectionForPositionAndStatus (string leagueKey, string position, string status)
            {
                EndPointType = "players";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};position={position};status={status}"
                };
            }


            public EndPoint PlayersCollectionForPositionStatusAndCount (string leagueKey, string position, string status, string count)
            {
                EndPointType = "players";
                return new EndPoint
                {
                    BaseUri      = baseUri,
                    ResourceType = $"/league/{leagueKey}/{EndPointType};position={position};status={status};count={count}"
                };
            }



        #endregion PLAYERS COLLECTION END POINTS ------------------------------------------------------------









        // https://developer.yahoo.com/fantasysports/guide/player-resource.html
        #region PLAYER END POINTS ------------------------------------------------------------

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

        #endregion PLAYER END POINTS ------------------------------------------------------------





        // https://developer.yahoo.com/fantasysports/guide/transaction-resource.html
        #region TRANSACTION END POINTS ------------------------------------------------------------

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

        #endregion TRANSACTION END POINTS ------------------------------------------------------------





        // https://developer.yahoo.com/fantasysports/guide/user-resource.html
        #region USER END POINTS ------------------------------------------------------------

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

        #endregion USER END POINTS ------------------------------------------------------------
    }
}
