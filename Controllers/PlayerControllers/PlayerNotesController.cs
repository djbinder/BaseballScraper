using System;
using BaseballScraper.Models;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace BaseballScraper.Controllers.PlayerControllers
{
    #pragma warning disable CS0414, CS0219
    [Route("player/form")]
    public class PlayerNotesController: Controller
    {
        private Constants _c = new Constants();
        private BaseballScraperContext _context;
        private readonly AirtableConfiguration _airtableConfig;


        public PlayerNotesController(BaseballScraperContext context, IOptions<AirtableConfiguration> airtableConfig)
        {
            _airtableConfig = airtableConfig.Value;
            _context        = context;
        }


        // [Route("notes")]
        public IActionResult ViewFormPage()
        {
            _c.Start.ThisMethod();
            _c.Complete.ThisMethod();
            return View("playernotes");
        }

        [HttpPost]
        [Route("createplayernote")]
        public IActionResult CreatePlayerNote()
        {
            _c.Start.ThisMethod();

            string hold = "hold";

            return Content(hold);

        }




    }
}