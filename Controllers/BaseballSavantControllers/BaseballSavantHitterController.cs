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


#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006, MA0016
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

        private CancellationToken cancellationToken = new CancellationToken();


        public BaseballSavantHitterController(Helpers helpers, CsvHandler csvHandler, BaseballSavantHitterEndPoints hitterEndpoints, BaseballScraperContext context, ProjectDirectoryEndPoints projectDirectory)
        {
            _helpers          = helpers;
            _csvHandler       = csvHandler;
            _hitterEndpoints  = hitterEndpoints;
            _context     = context;
            _projectDirectory = projectDirectory;
        }

        public BaseballSavantHitterController() {}




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


        // _expectedStatsStringAppendix = "_x_stats.csv";
        public string XStatsPathAndFileToWriteForToday
        {
            get => $"{HitterWriteDirectory}{_todaysDateString}{_expectedStatsStringAppendix}";
        }


        // (A) HitterWriteDirectory > BaseballData/02_WRITE/BASEBALL_SAVANT/HITTERS/
        // (B)
        // (C) _velocityAndBarrelsStringAppendix > "_exit_velocity_barrels.csv";
        public string ExitVeloPathAndFileToWriteForToday
        {
            get => $"{HitterWriteDirectory}{_todaysDateString}{_velocityAndBarrelsStringAppendix}";
        }

        // string xStatsReportPath = "BaseballData/02_WRITE/BASEBALL_SAVANT/HITTERS/8_27_2019_x_stats.csv";


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
        public ActionResult DAILY_REPORT_RUNNER(int year, int minAtBats)
        {
            _helpers.OpenMethod(1);

            var xStatsList = GetAllXStats_CSV(year, minAtBats);
            AddAll_DB(xStatsList);

            var exitVeloList = GetAllExitVelo_CSV(year, minAtBats);
            AddAll_DB(exitVeloList);

            return Ok();
        }


        // TWO PRIMARY REGIONS:
        // 1) Expected Statistics
        // 2) Expected Velocity and Barrels


        /* PRIMARY REGION 1 */
        #region EXPECTED STATISTICS ------------------------------------------------------------

            private static string _expectedStatsStringAppendix = "_x_stats.csv";


            /* --------------------------------------------------------------- */
            /* X-Stats - CSV                                                   */
            /* --------------------------------------------------------------- */


                // STATUS [ August 27, 2019 ] : this works
                // X-Stats
                public List<XstatsHitter> GetAllXStats_CSV(int year, int minAtBats)
                {
                    _helpers.OpenMethod(1);

                    DownloadExpectedStatsCsv(year, minAtBats);

                    // int monthNumber = 9; int dayNumber   = 1; int yearNumber  = 2019;

                    var xStatsHitters = new List<XstatsHitter>();

                    if(SIO.File.Exists(XStatsPathAndFileToWriteForToday))
                    {
                        xStatsHitters = GetAllXStats_CSV(XStatsPathAndFileToWriteForToday).ToList();
                    }

                    else
                    {
                        Thread.Sleep(5000);
                        xStatsHitters = GetAllXStats_CSV(XStatsPathAndFileToWriteForToday).ToList();
                    }

                    return xStatsHitters;
                }


                // STATUS [ August 27, 2019 ] : this works
                // X-Stats
                public IActionResult DownloadExpectedStatsCsv(int year, int minAtBats, BaseballSavantHitterEndPoints.BaseballSavantPositionEnum position = BaseballSavantHitterEndPoints.BaseballSavantPositionEnum.All)
                {
                    _helpers.OpenMethod(1);
                    var csvEndPoint = _hitterEndpoints.HitterExpectedStatisticsEndPoint_Csv(
                        year,
                        minAtBats,
                        position
                    ).EndPointUri;

                    _csvHandler.DownloadCsvFromLink(csvEndPoint, XStatsPathAndFileToWriteForToday);

                    PrintCsvFileDownloadDetails(csvEndPoint, XStatsPathAndFileToWriteForToday );

                    return Ok();
                }


                // STATUS [ August 27, 2019 ] : this works
                // X-Stats
                public IList<XstatsHitter> GetAllXStats_CSV(string pathAndFileToWrite)
                {
                    _helpers.OpenMethod(1);

                    List<object> allRowsInCsv = _csvHandler.ReadCsvRecordsToList(
                        pathAndFileToWrite,
                        typeof(XstatsHitter),
                        typeof(XstatsHitterClassMap)
                    ).ToList();

                    List<XstatsHitter> hitters = new List<XstatsHitter>();

                    foreach(object row in allRowsInCsv)
                    {
                        XstatsHitter playerRow = row as XstatsHitter;

                        var playerBase =_context.SfbbPlayerBases.SingleOrDefault(s => s.MLBID == playerRow.MLBID);

                        if(playerBase != null)
                            playerRow.IDPLAYER = playerBase.IDPLAYER;

                        hitters.Add(playerRow);
                    }
                    return hitters;
                }



            /* --------------------------------------------------------------- */
            /* X-Stats - CRUD                                                  */
            /* --------------------------------------------------------------- */


            /* ----- CRUD - CREATE - X-Stats ----- */

                // // STATUS [ August 27, 2019 ] : this works
                // // X-Stats
                // public IActionResult AddOne_DB(XstatsHitter hitter)
                // {
                //     _helpers.OpenMethod(1);
                //     int countAdded = 0; int countNotAdded = 0;

                //     // XStatsHitter
                //     var checkForHitterInDb = _context.XStatsHitters.SingleOrDefault(h => h.MLBID == hitter.MLBID);

                //     var nullCheck      = (checkForHitterInDb == null) ? _context.Add(hitter) : null;
                //     int manageCounters = (checkForHitterInDb == null) ? countAdded++ : countNotAdded++;

                //     // if(hitterCheck == null)
                //     // {
                //     //     _context.Add(hitter);
                //     // }
                //     // else
                //     // {
                //     //     // hitterCheck.PlateAppearances = hitter.PlateAppearances;
                //     //     // Update(hitterCheck);
                //     // }
                //     _context.SaveChanges();

                //     _context.PrintDatabaseAddOutcomes(
                //         countAdded,
                //         countNotAdded,
                //         typeof(BaseballSavantHitterController)
                //     );

                //     return Ok();
                // }



                // STATUS [ August 27, 2019 ] : should work but haven't tested
                // X-Stats
                public IActionResult AddAll_DB(List<XstatsHitter> hitters)
                {
                    _helpers.OpenMethod(1);
                    int countAdded = 0; int countNotAdded = 0; int countUpdated = 0;

                    foreach(var hitter in hitters)
                    {
                        // XstatsHitter
                        var checkForHitterInDb = _context.XStatsHitters.SingleOrDefault(x => x.MLBID == hitter.MLBID);

                        if(checkForHitterInDb == null)
                        {
                            _context.Entry(hitter).State = EntityState.Added;
                            _context.Set<XstatsHitter>().Add(hitter);
                        }

                        else
                        {
                            _context.Entry(checkForHitterInDb).State = EntityState.Unchanged;
                        }

                        int manageCounters = (checkForHitterInDb == null) ? countAdded++ : countNotAdded++;
                    }
                    _context.SaveChanges();

                    _context.PrintDatabaseAddOutcomes(countAdded, countNotAdded, typeof(BaseballSavantHitterController));

                    C.WriteLine($"Update: {countUpdated}");
                    return Ok();
                }


            /* ----- CRUD - UPDATE - X-Stats ----- */

                // STATUS [ August 27, 2019 ] : haven't tested
                // X-Stats
                public ActionResult<XstatsHitter> UpdateOne_DB(XstatsHitter hitter)
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
            /* EXIT VELO & BARRELS - CSV                                       */
            /* --------------------------------------------------------------- */

                // STATUS [ August 27, 2019 ] : this works
                // Exit Velo & Barrels
                public List<ExitVelocityAndBarrelsHitter> GetAllExitVelo_CSV(int year, int minAtBats)
                {
                    _helpers.OpenMethod(1);

                    DownloadExitVelocityAndBarrelsCsv(year, minAtBats);

                    var exitVeloList = new List<ExitVelocityAndBarrelsHitter>();

                    if(SIO.File.Exists(ExitVeloPathAndFileToWriteForToday))
                    {
                        exitVeloList = GetAllExitVelo_CSV(ExitVeloPathAndFileToWriteForToday).ToList();
                    }

                    else
                    {
                        Thread.Sleep(5000);
                        exitVeloList = GetAllExitVelo_CSV(ExitVeloPathAndFileToWriteForToday).ToList();
                    }

                    return exitVeloList;
                }


                // STATUS [ August 27, 2019 ] : this works
                // Exit Velo & Barrels
                public IActionResult DownloadExitVelocityAndBarrelsCsv(int year, int minAtBats)
                {
                    _helpers.OpenMethod(1);
                    var csvEndPoint = _hitterEndpoints.HitterExitVelocityAndBarrelsEndPoint_Csv(
                        year,
                        minAtBats
                    ).EndPointUri;

                    _csvHandler.DownloadCsvFromLink(csvEndPoint, ExitVeloPathAndFileToWriteForToday);

                    PrintCsvFileDownloadDetails(csvEndPoint, ExitVeloPathAndFileToWriteForToday);
                    return Ok();
                }


                // STATUS [ August 27, 2019 ] : this works
                // Exit Velo & Barrels
                public IList<ExitVelocityAndBarrelsHitter> GetAllExitVelo_CSV(string pathAndFileToWrite)
                {
                    _helpers.OpenMethod(1);

                    List<object> allRowsInCsv = _csvHandler.ReadCsvRecordsToList(
                        pathAndFileToWrite,
                        typeof(ExitVelocityAndBarrelsHitter),
                        typeof(ExitVelocityAndBarrelsHitterClassMap)
                    ).ToList();

                    var exitVeloList = new List<ExitVelocityAndBarrelsHitter>();

                    foreach(object row in allRowsInCsv)
                    {
                        ExitVelocityAndBarrelsHitter playerRow = row as ExitVelocityAndBarrelsHitter;

                        SfbbPlayerBase playerBase =_context.SfbbPlayerBases.SingleOrDefault(s => s.MLBID == playerRow.MLBID);

                        if(playerBase != null)
                            playerRow.IDPLAYER = playerBase.IDPLAYER;

                        exitVeloList.Add(playerRow);
                    }
                    return exitVeloList;
                }


            /* --------------------------------------------------------------- */
            /* CRUD - EXIT VELO & BARRELS                                      */
            /* --------------------------------------------------------------- */


            /* ----- CRUD - CREATE - EXIT VELO & BARRELS ----- */


                // STATUS [ August 27, 2019 ] : should work but haven't tested
                // Exit Velo & Barrels
                public async Task<ActionResult> AddAllAsync_DB(List<ExitVelocityAndBarrelsHitter> hitters)
                {
                    _helpers.OpenMethod(3);
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
                    await _context.SaveChangesAsync(cancellationToken);
                    return Ok();
                }


                // STATUS [ August 27, 2019 ] : should work but haven't tested
                // Exit Velo & Barrels
                public ActionResult AddAll_DB(List<ExitVelocityAndBarrelsHitter> hitters)
                {
                    _helpers.OpenMethod(1);

                    int countAdded = 0; int countNotAdded = 0; int countUpdated = 0;

                    foreach(var hitter in hitters)
                    {
                        var checkDbForHitter = _context.ExitVelocityAndBarrelsHitters.SingleOrDefault(x => x.MLBID == hitter.MLBID);

                        if(checkDbForHitter == null)
                        {
                            _context.Entry(hitter).State = EntityState.Added;
                            _context.Set<ExitVelocityAndBarrelsHitter>().Add(hitter);
                        }
                        else
                        {
                            _context.Entry(checkDbForHitter).State = EntityState.Unchanged;
                        }
                        int manageCounters = (checkDbForHitter == null) ? countAdded++ : countNotAdded++;
                    }
                    _context.PrintDatabaseAddOutcomes(countAdded, countNotAdded, typeof(BaseballSavantHitterController));
                    _context.SaveChanges();
                    return Ok();
                }



            /* ----- CRUD - READ - EXIT VELO & BARRELS ----- */

                // STATUS [ August 27, 2019 ] : should work but haven't tested
                // Exit Velo & Barrels
                public ExitVelocityAndBarrelsHitter GetOne_DB(int playerId)
                {
                    _helpers.OpenMethod(1);
                    ExitVelocityAndBarrelsHitter hitter =
                        _context.ExitVelocityAndBarrelsHitters.SingleOrDefault(h => h.MLBID == playerId);

                    return hitter;
                }


                // STATUS [ August 27, 2019 ] : should work but haven't tested
                // Exit Velo & Barrels
                public List<ExitVelocityAndBarrelsHitter> GetMany_DB(int[] playerIds)
                {
                    _helpers.OpenMethod(1);
                    List<ExitVelocityAndBarrelsHitter> selectedHitters =
                        _context.ExitVelocityAndBarrelsHitters.Where(t => playerIds.Contains(t.MLBID)).ToList();

                    return selectedHitters;
                }



            /* ----- CRUD - UPDATE - EXIT VELO & BARRELS ----- */

                // STATUS [ August 27, 2019 ] : haven't tested
                // Exit Velo & Barrels
                public ActionResult<ExitVelocityAndBarrelsHitter> UpdateOne_DB(ExitVelocityAndBarrelsHitter hitter)
                {
                    _helpers.OpenMethod(1);
                    _context.Entry(hitter).Property("IDPLAYER").IsModified = true;
                    _context.SaveChanges();
                    return Ok(hitter);
                }



            /* ----- CRUD - DELETE - EXIT VELO & BARRELS ----- */

                // STATUS [ August 27, 2019 ] : this works
                // Exit Velo & Barrels
                public ActionResult DeleteOne_DB(ExitVelocityAndBarrelsHitter hitter)
                {
                    _helpers.OpenMethod(1);
                    _context.Remove(hitter);
                    _context.SaveChanges();
                    return Ok(hitter);
                }


                // STATUS [ August 27, 2019 ] : this works
                // Exit Velo & Barrels
                public ActionResult DeleteMany_DB(List<ExitVelocityAndBarrelsHitter> hitters)
                {
                    _helpers.OpenMethod(1);
                    hitters.ForEach(hitter => DeleteOne_DB(hitter));
                    return Ok();
                }


                // STATUS [ August 27, 2019 ] : should work but haven't tested'
                // Exit Velo & Barrels
                public ActionResult DeleteAll(List<ExitVelocityAndBarrelsHitter> hitters)
                {
                    _helpers.OpenMethod(1);
                    foreach(var hitter in hitters)
                    {
                        _context.Remove(hitter);
                    }
                    _context.SaveChangesAsync(cancellationToken);
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


            public void PrintDatabaseAddOutcomes(int countAdded, int countNotAdded)
            {
                C.WriteLine($"\n-------------------------------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(BaseballSavantHitterController));
                C.WriteLine($"ADDED TO DB   : {countAdded}");
                C.WriteLine($"ALREADY IN DB : {countNotAdded}");
                C.WriteLine($"-------------------------------------------------------------------\n");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}



// // STATUS [ August 27, 2019 ] : haven't tested
// // X-Stats
// public IActionResult AddMany_DB(List<XstatsHitter> hitters)
// {
//     _helpers.StartMethod();
//     hitters.ForEach((hitter) => AddOne_DB(hitter));
//     return Ok();
// }




// // STATUS [ August 27, 2019 ] : this works
// // Exit Velo & Barrels
// public IActionResult AddMany_DB(List<ExitVelocityAndBarrelsHitter> hitters)
// {
//     _helpers.OpenMethod(1);
//     hitters.ForEach((hitter) => AddOne_DB(hitter));
//     return Ok();
// }






// // STATUS [ August 27, 2019 ] : not sure if this works
// // Exit Velo & Barrels
// public IActionResult AddOne_DB(ExitVelocityAndBarrelsHitter hitter)
// {
//     _helpers.OpenMethod(1);
//     int countAdded = 0; int countNotAdded = 0;

//     var hitterCheck = _context.ExitVelocityAndBarrelsHitters.SingleOrDefault(h => h.MLBID == hitter.MLBID);

//     if(hitterCheck == null)
//     {
//         C.WriteLine($"EXIT VELO: NEW");
//         _context.Add(hitter);
//         _context.SaveChanges();
//         return Ok(hitter);
//     }
//     if(hitterCheck.IDPLAYER == null)
//     {
//         try
//         {
//             _context.Entry(hitter).Property("IDPLAYER").IsModified = true;
//             _context.Update(hitter);
//             _context.SaveChanges();
//             return Ok(hitter);
//         }
//         catch
//         {
//             C.WriteLine("CATCH");
//         }
//     }

//     else
//     {
//         // _context.SaveChanges();
//     }

//     _context.PrintDatabaseAddOutcomes(
//         countAdded,
//         countNotAdded,
//         typeof(BaseballSavantHitterController)
//     );

//     return Ok();
// }
