using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BaseballScraper.Controllers.CbsControllers;
using BaseballScraper.Controllers.EspnControllers;
using BaseballScraper.Controllers.YahooControllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Cbs;
using BaseballScraper.Models.ConfigurationModels;
using BaseballScraper.Models.Yahoo;
using SharpPad;
using GraphQL.Types;
using GraphQL;
using C = System.Console;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers
{
    public class HomeController: Controller
    {
        private readonly Helpers               _helpers;
        private readonly RdotNetConnector      _r;
        private readonly AirtableConfiguration _airtableConfig;
        private readonly TwitterConfiguration  _twitterConfiguration;
        private readonly GoogleSheetsConnector _gSC;
        private readonly EmailHelper           _emailHelper;
        private readonly ExcelHandler          _excelHander;
        private readonly PythonConnector       _pythonConnector;
        private readonly DataTabler            _dataTabler;
        private readonly CsvHandler            _csvHandler;

        private readonly CbsTransactionTrendsController   _cbsTrendsController;
        private readonly EspnTransactionTrendsController  _espnTrendsController;
        private readonly YahooTransactionTrendsController _yahooTrendsController;


        public HomeController (Helpers helpers, RdotNetConnector r, IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig, GoogleSheetsConnector gSC, EmailHelper emailHelper, ExcelHandler excelHandler, PythonConnector pythonConnector, DataTabler dataTabler, CsvHandler csvHandler, CbsTransactionTrendsController cbsTrendsController)
        {
            _helpers              = helpers;
            _r                    = r;
            _airtableConfig       = airtableConfig.Value;
            _twitterConfiguration = twitterConfig.Value;
            _gSC                  = gSC;
            _emailHelper          = emailHelper;
            _excelHander          = excelHandler;
            _pythonConnector      = pythonConnector;
            _dataTabler           = dataTabler;
            _csvHandler           = csvHandler;
            _cbsTrendsController  = cbsTrendsController;

            // this._cbsTrendsController = cbsTrendsController;
        }


        public HomeController(){}



        // STATUS [ July 24, 2019 ]: this works and is needed for transactions trends method below
        public HomeController(CbsTransactionTrendsController cbsTrendsController, EspnTransactionTrendsController espnTrendsController, YahooTransactionTrendsController yahooTrendsController)
        {
            _cbsTrendsController   = cbsTrendsController;
            _espnTrendsController  = espnTrendsController;
            _yahooTrendsController = yahooTrendsController;
        }




        public int XUnitPractice(int number)
        {
            int newNumber = number * 3;
            C.WriteLine($"newNumber : {newNumber}");
            C.WriteLine("hold");
            return newNumber;
        }


        /// <summary>
        /// The home controller
        /// </summary>
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            C.WriteLine("INDEX!");
            int indexNumber = XUnitPractice(300);
            C.WriteLine($"indexNumber : {indexNumber}");
            return View("Dashboard");

        }


        [HttpGet("dashboard")]
        public IActionResult ViewDashboard()
        {
            C.WriteLine("DASHBOARD");

            ISchema schema = Schema.For(@"
                type Query {
                    hello: String
                }
            ");

            var root = new { Hello = "Hello World!" };
            string json = schema.Execute(_ =>
            {
                _.Query = "{ hello }";
                _.Root = root;
            });

            C.WriteLine(json);

            return View("Dashboard");
        }


        [HttpGet("tester_1")]
        public IActionResult GenericTesterOne()
        {
            C.WriteLine("HomeController > GenericTesterOne");
            return Ok();
        }

        [HttpGet("tester_2")]
        public IActionResult GenericTesterTwo()
        {
            C.WriteLine("HomeController > GenericTesterTwo");
            return Ok();
        }

        [HttpGet("tester_3")]
        public IActionResult GenericTesterThree()
        {
            C.WriteLine("HomeController > GenericTesterThree");
            return Ok();
        }





        [HttpGet("player_bases")]
        public IActionResult ViewPlayerBases()
        {
            C.WriteLine("HomeController > ViewPlayerBases()");
            return View("PlayerBases");
        }


        // private const string cbsUrlForMostAddedAllFootball = "https://www.cbssports.com/fantasy/football/trends/added/all";

        private const string cbsUrlForMostAddedAllBaseball = "https://www.cbssports.com/fantasy/baseball/trends/added/all";




        // STATUS [ July 24, 2019 ]: CBS and Yahoo can get the data; ESPN can't; Not sure if the view works
        // see 'views_code.txt' file for cshtml code
        [HttpGet("transaction_trends")]
        public IActionResult ViewTransactionTrends()
        {
            C.WriteLine("TransactionTrends");

            List<CbsMostAddedOrDroppedPlayer> cbsPlayers    = _cbsTrendsController.GetListOfCbsMostAddedOrDropped(cbsUrlForMostAddedAllBaseball);

            C.WriteLine($"cbsPlayers.Count: {cbsPlayers.Count}");

            // List<EspnTransactionTrendPlayer> espnPlayers    = _espnTrendsController.GetListOfMostAddedPlayers();

            List<YahooTransactionTrendsPlayer> yahooPlayers = _yahooTrendsController.GetTrendsForTodayAllPositions();
            C.WriteLine($"yahooPlayers.Count: {yahooPlayers.Count}");


            return View("TransactionTrends");
        }








    }
}
