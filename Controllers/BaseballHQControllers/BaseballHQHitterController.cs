using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.BaseballHq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using C = System.Console;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.BaseballHQControllers
{
    [Route("api/hq/[controller]")]
    [ApiController]
    // [ApiExplorerSettings(IgnoreApi = true)]
    public class BaseballHQHitterController : ControllerBase
    {
        private readonly Helpers                       _helpers;
        private readonly CsvHandler                    _csvHandler;
        private readonly BaseballHqUtilitiesController _hqUtilitiesController;
        private readonly BaseballScraperContext        _context;
        private readonly ProjectDirectoryEndPoints     _projectDirectory;


        public BaseballHQHitterController(Helpers helpers, BaseballHqUtilitiesController hqUtilitiesController, CsvHandler csvHandler, BaseballScraperContext context, ProjectDirectoryEndPoints projectDirectory)
        {
            _helpers               = helpers;
            _hqUtilitiesController = hqUtilitiesController;
            _csvHandler            = csvHandler;
            _context               = context;
            _projectDirectory      = projectDirectory;
        }

        public BaseballHQHitterController() {}


        // Defined by me
        private readonly string        _csvFileNameBase = "HqHitterReport_";



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


        // BaseballData/02_WRITE/BASEBALL_HQ/HITTERS/
        // * Set in ProjectDirectoryEndPoints
        private string BaseballHqHitterWriteDirectory
        {
            get => _projectDirectory.BaseballHqHitterWriteRelativePath;
        }

        // BaseballData/02_WRITE/BASEBALL_HQ/_archive/
        // * Set in ProjectDirectoryEndPoints
        private string BaseballHqArchiveDirectory
        {
            get => _projectDirectory.BaseballHqArchiveRelativePath;
        }



        #region ROS PROJECTIONS AND YEAR TO DATE ------------------------------------------------------------

            [HttpPost("mrc")]
            public async Task<IActionResult> MASTER_REPORT_CALLER(bool openRosFileAfterMove, bool openYtdFileAfterMove)
            {
                _helpers.OpenMethod(3);
                await UpdateBothHqHitterDatabases(openRosFileAfterMove, openYtdFileAfterMove);
                return Ok();
            }


            // STATUS [ August 13, 2019 ] : this works
            // * PRIMARY METHOD FOR EVERYTHING BELOW
            /// <summary>
            ///     * Gets all hitter reports from hq and adds them to database
            ///     * Updates the two main bases with day from today
            /// </summary>
            /// <remarks>
            ///     * SEE: MasterHitterController (as of August 13, 2019)
            /// </remarks>
            [HttpPost("update_all")]
            public async Task<ActionResult> UpdateBothHqHitterDatabases(bool openRosFileAfterMove, bool openYtdFileAfterMove)
            {
                _helpers.OpenMethod(3);

                // * PRIMARY METHOD - REST OF SEASON PROJECTIONS
                await UpdateDatabaseWithTodaysHitterRestOfSeasonProjectionsData(openRosFileAfterMove);

                // * PRIMARY METHOD - YEAR TO DATE
                await UpdateDatabaseWithTodaysHitterYearToDateData(openYtdFileAfterMove);
                return Ok();
            }


            // STATUS [ August 13, 2019 ] : this should work but not tested
            // * Downloads a report that includes both projections and YTD
            // * Better to use to separate methods for projections and YTD that are below
            [HttpGet]
            public async Task DownloadHitterStatsAndProjectionsReport()
            {
                await DownloadHqHitterReport(
                    _currentStatsAndProjectionsReportSelector,
                    _hqYearToDateAndProjectionReportInitialCsvFileName,     // BaseballHQ_M_B_PY.csv
                    _csvFileNameBase,                                       // HqHitterReport_8_12_2019
                    false
                );
            }

            // Projections and YTD
            // Named by Baseball Hq when downloading to local downloads folder
            public string _hqYearToDateAndProjectionReportInitialCsvFileName
            {
                get => "BaseballHQ_M_B_PY.csv";
            }

            // Css selector used to identify link to click to download hitter report
            // Projections and YTD
            private readonly string _currentStatsAndProjectionsReportSelector = "table.stats-links:nth-child(5) > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(2) > a:nth-child(8)";

        #endregion ROS PROJECTIONS AND YEAR TO DATE ------------------------------------------------------------





        #region HQ HITTERS ROS PROJECTIONS  ------------------------------------------------------------


            /* --------------------------------------------------------------- */
            /* BUILDING BLOCKS - REST OF SEASON PROJECTIONS                    */
            /* --------------------------------------------------------------- */

            // Defined by Baseball HQ
            // Can be found in the html of the hq page
            private readonly string _hitterRestOfSeasonProjectionsReportSelector = "#node-384 > div > table:nth-child(5) > tbody > tr:nth-child(3) > td:nth-child(2) > a:nth-child(6)";

            // Defined by Baseball HQ
            // Named by Baseball Hq when downloading to local downloads folder
            public string _hqRestOfSeasonProjectionReportInitialCsvFileName
            {
                get => "BaseballHQ_M_B_P.csv";
            }

            // Defined by me
            private readonly string _hitterRestOfSeasonProjectionsCsvFileIdentifier = "PROJ_";

            // Defined by me
            // Returns: HqHitterReport_PROJ_
            private string _hitterRestOfSeasonProjectionsCsvFileNameBase
            {
                get
                {
                    return $"{_csvFileNameBase}{_hitterRestOfSeasonProjectionsCsvFileIdentifier}";
                }
                set {}
            }


            /* --------------------------------------------------------------- */
            /* PRIMARY METHOD - REST OF SEASON PROJECTIONS                     */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // * PRIMARY METHOD - REST OF SEASON PROJECTIONS
            /// <summary>
            ///     Gets ROS csv from hq site and adds hitters to database
            /// </summary>
            /// <remarks>
            ///     * Should be run each day
            ///     * 1) Checks if Csv File for todays exists in Hq Target_Write folders
            ///     * 2) If file doesn't exist, go to Hq website and download report
            ///     * 3) Check if there are records in the database table
            ///     * 4) If there are records, delete them (so you can replace them with todays data)
            ///     * 5) Add records to the HqHitter ROS Projection table
            /// </remarks>
            [HttpPost("update_projections")]
            public async Task<IActionResult> UpdateDatabaseWithTodaysHitterRestOfSeasonProjectionsData(bool openFileAfterMove)
            {
                _helpers.OpenMethod(3);
                /*     STEP 1     */
                string todaysFileName = GetTodaysCsvFileName(_hitterRestOfSeasonProjectionsCsvFileNameBase);
                bool fileCheck        = DoesCsvFileForTodayExist(todaysFileName);

                /*     STEP 2     */
                if(fileCheck == false)
                {
                    /*     PROCESS A     */
                    await DownloadRestOfSeasonProjections(openFileAfterMove);
                }

                /*     STEP 3     */
                bool areThereRecordsInDatabase = CheckIfThereAreRestOfSeasonProjectionsRecordsInDatabase();

                /*     STEP 4     */
                if(areThereRecordsInDatabase == true)
                {
                    List<HqHitterRestOfSeasonProjection> hqHitters = GetListOfAllRestOfSeasonProjectionRecords();
                    // await DeleteAllRestOfSeasonProjectionRecords(hqHitters);
                }

                /*     STEP 5     */
                /*     PROCESS B     */
                await CreateRestOfSeasonProjectionsListFromCsvAndAddToDatabase(todaysFileName);
                return Ok();
            }


            /* --------------------------------------------------------------- */
            /* HQ CSV TO DATABASE - REST OF SEASON PROJECTION                  */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // * PROCESS A
            /// <summary>
            ///     * Downloads csv from hq
            ///     * File should be downloaded each day to get the latest and greatest data
            /// </summary>
            /// <remarks>
            ///     * STEPS
            ///       * 1) Go to baseball hq website and log in
            ///       * 2) Navigate to reports page and download report to local downloads folder
            ///       * 3) Move report to hq target write folder
            ///     * PARAMATERS
            ///       * _hqRestOfSeasonProjectionReportInitialCsvFileName  = BaseballHQ_M_B_P.csv
            ///       * _hitterRestOfSeasonProjectionsCsvFileNameBase      = HqHitterReport_PROJ_
            /// </remarks>
            [HttpGet]
            [Route("ros")]
            public async Task DownloadRestOfSeasonProjections(bool openFileAfterMove)
            {
                _helpers.OpenMethod(3);
                await DownloadHqHitterReport(
                    _hitterRestOfSeasonProjectionsReportSelector,
                    _hqRestOfSeasonProjectionReportInitialCsvFileName,
                    _hitterRestOfSeasonProjectionsCsvFileNameBase,
                    openFileAfterMove
                );
            }


            // STATUS [ August 13, 2019 ] : this works
            // * PROCESS B
            /// <summary>
            ///     * Outcome is that all records are added to the database
            /// </summary>
            /// <remarks>
            ///     * STEPS
            ///     * 1) CreateListOfRestOfSeasonProjectionObjectsFromCsvRows(todaysFileName)
            ///     * 2) CreateListOfHqHittersRestOfSeasonProjections(listOfGenericObjects)
            ///     * 3) await AddRestOfSeasonProjectionsToDatabase(listHittersYTD);
            /// </remarks>
            ///
            [HttpPost]
            [Route("ros")]
            public async Task<ActionResult> CreateRestOfSeasonProjectionsListFromCsvAndAddToDatabase(string todaysFileName)
            {
                _helpers.OpenMethod(3);
                IList<object> listOfGenericObjects      = CreateListOfRestOfSeasonProjectionObjectsFromCsvRows(todaysFileName);
                List<HqHitterRestOfSeasonProjection> listHittersRosProjection = CreateListOfHqHittersRestOfSeasonProjections(listOfGenericObjects);
                await AddRestOfSeasonProjectionsToDatabase(listHittersRosProjection);
                return Ok();
            }


            // STATUS [ August 13, 2019 ] : this works
            // PROCESS B : STEP 1
            /// <summary>
            ///     Creates a list of generic objects (to be turned into ROS hitters) from csv
            /// </summary>
            /// <param name="csvFileName">
            ///     * 'csvFileName' is just the file name; no path included
            ///     * Ex: csvFileName e.g., HqHitterReport_PROJ_8_12_2019.csv
            /// </param>
            [HttpGet]
            [Route("ros_list_objects")]
            public IList<object> CreateListOfRestOfSeasonProjectionObjectsFromCsvRows(string csvFileName)
            {
                _helpers.OpenMethod(1);
                var playerRecordsList = _csvHandler.ReadCsvRecordsToList(
                    BaseballHqHitterWriteDirectory,
                    csvFileName,
                    typeof(HqHitterRestOfSeasonProjection),
                    typeof(HqHitterRestOfSeasonProjectionClassMap),
                    true
                );
                return playerRecordsList;
            }


            // STATUS [ August 12, 2019 ] : this works
            // PROCESS B : STEP 2
            /// <summary>
            ///     Takes a list of generic objects and turns them into HqHitterRestOfSeasonProjection
            /// </summary>
            /// <param name="playerRecordsList">
            ///     * A list generated from CreateListOfRestOfSeasonProjectionObjectsFromCsvRows() method
            /// </param>
            [HttpGet]
            [Route("ros_list")]
            public List<HqHitterRestOfSeasonProjection> CreateListOfHqHittersRestOfSeasonProjections(IList<object> playerRecordsList)
            {
                _helpers.OpenMethod(1);
                List<HqHitterRestOfSeasonProjection> listOfHitterRestOfSeasonProjections = new List<HqHitterRestOfSeasonProjection>();

                foreach(object playerObject in playerRecordsList)
                {
                    HqHitterRestOfSeasonProjection player = playerObject as HqHitterRestOfSeasonProjection;
                    listOfHitterRestOfSeasonProjections.Add(player);
                }
                return listOfHitterRestOfSeasonProjections;
            }


            /* --------------------------------------------------------------- */
            /* CRUD - CREATE - REST OF SEASON PROJECTION                       */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> CREATE | ONE
            /// <summary>
            ///     * Add one HqHitterRestOfSeasonProjection to the database
            /// </summary>
            [HttpPost]
            [Route("add_ros/{hitter}")]
            public async Task<IActionResult> AddRestOfSeasonProjectionToDatabase(HqHitterRestOfSeasonProjection hitter)
            {
                await _context.AddAsync(hitter);
                await _context.SaveChangesAsync();
                return Ok();
            }

            // STATUS [ August 12, 2019 ] : this works
            // PROCESS B : STEP 3
            // CRUD -> CREATE | MANY
            /// <summary>
            ///     * Add list of HqHitterRestOfSeasonProjection hitters to the database
            /// </summary>
            [HttpPost]
            [Route("add_ros/all")]
            public async Task<IActionResult> AddRestOfSeasonProjectionsToDatabase(List<HqHitterRestOfSeasonProjection> listOfHitterRestOfSeasonProjections)
            {
                _helpers.OpenMethod(3);
                foreach(var player in listOfHitterRestOfSeasonProjections)
                {
                    var checkDbForBase = _context.HqHitterRestOfSeasonProjections.SingleOrDefault(h => h.HQID == player.HQID);
                    if(checkDbForBase is null)
                    {
                        await _context.AddAsync(player);
                    }
                    else
                    {
                        // C.WriteLine("HQ ROS HITTER EXISTS");
                    }
                }
                await _context.SaveChangesAsync();
                return Ok();
            }


            /* --------------------------------------------------------------- */
            /* CRUD - READ - REST OF SEASON PROJECTION                         */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> READ | ONE
            /// <summary>
            ///     * Read one HqHitterRestOfSeasonProjection hitter from database
            /// </summary>
            [HttpGet]
            [Route("read_ros/{hqid}")]
            public async Task<HqHitterRestOfSeasonProjection> GetOneRestOfSeasonProjectionRecord(int? hqid)
            {
                HqHitterRestOfSeasonProjection player = await _context.HqHitterRestOfSeasonProjections.SingleOrDefaultAsync(h => h.HQID == hqid);
                return player;
            }

            /// STATUS [ August 13, 2019 ] : this works
            /// CRUD -> READ | ALL
            /// <summary>
            ///     * Read all HqHitterRestOfSeasonProjection hitters from database
            /// </summary>
            [HttpGet]
            [Route("read_ros/all")]
            public List<HqHitterRestOfSeasonProjection> GetListOfAllRestOfSeasonProjectionRecords()
            {
                _helpers.OpenMethod(1);
                List<HqHitterRestOfSeasonProjection> hqHitters = _context.HqHitterRestOfSeasonProjections.ToList();
                return hqHitters;
            }


            /* --------------------------------------------------------------- */
            /* CRUD - UPDATE - REST OF SEASON PROJECTION                       */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : haven't tested but should work
            // CRUD -> UPDATE | ONE
            [HttpPut("update_ros/{hitter}")]
            public async Task<IActionResult> UpdateOneRestOfSeasonProjectionRecord(HqHitterRestOfSeasonProjection hitter)
            {
                if(hitter == null)
                {
                    return NotFound();
                }

                HqHitterRestOfSeasonProjection playerToUpdate = await _context.HqHitterRestOfSeasonProjections.FirstOrDefaultAsync(h => h.HQID == hitter.HQID);

                if(ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(playerToUpdate);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    catch(DbUpdateException /* ex */)
                    {
                        C.WriteLine("Database update error");
                    }
                }
                return Ok();
            }



            /* --------------------------------------------------------------- */
            /* CRUD - DELETE - REST OF SEASON PROJECTION                       */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> DELETE | ONE
            [HttpDelete("delete_ros/{hqId}")]
            public async Task<IActionResult> DeleteOneRestOfSeasonProjectionRecord(int hqId)
            {
                _helpers.OpenMethod(3);
                HqHitterRestOfSeasonProjection hitter = await _context.HqHitterRestOfSeasonProjections.SingleOrDefaultAsync(h => h.HQID == hqId);
                _context.Remove(hitter);
                await _context.SaveChangesAsync();
                return Ok();
            }

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> DELETE | ALL
            [HttpDelete("delete_ros/all")]
            public async Task<IActionResult> DeleteAllRestOfSeasonProjectionRecords(List<HqHitterRestOfSeasonProjection> hitters)
            {
                _helpers.OpenMethod(3);
                foreach(var hitter in hitters)
                {
                    await DeleteOneRestOfSeasonProjectionRecord(hitter.HQID);
                }
                return Ok();
            }


            // STATUS [ August 13, 2019 ] : this works
            // * Checks for number of items in database
            // * If number is greater than 0, returns TRUE
            // * Else, returns FALSE
            [ApiExplorerSettings(IgnoreApi = true)]
            public bool CheckIfThereAreRestOfSeasonProjectionsRecordsInDatabase()
            {
                _helpers.OpenMethod(1);
                List<HqHitterRestOfSeasonProjection> hqHitters = GetListOfAllRestOfSeasonProjectionRecords();
                int count = hqHitters.Count;
                bool areThereRecordsInDatabase;

                if(count > 0)
                    areThereRecordsInDatabase = true;

                else
                    areThereRecordsInDatabase = false;

                return areThereRecordsInDatabase;
            }


        #endregion HQ HITTERS ROS PROJECTIONS  ------------------------------------------------------------





        #region HQ HITTERS YEAR-TO-DATE (YTD)  ------------------------------------------------------------


            /* --------------------------------------------------------------- */
            /* BUILDING BLOCKS - YEAR TO DATE                                  */
            /* --------------------------------------------------------------- */

            // Defined by Baseball HQ
            // Can be found in the html of the hq page
            private readonly string _hitterYearToDateReportSelector = "table.stats-links:nth-child(5) > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(2) > a:nth-child(9)";

            // Defined by Baseball HQ
            // Named by Baseball Hq when downloading to local downloads folder
            public string _hqYearToDateReportInitialCsvFileName
            {
                get => "BaseballHQ_M_B_Y.csv";
            }

            // Defined by me
            private readonly string _hitterYearToDateCsvFileIdentifier = "YTD_";


            // Defined by me
            // Returns: HqHitterReport_YTD_
            private string _hitterYearToDateCsvFileNameBase
            {
                get
                {
                    return $"{_csvFileNameBase}{_hitterYearToDateCsvFileIdentifier}";
                }
                set {}
            }



            /* --------------------------------------------------------------- */
            /* PRIMARY METHOD - YEAR TO DATE                                   */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // * PRIMARY METHOD - YEAR TO DATE
            /// <summary>
            ///     Gets YTD csv from hq site and adds hitters to database
            /// </summary>
            /// <remarks>
            ///     * Should be run each day
            ///     * 1) Checks if Csv File for todays exists in Hq Target_Write folders
            ///     * 2) If file doesn't exist, go to Hq website and download report
            ///     * 3) Check if there are records in the database table
            ///     * 4) If there are records, delete them (so you can replace them with todays data)
            ///     * 5) Add records to the HqHitterYearToDate table
            /// </remarks>

            [HttpPost("update_ytd")]
            public async Task<IActionResult> UpdateDatabaseWithTodaysHitterYearToDateData(bool openFileAfterMove)
            {
                _helpers.OpenMethod(3);
                /*     STEP 1     */
                string todaysFileName = GetTodaysCsvFileName(_hitterYearToDateCsvFileNameBase);
                bool fileCheck        = DoesCsvFileForTodayExist(todaysFileName);

                /*     STEP 2     */
                if(fileCheck == false)
                {
                    /*     PROCESS A     */
                    await DownloadYearToDateReport(openFileAfterMove);
                }

                /*     STEP 3     */
                bool areThereRecordsInDatabase = CheckIfThereAreYearToDateRecordsInDatabase();

                /*     STEP 4     */
                if(areThereRecordsInDatabase == true)
                {
                    List<HqHitterYearToDate> hqHitters = GetListOfAllYearToDateRecordsInDatabase();
                    // await DeleteAllYearToDateRecords(hqHitters);
                }

                /*     STEP 5     */
                /*     PROCESS A     */
                await CreateYearToDatePlayerListFromCsvAndAddToDatabase(todaysFileName);

                // PrintDetailsOfTodaysYearToDateDatabaseUpdate(todaysFileName, fileCheck, areThereRecordsInDatabase, listHittersYTD);
                return Ok();
            }


            /* --------------------------------------------------------------- */
            /* HQ CSV TO DATABASE - YEAR TO DATE                               */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // * PROCESS A
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
            [HttpGet("ytd")]
            public async Task DownloadYearToDateReport(bool openFileAfterMove)
            {
                _helpers.OpenMethod(3);
                await DownloadHqHitterReport(
                    _hitterYearToDateReportSelector,
                    _hqYearToDateReportInitialCsvFileName,
                    _hitterYearToDateCsvFileNameBase,
                    openFileAfterMove
                );
            }

            // STATUS [ August 13, 2019 ] : this works
            // * PROCESS B
            // * Outcome is that all records are added to the database
            // * This is made up of three immediate below methods
            // * 1) CreateListOfYearToDateObjectsFromCsvRows(todaysFileName)
            // * 2) CreateListOfHqHittersYTD(listOfGenericObjects)
            // * 3) await AddYearToDateRecordsToDatabase(listHittersYTD);
            [HttpPost("ytd")]
            public async Task<ActionResult> CreateYearToDatePlayerListFromCsvAndAddToDatabase(string todaysFileName)
            {
                _helpers.OpenMethod(3);
                IList<object> listOfGenericObjects      = CreateListOfYearToDateObjectsFromCsvRows(todaysFileName);
                List<HqHitterYearToDate> listHittersYTD = CreateListOfHqHittersYTD(listOfGenericObjects);
                await AddYearToDateRecordsToDatabase(listHittersYTD);
                return Ok();
            }

            // STATUS [ August 13, 2019 ] : this works
            // PROCESS B : STEP 1
            // 'csvFileName' is just the file name; no path included
            // csvFileName e.g., HqHitterReport_YTD_8_12_2019.csv
            [HttpGet("ytd_list_objects")]
            public IList<object> CreateListOfYearToDateObjectsFromCsvRows(string csvFileName)
            {
                _helpers.OpenMethod(1);
                IList<object> playerRecordsList = _csvHandler.ReadCsvRecordsToList(
                    BaseballHqHitterWriteDirectory,
                    csvFileName,
                    typeof(HqHitterYearToDate),
                    typeof(HqHitterYearToDateClassMap),
                    true
                );
                return playerRecordsList;
            }

            // STATUS [ August 13, 2019 ] : this works
            // PROCESS B : STEP 2
            // * Create List<HqHitterYearToDate>
            [HttpGet("ytd_list")]
            public List<HqHitterYearToDate> CreateListOfHqHittersYTD(IList<object> playerRecordsList)
            {
                _helpers.OpenMethod(1);
                List<HqHitterYearToDate> listHittersYTD = new List<HqHitterYearToDate>();
                foreach(object playerObject in playerRecordsList)
                {
                    HqHitterYearToDate player = playerObject as HqHitterYearToDate;
                    listHittersYTD.Add(player);
                }
                return listHittersYTD;
            }


            /* --------------------------------------------------------------- */
            /* CRUD - CREATE - YEAR TO DATE                                    */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> CREATE | ONE
            [HttpPost("add_ytd/{hitter}")]
            public async Task<IActionResult> AddYearToDateRecordToDatabase(HqHitterYearToDate hitter)
            {
                await _context.AddAsync(hitter);
                await _context.SaveChangesAsync();
                return Ok();
            }

            // STATUS [ August 13, 2019 ] : this works
            // PROCESS B : STEP 3
            // CRUD -> CREATE | MANY
            [HttpPost("add_ytd/all")]
            public async Task<IActionResult> AddYearToDateRecordsToDatabase(List<HqHitterYearToDate> listOfHitterYearToDateRecords)
            {
                _helpers.OpenMethod(3);
                foreach(var player in listOfHitterYearToDateRecords)
                {
                    var checkDbForBase = _context.HqHitterYearToDates.SingleOrDefault(h => h.HQID == player.HQID);
                    if(checkDbForBase is null)
                    {
                        await _context.AddAsync(player);
                    }
                    else
                    {

                    }
                }
                await _context.SaveChangesAsync();
                return Ok();
            }


            /* --------------------------------------------------------------- */
            /* CRUD - READ - YEAR TO DATE                                      */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> READ | ONE
            [HttpGet("read_ytd/{hqid}")]
            public async Task<HqHitterYearToDate> GetOneYearToDateRecord(int? hqid)
            {
                HqHitterYearToDate player = await _context.HqHitterYearToDates.SingleOrDefaultAsync(h => h.HQID == hqid);
                return player;
            }

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> READ | ALL
            [HttpGet("read_ytd/all")]
            public List<HqHitterYearToDate> GetListOfAllYearToDateRecordsInDatabase()
            {
                _helpers.OpenMethod(1);
                List<HqHitterYearToDate> hqHitters = _context.HqHitterYearToDates.ToList();
                return hqHitters;
            }


            /* --------------------------------------------------------------- */
            /* CRUD - UPDATE - YEAR TO DATE                                    */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> UPDATE | ONE
            [HttpPut("update_ytd/{hitter}")]
            public async Task<IActionResult> UpdateOneYearToDateRecord(HqHitterYearToDate hitter)
            {
                if(hitter == null)
                {
                    return NotFound();
                }

                HqHitterYearToDate playerToUpdate = await _context.HqHitterYearToDates.FirstOrDefaultAsync(h => h.HQID == hitter.HQID);

                if(ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(playerToUpdate);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    catch(DbUpdateException /* ex */)
                    {
                        C.WriteLine("Database update error");
                    }
                }
                return Ok();
            }


            /* --------------------------------------------------------------- */
            /* CRUD - DELETE - YEAR TO DATE                                    */
            /* --------------------------------------------------------------- */

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> DELETE | ONE
            [HttpDelete("delete_ytd/{hqId}")]
            public async Task<IActionResult> DeleteOneYearToDateRecord(int hqId)
            {
                _helpers.OpenMethod(3);
                var hitter = await _context.HqHitterYearToDates.SingleOrDefaultAsync(h => h.HQID == hqId);
                _context.Remove(hitter);
                await _context.SaveChangesAsync();
                return Ok();
            }

            // STATUS [ August 13, 2019 ] : this works
            // CRUD -> DELETE | ALL
            [HttpDelete("delete_ytd/all")]
            public async Task<IActionResult> DeleteAllYearToDateRecords(List<HqHitterYearToDate> hitters)
            {
                _helpers.OpenMethod(3);
                foreach(var hitter in hitters)
                {
                    await DeleteOneYearToDateRecord(hitter.HQID);
                }
                return Ok();
            }


            // STATUS [ August 13, 2019 ] : this works
            // * Checks for number of items in database
            // * If number is greater than 0, returns TRUE
            // * Else, returns FALSE
            // * Example:
            // *    string todaysFileName = GetTodaysCsvFileName(_hitterYearToDateCsvFileNameBase);
            // *    var fileCheck = DoesCsvFileForTodayExist(todaysFileName);
            [ApiExplorerSettings(IgnoreApi = true)]
            public bool CheckIfThereAreYearToDateRecordsInDatabase()
            {
                _helpers.OpenMethod(1);
                List<HqHitterYearToDate> hqHitters = GetListOfAllYearToDateRecordsInDatabase();
                int count = hqHitters.Count;
                bool areThereRecordsInDatabase;

                if(count > 0)
                    areThereRecordsInDatabase = true;

                else
                    areThereRecordsInDatabase = false;

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
            [HttpGet("ros_proj")]
            public async Task DownloadHqHitterReport(string reportCssSelector, string downloadedCsvFileName, string fileNamePrefix, bool openFileAfterMove)
            {
                _helpers.OpenMethod(3);
                PrintDownloadedReportDetails(reportCssSelector, downloadedCsvFileName, fileNamePrefix);
                var page = await _hqUtilitiesController.CreateChromePage();
                await _hqUtilitiesController.LoginToBaseballHq(page);
                await _hqUtilitiesController.DownloadHqReport(page, reportCssSelector);
                await _hqUtilitiesController.MoveReportToHqFolder(openFileAfterMove, downloadedCsvFileName,BaseballHqHitterWriteDirectory, fileNamePrefix);
            }


            // STATUS [ August 13, 2019 ] : this works
            // * 'todaysReportName' elements
            // * 1) csvFileNameBase    e.g., -> HqHitterReport_PROJ_
            // * 2) todaysDateString   e.g., -> 8_12_2019
            // * 3) todaysReportName   e.g., -> HqHitterReport_PROJ_8_12_2019.csv
            [ApiExplorerSettings(IgnoreApi = true)]
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
            [ApiExplorerSettings(IgnoreApi = true)]
            public bool DoesCsvFileForTodayExist(string todaysReportName)
            {
                bool doesCsvFileForTodayExist = false;

                // string fullHqPathAndFileToCheck =               $"{_hqHitterTargetWriteFolderPath}{todaysReportName}";
                // string fullHqArchivePathAndFileToCheck =        $"{_hqHitterTargetWriteArchiveFolderPath}{todaysReportName}";
                // string fullHqPathAndFileToCheckCleaned =        $"{_hqHitterTargetWriteFolderPath}_{todaysReportName}";
                // string fullHqArchivePathAndFileToCheckCleaned = $"{_hqHitterTargetWriteArchiveFolderPath}_{todaysReportName}";
                string fullHqPathAndFileToCheck =               $"{BaseballHqHitterWriteDirectory}{todaysReportName}";
                string fullHqArchivePathAndFileToCheck =        $"{BaseballHqArchiveDirectory}{todaysReportName}";
                string fullHqPathAndFileToCheckCleaned =        $"{BaseballHqHitterWriteDirectory}_{todaysReportName}";
                string fullHqArchivePathAndFileToCheckCleaned = $"{BaseballHqArchiveDirectory}_{todaysReportName}";

                if(System.IO.File.Exists(fullHqPathAndFileToCheck))
                    doesCsvFileForTodayExist = true;

                else if(System.IO.File.Exists(fullHqArchivePathAndFileToCheck))
                    doesCsvFileForTodayExist = true;

                else if(System.IO.File.Exists(fullHqPathAndFileToCheckCleaned))
                    doesCsvFileForTodayExist = true;

                else if(System.IO.File.Exists(fullHqArchivePathAndFileToCheckCleaned))
                    doesCsvFileForTodayExist = true;

                else
                    C.WriteLine("DoesCsvFileForTodayExist(): FILE FOR TODAY DOES NOT EXIST");

                return doesCsvFileForTodayExist;
            }


        #endregion HELPERS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            [ApiExplorerSettings(IgnoreApi = true)]
            private void PrintDownloadedReportDetails(string reportCssSelector, string downloadedCsvFileName, string fileNamePrefix)
            {
                C.WriteLine($"\n-------------------------------------------------------------------");
                C.WriteLine($"CSS Selector: {reportCssSelector}");
                C.WriteLine($"Downloaded Csv FileName: {downloadedCsvFileName}");
                C.WriteLine($"File Name Prefix: {fileNamePrefix}");
                C.WriteLine($"-------------------------------------------------------------------\n");
            }

            [ApiExplorerSettings(IgnoreApi = true)]
            public void PrintDetailsOfTodaysYearToDateDatabaseUpdate(string todaysFileName, bool fileCheck, bool areThereRecordsInDatabase,List<HqHitterYearToDate> listHittersYTD)
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

        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
