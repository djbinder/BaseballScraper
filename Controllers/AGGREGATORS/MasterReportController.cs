using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BaseballScraper.Controllers.BaseballHQControllers;
using BaseballScraper.Controllers.BaseballSavantControllers;
using BaseballScraper.Controllers.FanGraphsControllers;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Player;
using C = System.Console;
using BaseballScraper.Models.ConfigurationModels;
using Microsoft.Extensions.Options;


#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.AGGREGATORS
{
    [Route("master_report")]
    public class MasterReportController : Controller
    {
        private readonly Helpers                        _helpers;
        private readonly PlayerBaseController           _playerBaseController;
        private readonly BaseballSavantHitterController _baseballSavantHitterController;
        private readonly BaseballHqHitterController     _hqHitterController;
        private readonly FanGraphsSpController          _fanGraphsSpController;
        private readonly BaseballSavantSpController     _baseballSavantSpController;
        private readonly AirtableManager                _airtableManager;




        public MasterReportController(Helpers helpers, PlayerBaseController playerBaseController, BaseballSavantHitterController baseballSavantHitterController, BaseballHqHitterController hqHitterController, FanGraphsSpController fanGraphsSpController, BaseballSavantSpController baseballSavantSpController, AirtableManager airtableManager)
        {
            _helpers                        = helpers;
            _playerBaseController           = playerBaseController;
            _baseballSavantHitterController = baseballSavantHitterController;
            _hqHitterController             = hqHitterController;
            _fanGraphsSpController          = fanGraphsSpController;
            _baseballSavantSpController     = baseballSavantSpController;
            _airtableManager                = airtableManager;

        }

        public MasterReportController(){}



        // https://127.0.0.1:5001/master_report/report_runner_view
        [HttpGet("report_runner_view")]
        public IActionResult GoToReportRunnerPage()
        {
            _helpers.OpenMethod(1);
            _helpers.CloseMethod(1);
            return View("ReportRunner");
        }


        /*
            https://127.0.0.1:5001/master_report/full_season
        */
        [HttpPost("full_season")]
        public async Task<IActionResult> UpdateFullSeasonReports()
        {
            _helpers.OpenMethod(3);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // 1) PLAYER BASES : Sfbb & Crunch Time
            await _playerBaseController.MASTER_REPORT_CALLER("A7:AQ2333");
            Mark(1, stopWatch, "PLAYER BASE");






            bool shouldTheseBeRun = false;
            if(shouldTheseBeRun == true)
            {
                // 2) SAVANT | HITTER : X-Stats and Exit Velo
                _baseballSavantHitterController.MASTER_REPORT_CALLER(2019, 100);
                Mark(2, stopWatch, "BASEBALL SAVANT HITTER");

                // 3) HQ | HITTER : YTD & ROS Projections
                await _hqHitterController.MASTER_REPORT_CALLER(false, false);
                Mark(3, stopWatch,"HQ HITTER");


                // 4) FANGRAPHS | SP | wPDI, mPDI
                await _fanGraphsSpController.MASTER_REPORT_CALLER();
                Mark(4, stopWatch, "FANGRAPH SP");


                // 5) SAVANT | SP | CSW
                await _baseballSavantSpController.MASTER_REPORT_CALLER();
                Mark(5, stopWatch, "BASEBALL SAVANT PITCHER");

            }


            stopWatch.Stop();
            _helpers.CompleteMethod();
            return Ok();
        }







        #region PRINTING PRESS ------------------------------------------------------------

        public void Mark(int orderNumber, Stopwatch stopWatch, string completedType)
        {
            C.ForegroundColor = ConsoleColor.DarkMagenta;
            C.WriteLine($"\n[ {orderNumber} ] {stopWatch.Elapsed.Seconds} seconds | {completedType}\n");
            C.ResetColor();
        }

        // C.WriteLine($"\n-------------------------------------------------------------------");
        // _helpers.PrintNameSpaceControllerNameMethodName(typeof(BaseballHqHitterController));

        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}






// #region PLAYER BASE --------------------
// #endregion PLAYER BASE --------------------




// #region FULL SEASON REPORTS --------------------

// bool runFullSeasonReports = false;
// if(runFullSeasonReports == true)
// {

// }

// #endregion FULL SEASON REPORTS --------------------



// #region SINGLE DAY REPORTS --------------------
// #endregion SINGLE DAY REPORTS --------------------
