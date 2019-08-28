using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.Player;
using Ganss.Excel;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS0168, CS0219, CS0414, CS1998, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.PlayerControllers
{


    [Route("api/player/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PlayerBaseController: ControllerBase
    {
        private readonly Helpers                   _helpers;
        private readonly BaseballScraperContext    _context;
        private readonly CsvHandler                _csvHandler;
        private readonly PlayerBaseFromGoogleSheet _playerBaseFromGoogleSheet;
        private readonly GoogleSheetsConnector     _googleSheetsConnector;
        private readonly ProjectDirectoryEndPoints _baseballData;

        public readonly string _sfbbMapDocName = "SfbbPlayerIdMap";
        public readonly string _sfbbMapTabId = "SFBB_PLAYER_ID_MAP";

        // private readonly string _crunchTimePlayerBaseCsv = "http://crunchtimebaseball.com/master.csv";
        // private readonly string _crunchTimeCsvTargetWritePath = "BaseballData/02_Target_Write/PlayerBase_Target_Write/";
        // private readonly string _crunchTimePlayerMapFileName = "CrunchtimePlayerBaseCsv";

        private Dictionary<string, object> _d = new Dictionary<string, object>();

        public PlayerBaseController(Helpers helpers, BaseballScraperContext context, CsvHandler csvHandler, PlayerBaseFromGoogleSheet playerBaseFromGoogleSheet, GoogleSheetsConnector googleSheetsConnector, ProjectDirectoryEndPoints baseballData)
        {
            _helpers                   = helpers;
            _context                   = context;
            _csvHandler                = csvHandler;
            _googleSheetsConnector     = googleSheetsConnector;
            _playerBaseFromGoogleSheet = playerBaseFromGoogleSheet;
            _baseballData              = baseballData;
        }


        public PlayerBaseController() {}


        // https://127.0.0.1:5001/api/player/playerbase/test
        [Route("test")]
        public void ViewPlayerBaseHome()
        {
        }

        /*
            https://127.0.0.1:5001/api/player/playerbase/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
            var playerBases = CreateListOfCrunchTimePlayerBasesForToday();
            await AddAllCrunchTimePlayerBasesToDatabase(playerBases);
        }


        // List of Player Base resources
        // * CrunchTime Map : http://crunchtimebaseball.com/baseball_map.html
        // * CrunchTime CSV : http://crunchtimebaseball.com/master.csv
        // * SFBB Player Id Map : http://bit.ly/2UdNAGy



        #region CRUNCHTIME ------------------------------------------------------------

            // "BaseballData/02_WRITE/PlayerBase/CRUNCH_TIME/"
            // * Defined in: ProjectDirectoryEndPoints
            private string CrunchTimeWriteFolder
            {
               get => _baseballData.CrunchTimeWriteDirectoryRelativePath;
            }

            // "CrunchTime_Csv"
            // * Defined in: ProjectDirectoryEndPoints
            private string CrunchTimeReportFileBaseName
            {
                get => _baseballData.CrunchTimeReportFileBaseName;
            }

            // "http://crunchtimebaseball.com/master.csv"
            // * Defined in: ProjectDirectoryEndPoints
            public string CrunchTimeCsvSourceUrl
            {
                get => _baseballData.CrunchTimePlayerBaseCsvSourceUrl;
            }


            // STATUS [ August 7, 2019 ] : this works
            /// <summary>
            /// * Downloads CrunchTime CSV and saves to local directory
            /// </summary>
            /// <remarks>
            /// * STEPS:
            /// * 1) Downloads CSV From http://crunchtimebaseball.com/master.csv
            /// * 2) Saves CSV to local directory
            /// *    > Directory Example: BaseballData/02_WRITE_CRUNCH_TIME
            /// *    > File Name Example: CrunchTime_Csv_8_26_2019.csv
            /// *    > These are set in corresponding EndPoints class
            /// </remarks>
            public void GetCrunchTimePlayerBaseCsvForToday()
            {
                string todayString       = _csvHandler.TodaysDateString();
                string locationToWriteTo = $"{CrunchTimeWriteFolder}{CrunchTimeReportFileBaseName}_{todayString}.csv";
                bool doesFileExist       = _csvHandler.CheckIfFileExists(locationToWriteTo);

                Console.WriteLine($"locationToWriteTo: {locationToWriteTo}");
                Console.WriteLine($"doesFileExist : {doesFileExist}");

                if(doesFileExist == true)
                { // Console.WriteLine("FILE ALREADY EXISTS");
                }
                else
                {
                    Console.WriteLine("FILE DOES NOT EXIST - DOWNLOADING TODAY'S REPORT");
                    _csvHandler.DownloadCsvFromLink(CrunchTimeCsvSourceUrl, locationToWriteTo);
                }
                PrintCrunchTimeCsvDownloadInfo(todayString);
            }


            // public void ReadCrunchTimePlayerBaseCsv(string fullFilePathAndName)
            public List<CrunchTimePlayerBase> CreateListOfCrunchTimePlayerBasesForToday()
            {
                string todayString         = _csvHandler.TodaysDateString();
                string fullFilePathAndName = $"{CrunchTimeWriteFolder}{CrunchTimeReportFileBaseName}_{todayString}.csv";

                var listOfObjects = _csvHandler.ReadCsvRecordsToList(
                    fullFilePathAndName,
                    typeof(CrunchTimePlayerBase),
                    typeof(CrunchTimePlayerBaseClassMap)
                );

                List<CrunchTimePlayerBase> crunchTimePlayerBases = new List<CrunchTimePlayerBase>();

                foreach(var csvRow in listOfObjects)
                {
                    CrunchTimePlayerBase crunchTimePlayerBase = csvRow as CrunchTimePlayerBase;
                    crunchTimePlayerBases.Add(crunchTimePlayerBase);
                }

                // Console.WriteLine($"listOfObjects: {listOfObjects.Count}");
                // Console.WriteLine($"crunchTimePlayerBases: {crunchTimePlayerBases.Count}");
                return crunchTimePlayerBases;
            }



            /* --------------------------------------------------------------- */
            /* CRUD - CRUNCH TIME PLAYERBASE                                   */
            /* --------------------------------------------------------------- */

            /* ----- CRUD - CREATE - CRUNCH TIME PLAYERBASE ----- */


            // STATUS [ August 27, 2019 ] : this works
            [HttpPost("crunch_time_player_base/all")]
            public async Task<ActionResult> AddAllCrunchTimePlayerBasesToDatabase(List<CrunchTimePlayerBase> crunchTimePlayerBases)
            {
                int counter = 1;
                foreach(var playerBase in crunchTimePlayerBases)
                {
                    var checkDbForPlayerBase = _context.CrunchTimePlayerBases.SingleOrDefault(ct => ct.MlbId == playerBase.MlbId);

                    if(checkDbForPlayerBase == null)
                    {
                        await _context.AddAsync(playerBase);
                    }

                    else
                    {
                        Console.WriteLine($"{counter} | CRUNCH TIME PLAYER BASE ALREADY EXISTS");
                    }
                    counter++;
                }
                await _context.SaveChangesAsync();
                return Ok();
            }


            // STATUS [ August 27, 2019 ] : haven't tested but should work
            [HttpPost("crunch_time_player_base/{crunchTimePlayerBase.MlbId}")]
            public async Task<ActionResult> AddOneCrunchTimePlayerBasesToDatabase(CrunchTimePlayerBase crunchTimePlayerBase)
            {
                await _context.AddAsync(crunchTimePlayerBase);
                await _context.SaveChangesAsync();
                return Ok();
            }


            /* ----- CRUD - READ - CRUNCH TIME PLAYERBASE ----- */

            // STATUS [ August 27, 2019 ] : haven't tested but should work
            [HttpGet("crunch_time_player_base/{mlbId}")]
            public async Task<ActionResult> GetCrunchTimePlayerBasesFromDatabase(int mlbId)
            {
                var player = _context.CrunchTimePlayerBases.SingleOrDefault(p => p.MlbId == mlbId);
                return Ok(player);
            }


            /* ----- CRUD - DELETE - CRUNCH TIME PLAYERBASE ----- */

            // STATUS [ August 27, 2019 ] : this works
            [HttpDelete("crunch_time_player_base/delete_all")]
            public ActionResult DeleteAllCrunchTimePlayerBases()
            {
                _context.CrunchTimePlayerBases.RemoveRange(_context.CrunchTimePlayerBases);
                _context.SaveChanges();
                return Ok();
            }

        #endregion CRUNCHTIME ------------------------------------------------------------





        #region SFBB DATABASE ------------------------------------------------------------

            public IEnumerable<SfbbPlayerBase> GetAllSfbbPlayerBasesFromGoogleSheet(string range)
            {
                _helpers.StartMethod();
                IList<IList<object>> allPlayerBaseObjects = _googleSheetsConnector.ReadDataFromSheetRange(
                    _sfbbMapDocName,
                    _sfbbMapTabId,
                    range
                );

                PrintPlayerBaseObjectDetails(allPlayerBaseObjects, _sfbbMapDocName, _sfbbMapTabId, range);

                List<SfbbPlayerBase> allPlayerBases = new List<SfbbPlayerBase>();
                int counter = 1;
                int mlbIdCounter = 1;
                int hqIdCounter = 1;

                foreach(IList<object> row in allPlayerBaseObjects)
                {
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
                        // MLBID           = Int32.Parse(row[10].ToString()),
                        // MLBID           = _csvHandler.ParseNullableInt(row[10].ToString()),
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
                        FANTPROSNAME     = row[28].ToString(),
                        LASTCOMMAFIRST  = row[29].ToString(),
                        ROTOWIREID      = row[30].ToString(),
                        FANDUELNAME     = row[31].ToString(),
                        FANDUELID       = row[32].ToString(),
                        DRAFTKINGSNAME  = row[33].ToString(),
                        OTTONEUID       = row[34].ToString(),
                        // HQID            = Int32.Parse(row[35].ToString()),
                        RAZZBALLNAME    = row[36].ToString(),
                        FANTRAXID       = row[37].ToString(),
                        FANTRAXNAME     = row[38].ToString(),
                        ROTOWIRENAME    = row[39].ToString(),
                        ALLPOS          = row[40].ToString(),
                        NFBCLASTFIRST   = row[41].ToString()
                    };

                    if(row[10].ToString() == "")
                    {
                        // playerBase.MLBID = mlbIdCounter;
                        mlbIdCounter++;
                    }

                    else
                    {
                        string mlbIdString = row[10].ToString();
                        int mlbIdInt = Int32.Parse(mlbIdString);
                        playerBase.MLBID = mlbIdInt;
                    }

                    if(row[35].ToString() == "")
                    {
                        // playerBase.HQID = hqIdCounter;
                        hqIdCounter++;
                    }

                    else
                    {
                        string hqString = row[35].ToString();
                        int hqIdInt = int.Parse(hqString);
                        playerBase.HQID = hqIdInt;
                    }

                    allPlayerBases.Add(playerBase);
                    // Console.WriteLine($"NEW: {playerBase.PLAYERNAME}\t MLB ID: {playerBase.MLBID}\t HQ ID: {playerBase.HQID}");
                }
                return allPlayerBases;
            }

            /* --------------------------------------------------------------- */
            /* CRUD - SFBB PLAYERBASE                                          */
            /* --------------------------------------------------------------- */


            /* ----- CRUD - CREATE - SFBB PLAYERBASE ----- */

            // STATUS [ August 19, 2019 ] : this works but needs updates
            // * Pull range out as a parameter
            // * Define list outside of method
            /// <summary>
            ///     Add list of playerBases
            /// </summary>
            [HttpPost("sfbb_player_base")]
            public async Task<ActionResult> AddSfbbPlayerBasesToDatabase()
            {
                _helpers.StartMethod();
                var allPlayerBases = GetAllSfbbPlayerBasesFromGoogleSheet("A7:AQ2333");
                int counter = 1;
                foreach(SfbbPlayerBase playerBase in allPlayerBases)
                {
                    await AddSfbbPlayerBaseToDatabase(playerBase);
                    if(counter <= 3000)
                    {
                        counter++;
                    }
                }
                return Ok();
            }


            // STATUS [ August 19, 2019 ] : should work and be faster than AddSfbbPlayerBasesToDatabase()
            // [HttpPost("sfbb_player_base/all")]
            // public async Task<ActionResult> AddAllSfbbPlayerBasesToDatabase(IEnumerable<SfbbPlayerBase> allPlayerBases)
            // {
            //     _helpers.StartMethod();
            //     foreach(SfbbPlayerBase playerBase in allPlayerBases)
            //     {
            //         try
            //         {
            //             await _context.AddAsync(playerBase);
            //         }
            //         catch
            //         {
            //             Console.WriteLine($"ISSUE ADDING: {playerBase.PLAYERNAME} to SFBB DATABASE");
            //         }
            //     }
            //     await _context.SaveChangesAsync();
            //     return Ok();
            // }


            // STATUS [ August 27, 2019 ] : not tested but should work; should be faster than 'AddSfbbPlayerBasesToDatabase()'
            // * Because SaveChangesAsync is only called once
            // List would be something like:
            // * var allPlayerBases = _playerBaseFromGoogleSheet.GetAllPlayerBasesFromGoogleSheet("A7:AQ2333");
            // [HttpPost("sfbb_player_base/all")]
            // public async Task<ActionResult> AddAllSfbbPlayerBasesToDatabase(List<SfbbPlayerBase> allPlayerBases)
            // {
            //     _helpers.StartMethod();
            //     foreach(SfbbPlayerBase playerBase in allPlayerBases)
            //     {
            //         await _context.AddAsync(playerBase);
            //     }
            //     await _context.SaveChangesAsync();
            //     return Ok();
            // }


            // STATUS [ August 19, 2019 ] : this works
            /// <summary>
            ///     Add one SfbbPlayer to database
            /// </summary>
            /// <remarks>
            ///     * A variety of errors pop up in this process
            ///     * StringBuilder used to track paths:
            ///     * > A --> B : Exists; do nothing
            ///     * > A --> C : Does not exist; Create successful
            ///     * > A --> D --> E : Does not exist; Some kind of error; Create successful
            ///     * > A --> D --> F : Does not exist; Some kind of error; Create fails
            ///     * Connected to a Google Sheet that I do not own (See: https://bit.ly/2MntLLP)
            ///     * I auto import that data to a sheet I do own
            /// </remarks>
            /// <param name="playerBase">
            ///     An SfbbPlayerBase
            /// </param>
            [HttpPost("sfbb_player_base/{playerBase.IDPLAYER}")]
            public async Task<ActionResult> AddSfbbPlayerBaseToDatabase(SfbbPlayerBase playerBase)
            {
                _helpers.StartMethod();
                StringBuilder sb = new StringBuilder();
                try
                {
                    sb.Append("A"); // A --> CHECK IF PLAYER EXISTS IN DATABASE
                    SfbbPlayerBase pBase = _context.SfbbPlayerBases.SingleOrDefault(y => y.IDPLAYER == playerBase.IDPLAYER);

                    if(pBase != null)
                    {
                        Console.WriteLine($"SFBB PLAYER BASE EXISTS: {pBase.PLAYERNAME}");
                        sb.Append(" --> B"); // B --> ALREADY IN DB
                    }

                    else
                    {
                        sb.Append(" --> C"); // C --> ADD TO DATABASE
                        await _context.AddAsync(playerBase);
                        await _context.SaveChangesAsync();
                    }
                }
                catch(Exception ex)
                {
                    sb.Append(" --> D"); // D --> NOT IN DATABASE
                    try
                    {
                        sb.Append(" --> E"); // E --> ADD TO DB
                        await _context.AddAsync(playerBase);
                        await _context.SaveChangesAsync();
                    }

                    catch(Exception ex2)
                    {
                        sb.Append(" --> F"); // F --> NOT ADDED TO DATABASE - DUPLICATE KEY
                        PrintPlayerBaseAndErrorPath(playerBase, sb);
                    }
                }
                return Ok();
            }

            [HttpPost("sfbb_player_base/add_all")]
            public IActionResult AddAllSfbbPlayerBasesToDatabase(List<SfbbPlayerBase> allPlayerBases)
            {
                _helpers.StartMethod();
                int counter = 1;
                foreach(SfbbPlayerBase playerBase in allPlayerBases)
                {
                    var checkDbForBase = _context.SfbbPlayerBases.SingleOrDefault(pb => pb.IDPLAYER == playerBase.IDPLAYER);

                    if(checkDbForBase == null)
                    {
                        try
                        {
                            // Console.WriteLine($"TRY");
                            _context.SfbbPlayerBases.Attach(playerBase);
                            _context.Add(playerBase);
                        }
                        catch
                        {
                            // Console.WriteLine($"CATCH");
                            // Console.WriteLine($"playerBase: {playerBase.PLAYERNAME}");
                        }
                    }

                    else
                    {
                        // Console.WriteLine($"playerBase: {playerBase.PLAYERNAME}\t checkDbForBase: {checkDbForBase.PLAYERNAME}");
                        // Console.WriteLine($"ELSE");
                        // Console.WriteLine($"playerBase: {playerBase.PLAYERNAME}");
                        // Console.WriteLine($"{counter} | SFBB PLAYER BASE ALREADY EXISTS");
                    }
                    counter++;
                }
                // Console.WriteLine($"PRESS ANY KEY TO CONTINUE");
                // Console.ReadLine();
                _context.SaveChanges();
                return Ok();
            }



            /* ----- CRUD - READ - SFBB PLAYERBASE ----- */

            // STATUS [ August 19, 2019 ] : haven't tested
            public void GetOnePlayerBaseColumnFromGoogleSheet(string range)
            {
                var allPlayerBaseObjects = _googleSheetsConnector.ReadDataFromSheetRange(_sfbbMapDocName, _sfbbMapTabId, range);
                IEnumerable<SfbbPlayerBase> allPlayerBases = Enumerable.Empty<SfbbPlayerBase>();
            }


            // STATUS [ August 19, 2019 ] : haven't tested
            public SfbbPlayerBase GetOneFromDatabase(int mlbId)
            {
                SfbbPlayerBase sfbbPlayer = _context.SfbbPlayerBases.SingleOrDefault(s => s.MLBID == mlbId);
                return sfbbPlayer;
            }

        #endregion SFBB DATABASE ------------------------------------------------------------





        [Route("google_sheet")]
        public class PlayerBaseFromGoogleSheet : PlayerBaseController
        {

            new private static readonly string         _sfbbMapDocName = "SfbbPlayerIdMap";
            new private static readonly string         _sfbbMapTabId = "SFBB_PLAYER_ID_MAP";
            new private readonly Helpers               _helpers;
            new private readonly GoogleSheetsConnector _googleSheetsConnector;



            public PlayerBaseFromGoogleSheet(Helpers helpers, GoogleSheetsConnector googleSheetsConnector)
            {
                _helpers               = helpers;
                _googleSheetsConnector = googleSheetsConnector;
            }

            public PlayerBaseFromGoogleSheet(){}



            /* --------------------------------------------------------------- */
            /* ALL PLAYER BASES                                                */
            /* --------------------------------------------------------------- */

            // range example: "A5:AP2284"
            public IList<IList<object>> GetAllPlayerBaseObjectsFromGoogleSheet(string range)
            {
                _helpers.StartMethod();
                var allPlayerBases = _googleSheetsConnector.ReadDataFromSheetRange(_sfbbMapDocName, _sfbbMapTabId, range);
                var countOfAllPlayerBases = allPlayerBases.ToList().Count();
                // Console.WriteLine($"Current # of Players (sheets): {countOfAllPlayerBases}");
                return allPlayerBases;
            }


            // _sfbbMapDocName = "SfbbPlayerIdMap"
            // _sfbbMapTabId = "SFBB_PLAYER_ID_MAP"
            // public IEnumerable<SfbbPlayerBase> GetAllPlayerBasesFromGoogleSheet(string range)
            // {
            //     IList<IList<object>> allPlayerBaseObjects = _googleSheetsConnector.ReadDataFromSheetRange(
            //         _sfbbMapDocName,
            //         _sfbbMapTabId,
            //         range
            //     );

            //     PrintPlayerBaseObjectDetails(allPlayerBaseObjects, _sfbbMapDocName, _sfbbMapTabId, range);

            //     List<SfbbPlayerBase> allPlayerBases = new List<SfbbPlayerBase>();
            //     int counter = 1;
            //     int mlbIdCounter = 1;
            //     int hqIdCounter = 1;

            //     foreach(var row in allPlayerBaseObjects)
            //     {
            //         SfbbPlayerBase playerBase = new SfbbPlayerBase
            //         {
            //             IDPLAYER        = row[0].ToString(),
            //             PLAYERNAME      = row[1].ToString(),
            //             BIRTHDATE       = row[2].ToString(),
            //             FIRSTNAME       = row[3].ToString(),
            //             LASTNAME        = row[4].ToString(),
            //             TEAM            = row[5].ToString(),
            //             LG              = row[6].ToString(),
            //             POS             = row[7].ToString(),
            //             IDFANGRAPHS     = row[8].ToString(),
            //             FANGRAPHSNAME   = row[9].ToString(),
            //             // MLBID           = Int32.Parse(row[10].ToString()),
            //             // MLBID           = _csvHandler.ParseNullableInt(row[10].ToString()),
            //             MLBNAME         = row[11].ToString(),
            //             CBSID           = row[12].ToString(),
            //             CBSNAME         = row[13].ToString(),
            //             RETROID         = row[14].ToString(),
            //             BREFID          = row[15].ToString(),
            //             NFBCID          = row[16].ToString(),
            //             NFBCNAME        = row[17].ToString(),
            //             ESPNID          = row[18].ToString(),
            //             ESPNNAME        = row[19].ToString(),
            //             KFFLNAME        = row[20].ToString(),
            //             DAVENPORTID     = row[21].ToString(),
            //             BPID            = row[22].ToString(),
            //             YAHOOID         = row[23].ToString(),
            //             YAHOONAME       = row[24].ToString(),
            //             MSTRBLLNAME     = row[25].ToString(),
            //             BATS            = row[26].ToString(),
            //             THROWS          = row[27].ToString(),
            //             FANTPROSNAME     = row[28].ToString(),
            //             LASTCOMMAFIRST  = row[29].ToString(),
            //             ROTOWIREID      = row[30].ToString(),
            //             FANDUELNAME     = row[31].ToString(),
            //             FANDUELID       = row[32].ToString(),
            //             DRAFTKINGSNAME  = row[33].ToString(),
            //             OTTONEUID       = row[34].ToString(),
            //             // HQID            = Int32.Parse(row[35].ToString()),
            //             RAZZBALLNAME    = row[36].ToString(),
            //             FANTRAXID       = row[37].ToString(),
            //             FANTRAXNAME     = row[38].ToString(),
            //             ROTOWIRENAME    = row[39].ToString(),
            //             ALLPOS          = row[40].ToString(),
            //             NFBCLASTFIRST   = row[41].ToString()
            //         };

            //         if(row[10].ToString() == "")
            //         {
            //             // playerBase.MLBID = mlbIdCounter;
            //             mlbIdCounter++;
            //         }

            //         else
            //         {
            //             string mlbIdString = row[10].ToString();
            //             int mlbIdInt = Int32.Parse(mlbIdString);
            //             playerBase.MLBID = mlbIdInt;
            //         }

            //         if(row[35].ToString() == "")
            //         {
            //             // playerBase.HQID = hqIdCounter;
            //             hqIdCounter++;
            //         }

            //         else
            //         {
            //             string hqString = row[35].ToString();
            //             int hqIdInt = int.Parse(hqString);
            //             playerBase.HQID = hqIdInt;
            //         }

            //         allPlayerBases.Add(playerBase);
            //         // Console.WriteLine($"NEW: {playerBase.PLAYERNAME}\t MLB ID: {playerBase.MLBID}\t HQ ID: {playerBase.HQID}");

            //         // counter++;
            //     }
            //     return allPlayerBases;
            // }


            /* --------------------------------------------------------------- */
            /* ONE PLAYERS BASE                                                */
            /* --------------------------------------------------------------- */

            // public void GetOnePlayerBaseColumnFromGoogleSheet(string range)
            // {
            //     var allPlayerBaseObjects = _googleSheetsConnector.ReadDataFromSheetRange(_sfbbMapDocName, _sfbbMapTabId, range);
            //     IEnumerable<SfbbPlayerBase> allPlayerBases = Enumerable.Empty<SfbbPlayerBase>();
            // }


            // public SfbbPlayerBase GetOneFromDatabase(int mlbId)
            // {
            //     SfbbPlayerBase sfbbPlayer = _context.SfbbPlayerBases.SingleOrDefault(s => s.MLBID == mlbId);
            //     return sfbbPlayer;
            // }
        }




        [Route("excel")]
        public class PlayerBaseFromExcel : PlayerBaseController
        {
            // SEE:
            // * https://github.com/mganss/ExcelMappe

            new private readonly Helpers               _helpers;
            new private readonly GoogleSheetsConnector _googleSheetsConnector;

            public PlayerBaseFromExcel(Helpers helpers, GoogleSheetsConnector googleSheetsConnector)
            {
                _helpers               = helpers;
                _googleSheetsConnector = googleSheetsConnector;
            }

            public PlayerBaseFromExcel(){}


            /* --------------------------------------------------------------- */
            /* ALL PLAYER BASES                                                */
            /* --------------------------------------------------------------- */


            // STATUS: this works
            /// <summary>
            ///     Retrieves all records from PlayerBase.xlsx document
            /// </summary>
            /// <returns>
            ///     IEnumerable of PlayerBases
            /// </returns>
            public IEnumerable<PlayerBase> GetAllPlayerBasesFromExcel()
            {
                // Ganss.Excel.ExcelMapper+<Fetch>d__34`1[BaseballScraper.Models.PlayerBase]
                // IEnumerable<PlayerBase> allPlayerBases
                var allPlayerBases = new ExcelMapper("BaseballData/PlayerBase/PlayerBase.xlsx").Fetch<PlayerBase>();

                var countOfAllPlayerBases = allPlayerBases.ToList().Count();
                // Console.WriteLine($"Current # of Players: {countOfAllPlayerBases}");
                return allPlayerBases;
            }


            // STATUS: this works
            /// <summary>
            ///     Retrieves all records from PlayerBase.xlsx document for one team
            /// </summary>
            /// <param name="teamNameFull">
            ///     A full mlb team name (e.g., "Boston Red Sox"
            /// </param>
            /// <example>
            ///    var players = GetAllPlayerBasesForOneMlbTeam("Chicago Cubs");
            /// </example>
            /// <returns>
            ///     IEnumerable of PlayerBases
            /// </returns>
            public IEnumerable<PlayerBase> GetAllPlayerBasesForOneMlbTeam(string teamNameFull)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var playerBasesForOneMlbTeam  =
                    from playerBases in allPlayerBases
                    where playerBases.MlbTeamLong == teamNameFull
                    select playerBases;

                playerBasesForOneMlbTeam.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
                return playerBasesForOneMlbTeam;
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




            /* --------------------------------------------------------------- */
            /* ONE PLAYERS BASE                                                */
            /* --------------------------------------------------------------- */


            // STATUS: these all work
            // TODO: there has got to be a way where you only need one method vs. separating into each like below
            /// <summary>
            ///     Each of methods in this section returns a player (from IEnumerable PlayerBases)
            ///     The only difference is the type of Id you are passing in (e.g. MlbId, FanGraphsPlayerId, EspnPlayerId etc.)
            /// </summary>
            /// <returns>
            ///     IEnumerable of PlayerBases (i.e. a PlayerBase for one player)
            /// </returns>



            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromBaseballHqId(string playersBaseballHqPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.BaseballHqPlayerId == playersBaseballHqPlayerId
                    select playerBases;

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromMlbId(string playersMlbId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                // IEnumerable<PlayerBase> onePlayersBase
                var onePlayersBase = from playerBases in allPlayerBases
                    where playerBases.MlbId == playersMlbId
                    select playerBases;

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
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

                onePlayersBase.ToList().ForEach((playerBase) => Console.WriteLine(playerBase.CbsName));
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromYahooName(string playersYahooName)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where playerBases.YahooName == playersYahooName
                    select playerBases;

                return onePlayersBase;
            }
        }



        #region ALL: GENERATE PLAYER BASE(S) ------------------------------------------------------------

            public class PlayerBaseGenerator
            {
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

                        listOfPlayersIds.ForEach((id) => Console.WriteLine($"id: {id}"));

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
                            { "MlbId"                      , playersBase.MlbId                      },
                            { "SfbbPlayerId"               , playersBase.SfbbPlayerId               },
                            { "BaseballHqPlayerId"         , playersBase.BaseballHqPlayerId         },
                            { "DavenportId"                , playersBase.DavenportId                },
                            { "BaseballProspectusPlayerId" , playersBase.BaseballProspectusPlayerId },
                            { "BaseballReferencePlayerId"  , playersBase.BaseballReferencePlayerId  },
                            { "CbsPlayerId"                , playersBase.CbsPlayerId                },
                            { "EspnPlayerId"               , playersBase.EspnPlayerId               },
                            { "FanGraphsPlayerId"          , playersBase.FanGraphsPlayerId          },
                            { "LahmanPlayerId"             , playersBase.LahmanPlayerId             },
                            { "NfbcPlayerId"               , playersBase.NfbcPlayerId               },
                            { "RetroPlayerId"              , playersBase.RetroPlayerId              },
                            { "YahooPlayerId"              , playersBase.YahooPlayerId              },
                            { "OttoneuPlayerId"            , playersBase.OttoneuPlayerId            },
                            { "RotoWirePlayerId"           , playersBase.RotoWirePlayerId           }
                        };

                        foreach (var kvp in dictionaryOfPlayersIds)
                            {
                                Console.WriteLine($"{kvp.Key} -->  {kvp.Value}");
                            }
                        return dictionaryOfPlayersIds;
                    }


                    // // NOTE: for testing purposes only; tests all of the other methods in this section
                    // public void TestAll()
                    // {
                    //     string mlbId = "595918";
                    //     string sfbbId = "coleaj01";
                    //     string baseballHqId = "4545";
                    //     string davenportId = "COLE19920105A";
                    //     string baseballProspectusId = "68086";
                    //     string baseballReferenceId = "coleaj01";
                    //     string cbsId = "1839916";
                    //     string espnId = "31595";
                    //     string fanGraphsId = "11467";
                    //     string lahmanId = "coleaj01";
                    //     string nfbcId = "9638";
                    //     string retroId = "colea002";
                    //     string yahooId = "9638";
                    //     string ottoneuId = "14940";
                    //     string rotoWireId = "11446";

                    //     // var playersBase = GetOnePlayersBaseFromMlbId(playerIdToQuery);
                    //     GetOnePlayersBaseFromMlbId(mlbId);
                    //     GetOnePlayersBaseFromSfbbId(sfbbId);
                    //     GetOnePlayersBaseFromBaseballHqId(baseballHqId);
                    //     GetOnePlayersBaseFromDavenportId(davenportId);
                    //     GetOnePlayersBaseFromBaseballProspectusId(baseballProspectusId);
                    //     GetOnePlayersBaseFromBaseballReferenceId(baseballReferenceId);
                    //     GetOnePlayersBaseFromCbsId(cbsId);
                    //     GetOnePlayersBaseFromEspnId(espnId);
                    //     GetOnePlayersBaseFromFanGraphsId(fanGraphsId);
                    //     GetOnePlayersBaseFromLahmanId(lahmanId);
                    //     GetOnePlayersBaseFromNfbcId(nfbcId);
                    //     GetOnePlayersBaseFromRetroId(retroId);
                    //     GetOnePlayersBaseFromYahooId(yahooId);
                    //     GetOnePlayersBaseFromOttoneuId(ottoneuId);
                    //     GetOnePlayersBaseFromRotoWireId(rotoWireId);
                    // }


                    // NOTE: not sure either of these are actually needed; but good practice regardless
                    // public void GetPlayerBaseProperties()
                    // {
                    //     PropertyInfo[] propertyInfos = typeof(PlayerBase).GetProperties();

                    //     Array.Sort(propertyInfos,
                    //         delegate(PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
                    //         { return propertyInfo1.Name.CompareTo(propertyInfo2.Name); });

                    //     foreach(PropertyInfo propertyInfo in propertyInfos)
                    //     {
                    //         Console.WriteLine(propertyInfo.Name);
                    //     }
                    // }

            }


        #endregion ALL: GENERATE PLAYER BASE(S) ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            private void PrintPlayerBaseAndErrorPath(SfbbPlayerBase playerBase, StringBuilder sb)
            {
                Console.WriteLine($"\n---------------------------------------------");
                Console.WriteLine(playerBase.PLAYERNAME);
                Console.WriteLine($"{sb}");
                Console.WriteLine($"---------------------------------------------\n");
            }


            public void PrintPlayerBaseObjectDetails(IList<IList<object>> allPlayerBaseObjects, string docName, string tabName, string range)
            {
                Console.WriteLine($"\n---------------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(PlayerBaseController));
                Console.WriteLine($"# OF PLAYERS  : {allPlayerBaseObjects.Count}");
                Console.WriteLine($"DOCUMENT NAME : {docName}");
                Console.WriteLine($"TAB NAME      : {tabName}");
                Console.WriteLine($"SHEET RANGE   : {range}");
                Console.WriteLine($"---------------------------------------------------\n");
            }


            private void PrintCrunchTimeCsvDownloadInfo(string todayString)
            {
                Console.WriteLine($"\n-------------------------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(PlayerBaseController));

                bool fileNowExists = false;
                string fullFilePathAndName = $"{CrunchTimeWriteFolder}{CrunchTimeReportFileBaseName}_{todayString}.csv";
                if(System.IO.File.Exists(fullFilePathAndName))
                {
                    fileNowExists = true;
                }
                Console.WriteLine($"DOWNLOADING FROM : {CrunchTimeCsvSourceUrl}");
                Console.WriteLine($"DOWNLOADING TO   : {CrunchTimeWriteFolder}");
                Console.WriteLine($"NEW FILE NAME    : {CrunchTimeReportFileBaseName}_{todayString}.csv");
                Console.WriteLine($"FILE EXISTS?     : {fileNowExists}");
                Console.WriteLine($"-------------------------------------------------------------\n");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------



    }
}
