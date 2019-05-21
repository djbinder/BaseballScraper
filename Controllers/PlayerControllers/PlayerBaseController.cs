using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.Player;
using Ganss.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace BaseballScraper.Controllers.Player
{

    #region OVERVIEW ------------------------------------------------------------

        /// <summary> Retrieve all records from PlayerBase.xlsx; retrieve individual player records from PlayerBase.xlsx </summary>
        /// <list> INDEX
        ///     <item> View all player bases <see cref="PlayerBaseController.ViewPlayerBaseHome" /> </item>
        ///     <item> Get all players bases <see cref="PlayerBaseController.GetAllPlayerBasesFromExcel" /> </item>
        ///     <item> Get all players bases by team <see cref="PlayerBaseController.GetAllPlayerBasesForOneMlbTeam(string)" /> </item>
        ///     <item> Get one player's base by mlb id <see cref="PlayerBaseController.GetOnePlayersBaseFromMlbId(string)" /> </item>
        ///     <item> Get one player's base by sfbb id <see cref="PlayerBaseController.GetOnePlayersBaseFromSfbbId(string)" /> </item>
        ///     <item> Get one player's base by baseball hq id <see cref="PlayerBaseController.GetOnePlayersBaseFromBaseballHqId(string)" /> </item>
        ///     <item> Get one player's base by davenport id <see cref="PlayerBaseController.GetOnePlayersBaseFromDavenportId(string)" /> </item>
        ///     <item> Get one player's base by baseball prospectus id <see cref="PlayerBaseController.GetOnePlayersBaseFromBaseballProspectusId(string)" /> </item>
        ///     <item> Get one player's base by baseball reference id <see cref="PlayerBaseController.GetOnePlayersBaseFromBaseballReferenceId(string)" /> </item>
        ///     <item> Get one player's base by cbs id <see cref="PlayerBaseController.GetOnePlayersBaseFromCbsId(string)" /> </item>
        ///     <item> Get one player's base by espn id <see cref="PlayerBaseController.GetOnePlayersBaseFromEspnId(string)" /> </item>
        ///     <item> Get one player's base by fangraphs id <see cref="PlayerBaseController.GetOnePlayersBaseFromFanGraphsId(string)" /> </item>
        ///     <item> Get one player's base by lahman id <see cref="PlayerBaseController.GetOnePlayersBaseFromLahmanId(string)" /> </item>
        ///     <item> Get one player's base by nfbc id <see cref="PlayerBaseController.GetOnePlayersBaseFromNfbcId(string)" /> </item>
        ///     <item> Get one player's base by retro id <see cref="PlayerBaseController.GetOnePlayersBaseFromRetroId(string)" /> </item>
        ///     <item> Get one player's base by yahoo id <see cref="PlayerBaseController.GetOnePlayersBaseFromYahooId(string)" /> </item>
        ///     <item> Get one player's base by ottoneu id <see cref="PlayerBaseController.GetOnePlayersBaseFromOttoneuId(string)" /> </item>
        ///     <item> Get one player's base by rotowire id <see cref="PlayerBaseController.GetOnePlayersBaseFromRotoWireId(string)" /> </item>
        /// </list>
        /// <list> RESOURCES
        ///     <item> https://github.com/mganss/ExcelMapper </item>
        ///     <item> https://www.smartfantasybaseball.com/tools/ </item>
        ///     <item> http://crunchtimebaseball.com/baseball_map.html </item>
        /// </list>

    #endregion OVERVIEW ------------------------------------------------------------



    #pragma warning disable CS0414, CS0219, IDE0051, IDE0059, CS1591, CS1570
    public class PlayerBaseController: Controller
    {
        private readonly Helpers _h = new Helpers();

        public GoogleSheetsConnector _gSC = new GoogleSheetsConnector();



        [HttpGet("player_base")]
        public IActionResult ViewPlayerBaseHome()
        {
            string content = "player note main content";
            PlayerBaseFromGoogleSheet.GetAllPlayerBaseObjectsFromGoogleSheet("A5:AP2284");
            return Content(content);
        }



        public class PlayerBaseFromGoogleSheet
        {
            private static readonly Helpers _h = new Helpers();
            private static readonly GoogleSheetsConnector _gSC = new GoogleSheetsConnector();
            private static readonly string sfbbMapDocName = "SfbbPlayerIdMap";
            private static readonly string sfbbMapTabId = "SFBB_PLAYER_ID_MAP";

            public PlayerBaseFromGoogleSheet()
            {
            }


            // PlayerBaseFromGoogleSheet.GetAllPlayerBaseObjectsFromGoogleSheet("A5:AP2284");
            public static IList<IList<Object>> GetAllPlayerBaseObjectsFromGoogleSheet(string range)
            {
                var allPlayerBases = _gSC.ReadDataFromSheetRange(sfbbMapDocName,sfbbMapTabId,range);

                var countOfAllPlayerBases = allPlayerBases.ToList().Count();
                    // Console.WriteLine($"Current # of Players (sheets): {countOfAllPlayerBases}");

                return allPlayerBases;
            }


            // STATUS May 13, 2019: In progress; currently only returns an empty IEnumerable
            public static IEnumerable<SfbbPlayerBase> GetAllPlayerBasesFromGoogleSheet(string range)
            {
                // _h.StartMethod();
                // Console.WriteLine($"range: {range}");
                IList<IList<object>> allPlayerBaseObjects = _gSC.ReadDataFromSheetRange(sfbbMapDocName,sfbbMapTabId,range);
                    // var countOfAllPlayerBases = allPlayerBaseObjects.ToList().Count();
                    // Console.WriteLine($"Current # of Players: {countOfAllPlayerBases}");

                // IEnumerable<SfbbPlayerBase> allPlayerBases = Enumerable.Empty<SfbbPlayerBase>();
                List<SfbbPlayerBase> allPlayerBases = new List<SfbbPlayerBase>();

                var intHolder = 0;
                foreach(var row in allPlayerBaseObjects)
                {
                    // Console.WriteLine(row.Count);
                    // int rowCounter = 0;
                    SfbbPlayerBase playerBase = new SfbbPlayerBase
                    {
                        IDPLAYER        = row[0].ToString(),
                        PLAYERNAME      = row[1].ToString(),
                        BIRTHDATE       = row[2].ToString(),
                        FIRSTNAME       = row[3].ToString(),
                        LASTNAME        = row[4].ToString(),
                        TEAM            = row[5].ToString(),
                        LG              = row[6].ToString(),
                        POS             = row[7].ToString(),
                        IDFANGRAPHS     = row[8].ToString(),
                        FANGRAPHSNAME   = row[9].ToString(),
                        MLBID           = row[10].ToString(),
                        MLBNAME         = row[11].ToString(),
                        CBSID           = row[12].ToString(),
                        CBSNAME         = row[13].ToString(),
                        RETROID         = row[14].ToString(),
                        BREFID          = row[15].ToString(),
                        NFBCID          = row[16].ToString(),
                        NFBCNAME        = row[17].ToString(),
                        ESPNID          = row[18].ToString(),
                        ESPNNAME        = row[19].ToString(),
                        KFFLNAME        = row[20].ToString(),
                        DAVENPORTID     = row[21].ToString(),
                        BPID            = row[22].ToString(),
                        YAHOOID         = row[23].ToString(),
                        YAHOONAME       = row[24].ToString(),
                        MSTRBLLNAME     = row[25].ToString(),
                        BATS            = row[26].ToString(),
                        THROWS          = row[27].ToString(),
                        FANPROSNAME     = row[28].ToString(),
                        LASTCOMMAFIRST  = row[29].ToString(),
                        ROTOWIREID      = row[30].ToString(),
                        FANDUELNAME     = row[31].ToString(),
                        FANDUELID       = row[32].ToString(),
                        DRAFTKINGSNAME  = row[33].ToString(),
                        OTTONEUID       = row[34].ToString(),
                        HQID            = row[35].ToString(),
                        RAZZBALLNAME    = row[36].ToString(),
                        FANTRAXID       = row[37].ToString(),
                        FANTRAXNAME     = row[38].ToString(),
                        ROTOWIRENAME    = row[39].ToString(),
                        ALLPOS          = row[40].ToString(),
                        NFBCLASTFIRST   = row[41].ToString()
                    };
                    allPlayerBases.Add(playerBase);

                    // try {
                    //     playerBase.NFBCLASTFIRST = row[41].ToString();
                    // }
                    // catch {
                    //     Console.WriteLine("error");
                    // }

                //     if(intHolder == 0)
                //     {
                //         Console.WriteLine($"ID PLAYER 1: {playerBase.IDPLAYER}");
                //         Console.WriteLine($"PLAYERNAME: {playerBase.PLAYERNAME}");
                //         Console.WriteLine($"RETROID: {playerBase.RETROID}");
                //         Console.WriteLine($"BIRTHDATE: {playerBase.ESPNNAME}");
                //         Console.WriteLine($"NFBCLASTFIRST: {playerBase.NFBCLASTFIRST}");
                //     }

                //     Console.WriteLine($"intHolder: {intHolder}");
                //     Console.WriteLine($"PLAYERNAME: {playerBase.PLAYERNAME}");
                //     Console.WriteLine($"RETROID: {playerBase.RETROID}");
                //     Console.WriteLine($"NFBCLASTFIRST: {playerBase.NFBCLASTFIRST}");

                //     intHolder++;
                // }

                // Console.WriteLine();

                // var intHolder2 = 0;
                // foreach(var player in allPlayerBases)
                // {
                //     if(intHolder2 == 0 )
                //     {
                //         Console.WriteLine(player.IDPLAYER);
                //         Console.WriteLine(player.PLAYERNAME);
                //         Console.WriteLine(player.RETROID);

                //         Console.WriteLine(player.ESPNNAME);
                //         Console.WriteLine(player.NFBCLASTFIRST);

                //     }
                //     intHolder2++;
                }
                return allPlayerBases;
            }


            public static void GetOnePlayerBaseColumnFromGoogleSheet(string range)
            {
                _h.StartMethod();
                var allPlayerBaseObjects = _gSC.ReadDataFromSheetRange(sfbbMapDocName,sfbbMapTabId,range);
                IEnumerable<SfbbPlayerBase> allPlayerBases = Enumerable.Empty<SfbbPlayerBase>();
            }
        }



        #region ALL PLAYER BASES ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Retrieves all records from PlayerBase.xlsx document </summary>
            /// <returns> IEnumerable<PlayerBase> allPlayerBases </returns>
            public IEnumerable<PlayerBase> GetAllPlayerBasesFromExcel()
            {
                // Ganss.Excel.ExcelMapper+<Fetch>d__34`1[BaseballScraper.Models.PlayerBase]
                // IEnumerable<PlayerBase> allPlayerBases
                var allPlayerBases = new ExcelMapper("BaseballData/PlayerBase/PlayerBase.xlsx").Fetch<PlayerBase>();

                var countOfAllPlayerBases = allPlayerBases.ToList().Count();
                    Console.WriteLine($"Current # of Players: {countOfAllPlayerBases}");

                return allPlayerBases;
            }


            // STATUS: this works
            /// <summary> Retrieves all records from PlayerBase.xlsx document for one team </summary>
            /// <param name="teamNameFull"> A full mlb team name (e.g., "Boston Red Sox" </param>
            /// <example> GetAllPlayerBasesForOneMlbTeam("Chicago Cubs"); </example>
            /// <returns> IEnumerable<PlayerBase> allPlayerBases </returns>
            public IEnumerable<PlayerBase> GetAllPlayerBasesForOneMlbTeam(string teamNameFull)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var playerBasesForOneMlbTeam  =
                    from playerBases in allPlayerBases
                    where playerBases.MlbTeamLong == teamNameFull
                    select playerBases;

                // foreach(var player in playerBasesForOneMlbTeam)
                // {
                //     Console.WriteLine(player.MlbName);
                // }

                return playerBasesForOneMlbTeam;
            }

        #endregion ALL PLAYER BASES ------------------------------------------------------------



        #region PLAYER QUERY BY ANY PLAYER ID TYPE ------------------------------------------------------------

            // STATUS: these all work
            // TODO: there has got to be a way where you only need one method vs. separating into each like below
            /// <summary> Each of methods in this section returns a player (from IEnumerable<PlayerBase>). The only difference is the type of Id you are passing in (e.g. MlbId, FanGraphsPlayerId, EspnPlayerId etc.) </summary>
            /// <returns>
            /// IEnumerable<PlayerBase> playerbase (i.e. a PlayerBase for one player)
            /// </returns>

            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromMlbId(string playersMlbId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                // IEnumerable<PlayerBase> onePlayersBase
                var onePlayersBase = from playerBases in allPlayerBases
                    where playerBases.MlbId == playersMlbId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromSfbbId(string playersSfbbPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.SfbbPlayerId == playersSfbbPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromBaseballHqId(string playersBaseballHqPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.BaseballHqPlayerId == playersBaseballHqPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromDavenportId(string playersDavenportPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.DavenportId == playersDavenportPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromBaseballProspectusId(string playersBaseballProspectusPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.BaseballProspectusPlayerId == playersBaseballProspectusPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromBaseballReferenceId(string playersBaseballReferencePlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.BaseballReferencePlayerId == playersBaseballReferencePlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromCbsId(string playersCbsPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.CbsPlayerId == playersCbsPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromEspnId(string playersEspnPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.EspnPlayerId == playersEspnPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromFanGraphsId(string playersFanGraphsPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.FanGraphsPlayerId == playersFanGraphsPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromLahmanId(string playersLahmanPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.LahmanPlayerId == playersLahmanPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromNfbcId(string playersNfbcPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.NfbcPlayerId == playersNfbcPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromRetroId(string playersRetroPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.RetroPlayerId == playersRetroPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromYahooId(string playersYahooPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.YahooPlayerId == playersYahooPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromOttoneuId(string playersOttoneuPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.OttoneuPlayerId == playersOttoneuPlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromRotoWireId(string playersRotoWirePlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.RotoWirePlayerId == playersRotoWirePlayerId
                    select playerBases;

                // ITEM type --> BaseballScraper.Models.PlayerBase
                foreach(var item in onePlayersBase)
                {
                    Console.WriteLine(item.CbsName);
                }
                return onePlayersBase;
            }


            // NOTE: for testing purposes only; tests all of the other methods in this section
            public void TestAll()
            {
                string mlbId = "595918";
                string sfbbId = "coleaj01";
                string baseballHqId = "4545";
                string davenportId = "COLE19920105A";
                string baseballProspectusId = "68086";
                string baseballReferenceId = "coleaj01";
                string cbsId = "1839916";
                string espnId = "31595";
                string fanGraphsId = "11467";
                string lahmanId = "coleaj01";
                string nfbcId = "9638";
                string retroId = "colea002";
                string yahooId = "9638";
                string ottoneuId = "14940";
                string rotoWireId = "11446";

                // var playersBase = GetOnePlayersBaseFromMlbId(playerIdToQuery);
                GetOnePlayersBaseFromMlbId(mlbId);
                GetOnePlayersBaseFromSfbbId(sfbbId);
                GetOnePlayersBaseFromBaseballHqId(baseballHqId);
                GetOnePlayersBaseFromDavenportId(davenportId);
                GetOnePlayersBaseFromBaseballProspectusId(baseballProspectusId);
                GetOnePlayersBaseFromBaseballReferenceId(baseballReferenceId);
                GetOnePlayersBaseFromCbsId(cbsId);
                GetOnePlayersBaseFromEspnId(espnId);
                GetOnePlayersBaseFromFanGraphsId(fanGraphsId);
                GetOnePlayersBaseFromLahmanId(lahmanId);
                GetOnePlayersBaseFromNfbcId(nfbcId);
                GetOnePlayersBaseFromRetroId(retroId);
                GetOnePlayersBaseFromYahooId(yahooId);
                GetOnePlayersBaseFromOttoneuId(ottoneuId);
                GetOnePlayersBaseFromRotoWireId(rotoWireId);
            }

        #endregion PLAYER QUERY BY ANY PLAYER ID TYPE ------------------------------------------------------------



        #region GET ALL IDs FOR A PLAYER ------------------------------------------------------------

            // STATUS: this works
            /// <summary> This method created a list of a player's ids from all id types available in the PlayerBase Excel file </summary>
            /// <param name="playersBase"> An instantiated PlayerBase</param>
            /// <returns> A list of all of a player's ids </returns>
            public List<string> GetAllPlayerIdsList(PlayerBase playersBase)
            {
            List<string> listOfPlayersIds = new List<string>
            {
                playersBase.MlbId,
                playersBase.SfbbPlayerId,
                playersBase.BaseballHqPlayerId,
                playersBase.DavenportId,
                playersBase.BaseballProspectusPlayerId,
                playersBase.BaseballReferencePlayerId,
                playersBase.CbsPlayerId,
                playersBase.EspnPlayerId,
                playersBase.FanGraphsPlayerId,
                playersBase.LahmanPlayerId,
                playersBase.NfbcPlayerId,
                playersBase.RetroPlayerId,
                playersBase.YahooPlayerId,
                playersBase.OttoneuPlayerId,
                playersBase.RotoWirePlayerId
            };

            foreach (var id in listOfPlayersIds)
                {
                    Console.WriteLine(id);
                }

                return listOfPlayersIds;
            }


            // STATUS: this works
            /// <summary> This method created a dictionary of a player's ids from all id types available in the PlayerBase; The keys are the id type, the values are the actual id number  </summary>
            /// <param name="playersBase"> An instantiated PlayerBase </param>
            /// <returns> A dictionary of all of a player's ids with key value pairs of Key: Id type, Value: id number/string </returns>
            public Dictionary<string, string> GetAllPlayerIdsDictionary(PlayerBase playersBase)
            {
            Dictionary<string, string> dictionaryOfPlayersIds = new Dictionary<string, string>
            {
                { "MlbId", playersBase.MlbId },
                { "SfbbPlayerId", playersBase.SfbbPlayerId },
                { "BaseballHqPlayerId", playersBase.BaseballHqPlayerId },
                { "DavenportId", playersBase.DavenportId },
                { "BaseballProspectusPlayerId", playersBase.BaseballProspectusPlayerId },
                { "BaseballReferencePlayerId", playersBase.BaseballReferencePlayerId },
                { "CbsPlayerId", playersBase.CbsPlayerId },
                { "EspnPlayerId", playersBase.EspnPlayerId },
                { "FanGraphsPlayerId", playersBase.FanGraphsPlayerId },
                { "LahmanPlayerId", playersBase.LahmanPlayerId },
                { "NfbcPlayerId", playersBase.NfbcPlayerId },
                { "RetroPlayerId", playersBase.RetroPlayerId },
                { "YahooPlayerId", playersBase.YahooPlayerId },
                { "OttoneuPlayerId", playersBase.OttoneuPlayerId },
                { "RotoWirePlayerId", playersBase.RotoWirePlayerId }
            };

            foreach (var kvp in dictionaryOfPlayersIds)
                {
                    Console.WriteLine($"{kvp.Key} -->  {kvp.Value}");
                }

                return dictionaryOfPlayersIds;
            }

        #endregion GET ALL IDs FOR A PLAYER ------------------------------------------------------------



        #region OTHER ------------------------------------------------------------

            // NOTE: not sure either of these are actually needed; but good practice regardless

            public void GetPlayerBaseProperties()
            {
                PropertyInfo[] propertyInfos = typeof(PlayerBase).GetProperties();

                Array.Sort(propertyInfos,
                    delegate(PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
                    { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

                foreach(PropertyInfo propertyInfo in propertyInfos)
                {
                    Console.WriteLine(propertyInfo.Name);
                }
            }

            public void GetAllPlayersByMlbTeam()
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var mlbTeamsPlayers =
                    from onePlayersBase in allPlayerBases
                    group onePlayersBase by onePlayersBase.MlbTeam into mlbTeams
                    select new
                    {
                        Team = mlbTeams.Key,
                        Count = mlbTeams.Count(),
                    };
            }

        #endregion OTHER ------------------------------------------------------------



    }
}
