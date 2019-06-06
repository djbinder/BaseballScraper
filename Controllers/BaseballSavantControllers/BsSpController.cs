using System;
using System.Collections;
using System.Collections.Generic;
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

        private readonly BaseballScraperContext _context;

        private static readonly BaseballSavantUriEndPoints _bsEndPoints = new BaseballSavantUriEndPoints();

        private readonly CsvHandler _csvH = new CsvHandler();

        private static String _targetWriteFolder = "BaseballData/02_Target_Write/BaseballSavant_Target_Write/";

        private static String _allSpCswSingleDayCvs = "Bs_AllSpSingleDayCsw";

        private static String _allSpCswSingleDayCvsPath = $"{_targetWriteFolder}{_allSpCswSingleDayCvs}";

        private static String _allSpCswDateRangeCvs = "Bs_SpCswDateRange";

        private static String _allSpCswDateRangeCvsPath = $"{_targetWriteFolder}{_allSpCswDateRangeCvs}";


        public BsSpController(BaseballScraperContext context)
        {
            _context = context;
        }

        [Route("test")]
        public void BaseballSavantTesting()
        {
            _h.StartMethod();
            // WriteBaseballSavantSingleDayDataToCsv(2019,5,21);
            // WriteBaseballSavantDateRangeDataToCsv(2019, 5, 1, 5, 15);
            // WriteBaseballSavantFullSeason(2019);
        }


        [Route("test/async")]
        public async Task BaseballSavantTestingAsync()
        {
            _h.StartMethod();

            // await ReadSpCswCsvSingleDayAsync(5,29,2019);
            // await ReadSpCswCsvDateRangeAsync(2019, 5, 1, 5, 29);
            await ReadSpCswCsvFullSeasonAsync(2019);
            _h.CompleteMethod();
        }





        #region READ BASEBALL SAVANT DATA ------------------------------------------------------------


            // STATUS [ June 5, 2019 ]: this works
            // A) SINGLE DAY
            /// <example>
            ///     List<StartingPitcherCsw> spCswList = await GetStartingPitcherCswAsync();
            /// </example>
            public async Task<List<StartingPitcherCswSingleDay>> ReadSpCswCsvSingleDayAsync(int monthNumber, int dayNumber, int year)
            {
                _h.StartMethod();

                Type spCswType = new StartingPitcherCswSingleDay().GetType();
                Type spCswModelMapType = new StartingPitcherCswClassMap().GetType();

                var csvFilePath = SetCsvFilePathSpCswSingleDay(year, monthNumber, dayNumber);

                List<dynamic> list = new List<dynamic>();
                List<StartingPitcherCswSingleDay> listSpCsw = new List<StartingPitcherCswSingleDay>();

                await _csvH.ReadCsvRecordsAsyncToList(csvFilePath, spCswType, spCswModelMapType, list);

                listSpCsw = CreateSpCswSingleDayList(list, year, monthNumber, dayNumber);

                PrintBsSpCswDetail(list);

                AddStartingPitcherCswsSingleDayToDatabaseFromList(listSpCsw);
                return listSpCsw;
            }


            // STATUS [ June 5, 2019 ]: this works
            // B) DATE RANGE
            public async Task<List<StartingPitcherCswDateRange>> ReadSpCswCsvDateRangeAsync(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                _h.StartMethod();

                Type spCswType = new StartingPitcherCswDateRange().GetType();
                Type spCswModelMapType = new StartingPitcherCswClassMap().GetType();

                var csvFilePath = SetCsvFilePathSpCswDateRange(year, startMonth, startDay, endMonth, endDay);

                List<dynamic> list = new List<dynamic>();
                List<StartingPitcherCswDateRange> listSpCsw = new List<StartingPitcherCswDateRange>();

                await _csvH.ReadCsvRecordsAsyncToList(csvFilePath, spCswType, spCswModelMapType, list);

                listSpCsw = CreateSpCswDateRangeList(list, year, startMonth, startDay, endMonth, endDay);

                PrintBsSpCswDetail(list);

                AddStartingPitcherCswsDateRangeToDatabaseFromList(listSpCsw);
                return listSpCsw;
            }


            // STATUS [ June 5, 2019 ]: this works
            // C) FULL SEASON
            public async Task<List<StartingPitcherCsw>> ReadSpCswCsvFullSeasonAsync(int year)
            {
                _h.StartMethod();

                Type spCswType = new StartingPitcherCsw().GetType();
                Type spCswModelMapType = new StartingPitcherCswClassMap().GetType();

                var csvFilePath = SetCsvFilePathSpCswFulLSeason(year);

                List<dynamic> list = new List<dynamic>();

                await _csvH.ReadCsvRecordsAsyncToList(csvFilePath, spCswType, spCswModelMapType, list);

                List<StartingPitcherCsw> listSpCsw = new List<StartingPitcherCsw>();
                listSpCsw = CreateSpCswFullSeasonList(list, year);

                PrintBsSpCswDetail(list);

                AddStartingPitcherCswsToDatabaseFromList(listSpCsw);

                return listSpCsw;
            }


        #endregion READ BASEBALL SAVANT DATA ------------------------------------------------------------





        #region CSV WRITERS ------------------------------------------------------------

            // STATUS [ June 5, 2019 ]: this works
            // A) SINGLE DAY
            public void WriteBaseballSavantSingleDayDataToCsv(int year, int monthNumber, int dayNumber)
            {
                var endPointUri = GetAllSpCswSingleDayEndPointUri(year, monthNumber, dayNumber);
                var targetCsvString = SetCsvFilePathSpCswSingleDay(year, monthNumber, dayNumber);
                DownloadSpCswCvsAndWriteToLocalCsv(endPointUri, $"{targetCsvString}");
            }

            // STATUS [ June 5, 2019 ]: this works
            // B) DATE RANGE
            public void WriteBaseballSavantDateRangeDataToCsv(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                var endPointUri = GetAllSpCswRangeOfDaysEndPointUri(year, startMonth, startDay, endMonth, endDay);
                var targetCsvString = SetCsvFilePathSpCswDateRange(year, startMonth, startDay, endMonth, endDay);
                DownloadSpCswCvsAndWriteToLocalCsv(endPointUri, $"{targetCsvString}");
            }

            // STATUS [ June 5, 2019 ]: this works
            // C) FULL SEASON
            public void WriteBaseballSavantFullSeason(int year)
            {
                var endPointUri = GetAllSpCswFullSeasonEndPointUri(year);
                var targetCsvString = SetCsvFilePathSpCswFulLSeason(year);
                DownloadSpCswCvsAndWriteToLocalCsv(endPointUri, $"{targetCsvString}");
            }

            // STATUS [ June 5, 2019 ]: this works
            public void DownloadSpCswCvsAndWriteToLocalCsv(string endPointUri, string targetCsvString)
            {
                _csvH.DownloadCsvFromLink(endPointUri, targetCsvString);
            }


        #endregion CSV WRITERS ------------------------------------------------------------





        // set the proper end point for the csv that you want to download
        #region ENDPOINTS ------------------------------------------------------------


            // STATUS [ June 5, 2019 ]: this works
            // A) SINGLE DAY
            public string GetAllSpCswSingleDayEndPointUri(int year, int monthNumber, int dayNumber)
            {
                BaseballSavantUriEndPoint endPoint = _bsEndPoints.AllSpCswSingleDay(year,monthNumber,dayNumber);
                return endPoint.EndPointUri;
            }

            // STATUS [ June 5, 2019 ]: this works
            // B) DATE RANGE
            public string GetAllSpCswRangeOfDaysEndPointUri(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                BaseballSavantUriEndPoint endPoint = _bsEndPoints.AllSpCswRangeOfDays(year, startMonth, startDay, endMonth, endDay);
                return endPoint.EndPointUri;
            }

            // STATUS [ June 5, 2019 ]: this works
            // C) FULL SEASON
            public string GetAllSpCswFullSeasonEndPointUri(int year)
            {
                BaseballSavantUriEndPoint endPoint = _bsEndPoints.AllSpCswFullSeason(year);
                return endPoint.EndPointUri;
            }


        #endregion ENDPOINTS ------------------------------------------------------------





        #region FILE PATHS ------------------------------------------------------------

            // STATUS [ June 5, 2019 ]: this works
            // A) SINGLE DAY
            public string SetCsvFilePathSpCswSingleDay(int year, int monthNumber, int dayNumber)
            {
                string filePath = $"{_allSpCswSingleDayCvsPath}{monthNumber}_{dayNumber}_{year}.csv";
                return filePath;
            }

            // STATUS [ June 5, 2019 ]: this works
            // B) DATE RANGE
            public string SetCsvFilePathSpCswDateRange(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                string filePath = $"{_allSpCswDateRangeCvsPath}{startMonth}_{startDay}_{year}_to_{endMonth}_{endDay}_{year}.csv";
                return filePath;
            }

            // STATUS [ June 5, 2019 ]: this works
            // C) FULL SEASON
            public string SetCsvFilePathSpCswFulLSeason(int year)
            {
                string filePath = $"{_allSpCswDateRangeCvsPath}_FULL_SEASON_{year}.csv";
                return filePath;
            }


        #endregion FILE PATHS ------------------------------------------------------------





        #region DATABASE ACTIONS ------------------------------------------------------------

            // STATUS [ June 5, 2019 ]: this works
            // A.1) SINGLE DAY
            public void AddStartingPitcherCswSingleDayToDatabase(StartingPitcherCswSingleDay spCsw)
            {
                _context.Add(spCsw);
                _context.SaveChanges();
            }

            // STATUS [ June 5, 2019 ]: this works
            // A.2) SINGLE DAY
            public void AddStartingPitcherCswsSingleDayToDatabaseFromList(List<StartingPitcherCswSingleDay> listSpCsw)
            {
                foreach(StartingPitcherCswSingleDay player in listSpCsw)
                    AddStartingPitcherCswSingleDayToDatabase(player);
            }

            // STATUS [ June 5, 2019 ]: this works
            // B.1) DATE RANGE
            public void AddStartingPitcherCswDateRangeToDatabase(StartingPitcherCswDateRange spCsw)
            {
                _context.Add(spCsw);
                _context.SaveChanges();
            }

            // STATUS [ June 5, 2019 ]: this works
            // B.2) DATE RANGE
            public void AddStartingPitcherCswsDateRangeToDatabaseFromList(List<StartingPitcherCswDateRange> listSpCsw)
            {
                foreach(StartingPitcherCswDateRange player in listSpCsw)
                    AddStartingPitcherCswDateRangeToDatabase(player);
            }

            // STATUS [ June 5, 2019 ]: this works
            // C.1) FULL SEASON
            public void AddStartingPitcherCswToDatabase(StartingPitcherCsw spCsw)
            {
                _context.Add(spCsw);
                _context.SaveChanges();
            }

            // STATUS [ June 5, 2019 ]: this works
            // C.2) FULL SEASON
            public void AddStartingPitcherCswsToDatabaseFromList(List<StartingPitcherCsw> listSpCsw)
            {
                foreach(StartingPitcherCsw player in listSpCsw)
                    AddStartingPitcherCswToDatabase(player);
            }


        #endregion DATABASE ACTIONS ------------------------------------------------------------




        #region LIST ACTIONS ------------------------------------------------------------

            // STATUS [ June 5, 2019 ]: this works
            // A) SINGLE DAY
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

            // STATUS [ June 5, 2019 ]: this works
            // B) DATE RANGE
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

            // STATUS [ June 5, 2019 ]: this works
            // C) FULL SEASON
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


        #endregion LIST ACTIONS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------


            public void PrintBsSpCswDetail(List<dynamic> list)
            {
                foreach(var record in list)
                {
                    var spCsw = record as StartingPitcherCsw;

                    // Console.WriteLine($"{spCsw.DatePitched}");

                    Console.WriteLine($"{spCsw.PlayerName}");
                    Console.WriteLine($"{spCsw.PlayerId}");
                    Console.WriteLine($"{spCsw.CswPitches}");
                    Console.WriteLine($"{spCsw.TotalPitches}");
                    Console.WriteLine($"{spCsw.PitchPercent}");

                    Console.WriteLine($"{spCsw.Ba}");
                    Console.WriteLine($"{spCsw.Iso}");
                    Console.WriteLine($"{spCsw.Babip}");
                    Console.WriteLine($"{spCsw.Slg}");
                    Console.WriteLine($"{spCsw.Woba}");

                    Console.WriteLine($"{spCsw.Xwoba}");
                    Console.WriteLine($"{spCsw.Xba}");
                    Console.WriteLine($"{spCsw.Hits}");
                    Console.WriteLine($"{spCsw.Abs}");
                    Console.WriteLine($"{spCsw.LaunchSpeed}");

                    Console.WriteLine($"{spCsw.LaunchAngle}");
                    Console.WriteLine($"{spCsw.SpinRate}");
                    Console.WriteLine($"{spCsw.Velocity}");
                    Console.WriteLine($"{spCsw.EffectiveSpeed}");
                    Console.WriteLine($"{spCsw.Whiffs}");

                    Console.WriteLine($"{spCsw.Swings}");
                    Console.WriteLine($"{spCsw.Takes}");
                    Console.WriteLine($"{spCsw.EffectiveMinVelocity}");
                    Console.WriteLine($"{spCsw.ReleaseExtension}");

                    Console.WriteLine($"{spCsw.Pos3IntStartDistance}");
                    Console.WriteLine($"{spCsw.Pos4IntStartDistance}");
                    Console.WriteLine($"{spCsw.Pos5IntStartDistance}");
                    Console.WriteLine($"{spCsw.Pos6IntStartDistance}");
                    Console.WriteLine($"{spCsw.Pos7IntStartDistance}");
                    Console.WriteLine($"{spCsw.Pos8IntStartDistance}");
                    Console.WriteLine($"{spCsw.Pos9IntStartDistance}");

                    Console.WriteLine();
                }
            }




        #endregion PRINTING PRESS ------------------------------------------------------------

    }

}


