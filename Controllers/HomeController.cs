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

namespace BaseballScraper.Controllers
{
#pragma warning disable CS0414, CS0169, CS0219
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


        public HomeController (IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig)
        {
            _airtableConfig       = airtableConfig.Value;
            _twitterConfiguration = twitterConfig.Value;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            _h.StartMethod();
            return View();
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