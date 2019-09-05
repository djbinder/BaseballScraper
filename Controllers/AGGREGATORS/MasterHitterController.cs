using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using static BaseballScraper.Controllers.PlayerControllers.PlayerBaseController;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.Controllers.BaseballSavantControllers;
using BaseballScraper.EndPoints;
using BaseballScraper.Controllers.BaseballHQControllers;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.AGGREGATORS
{
    [Route("master_hitter")]
    [ApiController]
    // [ApiExplorerSettings(IgnoreApi = true)]
    public class MasterHitterController : ControllerBase
    {
        private readonly Helpers                        _helpers;
        private readonly BaseballHqHitterController     _hqHitterController;
        private readonly PlayerBaseController           _playerBaseController;
        private readonly BaseballSavantHitterController _baseballSavantHitterController;
        private readonly ProjectDirectoryEndPoints      _projectDirectory;

        // PlayerBaseFromGoogleSheet gSheet,
        public MasterHitterController(Helpers helpers, BaseballHqHitterController hqHitterController,  PlayerBaseController playerBaseController, BaseballSavantHitterController baseballSavantHitterController, ProjectDirectoryEndPoints projectDirectory)
        {
            _helpers                            = helpers;
            _hqHitterController                 = hqHitterController;
            _playerBaseController               = playerBaseController;
            _baseballSavantHitterController     = baseballSavantHitterController;
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




        #region START ------------------------------------------------------------
        #endregion START ------------------------------------------------------------

    }
}
