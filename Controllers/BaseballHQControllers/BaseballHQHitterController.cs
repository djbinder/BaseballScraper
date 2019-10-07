using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.BaseballHq;
using BaseballScraper.Models.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using C = System.Console;

#pragma warning disable CC0061, CC0091, CS1998, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006, MA0016
namespace BaseballScraper.Controllers.BaseballHQControllers
{
    [Route("api/hq/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BaseballHqHitterController : ControllerBase
    {
        private readonly Helpers                       _helpers;
        private readonly CsvHandler                    _csvHandler;
        private readonly BaseballHqUtilitiesController _hqUtilitiesController;
        private readonly BaseballScraperContext        _context;
        private readonly ProjectDirectoryEndPoints     _projectDirectory;

        private System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();


        public BaseballHqHitterController(Helpers helpers, BaseballHqUtilitiesController hqUtilitiesController, CsvHandler csvHandler, BaseballScraperContext context, ProjectDirectoryEndPoints projectDirectory)
        {
            _helpers               = helpers;
            _hqUtilitiesController = hqUtilitiesController;
            _csvHandler            = csvHandler;
            _context               = context;
            _projectDirectory      = projectDirectory;
        }

        public BaseballHqHitterController() {}


        // * Defined by me
        // * = "HqHitterReport_"
        private string BaseballHqHitterReportPrefix
        {
            get => _projectDirectory.BaseballHqHitterReportPrefix;
        }


        // * Defined by me in ProjectDirectoryEndPoints
        // * = "BaseballData/02_WRITE/BASEBALL_HQ/HITTERS/"
        private string BaseballHqHitterWriteDirectory
        {
            get => _projectDirectory.BaseballHqHitterWriteRelativePath;
        }


        // * Defined by me in ProjectDirectoryEndPoints
        // * = "BaseballData/02_WRITE/BASEBALL_HQ/_archive/"
        private string BaseballHqArchiveDirectory
        {
            get => _projectDirectory.BaseballHqArchiveRelativePath;
        }



        /*
            https://127.0.0.1:5001/api/hq/baseballhqhitter/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
        }


        /*
            https://127.0.0.1:5001/api/hq/baseballhqhitter/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
        }


        /* -------------------- CONTROLLER OVERVIEW -------------------- */
        //
        // NOTES [ August 13, 2019 ]:
        // * Everything below uses a mixtures of this controller, hq utilities controller and csv handler
        // * UpdateBothHqHitterDatabases() is the primary method. It contains two other methods:
        // *    1) For Rest of Season Projections -> UpdateDatabaseWithTodaysHitterRestOfSeasonProjectionsData()
        // *    2) For Year To Date Performance -> UpdateDatabaseWithTodaysHitterYearToDateData();
        // * Should be run daily to ensure latest and greatest data
        // * Called by MasterHitterController (as of August 13, 2019)
        //
        // TO DO
        // * Somewhat slow; Look for opportunities to improve speed
        // * Convert comments to XML comments so Swagger can read
        // * Archive Csv files once data from csv is added to database (i.e., daily)
        // * * Use '_hqHitterTargetWriteArchiveFolderPath' path to move file to
        //
        // SWAGGER
        // * A header of [ApiExplorerSettings(IgnoreApi = true)] hides method from swagger documentation
        // * See: https://bit.ly/2KyPY7p






        #region ROS PROJECTIONS AND YEAR TO DATE ------------------------------------------------------------


            public async Task DAILY_REPORT_RUNNER(bool openRosFileAfterMove, bool openYtdFileAfterMove)
            {
                _helpers.OpenMethod(3);

                // * PRIMARY METHOD - REST OF SEASON PROJECTIONS
                List<HqHitterRestOfSeasonProjection> rosList =
                    await Task.Run(() => GetAllRosAsync_CSV(openRosFileAfterMove));
                await AddAllRosAsync_DB(rosList);

                // * PRIMARY METHOD - YEAR TO DATE
                List<HqHitterYearToDate> ytdList =
                    await Task.Run(() => GetAllYtdAsync_CSV(openYtdFileAfterMove));
                await AddAllYtdAsync_DB(ytdList);

                // await UpdateBothHqHitterDatabases(
                //     openRosFileAfterMove,
                //     openYtdFileAfterMove
                // );
            }



            // // STATUS [ August 13, 2019 ] : this works
            // // * PRIMARY METHOD FOR EVERYTHING BELOW
            // /// <summary>
            // ///     * Gets all hitter reports from hq and adds them to database
            // ///     * Updates the two main bases with day from today
            // /// </summary>
            // /// <remarks>
            // ///     * SEE: MasterHitterController (as of August 13, 2019)
            // /// </remarks>
            // [HttpPost("update_all")]
            // public async Task<ActionResult> UpdateBothHqHitterDatabases(bool openRosFileAfterMove, bool openYtdFileAfterMove)
            // {
            //     _helpers.OpenMethod(3);

            //     // * PRIMARY METHOD - REST OF SEASON PROJECTIONS
            //     List<HqHitterRestOfSeasonProjection> rosList =
            //         Task.Run(() => GetAllAsyncRos_CSV(openRosFileAfterMove)).Result;
            //     await AddAllAsync_DB(rosList);

            //     // * PRIMARY METHOD - YEAR TO DATE
            //     List<HqHitterYearToDate> ytdList =
            //         Task.Run(() => GetAllAsyncYtd_CSV(openYtdFileAfterMove)).Result;
            //     await AddAllYtdAsync_DB(ytdList);

            //     return Ok();
            // }


        #endregion ROS PROJECTIONS AND YEAR TO DATE ------------------------------------------------------------





        #region HQ HITTERS ROS PROJECTIONS  ------------------------------------------------------------


            /* --------------------------------------------------------------- */
            /* BUILDING BLOCKS - REST OF SEASON PROJECTIONS                    */
            /* --------------------------------------------------------------- */

            // * Defined by Baseball HQ
            // * Can be found in the html of the hq page
            private readonly string _hitterRestOfSeasonProjectionsReportSelector = "#node-384 > div > table:nth-child(5) > tbody > tr:nth-child(3) > td:nth-child(2) > a:nth-child(6)";


            // * Defined by Baseball HQ
            // * Named by Baseball Hq when downloading to local downloads folder
            public string HqRestOfSeasonProjectionReportInitialCsvFileName
            {
                get => "BaseballHQ_M_B_P.csv";
            }


            // * Defined by me
            // * = "HqHitterReport_PROJ_"
            private string HitterRestOfSeasonProjectionsCsvFileNameBase
            {
                get => _projectDirectory.BaseballHqHitterRosProjectionsCsvFileNameBase;
            }


        /* --------------------------------------------------------------- */
        /* PRIMARY METHOD - REST OF SEASON PROJECTIONS                     */
        /* --------------------------------------------------------------- */



        /* --------------------------------------------------------------- */
        /* HQ CSV - REST OF SEASON PROJECTION                              */
        /* --------------------------------------------------------------- */

        // STATUS [ August 13, 2019 ] : this works
        // ROS PROJECTION
        /// <summary>
        ///     * Downloads csv from hq
        ///     * File should be downloaded each day to get the latest and greatest data
        /// </summary>
        /// <param name="openFileAfterMove">todo: describe openFileAfterMove parameter on DownloadRestOfSeasonProjectionsAsync</param>
        /// <remarks>
        ///     * STEPS
        ///       * 1) Go to baseball hq website and log in
        ///       * 2) Navigate to reports page and download report to local downloads folder
        ///       * 3) Move report to hq target write folder
        ///     * PARAMATERS
        ///       * _hqRestOfSeasonProjectionReportInitialCsvFileName  = BaseballHQ_M_B_P.csv
        ///       * _hitterRestOfSeasonProjectionsCsvFileNameBase      = HqHitterReport_PROJ_
        /// </remarks>
        public async Task DownloadRestOfSeasonProjectionsAsync(bool openFileAfterMove)
        {
            // _helpers.OpenMethod(3);
            await DownloadHqHitterReportAsync(
                _hitterRestOfSeasonProjectionsReportSelector,
                HqRestOfSeasonProjectionReportInitialCsvFileName,
                HitterRestOfSeasonProjectionsCsvFileNameBase,
                openFileAfterMove
            );
        }


            // ROS PROJECTION
            // * Get the List from the task by:
            // * > var list = Task.Run(() => GetAllAsyncRos_CSV(openRosFileAfterMove)).Result;
            public async Task<List<HqHitterRestOfSeasonProjection>> GetAllRosAsync_CSV(bool openCsvAfterCreation)
            {
                // _helpers.OpenMethod(1);

                string todaysFileName = GetTodaysCsvFileName(HitterRestOfSeasonProjectionsCsvFileNameBase);
                bool fileCheck        = DoesCsvFileForTodayExist(todaysFileName);

                C.WriteLine($"\nROS: todaysFileName: {todaysFileName} fileCheck: {fileCheck}\n");

                if(!fileCheck)
                    await DownloadRestOfSeasonProjectionsAsync(openCsvAfterCreation);

                if(AreThereRosRecordsInDatabase())
                    await DeleteAllRos_DBAsync();

                C.WriteLine("Press any key to continue");
                C.ReadLine();

                // IList<object>
                var listOfCsvRows = _csvHandler.ReadCsvRecordsToList(
                    BaseballHqHitterWriteDirectory,
                    todaysFileName,
                    typeof(HqHitterRestOfSeasonProjection),
                    typeof(HqHitterRestOfSeasonProjectionClassMap)
                    // openCsvAfterCreation
                );

                var allRosList = new List<HqHitterRestOfSeasonProjection>();

                foreach(object playerObject in listOfCsvRows)
                {
                    HqHitterRestOfSeasonProjection player = playerObject as HqHitterRestOfSeasonProjection;
                    player.IDPLAYER = GetIdPlayerForPlayer(player);
                    // C.WriteLine($"ERROR --> {player.FirstName} {player.LastName} {player.MlbId}");
                    allRosList.Add(player);
                }
                return allRosList;
            }


            // ROS PROJECTION
            // Get the 'IDPLAYER' to join SfbbPlayerBase to HqHitterRestOfSeasonProjection
            public string GetIdPlayerForPlayer(HqHitterRestOfSeasonProjection player)
            {
                SfbbPlayerBase playerBase = new SfbbPlayerBase();

                List<SfbbPlayerBase> allPlayerBases = _context.SfbbPlayerBases.ToList();

                playerBase = (from bases in allPlayerBases
                              where bases.MLBID == player.MlbId
                              select bases).FirstOrDefault();

                // * this is called 'null propagation'
                // * if playerBase is null, IDPLAYER is null; otherwise IDPLAYER = playerBase.IDPLAYER
                string IDPLAYER = playerBase?.IDPLAYER;
                return IDPLAYER;
            }



        /* --------------------------------------------------------------- */
        /* CRUD - REST OF SEASON PROJECTION                                */
        /* --------------------------------------------------------------- */

        /* ----- CRUD - CREATE - ROS PROJECTIONS ----- */


        // STATUS [ August 13, 2019 ] : this works
        // ROS PROJECTION
        /// <summary>
        ///     * Add one HqHitterRestOfSeasonProjection to the database
        /// </summary>
        /// <param name="hitter">todo: describe hitter parameter on AddOneRosAsync_DB</param>
        public async Task<IActionResult> AddOneRosAsync_DB(HqHitterRestOfSeasonProjection hitter)
        {
            _helpers.OpenMethod(3);
            _context.HqHitterRestOfSeasonProjections.Attach(hitter);
            await _context.SaveChangesAsync(cancellationToken);
            return Ok();
        }


        // STATUS [ August 12, 2019 ] : this works
        // ROS PROJECTION
        /// <summary>
        ///     * Add list of HqHitterRestOfSeasonProjection hitters to the database
        /// </summary>
        /// <param name="listOfHitterRestOfSeasonProjections">todo: describe listOfHitterRestOfSeasonProjections parameter on AddAllRosAsync_DB</param>
        public async Task AddAllRosAsync_DB(IList<HqHitterRestOfSeasonProjection> listOfHitterRestOfSeasonProjections)
        {
            // _helpers.OpenMethod(3);
            int countAdded = 0; int countNotAdded = 0;

            foreach(HqHitterRestOfSeasonProjection player in listOfHitterRestOfSeasonProjections)
            {
                // HqHitterRestOfSeasonProjection
                var checkDbForBase = _context.HqHitterRestOfSeasonProjections.SingleOrDefault(h => h.HQID == player.HQID);

                if(checkDbForBase is null)
                {
                    _context.Entry(player).State = EntityState.Added;
                    await _context.Set<HqHitterRestOfSeasonProjection>().AddAsync(player, cancellationToken);
                }
                else
                {
                    _context.Entry(checkDbForBase).State = EntityState.Unchanged;
                }
                int manageCounters = (checkDbForBase == null) ? countAdded++ : countNotAdded++;
            }
            await _context.SaveChangesAsync(cancellationToken);
            _context.PrintDatabaseAddOutcomes(countAdded, countNotAdded, typeof(BaseballHqHitterController));
        }




        /* ----- CRUD - READ - ROS PROJECTIONS ----- */


        // STATUS [ August 13, 2019 ] : this works
        // ROS PROJECTION
        /// <summary>
        ///     * Read one HqHitterRestOfSeasonProjection hitter from database
        /// </summary>
        /// <param name="hqid">todo: describe hqid parameter on GetOneRosAsync_DB</param>
        public async Task<HqHitterRestOfSeasonProjection> GetOneRosAsync_DB(int? hqid)
        {
            // _helpers.OpenMethod(3);
            return await _context.HqHitterRestOfSeasonProjections.SingleOrDefaultAsync(h => h.HQID == hqid, cancellationToken);
            // return player;
        }


        // STATUS [ August 13, 2019 ] : this works
        // ROS PROJECTION
        /// <summary>
        ///     * Read all HqHitterRestOfSeasonProjection hitters from database
        /// </summary>
        public IList<HqHitterRestOfSeasonProjection> GetAllRos_DB()
        {
            // _helpers.OpenMethod(1);
            return _context.HqHitterRestOfSeasonProjections.ToList();
        }


        /* ----- CRUD - UPDATE - ROS PROJECTIONS ----- */

        // STATUS [ August 13, 2019 ] : haven't tested but should work
        // ROS PROJECTION
        // public async Task<IActionResult> UpdateOneRosAsync_DB(HqHitterRestOfSeasonProjection hitter)
        // {
        //     // _helpers.OpenMethod(3);
        //     // Add update code here
        //     return Ok();
        // }


        /* ----- CRUD - DELETE - ROS PROJECTIONS ----- */

        // STATUS [ August 13, 2019 ] : this works
        // ROS PROJECTION
        public void DeleteOneRos_DB(HqHitterRestOfSeasonProjection hitter)
        {
            // _helpers.OpenMethod(3);
            _context.Entry(hitter).State = EntityState.Detached;
            _context.Remove(hitter);
            _context.SaveChanges();
        }


        // ROS PROJECTION
        public async Task<IActionResult> DeleteOneRosAsync_DB(int hqId)
        {
            // _helpers.OpenMethod(3);
            HqHitterRestOfSeasonProjection hitter = await _context.HqHitterRestOfSeasonProjections.SingleOrDefaultAsync(h => h.HQID == hqId, cancellationToken);
            _context.Remove(hitter);
            await _context.SaveChangesAsync(cancellationToken);
            return Ok();
        }


        // ROS PROJECTION
        public async Task DeleteAllRos_DBAsync()
        {
            // _helpers.OpenMethod(3);
            var allHittersInDb = _context.HqHitterRestOfSeasonProjections.ToList();
            _context.RemoveRange(allHittersInDb);
            await _context.SaveChangesAsync(cancellationToken);
        }


        // STATUS [ August 13, 2019 ] : this works
        // ROS PROJECTION
        // * Checks for number of items in database
        // * If number is greater than 0, returns TRUE
        // * Else, returns FALSE
        public bool AreThereRosRecordsInDatabase()
        {
            // _helpers.OpenMethod(1);
            int count = GetAllRos_DB().Count;
            bool areThereRecordsInDatabase = (count > 0) ? true : false;
            C.WriteLine($"count: {count} {areThereRecordsInDatabase}");
            return areThereRecordsInDatabase;
        }


        #endregion HQ HITTERS ROS PROJECTIONS  ------------------------------------------------------------





        #region HQ HITTERS YEAR-TO-DATE (YTD)  ------------------------------------------------------------


        /* --------------------------------------------------------------- */
        /* BUILDING BLOCKS - YEAR TO DATE                                  */
        /* --------------------------------------------------------------- */

        // * Defined by Baseball HQ
        // * Can be found in the html of the hq page
        private readonly string _hitterYearToDateReportSelector = "table.stats-links:nth-child(5) > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(2) > a:nth-child(9)";


        // * Defined by Baseball HQ
        // * Named by Baseball Hq when downloading to local downloads folder
        public string HqYearToDateReportInitialCsvFileName
        {
            get => "BaseballHQ_M_B_Y.csv";
        }


        // * Defined by me
        // * Returns: HqHitterReport_YTD_
        private string HitterYearToDateCsvFileNameBase
        {
            get
            {
                return _projectDirectory.BaseballHqHitterYearToDateCsvFileNameBase;
            }
        }

        /* --------------------------------------------------------------- */
        /* PRIMARY METHOD - YEAR TO DATE                                   */
        /* --------------------------------------------------------------- */


        /* --------------------------------------------------------------- */
        /* HQ CSV TO DATABASE - YEAR TO DATE                               */
        /* --------------------------------------------------------------- */


        // STATUS [ August 13, 2019 ] : this works
        // YEAR TO DATE
        // * File should be downloaded each day to get the latest and greatest data
        //
        // * STEPS
        // * 1) Go to baseball hq website and log in
        // * 2) Navigate to reports page and download report to local downloads folder
        // * 3) Move report to hq target write folder
        //
        // * PARAMATERS
        // * _hqYearToDateReportInitialCsvFileName = BaseballHQ_M_B_Y.csv
        // * _hitterYearToDateCsvFileNameBase      = HqHitterReport_YTD_
        public async Task DownloadYearToDateReportAsync(bool openFileAfterMove)
        {
            // _helpers.OpenMethod(3);
            await DownloadHqHitterReportAsync(
                _hitterYearToDateReportSelector,
                HqYearToDateReportInitialCsvFileName,
                HitterYearToDateCsvFileNameBase,
                openFileAfterMove
            );
        }

        // STATUS [ August 13, 2019 ] : this works
        // YEAR TO DATE
        /// <summary>
        ///     Gets YTD csv from hq site and adds hitters to database
        /// </summary>
        /// <param name="openFileAfterMove">todo: describe openFileAfterMove parameter on GetAllYtdAsync_CSV</param>
        /// <remarks>
        ///     * Should be run each day
        ///     * 1) Checks if Csv File for todays exists in Hq Target_Write folders
        ///     * 2) If file doesn't exist, go to Hq website and download report
        ///     * 3) Check if there are records in the database table
        ///     * 4) If there are records, delete them (so you can replace them with todays data)
        ///     * 5) Add records to the HqHitterYearToDate table
        /// </remarks>
        public async Task<List<HqHitterYearToDate>> GetAllYtdAsync_CSV(bool openFileAfterMove)
        {
            // _helpers.OpenMethod(3);
            string todaysFileName = GetTodaysCsvFileName(HitterYearToDateCsvFileNameBase);
            bool fileCheck        = DoesCsvFileForTodayExist(todaysFileName);

            C.WriteLine($"\nROS: todaysFileName: {todaysFileName} fileCheck: {fileCheck}\n");

            if(!fileCheck)
                await DownloadYearToDateReportAsync(openFileAfterMove);

            if(AreThereYtdRecordsInDatabase())
                await DeleteAllYtdAsync_DB();

            C.WriteLine("Press any key to continue");
            C.ReadLine();

            IList<object> playerRecordsList = _csvHandler.ReadCsvRecordsToList(
                    BaseballHqHitterWriteDirectory,
                    todaysFileName,
                    typeof(HqHitterYearToDate),
                    typeof(HqHitterYearToDateClassMap)
                    // openFileAfterMove
                );

            List<HqHitterYearToDate> listHittersYTD = new List<HqHitterYearToDate>();
            foreach(object playerObject in playerRecordsList)
            {
                HqHitterYearToDate player = playerObject as HqHitterYearToDate;
                player.IDPLAYER = GetIdPlayerForPlayer(player);
                listHittersYTD.Add(player);
            }

            // PrintDetailsOfTodaysYearToDateDatabaseUpdate(todaysFileName, fileCheck, areThereRecordsInDatabase, listHittersYTD);
            return listHittersYTD;
        }


        // YEAR TO DATE
        // Get the 'IDPLAYER' to join SfbbPlayerBase to HqHitterRestOfSeasonProjection
        public string GetIdPlayerForPlayer(HqHitterYearToDate player)
        {
            SfbbPlayerBase playerBase = new SfbbPlayerBase();

            List<SfbbPlayerBase> allPlayerBases = _context.SfbbPlayerBases.ToList();

            playerBase = (from bases in allPlayerBases
                            where bases.MLBID == player.MlbId
                            select bases).FirstOrDefault();

            // * this is called 'null propagation'
            // * if playerBase is null, IDPLAYER is null; otherwise IDPLAYER = playerBase.IDPLAYER
            string IDPLAYER = playerBase?.IDPLAYER;
            return IDPLAYER;
        }


        /* --------------------------------------------------------------- */
        /* CRUD - YEAR TO DATE                                             */
        /* --------------------------------------------------------------- */


        /* ----- CRUD - READ - YEAR TO DATE ----- */

        // STATUS [ August 13, 2019 ] : this works
        // YEAR TO DATE
        public async Task<IActionResult> AddOneYtdAsync_DB(HqHitterYearToDate hitter)
        {
            await _context.AddAsync(hitter, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Ok();
        }


        // STATUS [ August 13, 2019 ] : this works
        // YEAR TO DATE
        public async Task AddAllYtdAsync_DB(IList<HqHitterYearToDate> listOfHitterYearToDateRecords)
        {
            // _helpers.OpenMethod(3);
            int countAdded = 0; int countNotAdded = 0;

            foreach(HqHitterYearToDate player in listOfHitterYearToDateRecords)
            {
                var checkDbForBase = _context.HqHitterYearToDates.SingleOrDefault(h => h.HQID == player.HQID);
                if(checkDbForBase is null)
                {
                    _context.Entry(player).State = EntityState.Added;
                    await _context.Set<HqHitterYearToDate>().AddAsync(player, cancellationToken);
                }
                else
                {
                    _context.Entry(checkDbForBase).State = EntityState.Unchanged;
                }
                int manageCounters = (checkDbForBase == null) ? countAdded++ : countNotAdded++;
            }
            await _context.SaveChangesAsync(cancellationToken);
            _context.PrintDatabaseAddOutcomes(countAdded, countNotAdded, typeof(BaseballHqHitterController));
        }



        /* ----- CRUD - READ - YEAR TO DATE ----- */


        // STATUS [ August 13, 2019 ] : this works
        // YEAR TO DATE
        public async Task<HqHitterYearToDate> GetOneYtdAsync_DB(int? hqid)
        {
            return await _context.HqHitterYearToDates.SingleOrDefaultAsync(h => h.HQID == hqid, cancellationToken);
        }

        // STATUS [ August 13, 2019 ] : this works
        // YEAR TO DATE
        public IList<HqHitterYearToDate> GetAllYtd_DB()
        {
            // _helpers.OpenMethod(1);
            return _context.HqHitterYearToDates.ToList();
        }


        /* ----- CRUD - UPDATE - YEAR TO DATE ----- */



        /* ----- CRUD - DELETE - YEAR TO DATE ----- */


        // STATUS [ August 13, 2019 ] : this works
        // YEAR TO DATE
        public async Task<IActionResult> DeleteOneAsyncYtd_DB(int hqId)
        {
            // _helpers.OpenMethod(3);
            var hitter = await _context.HqHitterYearToDates.SingleOrDefaultAsync(h => h.HQID == hqId, cancellationToken);
            _context.Remove(hitter);
            await _context.SaveChangesAsync(cancellationToken);
            return Ok();
        }


        // STATUS [ August 13, 2019 ] : this works
        // CRUD -> DELETE | ALL
        public async Task DeleteAllYtdAsync_DB()
        {
            // _helpers.OpenMethod(3);
            var hitters = _context.HqHitterYearToDates.ToList();
            _context.RemoveRange(hitters);
            await _context.SaveChangesAsync(cancellationToken);
        }


        // STATUS [ August 13, 2019 ] : this works
        // * Checks for number of items in database
        // * If number is greater than 0, returns TRUE
        // * Else, returns FALSE
        // * Example:
        // *    string todaysFileName = GetTodaysCsvFileName(_hitterYearToDateCsvFileNameBase);
        // *    var fileCheck = DoesCsvFileForTodayExist(todaysFileName);
        public bool AreThereYtdRecordsInDatabase()
        {
            // _helpers.OpenMethod(1);
            int count = GetAllYtd_DB().Count;
            bool areThereRecordsInDatabase = (count > 0) ? true : false;
            return areThereRecordsInDatabase;
        }


        #endregion HQ HITTERS YEAR-TO-DATE (YTD)  ------------------------------------------------------------





        #region HELPERS ------------------------------------------------------------


        // reportCssSelector options:
        // * YTD and ROS projections:   _currentStatsAndProjectionsReportSelector
        // * ROS projections:           _hitterProjectionsReportSelector
        // * YTD:                       _hitterYearToDateReportSelector
        //
        // Used By:
        // * DownloadHitterStatsAndProjectionsReport()
        // * DownloadRestOfSeasonProjections()
        // * DownloadYearToDateReport()
        //
        // _hqHitterTargetWriteFolderPath:
        // * "BaseballData/02_Target_Write/BaseballHQ_Target_Write/HqHitterFiles/";
        public async Task DownloadHqHitterReportAsync(string reportCssSelector, string downloadedCsvFileName, string fileNamePrefix, bool openFileAfterMove)
        {
            // _helpers.OpenMethod(3);
            PrintDownloadedReportDetails(reportCssSelector, downloadedCsvFileName, fileNamePrefix, openFileAfterMove);
            var page = await _hqUtilitiesController.CreateChromePageAsync();
            await _hqUtilitiesController.LoginToBaseballHqAsync(page);
            await _hqUtilitiesController.DownloadHqReportAsync(page, reportCssSelector);
            await _hqUtilitiesController.MoveReportToHqFolderAsync(openFileAfterMove, downloadedCsvFileName,BaseballHqHitterWriteDirectory, fileNamePrefix);
        }


        // STATUS [ August 13, 2019 ] : this works
        // * 'todaysReportName' elements
        // * 1) csvFileNameBase    e.g., -> HqHitterReport_PROJ_
        // * 2) todaysDateString   e.g., -> 8_12_2019
        // * 3) todaysReportName   e.g., -> HqHitterReport_PROJ_8_12_2019.csv
        public string GetTodaysCsvFileName(string csvFileNameBase)
        {
            string todaysDateString = _csvHandler.TodaysDateString();
            string todaysReportName = $"{csvFileNameBase}{todaysDateString}.csv";
            return todaysReportName;
        }


        // STATUS [ August 13, 2019 ] : this works
        // * Checks 4 different path and file name combos for a csv for today
        // * If path+fileName exists, returns TRUE
        // * If path+fileName does NOT exist, returns FALSE
        public bool DoesCsvFileForTodayExist(string todaysReportName)
        {
            bool doesCsvFileForTodayExist = false;

            string fullHqPathAndFileToCheck               =   $"{BaseballHqHitterWriteDirectory}{todaysReportName}";
            string fullHqArchivePathAndFileToCheck        =   $"{BaseballHqArchiveDirectory}{todaysReportName}";
            string fullHqPathAndFileToCheckCleaned        =   $"{BaseballHqHitterWriteDirectory}_{todaysReportName}";
            string fullHqArchivePathAndFileToCheckCleaned =   $"{BaseballHqArchiveDirectory}_{todaysReportName}";

            if(System.IO.File.Exists(fullHqPathAndFileToCheck))
                doesCsvFileForTodayExist = true;

            else if(System.IO.File.Exists(fullHqArchivePathAndFileToCheck))
                doesCsvFileForTodayExist = true;

            else if(System.IO.File.Exists(fullHqPathAndFileToCheckCleaned))
                doesCsvFileForTodayExist = true;

            else if(System.IO.File.Exists(fullHqArchivePathAndFileToCheckCleaned))
                doesCsvFileForTodayExist = true;

            // else
                // C.WriteLine("DoesCsvFileForTodayExist(): FILE FOR TODAY DOES NOT EXIST");

            return doesCsvFileForTodayExist;
        }


        #endregion HELPERS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------


        private void PrintDownloadedReportDetails(string reportCssSelector, string downloadedCsvFileName, string fileNamePrefix, bool openFileAfterMove)
        {
            C.WriteLine($"\n-------------------------------------------------------------------");
            _helpers.PrintNameSpaceControllerNameMethodName(typeof(BaseballHqHitterController));
            C.WriteLine($"CLICK CSS SELECTOR   : {reportCssSelector}");
            C.WriteLine($"CSV DL FILE NAME     : {downloadedCsvFileName}");
            C.WriteLine($"CSV FILE NAME PREFIX : {fileNamePrefix}");
            C.WriteLine($"MOVE TO              : {BaseballHqHitterWriteDirectory}");
            C.WriteLine($"OPEN CSV AFTER MOVE  : {openFileAfterMove}");
            C.WriteLine($"-------------------------------------------------------------------\n");
        }


        private void PrintDatabaseAddOutcomes(string todaysFileName, bool fileCheck, bool areThereRecordsInDatabase,List<HqHitterYearToDate> listHittersYTD)
        {
            C.WriteLine($"\n-----------------------------------------------");
            C.WriteLine($"DETAILS FOR TODAYS YTD DATABASE UPDATE");
            C.WriteLine($"-----------------------------------------------");
            C.WriteLine($"Todays Csv File: {todaysFileName}");
            C.WriteLine($"Does Csv File Exist? {fileCheck}");
            C.WriteLine($"Are there records in database? {areThereRecordsInDatabase}");
            C.WriteLine($"UpdateDatabaseWithTodaysData(): adding {listHittersYTD.Count} to database");
            C.WriteLine($"-----------------------------------------------\n");
        }


        private void PrintDatabaseAddOutcomes(string todaysFileName, bool fileCheck, bool areThereRecordsInDatabase,List<HqHitterRestOfSeasonProjection> listHittersROS)
        {
            C.WriteLine($"\n-----------------------------------------------");
            C.WriteLine($"DETAILS FOR TODAYS ROS DATABASE UPDATE");
            C.WriteLine($"-----------------------------------------------");
            C.WriteLine($"Todays Csv File: {todaysFileName}");
            C.WriteLine($"Does Csv File Exist? {fileCheck}");
            C.WriteLine($"Are there records in database? {areThereRecordsInDatabase}");
            C.WriteLine($"UpdateDatabaseWithTodaysData(): adding {listHittersROS.Count} to database");
            C.WriteLine($"-----------------------------------------------\n");
        }


        private void PrintDatabaseAddOutcomes(int countAddedNew, int deletedAndAdded)
        {
            C.WriteLine($"\n-------------------------------------------------------------------");
            _helpers.PrintNameSpaceControllerNameMethodName(typeof(BaseballHqHitterController));
            C.WriteLine($"ADDED NEW TO DB : {countAddedNew}");
            C.WriteLine($"UPDATED IN DB   : {deletedAndAdded}");
            C.WriteLine($"-------------------------------------------------------------------\n");
        }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
