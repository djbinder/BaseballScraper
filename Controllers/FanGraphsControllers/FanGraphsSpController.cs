using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.FanGraphs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using C = System.Console;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.FanGraphsControllers
{
    [Route("api/fangraphs/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FanGraphsSpController : ControllerBase
    {
        private readonly Helpers                      _helpers;
        private readonly FanGraphsUriEndPoints        _endPoints;
        private readonly GoogleSheetsConnector        _googleSheetsConnector;
        private readonly CsvHandler                   _csvHandler;
        private readonly BaseballScraperContext       _context;
        private readonly ProjectDirectoryEndPoints    _projectDirectory;

        public System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();



        public FanGraphsSpController(Helpers helpers, FanGraphsUriEndPoints endPoints, GoogleSheetsConnector googleSheetsConnector, CsvHandler csvHandler, BaseballScraperContext context, ProjectDirectoryEndPoints projectDirectory)
        {
            _helpers               = helpers;
            _endPoints             = endPoints;
            _googleSheetsConnector = googleSheetsConnector;
            _csvHandler            = csvHandler;
            _context               = context;
            _projectDirectory      = projectDirectory;
        }

        public FanGraphsSpController(){}





        // Defined by FanGraphs
        // SEE: https://bit.ly/33abnet
        // * Outcome A: Out of Zone / Swung On / No Contact
        // * Outcome B: Out of Zone / Swung On / Contact Made
        // * Outcome C: Out of Zone / No Swing
        // * Outcome D: In Zone / Swung On / No Contact
        // * Outcome E: In Zone / Swung On / Contact Made
        // * Outcome F: In Zone / No Swing


        // wPDI weights
        // * Defined by FanGraphs
        // See: https://bit.ly/33abnet
        public double OutcomeA_index = 1;           // Out of Zone / Swung On / No Contact
        public double OutcomeB_index = .65;         // Out of Zone / Swung On / Contact Made
        public double OutcomeC_index = .1;          // Out of Zone / No Swing
        public double OutcomeD_index = .900;        // In Zone / Swung On / No Contact
        public double OutcomeE_index = 0;           // In Zone / Swung On / Contact Made
        public double OutcomeF_index = .8;          // In Zone / No Swing


        // mPDI weights
        // * Defined by FanGraphs
        // See: https://bit.ly/2YPh1TU
        public double madduxOutcomeA_index = 1;     // Out of Zone / Swung On / No Contact
        public double madduxOutcomeB_index = 1;     // Out of Zone / Swung On / Contact Made
        public double madduxOutcomeC_index = 0;     // Out of Zone / No Swing
        public double madduxOutcomeD_index = 0;     // In Zone / Swung On / No Contact
        public double madduxOutcomeE_index = 0;     // In Zone / Swung On / Contact Made
        public double madduxOutcomeF_index = 1;     // In Zone / No Swing


        private string _wPdiReportPrefix  = "SpWpdiReport";
        private string _wPdiSheetName     = "wPDI";
        private string _wPdiSheetRange    = "A4:MN10004";
        private string _wPdiJsonGroupName = "wPDI";


        // BaseballData/02_WRITE/FANGRAPHS/PITCHERS/
        private string PitcherWriteDirectory
        {
            get => _projectDirectory.FanGraphsPitcherWriteRelativePath;
        }



        /*
            https://127.0.0.1:5001/api/fangraphs/FgSpWpdiReport/test
        */
        [HttpGet("test")]
        [ApiExplorerSettings(IgnoreApi = false)]
        public void TestController()
        {
            _helpers.StartMethod();
            _googleSheetsConnector.SetGradientConditionalFormat();
        }


        /*
            https://127.0.0.1:5001/api/fangraphs/FgSpWpdiReport/async
        */
        [HttpGet("async")]
        [ApiExplorerSettings(IgnoreApi = false)]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();

            await RunFgSpWpdiReport(2019, minInningsPitched_:1);
            // var listFromDatabase = GetMany_DB(season: 2019, minInningsPitched: 100);
            // AddManyToGsheet(listFromDatabase);

            _helpers.CompleteMethod();
        }





        // Controller Purpose:
        //  * Retrieve data needed to calculate wPDI and mPDI from FanGraphs
        //  * Add wPDI / mPDI data to project database
        //  * Calculate wPDI and mPDI
        //  * Query database
        //  * Add wPDI / mPDI data to CSV or Google Sheet

        // A) Retrieve data from FanGraphs
        // * If it does not already exist in database:
        // *    1) Go to FanGraphs url / endPoint
        // *    2) Download CSV file to local downloads folder
        // *    3) Move CSV file from local downloads folder to project BaseballData folder
        // *    4) Add data in CSV to database
        // * If data is in database:
        //      1) Query data in database
        //      2) Add data to google sheet if wanted / appropriate


        public async Task<IActionResult> DAILY_REPORT_RUNNER()
        {
            _helpers.OpenMethod(3);
            await RunFgSpWpdiReport(2019, minInningsPitched_:1);
            return Ok();
        }




        #region FgSpWpdiReport PRIMARY METHOD ------------------------------------------------------------

            // STEP 0: Check if there is already a Csv file created for today
            // STEP 1: set report parameters and download CSV to local downloads folder
            // STEP 2: move the CSV from local downloads folder to project Target_Write folder
            // STEP 3: add each pitcher to database OR update pitcher in database
            public async Task<ActionResult> RunFgSpWpdiReport(int season_, int minInningsPitched_=1)
            {
                _helpers.OpenMethod(3);

                // if CSV doesn't exist
                if(CheckIfCsvFileForTodayExists() == false)
                {
                    await DownloadCsvToLocalDownloads(
                        minInningsPitched_: minInningsPitched_
                    );

                    MoveCsvToProjectFolder(_wPdiReportPrefix);
                }

                List<FanGraphsPitcherWpdi> pitcherList = GetAll_CSV(season_);
                AddAll_DB(pitcherList, season_);

                // Query database
                List<FanGraphsPitcherWpdi> listFromDatabase = GetMany_DB(
                    season:season_,
                    minInningsPitched:minInningsPitched_
                );

                foreach(FanGraphsPitcherWpdi pitcher in listFromDatabase)
                {
                    double mPDI = CalculateSpMpdi(
                        pitcher.ZonePercentage,
                        pitcher.OSwingPercentage,
                        pitcher.OContactPercentage,
                        pitcher.ZSwingPercentage
                    );
                }
                PrintWpdiReportPrimaryMethodInfo(CheckIfCsvFileForTodayExists(), season_, minInningsPitched_);
                return Ok();
            }


        #endregion FgSpWpdiReport PRIMARY METHOD ------------------------------------------------------------





        #region GOOGLE SHEETS ------------------------------------------------------------


            /*
                Example:
                var listFromDatabase = GetMany(season: 2019, minInningsPitched: 100);
                AddManyToGsheet(listFromDatabase);
            */
            public ActionResult AddMany_GSheet(List<FanGraphsPitcherWpdi> listOfPitchers)
            {
                List<IList<object>> listOfLists = new List<IList<object>>();

                int counter = 1;
                IList<object> headers = new List<object>();

                foreach(var player in listOfPitchers)
                {
                    PropertyInfo[] playerProperties = player.GetType().GetProperties();
                    if(counter == 1)
                    {
                        foreach(var property in playerProperties)
                        {
                            string propertyInfoName = property.Name;
                            headers.Add(propertyInfoName);
                        }
                        listOfLists.Add(headers);
                    }

                    IList<object> statValues = new List<object>();

                    foreach(var propertyInfo in playerProperties)
                    {
                        var value = propertyInfo.GetValue(player);
                        statValues.Add(value);
                    }
                    counter++;
                    listOfLists.Add(statValues);
                }
                _googleSheetsConnector.WriteGoogleSheetRows(listOfLists, "wPDI", "A4:MN10004","wPDI");
                return Ok();
            }


            public async Task<ActionResult> AddManyAsync_GSheet(List<FanGraphsPitcherWpdi> listOfPitchers)
            {
                List<IList<object>> listOfLists = new List<IList<object>>();

                int counter = 1;
                IList<object> headers = new List<object>();

                foreach(var player in listOfPitchers)
                {
                    PropertyInfo[] playerProperties = player.GetType().GetProperties();
                    if(counter == 1)
                    {
                        foreach(var property in playerProperties)
                        {
                            string propertyInfoName = property.Name;
                            headers.Add(propertyInfoName);
                        }
                        listOfLists.Add(headers);
                    }

                    IList<object> statValues = new List<object>();

                    foreach(var propertyInfo in playerProperties)
                    {
                        var value = propertyInfo.GetValue(player);
                        statValues.Add(value);
                    }
                    counter++;
                    listOfLists.Add(statValues);
                }
                await _googleSheetsConnector.WriteGoogleSheetRowsAsync(listOfLists, "wPDI", "A4:MN10004","wPDI");
                return Ok();
            }

        #endregion GOOGLE SHEETS ------------------------------------------------------------





        #region FANGRAPHS CSV ------------------------------------------------------------

            // 1) Check if CSV Exists
            // 2) Download CSV to Local Downloads folder
            // 3) Move CSV to project folder
            // 4) Get List of pitchers from CSV



            /* --------------------------------------------------------------- */
            /* CURRENT SEASON / IN-SEASON                                      */
            /* --------------------------------------------------------------- */

            // STATUS [ July 31, 2019 ] : this works
            // STEP 0: Check if there is already a Csv file created for today
            // * Returns 'false' if file doesn't exist; returns 'true' if it does
            // * If the file already exists, you do not need to run the report again
            public bool CheckIfCsvFileForTodayExists()
            {
                _helpers.OpenMethod(1);

                var fileInfo = new DirectoryInfo(PitcherWriteDirectory).GetFiles();

                DateTime today = DateTime.Now;
                int year       = today.Year;
                int month      = today.Month;
                int day        = today.Day;

                string fileName = $"{_wPdiReportPrefix}_{month}_{day}_{year}.csv";

                bool doesCsvReportExistForToday = false;

                foreach(FileInfo file in fileInfo)
                {
                    if(file.Name == fileName)
                        doesCsvReportExistForToday = true;
                }
                return doesCsvReportExistForToday;
            }


            // STATUS [ July 31, 2019 ] : this works
            // STEP 1: set report parameters and download CSV to local downloads folder
            // * Goes to a FanGraphs page and downloads the CSV from that page to local downloads folder
            // * Ultimately the CSV should be moved to project data folder (See MoveFgSpWpdiReportToTargetFolder() method)
            // * This should not run if a Csv file for current day already exists
            // * Example endPoint:
            // *    https://www.fangraphs.com/leaders.aspx?pos=all&stats=sta&lg=all&qual=150&type=c,8,13,210,204,205,208,207,111,105,106,109,108&season=2018&month=0&season1=2018&ind=0&team=0&rost=0&age=0&filter=&players=0&startdate=2018-01-01&enddate=2018-12-31
            public async Task DownloadCsvToLocalDownloads(
                int minInningsPitched_ = 0,
                int year_              = 0,
                int startMonth_        = 0,
                int startDay_          = 0,
                int endMonth_          = 0,
                int endDay_            = 0
            )
            {
                _helpers.OpenMethod(3);
                var fgUrl = _endPoints.StartingPitcherWpdiEndPoint(
                    minInningsPitched : minInningsPitched_,
                    year              : year_,
                    startMonth        : startMonth_,
                    startDay          : startDay_,
                    endMonth          : endMonth_,
                    endDay            : endDay_
                ).EndPointUri.ToString();

                var csvSelector = _endPoints.FanGraphsCsvHtmlSelector();

                await _csvHandler.ClickLinkToDownloadCsvFile(
                    fgUrl,
                    csvSelector
                );
            }


            // STATUS [ July 31, 2019 ] : this works
            // STEP 2: move the CSV from local downloads folder to project Target_Write folder
            // * Looks for the file that was last updated in local downloads
            // * This should not run if a Csv file for current day already exists
            // * Once it finds last updated file, it moves and renames the file
            // * Ends up something like: "SpWpdiReport_07_09_2019.csv"
            public ActionResult MoveCsvToProjectFolder(string fileNamePrefix, int reportMonth = 0, int reportYear  = 0,int reportDay = 0)
            {
                _helpers.OpenMethod(1);
                string downloadsFolder   = _endPoints.LocalDownloadsFolderLocation();

                _csvHandler.MoveCsvFileToFolder(
                    downloadsFolder,
                    PitcherWriteDirectory,
                    fileNamePrefix,
                    month:reportMonth,
                    year:reportYear,
                    day:reportDay
                );
                PrintCsvMoveInfo(downloadsFolder, PitcherWriteDirectory, fileNamePrefix, reportMonth, reportDay, reportYear);
                return Ok();
            }


            // STATUS [ July 31, 2019 ] : this works
            // Create list of Wpdi Pitchers from CSV
            // * Uses the last updated CSV
            public List<FanGraphsPitcherWpdi> GetAll_CSV(int season)
            {
                _helpers.OpenMethod(1);

                string fileToRead = _csvHandler.GetPathToLastUpdatedFileInFolder(PitcherWriteDirectory);

                JObject records = _csvHandler.ReadCsvRecordsToJObject(
                    fileToRead,
                    typeof(FanGraphsPitcherWpdi),
                    typeof(WpdiReportClassMap)
                );

                var pitchers = new List<FanGraphsPitcherWpdi>();
                var pitcher  = new FanGraphsPitcherWpdi();

                foreach(KeyValuePair<string, JToken> kvp in records)
                {
                    JToken allPitchers = kvp.Value;
                    int countOfPitchers = allPitchers.Count();

                    for(var recordCounter = 0; recordCounter < countOfPitchers; recordCounter++)
                    {
                        JToken currentPitcher = allPitchers[recordCounter];
                        pitcher = CreateNewWpdiPitcherInstance(currentPitcher, season);
                        pitchers.Add(pitcher);
                    }
                }
                return pitchers;
            }




        #endregion FANGRAPHS CSV ------------------------------------------------------------





        #region DATABASE ------------------------------------------------------------


            /* --------------------------------------------------------------- */
            /* CRUD - WPDI PITCHER                                             */
            /* --------------------------------------------------------------- */

            /* ----- CRUD - CREATE - WPDI PITCHER ----- */


            // STATUS [ July 31, 2019 ] : this works
            // STEP 3: add each pitcher to database OR update pitcher in database
            // * Checks if record for pitcher already exists
            public async Task AddOne_DB(FanGraphsPitcherWpdi pitcher, int season)
            {
                _helpers.OpenMethod(3);
                pitcher.Season = season;

                // type is FanGraphsPitcherForWpdiReport
                var exists = _context.FanGraphsPitchersForWpdiReport.SingleOrDefault(sp => sp.PlayerYearConcat == pitcher.PlayerYearConcat);

                if(exists != null)
                {
                    try
                    {
                        _context.Update(exists);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    catch { C.WriteLine("Error either adding or updating pitcher"); }
                }
                else
                {
                    await _context.AddAsync(pitcher);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }


            public IActionResult AddAll_DB(List<FanGraphsPitcherWpdi> pitchers, int season)
            {
                _helpers.OpenMethod(1);

                int countAdded = 0; int countNotAdded = 0;
                foreach(var sp in pitchers)
                {
                    sp.Season = season;

                    var checkDbForPitcher = _context.FanGraphsPitchersForWpdiReport.SingleOrDefault(p => p.PlayerYearConcat == sp.PlayerYearConcat);

                    var nullCheck = checkDbForPitcher == null ? _context.Add(sp) : null;
                    int manageCounters = (checkDbForPitcher == null) ? countAdded++ : countNotAdded++;

                    // if(checkDbForPitcher == null)
                    //     _context.Add(sp);
                }
                PrintDatabaseAddOutcomes(countAdded, countNotAdded);
                _context.SaveChanges();
                return Ok();
            }



            /* ----- CRUD - READ - WPDI PITCHER ----- */


            // STATUS [ August 1, 2019 ] : haven't tested if this works
            public FanGraphsPitcherWpdi GetOne_DB(int fangraphsid)
            {
                var pitcher = _context.FanGraphsPitchersForWpdiReport.SingleOrDefault(p => p.FanGraphsId == fangraphsid);
                return pitcher;
            }


            // STATUS [ July 31, 2019 ] : this works
            // Query database for pitchers and their wPDIs
            public List<FanGraphsPitcherWpdi> GetMany_DB(int season, int minInningsPitched = 0)
            {
                _helpers.OpenMethod(1);
                var pitchers = _context.FanGraphsPitchersForWpdiReport
                    .OrderByDescending(w => w.Wpdi)
                    .Where(
                        y => y.Season == season &&
                        y.InningsPitched > minInningsPitched
                    ).ToList();

                // PrintPitcherWpdiBasics(pitchers);
                return pitchers;
            }

            /* ----- CRUD - UPDATE - WPDI PITCHER ----- */


            // need to add


            /* ----- CRUD - DELETE - WPDI PITCHER ----- */

            // STATUS [ August 1, 2019 ] : haven't tested if this works
            public async Task<IActionResult> DeleteOne_DB(int fangraphsid)
            {
                var pitcher = await _context.FanGraphsPitchersForWpdiReport.SingleOrDefaultAsync(p => p.FanGraphsId == fangraphsid);
                _context.Remove(pitcher);
                await _context.SaveChangesAsync(cancellationToken);
                return Ok();
            }


            // STATUS [ July 31, 2019 ] : this works
            // Delete pitchers in database from list
            // Example:
            /*
                var pitcherList = RetrieveSpWpdiFromDatabaseToList(season:2018);
                DeleteAllRecordsInDatabase(pitcherList);
            */
            public ActionResult DeleteMany(List<FanGraphsPitcherWpdi> pitchers)
            {
                foreach(var pitcher in pitchers)
                {
                    _context.Remove(pitcher);
                }
                _context.SaveChanges();
                return Ok();
            }

        #endregion DATABASE ------------------------------------------------------------




        #region PREVIOUS SEASONS ------------------------------------------------------------

            // DON'T DELETE - SHOULD BE RE-WORKED

            // Only needs to be run once ever
            // Uses CSV files from target_write folder to players to database
            // public async Task AddPreviousSeasons_DB()
            // {
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2018);
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2017);
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2016);
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2015);
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2014);
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2013);
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2012);
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2011);
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2010);
            //     await MoveDataFromFanGraphsToDatabaseFullSeasonAsync(2009);
            // }


            // // STATUS [ August 31, 2019 ] : this should work but haven't tested
            // public async Task AddAllPreviousSeasons()
            // {
            //     await RunFgSpWpdiReport(2018, minInningsPitched_:1);
            //     await RunFgSpWpdiReport(2017, minInningsPitched_:1);
            //     await RunFgSpWpdiReport(2016, minInningsPitched_:1);
            //     await RunFgSpWpdiReport(2015, minInningsPitched_:1);
            //     await RunFgSpWpdiReport(2014, minInningsPitched_:1);
            // }


            // // CSVs should exist for 2009 - 2019
            // // * File name format: 'SpWpdiReport_XXXX.csv' where XXXX is the year
            // public async Task MoveDataFromFanGraphsToDatabaseFullSeasonAsync(int year)
            // {
            //     // type is List<FanGraphsPitcherForWpdiReport>
            //     var pitcherList = CreateListOfWpdiPitchersFromCsv(year);

            //     AddAll(pitcherList, year);
            //     foreach(var pitcher in pitcherList)
            //     {
            //         pitcher.Season = year;
            //         await AddOne_DB(pitcher, year);
            //     }
            // }

            // CSVs should exist for 2009 - 2019
            // * File name format: 'SpWpdiReport_XXXX.csv' where XXXX is the year
            // public IActionResult MoveDataFromFanGraphsToDatabaseFullSeason(int year)
            // {
            //     List<FanGraphsPitcherForWpdiReport> pitcherList = CreateListOfWpdiPitchersFromCsv(year);
            //     AddAll(pitcherList, year);
            //     return Ok();
            // }

        #endregion PREVIOUS SEASONS ------------------------------------------------------------





        #region CREATE FanGraphsPitcherWpdi INSTANCE ------------------------------------------------------------


            /* --------------------------------------------------------------- */
            /* CREATE INSTANCE                                                 */
            /* --------------------------------------------------------------- */

            // STATUS [ July 31, 2019 ] : this works
            // Create instance of FanGraphsPitcherForWpdiReport
            // * Conversions of strings to double occurs within the model
            public FanGraphsPitcherWpdi CreateNewWpdiPitcherInstance(JToken currentPitcher, int season)
            {
                DateTime today = DateTime.Now;
                FanGraphsPitcherWpdi pitcher = new FanGraphsPitcherWpdi
                {
                    FanGraphsId                 = (int)currentPitcher     ["FanGraphsId"           ],
                    PitcherName                 = currentPitcher          ["PitcherName"           ].ToString(),
                    Team                        = currentPitcher          ["Team"                  ].ToString(),
                    GamesStarted                = (int)currentPitcher     ["GamesStarted"          ],
                    InningsPitched              = (decimal)currentPitcher ["InningsPitched"        ],

                    ZonePercentageString        = currentPitcher          ["ZonePercentage"        ].ToString(),
                    ZonePercentage              = (double)currentPitcher  ["ZonePercentage"        ],

                    OSwingPercentageString      = currentPitcher          ["OSwingPercentage"      ].ToString(),
                    OSwingPercentage            = (double)currentPitcher  ["OSwingPercentage"      ],

                    ZSwingPercentageString      = currentPitcher          ["ZSwingPercentage"      ].ToString(),
                    ZSwingPercentage            = (double)currentPitcher  ["ZSwingPercentage"      ],

                    ZContactPercentageString    = currentPitcher          ["ZContactPercentage"    ].ToString(),
                    ZContactPercentage          = (double)currentPitcher  ["ZContactPercentage"    ],

                    OContactPercentageString    = currentPitcher          ["OContactPercentage"    ].ToString(),
                    OContactPercentage          = (double)currentPitcher  ["OContactPercentage"    ],

                    ZonePercentageStringPfx     = currentPitcher          ["ZonePercentagePfx"     ].ToString(),
                    ZonePercentagePfx           = (double)currentPitcher  ["ZonePercentagePfx"     ],

                    OSwingPercentageStringPfx   = currentPitcher          ["OSwingPercentagePfx"   ].ToString(),
                    OSwingPercentagePfx         = (double)currentPitcher  ["OSwingPercentagePfx"   ],

                    ZSwingPercentageStringPfx   = currentPitcher          ["ZSwingPercentagePfx"   ].ToString(),
                    ZSwingPercentagePfx         = (double)currentPitcher  ["ZSwingPercentagePfx"   ],

                    ZContactPercentageStringPfx = currentPitcher          ["ZContactPercentagePfx" ].ToString(),
                    ZContactPercentagePfx       = (double)currentPitcher  ["ZContactPercentagePfx" ],

                    OContactPercentageStringPfx = currentPitcher          ["OContactPercentagePfx" ].ToString(),
                    OContactPercentagePfx       = (double)currentPitcher  ["OContactPercentagePfx" ],
                };

                var pitcherApercentage = pitcher.Apercentage = CalculateApercentage(
                    pitcher.ZonePercentage,
                    pitcher.OSwingPercentage,
                    pitcher.OContactPercentage
                );

                var pitcherBpercentage = pitcher.Bpercentage = CalculateBpercentage(
                    pitcher.ZonePercentage,
                    pitcher.OSwingPercentage,
                    pitcher.OContactPercentage
                );

                var pitcherCpercentage = pitcher.Cpercentage = CalculateCpercentage(
                    pitcher.ZonePercentage,
                    pitcher.OSwingPercentage
                );

                var pitcherDpercentage = pitcher.Dpercentage = CalculateDpercentage(
                    pitcher.ZonePercentage,
                    pitcher.ZSwingPercentage,
                    pitcher.ZContactPercentage
                );

                var pitcherEpercentage = pitcher.Epercentage = CalculateEpercentage(
                    pitcher.ZonePercentage,
                    pitcher.ZSwingPercentage,
                    pitcher.ZContactPercentage
                );

                var pitcherFpercentage = pitcher.Fpercentage = CalculateFpercentage(
                    pitcher.ZonePercentage,
                    pitcher.ZSwingPercentage
                );

                pitcher.OutcomeApercentage = CalculateFinalOutcomePercentage(OutcomeA_index, pitcherApercentage);
                pitcher.OutcomeBpercentage = CalculateFinalOutcomePercentage(OutcomeB_index, pitcherBpercentage);
                pitcher.OutcomeCpercentage = CalculateFinalOutcomePercentage(OutcomeC_index, pitcherCpercentage);
                pitcher.OutcomeDpercentage = CalculateFinalOutcomePercentage(OutcomeD_index, pitcherDpercentage);
                pitcher.OutcomeEpercentage = CalculateFinalOutcomePercentage(OutcomeE_index, pitcherEpercentage);
                pitcher.OutcomeFpercentage = CalculateFinalOutcomePercentage(OutcomeF_index, pitcherFpercentage);

                pitcher.Wpdi = CalculateSpWpdi(
                    pitcher.ZonePercentage,
                    pitcher.OSwingPercentage,
                    pitcher.OContactPercentage,
                    pitcher.ZSwingPercentage,
                    pitcher.ZContactPercentage
                );

                pitcher.OutcomeApercentage_mPDI = CalculateApercentage(
                    pitcher.ZonePercentage,
                    pitcher.OSwingPercentage,
                    pitcher.OContactPercentage
                );

                pitcher.OutcomeBpercentage_mPDI = CalculateBpercentage(
                    pitcher.ZonePercentage,
                    pitcher.OSwingPercentage,
                    pitcher.OContactPercentage
                );

                pitcher.OutcomeFpercentage_mPDI = CalculateFpercentage(
                    pitcher.ZonePercentage,
                    pitcher.ZSwingPercentage
                );

                pitcher.Mpdi = CalculateSpMpdi(
                    pitcher.ZonePercentage,
                    pitcher.OSwingPercentage,
                    pitcher.OContactPercentage,
                    pitcher.ZSwingPercentage
                );

                if(season == 0)
                    pitcher.Season = today.Year;

                else
                    pitcher.Season = season;

                return pitcher;
            }


            /* --------------------------------------------------------------- */
            /* CALCULATE WPDI METRICS                                          */
            /* --------------------------------------------------------------- */

            // STATUS [ July 31, 2019 ] : these all work
            // * Formulas defined by FanGraphs
            // * Formulas for components:
            // > * A% = Out of Zone / Swung On / No Contact = (1 – Zone%) * (O-Swing%) * (1 – O-Contact%)
            // > * B% = Out of Zone / Swung On / Contact Made = (1 – Zone%) * (O-Swing%) * O-Contact%
            // > * C% = Out of Zone / No Swing = (1 – Zone%) * (1- O-Swing%)
            // > * D% = In Zone / Swung On / No Contact = (Zone%) * (Z-Swing%) * (1 – Z-Contact%)
            // > * E% = In Zone / Swung On / Contact Made = (Zone%) * (Z-Swing%) * Z-Contact%
            // > * F% = In Zone / No Swing = (Zone%) * (1- Z-Swing%)


            /* ----- CALCULATE INDIVIDUAL WPDI COMPONENTS ----- */

            public double CalculateApercentage(double zonePercentage, double oSwingPercentage, double oContactPercentage)
            {
                double aPercentage = (1 - zonePercentage) * (oSwingPercentage) * (1 - oContactPercentage);
                return aPercentage;
            }

            public double CalculateBpercentage(double zonePercentage, double oSwingPercentage, double oContactPercentage)
            {
                double bPercentage = (1 - zonePercentage) * oSwingPercentage * oContactPercentage;
                return bPercentage;
            }

            public double CalculateCpercentage(double zonePercentage, double oSwingPercentage)
            {
                double cPercentage = (1 - zonePercentage) * (1 - oSwingPercentage);
                return cPercentage;
            }

            public double CalculateDpercentage(double zonePercentage, double zSwingPercentage, double zContactPercentage)
            {
                double dPercentage = zonePercentage * zSwingPercentage * (1 - zContactPercentage);
                return dPercentage;
            }

            public double CalculateEpercentage(double zonePercentage, double zSwingPercentage, double zContactPercentage)
            {
                double ePercentage = zonePercentage * zSwingPercentage * zContactPercentage;
                return ePercentage;
            }

            public double CalculateFpercentage(double zonePercentage, double zSwingPercentage)
            {
                double fPercentage = zonePercentage * (1 - zSwingPercentage);
                return fPercentage;
            }

            public double CalculateFinalOutcomePercentage(double outcomeIndex, double outcomePercentage)
            {
                double finalValue = outcomeIndex * outcomePercentage;
                return finalValue;
            }


            /* ----- CALCULATE WPDI ----- */

            public double CalculateSpWpdi(double zonePercentage, double oSwingPercentage, double oContactPercentage, double zSwingPercentage, double zContactPercentage)
            {
                double aPercentage = CalculateApercentage(zonePercentage, oSwingPercentage, oContactPercentage);
                double bPercentage = CalculateBpercentage(zonePercentage, oSwingPercentage, oContactPercentage);
                double cPercentage = CalculateCpercentage(zonePercentage, oSwingPercentage);
                double dPercentage = CalculateDpercentage(zonePercentage, zSwingPercentage, zContactPercentage);
                double ePercentage = CalculateEpercentage(zonePercentage, zSwingPercentage, zContactPercentage);
                double fPercentage = CalculateFpercentage(zonePercentage, zSwingPercentage);


                double finalA = OutcomeA_index * aPercentage;
                double finalB = OutcomeB_index * bPercentage;
                double finalC = OutcomeC_index * cPercentage;
                double finalD = OutcomeD_index * dPercentage;
                double finalE = OutcomeE_index * ePercentage;
                double finalF = OutcomeF_index * fPercentage;


                var wPdi =
                    (OutcomeA_index * aPercentage) +
                    (OutcomeB_index * bPercentage) +
                    (OutcomeC_index * cPercentage) +
                    (OutcomeD_index * dPercentage) +
                    (OutcomeE_index * ePercentage) +
                    (OutcomeF_index * fPercentage);

                // PrintWpdiPercentage(aPercentage, bPercentage, cPercentage, dPercentage, ePercentage, fPercentage);
                // PrintIndividualOutcomes(finalA, finalB, finalC, finalD, finalE, finalF);
                return wPdi;
            }


            // SEE: https://fantasy.fangraphs.com/introducing-maddux-plate-discipline-index-mpdi-for-pitchers/
            // C, D, E are not included because they are weighted as 0
            public double CalculateSpMpdi(double zonePercentage, double oSwingPercentage, double oContactPercentage, double zSwingPercentage)
            {
                double aPercentage = CalculateApercentage(zonePercentage, oSwingPercentage, oContactPercentage);
                double bPercentage = CalculateBpercentage(zonePercentage, oSwingPercentage, oContactPercentage);
                double fPercentage = CalculateFpercentage(zonePercentage, zSwingPercentage);

                double finalA = madduxOutcomeA_index * aPercentage;
                double finalB = madduxOutcomeB_index * bPercentage;
                double finalF = madduxOutcomeF_index * fPercentage;

                var mPDI = finalA + finalB + finalF;
                return mPDI;
            }


        #endregion CREATE FanGraphsPitcherWpdi INSTANCE ------------------------------------------------------------







        #region PRINTING PRESS ------------------------------------------------------------

            private void PrintPitcherWpdiBasics(List<FanGraphsPitcherWpdi> pitchers)
            {
                int counter = 1;
                foreach(var pitcher in pitchers)
                {
                    C.Write($"{counter}.");
                    PrintPitcherWpdiBasics(pitcher);
                    counter++;
                }
            }

            private void PrintPitcherWpdiBasics(FanGraphsPitcherWpdi pitcher)
            {
                Console.WriteLine($"{pitcher.PitcherName}\t\t{pitcher.InningsPitched}\t{pitcher.Wpdi}");
            }

            private void PrintAllPitcherWpdiData(FanGraphsPitcherWpdi pitcher)
            {
                C.WriteLine($"\n-----------------------------------------------------------------");
                C.WriteLine($"\n{pitcher.PitcherName}\t {pitcher.Team}\t {pitcher.FanGraphsId}");
                C.WriteLine($"GAMES: {pitcher.GamesStarted}\t IP: {pitcher.InningsPitched}");

                C.WriteLine($"WPDI: {pitcher.Wpdi}");

                C.WriteLine($"\nZONE%   : {pitcher.ZonePercentage}");
                C.WriteLine($"O-SWING%   : {pitcher.OSwingPercentage}");
                C.WriteLine($"Z-SWING%   : {pitcher.ZSwingPercentage}");
                C.WriteLine($"O-CONTACT% : {pitcher.OContactPercentage}");
                C.WriteLine($"Z-CONTACT% : {pitcher.ZContactPercentage}");

                C.WriteLine($"\naPercentage : {pitcher.Apercentage}");
                C.WriteLine($"bPercentage :   {pitcher.Bpercentage}");
                C.WriteLine($"cPercentage :   {pitcher.Cpercentage}");
                C.WriteLine($"dPercentage :   {pitcher.Dpercentage}");
                C.WriteLine($"ePercentage :   {pitcher.Epercentage}");
                C.WriteLine($"fPercentage :   {pitcher.Fpercentage}");

                C.WriteLine($"\nfinalA : {pitcher.OutcomeApercentage}");
                C.WriteLine($"finalB :   {pitcher.OutcomeBpercentage}");
                C.WriteLine($"finalC :   {pitcher.OutcomeCpercentage}");
                C.WriteLine($"finalD :   {pitcher.OutcomeDpercentage}");
                C.WriteLine($"finalE :   {pitcher.OutcomeEpercentage}");
                C.WriteLine($"finalF :   {pitcher.OutcomeFpercentage}");

                C.WriteLine($"\n-----------------------------------------------------------------\n");
            }


            private void PrintPitcherPlateDiscriplineData(FanGraphsPitcherWpdi pitcher)
            {
                C.WriteLine($"\n{pitcher.PitcherName}\t {pitcher.Team}\t {pitcher.FanGraphsId}");
                C.WriteLine($"GAMES      : {pitcher.GamesStarted}\t IP: {pitcher.InningsPitched}");
                C.WriteLine($"ZONE%      : {pitcher.ZonePercentage}");
                C.WriteLine($"O-SWING%   : {pitcher.OSwingPercentage}\t Z-SWING%: {pitcher.ZSwingPercentage}");
                C.WriteLine($"O-CONTACT% : {pitcher.OContactPercentage}\t Z-CONTACT%: {pitcher.ZContactPercentage}\n");
            }


            private void PrintWpdiPercentage(double aPercentage, double bPercentage, double cPercentage, double dPercentage, double ePercentage, double fPercentage)
            {
                C.WriteLine($"\naPercentage : {aPercentage}");
                C.WriteLine($"bPercentage : {bPercentage}");
                C.WriteLine($"cPercentage : {cPercentage}");
                C.WriteLine($"dPercentage : {dPercentage}");
                C.WriteLine($"ePercentage : {ePercentage}");
                C.WriteLine($"fPercentage : {fPercentage}\n");
            }

            private void PrintIndividualOutcomes(double finalA, double finalB, double finalC, double finalD, double finalE, double finalF)
            {
                C.WriteLine($"\nfinalA : {finalA}");
                C.WriteLine($"finalB : {finalB}");
                C.WriteLine($"finalC : {finalC}");
                C.WriteLine($"finalD : {finalD}");
                C.WriteLine($"finalE : {finalE}");
                C.WriteLine($"finalF : {finalF}\n");
            }


            private void PrintCsvMoveInfo(string movingFromDirectory, string movingToDirectory, string fileNamePrefix, int reportMonth, int reportDay, int reportYear)
            {
                C.WriteLine($"\n--------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(FanGraphsSpController));
                C.WriteLine($"MOVING FROM : {movingFromDirectory}");
                C.WriteLine($"MOVING TO   : {movingToDirectory}");
                C.WriteLine($"PREFIX      : {fileNamePrefix}");
                C.WriteLine($"REPORT DATE : {reportMonth}.{reportDay}.{reportYear}");
                C.WriteLine($"--------------------------------------------\n");
            }


            private void PrintWpdiReportPrimaryMethodInfo(bool doesCsvReportExistForToday, int season, int minInningsPitched)
            {
                C.WriteLine($"\n--------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(FanGraphsSpController));
                C.WriteLine($"REPORT DETAILS          : SEASON > {season}  MIN IP > {minInningsPitched}");
                C.WriteLine($"TODAY'S REPORT EXISTS?  : {doesCsvReportExistForToday}");
                C.WriteLine($"--------------------------------------------\n");
            }



            public void PrintDatabaseAddOutcomes(int countAdded, int countNotAdded)
            {
                C.WriteLine($"\n-------------------------------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(FanGraphsSpController));
                C.WriteLine($"ADDED TO DB   : {countAdded}");
                C.WriteLine($"ALREADY IN DB : {countNotAdded}");
                C.WriteLine($"-------------------------------------------------------------------\n");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
