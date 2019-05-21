using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BaseballScraper.Infrastructure;
using RDotNet;
using System;
using BaseballScraper.Models.FanGraphs;
using CsvHelper;
using System.IO;
using System.Threading.Tasks;
using BaseballScraper.Models.Lahman;
using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;

namespace BaseballScraper.Controllers
{
#pragma warning disable CS0414, CS0169, CS0219, IDE0052, CS1591
    public class HomeController: Controller
    {
        private readonly Helpers _h = new Helpers();
        private readonly AirtableConfiguration _airtableConfig;
        private readonly TwitterConfiguration _twitterConfiguration;
        private readonly ExcelHandler _eM     = new ExcelHandler();
        private readonly PythonConnector _pC = new PythonConnector();
        private readonly RdotNetConnector _r = new RdotNetConnector();
        private readonly DataTabler _dT      = new DataTabler();
        private readonly CsvHandler _cH      = new CsvHandler();
        private readonly HtmlScraper _hS = new HtmlScraper();

        private readonly GoogleSheetsConnector _gSC = new GoogleSheetsConnector();


        public HomeController (IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig)
        {
            _airtableConfig       = airtableConfig.Value;
            _twitterConfiguration = twitterConfig.Value;
        }


        // [HttpGet]
        // [Route("")]
        // public IActionResult Index()
        // {
        //     Console.WriteLine("INDEX");
        //     return RedirectToAction("OnGet","Pages/Dashboard.cshtml.cs");
        // }

        public void MainTest()
        {
            Console.WriteLine("test");
        }


        [HttpGet("google_sheets")]
        public void ConnectToGoogleSheetsConnector()
        {
            _h.StartMethod();
            Console.WriteLine("connecting to google_sheets");
            _gSC.ConnectToGoogle();

            // List<IList<object>> data
            List<IList<object>> objectsListMain = new List<IList<object>>();

            List<object> objects = new List<object>();

            var object1 = "CUBS!!!";
            objects.Add(object1);
            objects.Add("CUBS2");
            objects.Add("CUBS3");

            List<object> objectsList2 = new List<object>();
            objectsList2.Add("BREWERS");
            objectsList2.Add("cards");



            objectsListMain.Add(objects);
            objectsListMain.Add(objectsList2);

            Console.WriteLine();
            Console.WriteLine("HOME > starting UpdateData");
            Console.WriteLine();

            _gSC.UpdateData(objectsListMain);
        }







        [HttpGet]
        [Route("datatable")]
        public void DoDataTableThings()
        {

        }

        [HttpGet]
        [Route("mapper")]
        public void ConnectToMapperHome()
        {

        }

        [HttpGet]
        [Route("r/pitchers")]
        public void CreatePitcherWinsVector()
        {
            _r.GetLahmanPlayerInfo("betts");
            // _r.GetLahmanTeamInfo("CH");
        }

        [HttpGet]
        [Route("logging")]
        public IActionResult Logging()
        {
            // Log.Logger = new LoggerConfiguration()
            // .MinimumLevel.Debug()
            // .WriteTo.Console()
            // .WriteTo.File("Logs//BaseballScraperLog.txt", rollingInterval: RollingInterval.Day)
            // .CreateLogger();

            // // Log.Information("Hello, world!");
            // Log.Information("The time is {Now}", DateTime.Now);

            // Log.CloseAndFlush();

            // var config = new NLog.Config.LoggingConfiguration();

            // var logfile    = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            // var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            // config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // NLog.LogManager.Configuration = config;

            return View();
        }
    }



}