//   BaseballSavant_AllSpSingleDayCsw.csv
//      savant_data.csv



//https://baseballsavant.mlb.com/statcast_search?hfPT=&hfAB=&hfBBT=&hfPR=foul%5C.%5C.tip%7Cswinging%5C.%5C.pitchout%7Cswinging%5C.%5C.strike%7Cswinging%5C.%5C.strike%5C.%5C.blocked%7C&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea=2019%7C&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt=2019-05-27&game_date_lt=2019-05-27&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=50&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_pas=0


// https://baseballsavant.mlb.com/statcast_search/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7CPO%7CS%7C=&hfSea=&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt={}&game_date_lt={}&pitchers_lookup%5B%5D={}&team=&position=&hfRO=&home_road=&hfFlag=&metric_1=&hfInn=&min_pitches=0&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_abs=0&type=details&






// // IEnumerable<dynamic>
// // CsvHelper.CsvReader+<GetRecords>d__65
// var spCswEnumerable = spCswEnumerableTask.Result;

// // _h.EnumerateOverRecordsDynamic(spCswEnumerable);

// // CsvHelper.CsvReader+<GetRecords>d__65
// // CsvHelper.CsvReader+<GetRecords>d__65
// IEnumerator<dynamic> spCswEnumerator = spCswEnumerable.GetEnumerator();


