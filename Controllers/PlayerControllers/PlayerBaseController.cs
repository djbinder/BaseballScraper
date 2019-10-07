using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.Player;
using Ganss.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using C = System.Console;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Globalization;

#pragma warning disable CC0061, CS0168, CS0219, CS0414, CS1998, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006, MA0002, MA0016, MA0051
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
        private readonly GoogleSheetsConnector     _googleSheetsConnector;
        private readonly ProjectDirectoryEndPoints _baseballData;

        private System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();

        private readonly string _sfbbMapDocName   = "SfbbPlayerIdMap";
        private readonly string _sfbbMapTabId     = "SFBB_PLAYER_ID_MAP";
        private readonly string _googleSheetRange = "A7:AQ2333";
        private readonly string _googleSheetErrorRange = "A2030:AQ2050";


        private readonly Dictionary<string, object> _d = new Dictionary<string, object>(StringComparer.Ordinal);

        public PlayerBaseController(Helpers helpers, BaseballScraperContext context, CsvHandler csvHandler, GoogleSheetsConnector googleSheetsConnector, ProjectDirectoryEndPoints baseballData)
        {
            _helpers                   = helpers;
            _context                   = context;
            _csvHandler                = csvHandler;
            _googleSheetsConnector     = googleSheetsConnector;
            _baseballData              = baseballData;
        }


        public PlayerBaseController() {}


        // https://127.0.0.1:5001/api/player/playerbase/test
        [Route("test")]
        public void TestController()
        {
        }

        /*
            https://127.0.0.1:5001/api/player/playerbase/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
            var playerBases = GetAllToday_CSV();
            await AddAll_DB(playerBases);
        }



        /* -------------------- CONTROLLER OVERVIEW -------------------- */
        //
        // NOTES [ August 13, 2019 ]:
        // *
        // RESOURCES
        // * CrunchTime Map : http://crunchtimebaseball.com/baseball_map.html
        // * CrunchTime CSV : http://crunchtimebaseball.com/master.csv
        // * SFBB Player Id Map : http://bit.ly/2UdNAGy
        //
        // TO DO
        // *




        // range is something like: "A7:AQ2333"
        public async Task<IActionResult> DAILY_REPORT_RUNNER(string range)
        {
            _helpers.OpenMethod(3);


            // List<SfbbPlayerBase>
            var sfbbPlayerBasesList = GetAll_GSheet(_googleSheetRange);
            await AddAllAsync_DB(sfbbPlayerBasesList);


            // List<CrunchTimePlayerBase>
            var crunchTimePlayerBases = GetAllToday_CSV();
            await AddAll_DB(crunchTimePlayerBases);

            return Ok();
        }


        #region CRUNCHTIME ------------------------------------------------------------


        /* --------------------------------------------------------------- */
        /* CRUNCH TIME - BUILDING BLOCKS                                   */
        /* --------------------------------------------------------------- */


            // * = "BaseballData/02_WRITE/PlayerBase/CRUNCH_TIME/"
            // * Defined by me in: ProjectDirectoryEndPoints
            private string CrunchTimeWriteFolder
            {
               get => _baseballData.CrunchTimeWriteDirectoryRelativePath;
            }


            // * = "CrunchTime_Csv"
            // * Defined by me in: ProjectDirectoryEndPoints
            private string CrunchTimeReportFileBaseName
            {
                get => _baseballData.CrunchTimeReportFileBaseName;
            }


            // * = "http://crunchtimebaseball.com/master.csv"
            // * Defined by Crunch Time; Pulled from ProjectDirectoryEndPoints
            public string CrunchTimeCsvSourceUrl
            {
                get => _baseballData.CrunchTimePlayerBaseCsvSourceUrl;
            }



        /* --------------------------------------------------------------- */
        /* CRUNCH TIME - CSV                                               */
        /* --------------------------------------------------------------- */


            // STATUS [ August 7, 2019 ] : this works
            // CRUNCH TIME PLAYER BASE
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
            public void DownloadTodaysCrunchTimeCSV()
            {
                _helpers.OpenMethod(1);
                string todayString       = _csvHandler.TodaysDateString();
                string locationToWriteTo = $"{CrunchTimeWriteFolder}{CrunchTimeReportFileBaseName}_{todayString}.csv";
                bool doesFileExist       = _csvHandler.CheckIfFileExists(locationToWriteTo);

                if(!doesFileExist)
                {
                    _csvHandler.DownloadCsvFromLink(
                        CrunchTimeCsvSourceUrl,
                        locationToWriteTo
                    );
                }
                PrintCrunchTimeCsvDownloadInfo(todayString);
                _helpers.CloseMethod(1);
            }


            // CRUNCH TIME PLAYER BASE
            public List<CrunchTimePlayerBase> GetAllToday_CSV()
            {
                _helpers.OpenMethod(1);

                DownloadTodaysCrunchTimeCSV();

                string todayString         = _csvHandler.TodaysDateString();
                string fullFilePathAndName = $"{CrunchTimeWriteFolder}{CrunchTimeReportFileBaseName}_{todayString}.csv";

                IList<object> listOfObjects = _csvHandler.ReadCsvRecordsToList(
                    fullFilePathAndName,
                    typeof(CrunchTimePlayerBase),
                    typeof(CrunchTimePlayerBaseClassMap)
                );

                List<CrunchTimePlayerBase> crunchTimePlayerBases = new List<CrunchTimePlayerBase>();

                foreach(object csvRow in listOfObjects)
                {
                    CrunchTimePlayerBase crunchTimePlayerBase = csvRow as CrunchTimePlayerBase;
                    crunchTimePlayerBases.Add(crunchTimePlayerBase);
                }
                _helpers.CloseMethod(1);
                return crunchTimePlayerBases;
            }



            public List<CrunchTimePlayerBase> GetAllToday_CSV(string filePath)
            {
                _helpers.OpenMethod(1);

                IList<object> listOfObjects = _csvHandler.ReadCsvRecordsToList(
                    csvFilePath     : filePath,
                    modelType       : typeof(CrunchTimePlayerBase),
                    modelMapType    : typeof(CrunchTimePlayerBaseClassMap)
                );

                List<CrunchTimePlayerBase> crunchTimePlayerBases = new List<CrunchTimePlayerBase>();

                foreach(object csvRow in listOfObjects)
                {
                    CrunchTimePlayerBase crunchTimePlayerBase = csvRow as CrunchTimePlayerBase;
                    crunchTimePlayerBases.Add(crunchTimePlayerBase);
                }
                _helpers.CloseMethod(1);
                return crunchTimePlayerBases;
            }


        /* --------------------------------------------------------------- */
        /* CRUD - CRUNCH TIME PLAYERBASE                                   */
        /* --------------------------------------------------------------- */

        /* ----- CRUD - CREATE - CRUNCH TIME PLAYERBASE ----- */

            // STATUS [ August 27, 2019 ] : haven't tested but should work
            // CRUNCH TIME PLAYER BASE
            public async Task<ActionResult> AddOne_DB(CrunchTimePlayerBase crunchTimePlayerBase)
            {
                // _helpers.OpenMethod(3);
                _context.CrunchTimePlayerBases.Attach(crunchTimePlayerBase);
                await _context.AddAsync(crunchTimePlayerBase, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Ok();
            }


            // STATUS [ August 27, 2019 ] : this works
            // CRUNCH TIME PLAYER BASE
            public async Task<ActionResult> AddAll_DB(List<CrunchTimePlayerBase> crunchTimePlayerBases)
            {
                _helpers.OpenMethod(3);

                int playersAddedCounter = 0; int playersExistCounter = 0;

                foreach(CrunchTimePlayerBase playerBase in crunchTimePlayerBases)
                {
                    var checkDbForPlayerBase =_context.CrunchTimePlayerBases.SingleOrDefault(ct => ct.MlbId == playerBase.MlbId);

                    if(checkDbForPlayerBase == null)
                    {
                        _context.CrunchTimePlayerBases.Attach(playerBase);
                        await _context.AddAsync(playerBase, cancellationToken);
                        playersAddedCounter++;
                    }

                    else
                    {
                        playersExistCounter++;
                    }
                }
                await _context.SaveChangesAsync(cancellationToken);

                _context.PrintDatabaseAddOutcomes(
                    playersAddedCounter,
                    playersExistCounter,
                    typeof(PlayerBaseController)
                );

                return Ok();
            }


        /* ----- CRUD - READ - CRUNCH TIME PLAYERBASE ----- */

            // STATUS [ August 27, 2019 ] : haven't tested but should work
            // CRUNCH TIME PLAYER BASE
            public async Task<ActionResult> GetOne_DB(int mlbId)
            {
                _helpers.OpenMethod(3);
                var player = _context.CrunchTimePlayerBases.SingleOrDefault(p => p.MlbId == mlbId);
                return Ok(player);
            }


        /* ----- CRUD - DELETE - CRUNCH TIME PLAYERBASE ----- */

            // STATUS [ August 27, 2019 ] : this works
            // CRUNCH TIME PLAYER BASE
            public ActionResult DeleteAll_DB()
            {
                _helpers.OpenMethod(1);
                _context.CrunchTimePlayerBases.RemoveRange(_context.CrunchTimePlayerBases);
                _context.SaveChanges();
                return Ok();
            }


        #endregion CRUNCHTIME ------------------------------------------------------------





        #region SFBB ------------------------------------------------------------

        /* --------------------------------------------------------------- */
        /* SFBB - GOOGLE SHEET                                             */
        /* --------------------------------------------------------------- */

            // SFBB PLAYER BASE
            public IList<IList<object>> GetAll_GSheet()
            {
                _helpers.StartMethod();

                var allPlayerBases = _googleSheetsConnector.ReadDataFromSheetRange(
                    _sfbbMapDocName,    // "SfbbPlayerIdMap"
                    _sfbbMapTabId,      // "SFBB_PLAYER_ID_MAP"
                    _googleSheetRange   // "A7:AQ2333"
                );
                return allPlayerBases;
            }



            // SFBB PLAYER BASE
            public List<SfbbPlayerBase> GetAll_GSheet(string range)
            {
                _helpers.OpenMethod(1);
                IList<IList<object>> allPlayerBaseObjects = _googleSheetsConnector.ReadDataFromSheetRange(
                    _sfbbMapDocName,    // "SfbbPlayerIdMap"
                    _sfbbMapTabId,      // "SFBB_PLAYER_ID_MAP"
                    range               // "A7:AQ2333"
                );

                PrintPlayerBaseObjectDetails(allPlayerBaseObjects, _sfbbMapDocName, _sfbbMapTabId, range);

                List<SfbbPlayerBase> allPlayerBases = new List<SfbbPlayerBase>();

                foreach(IList<object> row in allPlayerBaseObjects)
                {
                    SfbbPlayerBase playerBase = InstantiateSfbbPlayerBase(row);
                    allPlayerBases.Add(playerBase);
                }

                var filteredList = FilterOutDuplicateRecords(allPlayerBases);

                _helpers.CloseMethod(1);
                return filteredList;
            }


            // *  The source Sfbb Player Base sheet (that I do not own) has one guy in there twice
            // * This filters out any player listed twice
            public List<SfbbPlayerBase> FilterOutDuplicateRecords(List<SfbbPlayerBase> allPlayerBases)
            {
                var filteredList = new List<SfbbPlayerBase>();
                List<string> idPlayers = new List<string>();

                foreach(var p in allPlayerBases)
                {
                    if(idPlayers.Contains(p.IDPLAYER)){}
                    else
                    {
                        filteredList.Add(p);
                        idPlayers.Add(p.IDPLAYER);
                    }
                }

                C.WriteLine($"OLD COUNT: {allPlayerBases.Count}");
                C.WriteLine($"NEW COUNT: {filteredList.Count}");
                return filteredList;
            }



        /* --------------------------------------------------------------- */
        /* CRUD - SFBB PLAYERBASE                                          */
        /* --------------------------------------------------------------- */


        /* ----- CRUD - CREATE - SFBB PLAYERBASE ----- */


            // STATUS [ August 19, 2019 ] : this works
            // SFBB PLAYER BASE
            // POST ONE SfbbplayerBase Option 2
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
            public async Task<ActionResult> AddOneAsync_DB(SfbbPlayerBase playerBase)
            {
                // _helpers.OpenMethod(3);
                StringBuilder sb = new StringBuilder();
                try
                {
                    sb.Append('A'); // A --> CHECK IF PLAYER EXISTS IN DATABASE
                    SfbbPlayerBase pBase = _context.SfbbPlayerBases.SingleOrDefault(y => y.IDPLAYER == playerBase.IDPLAYER);

                    if(pBase != null)
                    {
                        // C.WriteLine($"SFBB PLAYER BASE EXISTS: {pBase.PLAYERNAME}");
                        sb.Append(" --> B"); // B --> ALREADY IN DB
                    }

                    else
                    {
                        sb.Append(" --> C"); // C --> ADD TO DATABASE
                        await _context.AddAsync(playerBase, cancellationToken);
                    }
                }
                catch(Exception ex)
                {
                    sb.Append(" --> D"); // D --> NOT IN DATABASE
                    try
                    {
                        sb.Append(" --> E"); // E --> ADD TO DB
                        await _context.AddAsync(playerBase, cancellationToken);
                        // await _context.SaveChangesAsync(cancellationToken);
                    }

                    catch(Exception ex2)
                    {
                        sb.Append(" --> F"); // F --> NOT ADDED TO DATABASE - DUPLICATE KEY
                        PrintPlayerBaseAndErrorPath(playerBase, sb);
                    }
                }
                // await _context.SaveChangesAsync(cancellationToken);
                return Ok();
            }


        // STATUS [ August 19, 2019 ] : this works but needs updates
        // SFBB PLAYER BASE
        // POST ALL SfbbplayerBase Option 1
        // * Pull range out as a parameter
        // * Define list outside of method
        /// <summary>
        ///     * Add list of playerBases
        ///     * AddAll_DB Option 1 for SfbbPlayerBases
        /// </summary>
        /// <param name="allPlayerBases">todo: describe allPlayerBases parameter on AddAllAsync_DB</param>
        public async Task<ActionResult> AddAllAsync_DB(List<SfbbPlayerBase> allPlayerBases)
            {
                _helpers.OpenMethod(3);

                int playersAddedCounter = 0; int playersExistCounter = 0; int counter = 1;

                foreach(var p in allPlayerBases)
                {
                    var checkDbForBase =_context.SfbbPlayerBases.SingleOrDefault(pb => pb.IDPLAYER == p.IDPLAYER);

                    if(checkDbForBase == null)
                    {
                        _context.Entry(p).State = EntityState.Added;
                        await _context.Set<SfbbPlayerBase>().AddAsync(p, cancellationToken);
                    }
                    else
                    {
                        _context.Entry(checkDbForBase).State = EntityState.Unchanged;
                    }

                    if(counter % 500 == 0 )
                    {
                        C.WriteLine(counter);
                    }

                    int manageCounters = (checkDbForBase == null) ? playersAddedCounter++ : playersExistCounter++;
                    counter++;
                }

                await _context.SaveChangesAsync(cancellationToken);
                _context.PrintDatabaseAddOutcomes(playersAddedCounter, playersExistCounter, typeof(PlayerBaseController));
                return Ok();
            }


            // STATUS [ September 6, 2019 ] : not sure if this works; haven't tested
            public EntityEntry<SfbbPlayerBase> AddOne_DB(SfbbPlayerBase s) => _context.Add(s);


            // STATUS [ September 6, 2019 ] : not sure if this works; haven't tested
            public void AddAllList_DB (List<SfbbPlayerBase> allPlayerBases) => allPlayerBases.ForEach(pb => _context.Add(pb));


            // STATUS [ September 6, 2019 ] : not sure if this works; haven't tested
            public void AddRange_DB (List<SfbbPlayerBase> allPlayerBases) => _context.SfbbPlayerBases.AddRange(allPlayerBases);



            // TO DO : https://blog.zhaytam.com/2019/03/14/generic-repository-pattern-csharp/
            // TO DO : https://stackoverflow.com/questions/39656794/entity-framework-update-insert-multiple-entities



            // _context.Set<SfbbPlayerBase>().AddRange(someOfThePlayerBases);
            // someOfThePlayerBases = new List<SfbbPlayerBase>();
            // _context.ChangeTracker.DetectChanges();
            // _context.SaveChanges();
            // _context?.Dispose();
            // nullCheck      = (checkDbForPlayer == null) ? _context.Add(pb) : null;





        /* ----- CRUD - READ - SFBB PLAYERBASE ----- */


            // SFBB PLAYER BASE
            public SfbbPlayerBase InstantiateSfbbPlayerBase(IList<object> row)
            {
                const int counter = 1; int mlbIdCounter = 1; int hqIdCounter = 1;

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
                    FANTPROSNAME    = row[28].ToString(),
                    LASTCOMMAFIRST  = row[29].ToString(),
                    ROTOWIREID      = row[30].ToString(),
                    FANDUELNAME     = row[31].ToString(),
                    FANDUELID       = row[32].ToString(),
                    DRAFTKINGSNAME  = row[33].ToString(),
                    OTTONEUID       = row[34].ToString(),
                    RAZZBALLNAME    = row[36].ToString(),
                    FANTRAXID       = row[37].ToString(),
                    FANTRAXNAME     = row[38].ToString(),
                    ROTOWIRENAME    = row[39].ToString(),
                    ALLPOS          = row[40].ToString(),
                    NFBCLASTFIRST   = row[41].ToString(),
                };

                // if(string.Equals(row[10].ToString(), "", StringComparison.Ordinal))
                if(!string.IsNullOrEmpty(row[10].ToString()))
                {
                    mlbIdCounter++;
                }

                else
                {
                    string mlbIdString = row[10].ToString();
                    int mlbIdInt = int.Parse(mlbIdString, NumberStyles.None, CultureInfo.InvariantCulture);
                    playerBase.MLBID = mlbIdInt;
                }

                // if(string.Equals(row[35].ToString(), "", StringComparison.Ordinal))
                if(!string.IsNullOrEmpty(row[35].ToString()))
                {
                    hqIdCounter++;
                }

                else
                {
                    string hqString = row[35].ToString();
                    // int hqIdInt = int.Parse(hqString, NumberStyles.None, CultureInfo.InvariantCulture);
                    int hqIdInt = int.Parse(hqString);
                    playerBase.HQID = hqIdInt;
                }

                return playerBase;
            }


        /* --------------------------------------------------------------- */
        /* SFBB - HELPERS                                                  */
        /* --------------------------------------------------------------- */


            // // SFBB PLAYER BASE
            // public int NumberOfSfbbPlayerBasesInDatabase()
            // {
            //     int playerBaseCount = _context.SfbbPlayerBases.Count();
            //     return playerBaseCount;
            // }


            // // SFBB PLAYER BASE
            // public int NumberOfSfbbPlayerBasesInGoogleSheet()
            // {
            //     int playerBaseCount = GetAll_GSheet(_googleSheetRange).Count();
            //     return playerBaseCount;
            // }


        #endregion SFBB DATABASE ------------------------------------------------------------







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

                var countOfAllPlayerBases = allPlayerBases.ToList().Count;
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
                    where string.Equals(playerBases.MlbTeamLong, teamNameFull
, StringComparison.Ordinal)
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
            // TO-DO: there has got to be a way where you only need one method vs. separating into each like below
            /// <summary>
            ///     Each of methods in this section returns a player (from IEnumerable PlayerBases)
            ///     The only difference is the type of Id you are passing in (e.g. MlbId, FanGraphsPlayerId, EspnPlayerId etc.)
            /// </summary>
            /// <param name="playersBaseballHqPlayerId">todo: describe playersBaseballHqPlayerId parameter on GetOnePlayersBaseFromBaseballHqId</param>
            /// <returns>
            ///     IEnumerable of PlayerBases (i.e. a PlayerBase for one player)
            /// </returns>



            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromBaseballHqId(string playersBaseballHqPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where string.Equals(playerBases.BaseballHqPlayerId, playersBaseballHqPlayerId
, StringComparison.Ordinal)
                    select playerBases;

                onePlayersBase.ToList().ForEach((playerBase) => C.WriteLine(playerBase.CbsName));
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromBaseballProspectusId(string playersBaseballProspectusPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where string.Equals(playerBases.BaseballProspectusPlayerId, playersBaseballProspectusPlayerId
, StringComparison.Ordinal)
                    select playerBases;

                onePlayersBase.ToList().ForEach((playerBase) => C.WriteLine(playerBase.CbsName));
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromBaseballReferenceId(string playersBaseballReferencePlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where string.Equals(playerBases.BaseballReferencePlayerId, playersBaseballReferencePlayerId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.CbsPlayerId, playersCbsPlayerId
, StringComparison.Ordinal)
                    select playerBases;

                onePlayersBase.ToList().ForEach((playerBase) => C.WriteLine(playerBase.CbsName));
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromDavenportId(string playersDavenportPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where string.Equals(playerBases.DavenportId, playersDavenportPlayerId
, StringComparison.Ordinal)
                    select playerBases;

                onePlayersBase.ToList().ForEach((playerBase) => C.WriteLine(playerBase.CbsName));
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromEspnId(string playersEspnPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where string.Equals(playerBases.EspnPlayerId, playersEspnPlayerId
, StringComparison.Ordinal)
                    select playerBases;

                onePlayersBase.ToList().ForEach((playerBase) => C.WriteLine(playerBase.CbsName));
                return onePlayersBase;
            }


            // NOTE: see XML comments at beginning of region
            public IEnumerable<PlayerBase> GetOnePlayersBaseFromFanGraphsId(string playersFanGraphsPlayerId)
            {
                var allPlayerBases = GetAllPlayerBasesFromExcel();

                var onePlayersBase =
                    from playerBases in allPlayerBases
                    where string.Equals(playerBases.FanGraphsPlayerId, playersFanGraphsPlayerId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.LahmanPlayerId, playersLahmanPlayerId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.MlbId, playersMlbId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.NfbcPlayerId, playersNfbcPlayerId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.OttoneuPlayerId, playersOttoneuPlayerId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.RetroPlayerId, playersRetroPlayerId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.RotoWirePlayerId, playersRotoWirePlayerId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.SfbbPlayerId, playersSfbbPlayerId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.YahooPlayerId, playersYahooPlayerId
, StringComparison.Ordinal)
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
                    where string.Equals(playerBases.YahooName, playersYahooName
, StringComparison.Ordinal)
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
                            playersBase.RotoWirePlayerId,
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
                        (StringComparer.Ordinal)
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
                            { "RotoWirePlayerId"           , playersBase.RotoWirePlayerId           },
                        };

                        foreach (var kvp in dictionaryOfPlayersIds)
                            {
                                C.WriteLine($"{kvp.Key} -->  {kvp.Value}");
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
                C.WriteLine($"\n---------------------------------------------");
                C.WriteLine(playerBase.PLAYERNAME);
                C.WriteLine($"{sb}");
                C.WriteLine($"---------------------------------------------\n");
            }


            public void PrintPlayerBaseObjectDetails(IList<IList<object>> allPlayerBaseObjects, string docName, string tabName, string range)
            {
                C.WriteLine($"\n---------------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(PlayerBaseController));
                C.WriteLine($"# OF PLAYERS  : {allPlayerBaseObjects.Count}");
                C.WriteLine($"DOCUMENT NAME : {docName}");
                C.WriteLine($"TAB NAME      : {tabName}");
                C.WriteLine($"SHEET RANGE   : {range}");
                C.WriteLine($"---------------------------------------------------\n");
            }


            private void PrintCrunchTimeCsvDownloadInfo(string todayString)
            {
                C.WriteLine($"\n-------------------------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(PlayerBaseController));

                bool fileNowExists = false;
                string fullFilePathAndName = $"{CrunchTimeWriteFolder}{CrunchTimeReportFileBaseName}_{todayString}.csv";
                if(System.IO.File.Exists(fullFilePathAndName))
                {
                    fileNowExists = true;
                }
                C.WriteLine($"DOWNLOADING FROM : {CrunchTimeCsvSourceUrl}");
                C.WriteLine($"DOWNLOADING TO   : {CrunchTimeWriteFolder}");
                C.WriteLine($"NEW FILE NAME    : {CrunchTimeReportFileBaseName}_{todayString}.csv");
                C.WriteLine($"FILE EXISTS?     : {fileNowExists}");
                C.WriteLine($"-------------------------------------------------------------\n");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}