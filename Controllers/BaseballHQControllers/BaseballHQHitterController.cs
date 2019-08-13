using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.BaseballHq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using C = System.Console;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.BaseballHQControllers
{
    [Route("api/hq/[controller]")]
    [ApiController]
    public class BaseballHQHitterController : ControllerBase
    {
        private readonly Helpers                  _helpers;
        private readonly CsvHandler               _csvHandler;
        private readonly BaseballHqUtilitiesController _hqUtilitiesController;
        private readonly BaseballScraperContext _context;


        public BaseballHQHitterController(Helpers helpers, BaseballHqUtilitiesController hqUtilitiesController, CsvHandler csvHandler, BaseballScraperContext context)
        {
            _helpers               = helpers;
            _hqUtilitiesController = hqUtilitiesController;
            _csvHandler            = csvHandler;
            _context               = context;

        }

        public BaseballHQHitterController() {}


        // Css selector used to identify link to click to download hitter report
        // Projections and YTD
        private readonly string _currentStatsAndProjectionsReportSelector = "table.stats-links:nth-child(5) > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(2) > a:nth-child(8)";

        // Projections
        private readonly string _hitterRestOfSeasonProjectionsReportSelector = "#node-384 > div > table:nth-child(5) > tbody > tr:nth-child(3) > td:nth-child(2) > a:nth-child(6)";

        // YTD
        private readonly string _hitterYearToDateReportSelector = "table.stats-links:nth-child(5) > tbody:nth-child(1) > tr:nth-child(3) > td:nth-child(2) > a:nth-child(9)";



        public readonly string _hqHitterTargetWriteFolderPath = "BaseballData/02_Target_Write/BaseballHQ_Target_Write/HqHitterFiles/";

        // All Reports
        private readonly string _csvFileNameBase = "HqHitterReport_";


        // YTD
        private readonly string _hitterYearToDateCsvFileIdentifier = "YTD_";

        // Projections
        private readonly string _hitterRestOfSeasonProjectionsCsvFileIdentifier = "PROJ_";


        // YTD
        // Returns: HqHitterReport_YTD_
        private string _hitterYearToDateCsvFileNameBase
        {
            get
            {
                return $"{_csvFileNameBase}{_hitterYearToDateCsvFileIdentifier}";
            }
            set {}
        }


        // Projections
        // Returns: HqHitterReport_PROJ_
        private string _hitterRestOfSeasonProjectionsCsvFileNameBase
        {
            get
            {
                return $"{_csvFileNameBase}{_hitterRestOfSeasonProjectionsCsvFileIdentifier}";

            }
            set {}
        }


        // Projections and YTD
        // Named by Baseball Hq when downloading to local downloads folder
        public string _hqYearToDateAndProjectionReportInitialCsvFileName
        {
            get
            {
                return "BaseballHQ_M_B_PY.csv";
            }
        }

        // Projections
        // Named by Baseball Hq when downloading to local downloads folder
        public string _hqRestOfSeasonProjectionReportInitialCsvFileName
        {
            get
            {
                return "BaseballHQ_M_B_P.csv";
            }
        }

        // YTD
        // Named by Baseball Hq when downloading to local downloads folder
        public string _hqYearToDateReportInitialCsvFileName
        {
            get
            {
                return "BaseballHQ_M_B_Y.csv";
            }
        }



        /*
            https://127.0.0.1:5001/api/hq/baseballhqhitter/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
        }


        // STATUS [ July 8, 2019 ] : this works
        /*
            https://127.0.0.1:5001/api/hq/baseballhqhitter/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();

            // await UpdateYearToDateRecord(4515);
            // var hittersYTD_list = GetListOfAllRecordsInDatabase();
            // foreach(var hitter in hittersYTD_list)
            // {
            //     await DeleteAllRecordsInTable(hitter.HqPlayerId);
            // }

            // string todaysYearToDateCsvFileName  = GetTodaysCsvFileName(_hitterYearToDateCsvFileNameBase);
            // C.WriteLine(todaysYearToDateCsvFileName);
            // var listObject = CreateListOfYearToDateObjectsFromCsvRows(todaysYearToDateCsvFileName);
            // var ytdList = CreateListOfHqHittersYTD(listObject);
            // await AddYearToDateRecordsToDatabase(ytdList);



            // string todaysCsvFileName = GetTodaysCsvFileName(_hitterRestOfSeasonProjectionsCsvFileNameBase);
            // C.WriteLine($"\nTESTING todaysCsvFileName: {todaysCsvFileName}");

            // var playerListObject = CreateListOfRestOfSeasonProjectionObjectsFromCsvRows(todaysCsvFileName);
            // var playerList = CreateListOfHqHittersRestOfSeasonProjections(playerListObject);
            // await AddRestOfSeasonProjectionsToDatabase(playerList);


        }




        #region DOWNLOAD REPORT FROM BASEBALL HQ ------------------------------------------------------------


            // PRIMARY METHOD
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
            public async Task DownloadHqHitterReport(string reportCssSelector, string downloadedCsvFileName, string fileNamePrefix)
            {
                PrintDownloadedReportDetails(reportCssSelector, downloadedCsvFileName, fileNamePrefix);
                var page = await _hqUtilitiesController.CreateChromePage();
                await _hqUtilitiesController.LoginToBaseballHq(page);
                await _hqUtilitiesController.DownloadHqReport(page, reportCssSelector);
                await _hqUtilitiesController.MoveReportToHqFolder(true, downloadedCsvFileName,_hqHitterTargetWriteFolderPath, fileNamePrefix);
            }

            public async Task DownloadHitterStatsAndProjectionsReport()
            {
                await DownloadHqHitterReport(
                    _currentStatsAndProjectionsReportSelector,
                    _hqYearToDateAndProjectionReportInitialCsvFileName,     // BaseballHQ_M_B_PY.csv
                    _csvFileNameBase);                                      // HqHitterReport_8_12_2019
            }

            public async Task DownloadRestOfSeasonProjections()
            {
                await DownloadHqHitterReport(
                    _hitterRestOfSeasonProjectionsReportSelector,
                    _hqRestOfSeasonProjectionReportInitialCsvFileName,      // BaseballHQ_M_B_P.csv
                    _hitterRestOfSeasonProjectionsCsvFileNameBase);         // HqHitterReport_PROJ_8_12_2019
            }


            public async Task DownloadYearToDateReport()
            {
                await DownloadHqHitterReport(
                    _hitterYearToDateReportSelector,
                    _hqYearToDateReportInitialCsvFileName,                  // BaseballHQ_M_B_Y.csv
                    _hitterYearToDateCsvFileNameBase);                      // HqHitterReport_YTD_
            }

        #endregion DOWNLOAD REPORT FROM BASEBALL HQ ------------------------------------------------------------





        #region HQ HITTERS ROS PROJECTIONS  ------------------------------------------------------------

            /* EXAMPLE OF THESE METHODS
                var playerListObject = CreateListOfRestOfSeasonProjectionObjectsFromCsvRows(todaysCsvFileName);
                var playerList = CreateListOfHqHittersRestOfSeasonProjections(playerListObject);
                await AddRestOfSeasonProjectionsToDatabase(playerList);
            */


            // STATUS [ August 12, 2019 ] : this works
            // STEP 1
            // 'csvFileName' is just the file name; no path included
            // csvFileName e.g., HqHitterReport_PROJ_8_12_2019.csv
            public IList<object> CreateListOfRestOfSeasonProjectionObjectsFromCsvRows(string csvFileName)
            {
                // C.WriteLine($"fileName: {fileName}");
                var playerRecordsList = _csvHandler.ReadCsvRecordsToList(
                    _hqHitterTargetWriteFolderPath,
                    csvFileName,
                    typeof(HqHitterRestOfSeasonProjection),
                    typeof(HqHitterRestOfSeasonProjectionClassMap),
                    true
                );
                return playerRecordsList;
            }


            // STATUS [ August 12, 2019 ] : this works
            // STEP 2
            public List<HqHitterRestOfSeasonProjection> CreateListOfHqHittersRestOfSeasonProjections(IList<object> playerRecordsList)
            {
                List<HqHitterRestOfSeasonProjection> listOfHitterRestOfSeasonProjections = new List<HqHitterRestOfSeasonProjection>();

                foreach(var playerObject in playerRecordsList)
                {
                    var player = playerObject as HqHitterRestOfSeasonProjection;
                    listOfHitterRestOfSeasonProjections.Add(player);
                }
                return listOfHitterRestOfSeasonProjections;
            }


            // STATUS [ August 12, 2019 ] : this works
            // STEP 3
            public async Task AddRestOfSeasonProjectionsToDatabase(List<HqHitterRestOfSeasonProjection> listOfHitterRestOfSeasonProjections)
            {
                foreach(var player in listOfHitterRestOfSeasonProjections)
                {
                    await _context.AddAsync(player);
                    await _context.SaveChangesAsync();
                }
            }


        #endregion HQ HITTERS ROS PROJECTIONS  ------------------------------------------------------------





        #region HQ HITTERS YEAR-TO-DATE (YTD)  ------------------------------------------------------------


            public async Task<IActionResult> UpdateDatabaseWithTodaysData(string csvFileName)
            {
                bool areThereRecordsInDatabase = CheckIfThereAreRecordsInDatabase();
                if(areThereRecordsInDatabase == true)
                {
                    List<HqHitterYearToDate> hqHitters = GetListOfAllRecordsInDatabase();
                    await DeleteAllRecords(hqHitters);
                }
                var playerListFromCsv = CreateListOfYearToDateObjectsFromCsvRows(csvFileName);
                var listHittersYTD = CreateListOfHqHittersYTD(playerListFromCsv);
                await AddYearToDateRecordsToDatabase(listHittersYTD);
                return Ok();
            }

            public IList<object> CreateListOfYearToDateObjectsFromCsvRows(string csvFileName)
            {
                // C.WriteLine($"fileName: {fileName}");
                var playerRecordsList = _csvHandler.ReadCsvRecordsToList(
                    _hqHitterTargetWriteFolderPath,
                    csvFileName,
                    typeof(HqHitterYearToDate),
                    typeof(HqHitterYearToDateClassMap),
                    true
                );
                return playerRecordsList;
            }


            public List<HqHitterYearToDate> CreateListOfHqHittersYTD(IList<object> playerRecordsList)
            {
                List<HqHitterYearToDate> listHittersYTD = new List<HqHitterYearToDate>();

                foreach(var playerObject in playerRecordsList)
                {
                    var player = playerObject as HqHitterYearToDate;
                    listHittersYTD.Add(player);
                }
                return listHittersYTD;
            }


            // CRUD -> CREATE | ONE
            public async Task<IActionResult> AddYearToDateRecordToDatabase(HqHitterYearToDate hitter)
            {
                await _context.AddAsync(hitter);
                await _context.SaveChangesAsync();
                return Ok();
            }


            // CRUD -> CREATE | MANY
            public async Task AddYearToDateRecordsToDatabase(List<HqHitterYearToDate> listOfHitterYearToDateRecords)
            {
                foreach(var player in listOfHitterYearToDateRecords)
                {
                    await _context.AddAsync(player);
                    await _context.SaveChangesAsync();
                }
            }


            // CRUD -> READ
            public async Task<HqHitterYearToDate> GetOneHqHitterYearToDate(int? hqid)
            {
                var player = await _context.BaseballHqHitterYTD.SingleOrDefaultAsync(h => h.HqPlayerId == hqid);
                return player;
            }


            // CRUD -> UPDATE
            public async Task<IActionResult> UpdateYearToDateRecord(HqHitterYearToDate hitter)
            {
                if(hitter == null)
                {
                    return NotFound();
                }

                var playerToUpdate = await _context.BaseballHqHitterYTD.FirstOrDefaultAsync(h => h.HqPlayerId == hitter.HqPlayerId);

                if(ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(playerToUpdate);
                        await _context.SaveChangesAsync();
                        return Content("DATABASE UPDATE DONE");
                    }
                    catch(DbUpdateException /* ex */)
                    {
                        C.WriteLine("Database update error");
                    }
                }
                return Content("DATABASE UPDATE DONE");
            }


            // CRUD -> DELETE | ONE
            public async Task<IActionResult> DeleteOneRecord(int hqId)
            {
                var hitter = await _context.BaseballHqHitterYTD.SingleOrDefaultAsync(h => h.HqPlayerId == hqId);
                _context.Remove(hitter);
                await _context.SaveChangesAsync();
                return Content("ALL RECORDS DELETED");
            }

            public async Task<IActionResult> DeleteAllRecords(List<HqHitterYearToDate> hitters)
            {
                foreach(var hitter in hitters)
                {
                    await DeleteOneRecord(hitter.HqPlayerId);
                }
                return Ok();
            }


            public List<HqHitterYearToDate> GetListOfAllRecordsInDatabase()
            {
                List<HqHitterYearToDate> hqHitters = _context.BaseballHqHitterYTD.ToList();
                return hqHitters;
            }

            public bool CheckIfThereAreRecordsInDatabase()
            {
                List<HqHitterYearToDate> hqHitters = GetListOfAllRecordsInDatabase();
                int count = hqHitters.Count;
                bool areThereRecordsInDatabase;
                if(count > 0)
                {
                    areThereRecordsInDatabase = true;
                }

                else
                {
                    areThereRecordsInDatabase = false;
                }
                return areThereRecordsInDatabase;
            }


        #endregion HQ HITTERS YEAR-TO-DATE (YTD)  ------------------------------------------------------------





        public string GetTodaysCsvFileName(string csvFileNameBase)
        {
            // e.g., HqHitterReport_PROJ_
            // C.WriteLine($"GetTodaysReportCsvName csvFileNameBase: {csvFileNameBase}");

            // e.g., 8_12_2019
            string todaysDateString = _csvHandler.TodaysDateString();
            // C.WriteLine($"GetTodaysReportCsvName todaysDateString: {todaysDateString}");

            // e.g., HqHitterReport_PROJ_8_12_2019.csv
            string todaysReportName = $"{csvFileNameBase}{todaysDateString}.csv";
            // C.WriteLine($"GetTodaysReportCsvName todaysReportName: {todaysReportName}");

            return todaysReportName;
        }





        #region PRINTING PRESS ------------------------------------------------------------

            private void PrintDownloadedReportDetails(string reportCssSelector, string downloadedCsvFileName, string fileNamePrefix)
            {
                C.WriteLine($"\n-------------------------------------------------------------------");
                C.WriteLine($"CSS Selector: {reportCssSelector}");
                C.WriteLine($"Downloaded Csv FileName: {downloadedCsvFileName}");
                C.WriteLine($"File Name Prefix: {fileNamePrefix}");
                C.WriteLine($"-------------------------------------------------------------------\n");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}




// string todaysReportName = $"{_hitterReportBaseFileName}{todaysDateString}";



// var page = await _hqUtilitiesController.CreateChromePage();
// await _hqUtilitiesController.LoginToBaseballHq(page);
// await _hqUtilitiesController.DownloadHqReport(page, _currentStatsAndProjectionsReportSelector);
// await _hqUtilitiesController.MoveReportToHqFolder(true, _hitterReportBaseFileName);
