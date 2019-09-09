using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BaseballScraper.EndPoints.BaseballSavantUriEndPoints;
using Microsoft.AspNetCore.Mvc;
using C = System.Console;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.BaseballSavant;


#pragma warning disable CS0219, CS0414, CS1998, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.BaseballSavantControllers
{
    [Route("api/baseballsavant/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BaseballSavantSpController: ControllerBase
    {
        private readonly Helpers                        _helpers;
        private readonly DataTabler                     _dataTabler;
        private readonly BaseballScraperContext         _context;
        private readonly BaseballSavantPitcherEndPoints _baseballSavantEndPoints;
        private readonly CsvHandler                     _csvHandler;
        private readonly ProjectDirectoryEndPoints      _projectDirectory;

        public System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();




        public BaseballSavantSpController(Helpers helpers, BaseballScraperContext context, DataTabler dataTabler, BaseballSavantPitcherEndPoints baseballSavantEndPoints, CsvHandler csvHandler, ProjectDirectoryEndPoints projectDirectory)
        {
            _helpers                 = helpers;
            _context                 = context;
            _dataTabler              = dataTabler;
            _baseballSavantEndPoints = baseballSavantEndPoints;
            _csvHandler              = csvHandler;
            _projectDirectory        = projectDirectory;
        }

        public BaseballSavantSpController(){}




        // BaseballData/02_WRITE/BASEBALL_SAVANT/PITCHERS/
        private string PitcherWriteDirectory
        {
            get => _projectDirectory.BaseballSavantPitcherWriteRelativePath;
        }

        // BaseballData/02_WRITE/BASEBALL_SAVANT/_archive
        private string ArchiveDirectory
        {
            get => _projectDirectory.BaseballSavantArchiveDirectory;
        }

        private string _allSpCswSingleDayCvs
        {
            get => "Bs_AllSpSingleDayCsw";
        }

        private string _allSpCswSingleDayCvsPath
        {
            get => $"{PitcherWriteDirectory}{_allSpCswSingleDayCvs}";
        }

        private static string _allSpCswDateRangeCvs
        {
            get => "Bs_SpCswDateRange";
        }

        private string _allSpCswDateRangeCvsPath
        {
            get => $"{PitcherWriteDirectory}{_allSpCswDateRangeCvs}";
        }

        private List<dynamic> _dynamicList = new List<dynamic>();



        /*
            https://127.0.0.1:5001/api/baseballsavant/BsSp/test
        */
        [Route("test")]
        public void BaseballSavantTesting()
        {
            _helpers.StartMethod();
        }


        /*
            https://127.0.0.1:5001/api/baseballsavant/BsSp/test/async
        */
        [Route("test/async")]
        public async Task BaseballSavantTestingAsync()
        {
            await WriteBaseballSavantDataForYesterdayToCsv();
        }


        public async Task<IActionResult> DAILY_REPORT_RUNNER()
        {
            _helpers.OpenMethod(3);

            int year = 2019;
            int currentMonth = 8;
            int currentDay = DateTime.Now.Day;

            #region FULL SEASON
                Archive(PitcherWriteDirectory, ArchiveDirectory);
                WriteBaseballSavantFullSeasonData(year);
                var cswFullSeason = await ReadSpCswCsvFullSeasonAsync(year);
                AddStartingPitcherCswsFullSeasonToDatabaseFromList(cswFullSeason);
            #endregion FULL SEASON

            // #region SINGLE DAY
            // Archive(PitcherWriteDirectory, ArchiveDirectory);
            // WriteBaseballSavantSingleDayDataToCsv(year, currentMonth, currentDay - 1);
            // var cswSingleDay  = await ReadSpCswCsvSingleDayAsync(currentMonth, currentDay - 1, year);
            // AddStartingPitcherCswsSingleDayToDatabaseFromList(cswSingleDay);
            // #endregion SINGLE DAY




            // WriteBaseballSavantDateRangeDataToCsv(year, currentMonth, 2, currentMonth, currentDay);
            // var cswDateRange  = await ReadSpCswCsvDateRangeAsync(year, currentMonth, 2, currentMonth, currentDay);
            // AddStartingPitcherCswsDateRangeToDatabaseFromList(cswDateRange);

            return Ok();
        }


        public void Archive(string archiveFromDirectory, string archiveToDirectory)
        {
            _helpers.OpenMethod(1);
            C.WriteLine($"Archive From Directory : {archiveFromDirectory}");
            C.WriteLine($"Archive To Directory   : {archiveToDirectory}");
            _csvHandler.MoveMultipleFiles(archiveFromDirectory, archiveToDirectory);
        }






        public async Task WriteBaseballSavantDataForYesterdayToCsv()
        {
            // get date information for yesterday
            DateTime yesterday = DateTime.Today.AddDays(-1);

            WriteBaseballSavantSingleDayDataToCsv(
                yesterday.Year,
                yesterday.Month,
                yesterday.Day
            );

            var spCswList = await ReadSpCswCsvSingleDayAsync(
                yesterday.Month,
                yesterday.Day,
                yesterday.Year
            );
            PrintStartingPitcherCswSingleDay(spCswList);
        }


        // 3 Paths
        // * 1) SINGLE DAY CSW DATA
        //      > void Write to CSV
        //      > string Get endpoint
        //      > string Set file path
        //      > async Read list
        //      > POST Add one
        //      > POST Add all
        //      > List Create list
        // * 2) DATE RANGE CSW DATA
        //      > void Write to CSV
        //      > string Get endpoint
        //      > string Set file path
        //      > async Read list
        //      > POST Add one
        //      > POST Add all
        //      > List Create list
        // * 3) FULL SEASON CSW DATA
        //      > void Write to CSV
        //      > string Get endpoint
        //      > string Set file path
        //      > async Read list
        //      > POST Add one
        //      > POST Add all
        //      > List Create list




        // PATH 1: SINGLE DAY CSW DATA
        #region SP CALLED-STRIKE + WALK DATA - SINGLE DAY ------------------------------------------------------------


            // STATUS [ June 5, 2019 ]: this works
            // SINGLE DAY
            /// <summary>
            ///     Write SP CSW data from Baseball Savant to CSV File
            ///     Uses 'GetAllSpCswSingleDayEndPointUri' end point
            /// </summary>
            /// <remarks>
            ///     If the file already exists, it will not create a new file
            /// </remarks>
            /// <param name="year">
            ///     ints that represents the year you want to get data for and write to a CSV
            /// </param>
            /// <param name="monthNumber">
            ///     ints that represents the month number you want to get data for and write to a CSV
            /// </param>
            /// <param name="dayNumber">
            ///     ints that represents the day number you want to get data for and write to a CSV
            /// </param>
            public void WriteBaseballSavantSingleDayDataToCsv(int year, int monthNumber, int dayNumber)
            {
                _helpers.OpenMethod(1);
                bool doesCsvFileExist = false;
                var endPointUri = GetAllSpCswSingleDayEndPointUri(
                    year,
                    monthNumber,
                    dayNumber
                );

                var targetCsvString = SetCsvFilePathSpCswSingleDay(
                    year,
                    monthNumber,
                    dayNumber
                );

                // check if file exists
                if(System.IO.File.Exists(targetCsvString))
                    doesCsvFileExist = true;

                // file does not exist
                else
                    DownloadSpCswCvsAndWriteToLocalCsv(endPointUri, $"{targetCsvString}");
            }


            // STATUS [ June 5, 2019 ]: this works
            // SINGLE DAY
            /// <summary>
            ///     Create end point for 'WriteBaseballSavantSingleDayDataToCsv' method to write to
            /// </summary>
            public string GetAllSpCswSingleDayEndPointUri(int year, int monthNumber, int dayNumber)
            {
                _helpers.OpenMethod(1);
                BaseballSavantUriEndPoint endPoint = _baseballSavantEndPoints.AllSpCswSingleDay(
                    year,
                    monthNumber,
                    dayNumber
                );
                return endPoint.EndPointUri;
            }


            // STATUS [ June 5, 2019 ]: this works
            // SINGLE DAY
            /// <summary>
            ///     Creates a string of the fill path name
            /// </summary>
            public string SetCsvFilePathSpCswSingleDay(int year, int monthNumber, int dayNumber)
            {
                _helpers.OpenMethod(1);
                string filePath = $"{_allSpCswSingleDayCvsPath}{monthNumber}_{dayNumber}_{year}.csv";
                return filePath;
            }


            // STATUS [ June 5, 2019 ]: this works
            // SINGLE DAY
            /// <summary>
            ///     Read CSV File of 'StartingPitcherCswSingleDay' instances
            ///     The file includes SP CSW(called strike + whiffs) for pitchers that pitched on a certain day
            /// </summary>
            /// <remarks>
            ///     * This reads a given file in BaseballData/02_Target_Write/BaseballSavant_Target_Write project folder
            ///     * If a file does not exist for the date entered, you'll get an error
            ///     * Uses CsvHandler from project Infrastructure to read the file
            ///     * 'csvFilePath' will be something like:
            ///         BaseballData/02_Target_Write/BaseballSavant_Target_Write/Bs_AllSpSingleDayCsw5_29_2019.csv
            ///     * file name example: Bs_AllSpSingleDayCsw5_29_2019.csv
            ///     * Printer --> PrintBsSpCswDetail(_dynamicList);
            /// </remarks>
            /// <param name="monthNumber">
            ///     ints that represents the month number you want to get data for and write to a CSV
            /// </param>
            /// <param name="dayNumber">
            ///     ints that represents the day number you want to get data for and write to a CSV
            /// </param>
            /// <param name="year">
            ///     ints that represents the year you want to get data for and write to a CSV
            /// </param>
            /// <example>
            ///     var spCswList = await ReadSpCswCsvSingleDayAsync(5,29,2019);
            /// </example>
            /// <returns>
            ///     A list of StartingPitcherCswSingleDay
            /// </returns>
            public async Task<List<StartingPitcherCswSingleDay>> ReadSpCswCsvSingleDayAsync(int monthNumber, int dayNumber, int year)
            {
                _helpers.OpenMethod(1);

                var csvFilePath = SetCsvFilePathSpCswSingleDay(
                    year,
                    monthNumber,
                    dayNumber
                );

                await _csvHandler.ReadCsvRecordsAsyncToList(
                    csvFilePath,
                    typeof(StartingPitcherCswSingleDay),
                    typeof(StartingPitcherCswClassMap),
                    _dynamicList
                );

                var listSpCswSingleDay = CreateSpCswSingleDayList(
                    _dynamicList,
                    year,
                    monthNumber,
                    dayNumber
                );
                return listSpCswSingleDay;
            }



            // STATUS [ June 5, 2019 ]: this works
            // SINGLE DAY
            /// <summary>
            ///     Add StartingPitcherCswSingleDay to Database
            /// </summary>
            [HttpPost("add/one/csw_single_day")]
            public ActionResult AddStartingPitcherCswSingleDayToDatabase(StartingPitcherCswSingleDay spCsw)
            {
                _helpers.OpenMethod(1);
                _context.Add(spCsw);
                _context.SaveChanges();
                return Ok(spCsw);
            }


            // STATUS [ June 5, 2019 ]: this works
            // SINGLE DAY
            /// <summary>
            ///     Add StartingPitcherCswSingleDay to Database from list
            /// </summary>
            [HttpPost("add/all/csw_single_day")]
            public ActionResult AddStartingPitcherCswsSingleDayToDatabaseFromList(List<StartingPitcherCswSingleDay> listSpCsw)
            {
                _helpers.OpenMethod(1);
                int countAdded = 0; int countNotAdded = 0;

                foreach(var sp in listSpCsw)
                {
                    var checkForCompositeKeyInDb =
                        _context.StartingPitcherCswsSingleDays.Find(
                            sp.PlayerId,
                            sp.DatePitched
                        );

                     var nullCheck      = (checkForCompositeKeyInDb == null) ? _context.Add(sp) : null;
                     int manageCounters = (checkForCompositeKeyInDb == null) ? countAdded++ : countNotAdded++;
                }

                PrintDatabaseAddOutcomes(countAdded, countNotAdded);
                _context.SaveChanges();
                return Ok();
            }


            // STATUS [ June 5, 2019 ]: this works
            // SINGLE DAY
            /// <summary>
            ///     Create list of StartingPitcherCswSingleDay
            /// </summary>
            public List<StartingPitcherCswSingleDay> CreateSpCswSingleDayList(List<dynamic> list, int year, int monthNumber, int dayNumber)
            {
                _helpers.OpenMethod(1);

                var listSpCsw = new List<StartingPitcherCswSingleDay>();
                DateTime datePitched = new DateTime(year, monthNumber, dayNumber);

                foreach(dynamic record in list)
                {
                    var spCsw = record as StartingPitcherCswSingleDay;
                    spCsw.DatePitched = datePitched;
                    listSpCsw.Add(spCsw);
                }
                return listSpCsw;
            }


        #endregion SP CALLED-STRIKE + WALK DATA - SINGLE DAY ------------------------------------------------------------





        // PATH 2: DATE RANGE CSW DATA
        #region SP CALLED-STRIKE + WALK DATA - DATE RANGE ------------------------------------------------------------


            // STATUS [ June 5, 2019 ]: this works
            // DATE RANGE
            /// <summary>
            ///     Write SP CSW data from Baseball Savant to CSV File
            ///     Uses 'GetAllSpCswRangeOfDaysEndPointUri' end point
            /// </summary>
            /// <param name="year">
            ///     ints that represents the year you want to get data for and write to a CSV
            /// </param>
            /// <param name="startMonth">
            ///     ints that represents the start month number you want to get data for and write to a CSV
            /// </param>
            /// <param name="startDay">
            ///     ints that represents the start day number you want to get data for and write to a CSV
            /// </param>
            /// <param name="endMonth">
            ///     ints that represents the end month number you want to get data for and write to a CSV
            /// </param>
            /// <param name="endDay">
            ///     ints that represents the end day number you want to get data for and write to a CSV
            /// </param>
            public void WriteBaseballSavantDateRangeDataToCsv(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                _helpers.OpenMethod(1);
                var endPointUri = GetAllSpCswRangeOfDaysEndPointUri(
                    year,
                    startMonth,
                    startDay,
                    endMonth,
                    endDay
                );

                var targetCsvString = SetCsvFilePathSpCswDateRange(
                    year,
                    startMonth,
                    startDay,
                    endMonth,
                    endDay
                );

                DownloadSpCswCvsAndWriteToLocalCsv(endPointUri, $"{targetCsvString}");
            }


            // STATUS [ June 5, 2019 ]: this works
            // DATE RANGE
            /// <summary>
            ///     Create end point for 'WriteBaseballSavantDateRangeDataToCsv' method to write to
            /// </summary>
            public string GetAllSpCswRangeOfDaysEndPointUri(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                _helpers.OpenMethod(1);
                BaseballSavantUriEndPoint endPoint = _baseballSavantEndPoints.AllSpCswRangeOfDays(
                    year,
                    startMonth,
                    startDay,
                    endMonth,
                    endDay
                );
                return endPoint.EndPointUri;
            }


            // STATUS [ June 5, 2019 ]: this works
            // DATE RANGE
            /// <summary>
            ///     Creates a string of the fill path name
            /// </summary>
            public string SetCsvFilePathSpCswDateRange(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                _helpers.OpenMethod(1);
                string filePath = $"{_allSpCswDateRangeCvsPath}{startMonth}_{startDay}_{year}_to_{endMonth}_{endDay}_{year}.csv";
                return filePath;
            }


            // STATUS [ June 25, 2019 ]: this works
            // DATE RANGE
            /// <summary>
            ///     Read CSV File of 'StartingPitcherCswFullSeason' instances from day X to day Y
            ///     The file includes SP CSW(called strike + whiffs) for pitchers in that date range
            /// </summary>
            /// <remarks>
            ///     * This reads a given file in BaseballData/02_Target_Write/BaseballSavant_Target_Write project folder
            ///     * If a file does not exist for the date range entered, you'll get an error
            ///     * Uses CsvHandler from project Infrastructure to read the file
            ///     * 'csvFilePath' will be something like:
            ///         BaseballData/02_Target_Write/BaseballSavant_Target_Write/Bs_SpCswDateRange5_1_2019_to_5_15_2019.csv
            ///     * file name example: Bs_SpCswDateRange5_1_2019_to_5_15_2019.csv
            /// </remarks>
            /// <param name="year">
            ///     ints that represents the year you want to get data for and write to a CSV
            /// </param>
            /// <param name="startMonth">
            ///     ints that represents the start month number you want to get data for and write to a CSV
            /// </param>
            /// <param name="startDay">
            ///     ints that represents the start day number you want to get data for and write to a CSV
            /// </param>
            /// <param name="endMonth">
            ///     ints that represents the end month number you want to get data for and write to a CSV
            /// </param>
            /// <param name="endDay">
            ///     ints that represents the end day number you want to get data for and write to a CSV
            /// </param>
            /// <example>
            ///     var spCswList = await ReadSpCswCsvDateRangeAsync(2019, 5, 1, 5, 15);
            /// </example>
            /// <returns>
            ///     A list of StartingPitcherCswDateRange
            /// </returns>
            public async Task<List<StartingPitcherCswDateRange>> ReadSpCswCsvDateRangeAsync(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                _helpers.OpenMethod(3);
                var csvFilePath = SetCsvFilePathSpCswDateRange(
                    year,
                    startMonth,
                    startDay,
                    endMonth,
                    endDay
                );

                await _csvHandler.ReadCsvRecordsAsyncToList(
                    csvFilePath,
                    typeof(StartingPitcherCswDateRange),
                    typeof(StartingPitcherCswClassMap),
                    _dynamicList
                );

                var listSpCswDateRange = CreateSpCswDateRangeList(
                    _dynamicList,
                    year,
                    startMonth,
                    startDay,
                    endMonth,
                    endDay
                );

                return listSpCswDateRange;
            }


            // STATUS [ June 5, 2019 ]: this works
            // DATE RANGE
            /// <summary>
            ///     Add StartingPitcherCswDateRange to Database
            /// </summary>
            [HttpPost("add/one/csw_date_range")]
            public ActionResult AddStartingPitcherCswDateRangeToDatabase(StartingPitcherCswDateRange spCsw)
            {
                _helpers.OpenMethod(1);
                _context.Add(spCsw);
                _context.SaveChanges();
                return Ok(spCsw);
            }


            // STATUS [ June 5, 2019 ]: this works
            // DATE RANGE
            /// <summary>
            ///     Add StartingPitcherCswDateRange to Database from list
            /// </summary>
            [HttpPost("add/all/csw_date_range")]
            public ActionResult AddStartingPitcherCswsDateRangeToDatabaseFromList(List<StartingPitcherCswDateRange> listSpCsw)
            {
                _helpers.OpenMethod(1);
                int countAdded = 0; int countNotAdded = 0;

                foreach(var sp in listSpCsw)
                {
                    var checkForCompositeKeyInDb =
                        _context.StartingPitcherCswsFullSeason.Find(
                            sp.PlayerId,
                            sp.StartDate,
                            sp.EndDate
                        );

                    var nullCheck      = (checkForCompositeKeyInDb == null) ? _context.Add(sp) : null;
                    int manageCounters = (checkForCompositeKeyInDb == null) ? countAdded++ : countNotAdded++;
                }

                PrintDatabaseAddOutcomes(countAdded, countNotAdded);
                _context.SaveChanges();
                return Ok();
            }


            // STATUS [ June 5, 2019 ]: this works
            // DATE RANGE
            /// <summary>
            ///     Create list of StartingPitcherCswDateRange
            /// </summary>
            public List<StartingPitcherCswDateRange> CreateSpCswDateRangeList(List<dynamic> list, int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                _helpers.OpenMethod(1);

                var listSpCsw = new List<StartingPitcherCswDateRange>();

                DateTime startDate = new DateTime(year, startMonth, startDay);
                DateTime endDate   = new DateTime(year, endMonth, endDay);

                foreach(dynamic record in list)
                {
                    var spCsw = record as StartingPitcherCswDateRange;
                    spCsw.StartDate = startDate;
                    spCsw.EndDate   = endDate;
                    listSpCsw.Add(spCsw);
                }
                return listSpCsw;
            }


        #endregion SP CALLED-STRIKE + WALK DATA - DATE RANGE ------------------------------------------------------------





        // OPTION 3: FULL SEASON CSW DATA
        #region SP CALLED-STRIKE + WALK DATA - FULL SEASON ------------------------------------------------------------


            // STATUS [ June 5, 2019 ]: this works
            // FULL SEASON
            /// <summary>
            ///     Write SP CSW data from Baseball Savant to CSV File
            ///     Uses 'GetAllSpCswFullSeasonEndPointUri' end point
            /// </summary>
            /// <param name="year">
            ///     The year / season you want to search for
            /// </param>
            public void WriteBaseballSavantFullSeasonData(int year)
            {
                _helpers.OpenMethod(1);

                var endPointUri     = GetAllSpCswFullSeasonEndPointUri(year);
                var targetCsvString = SetCsvFilePathSpCswFulLSeason(year);

                DownloadSpCswCvsAndWriteToLocalCsv(endPointUri, $"{targetCsvString}");
            }


            // STATUS [ June 5, 2019 ]: this works
            // FULL SEASON
            /// <summary>
            ///     Create end point for 'WriteBaseballSavantFullSeasonData' method to write to
            /// </summary>
            public string GetAllSpCswFullSeasonEndPointUri(int year)
            {
                _helpers.OpenMethod(1);
                BaseballSavantUriEndPoint endPoint = _baseballSavantEndPoints.AllSpCswFullSeason(year);
                return endPoint.EndPointUri;
            }


            // STATUS [ June 5, 2019 ]: this works
            // FULL SEASON
            /// <summary>
            ///     Creates a string of the fill path name
            /// </summary>
            public string SetCsvFilePathSpCswFulLSeason(int year)
            {
                _helpers.OpenMethod(1);
                string filePath = $"{_allSpCswDateRangeCvsPath}_FULL_SEASON_{year}.csv";
                return filePath;
            }


            // STATUS [ June 25, 2019 ]: this works
            // FULL SEASON
            /// <summary>
            ///     Read CSV File of 'StartingPitcherCsw' instances for a year / season
            ///     The file includes SP CSW(called strike + whiffs) for pitchers in that season
            /// </summary>
            /// <remarks>
            ///     * This reads a given file in BaseballData/02_Target_Write/BaseballSavant_Target_Write project folder
            ///     * If a file does not exist for the season / year entered, you'll get an error
            ///     * Uses CsvHandler from project Infrastructure to read the file
            ///     * 'csvFilePath' will be something like:
            ///         BaseballData/02_Target_Write/BaseballSavant_Target_Write/Bs_SpCswDateRange_FULL_SEASON_2019.csv
            ///     * file name example: Bs_SpCswDateRange_FULL_SEASON_2019.csv
            /// </remarks>
            /// <param name="year">
            ///     The year / season you want to search for
            /// </param>
            /// <example>
            ///     var spCswList = await ReadSpCswCsvFullSeasonAsync(2019)
            /// </example>
            /// <returns>
            ///     A list of StartingPitcherCsw
            /// </returns>
            public async Task<List<StartingPitcherCswFullSeason>> ReadSpCswCsvFullSeasonAsync(int year)
            {
                _helpers.OpenMethod(3);
                string csvFilePath = SetCsvFilePathSpCswFulLSeason(year);

                await _csvHandler.ReadCsvRecordsAsyncToList(
                    csvFilePath,
                    typeof(StartingPitcherCswFullSeason),
                    typeof(StartingPitcherCswClassMap),
                    _dynamicList
                );

                var listSpCsw = CreateSpCswFullSeasonList(
                    _dynamicList,
                    year
                );

                return listSpCsw;
            }


            // STATUS [ June 5, 2019 ]: this works
            // FULL SEASON
            /// <summary>
            ///     Add StartingPitcherCsw to Database
            /// </summary>
            [HttpPost("add/one/csw_season")]
            public ActionResult AddStartingPitcherCswFullSeasonToDatabase(StartingPitcherCswFullSeason spCsw)
            {
                _helpers.OpenMethod(1);
                _context.Add(spCsw);
                _context.SaveChanges();
                return Ok(spCsw);
            }


            // STATUS [ June 5, 2019 ]: this works
            // FULL SEASON
            /// <summary>
            ///     Add StartingPitcherCsw to Database from list
            /// </summary>
            [HttpPost("add/all/csw_season")]
            public ActionResult AddStartingPitcherCswsFullSeasonToDatabaseFromList(List<StartingPitcherCswFullSeason> listSpCsw)
            {
                _helpers.OpenMethod(1);

                int countAdded = 0; int countNotAdded = 0;
                foreach(var sp in listSpCsw)
                {
                    var checkForCompositeKeyInDb =
                        _context.StartingPitcherCswsFullSeason.Find(
                            sp.PlayerId,
                            sp.Season
                        );

                    var nullCheck      = (checkForCompositeKeyInDb == null) ? _context.Add(sp) : null;
                    int manageCounters = (checkForCompositeKeyInDb == null) ? countAdded++ : countNotAdded++;
                }

                PrintDatabaseAddOutcomes(countAdded, countNotAdded);
                _context.SaveChanges();
                return Ok();
            }


            // STATUS [ June 5, 2019 ]: this works
            // FULL SEASON
            /// <summary>
            ///     Create list of StartingPitcherCsw
            /// </summary>
            public List<StartingPitcherCswFullSeason> CreateSpCswFullSeasonList(List<dynamic> list, int year)
            {
                _helpers.OpenMethod(1);
                var listSpCsw = new List<StartingPitcherCswFullSeason>();
                foreach(dynamic record in list)
                {
                    var spCsw = record as StartingPitcherCswFullSeason;
                    spCsw.Season = year;
                    listSpCsw.Add(spCsw);
                }
                return listSpCsw;
            }


        #endregion SP CALLED-STRIKE + WALK DATA - FULL SEASON ------------------------------------------------------------





        #region GENERAL HELPERS ------------------------------------------------------------

            // STATUS [ June 5, 2019 ]: this works
            public void DownloadSpCswCvsAndWriteToLocalCsv(string endPointUri, string targetCsvString)
            {
                _helpers.OpenMethod(1);
                _csvHandler.DownloadCsvFromLink(endPointUri, targetCsvString);
            }

        #endregion GENERAL HELPERS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------


            public void PrintDatabaseAddOutcomes(int countAdded, int countNotAdded)
            {
                C.WriteLine($"\n-------------------------------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(BaseballSavantSpController));
                C.WriteLine($"ADDED TO DB   : {countAdded}");
                C.WriteLine($"ALREADY IN DB : {countNotAdded}");
                C.WriteLine($"-------------------------------------------------------------------\n");
            }


            public void PrintBsSpCswDetail(List<dynamic> list)
            {
                int count = 1;
                foreach(var record in list)
                {
                    var spCsw = record as StartingPitcherCsw;

                    // Console.WriteLine($"{spCsw.DatePitched}");

                    C.ForegroundColor = ConsoleColor.Magenta;
                    C.WriteLine($"\n---------------------------------------------------------");
                    C.WriteLine($"{count}. PLAYER: {spCsw.PlayerName}  ID: {spCsw.PlayerId}");
                    C.WriteLine($"--------------------------------------------------------");
                    C.ResetColor();
                    C.WriteLine($"TOTAL PITCHES    | {spCsw.TotalPitches}");
                    C.WriteLine($"CSW PITCHES      | {spCsw.CswPitches}");
                    C.WriteLine($"CSW PITCH %      | {spCsw.CswPitchPercent}");
                    C.WriteLine($"AT BATS          | {spCsw.Abs}");
                    C.WriteLine($"SPIN RATE        | {spCsw.SpinRate}");
                    C.WriteLine($"VELOCITY         | {spCsw.Velocity}");
                    C.WriteLine($"EFFECTIVE SPEED  | {spCsw.EffectiveSpeed}");
                    C.WriteLine($"WHIFFS           | {spCsw.Whiffs}\n");
                    C.WriteLine($"--------------------------------------------------------\n");


                    count++;
                }
            }

            public void PrintStartingPitcherCswSingleDay(List<StartingPitcherCswSingleDay> spCswList)
            {
                // type = IOrderedEnumerable<StartingPitcherCswSingleDay> sortedList;
                var sortedlist = from element in spCswList
                        orderby element.CswPitchPercent descending
                        select  element;

                string[] tableHeaders =
                {
                    "Name",
                    "Pitches",
                    "CswPitches",
                    "CSW%"
                };

                var dataTable = _dataTabler.CreateDataTableWithCustomHeaders("CSW PITCHERS", tableHeaders);

                foreach(var pitcher in sortedlist)
                {
                    object[] pitcherData =
                    {
                        pitcher.PlayerName,
                        pitcher.TotalPitches,
                        pitcher.CswPitches,
                        pitcher.CswPitchPercent
                    };
                    _dataTabler.InsertDataRowIntoTable(dataTable, pitcherData);
                }
                _dataTabler.PrintTable(dataTable);
            }

        #endregion PRINTING PRESS ------------------------------------------------------------

    }
}



// Console.WriteLine($"{spCsw.Swings}");
// Console.WriteLine($"{spCsw.Takes}");
// Console.WriteLine($"{spCsw.EffectiveMinVelocity}");
// Console.WriteLine($"{spCsw.ReleaseExtension}");

// Console.WriteLine($"{spCsw.Pos3IntStartDistance}");
// Console.WriteLine($"{spCsw.Pos4IntStartDistance}");
// Console.WriteLine($"{spCsw.Pos5IntStartDistance}");
// Console.WriteLine($"{spCsw.Pos6IntStartDistance}");
// Console.WriteLine($"{spCsw.Pos7IntStartDistance}");
// Console.WriteLine($"{spCsw.Pos8IntStartDistance}");
// Console.WriteLine($"{spCsw.Pos9IntStartDistance}");
