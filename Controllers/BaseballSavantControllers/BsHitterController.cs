using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.BaseballSavant;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static BaseballScraper.EndPoints.BaseballSavantUriEndPoints;
using C = System.Console;
using SIO = System.IO;


#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.BaseballSavantControllers
{
    [Route("api/baseballsavant/[controller]")]
    [ApiController]
    // [ApiExplorerSettings(IgnoreApi = true)]
    public class BsHitterController : ControllerBase
    {
        private readonly Helpers _helpers;
        private readonly CsvHandler _csvHandler;
        private readonly BaseballSavantHitterEndPoints _hitterEndpoints;
        private readonly BaseballScraperContext _context;


        public BsHitterController(Helpers helpers, CsvHandler csvHandler, BaseballSavantHitterEndPoints hitterEndpoints, BaseballScraperContext context)
        {
            _helpers = helpers;
            _csvHandler = csvHandler;
            _hitterEndpoints = hitterEndpoints;
            this._context = context;
        }

        public BsHitterController() {}


        /*
            https://127.0.0.1:5001/api/baseballsavant/bshitter/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
            // DownloadCsvFromBaseballSavant();
            DownloadExitVelocityAndBarrelsCsv(2019, 100);
        }


        /*
            https://127.0.0.1:5001/api/baseballsavant/bshitter/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();

        }




        #region START ------------------------------------------------------------


            // STATUS [ July 16, 2019 ] : this works but a lot more needs to be done
            public void DownloadCsvFromBaseballSavant()
            {
                // BaseballData/02_Target_Write/BaseballSavant_Target_Write
                var endPoint = _hitterEndpoints.HitterEndPoint(minPlateAppearances: 100).EndPointUri;
                C.WriteLine(endPoint);
                _csvHandler.DownloadCsvFromLink(endPoint,"BaseballData/02_Target_Write/BaseballSavant_Target_Write/Bs_Hitters_xWOBA_07172019.csv");
            }


            private static string _targetWriteDirectory = "BaseballData/02_Target_Write/BaseballSavant_Target_Write/BbSavant_Hitters/";
            private static string _velocityAndBarrelsStringAppendix = "_exit_velocity_barrels.csv";

            private string _todaysDateString
            {
                get
                {
                    return _csvHandler.TodaysDateString();
                }
            }


            public IActionResult DownloadExitVelocityAndBarrelsCsv(int year, int minAtBats)
            {
                var csvEndPoint = _hitterEndpoints.HitterExitVelocityAndBarrelsEndPoint_Csv(year, minAtBats).EndPointUri;

                string pathAndFileToWrite = $"{_targetWriteDirectory}{_todaysDateString}{_velocityAndBarrelsStringAppendix}";

                _csvHandler.DownloadCsvFromLink(csvEndPoint, pathAndFileToWrite);

                C.WriteLine(SIO.File.Exists(pathAndFileToWrite) ? "File exists" : "File does not exist");
                if(SIO.File.Exists(pathAndFileToWrite))
                {
                    var hitters = CreateListOfObjectsFromCsvRows(pathAndFileToWrite);
                }

                else
                {
                    Thread.Sleep(5000);
                    var hitters = CreateListOfObjectsFromCsvRows(pathAndFileToWrite);
                }

                return Ok();
            }


            public IList<ExitVelocityAndBarrelsHitter> CreateListOfObjectsFromCsvRows(string pathAndFileToWrite)
            {
                IList<object> allRowsList = _csvHandler.ReadCsvRecordsToList(pathAndFileToWrite, typeof(ExitVelocityAndBarrelsHitter), typeof(ExitVelocityAndBarrelsHitterClassMap));

                List<ExitVelocityAndBarrelsHitter> hitters = new List<ExitVelocityAndBarrelsHitter>();

                    foreach(var row in allRowsList)
                    {
                        var playerRow = row as ExitVelocityAndBarrelsHitter;
                        hitters.Add(playerRow);
                        Add(playerRow);
                        // C.WriteLine(playerRow.FirstName);
                        // _context.Add(playerRow);
                        // _context.SaveChanges();
                    }
                return hitters;
            }



            /* --------------------------------------------------------------- */
            /* CRUD - CREATE - REST OF SEASON PROJECTION                       */
            /* --------------------------------------------------------------- */

            public IActionResult Add(ExitVelocityAndBarrelsHitter hitter)
            {
                var checkForPlayer = _context.ExitVelocityAndBarrelsHitter.SingleOrDefault(h => h.PlayerId == hitter.PlayerId);

                if(checkForPlayer.PlayerId > 0)
                {
                    C.WriteLine("record exists");
                }

                else
                {
                    C.WriteLine("record does NOT exist");
                    _context.Add(hitter);
                    _context.SaveChanges();
                }

                return Ok();
            }





            public void PrintCsvFileDownloadDetails(string csvEndPoint, string pathAndFileToWrite)
            {
                C.WriteLine($"\n--------------------------------------------");
                C.WriteLine($"EndPoint: {csvEndPoint}");
                C.WriteLine($"File Path: {pathAndFileToWrite}");
                C.WriteLine($"--------------------------------------------\n");
            }




        #endregion START ------------------------------------------------------------
    }

}
