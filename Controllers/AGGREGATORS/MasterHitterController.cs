using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using BaseballScraper.Controllers.BaseballHQControllers;
using static BaseballScraper.Controllers.PlayerControllers.PlayerBaseController;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.Controllers.BaseballSavantControllers;
using System.Collections.Generic;
using BaseballScraper.Models.Player;
using BaseballScraper.EndPoints;
using System.Diagnostics;
using System;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.AGGREGATORS
{
    [Route("master_hitter")]
    [ApiController]
    // [ApiExplorerSettings(IgnoreApi = true)]
    public class MasterHitterController : ControllerBase
    {

        private readonly Helpers _helpers;
        private readonly BaseballHQHitterController _hqHitterController;
        private readonly PlayerBaseFromGoogleSheet gSheet;
        private readonly PlayerBaseController playerBaseController;
        private readonly BaseballSavantHitterController baseballSavantHitterController;
        private readonly ProjectDirectoryEndPoints _projectDirectory;

        public MasterHitterController(Helpers helpers, BaseballHQHitterController hqHitterController, PlayerBaseFromGoogleSheet gSheet, PlayerBaseController playerBaseController, BaseballSavantHitterController baseballSavantHitterController, ProjectDirectoryEndPoints projectDirectory)
        {
            _helpers                            = helpers;
            _hqHitterController                 = hqHitterController;
            this.gSheet                         = gSheet;
            this.playerBaseController           = playerBaseController;
            this.baseballSavantHitterController = baseballSavantHitterController;
            _projectDirectory                   = projectDirectory;

        }

        public MasterHitterController() {}


        /*
            https://127.0.0.1:5001/master_hitter/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
        }


        // STATUS [ July 8, 2019 ] : this works
        /*
            https://127.0.0.1:5001/master_hitter/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
        }


        /*
            https://127.0.0.1:5001/master_hitter/database_update
        */
        [HttpGet("database_update")]
        public async Task<ActionResult> UpdateDailyHitterDatabases()
        {
            _helpers.StartMethod();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            TimeSpan ts;


            /* ADD ALL SFBB PLAYER BASES */
            var allPlayerBases = playerBaseController.GetAllSfbbPlayerBasesFromGoogleSheet("A7:AQ2333");
            var toList = new List<SfbbPlayerBase>();
            foreach(var p in allPlayerBases)
            {
                toList.Add(p);
            }
            playerBaseController.AddAllSfbbPlayerBasesToDatabase(toList);
            ts = sw.Elapsed;
            Console.WriteLine($"[ 1 ] {ts}");


            /* ADD ALL CRUNCH TIME PLAYER BASES */
            List<CrunchTimePlayerBase> crunchTimePlayerBases = playerBaseController.CreateListOfCrunchTimePlayerBasesForToday();
            await playerBaseController.AddAllCrunchTimePlayerBasesToDatabase(crunchTimePlayerBases);
            ts = sw.Elapsed;
            Console.WriteLine($"[ 2 ] {ts}");


            /* ADD X-STATS & EXIT VELO REPORTS */
            baseballSavantHitterController.DownloadAndAddAllHitterReports(2019, 100);
            ts = sw.Elapsed;
            Console.WriteLine($"[ 3 ] {ts}");


            /* ADD YTD & ROS PROJECTION HITTER REPORTS */
            await _hqHitterController.UpdateBothHqHitterDatabases(false, false);
            ts = sw.Elapsed;
            Console.WriteLine($"[ 4 ] {ts}");


            return Ok();
        }


        #region START ------------------------------------------------------------
        #endregion START ------------------------------------------------------------

    }
}
