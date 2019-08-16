using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.FanGraphs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.FanGraphsControllers
{
    [Route("api/fangraphs/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FgSpWpdiReportController : ControllerBase
    {
        private readonly Helpers                      _helpers;

        private readonly FanGraphsUriEndPoints        _endPoints;

        private readonly GoogleSheetsConnector        _googleSheetsConnector;

        private readonly CsvHandler                   _csvHandler;

        private readonly BaseballScraperContext       _context;


        public FgSpWpdiReportController(Helpers helpers, FanGraphsUriEndPoints endPoints, GoogleSheetsConnector googleSheetsConnector, CsvHandler csvHandler, BaseballScraperContext context)
        {
            _helpers               = helpers;
            _endPoints             = endPoints;
            _googleSheetsConnector = googleSheetsConnector;
            _csvHandler            = csvHandler;
            _context               = context;
        }



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



        /*
            https://127.0.0.1:5001/api/fangraphs/FgSpWpdiReport/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
            _googleSheetsConnector.SetGradientConditionalFormat();
            // CheckIfCsvFileForTodayExists();
        }


        /*
            https://127.0.0.1:5001/api/fangraphs/FgSpWpdiReport/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();

            await RunFgSpWpdiReport(2019, minInningsPitched_:1);
            var listFromDatabase = GetMany(season: 2019, minInningsPitched: 100);
            AddManyToGsheet(listFromDatabase);
            // await RunFgSpWpdiReport(2017, minInningsPitched_:1);
            // await RunFgSpWpdiReport(2016, minInningsPitched_:1);
            // await RunFgSpWpdiReport(2015, minInningsPitched_:1);
            // await RunFgSpWpdiReport(2014, minInningsPitched_:1);

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



        #region FgSpWpdiReport PRIMARY METHOD ------------------------------------------------------------


            public async Task RunFgSpWpdiReport(int season_, int minInningsPitched_=0)
            {
                // STEP 0: Check if there is already a Csv file created for today
                bool doesCsvReportExistForToday = CheckIfCsvFileForTodayExists();

                // if CSV doesn't exist
                if(doesCsvReportExistForToday == false)
                {
                    // STEP 1: set report parameters and download CSV to local downloads folder
                    await DownloadFgSpWpdiCsvToLocalDownloadsFolder(
                        minInningsPitched_: minInningsPitched_
                    );

                    // STEP 2: move the CSV from local downloads folder to project Target_Write folder
                    MoveFgSpWpdiReportToTargetFolder(_wPdiReportPrefix);
                }

                // Create list of pitchers from CSV
                var pitcherList = CreateListOfWpdiPitchersFromCsv(season_);
                foreach(var pitcher in pitcherList)
                {
                    // STEP 3: add each pitcher to database OR update pitcher in database
                    await AddOne(pitcher, season_);
                }

                // Query database
                var listFromDatabase = GetMany(season:season_, minInningsPitched:minInningsPitched_);

                foreach(var pitcher in listFromDatabase)
                {
                    double mPDI = CalculateSpMpdi(pitcher.ZonePercentage, pitcher.OSwingPercentage, pitcher.OContactPercentage, pitcher.ZSwingPercentage);
                    Console.WriteLine($"{pitcher.PitcherName}\t{mPDI}");
                }
            }

        #endregion FgSpWpdiReport PRIMARY METHOD ------------------------------------------------------------






        #region GOOGLE SHEETS ------------------------------------------------------------


            /*
                Example:
                var listFromDatabase = GetMany(season: 2019, minInningsPitched: 100);
                AddManyToGsheet(listFromDatabase);
            */
            public ActionResult AddManyToGsheet(List<FanGraphsPitcherForWpdiReport> listOfPitchers)
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



            public async Task<ActionResult> AddManyToGsheetAsync(List<FanGraphsPitcherForWpdiReport> listOfPitchers)
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





        #region RETRIEVE DATA FROM FANGRAPHS ------------------------------------------------------------


            /* --------------------------------------------------------------- */
            /* CURRENT SEASON / IN-SEASON                                      */
            /* --------------------------------------------------------------- */

            // STATUS [ July 31, 2019 ] : this works
            // STEP 0: Check if there is already a Csv file created for today
            // * Returns 'false' if file doesn't exist; returns 'true' if it does
            // * If the file already exists, you do not need to run the report again
            public bool CheckIfCsvFileForTodayExists()
            {
                string targetWriteFolderPath = _endPoints.FanGraphsTargetWriteFolderLocation();
                var fileInfo = new DirectoryInfo(targetWriteFolderPath).GetFiles();

                DateTime today = DateTime.Now;
                int year       = today.Year;
                int month      = today.Month;
                int day        = today.Day;

                string fileName = $"{_wPdiReportPrefix}_{month}_{day}_{year}.csv";

                bool doesCsvReportExistForToday = false;

                foreach(FileInfo file in fileInfo)
                {
                    if(file.Name == fileName)
                    {
                        doesCsvReportExistForToday = true;
                    }
                }
                Console.WriteLine($"Does CSV for today exist: {doesCsvReportExistForToday}");
                return doesCsvReportExistForToday;
            }


            // STATUS [ July 31, 2019 ] : this works
            // STEP 1: set report parameters and download CSV to local downloads folder
            // * Goes to a FanGraphs page and downloads the CSV from that page to local downloads folder
            // * Ultimately the CSV should be moved to project data folder (See MoveFgSpWpdiReportToTargetFolder() method)
            // * This should not run if a Csv file for current day already exists
            // * Example endPoint:
            // *    https://www.fangraphs.com/leaders.aspx?pos=all&stats=sta&lg=all&qual=150&type=c,8,13,210,204,205,208,207,111,105,106,109,108&season=2018&month=0&season1=2018&ind=0&team=0&rost=0&age=0&filter=&players=0&startdate=2018-01-01&enddate=2018-12-31
            public async Task DownloadFgSpWpdiCsvToLocalDownloadsFolder(
                int minInningsPitched_ = 0,
                int year_              = 0,
                int startMonth_        = 0,
                int startDay_          = 0,
                int endMonth_          = 0,
                int endDay_            = 0
            )
            {
                var fgUrl = _endPoints.StartingPitcherWpdiEndPoint(
                    minInningsPitched : minInningsPitched_,
                    year              : year_,
                    startMonth        : startMonth_,
                    startDay          : startDay_,
                    endMonth          : endMonth_,
                    endDay            : endDay_
                ).EndPointUri.ToString();

                var csvSelector = _endPoints.FanGraphsCsvHtmlSelector();
                await _csvHandler.ClickLinkToDownloadCsvFile(fgUrl, csvSelector);
            }


            // STATUS [ July 31, 2019 ] : this works
            // STEP 2: move the CSV from local downloads folder to project Target_Write folder
            // * Looks for the file that was last updated in local downloads
            // * This should not run if a Csv file for current day already exists
            // * Once it finds last updated file, it moves and renames the file
            // * Ends up something like: "SpWpdiReport_07_09_2019.csv"
            public ActionResult MoveFgSpWpdiReportToTargetFolder(
                string fileNamePrefix,
                int reportMonth = 0,
                int reportYear  = 0,
                int reportDay   = 0
            )
            {
                var downloadsFolder   = _endPoints.LocalDownloadsFolderLocation();
                var targetWriteFolder = _endPoints.FanGraphsTargetWriteFolderLocation();

                _csvHandler.MoveCsvFileToFolder(
                    downloadsFolder,
                    targetWriteFolder,
                    fileNamePrefix,
                    month:reportMonth,
                    year:reportYear,
                    day:reportDay
                );
                return Ok();
            }


            /* --------------------------------------------------------------- */
            /* FULL / COMPLETED SEASON */
            /* --------------------------------------------------------------- */

            // Only needs to be run once ever
            // Uses CSV files from target_write folder to players to database
            public async Task MoveDataFromAllPreviousSeasonsToDatabase()
            {
                await MoveDataFromFanGraphsToDatabaseFullSeason(2018);
                await MoveDataFromFanGraphsToDatabaseFullSeason(2017);
                await MoveDataFromFanGraphsToDatabaseFullSeason(2016);
                await MoveDataFromFanGraphsToDatabaseFullSeason(2015);
                await MoveDataFromFanGraphsToDatabaseFullSeason(2014);
                await MoveDataFromFanGraphsToDatabaseFullSeason(2013);
                await MoveDataFromFanGraphsToDatabaseFullSeason(2012);
                await MoveDataFromFanGraphsToDatabaseFullSeason(2011);
                await MoveDataFromFanGraphsToDatabaseFullSeason(2010);
                await MoveDataFromFanGraphsToDatabaseFullSeason(2009);
            }


            // CSVs should exist for 2009 - 2019
            // * File name format: 'SpWpdiReport_XXXX.csv' where XXXX is the year
            public async Task MoveDataFromFanGraphsToDatabaseFullSeason(int year)
            {
                var pitcherList = CreateListOfWpdiPitchersFromCsv(year);
                foreach(var pitcher in pitcherList)
                {
                    pitcher.Season = year;
                    await AddOne(pitcher, year);
                }
            }

        #endregion RETRIEVE DATA FROM FANGRAPHS ------------------------------------------------------------





        #region DATABASE ------------------------------------------------------------


            /* --------------------------------------------------------------- */
            /* ONE PITCHER                                                     */
            /* --------------------------------------------------------------- */


            // STATUS [ July 31, 2019 ] : this works
            // STEP 3: add each pitcher to database OR update pitcher in database
            // * Checks if record for pitcher already exists
            [HttpPost("add")]
            public async Task AddOne(FanGraphsPitcherForWpdiReport pitcher, int season)
            {
                pitcher.Season = season;
                FanGraphsPitcherForWpdiReport exists = _context.FanGraphsPitcherForWpdiReport.SingleOrDefault(sp => sp.PlayerYearConcat == pitcher.PlayerYearConcat);

                if(exists != null)
                {
                    try
                    {
                        _context.Update(exists);
                        await _context.SaveChangesAsync();
                    }
                    catch { Console.WriteLine("Error either adding or updating pitcher"); }
                }
                else
                {
                    await _context.AddAsync(pitcher);
                    await _context.SaveChangesAsync();
                }
            }


            // STATUS [ August 1, 2019 ] : haven't tested if this works
            [HttpPost("create")]
            public async Task<IActionResult> CreateOne(FanGraphsPitcherForWpdiReport pitcher, int season)
            {
                pitcher.Season = season;
                if(ModelState.IsValid)
                {
                    await _context.AddAsync(pitcher);
                    await _context.SaveChangesAsync();
                }
                return Ok();
            }


            // STATUS [ August 1, 2019 ] : haven't tested if this works
            [HttpGet("get/{fangraphsid}")]
            public FanGraphsPitcherForWpdiReport GetOne(int fangraphsid)
            {
                FanGraphsPitcherForWpdiReport pitcher = _context.FanGraphsPitcherForWpdiReport.SingleOrDefault(p => p.FanGraphsId == fangraphsid);
                return pitcher;
            }


            // STATUS [ August 1, 2019 ] : haven't tested if this works
            [HttpGet("delete/{fangraphsid}")]
            public async Task<IActionResult> DeleteOne(int fangraphsid)
            {
                FanGraphsPitcherForWpdiReport pitcher = await _context.FanGraphsPitcherForWpdiReport.SingleOrDefaultAsync(p => p.FanGraphsId == fangraphsid);
                _context.Remove(pitcher);
                await _context.SaveChangesAsync();
                return Ok();
            }



            /* --------------------------------------------------------------- */
            /* MULTIPLE PITCHERS                                               */
            /* --------------------------------------------------------------- */


            [HttpPost("add")]
            public async Task AddMany(List<FanGraphsPitcherForWpdiReport> pitchers, int season)
            {
                foreach(var pitcher in pitchers)
                {
                    await AddOne(pitcher, season);
                }
            }


            // STATUS [ July 31, 2019 ] : this works
            // Delete pitchers in database from list
            // Example:
            /*
                var pitcherList = RetrieveSpWpdiFromDatabaseToList(season:2018);
                DeleteAllRecordsInDatabase(pitcherList);
            */
            public void DeleteMany(List<FanGraphsPitcherForWpdiReport> pitchers)
            {
                foreach(var pitcher in pitchers)
                {
                    _context.Remove(pitcher);
                    _context.SaveChanges();
                }
            }


            // STATUS [ July 31, 2019 ] : this works
            // Query database for pitchers and their wPDIs
            public List<FanGraphsPitcherForWpdiReport> GetMany(int season, int minInningsPitched = 0)
            {
                var pitchers = _context.FanGraphsPitcherForWpdiReport
                    .OrderByDescending(w => w.Wpdi)
                    .Where(y => y.Season == season && y.InningsPitched > minInningsPitched)
                    .ToList();

                // PrintPitcherWpdiBasics(pitchers);
                return pitchers;
            }


        #endregion DATABASE ------------------------------------------------------------





        #region CREATE FanGraphsPitcherForWpdiReport INSTANCE ------------------------------------------------------------


            // STATUS [ July 31, 2019 ] : this works
            // Create list of Wpdi Pitchers from CSV
            // * Uses the last updated CSV
            public List<FanGraphsPitcherForWpdiReport> CreateListOfWpdiPitchersFromCsv(int season)
            {
                // Thread.Sleep(10000);
                string targetWriteFolderPath = _endPoints.FanGraphsTargetWriteFolderLocation();
                string fileToRead            = _csvHandler.GetPathToLastUpdatedFileInFolder(targetWriteFolderPath);

                JObject records = _csvHandler.ReadCsvRecordsToJObject(
                    fileToRead,
                    typeof(FanGraphsPitcherForWpdiReport),
                    typeof(WpdiReportClassMap)
                );

                List<FanGraphsPitcherForWpdiReport> pitchers = new List<FanGraphsPitcherForWpdiReport>();
                FanGraphsPitcherForWpdiReport pitcher        = new FanGraphsPitcherForWpdiReport();

                foreach(var kvp in records)
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


            // STATUS [ July 31, 2019 ] : this works
            // Create instance of FanGraphsPitcherForWpdiReport
            // * Conversions of strings to double occurs within the model
            public FanGraphsPitcherForWpdiReport CreateNewWpdiPitcherInstance(JToken currentPitcher, int season)
            {
                DateTime today = DateTime.Now;
                FanGraphsPitcherForWpdiReport pitcher = new FanGraphsPitcherForWpdiReport
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

                var pitcherApercentage = pitcher.Apercentage = CalculateApercentage(pitcher.ZonePercentage, pitcher.OSwingPercentage, pitcher.OContactPercentage);
                var pitcherBpercentage = pitcher.Bpercentage = CalculateBpercentage(pitcher.ZonePercentage, pitcher.OSwingPercentage, pitcher.OContactPercentage);
                var pitcherCpercentage = pitcher.Cpercentage = CalculateCpercentage(pitcher.ZonePercentage, pitcher.OSwingPercentage);
                var pitcherDpercentage = pitcher.Dpercentage = CalculateDpercentage(pitcher.ZonePercentage, pitcher.ZSwingPercentage, pitcher.ZContactPercentage);
                var pitcherEpercentage = pitcher.Epercentage = CalculateEpercentage(pitcher.ZonePercentage, pitcher.ZSwingPercentage, pitcher.ZContactPercentage);
                var pitcherFpercentage = pitcher.Fpercentage = CalculateFpercentage(pitcher.ZonePercentage, pitcher.ZSwingPercentage);

                pitcher.OutcomeApercentage = CalculateFinalOutcomePercentage(OutcomeA_index, pitcherApercentage);
                pitcher.OutcomeBpercentage = CalculateFinalOutcomePercentage(OutcomeB_index, pitcherBpercentage);
                pitcher.OutcomeCpercentage = CalculateFinalOutcomePercentage(OutcomeC_index, pitcherCpercentage);
                pitcher.OutcomeDpercentage = CalculateFinalOutcomePercentage(OutcomeD_index, pitcherDpercentage);
                pitcher.OutcomeEpercentage = CalculateFinalOutcomePercentage(OutcomeE_index, pitcherEpercentage);
                pitcher.OutcomeFpercentage = CalculateFinalOutcomePercentage(OutcomeF_index, pitcherFpercentage);

                pitcher.Wpdi = CalculateSpWpdi(pitcher.ZonePercentage, pitcher.OSwingPercentage, pitcher.OContactPercentage, pitcher.ZSwingPercentage, pitcher.ZContactPercentage);


                pitcher.OutcomeApercentage_mPDI = CalculateApercentage(pitcher.ZonePercentage, pitcher.OSwingPercentage, pitcher.OContactPercentage);

                pitcher.OutcomeBpercentage_mPDI = CalculateBpercentage(pitcher.ZonePercentage, pitcher.OSwingPercentage, pitcher.OContactPercentage);

                pitcher.OutcomeFpercentage_mPDI = CalculateFpercentage(pitcher.ZonePercentage, pitcher.ZSwingPercentage);

                pitcher.Mpdi = CalculateSpMpdi(pitcher.ZonePercentage, pitcher.OSwingPercentage, pitcher.OContactPercentage, pitcher.ZSwingPercentage);


                if(season == 0)
                {
                    pitcher.Season = today.Year;
                }

                else
                {
                    pitcher.Season = season;
                }

                return pitcher;
            }


        #endregion CREATE FanGraphsPitcherForWpdiReport INSTANCE ------------------------------------------------------------





        #region CALCULATE WPDI ------------------------------------------------------------

            // STATUS [ July 31, 2019 ] : these all work
            // Formulas defined by FanGraphs
            // * A% = Out of Zone / Swung On / No Contact = (1 – Zone%) * (O-Swing%) * (1 – O-Contact%)
            // * B% = Out of Zone / Swung On / Contact Made = (1 – Zone%) * (O-Swing%) * O-Contact%
            // * C% = Out of Zone / No Swing = (1 – Zone%) * (1- O-Swing%)
            // * D% = In Zone / Swung On / No Contact = (Zone%) * (Z-Swing%) * (1 – Z-Contact%)
            // * E% = In Zone / Swung On / Contact Made = (Zone%) * (Z-Swing%) * Z-Contact%
            // * F% = In Zone / No Swing = (Zone%) * (1- Z-Swing%)

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


        #endregion CALCULATE WPDI ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintPitcherWpdiBasics(List<FanGraphsPitcherForWpdiReport> pitchers)
            {
                int counter = 1;
                foreach(var pitcher in pitchers)
                {
                    Console.Write($"{counter}.");
                    PrintPitcherWpdiBasics(pitcher);
                    counter++;
                }
            }

            public void PrintPitcherWpdiBasics(FanGraphsPitcherForWpdiReport pitcher)
            {
                Console.WriteLine($"{pitcher.PitcherName}\t\t{pitcher.InningsPitched}\t{pitcher.Wpdi}");
            }

            public void PrintAllPitcherWpdiData(FanGraphsPitcherForWpdiReport pitcher)
            {
                Console.WriteLine($"\n-----------------------------------------------------------------");
                Console.WriteLine($"\n{pitcher.PitcherName}\t {pitcher.Team}\t {pitcher.FanGraphsId}");
                Console.WriteLine($"GAMES: {pitcher.GamesStarted}\t IP: {pitcher.InningsPitched}");

                Console.WriteLine($"WPDI: {pitcher.Wpdi}");

                Console.WriteLine($"\nZONE%: {pitcher.ZonePercentage}");
                Console.WriteLine($"O-SWING%: {pitcher.OSwingPercentage}");
                Console.WriteLine($"Z-SWING%: {pitcher.ZSwingPercentage}");
                Console.WriteLine($"O-CONTACT%: {pitcher.OContactPercentage}");
                Console.WriteLine($"Z-CONTACT%: {pitcher.ZContactPercentage}");

                Console.WriteLine($"\naPercentage: {pitcher.Apercentage}");
                Console.WriteLine($"bPercentage:   {pitcher.Bpercentage}");
                Console.WriteLine($"cPercentage:   {pitcher.Cpercentage}");
                Console.WriteLine($"dPercentage:   {pitcher.Dpercentage}");
                Console.WriteLine($"ePercentage:   {pitcher.Epercentage}");
                Console.WriteLine($"fPercentage:   {pitcher.Fpercentage}");

                Console.WriteLine($"\nfinalA: {pitcher.OutcomeApercentage}");
                Console.WriteLine($"finalB:   {pitcher.OutcomeBpercentage}");
                Console.WriteLine($"finalC:   {pitcher.OutcomeCpercentage}");
                Console.WriteLine($"finalD:   {pitcher.OutcomeDpercentage}");
                Console.WriteLine($"finalE:   {pitcher.OutcomeEpercentage}");
                Console.WriteLine($"finalF:   {pitcher.OutcomeFpercentage}");

                Console.WriteLine($"\n-----------------------------------------------------------------\n");
            }


            public void PrintPitcherPlateDiscriplineData(FanGraphsPitcherForWpdiReport pitcher)
            {
                Console.WriteLine($"\n{pitcher.PitcherName}\t {pitcher.Team}\t {pitcher.FanGraphsId}");
                Console.WriteLine($"GAMES: {pitcher.GamesStarted}\t IP: {pitcher.InningsPitched}");
                Console.WriteLine($"ZONE%: {pitcher.ZonePercentage}");
                Console.WriteLine($"O-SWING%: {pitcher.OSwingPercentage}\t Z-SWING%: {pitcher.ZSwingPercentage}");
                Console.WriteLine($"O-CONTACT%: {pitcher.OContactPercentage}\t Z-CONTACT%: {pitcher.ZContactPercentage}\n");
            }


            public void PrintWpdiPercentage(double aPercentage, double bPercentage, double cPercentage, double dPercentage, double ePercentage, double fPercentage)
            {
                Console.WriteLine($"\naPercentage: {aPercentage}");
                Console.WriteLine($"bPercentage: {bPercentage}");
                Console.WriteLine($"cPercentage: {cPercentage}");
                Console.WriteLine($"dPercentage: {dPercentage}");
                Console.WriteLine($"ePercentage: {ePercentage}");
                Console.WriteLine($"fPercentage: {fPercentage}\n");
            }

            public void PrintIndividualOutcomes(double finalA, double finalB, double finalC, double finalD, double finalE, double finalF)
            {
                Console.WriteLine($"\nfinalA: {finalA}");
                Console.WriteLine($"finalB: {finalB}");
                Console.WriteLine($"finalC: {finalC}");
                Console.WriteLine($"finalD: {finalD}");
                Console.WriteLine($"finalE: {finalE}");
                Console.WriteLine($"finalF: {finalF}\n");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}