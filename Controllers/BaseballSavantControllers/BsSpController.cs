using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
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

        private static readonly BaseballSavantUriEndPoints _bsEndPoints = new BaseballSavantUriEndPoints();

        private readonly CsvHandler _csvH = new CsvHandler();

        private static String _targetWriteFolder = "BaseballData/02_Target_Write/BaseballSavant_Target_Write/";
        private static String _allSpCswSingleDayCvs = "Bs_AllSpSingleDayCsw";

        private static String _allSpCswSingleDayCvsPath = $"{_targetWriteFolder}{_allSpCswSingleDayCvs}";




        [Route("test")]
        public void BaseballSavantTesting(int year, int monthNumber, int dayNumber)
        {
            _h.StartMethod();


        }


        [Route("test/async")]
        public async Task BaseballSavantTestingAsync()
        {
            _h.StartMethod();
            await CreateSpCsw();
            _h.CompleteMethod();
        }


        /// <example>
        ///     GetAllSpCswSingleDayPerformances(2019, 05, 01);
        /// </example>
        public void GetAllSpCswSingleDayPerformances(int year, int monthNumber, int dayNumber)
        {
            var endPointUri = GetAllSpCswSingleDayEndPointUri(year, monthNumber, dayNumber);
            var dateStringSuffix = CreateTargetCsvDataSuffix(year, monthNumber, dayNumber);
            WriteBaseballSavantDataToCsv(endPointUri, $"{_allSpCswSingleDayCvsPath}{dateStringSuffix}.csv");
        }



        public string GetAllSpCswSingleDayEndPointUri(int year, int monthNumber, int dayNumber)
        {
            // set the proper end point for the csv that you want to download
            BaseballSavantUriEndPoint endPoint = _bsEndPoints.AllSpSingleDayCsw(year,monthNumber,dayNumber);
            Console.WriteLine($"CSW endPoint: {endPoint.EndPointUri}");

            var endPointUri = endPoint.EndPointUri;
            return endPointUri;
        }


        public void WriteBaseballSavantDataToCsv(string endPointUri, string targetCsvString)
        {
            _csvH.DownloadCsvFromLink(endPointUri, targetCsvString);
        }


        public string CreateTargetCsvDataSuffix(int year, int monthNumber, int dayNumber)
        {
            var dateStringSuffix = $"{monthNumber}_{dayNumber}_{year}";
            Console.WriteLine($"dateStringSuffix: {dateStringSuffix}");
            return dateStringSuffix;
        }


        public async Task CreateSpCsw ()
        {
            _h.StartMethod();

            var spCsw = new StartingPitcherCsw();
            Type spCswType = spCsw.GetType();

            var spCswModelMap = new StartingPitcherCswClassMap();
            Type spCswModelMapType = spCswModelMap.GetType();

            var csvFilePath = "";

            // List<StartingPitcherCsw> list = new List<StartingPitcherCsw>();
            List<dynamic> list = new List<dynamic>();

            // System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[System.Collections.Generic.IEnumerable`1[System.Object],BaseballScraper.Infrastructure.CsvHandler+<ReadCsvRecordsAsync>d__5]
            // Task<IEnumerable<dynamic>> spCswEnumerableTask = _csvH.ReadCsvRecordsAsync(csvFilePath, spCswType, spCswModelMapType);
            // var spCswEnumerableTask = _csvH.ReadCsvRecordsAsync(csvFilePath, spCswType, spCswModelMapType);
            await _csvH.ReadCsvRecordsAsync2(csvFilePath, spCswType, spCswModelMapType, list);

            foreach(var l in list)
            {
                // Console.WriteLine($"l {l}");
                spCsw = l as StartingPitcherCsw;
                // Console.WriteLine($"spCsw: {spCsw}");
                Console.WriteLine($"{spCsw.PlayerName}");
                Console.WriteLine($"{spCsw.PlayerId}");
                Console.WriteLine($"{spCsw.CswPitches}");
                Console.WriteLine($"{spCsw.TotalPitches}");
                Console.WriteLine($"{spCsw.PitchPercent}");
                Console.WriteLine();
            }

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
        }

    }

}


//   BaseballSavant_AllSpSingleDayCsw.csv
//      savant_data.csv



//https://baseballsavant.mlb.com/statcast_search?hfPT=&hfAB=&hfBBT=&hfPR=foul%5C.%5C.tip%7Cswinging%5C.%5C.pitchout%7Cswinging%5C.%5C.strike%7Cswinging%5C.%5C.strike%5C.%5C.blocked%7C&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea=2019%7C&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt=2019-05-27&game_date_lt=2019-05-27&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=50&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_pas=0


// https://baseballsavant.mlb.com/statcast_search/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7CPO%7CS%7C=&hfSea=&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt={}&game_date_lt={}&pitchers_lookup%5B%5D={}&team=&position=&hfRO=&home_road=&hfFlag=&metric_1=&hfInn=&min_pitches=0&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_abs=0&type=details&
