using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BaseballScraper.Controllers.BaseballHQControllers;
using BaseballScraper.Controllers.BaseballSavantControllers;
using BaseballScraper.Controllers.FanGraphsControllers;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Player;
using Microsoft.AspNetCore.Mvc;
using C = System.Console;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.AGGREGATORS
{
    [Route("master_report")]
    public class MasterReportController : Controller
    {
        private readonly Helpers                        _helpers;
        private readonly PlayerBaseController           _playerBaseController;
        private readonly BaseballSavantHitterController _baseballSavantHitterController;
        private readonly BaseballHQHitterController     _hqHitterController;
        private readonly FanGraphsSpController          _fanGraphsSpController;


        public MasterReportController(Helpers helpers, PlayerBaseController playerBaseController, BaseballSavantHitterController baseballSavantHitterController, BaseballHQHitterController hqHitterController, FanGraphsSpController fanGraphsSpController)
        {
            _helpers                        = helpers;
            _playerBaseController           = playerBaseController;
            _baseballSavantHitterController = baseballSavantHitterController;
            _hqHitterController             = hqHitterController;
            _fanGraphsSpController          = fanGraphsSpController;
        }

        public MasterReportController(){}



        /*
            https://127.0.0.1:5001/master_report/full_season
        */
        [HttpPost("full_season")]
        public async Task<IActionResult> UpdateFullSeasonReports()
        {
            _helpers.OpenMethod(3);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // /* ADD ALL SFBB & CRUNCH TIME PLAYER BASES */
            // await _playerBaseController.MASTER_REPORT_CALLER("A7:AQ2333");
            // C.WriteLine($"[ 1 ] {sw.ElapsedMilliseconds}");


            // /* ADD X-STATS & EXIT VELO REPORTS */
            // _baseballSavantHitterController.MASTER_REPORT_CALLER(2019, 100);
            // C.WriteLine($"[ 2 ] {sw.ElapsedMilliseconds}");


            // /* ADD YTD & ROS PROJECTION HITTER REPORTS */
            // await _hqHitterController.MASTER_REPORT_CALLER(false, false);
            // C.WriteLine($"[ 3 ] {sw.Elapsed}");

            /* FANGRAPHS SP */
            await _fanGraphsSpController.MASTER_REPORT_CALLER();
            C.WriteLine($"[ 4 ] {sw.Elapsed.Seconds}");


            sw.Stop();
            return Ok();
        }


        // https://127.0.0.1:5001/master_report/report_runner_view
        [HttpGet("report_runner_view")]
        public IActionResult GoToReportRunnerPage()
        {
            _helpers.OpenMethod(1);
            _helpers.CloseMethod(1);
            return View("ReportRunner");
        }
    }
}






// _helpers.OpenMethod(3);

// Stopwatch sw = new Stopwatch();
// sw.Start();

// /* ADD ALL SFBB PLAYER BASES */
// // IEnumerable<SfbbPlayerBase> allPlayerBases = _playerBaseController.GetAllSfbbPlayerBasesFromGoogleSheet("A7:AQ2333");
// // List<SfbbPlayerBase> allPlayerBasesList    = new List<SfbbPlayerBase>();

// // foreach(SfbbPlayerBase sfbbPlayerBase in allPlayerBases)
// // {
// //     allPlayerBasesList.Add(sfbbPlayerBase);
// // }

// // _playerBaseController.AddAllSfbbPlayerBasesToDatabase(allPlayerBasesList);
// await _playerBaseController.MASTER_REPORT_CALLER("A7:AQ2333");
// C.WriteLine($"[ 1 ] {sw.ElapsedMilliseconds}");


// /* ADD ALL CRUNCH TIME PLAYER BASES */
// // List<CrunchTimePlayerBase> crunchTimePlayerBases = _playerBaseController.CreateListOfCrunchTimePlayerBasesForToday();
// // await _playerBaseController.AddAllCrunchTimePlayerBasesToDatabase(crunchTimePlayerBases);
// // C.WriteLine($"[ 2 ] {sw.ElapsedMilliseconds}");


// /* ADD X-STATS & EXIT VELO REPORTS */
// // _baseballSavantHitterController.DownloadAndAddAllHitterReports(2019, 100);
// _baseballSavantHitterController.MASTER_REPORT_CALLER(2019, 100);
// C.WriteLine($"[ 3 ] {sw.ElapsedMilliseconds}");


// /* ADD YTD & ROS PROJECTION HITTER REPORTS */
// // await _hqHitterController.UpdateBothHqHitterDatabases(false, false);
// await _hqHitterController.MASTER_REPORT_CALLER(false, false);
// C.WriteLine($"[ 4 ] {sw.Elapsed}");

// sw.Stop();
// return Ok();
