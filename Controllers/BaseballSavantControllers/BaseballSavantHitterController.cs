using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.BaseballSavant;
using BaseballScraper.Models.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly Helpers                       _helpers;
        private readonly CsvHandler                    _csvHandler;
        private readonly BaseballSavantHitterEndPoints _hitterEndpoints;
        private readonly BaseballScraperContext        _context;
        private readonly ProjectDirectoryEndPoints     _projectDirectory;


        public BaseballSavantHitterController(Helpers helpers, CsvHandler csvHandler, BaseballSavantHitterEndPoints hitterEndpoints, BaseballScraperContext context, ProjectDirectoryEndPoints projectDirectory)
        {
            _helpers          = helpers;
            _csvHandler       = csvHandler;
            _hitterEndpoints  = hitterEndpoints;
            this._context     = context;
            _projectDirectory = projectDirectory;
        }

        public BaseballSavantHitterController() {}



        // private static string _targetWriteDirectory = "BaseballData/02_Target_Write/BaseballSavant_Target_Write/BbSavant_Hitters/";


        // BaseballData/02_WRITE/BASEBALL_SAVANT/HITTERS/
        private string HitterWriteDirectory
        {
            get => _projectDirectory.BaseballSavantHitterWriteRelativePath;
        }


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
        }


        /*
            https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
        }


        // STATUS [ August 27, 2019 ] : this works (not sure about route though)
        [HttpPost("mrc")]
        public ActionResult MASTER_REPORT_CALLER(int year, int minAtBats)
        {
            _helpers.OpenMethod(1);
            DownloadAndAddXStats(year, minAtBats);
            DownloadAndAddExitVeloAndBarrels(year, minAtBats);
            return Ok();
        }


        // TWO PRIMARY REGIONS:
        // 1) Expected Statistics
        // 2) Expected Velocity and Barrels


        /* PRIMARY REGION 1 */
        #region EXPECTED STATISTICS ------------------------------------------------------------

            private static string _expectedStatsStringAppendix = "_x_stats.csv";


            /* --------------------------------------------------------------- */
            /* CSV - X-Stats                                                   */
            /* --------------------------------------------------------------- */


            // STATUS [ August 27, 2019 ] : this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/xstats/download_and_post
            [HttpPost("xstats/download_and_post")]
            public ActionResult DownloadAndAddXStats(int year, int minAtBats)
            {
                _helpers.OpenMethod(1);
                DownloadExpectedStatsCsv(year, minAtBats);
                string xStatsReportPath = "BaseballData/02_WRITE/BASEBALL_SAVANT/HITTERS/8_27_2019_x_stats.csv";
                var xList = CreateListOfXStatsObjectsFromCsvRows(xStatsReportPath).ToList();
                AddAll(xList);
                return Ok();
            }


            // STATUS [ August 27, 2019 ] : this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/xstats/csv/download
            [HttpGet("xstats/csv/download")]
            public IActionResult DownloadExpectedStatsCsv(int year, int minAtBats, BaseballSavantHitterEndPoints.BaseballSavantPositionEnum position = BaseballSavantHitterEndPoints.BaseballSavantPositionEnum.All)
            {
                _helpers.OpenMethod(1);
                var csvEndPoint = _hitterEndpoints.HitterExpectedStatisticsEndPoint_Csv(year, minAtBats, position).EndPointUri;

                // string pathAndFileToWrite = $"{_targetWriteDirectory}{_todaysDateString}{_expectedStatsStringAppendix}";
                string pathAndFileToWrite = $"{HitterWriteDirectory}{_todaysDateString}{_expectedStatsStringAppendix}";

                PrintCsvFileDownloadDetails(csvEndPoint, pathAndFileToWrite);

                _csvHandler.DownloadCsvFromLink(csvEndPoint, pathAndFileToWrite);

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


            // STATUS [ August 27, 2019 ] : this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/xstats/csv/list
            [HttpGet("xstats/csv/list")]
            public IList<XstatsHitter> CreateListOfXStatsObjectsFromCsvRows(string pathAndFileToWrite)
            {
                _helpers.OpenMethod(1);
                List<object> allRowsList = _csvHandler.ReadCsvRecordsToList(pathAndFileToWrite, typeof(XstatsHitter), typeof(XstatsHitterClassMap)).ToList();

                List<XstatsHitter> hitters = new List<XstatsHitter>();

                foreach(object row in allRowsList)
                {
                    XstatsHitter playerRow = row as XstatsHitter;
                    // C.WriteLine($"{playerRow.FirstName} {playerRow.LastName}");

                    var playerBase = _context.SfbbPlayerBases.SingleOrDefault(s => s.MLBID == playerRow.MLBID);

                    if(playerBase != null)
                    {
                        playerRow.IDPLAYER = playerBase.IDPLAYER;
                    }

                    hitters.Add(playerRow);
                    // Add(playerRow);
                    // _context.Add(playerRow);
                }
                // _context.SaveChanges();
                return hitters;
            }



            /* --------------------------------------------------------------- */
            /* CRUD - X-Stats                                                  */
            /* --------------------------------------------------------------- */


            /* ----- CRUD - CREATE - X-Stats ----- */

            // STATUS [ August 27, 2019 ] : this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/xstats/add
            [HttpPost("xstats/add")]
            public IActionResult Add(XstatsHitter hitter)
            {
                _helpers.StartMethod();
                var hitterCheck = _context.XStatsHitters.SingleOrDefault(h => h.MLBID == hitter.MLBID);

                if(hitterCheck == null)
                {
                    _context.Add(hitter);
                    _context.SaveChanges();
                }
                else
                {
                    // hitterCheck.PlateAppearances = hitter.PlateAppearances;
                    // Update(hitterCheck);
                }
                return Ok();
            }

            // STATUS [ August 27, 2019 ] : haven't tested
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/xstats/add_many
            [HttpPost("xstats/add_many")]
            public IActionResult AddMany(List<XstatsHitter> hitters)
            {
                _helpers.StartMethod();
                hitters.ForEach((hitter) => Add(hitter));
                return Ok();
            }


            // STATUS [ August 27, 2019 ] : should work but haven't tested
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/xstats/add_all
            [HttpPost("xstats/add_all")]
            public IActionResult AddAll(List<XstatsHitter> hitters)
            {
                _helpers.OpenMethod(1);
                int counter = 1;
                foreach(var hitter in hitters)
                {
                    var checkDbForHitter = _context.XStatsHitters.SingleOrDefault(x => x.MLBID == hitter.MLBID);
                    if(checkDbForHitter == null)
                    {
                        _context.Add(hitter);
                    }
                    else
                    {
                        // Console.WriteLine($"{counter} | X STATS HITTER ALREADY EXISTS");
                    }
                    counter++;
                }
                _context.SaveChanges();
                return Ok();
            }


            /* ----- CRUD - UPDATE - X-Stats ----- */

            // STATUS [ August 27, 2019 ] : haven't tested
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/xtats/put
            [HttpPut("xstats/put")]
            public ActionResult<XstatsHitter> Update(XstatsHitter hitter)
            {
                _context.Update(hitter);
                _context.SaveChanges();
                return Ok(hitter);
            }


        #endregion EXPECTED STATISTICS ------------------------------------------------------------





        /* PRIMARY REGION 2 */
        #region EXIT VELO & BARRELS HITTER ------------------------------------------------------------

            // See: https://atmlb.com/31IMZzg

            private static string _velocityAndBarrelsStringAppendix = "_exit_velocity_barrels.csv";


            /* --------------------------------------------------------------- */
            /* CSV - EXIT VELO & BARRELS                                       */
            /* --------------------------------------------------------------- */

            // STATUS [ August 27, 2019 ] : this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/download_and_post
            [HttpPost("velo_barrel/download_and_post")]
            public ActionResult DownloadAndAddExitVeloAndBarrels(int year, int minAtBats)
            {
                DownloadExitVelocityAndBarrelsCsv(year, minAtBats);
                string exitVeloReportPath = "BaseballData/02_WRITE/BASEBALL_SAVANT/HITTERS/8_27_2019_exit_velocity_barrels.csv";
                List<ExitVelocityAndBarrelsHitter> exitVeloList = CreateListOfExitVelocityAndBarrelsHittersFromCsvRows(exitVeloReportPath).ToList();
                AddAll(exitVeloList);
                return Ok();
            }


            // STATUS [ August 27, 2019 ] : this works
            // DownloadExitVelocityAndBarrelsCsv(2019, 100);
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/csv/download
            [HttpGet("velo_barrel/csv/download")]
            public IActionResult DownloadExitVelocityAndBarrelsCsv(int year, int minAtBats)
            {
                _helpers.OpenMethod(1);
                var csvEndPoint = _hitterEndpoints.HitterExitVelocityAndBarrelsEndPoint_Csv(year, minAtBats).EndPointUri;

                string pathAndFileToWrite = $"{HitterWriteDirectory}{_todaysDateString}{_velocityAndBarrelsStringAppendix}";

                _csvHandler.DownloadCsvFromLink(csvEndPoint, pathAndFileToWrite);

                PrintCsvFileDownloadDetails(csvEndPoint, pathAndFileToWrite);
                if(SIO.File.Exists(pathAndFileToWrite))
                {
                    IList<ExitVelocityAndBarrelsHitter> hitters = CreateListOfExitVelocityAndBarrelsHittersFromCsvRows(pathAndFileToWrite);
                }

                else
                {
                    Thread.Sleep(5000);
                    IList<ExitVelocityAndBarrelsHitter> hitters = CreateListOfExitVelocityAndBarrelsHittersFromCsvRows(pathAndFileToWrite);
                }
                return Ok();
            }


            // STATUS [ August 27, 2019 ] : this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/csv/list
            [HttpGet("velo_barrel/csv/list")]
            public IList<ExitVelocityAndBarrelsHitter> CreateListOfExitVelocityAndBarrelsHittersFromCsvRows(string pathAndFileToWrite)
            {
                _helpers.OpenMethod(1);
                List<object> allRowsList = _csvHandler.ReadCsvRecordsToList(pathAndFileToWrite, typeof(ExitVelocityAndBarrelsHitter), typeof(ExitVelocityAndBarrelsHitterClassMap)).ToList();

                List<ExitVelocityAndBarrelsHitter> hitters = new List<ExitVelocityAndBarrelsHitter>();

                foreach(object row in allRowsList)
                {
                    ExitVelocityAndBarrelsHitter playerRow = row as ExitVelocityAndBarrelsHitter;

                    SfbbPlayerBase playerBase = _context.SfbbPlayerBases.SingleOrDefault(s => s.MLBID == playerRow.MLBID);

                    if(playerBase != null)
                    {
                        playerRow.IDPLAYER = playerBase.IDPLAYER;
                    }
                    hitters.Add(playerRow);
                }
                return hitters;
            }


            /* --------------------------------------------------------------- */
            /* CRUD - EXIT VELO & BARRELS                                      */
            /* --------------------------------------------------------------- */


            /* ----- CRUD - CREATE - EXIT VELO & BARRELS ----- */

            // STATUS [ August 27, 2019 ] : not sure if this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/add
            [HttpPost("velo_barrel/add")]
            public IActionResult Add(ExitVelocityAndBarrelsHitter hitter)
            {
                _helpers.StartMethod();
                var hitterCheck = _context.ExitVelocityAndBarrelsHitters.SingleOrDefault(h => h.MLBID == hitter.MLBID);

                if(hitterCheck == null)
                {
                    C.WriteLine($"EXIT VELO: NEW");
                    _context.Add(hitter);
                    _context.SaveChanges();
                    return Ok(hitter);
                }
                if(hitterCheck.IDPLAYER == null)
                {
                    try
                    {
                        _context.Entry(hitter).Property("IDPLAYER").IsModified = true;
                        _context.Update(hitter);
                        _context.SaveChanges();
                        return Ok(hitter);
                    }
                    catch
                    {
                        C.WriteLine("CATCH");
                    }
                }

                else
                {
                    // _context.SaveChanges();
                }
                return Ok();
            }

            // STATUS [ August 27, 2019 ] : this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/add_many
            [HttpPost("velo_barrel/add_many")]
            public IActionResult AddMany(List<ExitVelocityAndBarrelsHitter> hitters)
            {
                hitters.ForEach((hitter) => Add(hitter));
                return Ok();
            }

            // STATUS [ August 27, 2019 ] : should work but haven't tested
            [HttpPost("velo_barrel/add_all_async")]
            public async Task<ActionResult> AddAllAsync(List<ExitVelocityAndBarrelsHitter> hitters)
            {
                _helpers.StartMethod();
                int counter = 1;
                foreach(var hitter in hitters)
                {
                    var checkDbForHitter = _context.ExitVelocityAndBarrelsHitters.SingleOrDefault(x => x.MLBID == hitter.MLBID);
                    if(checkDbForHitter == null)
                    {
                        _context.Add(hitter);
                    }
                    else
                    {
                        // Console.WriteLine($"{counter} | EXIT VELO AND BARREL HITTER ALREADY EXISTS");
                    }
                    counter++;
                }
                await _context.SaveChangesAsync();
                return Ok();
            }

            // STATUS [ August 27, 2019 ] : should work but haven't tested
            [HttpPost("velo_barrel/add_all")]
            public ActionResult AddAll(List<ExitVelocityAndBarrelsHitter> hitters)
            {
                _helpers.OpenMethod(1);
                int counter = 1;
                foreach(var hitter in hitters)
                {
                    var checkDbForHitter = _context.ExitVelocityAndBarrelsHitters.SingleOrDefault(x => x.MLBID == hitter.MLBID);
                    if(checkDbForHitter == null)
                    {
                        _context.Add(hitter);
                    }
                    else
                    {
                        // Console.WriteLine($"{counter} | EXIT VELO AND BARREL HITTER ALREADY EXISTS");
                    }
                    counter++;
                }
                _context.SaveChanges();
                return Ok();
            }


            /* ----- CRUD - READ - EXIT VELO & BARRELS ----- */

            // STATUS [ August 27, 2019 ] : should work but haven't tested
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/get
            [Produces(typeof(ExitVelocityAndBarrelsHitter))]
            [HttpGet("velo_barrel/get")]
            public ExitVelocityAndBarrelsHitter Get(int playerId)
            {
                ExitVelocityAndBarrelsHitter hitter = _context.ExitVelocityAndBarrelsHitters.SingleOrDefault(h => h.MLBID == playerId);
                return hitter;
            }


            // STATUS [ August 27, 2019 ] : should work but haven't tested
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/get_many
            [HttpGet("velo_barrel/get_many")]
            public List<ExitVelocityAndBarrelsHitter> GetMany(int[] playerIds)
            {
                var selectedHitters = _context.ExitVelocityAndBarrelsHitters.Where(t => playerIds.Contains(t.MLBID)).ToList();
                return selectedHitters;
            }


            /* ----- CRUD - UPDATE - EXIT VELO & BARRELS ----- */

            // STATUS [ August 27, 2019 ] : haven't tested
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/put
            [HttpPut("velo_barrel/put")]
            public ActionResult<ExitVelocityAndBarrelsHitter> Update(ExitVelocityAndBarrelsHitter hitter)
            {
                _context.Entry(hitter).Property("IDPLAYER").IsModified = true;
                // _context.Update(hitter);
                _context.SaveChanges();
                return Ok(hitter);
            }


            /* ----- CRUD - DELETE - EXIT VELO & BARRELS ----- */

            // STATUS [ August 27, 2019 ] : this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/delete
            [HttpDelete("velo_barrel/delete")]
            public ActionResult Delete(ExitVelocityAndBarrelsHitter hitter)
            {
                _helpers.StartMethod();
                _context.Remove(hitter);
                _context.SaveChanges();
                return Ok(hitter);
            }


            // STATUS [ August 27, 2019 ] : this works
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/delete_many
            [HttpDelete("velo_barrel/delete_many")]
            public ActionResult DeleteMany(List<ExitVelocityAndBarrelsHitter> hitters)
            {
                hitters.ForEach(hitter => Delete(hitter));
                return Ok();
            }


            // STATUS [ August 27, 2019 ] : should work but haven't tested
            // Link: https://127.0.0.1:5001/api/baseballsavant/baseballsavanthitter/velo_barrel/delete_all
            [HttpDelete("velo_barrel/delete_all")]
            public ActionResult DeleteAll(List<ExitVelocityAndBarrelsHitter> hitters)
            {
                _helpers.StartMethod();
                foreach(var hitter in hitters)
                {
                    _context.Remove(hitter);
                }
                _context.SaveChangesAsync();
                return Ok();
            }


        #endregion EXIT VELO & BARRELS HITTER ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            private void PrintCsvFileDownloadDetails(string csvEndPoint, string pathAndFileToWrite)
            {
                bool doesFileExistLocally = false;
                if(SIO.File.Exists(pathAndFileToWrite))
                {
                    doesFileExistLocally = true;
                }

                // C.WriteLine(SIO.File.Exists(pathAndFileToWrite) ? "FILE EXISTS?" : "X-Stats File does not exist");
                C.WriteLine($"\n--------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(BaseballSavantHitterController));
                C.WriteLine($"CSV LINK        : {csvEndPoint}");
                C.WriteLine($"LOCAL FILE PATH : {pathAndFileToWrite}");
                C.WriteLine($"FILE EXISTS?    : {doesFileExistLocally}");
                C.WriteLine($"--------------------------------------------\n");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
