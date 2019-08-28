using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using BaseballScraper.Controllers.BaseballHQControllers;
using static BaseballScraper.Controllers.PlayerControllers.PlayerBaseController;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.Controllers.BaseballSavantControllers;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.AGGREGATORS
{
    [Route("master_hitter")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MasterHitterController : ControllerBase
    {

        private readonly Helpers _helpers;
        private readonly BaseballHQHitterController _hqHitterController;
        private readonly PlayerBaseFromGoogleSheet gSheet;
        private readonly PlayerBaseController playerBaseController;
        private readonly BaseballSavantHitterController baseballSavantHitterController;

        public MasterHitterController(Helpers helpers, BaseballHQHitterController hqHitterController, PlayerBaseFromGoogleSheet gSheet, PlayerBaseController playerBaseController, BaseballSavantHitterController baseballSavantHitterController)
        {
            _helpers = helpers;
            _hqHitterController = hqHitterController;
            this.gSheet = gSheet;
            this.playerBaseController = playerBaseController;
            this.baseballSavantHitterController = baseballSavantHitterController;

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
            // await playerBaseController.AddPlayerBasesToDatabase();
            // await _hqHitterController.UpdateBothHqHitterDatabases();
            // baseballSavantHitterController.DownloadAndAdd();
            return Ok();
        }


        #region START ------------------------------------------------------------
        #endregion START ------------------------------------------------------------

    }
}
