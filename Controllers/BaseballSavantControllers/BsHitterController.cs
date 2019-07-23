using System;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using static BaseballScraper.EndPoints.BaseballSavantUriEndPoints;
using C = System.Console;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.BaseballSavantControllers
{
    [Route("api/baseballsavant/[controller]")]
    [ApiController]
    public class BsHitterController : ControllerBase
    {
        private readonly Helpers _helpers;
        private readonly CsvHandler _csvHandler;
        private readonly BaseballSavantHitterEndPoints _hitterEndpoints;



        string _dummyCsvUrl = "https://baseballsavant.mlb.com/statcast_search/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea=2019%7C&hfSit=&player_type=batter&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt=2019-06-16&game_date_lt=&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=0&min_results=0&group_by=name&sort_col=xwoba&player_event_sort=h_launch_speed&sort_order=desc&min_pas=50&";


        public BsHitterController(Helpers helpers, CsvHandler csvHandler, BaseballSavantHitterEndPoints hitterEndpoints)
        {
            _helpers = helpers;
            _csvHandler = csvHandler;
            _hitterEndpoints = hitterEndpoints;
        }

        public BsHitterController() {}


        /*
            https://127.0.0.1:5001/api/baseballsavant/bshitter/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
            DownloadCsvFromBaseballSavant();
        }


        /*
            https://127.0.0.1:5001/api/baseballsavant/bshitter/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
        }




        #region START ------------------------------------------------------------


            // STATUS [ July 16, 2019 ] : this works but a lot more needs to be done
            public void DownloadCsvFromBaseballSavant()
            {
                // BaseballData/02_Target_Write/BaseballSavant_Target_Write
                var endPoint = _hitterEndpoints.HitterEndPoint(minPlateAppearances: 100).EndPointUri;
                C.WriteLine(endPoint);


                _csvHandler.DownloadCsvFromLink(endPoint,"BaseballData/02_Target_Write/BaseballSavant_Target_Write/Bs_Hitters_xWOBA_07172019.csv");
            }




        #endregion START ------------------------------------------------------------
    }

}
