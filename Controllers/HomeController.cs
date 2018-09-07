using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BaseballScraper.Infrastructure;
using RDotNet;
using System;
using BaseballScraper.Models.FanGraphs;

namespace BaseballScraper.Controllers
{
#pragma warning disable CS0414, CS0169
    public class HomeController: Controller
    {
        private Constants _c = new Constants();

        // this is referencing the model
        private readonly AirtableConfiguration _airtableConfig;
        private readonly TwitterConfiguration _twitterConfiguration;
        private readonly ExcelMapper _eM     = new ExcelMapper();
        private readonly PythonConnector _pC = new PythonConnector();
        private readonly RdotNetConnector _r = new RdotNetConnector();
        private readonly DataTabler _dT      = new DataTabler();

        // public HomeController (IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig)
        // {
        //     _airtableConfig       = airtableConfig.Value;
        //     _twitterConfiguration = twitterConfig.Value;
        // }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("datatable")]
        public void DoDataTableThings()
        {
            _c.Start.ThisMethod();

            // _dT.GetModelProperties();

            _dT.CreateDataTable("BaseballScraper");
        }


        [HttpGet]
        [Route("mapper")]
        public void ConnectToMapperHome()
        {
            Type thisObjectsType = typeof(FGHitter);
        }

        [HttpGet]
        [Route("python/start")]

        public void ViewPythonHome()
        {
            var scope = _pC.ConnectToPythonFile("HelloWorld.py");
        }

        [HttpGet]
        [Route("r/pitchers")]
        public void CreatePitcherWinsVector()
        {
            _r.RPractice();
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


    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController: ControllerBase
    {
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}