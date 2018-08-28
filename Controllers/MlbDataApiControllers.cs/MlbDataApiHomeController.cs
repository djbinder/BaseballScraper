using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.MlbDataApi;
using BaseballScraper.Infrastructure;

namespace BaseballScraper.Controllers.MlbDataApiControllers
{
    [Route("api/mlb")]
    [ApiController]
    public class MlbDataApiHomeController: Controller
    {
        private Constants _c = new Constants();

        private static MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();

        private static PostmanMethods _postman = new PostmanMethods();

        public IActionResult ViewMlbDataApiPage ()
        {
            _c.Start.ThisMethod();
            string message = "viewing mlb data api main page";
            return Content($"Message is {message}");
        }
    }


}