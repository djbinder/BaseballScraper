using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Yahoo.Resources;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006, MA0016, MA0051
namespace BaseballScraper.Controllers.YahooControllers.Resources
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class YahooRosterResourceController : ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private static YahooApiEndPoints _endPoints = new YahooApiEndPoints();
        private static YahooApiRequestController _yahooApiRequestController = new YahooApiRequestController();
        private readonly YahooAuthController _yahooAuthController = new YahooAuthController();
        private readonly PlayerBaseController _playerBaseController;
        private static readonly GoogleSheetsConnector _gSC = new GoogleSheetsConnector();


        public YahooRosterResourceController(YahooApiRequestController yahooApiRequestController, YahooAuthController yahooAuthController, PlayerBaseController playerBaseController)
        {
            _yahooApiRequestController = yahooApiRequestController;
            _yahooAuthController = yahooAuthController;
            _playerBaseController = playerBaseController;
        }

        public YahooRosterResourceController(){}


        #region DOCUMENT DETAILS [ CLICK TO EXPAND ]
        // Yahoo Doc Link: https://developer.yahoo.com/fantasysports/guide/#roster-resource
        // Roster resource description (from Yahoo):
        //      * Players on a team are organized into rosters corresponding to:
        //          * Certain weeks in the NFL
        //          * Crtain dates in MLB, NBA, and NHL
        //      * Each player is assigned a position if they’re in the starting lineup
        //      * You only get credit for stats from players in your starting lineup
        //      * You can use this API to edit your lineup by:
        //          * PUTting up new positions for the players on a roster
        //      * You can add/drop players from your roster by:
        //          *  `POSTing new transactions <#transactions-collection-POST>`__ to the league’s transactions collection.
        // URIs
        //      * https://fantasysports.yahooapis.com/fantasy/v2/team//roster
        //      * https://fantasysports.yahooapis.com/fantasy/v2/team//roster/
        //      * For NFL:
        //          * https://fantasysports.yahooapis.com/fantasy/v2/team//roster;week=10
        //      * For MLB, NBA, NHL
        //          * https://fantasysports.yahooapis.com/fantasy/v2/team//roster;date=2011-05-01
        //      * Example:
        //          * // https://fantasysports.yahooapis.com/fantasy/v2/team/253.l.102614.t.10/roster/players

        // MODEL REFERENCE:
        //  * Models/Yahoo/Resources/YahooRosterResource.cs

        // JSON EXAMPLE:
        //      see very bottom of document for an example of Yahoo Roster Resource json
        #endregion DOCUMENT DETAILS



        // https://127.0.0.1:5001/api/yahoo/yahoorosterresource/test
        [Route("test")]
        public void TestYahooRosterResourceController()
        {
            _h.StartMethod();
            var yahooRosterResourceInstance = CreateYahooRosterResourceInstance(1);
            _h.Dig(yahooRosterResourceInstance);
        }


        // https://127.0.0.1:5001/api/yahoo/yahoorosterresource/test/async
        [Route("test/async")]
        public async Task TestYahooRosterResourceControllerAsync()
        {
            _h.StartMethod();
            await AddPlayersForAllLeagueRostersToGoogleSheetAsync(10, "roster_import_test", "SheetsTestDoc");
        }




        #region YAHOO ROSTER RESOURCE - PRIMARY METHODS - GOOGLE SHEETS ------------------------------------------------------------


            // STATUS [ June 17, 2019 ] : this works
            /// <summary>
            ///     Add all players for one roster to a google sheet
            /// </summary>
            /// <param name="teamNumber">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Select the Id of the Manager you would want to view
            /// </param>
            /// <param name="tabName">
            ///     The name of the tab that you want to write the data to
            /// </param>
            /// <param name="gSheetsDocumentName">
            ///     This is from the 'gSheetNames.json' file in config data
            ///     It's the Value for Key = "DocumentName"
            /// </param>
            /// <example>
            ///     await AddPlayersForOneRosterToGoogleSheetAsync(1, "roster_import_test", "SheetsTestDoc")
            /// </example>
            private async Task AddPlayersForOneRosterToGoogleSheetAsync(int teamNumber, string tabName, string gSheetsDocumentName)
            {
                List<IList<object>> listOfAllPlayerLists = new List<IList<object>>();

                // read the sheet that you want to eventually write to
                // if there is currently no data (i.e., no rows populated) in the sheet, then add the header row
                var readSheet = _gSC.ReadDataFromSheetRange(gSheetsDocumentName, tabName,"A1:DB1000");
                if(readSheet == null)
                {
                    AddMostRelevantRosterPropertiesAsHeadersInGoogleSheet(listOfAllPlayerLists);
                }

                // get roster instance and team and manager details for roster
                var rosterInstance  = CreateYahooRosterResourceInstance(teamNumber);
                    var teamKey     = rosterInstance.TeamKey;
                    var teamId      = rosterInstance.TeamId;
                    var teamName    = rosterInstance.Name;
                    var managerId   = rosterInstance.Managers.Manager.ManagerId;
                    var managerName = rosterInstance.Managers.Manager.Nickname;

                // create list of all players on roster with all available data / columns / json
                // go through each player and select only the data you want to send to google sheet
                var allPlayers = CreateListOfPlayersOnRoster(teamNumber);
                foreach(var player in allPlayers)
                {
                    List<object> corePlayerInfo = new List<object>
                    {
                        teamKey,
                        teamId,
                        teamName,
                        managerId,
                        managerName,
                        player.PlayerId,
                        player.Name.Full,
                        player.Name.First,
                        player.Name.Last,
                        player.EditorialTeamFullName,
                        player.Status,
                        player.DisplayPosition,
                        player.PrimaryPosition,
                        player.SelectedPosition.Position,
                    };

                    // EligiblePositions.Positions can either be a string or array
                    // if it is a string, add the string to the list
                    if(player.EligiblePositions.RosterResourcePosition is string)
                    {
                        corePlayerInfo.Add(player.EligiblePositions.RosterResourcePosition);
                    }

                    // if EligiblePositions.Positions is an array, go through the array and get each value
                    // add that value to a string, then add the string to a list
                    else
                    {
                        JArray eligiblePositionJArray = (JArray)player.EligiblePositions.RosterResourcePosition;

                        var eligiblePositionCount = eligiblePositionJArray.Count;
                        string positionsString = "";

                        for(var i=0; i < eligiblePositionCount; i++)
                        {
                            if(i == 0)
                                positionsString = eligiblePositionJArray[i].ToString();

                            else
                                positionsString = $"{positionsString}, {eligiblePositionJArray[i].ToString()}";
                        }
                        corePlayerInfo.Add(positionsString);
                    }
                    listOfAllPlayerLists.Add(corePlayerInfo);
                }

                int? countOfExistingRowsWithData;
                string rangeToWriteDataTo;

                // if there is no data in the sheet yet, start at row
                // this adds the headers as well
                // if you do not check for null, you'll get an error
                if(readSheet == null)
                {
                    rangeToWriteDataTo = "A1:DB1000";
                    await _gSC.WriteGoogleSheetRowsAsync(listOfAllPlayerLists, tabName, rangeToWriteDataTo, gSheetsDocumentName);
                }

                else
                {
                    countOfExistingRowsWithData = readSheet.Count;
                    rangeToWriteDataTo = $"A{countOfExistingRowsWithData + 1}:DB1000";
                    await _gSC.WriteGoogleSheetRowsAsync(listOfAllPlayerLists, tabName, rangeToWriteDataTo, gSheetsDocumentName);
                }
            }


            // STATUS [ June 17, 2019 ] : this works
            /// <summary>
            ///     This is the same as the 'AddPlayersForOneRosterToGoogleSheetAsync()' method but for all teams
            /// </summary>
            private async Task AddPlayersForAllLeagueRostersToGoogleSheetAsync(int countOfTeams, string tabName, string gSheetsDocumentName)
            {
                for(var i = 1; i <= countOfTeams; i++)
                {
                    await AddPlayersForOneRosterToGoogleSheetAsync(i, tabName, gSheetsDocumentName);
                }
            }


        #endregion YAHOO ROSTER RESOURCE - PRIMARY METHODS - GOOGLE SHEETS ------------------------------------------------------------





        // 1.0 - YAHOO ROSTER RESOURCE
        // PATH:
        // ["fantasy_content"]["team"];
        #region YAHOO ROSTER RESOURCE - PRIMARY METHODS ------------------------------------------------------------


            // YAHOO ROSTER RESOURCE FULL
            // STATUS [ June 10, 2019 ] : this works
            /// <summary>
            ///     Instantiate new instance of yahoo roster resource
            /// </summary>
            /// <param name="teamNumber">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Select the Id of the Manager you would want to view
            /// </param>
            /// <example>
            ///     var yahooRosterResourceInstance = CreateYahooRosterResourceInstance(1);
            /// </example>
            public YahooRosterResource CreateYahooRosterResourceInstance(int teamNumber)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();

                var rosterResourceUri = _endPoints.RosterResourceEndPoint(leagueKey, teamNumber).EndPointUri;

                JObject rosterResourceJObject = _yahooApiRequestController.GenerateYahooResourceJObject(rosterResourceUri);

                JToken rosterResource = rosterResourceJObject["fantasy_content"]["team"];
                string rosterResourceString = rosterResource.ToString();

                var yahooRosterResource = new YahooRosterResource();
                yahooRosterResource = JsonConvert.DeserializeObject<YahooRosterResource>(rosterResourceString);

                return yahooRosterResource;
            }


            // YAHOO ROSTER RESOURCE FULL
            // STATUS [ June 13, 2019 ] : this works
            /// <summary>
            ///     Same as 'CreateYahooRosterResourceInstance()' but stops short of converting object to token then instance
            /// </summary>
            /// <param name="teamNumber">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Select the Id of the Manager you would want to view
            /// </param>
            /// <example>
            ///     var rosterResource = CreateRosterResourceJObject(6);
            /// </example>
            private JObject CreateRosterResourceJObject(int teamNumber)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                var rosterResourceUri = _endPoints.RosterResourceEndPoint(leagueKey, teamNumber).EndPointUri;

                JObject rosterResourceJObject = _yahooApiRequestController.GenerateYahooResourceJObject(rosterResourceUri);

                return rosterResourceJObject;
            }


            // YAHOO ROSTER RESOURCE FULL
            // STATUS [ June 13, 2019 ] : this works
            /// <summary>
            ///     Same as 'CreateYahooRosterResourceInstance()' but stops short of converting object to instance
            /// </summary>
            /// <param name="teamNumber">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Select the Id of the Manager you would want to view
            /// </param>
            /// <example>
            ///     var rosterResource = CreateYahooRosterResourceJToken(6);
            /// </example>
            private JToken CreateYahooRosterResourceJToken(int teamNumber)
            {
                string leagueKey = _yahooApiRequestController.GetTheGameIsTheGameLeagueKey();
                var rosterResourceUri = _endPoints.RosterResourceEndPoint(leagueKey, teamNumber).EndPointUri;
                JObject rosterResourceJObject = _yahooApiRequestController.GenerateYahooResourceJObject(rosterResourceUri);

                // children are:
                //      1) team_key 2) team_id 3) name 4) url 5) team_logos{}
                //      6) waiver_priority 7) number_of_moves 8) number_of_trades 9) roster_adds{}
                //      10) league_scoring_type 11) has_draft_grade 12) managers{} 13) roster{}
                JToken rosterResource = rosterResourceJObject["fantasy_content"]["team"];

                return rosterResource;
            }


            // YAHOO ROSTER RESOURCE FULL
            // STATUS [ June 13, 2019 ] : this works but not sure if needed
            private string CreateRosterResourceString(int teamNumber)
            {
                JToken rosterResource = CreateYahooRosterResourceJToken(teamNumber);
                string rosterResourceString = rosterResource.ToString();
                return rosterResourceString;
            }


            // YAHOO ROSTER RESOURCE FULL
            // STATUS [ June 13, 2019 ] : this works
            /// <summary>
            ///     Create list of YahooRosterResource instances for all leagues in team
            /// </summary>
            /// <param name="numberOfTeamsInLeague">
            ///     A number 0 - X; Where X is the total number of teams in the league;
            ///     Basically every manager has their own single number Id;
            ///     Select the Id of the Manager you would want to view
            /// </param>
            /// <example>
            ///     var roster = CreateListOfYahooRostersForAllTeams(10);
            /// </example>
            public List<YahooRosterResource> CreateListOfYahooRostersForAllTeams(int numberOfTeamsInLeague)
            {
                List<YahooRosterResource> allRosters = new List<YahooRosterResource>();
                for(var counter = 1; counter <= numberOfTeamsInLeague; counter++)
                {
                    var rosterResource = CreateYahooRosterResourceInstance(counter);
                    allRosters.Add(rosterResource);
                }
                return allRosters;
            }


        #endregion YAHOO ROSTER RESOURCE - PRIMARY METHODS ------------------------------------------------------------





        // 1.1 - ROSTER
        //      NOTE: this is NOT the same thing as Yahoo Roster Resource
        //              this is nested under Yahoo Roster Resource
        // MODEL PATH:
        //      YahooRosterResource > Roster
        // JSON PATH:
        //      ["fantasy_content"]["team"]["roster"];
        // Propertie(s): 1) coverage_type, 2) date 3) is_editable 4) players 5) outs_pitched
        #region YAHOO ROSTER RESOURCE | ROSTER - PRIMARY METHODS ------------------------------------------------------------

            // YAHOO ROSTER RESOURCE | ROSTER ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            private Roster DeserializeRoster(JToken rosterJToken)
            {
                Roster roster = JsonConvert.DeserializeObject<Roster>(rosterJToken.ToString());
                return roster;
            }


            // YAHOO ROSTER RESOURCE | ROSTER ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            public JToken CreateRosterJToken(int teamNumber)
            {
                var jObject = CreateRosterResourceJObject(teamNumber);
                var rosterJToken = jObject["roster"];
                return rosterJToken;
            }


        #endregion YAHOO ROSTER RESOURCE | ROSTER - PRIMARY METHODS ------------------------------------------------------------





        // 1.1.1 - PLAYERS
        // MODEL PATH:
        //      YahooRosterResource > Roster > Players
        // JSON PATH:
        //      ["fantasy_content"]["team"]["roster"]["players"];
        //
        // 1.1.1.1 - PLAYER
        // MODEL PATH:
        //      YahooRosterResource > Roster > Players > Player
        // JSON PATH:
        //      ["fantasy_content"]["team"]["roster"]["players"]["player"];
        #region YAHOO ROSTER RESOURCE | PLAYERS - PRIMARY METHODS ------------------------------------------------------------


            // YAHOO ROSTER RESOURCE | PLAYERS ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            private Player DeserializePlayerByIndex(JToken playerJToken, int playerIndex)
            {
                Player player = JsonConvert.DeserializeObject<Player>(playerJToken[playerIndex].ToString());
                return player;
            }


            // YAHOO ROSTER RESOURCE | PLAYERS ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            private Player DeserializePlayer(JToken playerJToken)
            {
                Player player = JsonConvert.DeserializeObject<Player>(playerJToken.ToString());
                return player;
            }


            // YAHOO ROSTER RESOURCE | PLAYERS ONLY
            // STATUS [ June 17, 2019 ] : this works
            public JToken CreatePlayersJToken(int teamNumber)
            {
                var jToken = CreateYahooRosterResourceJToken(teamNumber);
                var playersJToken = jToken["roster"]["players"];
                return playersJToken;
            }


            // YAHOO ROSTER RESOURCE | PLAYERS ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            public JToken CreatePlayerJToken(int teamNumber)
            {
                var jToken = CreateYahooRosterResourceJToken(teamNumber);
                var playerJToken = jToken["roster"]["players"]["player"];
                return playerJToken;
            }


            // YAHOO ROSTER RESOURCE | PLAYERS ONLY
            // STATUS [ June 13, 2019 ] : this works
            /// <summary>
            ///     Create list of Players on one manager's roster
            /// </summary>
            /// <example>
            ///     var rosterResourceToken = CreateRosterResourceJToken(6);
            ///     var listOfPlayers = CreateListOfPlayersOnRoster(rosterResourceToken);
            /// </example>
            public List<Player> CreateListOfPlayersOnRoster(int teamNumber)
            {
                JToken playersJToken = CreatePlayersJToken(teamNumber);
                var playersArray = playersJToken["player"];

                var newPlayer = new Player();
                var listOfPlayers = new List<Player>();

                foreach(var player in playersArray)
                {
                     newPlayer = JsonConvert.DeserializeObject<Player>(player.ToString());
                     listOfPlayers.Add(newPlayer);
                }
                // PrintPlayerInfoFromList(listOfPlayers);
                return listOfPlayers;
            }


            // YAHOO ROSTER RESOURCE | PLAYERS ONLY
            // STATUS [ June 13, 2019 ] : this works
            /// <summary>
            ///     Creates list of Players for all managers in league
            /// </summary>
            /// <example>
            ///     var roster  = CreateListOfPlayersAllRosters(10);
            /// </example>
            public List<List<Player>> CreateListOfPlayersAllRosters(int numberOfTeamsInLeague)
            {
                var listOfLists = new List<List<Player>>();

                for(var counter = 1; counter <= numberOfTeamsInLeague; counter++)
                {
                    var rosterResourceJObject = CreateRosterResourceJObject(counter);
                    // JToken rosterResourceToken = rosterResourceJObject["fantasy_content"]["team"];

                    var listOfPlayers = CreateListOfPlayersOnRoster(counter);
                    listOfLists.Add(listOfPlayers);
                }
                return listOfLists;
            }


        #endregion YAHOO ROSTER RESOURCE | PLAYERS - PRIMARY METHODS ------------------------------------------------------------





        // 1.1.1.1.1 - NAME
        // MODEL PATH:
        //      YahooRosterResource > Roster > Players > Player > Name
        // JSON PATH:
        //      ["fantasy_content"]["team"]["roster"]["players"]["player"][<PLAYER_INDEX>]["name"]
        // Propertie(s): 1) Full 2) First 3) Last 4) AsciiFirst 5) AsciiLast
        #region YAHOO ROSTER RESOURCE | PLAYER > NAME - PRIMARY METHODS ------------------------------------------------------------


            // YAHOO ROSTER RESOURCE | PLAYER NAME ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            private Name DeserializeName(JToken nameJToken)
            {
                Name name = JsonConvert.DeserializeObject<Name>(nameJToken.ToString());
                return name;
            }


            // YAHOO ROSTER RESOURCE | PLAYER NAME ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            public JToken CreateNameJToken(int teamNumber, int playerIndex)
            {
                var jObject = CreateRosterResourceJObject(teamNumber);
                var nameJToken = jObject["roster"]["players"]["player"][playerIndex]["name"];
                return nameJToken;
            }


            // YAHOO ROSTER RESOURCE | PLAYER NAME ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            public void AddNameValuesToList(Name name, [FromQuery]List<object> list)
            {
                foreach(PropertyInfo property in name.GetType().GetProperties())
                {
                    string propertyValue = property.GetValue(name, index: null).ToString();
                    list.Add(propertyValue);
                }
            }


        #endregion YAHOO ROSTER RESOURCE | PLAYER > NAME - PRIMARY METHODS ------------------------------------------------------------





        // 1.1.1.1.2 - TEAM LOGO
        // MODEL PATH:
        //      YahooRosterResource > Roster > Players > Player > Headshot
        // JSON PATH:
        //      ["fantasy_content"]["team"]["roster"]["players"]["player"][<PLAYER_INDEX>]["headshot"]
        // Propertie(s): 1) Url 2) Size
        #region YAHOO ROSTER RESOURCE | PLAYER > TEAM LOGO - PRIMARY METHODS ------------------------------------------------------------


            // YAHOO ROSTER RESOURCE | PLAYER TEAM LOGO ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            private TeamLogo DeserializeTeamLogo(JToken teamLogoJToken)
            {
                TeamLogo teamLogo = JsonConvert.DeserializeObject<TeamLogo>(teamLogoJToken.ToString());
                return teamLogo;
            }


            // YAHOO ROSTER RESOURCE | PLAYER TEAM LOGO ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            public JToken CreateTeamLogoJToken(int teamNumber, int playerIndex)
            {
                var jObject = CreateRosterResourceJObject(teamNumber);
                var teamLogoJToken = jObject["roster"]["players"]["player"][playerIndex]["headshot"];
                return teamLogoJToken;
            }


        #endregion YAHOO ROSTER RESOURCE | PLAYER > TEAM LOGO - PRIMARY METHODS ------------------------------------------------------------





        // 1.1.1.1.3 - POSITION TYPE
        // MODEL PATH:
        //      YahooRosterResource > Roster > Players > Player > PositionTYpe
        // JSON PATH:
        //      ["fantasy_content"]["team"]["roster"]["players"]["player"][<PLAYER_INDEX>]["position_type"]
        // public enum PositionType { B, P };
        #region YAHOO ROSTER RESOURCE | PLAYER > POSITION TYPE - PRIMARY METHODS ------------------------------------------------------------


            // YAHOO ROSTER RESOURCE | PLAYER POSITION TYPE ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            private PositionType DeserializePositionType(JToken positionTypeJToken)
            {
                PositionType positionType = JsonConvert.DeserializeObject<PositionType>(positionTypeJToken.ToString());
                return positionType;
            }


            // YAHOO ROSTER RESOURCE | PLAYER POSITION TYPE ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            public JToken CreatePositionTypeJToken(int teamNumber, int playerIndex)
            {
                var jObject = CreateRosterResourceJObject(teamNumber);
                var positionTypeJToken = jObject["roster"]["players"]["player"][playerIndex]["position_type"];
                return positionTypeJToken;
            }


        #endregion YAHOO ROSTER RESOURCE | PLAYER > POSITION TYPE - PRIMARY METHODS ------------------------------------------------------------





        // 1.1.1.1.4 - ELIGIBLE POSITIONS
        // MODEL PATH:
        //      YahooRosterResource > Roster > Players > Player > EligiblePositions
        // JSON PATH:
        //      ["fantasy_content"]["team"]["roster"]["players"]["player"][<PLAYER_INDEX>]["eligible_positions"]
        // Propertie(s): 1) Position (and PositionCasted[] as either string or string[])
        #region YAHOO ROSTER RESOURCE | PLAYER > ELIGIBLE POSITIONS - PRIMARY METHODS ------------------------------------------------------------


            // YAHOO ROSTER RESOURCE | PLAYER ELIGIBLE POSITIONS ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            private EligiblePositions DeserializeEligiblePositions(JToken eligiblePositionsJToken)
            {
                EligiblePositions eligiblePositions = JsonConvert.DeserializeObject<EligiblePositions>(eligiblePositionsJToken.ToString());
                return eligiblePositions;
            }


            // YAHOO ROSTER RESOURCE | PLAYER ELIGIBLE POSITIONS ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            public JToken CreateEligiblePositionsJToken(int teamNumber, int playerIndex)
            {
                var jObject = CreateRosterResourceJObject(teamNumber);
                var eligiblePositionsJToken = jObject["roster"]["players"]["player"][playerIndex]["eligible_positions"];
                return eligiblePositionsJToken;
            }


        #endregion YAHOO ROSTER RESOURCE | PLAYER > ELIGIBLE POSITIONS - PRIMARY METHODS ------------------------------------------------------------





        // 1.1.1.1.5 - SELECTED POSITION
        // MODEL PATH:
        //      YahooRosterResource > Roster > Players > Player > SelectedPositions
        // JSON PATH:
        //      ["fantasy_content"]["team"]["roster"]["players"]["player"][<PLAYER_INDEX>]["selected_position"]
        // Propertie(s): 1) CoverageType 2) Date 3) Position
        #region YAHOO ROSTER RESOURCE | PLAYER > SELECTED POSITION - PRIMARY METHODS ------------------------------------------------------------


            // YAHOO ROSTER RESOURCE | PLAYER SELECTED POSITION ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            private SelectedPosition DeserializeSelectedPosition(JToken selectedPositionJToken)
            {
                SelectedPosition selectedPosition = JsonConvert.DeserializeObject<SelectedPosition>(selectedPositionJToken.ToString());
                return selectedPosition;
            }


            // YAHOO ROSTER RESOURCE | PLAYER SELECTED POSITION ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            public JToken CreateSelectedPositionJToken(int teamNumber, int playerIndex)
            {
                var jObject = CreateRosterResourceJObject(teamNumber);
                var selectedPositionJToken = jObject["roster"]["players"]["player"][playerIndex]["selected_position"];
                return selectedPositionJToken;
            }


        #endregion YAHOO ROSTER RESOURCE | PLAYER > SELECTED POSITION - PRIMARY METHODS ------------------------------------------------------------





        #region YAHOO ROSTER RESOURCE | MANAGERS - PRIMARY METHODS ------------------------------------------------------------


            // YAHOO ROSTER RESOURCE | MANAGERS ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            private Manager DeserializeManager(JToken managerJToken)
            {
                Manager manager = JsonConvert.DeserializeObject<Manager>(managerJToken.ToString());
                return manager;
            }


            // YAHOO ROSTER RESOURCE | MANAGERS ONLY
            // STATUS [ June 17, 2019 ] : should work but haven't tested
            public JToken CreateManagerJToken(int teamNumber)
            {
                var jObject = CreateRosterResourceJObject(teamNumber);
                var managerJToken = jObject["managers"]["manager"];
                return managerJToken;
            }


            // YAHOO ROSTER RESOURCE | MANAGERS ONLY
            // STATUS [ June 13, 2019 ] : this works
            /// <summary>
            ///     Create instance of manager for one team in league
            /// </summary>
            /// <remarks>
            ///     This might break if there are co-managers; haven't tested that yet though
            /// </remarks>
            /// <example>
            ///     var managers = GetManagersForRoster(8);
            /// </example>
            public Manager GetMangersForRoster(int teamNumber)
            {
                var rosterResourceJObject = CreateRosterResourceJObject(teamNumber);
                JToken managersToken = rosterResourceJObject["fantasy_content"]["team"]["managers"]["manager"];
                string managersTokenString = managersToken.ToString();

                Manager manager = new Manager();
                    manager = JsonConvert.DeserializeObject<Manager>(managersTokenString);

                return manager;
            }


            // YAHOO ROSTER RESOURCE | MANAGERS ONLY
            // STATUS [ June 13, 2019 ] : this works
            /// <summary>
            ///     Create instance of manager all teams in league
            /// </summary>
            /// <remarks>
            ///     This might break if there are co-managers; haven't tested that yet though
            /// </remarks>
            /// <example>
            ///     var managersList = CreateListOfManagersForAllRosters(10);
            /// </example>
            public List<Manager> CreateListOfManagersForAllRosters(int numberOfTeamsInLeague)
            {
                var managerList = new List<Manager>();
                Manager manager = new Manager();

                for(var counter = 1; counter <= numberOfTeamsInLeague; counter++)
                {
                    manager = GetMangersForRoster(counter);
                    managerList.Add(manager);
                }
                return managerList;
            }


        #endregion YAHOO ROSTER RESOURCE | MANAGERS - PRIMARY METHODS ------------------------------------------------------------





        #region YAHOO ROSTER RESOURCE - SUPPORT METHODS | CONNECT TO GOOGLE SHEETS ------------------------------------------------------------


            // YAHOO ROSTER RESOURCE | GOOGLE SHEETS HEADERS
            // STATUS [ June 13, 2019 ] : this works
            /// <summary>
            ///     Create a list of headers of all available data in Yahoo Roster Resource json
            ///     This differs from next method as that method is a refined version of the headers you want
            /// </summary>
            /// <example>
            ///     AddAllPlayerModelPropertiesAsHeadersInGoogleSheet(listOfLists);
            /// </example>
            public void AddAllPlayerModelPropertiesAsHeadersInGoogleSheet(List<IList<object>> listOfLists)
            {
                List<object> headers = new List<object>();
                Player blankPlayer = new Player();
                PropertyInfo[] playerProperties = blankPlayer.GetType().GetProperties();
                foreach(var pProp in playerProperties)
                {
                    // Console.WriteLine(pProp.Name);
                    // Console.WriteLine(pProp.PropertyType);
                    // Console.WriteLine();
                    headers.Add(pProp.Name);
                }
                listOfLists.Add(headers);
            }


            // YAHOO ROSTER RESOURCE | GOOGLE SHEETS HEADERS
            // STATUS [ June 1y, 2019 ] : this works
            /// <summary>
            ///     Create a list of selected headers
            ///     This differs from the previous method as previous method gets you all headers
            /// </summary>
            /// <example>
            ///     AddMostRelevantRosterPropertiesAsHeadersInGoogleSheet(listOfLists);
            /// </example>
            public void AddMostRelevantRosterPropertiesAsHeadersInGoogleSheet(List<IList<object>> listOfLists)
            {
                List<object> headers = new List<object>()
                {
                    "Team Key",
                    "Team Id",
                    "Team",
                    "Manager Id",
                    "Manager",
                    "Player Id",
                    "Full Name",
                    "First Name",
                    "Last Name",
                    "MLB Team",
                    "Status",
                    "Display Position",
                    "Primary Position",
                    "Selected Position",
                    "Eligible Positions",
                };

                listOfLists.Add(headers);
            }


        #endregion YAHOO ROSTER RESOURCE - SUPPORT METHODS | CONNECT TO GOOGLE SHEETS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintPlayerInfoFromList(List<Models.Yahoo.Collections.Player> listOfPlayers)
            {
                Console.WriteLine();
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine($"PLAYERS IN LIST: {listOfPlayers.Count}");
                Console.WriteLine("----------------------------------------------------------");

                foreach(var player in listOfPlayers)
                {
                    Console.WriteLine($"{player.Name.Full}\n");
                }
                Console.WriteLine("----------------------------------------------------------");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------

    }
}



