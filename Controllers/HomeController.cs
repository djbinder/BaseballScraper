using System;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers
{
    public class HomeController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            Start.ThisMethod();
            return View();
        }


        protected void Submit_Click (object sender, EventArgs e)
        {
            Extensions.Spotlight("Button was clicked");
        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            Extensions.Spotlight("Button was clicked");
        }

        [HttpGet]
        [Route("DoSomething")]
        public IActionResult DoSomething()
        {
            Start.ThisMethod();
            return Content($"Consumer Secret Is");
        }
    }
}