using System;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BaseballScraper.Controllers
{
#pragma warning disable CS0414
    public class HomeController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";

        // this is referencing the model
        private readonly AirtableConfiguration _airtableConfig;
        private readonly TwitterConfiguration _twitterConfiguration;


        public HomeController (IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig)
        {
            _airtableConfig       = airtableConfig.Value;
            _twitterConfiguration = twitterConfig.Value;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            Start.ThisMethod();

            ViewData["ApiKey2"] = _airtableConfig.ApiKey;

            ViewData["ConsumerKey"]       = _twitterConfiguration.ConsumerKey;
            ViewData["ConsumerSecret"]    = _twitterConfiguration.ConsumerSecret;
            ViewData["AccessToken"]       = _twitterConfiguration.AccessToken;
            ViewData["AccessTokenSecret"] = _twitterConfiguration.AccessTokenSecret;

            return View();
        }
    }
}