// Roster Resource Json Example:

    // Method called:
        // var yahooRosterResourceInstance = CreateYahooRosterResourceInstance(1);

    // Return Json:
        // {
        //   "YahooTeamRosterRecordId": 0,
        //   "team_key": "388.l.XXXX.t.1",
        //   "team_id": 1,
        //   "name": "<team name>",
        //   "url": "https://baseball.fantasysports.yahoo.com/b1/XXXX/1",
        //   "team_logos": {
        //     "team_logo": {
        //       "url": "https://ct.yimg.com/cy/1916/58724909692_da39ab_192sq.jpg?ct=fantasy",
        //       "size": 0
        //     }
        //   },
        //   "waiver_priority": 1,
        //   "number_of_moves": 20,
        //   "number_of_trades": 0,
        //   "roster_adds": {
        //     "coverage_type": "week",
        //     "coverage_value": 11,
        //     "value": 0
        //   },
        //   "league_scoring_type": "head",
        //   "has_draft_grade": 0,
        //   "managers": {
        //     "manager": {
        //       "manager_id": 1,
        //       "nickname": <name>,
        //       "guid": "XXXXXXXXXXXXXXXXXXXXXXXX",
        //       "image_url": "https://ct.yimg.com/cy/4527/XXXXXXXXXXXX.jpg"
        //     }
        //   },
        //   "roster": {
        //     "coverage_type": 0,
        //     "date": "2019-06-12",
        //     "is_editable": 1,
        //     "players": {
        //       "@count": 26,
        //       "player": [
        //         {
        //           "player_key": "388.p.8653",
        //           "player_id": 8653,
        //           "name": {
        //             "full": "Justin Smoak",
        //             "first": "Justin",
        //             "last": "Smoak",
        //             "ascii_first": "Justin",
        //             "ascii_last": "Smoak"
        //           },
        //           "editorial_player_key": "mlb.p.8653",
        //           "editorial_team_key": "mlb.t.14",
        //           "editorial_team_full_name": "Toronto Blue Jays",
        //           "editorial_team_abbr": "Tor",
        //           "uniform_number": 14,
        //           "display_position": "1B",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/hrboZb37X4fYRQQGbHYXpQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04302019/8653.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/hrboZb37X4fYRQQGbHYXpQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04302019/8653.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "1B",
        //           "eligible_positions": {
        //             "position": [
        //               "1B",
        //               "IF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1559514000,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "1B"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.9557",
        //           "player_id": 9557,
        //           "name": {
        //             "full": "Javier Báez",
        //             "first": "Javier",
        //             "last": "Báez",
        //             "ascii_first": "Javier",
        //             "ascii_last": "Baez"
        //           },
        //           "editorial_player_key": "mlb.p.9557",
        //           "editorial_team_key": "mlb.t.16",
        //           "editorial_team_full_name": "Chicago Cubs",
        //           "editorial_team_abbr": "ChC",
        //           "uniform_number": 9,
        //           "display_position": "2B,3B,SS",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/Ql6Xeg4v4eMEm3G9BPJ8NQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04042019/9557.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/Ql6Xeg4v4eMEm3G9BPJ8NQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04042019/9557.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "2B",
        //           "eligible_positions": {
        //             "position": [
        //               "2B",
        //               "3B",
        //               "SS",
        //               "IF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560048480,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "2B"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.8723",
        //           "player_id": 8723,
        //           "name": {
        //             "full": "Josh Donaldson",
        //             "first": "Josh",
        //             "last": "Donaldson",
        //             "ascii_first": "Josh",
        //             "ascii_last": "Donaldson"
        //           },
        //           "editorial_player_key": "mlb.p.8723",
        //           "editorial_team_key": "mlb.t.15",
        //           "editorial_team_full_name": "Atlanta Braves",
        //           "editorial_team_abbr": "Atl",
        //           "uniform_number": 20,
        //           "display_position": "3B",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/g.QNebV5U1OkFazi2pgeqg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/8723.1.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/g.QNebV5U1OkFazi2pgeqg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/8723.1.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "3B",
        //           "eligible_positions": {
        //             "position": [
        //               "3B",
        //               "IF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560319380,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "3B"
        //           },
        //           "has_recent_player_notes": 1
        //         },
        //         {
        //           "player_key": "388.p.9581",
        //           "player_id": 9581,
        //           "name": {
        //             "full": "Adalberto Mondesi",
        //             "first": "Adalberto",
        //             "last": "Mondesi",
        //             "ascii_first": "Adalberto",
        //             "ascii_last": "Mondesi"
        //           },
        //           "editorial_player_key": "mlb.p.9581",
        //           "editorial_team_key": "mlb.t.7",
        //           "editorial_team_full_name": "Kansas City Royals",
        //           "editorial_team_abbr": "KC",
        //           "uniform_number": 27,
        //           "display_position": "2B,SS",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/F47nb1_qqP6XJzeWyaTmaQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/05012019/9581.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/F47nb1_qqP6XJzeWyaTmaQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/05012019/9581.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "2B",
        //           "eligible_positions": {
        //             "position": [
        //               "2B",
        //               "SS",
        //               "IF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560309900,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "SS"
        //           },
        //           "has_recent_player_notes": 1
        //         },
        //         {
        //           "player_key": "388.p.9750",
        //           "player_id": 9750,
        //           "name": {
        //             "full": "Jorge Polanco",
        //             "first": "Jorge",
        //             "last": "Polanco",
        //             "ascii_first": "Jorge",
        //             "ascii_last": "Polanco"
        //           },
        //           "editorial_player_key": "mlb.p.9750",
        //           "editorial_team_key": "mlb.t.9",
        //           "editorial_team_full_name": "Minnesota Twins",
        //           "editorial_team_abbr": "Min",
        //           "uniform_number": 11,
        //           "display_position": "SS",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/.0fiLNjNGpBDssppmXdHsQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04172019/9750.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/.0fiLNjNGpBDssppmXdHsQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04172019/9750.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "SS",
        //           "eligible_positions": {
        //             "position": [
        //               "SS",
        //               "IF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560316620,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "IF"
        //           },
        //           "has_recent_player_notes": 1
        //         },
        //         {
        //           "player_key": "388.p.10646",
        //           "player_id": 10646,
        //           "name": {
        //             "full": "Ronald Acuña Jr.",
        //             "first": "Ronald",
        //             "last": "Acuña Jr.",
        //             "ascii_first": "Ronald",
        //             "ascii_last": "Acuna Jr."
        //           },
        //           "editorial_player_key": "mlb.p.10646",
        //           "editorial_team_key": "mlb.t.15",
        //           "editorial_team_full_name": "Atlanta Braves",
        //           "editorial_team_abbr": "Atl",
        //           "uniform_number": 13,
        //           "display_position": "OF",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/lPy4WRjYLqCeJiasXLbAQw--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/10646.1.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/lPy4WRjYLqCeJiasXLbAQw--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/10646.1.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "OF",
        //           "eligible_positions": {
        //             "position": [
        //               "OF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560319680,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "OF"
        //           },
        //           "has_recent_player_notes": 1
        //         },
        //         {
        //           "player_key": "388.p.9552",
        //           "player_id": 9552,
        //           "name": {
        //             "full": "Mookie Betts",
        //             "first": "Mookie",
        //             "last": "Betts",
        //             "ascii_first": "Mookie",
        //             "ascii_last": "Betts"
        //           },
        //           "editorial_player_key": "mlb.p.9552",
        //           "editorial_team_key": "mlb.t.2",
        //           "editorial_team_full_name": "Boston Red Sox",
        //           "editorial_team_abbr": "Bos",
        //           "uniform_number": 50,
        //           "display_position": "OF",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/esECyEuPHb0m4d4gJv4acA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04032019/9552.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/esECyEuPHb0m4d4gJv4acA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04032019/9552.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "OF",
        //           "eligible_positions": {
        //             "position": [
        //               "OF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560044280,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "OF"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.9875",
        //           "player_id": 9875,
        //           "name": {
        //             "full": "Michael Conforto",
        //             "first": "Michael",
        //             "last": "Conforto",
        //             "ascii_first": "Michael",
        //             "ascii_last": "Conforto"
        //           },
        //           "editorial_player_key": "mlb.p.9875",
        //           "editorial_team_key": "mlb.t.21",
        //           "editorial_team_full_name": "New York Mets",
        //           "editorial_team_abbr": "NYM",
        //           "uniform_number": 30,
        //           "display_position": "OF",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/qtpBbFMxlHMO7jfa4lJeHQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/9875.1.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/qtpBbFMxlHMO7jfa4lJeHQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/9875.1.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "OF",
        //           "eligible_positions": {
        //             "position": [
        //               "OF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1559964600,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "OF"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.9558",
        //           "player_id": 9558,
        //           "name": {
        //             "full": "Kris Bryant",
        //             "first": "Kris",
        //             "last": "Bryant",
        //             "ascii_first": "Kris",
        //             "ascii_last": "Bryant"
        //           },
        //           "editorial_player_key": "mlb.p.9558",
        //           "editorial_team_key": "mlb.t.16",
        //           "editorial_team_full_name": "Chicago Cubs",
        //           "editorial_team_abbr": "ChC",
        //           "uniform_number": 17,
        //           "display_position": "3B,OF",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/MRXFDxRcqZw1xBwG_8Dicw--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04042019/9558.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/MRXFDxRcqZw1xBwG_8Dicw--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04042019/9558.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "3B",
        //           "eligible_positions": {
        //             "position": [
        //               "3B",
        //               "IF",
        //               "OF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560355980,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "Util"
        //           },
        //           "has_recent_player_notes": 1
        //         },
        //         {
        //           "player_key": "388.p.9106",
        //           "player_id": 9106,
        //           "name": {
        //             "full": "Anthony Rendon",
        //             "first": "Anthony",
        //             "last": "Rendon",
        //             "ascii_first": "Anthony",
        //             "ascii_last": "Rendon"
        //           },
        //           "editorial_player_key": "mlb.p.9106",
        //           "editorial_team_key": "mlb.t.20",
        //           "editorial_team_full_name": "Washington Nationals",
        //           "editorial_team_abbr": "Was",
        //           "uniform_number": 6,
        //           "display_position": "3B",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/yThaXj41_fN0lxRdfZOo7w--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04092019/9106.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/yThaXj41_fN0lxRdfZOo7w--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04092019/9106.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "3B",
        //           "eligible_positions": {
        //             "position": [
        //               "3B",
        //               "IF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560318420,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "BN"
        //           },
        //           "has_recent_player_notes": 1
        //         },
        //         {
        //           "player_key": "388.p.9282",
        //           "player_id": 9282,
        //           "name": {
        //             "full": "Didi Gregorius",
        //             "first": "Didi",
        //             "last": "Gregorius",
        //             "ascii_first": "Didi",
        //             "ascii_last": "Gregorius"
        //           },
        //           "editorial_player_key": "mlb.p.9282",
        //           "editorial_team_key": "mlb.t.10",
        //           "editorial_team_full_name": "New York Yankees",
        //           "editorial_team_abbr": "NYY",
        //           "uniform_number": 18,
        //           "display_position": "SS",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/uNTWOc40ko_8GhjDUnvqUw--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04032019/9282.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/uNTWOc40ko_8GhjDUnvqUw--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04032019/9282.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "SS",
        //           "eligible_positions": {
        //             "position": [
        //               "SS",
        //               "IF",
        //               "Util"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560261900,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "BN"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.10575",
        //           "player_id": 10575,
        //           "name": {
        //             "full": "Chris Paddack",
        //             "first": "Chris",
        //             "last": "Paddack",
        //             "ascii_first": "Chris",
        //             "ascii_last": "Paddack"
        //           },
        //           "editorial_player_key": "mlb.p.10575",
        //           "editorial_team_key": "mlb.t.25",
        //           "editorial_team_full_name": "San Diego Padres",
        //           "editorial_team_abbr": "SD",
        //           "uniform_number": 59,
        //           "display_position": "SP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/6yIfpNqYpnTQ3ApHYqiwrg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/10575.1.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/6yIfpNqYpnTQ3ApHYqiwrg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/10575.1.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "SP",
        //           "eligible_positions": {
        //             "position": [
        //               "SP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560315180,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "SP"
        //           },
        //           "has_recent_player_notes": 1
        //         },
        //         {
        //           "player_key": "388.p.10514",
        //           "player_id": 10514,
        //           "name": {
        //             "full": "Luis Castillo",
        //             "first": "Luis",
        //             "last": "Castillo",
        //             "ascii_first": "Luis",
        //             "ascii_last": "Castillo"
        //           },
        //           "editorial_player_key": "mlb.p.10514",
        //           "editorial_team_key": "mlb.t.17",
        //           "editorial_team_full_name": "Cincinnati Reds",
        //           "editorial_team_abbr": "Cin",
        //           "uniform_number": 58,
        //           "display_position": "SP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/JCV.sHhgP_S212cQ5IdfEA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04122019/10514.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/JCV.sHhgP_S212cQ5IdfEA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04122019/10514.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "SP",
        //           "eligible_positions": {
        //             "position": [
        //               "SP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560310800,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "SP"
        //           },
        //           "has_recent_player_notes": 1
        //         },
        //         {
        //           "player_key": "388.p.10214",
        //           "player_id": 10214,
        //           "name": {
        //             "full": "Edwin Díaz",
        //             "first": "Edwin",
        //             "last": "Díaz",
        //             "ascii_first": "Edwin",
        //             "ascii_last": "Diaz"
        //           },
        //           "editorial_player_key": "mlb.p.10214",
        //           "editorial_team_key": "mlb.t.21",
        //           "editorial_team_full_name": "New York Mets",
        //           "editorial_team_abbr": "NYM",
        //           "uniform_number": 39,
        //           "display_position": "RP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/E1zNpEVHAkL8PSHYAZDH.w--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/10214.1.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/E1zNpEVHAkL8PSHYAZDH.w--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/10214.1.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "RP",
        //           "eligible_positions": {
        //             "position": [
        //               "RP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560046860,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "RP"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.9333",
        //           "player_id": 9333,
        //           "name": {
        //             "full": "Matt Barnes",
        //             "first": "Matt",
        //             "last": "Barnes",
        //             "ascii_first": "Matt",
        //             "ascii_last": "Barnes"
        //           },
        //           "editorial_player_key": "mlb.p.9333",
        //           "editorial_team_key": "mlb.t.2",
        //           "editorial_team_full_name": "Boston Red Sox",
        //           "editorial_team_abbr": "Bos",
        //           "uniform_number": 32,
        //           "display_position": "RP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/LlJOnHexB3kibhGOgxftzA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04032019/9333.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/LlJOnHexB3kibhGOgxftzA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04032019/9333.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "RP",
        //           "eligible_positions": {
        //             "position": [
        //               "RP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1559856000,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "RP"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.10691",
        //           "player_id": 10691,
        //           "name": {
        //             "full": "Emilio Pagán",
        //             "first": "Emilio",
        //             "last": "Pagán",
        //             "ascii_first": "Emilio",
        //             "ascii_last": "Pagan"
        //           },
        //           "editorial_player_key": "mlb.p.10691",
        //           "editorial_team_key": "mlb.t.30",
        //           "editorial_team_full_name": "Tampa Bay Rays",
        //           "editorial_team_abbr": "TB",
        //           "uniform_number": 15,
        //           "display_position": "RP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/g_Hbo.MfPgiA5KM4cWLBkQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04252019/10691.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/g_Hbo.MfPgiA5KM4cWLBkQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04252019/10691.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "RP",
        //           "eligible_positions": {
        //             "position": [
        //               "RP",
        //               "P"
        //             ]
        //           },
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "RP"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.10133",
        //           "player_id": 10133,
        //           "name": {
        //             "full": "Taylor Rogers",
        //             "first": "Taylor",
        //             "last": "Rogers",
        //             "ascii_first": "Taylor",
        //             "ascii_last": "Rogers"
        //           },
        //           "editorial_player_key": "mlb.p.10133",
        //           "editorial_team_key": "mlb.t.9",
        //           "editorial_team_full_name": "Minnesota Twins",
        //           "editorial_team_abbr": "Min",
        //           "uniform_number": 55,
        //           "display_position": "RP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/MZXpklBwGZq67eNz_CNGPQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04172019/10133.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/MZXpklBwGZq67eNz_CNGPQ--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04172019/10133.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "RP",
        //           "eligible_positions": {
        //             "position": [
        //               "RP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560311640,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "RP"
        //           },
        //           "has_recent_player_notes": 1,
        //           "status": "DTD",
        //           "status_full": "Day-to-Day"
        //         },
        //         {
        //           "player_key": "388.p.9632",
        //           "player_id": 9632,
        //           "name": {
        //             "full": "Luke Jackson",
        //             "first": "Luke",
        //             "last": "Jackson",
        //             "ascii_first": "Luke",
        //             "ascii_last": "Jackson"
        //           },
        //           "editorial_player_key": "mlb.p.9632",
        //           "editorial_team_key": "mlb.t.15",
        //           "editorial_team_full_name": "Atlanta Braves",
        //           "editorial_team_abbr": "Atl",
        //           "uniform_number": 77,
        //           "display_position": "RP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/StXUQ3EZwAk.JUlafV4wow--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04082019/9632.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/StXUQ3EZwAk.JUlafV4wow--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04082019/9632.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "RP",
        //           "eligible_positions": {
        //             "position": [
        //               "RP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560036240,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "P"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.10711",
        //           "player_id": 10711,
        //           "name": {
        //             "full": "John Brebbia",
        //             "first": "John",
        //             "last": "Brebbia",
        //             "ascii_first": "John",
        //             "ascii_last": "Brebbia"
        //           },
        //           "editorial_player_key": "mlb.p.10711",
        //           "editorial_team_key": "mlb.t.24",
        //           "editorial_team_full_name": "St. Louis Cardinals",
        //           "editorial_team_abbr": "StL",
        //           "uniform_number": 60,
        //           "display_position": "RP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/ctxs0hojeMmGzroGEqf_0w--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04082019/10711.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/ctxs0hojeMmGzroGEqf_0w--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04082019/10711.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "RP",
        //           "eligible_positions": {
        //             "position": [
        //               "RP",
        //               "P"
        //             ]
        //           },
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "P"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.9546",
        //           "player_id": 9546,
        //           "name": {
        //             "full": "Eduardo Rodriguez",
        //             "first": "Eduardo",
        //             "last": "Rodriguez",
        //             "ascii_first": "Eduardo",
        //             "ascii_last": "Rodriguez"
        //           },
        //           "editorial_player_key": "mlb.p.9546",
        //           "editorial_team_key": "mlb.t.2",
        //           "editorial_team_full_name": "Boston Red Sox",
        //           "editorial_team_abbr": "Bos",
        //           "uniform_number": 57,
        //           "display_position": "SP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/OW6OyhXEV5urHCmvlk0Z8A--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04032019/9546.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/OW6OyhXEV5urHCmvlk0Z8A--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04032019/9546.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "SP",
        //           "eligible_positions": {
        //             "position": [
        //               "SP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560112860,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "BN"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.9597",
        //           "player_id": 9597,
        //           "name": {
        //             "full": "Noah Syndergaard",
        //             "first": "Noah",
        //             "last": "Syndergaard",
        //             "ascii_first": "Noah",
        //             "ascii_last": "Syndergaard"
        //           },
        //           "editorial_player_key": "mlb.p.9597",
        //           "editorial_team_key": "mlb.t.21",
        //           "editorial_team_full_name": "New York Mets",
        //           "editorial_team_abbr": "NYM",
        //           "uniform_number": 34,
        //           "display_position": "SP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/g7k8C0y7O6DdiDSmUIjvhg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/9597.1.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/g7k8C0y7O6DdiDSmUIjvhg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/03222019/9597.1.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "SP",
        //           "eligible_positions": {
        //             "position": [
        //               "SP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560287400,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "BN"
        //           },
        //           "has_recent_player_notes": 1,
        //           "status": "DTD",
        //           "status_full": "Day-to-Day"
        //         },
        //         {
        //           "player_key": "388.p.10509",
        //           "player_id": 10509,
        //           "name": {
        //             "full": "Walker Buehler",
        //             "first": "Walker",
        //             "last": "Buehler",
        //             "ascii_first": "Walker",
        //             "ascii_last": "Buehler"
        //           },
        //           "editorial_player_key": "mlb.p.10509",
        //           "editorial_team_key": "mlb.t.19",
        //           "editorial_team_full_name": "Los Angeles Dodgers",
        //           "editorial_team_abbr": "LAD",
        //           "uniform_number": 21,
        //           "display_position": "SP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/EgIgtIJi4sk4qrvdp_W6Uw--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04022019/10509.3.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/EgIgtIJi4sk4qrvdp_W6Uw--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04022019/10509.3.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "SP",
        //           "eligible_positions": {
        //             "position": [
        //               "SP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560122640,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "BN"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.9586",
        //           "player_id": 9586,
        //           "name": {
        //             "full": "Andrew Heaney",
        //             "first": "Andrew",
        //             "last": "Heaney",
        //             "ascii_first": "Andrew",
        //             "ascii_last": "Heaney"
        //           },
        //           "editorial_player_key": "mlb.p.9586",
        //           "editorial_team_key": "mlb.t.3",
        //           "editorial_team_full_name": "Los Angeles Angels",
        //           "editorial_team_abbr": "LAA",
        //           "uniform_number": 28,
        //           "display_position": "SP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/dgn5PVz2t1_dtXi8Oeh.ow--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04182019/9586.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/dgn5PVz2t1_dtXi8Oeh.ow--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04182019/9586.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "SP",
        //           "eligible_positions": {
        //             "position": [
        //               "SP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1559974020,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "BN"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.10683",
        //           "player_id": 10683,
        //           "name": {
        //             "full": "Nick Pivetta",
        //             "first": "Nick",
        //             "last": "Pivetta",
        //             "ascii_first": "Nick",
        //             "ascii_last": "Pivetta"
        //           },
        //           "editorial_player_key": "mlb.p.10683",
        //           "editorial_team_key": "mlb.t.22",
        //           "editorial_team_full_name": "Philadelphia Phillies",
        //           "editorial_team_abbr": "Phi",
        //           "uniform_number": 43,
        //           "display_position": "SP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/0V9cPkV8GJ6KSqo2qOwrjg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04102019/10683.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/0V9cPkV8GJ6KSqo2qOwrjg--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04102019/10683.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "SP",
        //           "eligible_positions": {
        //             "position": [
        //               "SP",
        //               "P"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560033180,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "BN"
        //           }
        //         },
        //         {
        //           "player_key": "388.p.9630",
        //           "player_id": 9630,
        //           "name": {
        //             "full": "Joey Gallo",
        //             "first": "Joey",
        //             "last": "Gallo",
        //             "ascii_first": "Joey",
        //             "ascii_last": "Gallo"
        //           },
        //           "editorial_player_key": "mlb.p.9630",
        //           "editorial_team_key": "mlb.t.13",
        //           "editorial_team_full_name": "Texas Rangers",
        //           "editorial_team_abbr": "Tex",
        //           "uniform_number": 13,
        //           "display_position": "1B,OF",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/qEpFkbHDERk78ftJiq3snA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04302019/9630.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/qEpFkbHDERk78ftJiq3snA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/04302019/9630.png",
        //           "is_undroppable": 0,
        //           "position_type": 0,
        //           "primary_position": "1B",
        //           "eligible_positions": {
        //             "position": [
        //               "1B",
        //               "IF",
        //               "OF",
        //               "Util",
        //               "DL"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560290700,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "DL"
        //           },
        //           "has_recent_player_notes": 1,
        //           "status": "DL10",
        //           "status_full": "10-Day Disabled List",
        //           "on_disabled_list": 1
        //         },
        //         {
        //           "player_key": "388.p.10762",
        //           "player_id": 10762,
        //           "name": {
        //             "full": "Caleb Smith",
        //             "first": "Caleb",
        //             "last": "Smith",
        //             "ascii_first": "Caleb",
        //             "ascii_last": "Smith"
        //           },
        //           "editorial_player_key": "mlb.p.10762",
        //           "editorial_team_key": "mlb.t.28",
        //           "editorial_team_full_name": "Miami Marlins",
        //           "editorial_team_abbr": "Mia",
        //           "uniform_number": 31,
        //           "display_position": "SP",
        //           "headshot": {
        //             "url": "https://s.yimg.com/iu/api/res/1.2/F6aSuc9briJOOo7QAUwWiA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/05022019/10762.png",
        //             "size": 1
        //           },
        //           "image_url": "https://s.yimg.com/iu/api/res/1.2/F6aSuc9briJOOo7QAUwWiA--~C/YXBwaWQ9eXNwb3J0cztjaD0yMzM2O2NyPTE7Y3c9MTc5MDtkeD04NTc7ZHk9MDtmaT11bGNyb3A7aD02MDtxPTEwMDt3PTQ2/https://s.yimg.com/xe/i/us/sp/v/mlb_cutout/players_l/05022019/10762.png",
        //           "is_undroppable": 0,
        //           "position_type": 1,
        //           "primary_position": "SP",
        //           "eligible_positions": {
        //             "position": [
        //               "SP",
        //               "P",
        //               "DL"
        //             ]
        //           },
        //           "has_player_notes": 1,
        //           "player_notes_last_timestamp": 1560283680,
        //           "selected_position": {
        //             "coverage_type": 0,
        //             "date": "2019-06-12",
        //             "position": "DL"
        //           },
        //           "has_recent_player_notes": 1,
        //           "status": "DL10",
        //           "status_full": "10-Day Disabled List",
        //           "on_disabled_list": 1
        //         }
        //       ]
        //     },
        //     "outs_pitched": {
        //       "coverage_type": "week",
        //       "coverage_value": 11,
        //       "value": 44
        //     }
        //   }
        // }
