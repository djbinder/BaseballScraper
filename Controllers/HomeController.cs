using System;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NLog;
using Serilog;

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

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("Logs//BaseballScraperLog.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            Log.Information("Hello, world!");
            Log.Information("The time is {Now}", DateTime.Now);

            Log.CloseAndFlush();

            var config = new NLog.Config.LoggingConfiguration();

            var logfile    = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            return View();
        }
    }
}