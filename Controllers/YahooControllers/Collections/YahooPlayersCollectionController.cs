using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Yahoo.Filters;
using BaseballScraper.Models.Yahoo.Collections.YahooPlayersCollection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers.Collections
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    // [ApiExplorerSettings(IgnoreApi = true)]
    public class YahooPlayersCollectionController : ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private static readonly YahooApiEndPoints _endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController _yahooApiRequestController;


        public YahooPlayersCollectionController(YahooApiRequestController yahooApiRequestController)
        {
            _yahooApiRequestController = yahooApiRequestController;
        }

        public YahooPlayersCollectionController(){}



        #region DOCUMENT DETAILS [ CLICK TO EXPAND ]
        // Yahoo Doc Link: https://developer.yahoo.com/fantasysports/guide/#players-collection
        // Players collection description (from Yahoo):
        //      * Enables you to get information from a collection of players simultaneously
        //      * To obtain general player info, the collection can be qualified in the URI by game, league or team
        //      * To obtain league or team related information, the collection is qualified by the relevant league or team
        //      * Each element beneath the Players Collection is a Player Resource
        // URIs
        //      * /players/{sub_resource}
        //      * /players;player_keys={player_key1},{player_key2}/{sub_resource}
        //      * /players;out={sub_resource_1},{sub_resource_2}
        //      * /players;player_keys={player_key1},{player_key2};out={sub_resource_1},{sub_resource_2}
        // Filters
        //      * position
        //          * /players;position=SP
        //      * status
        //          * /players;status=FA
        //      * search
        //          * /players;search=smith
        //      * sort
        //          * /players;sort=60
        //      * sort_type
        //          * /players;sort_type=season
        //      * sort_season
        //          * /players;sort_type=season;sort_season=2010
        //      * sort_date (Mlb, Nba, Nhl)
        //          * /players;sort_type=date;sort_date=2010-02-01
        //      * sort_week (Nfl)
        //          * /players;sort_type=week;sort_week=10
        //      * start
        //          * /players;start=25
        //      * count
        //          * /players;count=5

        // MODEL REFERENCE:
        //  * YahooPlayersCollection.cs

        // END POINTS REFERENCE:
        //  * PLAYERS COLLECTION END POINTS

        // JSON EXAMPLE:
        //      see very bottom of document for an example of Yahoo Players Collection json
        #endregion DOCUMENT DETAILS


        // https://127.0.0.1:5001/api/yahoo/yahooplayerscollection/test
        [HttpGet("test")]
        public void TestYahooPlayersCollectionController()
        {
            _h.StartMethod();
            // var player = FilterByPlayerNameThenCreateInstance("bryant");
            // var player = FilterByPlayerNameThenCreateInstance("smith");
            // var playerList = CreatePlayerListFromPlayersCollection(player);
        }



        #region YAHOO PLAYERS COLLECTION - PRIMARY METHODS - FILTER AND CREATE INSTANCE ------------------------------------------------------------

            #region FILTER OPTIONS [ CLICK TO EXPAND ]
                // FILTER OPTIONS:
                    // A) position
                        // Valid player positions (e.g., "3B", "SP")
                    // B) status
                        // 1) A (all available players)
                        // 2) FA (free agents only)
                        // 3) W (waivers only)
                        // 4) T (all taken players)
                        // 5) K (keepers only)
                    // C) search
                        // player name
                    // D) sort
                        // 1) {stat_id}
                            //  H/AB: "60"
                            //  Runs: "7"
                            //  HR: "12"
                            //  RBI: "13
                            //  SB: "16"
                            //  BB: "18"
                            //  AVG: "3"
                            //  IP: "50"
                            //  W: "28"
                            //  Ks: " 32"
                            //  Saves: "42"
                            //  Holds: "48"
                            //  ERA: "26"
                            //  WHIP: "27"
                        // 2) NAME (last, first)
                        // 3) OR (overall rank)
                        // 4) AR (actual rank)
                        // 5) PTS (fantasy points)
                    // E) sort_type
                        // 1) season
                        // 2) date (baseball, basketball, and hockey only)
                        // 3) week (football only)
                        // 4) lastweek (baseball, basketball, and hockey only)
                        // 5) lastmonth
                    // F) sort_season
                        // year
                    // G) sort_date (baseball only)
                        // YYYY-MM-DD
                    // H) sort_week (football only)
                        // week number
                    // I) start
                        // any integer 0 or greater
                    // J) count
                        // any integer greater than 0
            #endregion FILTER OPTIONS


            /* OVERALL SUMMARY OF SECTION */
            // STATUS [ June 11, 2019 ] : these all work
            /// <summary>
            ///     Query yahoo players collection with various filters
            ///     Each below is one (e.g., filter by player name) or a combo of filters (e.g., filter by player name, and position, and status)
            ///     See 'filter options' #region to view options for each of the filter types (e.g., status, )
            /// </summary>


            /// <example>
            ///    var collection = FilterByPlayerNameThenCreateInstance("smith"); OR
            ///    var player = FilterByPlayerNameThenCreateInstance("bryant");
            /// </example>
            [HttpGet("player")]
            public YahooPlayersCollection FilterByPlayerNameThenCreateInstance(string playerName)
            {
                string leagueKey            = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPlayerName(leagueKey, playerName).EndPointUri;
                YahooPlayersCollection yPc  = CreatePlayersCollectionInstance(uriPlayersCollection);
                return yPc;
            }


            /// <example>
            ///    var collection = FilterByPlayerNameAndPositionThenCreateInstance("smith","SP");
            /// </example>
            [HttpGet("player/{playerName}")]
            public YahooPlayersCollection FilterByPlayerNameAndPositionThenCreateInstance(string playerName, string position)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPlayerNameAndPosition(leagueKey, playerName, position).EndPointUri;
                YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
                // _h.Dig(yPc);
                return yPc;
            }


            /// <example>
            ///    var collection = FilterByPositionThenCreateInstance("3B");
            /// </example>
            [HttpGet("player_position")]
            public YahooPlayersCollection FilterByPositionThenCreateInstance(string position)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPosition(leagueKey, position).EndPointUri;
                YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
                return yPc;
            }


            /// <example>
            ///    var collection = FilterByPositionWithSortThenCreateInstance("3B","OR");
            /// </example>
            [HttpGet("player_position_sort")]
            public YahooPlayersCollection FilterByPositionWithSortThenCreateInstance(string position, string sortType)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPositionWithSort(leagueKey, position, sortType).EndPointUri;
                YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
                return yPc;
            }


            /// <example>
            ///     var collection = FilterByPositionAndStatusWithSortThenCreateInstance("3B","FA","OR");
            /// </example>
            [HttpGet("player_position_status_sort/{position}/{status}")]
            public YahooPlayersCollection FilterByPositionAndStatusWithSortThenCreateInstance(string position, string status, string sortType)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPositionAndStatusWithSort(leagueKey, position, status, sortType).EndPointUri;
                YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
                return yPc;
            }


            /// <example>
            ///     var collection = FilterByPositionAndStatusWithSortForLastWeekThenCreateInstance("3B","FA");
            /// </example>
            [HttpGet("player_position_status_sort_last_week/{position}/{status}")]
            public YahooPlayersCollection FilterByPositionAndStatusWithSortForLastWeekThenCreateInstance(string position, string status)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPositionAndStatusWithSortForLastWeek(leagueKey, position, status).EndPointUri;
                YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
                return yPc;
            }


            /// <example>
            ///    var collection = FilterByPositionAndStatusWithSortForLastMonthThenCreateInstance("3B","FA");
            /// </example>
            [HttpGet("player_position_status_sort_last_month/{position}/{status}")]
            public YahooPlayersCollection FilterByPositionAndStatusWithSortForLastMonthThenCreateInstance(string position, string status)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPositionAndStatusWithSortForLastMonth(leagueKey, position, status).EndPointUri;
                YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
                return yPc;
            }


            /// <example>
            ///    var collection = FilterByPositionAndStatusWithSortForTodayThenCreateInstance("3B","FA");
            /// </example>
            [HttpGet("player_position_status_sort_today/{position}/{status}")]
            public YahooPlayersCollection FilterByPositionAndStatusWithSortForTodayThenCreateInstance(string position, string status)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPositionAndStatusWithSortForToday(leagueKey, position, status).EndPointUri;
                // Console.WriteLine(uriPlayersCollection);
                YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
                return yPc;
            }


            /// <example>
            ///     var collection = FilterByPositionAndStatusThenCreateInstance("3B","FA");
            /// </example>
            [HttpGet("player_position_status/{position}/{status}")]
            public YahooPlayersCollection FilterByPositionAndStatusThenCreateInstance(string position, string status)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPositionAndStatus(leagueKey, position, status).EndPointUri;
                YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
                return yPc;
            }


            /// <example>
            ///    var collection = FilterByPositionStatusAndCountThenCreateInstance("3B","FA","10");
            /// </example>
            [HttpGet("player_position_status_count/{position}/{status}")]
            public YahooPlayersCollection FilterByPositionStatusAndCountThenCreateInstance(string position, string status, string count)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                string uriPlayersCollection = _endPoints.PlayersCollectionForPositionStatusAndCount(leagueKey, position, status, count).EndPointUri;
                YahooPlayersCollection yPc = CreatePlayersCollectionInstance(uriPlayersCollection);
                // _h.Dig(yPc);
                return yPc;
            }


            // STATUS [ June 18, 2019 ] : this works
            /// <example>
            ///    var collection = FilterByPositionWithSortThenCreateInstance("3B","OR");
            ///    var listOfPlayers = CreatePlayerListFromPlayersCollection(collection);
            /// </example>
            [HttpPost("players_list_collection")]
            public List<Player> CreatePlayerListFromPlayersCollection(YahooPlayersCollection collection)
            {
                var players = collection.Players;

                List<Player> listOfPlayers = new List<Player>();

                // if just one player is returned in the collection, players.Player will be a JObject
                if(players.Player is JObject)
                {
                    JObject playerJToken = (JObject)players.Player;
                    Player player = JsonConvert.DeserializeObject<Player>(playerJToken.ToString());
                    listOfPlayers.Add(player);
                }

                // if multiple players are returned in the collection, players.Player will be a JArray
                else
                {
                    JArray playersJArray = (JArray)players.Player;
                    int playersCount = playersJArray.Count;
                    for(var counter = 0; counter < playersCount; counter++)
                    {
                        var playerJToken = playersJArray[counter];
                        Player player = JsonConvert.DeserializeObject<Player>(playerJToken.ToString());
                        listOfPlayers.Add(player);
                    }
                }
                // PrintPlayerInfoFromList(listOfPlayers);
                return listOfPlayers;
            }


            // STATUS [ June 18, 2019 ] : this should work but haven't tested
            /// <example>
            ///     var collection = FilterByPositionWithSortThenCreateInstance("3B","OR");
            ///     var collectionPlayers = collection.Players;
            ///     var listPlayer = CreatePlayerListFromPlayers(collectionPlayers);
            /// </example>
            [HttpPost("players_list")]
            public List<Player> CreatePlayerListFromPlayers(Players players)
            {
                Player newPlayer = new Player();
                List<Player> listOfPlayers = new List<Player>();

                // if just one player is returned in the collection, players.Player will be a JObject
                if (players.Player is JObject playerJObject)
                {
                    Player player = JsonConvert.DeserializeObject<Player>(playerJObject.ToString());
                    listOfPlayers.Add(player);
                }

                // if multiple players are returned in the collection, players.Player will be a JArray
                else
                {
                    JArray playersJArray = (JArray)players.Player;
                    int playersCount = playersJArray.Count;
                    for(var counter = 0; counter < playersCount; counter++)
                    {
                        var playerJToken = playersJArray[counter];
                        Player player = JsonConvert.DeserializeObject<Player>(playerJToken.ToString());
                        listOfPlayers.Add(player);
                    }
                }
                return listOfPlayers;
            }


        #endregion  YAHOO PLAYERS COLLECTION - PRIMARY METHODS - FILTER AND CREATE INSTANCE ------------------------------------------------------------





        #region YAHOO PLAYERS COLLECTION - SUPPORT METHODS ------------------------------------------------------------


            // STATUS [ June 11, 2019 ] : this works
            /// <summary>
            ///     Used by the various methods about to generate the actual players collection based on the filters of those methods
            /// </summary>
            [HttpPost("players_collection")]
            private YahooPlayersCollection CreatePlayersCollectionInstance(string uriPlayersCollection)
            {
                JObject collectionJObject = _yahooApiRequestController.GenerateYahooResourceJObject(uriPlayersCollection);
                // _h.Dig(collectionJObject);

                JToken yahooPlayersCollectionToken = collectionJObject["fantasy_content"]["league"];

                string playersCollectionString = yahooPlayersCollectionToken.ToString();

                YahooPlayersCollection playersCollection = YahooPlayersCollection.FromJson(playersCollectionString);

                // PrintPlayerNameAndTeamName(playersCollectionString);
                return playersCollection;
            }


        #endregion YAHOO PLAYERS COLLECTION - SUPPORT METHODS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            [ApiExplorerSettings(IgnoreApi = true)]
            public void PrintPlayerNameAndTeamName(string PlayersCollectionString)
            {
                var playersCollection = YahooPlayersCollection.FromJson(PlayersCollectionString);
                // _h.Dig(playersCollection);

                List<Player> listOfPlayers = CreatePlayerListFromPlayersCollection(playersCollection);
                int count = listOfPlayers.Count;

                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"PLAYERS COLLECTION SEARCH || COUNT: {count}");
                Console.WriteLine("---------------------------------------------");

                foreach(var player in listOfPlayers)
                {
                    Console.WriteLine(player.Name.Full);
                    Console.WriteLine(player.EditorialTeamFullName);
                    Console.WriteLine();
                }
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine();
            }


            [ApiExplorerSettings(IgnoreApi = true)]
            public void PrintPlayerInfoFromList(List<Player> listOfPlayers)
            {
                Console.WriteLine();
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"PLAYERS IN LIST: {listOfPlayers.Count}");
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine();

                foreach(var player in listOfPlayers)
                {
                    Console.WriteLine($"{player.Name.Full}");
                }
            }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}






