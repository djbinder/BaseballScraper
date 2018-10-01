using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers.Player
{
    #pragma warning disable CS0414, CS0219
    public class PlayerBaseController: Controller
    {
        private Helpers _h = new Helpers();
        private ExcelMapper _eM = new ExcelMapper();


        [HttpGet]
        [Route("playerbase")]
        public IActionResult ViewPlayerBaseHome()
        {
            _eM.GetAllRecordsInSheet("BaseballData/PlayerBase.xlsm", "master");

            string content = "player note main content";

            return Content(content);
        }
    }
}