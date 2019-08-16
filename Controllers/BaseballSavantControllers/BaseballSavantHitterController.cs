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
    public class BaseballSavantHitterController : ControllerBase
    {
        private readonly Helpers _helpers;
        private readonly CsvHandler _csvHandler;
        private readonly BaseballSavantHitterEndPoints _hitterEndpoints;
        private readonly BaseballScraperContext _context;


        public BaseballSavantHitterController(Helpers helpers, CsvHandler csvHandler, BaseballSavantHitterEndPoints hitterEndpoints, BaseballScraperContext context)
        {
            _helpers = helpers;
            _csvHandler = csvHandler;
            _hitterEndpoints = hitterEndpoints;
            this._context = context;
        }

        public BaseballSavantHitterController() {}



        private static string _targetWriteDirectory = "BaseballData/02_Target_Write/BaseballSavant_Target_Write/BbSavant_Hitters/";


        private string _todaysDateString
        {
            get
            {
                return _csvHandler.TodaysDateString();
            }
        }


        /*
            https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
            // DownloadCsvFromBaseballSavant();
            // DownloadExitVelocityAndBarrelsCsv(2019, 100);
            DownloadExpectedStatsCsv(2019, 100);
        }


        /*
            https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();

        }


            // STATUS [ July 16, 2019 ] : this works but a lot more needs to be done
            public void DownloadCsvFromBaseballSavant()
            {
                // BaseballData/02_Target_Write/BaseballSavant_Target_Write
                var endPoint = _hitterEndpoints.HitterEndPoint(minPlateAppearances: 100).EndPointUri;
                C.WriteLine(endPoint);
                _csvHandler.DownloadCsvFromLink(endPoint,"BaseballData/02_Target_Write/BaseballSavant_Target_Write/Bs_Hitters_xWOBA_07172019.csv");
            }





            #region EXPECTED STATISTICS ------------------------------------------------------------

                private static string _expectedStatsStringAppendix = "_x_stats.csv";


                /* --------------------------------------------------------------- */
                /* CSV - X-Stats                                                   */
                /* --------------------------------------------------------------- */


                [HttpPost("xstats/download_and_post")]
                public ActionResult DownloadAndAdd()
                {
                    _helpers.StartMethod();
                    DownloadExpectedStatsCsv(2019, 100);
                    _helpers.CompleteMethod();
                    return Ok();
                }



                [HttpGet("xstats/csv/download")]
                public IActionResult DownloadExpectedStatsCsv(int year, int minAtBats, BaseballSavantHitterEndPoints.BaseballSavantPositionEnum position = BaseballSavantHitterEndPoints.BaseballSavantPositionEnum.All)
                {
                    var csvEndPoint = _hitterEndpoints.HitterExpectedStatisticsEndPoint_Csv(year, minAtBats, position).EndPointUri;

                    string pathAndFileToWrite = $"{_targetWriteDirectory}{_todaysDateString}{_expectedStatsStringAppendix}";

                    PrintCsvFileDownloadDetails(csvEndPoint, pathAndFileToWrite);

                    _csvHandler.DownloadCsvFromLink(csvEndPoint, pathAndFileToWrite);

                    C.WriteLine(SIO.File.Exists(pathAndFileToWrite) ? "File exists" : "File does not exist");
                    if(SIO.File.Exists(pathAndFileToWrite))
                    {
                        IList<XstatsHitter> hitters = CreateListOfXStatsObjectsFromCsvRows(pathAndFileToWrite);
                    }

                    else
                    {
                        Thread.Sleep(5000);
                        IList<XstatsHitter> hitters = CreateListOfXStatsObjectsFromCsvRows(pathAndFileToWrite);
                    }
                    return Ok();
                }


                [HttpGet("xstats/csv/list")]
                public IList<XstatsHitter> CreateListOfXStatsObjectsFromCsvRows(string pathAndFileToWrite)
                {
                    List<object> allRowsList = _csvHandler.ReadCsvRecordsToList(pathAndFileToWrite, typeof(XstatsHitter), typeof(XstatsHitterClassMap)).ToList();

                    List<XstatsHitter> hitters = new List<XstatsHitter>();

                    foreach(object row in allRowsList)
                    {
                        XstatsHitter playerRow = row as XstatsHitter;
                        C.WriteLine(playerRow.PlayerId);
                        hitters.Add(playerRow);
                        Add(playerRow);
                    }
                    return hitters;
                }



                /* --------------------------------------------------------------- */
                /* CRUD - X-Stats                                                  */
                /* --------------------------------------------------------------- */


                /* ----- CRUD - CREATE - EXIT VELO & BARRELS ----- */

                // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/xstats/add
                [HttpPost("xstats/add")]
                public IActionResult Add(XstatsHitter hitter)
                {
                    // XstatsHitter checkForPlayer = _context.XStatsHitter.SingleOrDefault(h => h.PlayerId == hitter.PlayerId);
                    _context.Add(hitter);
                    _context.SaveChanges();

                    return Ok();
                }

                // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/xstats/add_many
                [HttpPost("xstats/add_many")]
                public IActionResult AddMany(List<XstatsHitter> hitters)
                {
                    hitters.ForEach((hitter) => Add(hitter));
                    return Ok();
                }



            #endregion EXPECTED STATISTICS ------------------------------------------------------------







            #region EXIT VELO & BARRELS HITTER ------------------------------------------------------------

                // See: https://atmlb.com/31IMZzg


                private static string _velocityAndBarrelsStringAppendix = "_exit_velocity_barrels.csv";


                /* --------------------------------------------------------------- */
                /* CSV - EXIT VELO & BARRELS                                       */
                /* --------------------------------------------------------------- */


                // DownloadExitVelocityAndBarrelsCsv(2019, 100);
                [HttpGet("velo_barrel/csv/download")]
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

                [HttpGet("velo_barrel/csv/list")]
                public IList<ExitVelocityAndBarrelsHitter> CreateListOfObjectsFromCsvRows(string pathAndFileToWrite)
                {
                    List<object> allRowsList = _csvHandler.ReadCsvRecordsToList(pathAndFileToWrite, typeof(ExitVelocityAndBarrelsHitter), typeof(ExitVelocityAndBarrelsHitterClassMap)).ToList();

                    List<ExitVelocityAndBarrelsHitter> hitters = new List<ExitVelocityAndBarrelsHitter>();

                    foreach(object row in allRowsList)
                    {
                        ExitVelocityAndBarrelsHitter playerRow = row as ExitVelocityAndBarrelsHitter;
                        hitters.Add(playerRow);
                    }
                    return hitters;
                }


                /* --------------------------------------------------------------- */
                /* CRUD - EXIT VELO & BARRELS                                      */
                /* --------------------------------------------------------------- */


                /* ----- CRUD - CREATE - EXIT VELO & BARRELS ----- */

                // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/add
                [HttpPost("velo_barrel/add")]
                public IActionResult Add(ExitVelocityAndBarrelsHitter hitter)
                {
                    ExitVelocityAndBarrelsHitter checkForPlayer = _context.ExitVelocityAndBarrelsHitter.SingleOrDefault(h => h.PlayerId == hitter.PlayerId);

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

                // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/add_many
                [HttpPost("velo_barrel/add_many")]
                public IActionResult AddMany(List<ExitVelocityAndBarrelsHitter> hitters)
                {
                    hitters.ForEach((hitter) => Add(hitter));
                    return Ok();
                }


                /* ----- CRUD - READ - EXIT VELO & BARRELS ----- */

                // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/get
                [Produces(typeof(ExitVelocityAndBarrelsHitter))]
                [HttpGet("velo_barrel/get")]
                public ExitVelocityAndBarrelsHitter Get(int playerId)
                {
                    ExitVelocityAndBarrelsHitter hitter = _context.ExitVelocityAndBarrelsHitter.SingleOrDefault(h => h.PlayerId == playerId);
                    return hitter;
                }

                // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/get_many
                [HttpGet("velo_barrel/get_many")]
                public List<ExitVelocityAndBarrelsHitter> GetMany(int[] playerIds)
                {
                    var selectedHitters = _context.ExitVelocityAndBarrelsHitter.Where(t => playerIds.Contains(t.PlayerId)).ToList();
                    return selectedHitters;
                }


                /* ----- CRUD - UPDATE - EXIT VELO & BARRELS ----- */

                // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/put
                [HttpPut("velo_barrel/put")]
                public ActionResult<ExitVelocityAndBarrelsHitter> Update(ExitVelocityAndBarrelsHitter hitter)
                {
                    _context.Update(hitter);
                    _context.SaveChanges();
                    return Ok(hitter);
                }


                /* ----- CRUD - DELETE - EXIT VELO & BARRELS ----- */

                // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/delete
                [HttpDelete("velo_barrel/delete")]
                public ActionResult Delete(ExitVelocityAndBarrelsHitter hitter)
                {
                    _context.Remove(hitter);
                    _context.SaveChanges();
                    return Ok(hitter);
                }


                // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/delete_many
                [HttpDelete("velo_barrel/delete_many")]
                public ActionResult DeleteMany(List<ExitVelocityAndBarrelsHitter> hitters)
                {
                    hitters.ForEach(hitter => Delete(hitter));
                    return Ok();
                }


            #endregion EXIT VELO & BARRELS HITTER ------------------------------------------------------------





            #region PRINTING PRESS ------------------------------------------------------------


                public void PrintCsvFileDownloadDetails(string csvEndPoint, string pathAndFileToWrite)
                {
                    C.WriteLine($"\n--------------------------------------------");
                    C.WriteLine($"EndPoint: {csvEndPoint}");
                    C.WriteLine($"File Path: {pathAndFileToWrite}");
                    C.WriteLine($"--------------------------------------------\n");
                }

            #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
