using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.BaseballSavant;
using Microsoft.AspNetCore.Mvc;
using static BaseballScraper.EndPoints.BaseballSavantUriEndPoints;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.BaseballSavantControllers
{
    [Route("api/baseballsavant/[controller]")]
    [ApiController]
    public class BsSpController: ControllerBase
    {
        private static readonly Helpers _h = new Helpers();

        // injected in Startups.cs as 'services.AddSingleton<DataTabler>();'
        private readonly DataTabler _dataTabler;

        private readonly BaseballScraperContext _context;


        private static readonly BaseballSavantUriEndPoints _bsEndPoints = new BaseballSavantUriEndPoints();

        private readonly CsvHandler _csvH = new CsvHandler();

        private static string _targetWriteFolder = "BaseballData/02_Target_Write/BaseballSavant_Target_Write/";

        private static string _allSpCswSingleDayCvs = "Bs_AllSpSingleDayCsw";

        private static string _allSpCswSingleDayCvsPath = $"{_targetWriteFolder}{_allSpCswSingleDayCvs}";

        private static string _allSpCswDateRangeCvs = "Bs_SpCswDateRange";

        private static string _allSpCswDateRangeCvsPath = $"{_targetWriteFolder}{_allSpCswDateRangeCvs}";

        private Type _spCswTypeSingleDay = new StartingPitcherCswSingleDay().GetType();
        private Type _spCswModelMapTypeSingleDay = new StartingPitcherCswClassMap().GetType();
        private List<dynamic> _dynamicList = new List<dynamic>();
        private List<StartingPitcherCswSingleDay> _listSpCswSingleDay = new List<StartingPitcherCswSingleDay>();


        private Type _spCswTypeDateRange = new StartingPitcherCswDateRange().GetType();
        private Type _spCswModelMapTypeDateRange = new StartingPitcherCswClassMap().GetType();
        private List<StartingPitcherCswDateRange> _listSpCswDateRange = new List<StartingPitcherCswDateRange>();


        private Type _spCswType = new StartingPitcherCsw().GetType();
        private Type _spCswModelMapType = new StartingPitcherCswClassMap().GetType();
        private List<StartingPitcherCsw> _listSpCsw = new List<StartingPitcherCsw>();




        public BsSpController(BaseballScraperContext context, DataTabler dataTabler)
        {
            _context = context;
            _dataTabler = dataTabler;
        }


        // https://127.0.0.1:5001/api/baseballsavant/BsSp/test
        [Route("test")]
        public void BaseballSavantTesting()
        {
            _h.StartMethod();
        }


        // https://127.0.0.1:5001/api/baseballsavant/BsSp/test/async
        [Route("test/async")]
        public async Task BaseballSavantTestingAsync()
        {
            // var spCswList = await ReadSpCswCsvSingleDayAsync(6,24,2019);
            await WriteBaseballSavantDataForYesterdayToCsv();
        }




            public async Task WriteBaseballSavantDataForYesterdayToCsv()
            {
                // get date information for yesterday
                DateTime yesterday = DateTime.Today.AddDays(-1);
                WriteBaseballSavantSingleDayDataToCsv(yesterday.Year, yesterday.Month, yesterday.Day);
                var spCswList = await ReadSpCswCsvSingleDayAsync(yesterday.Month, yesterday.Day, yesterday.Year);
                PrintStartingPitcherCswSingleDay(spCswList);
            }


            public void PrintStartingPitcherCswSingleDay(List<StartingPitcherCswSingleDay> spCswList)
            {
                var sortedlist = from element in spCswList
                        orderby element.CswPitchPercent descending
                        select element;

                string[] tableHeaders = { "Name", "Pitches", "CswPitches", "CSW%" };
                var dataTable = _dataTabler.CreateDataTableWithCustomHeaders("CSW PITCHERS", tableHeaders);

                foreach(var pitcher in sortedlist)
                {
                    Object[] pitcherData = { pitcher.PlayerName, pitcher.TotalPitches, pitcher.CswPitches, pitcher.CswPitchPercent };
                    _dataTabler.InsertDataRowIntoTable(dataTable, pitcherData);
                }

                _dataTabler.PrintTable(dataTable);
            }



        // OPTION 1: SINGLE DAY CSW DATA
        #region SP CALLED-STRIKE + WALK DATA - SINGLE DAY ------------------------------------------------------------


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA | PRIMARY METHOD - SINGLE DAY
            /// <summary>
            ///     Write SP CSW data from Baseball Savant to CSV File
            ///     Uses 'GetAllSpCswSingleDayEndPointUri' end point
            /// </summary>
            /// <remarks>
            ///     If the file already exists, it will not create a new file
            /// </remarks>
            /// <param names="monthNumber", "dayNumber", "year">
            ///     ints that represent the month number, day number, and year you want to get data for and write to a CSV
            /// </param>
            public void WriteBaseballSavantSingleDayDataToCsv(int year, int monthNumber, int dayNumber)
            {
                var endPointUri = GetAllSpCswSingleDayEndPointUri(year, monthNumber, dayNumber);
                var targetCsvString = SetCsvFilePathSpCswSingleDay(year, monthNumber, dayNumber);
                // Console.WriteLine($"targetCsvString: {targetCsvString}");

                // check if file exists
                if(System.IO.File.Exists(targetCsvString))
                {
                    Console.WriteLine("FILE EXISTS");
                }

                // file does not exist
                else
                {
                    DownloadSpCswCvsAndWriteToLocalCsv(endPointUri, $"{targetCsvString}");
                    Console.WriteLine("CREATING NEW FILE");
                }
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA | HELPER METHOD - SINGLE DAY
            /// <summary>
            ///     Create end point for 'WriteBaseballSavantSingleDayDataToCsv' method to write to
            /// </summary>
            public string GetAllSpCswSingleDayEndPointUri(int year, int monthNumber, int dayNumber)
            {
                BaseballSavantUriEndPoint endPoint = _bsEndPoints.AllSpCswSingleDay(year,monthNumber,dayNumber);
                return endPoint.EndPointUri;
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA | HELPER METHOD - SINGLE DAY
            /// <summary>
            ///     Creates a string of the fill path name
            /// </summary>
            public string SetCsvFilePathSpCswSingleDay(int year, int monthNumber, int dayNumber)
            {
                string filePath = $"{_allSpCswSingleDayCvsPath}{monthNumber}_{dayNumber}_{year}.csv";
                return filePath;
            }


            // STATUS [ June 5, 2019 ]: this works
            // READ DATA | PRIMARY METHOD - SINGLE DAY
            /// <summary>
            ///     Read CSV File of 'StartingPitcherCswSingleDay' instances
            ///     The file includes SP CSW(called strike + whiffs) for pitchers that pitched on a certain day
            /// </summary>
            /// <remarks>
            ///     This reads a given file in BaseballData/02_Target_Write/BaseballSavant_Target_Write project folder
            ///     If a file does not exist for the date entered, you'll get an error
            ///     Uses CsvHandler from project Infrastructure to read the file
            ///     'csvFilePath' will be something like:
            ///         BaseballData/02_Target_Write/BaseballSavant_Target_Write/Bs_AllSpSingleDayCsw5_29_2019.csv
            /// </remarks>
            /// <param names="monthNumber", "dayNumber", "year">
            ///     ints that represent the month number, day number, and year you want to search for
            /// </param>
            /// <example>
            ///     var spCswList = await ReadSpCswCsvSingleDayAsync(5,29,2019);
            /// </example>
            /// <returns>
            ///     List<StartingPitcherCswSingleDay>
            /// </returns>
            public async Task<List<StartingPitcherCswSingleDay>> ReadSpCswCsvSingleDayAsync(int monthNumber, int dayNumber, int year)
            {
                // _h.StartMethod();
                // file name example: Bs_AllSpSingleDayCsw5_29_2019.csv
                var csvFilePath = SetCsvFilePathSpCswSingleDay(year, monthNumber, dayNumber);
                // Console.WriteLine($"csvFilePath: {csvFilePath}");

                await _csvH.ReadCsvRecordsAsyncToList(csvFilePath, _spCswTypeSingleDay, _spCswModelMapTypeSingleDay, _dynamicList);

                _listSpCswSingleDay = CreateSpCswSingleDayList(_dynamicList, year, monthNumber, dayNumber);

                // PrintBsSpCswDetail(_dynamicList);
                return _listSpCswSingleDay;
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA TO DATABASE - SINGLE DAY
            /// <summary>
            ///     Add StartingPitcherCswSingleDay to Database
            /// </summary>
            public void AddStartingPitcherCswSingleDayToDatabase(StartingPitcherCswSingleDay spCsw)
            {
                _context.Add(spCsw);
                _context.SaveChanges();
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA TO DATABASE - SINGLE DAY
            /// <summary>
            ///     Add StartingPitcherCswSingleDay to Database from list
            /// </summary>
            public void AddStartingPitcherCswsSingleDayToDatabaseFromList(List<StartingPitcherCswSingleDay> listSpCsw)
            {
                foreach(StartingPitcherCswSingleDay player in listSpCsw)
                    AddStartingPitcherCswSingleDayToDatabase(player);
            }


            // STATUS [ June 5, 2019 ]: this works
            // LIST HELPER METHOD - SINGLE DAY
            /// <summary>
            ///     Create List<StartingPitcherCswSingleDay>
            /// </summary>
            public List<StartingPitcherCswSingleDay> CreateSpCswSingleDayList(List<dynamic> list, int year, int monthNumber, int dayNumber)
            {
                List<StartingPitcherCswSingleDay> listSpCsw = new List<StartingPitcherCswSingleDay>();
                DateTime datePitched = new DateTime(year, monthNumber, dayNumber);

                foreach(var record in list)
                {
                    var spCsw = record as StartingPitcherCswSingleDay;
                    spCsw.DatePitched = datePitched;
                    listSpCsw.Add(spCsw);
                }
                return listSpCsw;
            }


        #endregion SP CALLED-STRIKE + WALK DATA - SINGLE DAY ------------------------------------------------------------





        // OPTION 2: DATE RANGE CSW DATA
        #region SP CALLED-STRIKE + WALK DATA - DATE RANGE ------------------------------------------------------------


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA | PRIMARY METHOD - DATE RANGE
            /// <summary>
            ///     Write SP CSW data from Baseball Savant to CSV File
            ///     Uses 'GetAllSpCswRangeOfDaysEndPointUri' end point
            /// </summary>
            /// <param names="year", "startMonth", "startDay", "endMonth", "endDay">
            ///     ints that represent the date range you want to search
            /// </param>
            public void WriteBaseballSavantDateRangeDataToCsv(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                var endPointUri = GetAllSpCswRangeOfDaysEndPointUri(year, startMonth, startDay, endMonth, endDay);
                var targetCsvString = SetCsvFilePathSpCswDateRange(year, startMonth, startDay, endMonth, endDay);
                DownloadSpCswCvsAndWriteToLocalCsv(endPointUri, $"{targetCsvString}");
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA | HELPER METHOD - DATE RANGE
            /// <summary>
            ///     Create end point for 'WriteBaseballSavantDateRangeDataToCsv' method to write to
            /// </summary>
            public string GetAllSpCswRangeOfDaysEndPointUri(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                BaseballSavantUriEndPoint endPoint = _bsEndPoints.AllSpCswRangeOfDays(year, startMonth, startDay, endMonth, endDay);
                return endPoint.EndPointUri;
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA | HELPER METHOD - DATE RANGE
            /// <summary>
            ///     Creates a string of the fill path name
            /// </summary>
            public string SetCsvFilePathSpCswDateRange(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                string filePath = $"{_allSpCswDateRangeCvsPath}{startMonth}_{startDay}_{year}_to_{endMonth}_{endDay}_{year}.csv";
                return filePath;
            }


            // STATUS [ June 25, 2019 ]: this works
            // READ DATA | PRIMARY METHOD - DATE RANGE
            /// <summary>
            ///     Read CSV File of 'StartingPitcherCswFullSeason' instances from day X to day Y
            ///     The file includes SP CSW(called strike + whiffs) for pitchers in that date range
            /// </summary>
            /// <remarks>
            ///     This reads a given file in BaseballData/02_Target_Write/BaseballSavant_Target_Write project folder
            ///     If a file does not exist for the date range entered, you'll get an error
            ///     Uses CsvHandler from project Infrastructure to read the file
            ///     'csvFilePath' will be something like:
            ///         BaseballData/02_Target_Write/BaseballSavant_Target_Write/Bs_SpCswDateRange5_1_2019_to_5_15_2019.csv
            /// </remarks>
            /// <param names="year", "startMonth", "startDay", "endMonth", "endDay">
            ///     ints that represent the date range you want to search
            /// </param>
            /// <example>
            ///     var spCswList = await ReadSpCswCsvDateRangeAsync(2019, 5, 1, 5, 15);
            /// </example>
            /// <returns>
            ///     List<StartingPitcherCswDateRange>
            /// </returns>
            public async Task<List<StartingPitcherCswDateRange>> ReadSpCswCsvDateRangeAsync(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                // file name example: Bs_SpCswDateRange5_1_2019_to_5_15_2019.csv
                var csvFilePath = SetCsvFilePathSpCswDateRange(year, startMonth, startDay, endMonth, endDay);
                // Console.WriteLine($"csvFilePath: {csvFilePath}");

                await _csvH.ReadCsvRecordsAsyncToList(csvFilePath, _spCswTypeDateRange, _spCswModelMapTypeDateRange, _dynamicList);

                _listSpCswDateRange = CreateSpCswDateRangeList(_dynamicList, year, startMonth, startDay, endMonth, endDay);

                // PrintBsSpCswDetail(_dynamicList);
                // AddStartingPitcherCswsDateRangeToDatabaseFromList(_listSpCswDateRange);
                return _listSpCswDateRange;
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA TO DATABASE - DATE RANGE
            /// <summary>
            ///     Add StartingPitcherCswDateRange to Database
            /// </summary>
            public void AddStartingPitcherCswDateRangeToDatabase(StartingPitcherCswDateRange spCsw)
            {
                _context.Add(spCsw);
                _context.SaveChanges();
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA TO DATABASE - DATE RANGE
            /// <summary>
            ///     Add StartingPitcherCswDateRange to Database from list
            /// </summary>
            public void AddStartingPitcherCswsDateRangeToDatabaseFromList(List<StartingPitcherCswDateRange> listSpCsw)
            {
                foreach(StartingPitcherCswDateRange player in listSpCsw)
                    AddStartingPitcherCswDateRangeToDatabase(player);
            }


            // STATUS [ June 5, 2019 ]: this works
            // LIST HELPER METHOD - DATE RANGE
            /// <summary>
            ///     Create List<StartingPitcherCswDateRange>>
            /// </summary>
            public List<StartingPitcherCswDateRange> CreateSpCswDateRangeList(List<dynamic> list, int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                List<StartingPitcherCswDateRange> listSpCsw = new List<StartingPitcherCswDateRange>();
                DateTime startDate = new DateTime(year, startMonth, startDay);
                DateTime endDate = new DateTime(year, endMonth, endDay);

                foreach(var record in list)
                {
                    var spCsw = record as StartingPitcherCswDateRange;
                    spCsw.StartDate = startDate;
                    spCsw.EndDate = endDate;
                    listSpCsw.Add(spCsw);
                }
                return listSpCsw;
            }


        #endregion SP CALLED-STRIKE + WALK DATA - DATE RANGE ------------------------------------------------------------




        // OPTION 3: FULL SEASON CSW DATA
        #region SP CALLED-STRIKE + WALK DATA - FULL SEASON ------------------------------------------------------------


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA | PRIMARY METHOD - FULL SEASON
            /// <summary>
            ///     Write SP CSW data from Baseball Savant to CSV File
            ///     Uses 'GetAllSpCswFullSeasonEndPointUri' end point
            /// </summary>
            /// <param name="year">
            ///     The year / season you want to search for
            /// </param>
            public void WriteBaseballSavantFullSeasonData(int year)
            {
                var endPointUri = GetAllSpCswFullSeasonEndPointUri(year);
                var targetCsvString = SetCsvFilePathSpCswFulLSeason(year);
                DownloadSpCswCvsAndWriteToLocalCsv(endPointUri, $"{targetCsvString}");
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA | HELPER METHOD - FULL SEASON
            /// <summary>
            ///     Create end point for 'WriteBaseballSavantFullSeasonData' method to write to
            /// </summary>
            public string GetAllSpCswFullSeasonEndPointUri(int year)
            {
                BaseballSavantUriEndPoint endPoint = _bsEndPoints.AllSpCswFullSeason(year);
                return endPoint.EndPointUri;
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA | HELPER METHOD - FULL SEASON
            /// <summary>
            ///     Creates a string of the fill path name
            /// </summary>
            public string SetCsvFilePathSpCswFulLSeason(int year)
            {
                string filePath = $"{_allSpCswDateRangeCvsPath}_FULL_SEASON_{year}.csv";
                return filePath;
            }


            // STATUS [ June 25, 2019 ]: this works
            // READ DATA | PRIMARY METHOD - FULL SEASON
            /// <summary>
            ///     Read CSV File of 'StartingPitcherCsw' instances for a year / season
            ///     The file includes SP CSW(called strike + whiffs) for pitchers in that season
            /// </summary>
            /// <remarks>
            ///     This reads a given file in BaseballData/02_Target_Write/BaseballSavant_Target_Write project folder
            ///     If a file does not exist for the season / year entered, you'll get an error
            ///     Uses CsvHandler from project Infrastructure to read the file
            ///     'csvFilePath' will be something like:
            ///         BaseballData/02_Target_Write/BaseballSavant_Target_Write/Bs_SpCswDateRange_FULL_SEASON_2019.csv
            /// </remarks>
            /// <param name="year">
            ///     The year / season you want to search for
            /// </param>
            /// <example>
            ///     var spCswList = await ReadSpCswCsvFullSeasonAsync(2019)
            /// </example>
            /// <returns>
            ///     List<StartingPitcherCsw>
            /// </returns>
            public async Task<List<StartingPitcherCsw>> ReadSpCswCsvFullSeasonAsync(int year)
            {
                // file name example: Bs_SpCswDateRange_FULL_SEASON_2019.csv
                var csvFilePath = SetCsvFilePathSpCswFulLSeason(year);

                await _csvH.ReadCsvRecordsAsyncToList(csvFilePath, _spCswType, _spCswModelMapType, _dynamicList);

                _listSpCsw = CreateSpCswFullSeasonList(_dynamicList, year);

                PrintBsSpCswDetail(_dynamicList);
                // AddStartingPitcherCswsToDatabaseFromList(_listSpCsw);
                return _listSpCsw;
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA TO DATABASE - FULL SEASON
            /// <summary>
            ///     Add StartingPitcherCsw to Database
            /// </summary>
            public void AddStartingPitcherCswToDatabase(StartingPitcherCsw spCsw)
            {
                _context.Add(spCsw);
                _context.SaveChanges();
            }


            // STATUS [ June 5, 2019 ]: this works
            // WRITE DATA TO DATABASE - FULL SEASON
            /// <summary>
            ///     Add StartingPitcherCsw to Database from list
            /// </summary>
            public void AddStartingPitcherCswsToDatabaseFromList(List<StartingPitcherCsw> listSpCsw)
            {
                foreach(StartingPitcherCsw player in listSpCsw)
                    AddStartingPitcherCswToDatabase(player);
            }


            // STATUS [ June 5, 2019 ]: this works
            // LIST HELPER METHOD - FULL SEASON
            /// <summary>
            ///     Create List<StartingPitcherCsw>
            /// </summary>
            public List<StartingPitcherCsw> CreateSpCswFullSeasonList(List<dynamic> list, int year)
            {
                List<StartingPitcherCsw> listSpCsw = new List<StartingPitcherCsw>();
                foreach(var record in list)
                {
                    var spCsw = record as StartingPitcherCsw;
                    listSpCsw.Add(spCsw);
                }
                return listSpCsw;
            }


        #endregion SP CALLED-STRIKE + WALK DATA - FULL SEASON ------------------------------------------------------------





        #region GENERAL HELPERS ------------------------------------------------------------

            // STATUS [ June 5, 2019 ]: this works
            public void DownloadSpCswCvsAndWriteToLocalCsv(string endPointUri, string targetCsvString)
            {
                _csvH.DownloadCsvFromLink(endPointUri, targetCsvString);
            }

        #endregion GENERAL HELPERS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintBsSpCswDetail(List<dynamic> list)
            {
                int count = 1;
                foreach(var record in list)
                {
                    var spCsw = record as StartingPitcherCsw;

                    // Console.WriteLine($"{spCsw.DatePitched}");

                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\n---------------------------------------------------------");
                    Console.WriteLine($"{count}. PLAYER: {spCsw.PlayerName}  ID: {spCsw.PlayerId}");
                    Console.WriteLine($"--------------------------------------------------------");
                    Console.ResetColor();
                    Console.WriteLine($"TOTAL PITCHES    | {spCsw.TotalPitches}");
                    Console.WriteLine($"CSW PITCHES      | {spCsw.CswPitches}");
                    Console.WriteLine($"CSW PITCH %      | {spCsw.CswPitchPercent}\n");

                    Console.WriteLine($"AT BATS          | {spCsw.Abs}");
                    Console.WriteLine($"SPIN RATE        | {spCsw.SpinRate}");
                    Console.WriteLine($"VELOCITY         | {spCsw.Velocity}");
                    Console.WriteLine($"EFFECTIVE SPEED  | {spCsw.EffectiveSpeed}");
                    Console.WriteLine($"WHIFFS           | {spCsw.Whiffs}\n");
                    Console.WriteLine($"--------------------------------------------------------\n");

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
                    count++;
                }
            }

        #endregion PRINTING PRESS ------------------------------------------------------------

    }
}
