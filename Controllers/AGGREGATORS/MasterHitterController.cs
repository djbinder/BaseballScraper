using System;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using C = System.Console;
using BaseballScraper.Controllers.BaseballHQControllers;

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

        public MasterHitterController(Helpers helpers, BaseballHQHitterController hqHitterController)
        {
            _helpers = helpers;
            this._hqHitterController = hqHitterController;
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
            await _hqHitterController.UpdateBothHqHitterDatabases();
            return Ok();
        }


        #region START ------------------------------------------------------------
        #endregion START ------------------------------------------------------------

    }
}
