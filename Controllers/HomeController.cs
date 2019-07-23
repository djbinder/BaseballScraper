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

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController: ControllerBase
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


        public HomeController (Helpers helpers, RdotNetConnector r, IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig, GoogleSheetsConnector gSC, EmailHelper emailHelper, ExcelHandler excelHandler, PythonConnector pythonConnector, DataTabler dataTabler, CsvHandler csvHandler)
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
        }


        public HomeController(){}



        [Route("")]
        public void MainTest()
        {
            _helpers.StartMethod();
        }




    }
}