// Players Collection Json Example:

    // Method called:
        // var collection = FilterByPositionStatusAndCountThenCreateInstance("3B","FA","3");

    // Return Json:
        // {
        //   "YahooPlayersCollectionRecordId": 0,
        //   "league_key": "388.l.XXXX",
        //   "league_id": "XXXX",
        //   "name": "<league name here>>",
        //   "url": "https://baseball.fantasysports.yahoo.com/b1/XXXX",
        //   "logo_url": "https://ct.yimg.com/cy/1901/57769830372_729edf_192sq.jpg?ct=fantasy",
        //   "password": null,
        //   "draft_status": "postdraft",
        //   "num_teams": "10",
        //   "edit_key": "2019-06-12",
        //   "weekly_deadline": null,
        //   "league_update_timestamp": "1560236346",
        //   "scoring_type": "head",
        //   "league_type": "private",
        //   "renew": "378_XXXXX",
        //   "renewed": null,
        //   "iris_group_chat_id": <league group chat id>,
        //   "short_invitation_url": <invitation url>
        //   "allow_add_to_dl_extra_pos": "1",
        //   "is_pro_league": "0",
        //   "is_cash_league": "0",
        //   "current_week": "11",
        //   "start_week": "1",
        //   "start_date": "2019-03-20",
        //   "end_week": "25",
        //   "end_date": "2019-09-29",
        //   "game_code": "mlb",
        //   "season": "2019",
        //   "players": {
        //     "@count": "3",
        //     "player": [
        //       {
        //         "player_key": "388.p.7066",
        //         "player_id": "7066",
        //         "name": {
        //           "full": "José Reyes",
        //           "first": "José",
        //           "last": "Reyes",
        //           "ascii_first": "Jose",
        //           "ascii_last": "Reyes"
        //         },
        //         "status": 3,
        //         "status_full": 0,
        //         "editorial_player_key": "mlb.p.7066",
        //         "editorial_team_key": "mlb.t.21",
        //         "editorial_team_full_name": "New York Mets",
        //         "editorial_team_abbr": "NYM",
        //         "uniform_number": "7",
        //         "display_position": "2B,3B,SS",
        //         "headshot": {
        //           "url": "https://s.yimg.com/iu/api/res/1.2/_lkIJ.Kg1NP6S9bH2iMSdg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03272018/7066.png",
        //           "size": 0
        //         },
        //         "image_url": "https://s.yimg.com/iu/api/res/1.2/_lkIJ.Kg1NP6S9bH2iMSdg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03272018/7066.png",
        //         "is_undroppable": "0",
        //         "position_type": 0,
        //         "primary_position": 6,
        //         "eligible_positions": {
        //           "position": [
        //             6,
        //             7,
        //             4,
        //             2,
        //             8
        //           ]
        //         }
        //       },
        //       {
        //         "player_key": "388.p.7264",
        //         "player_id": "7264",
        //         "name": {
        //           "full": "José Bautista",
        //           "first": "José",
        //           "last": "Bautista",
        //           "ascii_first": "Jose",
        //           "ascii_last": "Bautista"
        //         },
        //         "status": 3,
        //         "status_full": 0,
        //         "editorial_player_key": "mlb.p.7264",
        //         "editorial_team_key": "mlb.t.22",
        //         "editorial_team_full_name": "Philadelphia Phillies",
        //         "editorial_team_abbr": "Phi",
        //         "uniform_number": "19",
        //         "display_position": "3B,OF",
        //         "headshot": {
        //           "url": "https://s.yimg.com/iu/api/res/1.2/2z19.W91P_pUszWKQL.J3Q--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/10162017/7264.1.png",
        //           "size": 0
        //         },
        //         "image_url": "https://s.yimg.com/iu/api/res/1.2/2z19.W91P_pUszWKQL.J3Q--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/10162017/7264.1.png",
        //         "is_undroppable": "0",
        //         "position_type": 0,
        //         "primary_position": 7,
        //         "eligible_positions": {
        //           "position": [
        //             7,
        //             2,
        //             3,
        //             8
        //           ]
        //         }
        //       },
        //       {
        //         "player_key": "388.p.7628",
        //         "player_id": "7628",
        //         "name": {
        //           "full": "Russell Martin",
        //           "first": "Russell",
        //           "last": "Martin",
        //           "ascii_first": "Russell",
        //           "ascii_last": "Martin"
        //         },
        //         "editorial_player_key": "mlb.p.7628",
        //         "editorial_team_key": "mlb.t.19",
        //         "editorial_team_full_name": "Los Angeles Dodgers",
        //         "editorial_team_abbr": "LAD",
        //         "uniform_number": "55",
        //         "display_position": "C,3B",
        //         "headshot": {
        //           "url": "https://s.yimg.com/iu/api/res/1.2/IRQhif4r0AzVqLN.ChtI2w--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04022019/7628.3.png",
        //           "size": 0
        //         },
        //         "image_url": "https://s.yimg.com/iu/api/res/1.2/IRQhif4r0AzVqLN.ChtI2w--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04022019/7628.3.png",
        //         "is_undroppable": "0",
        //         "position_type": 0,
        //         "primary_position": 0,
        //         "eligible_positions": {
        //           "position": [
        //             0,
        //             7,
        //             2,
        //             8
        //           ]
        //         },
        //         "has_player_notes": "1",
        //         "player_notes_last_timestamp": "1560049200"
        //       }
        //     ]
        //   }
        // }