// // dynamic current = spCswEnumerator.Current;
// Console.WriteLine($"spCswEnumerator.Current: {spCswEnumerator.Current}");
// var current = spCswEnumerator.Current;
// Console.WriteLine($"current: {current}");


// foreach(var c in current)
// {
//     Console.WriteLine($"c: {c}");
// }


// foreach(var record in spCswEnumerable)
// {
//     Console.WriteLine($"record: {record}");
//     Console.WriteLine($"record.GetType: {record.GetType()}");
//     // var pitcher = record as StartingPitcherCsw;
//     // Console.WriteLine($"pitcher: {pitcher.PlayerName}");
// }

// _h.CompleteMethod();

// return spCsw;

            // public string CreateCsvFileNameToWriteToSingleDay(int year, int monthNumber, int dayNumber)
            // {
            //     var endPointUri = GetAllSpCswSingleDayEndPointUri(year, monthNumber, dayNumber);
            //     var dateStringSuffix = CreateTargetCsvDataSuffixSingleDay(year, monthNumber, dayNumber);
            //     var fileName = $"{endPointUri}{_allSpCswSingleDayCvsPath}{dateStringSuffix}.csv";
            //     return fileName;
            // }



            // public string CreateTargetCsvDataSuffixSingleDay(int year, int monthNumber, int dayNumber)
            // {
            //     var dateStringSuffix = $"{monthNumber}_{dayNumber}_{year}";
            //     return dateStringSuffix;
            // }

            // public string CreateTargetCsvDataSuffixRangeOfDays(int year, int startMonth, int startDay, int endMonth, int endDay)
            // {
            //     var dateStringSuffix = $"{startMonth}_{startDay}_{year}_to_{endMonth}_{endDay}_{year}";
            //     return dateStringSuffix;
            // }
