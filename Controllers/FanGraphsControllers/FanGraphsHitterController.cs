using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.FanGraphs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using static BaseballScraper.EndPoints.FanGraphsUriEndPoints;
using C = System.Console;

#pragma warning disable CC0091, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006, MA0016, MA0051
namespace BaseballScraper.Controllers.FanGraphsControllers
{
    [Route("api/fangraphs/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FanGraphsHitterController : ControllerBase
    {
        private readonly Helpers                   _helpers;
        private readonly GoogleSheetsConnector     _googleSheetsConnector;
        private readonly FanGraphsUriEndPoints     _fanGraphsEndPoints;
        private readonly ProjectDirectoryEndPoints _projectEndPoints;


        public FanGraphsHitterController(Helpers helpers, GoogleSheetsConnector googleSheetsConnector, FanGraphsUriEndPoints fanGraphsEndPoints, ProjectDirectoryEndPoints projectEndPoints)
        {
            _helpers               = helpers;
            _googleSheetsConnector = googleSheetsConnector;
            _fanGraphsEndPoints    = fanGraphsEndPoints;
            _projectEndPoints      = projectEndPoints;
        }


        public FanGraphsHitterController(){}


        private string GoogleChromePath
        {
            get => _projectEndPoints.GoogleChromePath;
        }



        /*
            https://127.0.0.1:5001/api/fangraphs/fghittermasterreport/hitters
        */
        [HttpGet("hitters")]
        public void TestFgHitterMasterReportController()
        {
            _helpers.StartMethod();
        }


        /*
            https://127.0.0.1:5001/api/fangraphs/fghittermasterreport/hitters/async
        */
        [HttpGet("hitters/async")]
        public async Task TestFgHitterMasterReportControllerAsync()
        {
            _helpers.StartMethod();
            List<FanGraphsHitter> scrapeForOutfieldersIn2018 = await ScrapeMasterHittersReportAsync(positionEnum: PositionEnum.Shortstop, minPlateAppearances: 100, league: "nl");
        }





        #region FANGRAPHS HITTER - PRIMARY METHODS - SCRAPE HTML ------------------------------------------------------------

        // STATUS [ July 8, 2019 ] : this works
        /// <summary>
        ///     Scapes FanGraphs hitter master report html
        ///     By default, returns 30 rows per page
        /// </summary>
        /// <remarks>
        ///     * Launches browser (i.e., Headless = false)
        ///     * // * Uses local chrome instance instead of Chromium (i.e., ExecutablePath = GoogleChromePath)
        ///     * See: FanGraphsUriEndPoints more info
        ///     * See: http://www.puppeteersharp.com/api/index.html
        /// </remarks>
        /// <param name="positionEnum">
        ///     See FanGraphsUriEndPoints for enum options
        ///     Examples are: PositionEnum.Catcher, PositionEnum.Outfield, PositionEnum.FirstBase
        ///     Optional parameter; Defaults to PositionEnum.All if no selection is passed in method
        /// </param>
        /// <param name="minPlateAppearances">
        ///     Min # of PAs a hitter needs to be included in the scrape
        ///     Optional parameter; Defaults to all qualified hitters if no value is passed in method
        /// </param>
        /// <param name="league">
        ///     "al" OR "nl"
        ///     Optional parameter; Defaults to both leagues if no value is passed in method
        /// </param>
        /// <param name="year">
        ///     The year or season (e.g., 2019) to search for
        ///     Optional parameter; Defaults to current year no value is passed in method
        /// </param>
        /// <example>
        ///     NO FILTERS:     var scrapeWithNoFilters = await ScrapeMasterHittersReport();
        ///     CATCHERS ONLY:  var scrapeForCatchers = await ScrapeMasterHittersReport(positionEnum: PositionEnum.Catcher);
        ///     OF IN 2018:     var scrapeForOutfieldersIn2018 = await ScrapeMasterHittersReport(positionEnum: PositionEnum.Outfield, year: 2018);
        ///     NL SS, 100PA+:  var scrapeForOutfieldersIn2018 = await ScrapeMasterHittersReport(positionEnum: PositionEnum.Shortstop, minPlateAppearances: 100, league: "nl");
        /// </example>
        public async Task<List<FanGraphsHitter>> ScrapeMasterHittersReportAsync(PositionEnum positionEnum = PositionEnum.All, int minPlateAppearances = 0, string league="all", int year = 0)
        {
            LaunchOptions options = new LaunchOptions { Headless = false, ExecutablePath = GoogleChromePath };
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            List<FanGraphsHitter> fgHitterList = new List<FanGraphsHitter>();

            using (Browser browser = await Puppeteer.LaunchAsync(options))
            using (Page page = await browser.NewPageAsync())
            {
                // scrape first page of html to get total number of pages that will be scraped
                const int pageNumber = 1;
                string endPoint = (_fanGraphsEndPoints.FgHitterMasterReport(
                    positionEnum: positionEnum,
                    minPlateAppearances: minPlateAppearances,
                    league: league,
                    year: year,
                    pageNumber: pageNumber
                ).EndPointUri);

                await page.GoToAsync(endPoint,10000000);

                int numberOfPagesToScrape = await GetNumberOfPagesToScapeAsync(page, endPoint);

                // loop through each page returned in the search
                for(var pageCounter = 1; pageCounter <= numberOfPagesToScrape; pageCounter++)
                {
                    endPoint = (_fanGraphsEndPoints.FgHitterMasterReport(
                        positionEnum: positionEnum,
                        minPlateAppearances: minPlateAppearances,
                        league: league,
                        year: year,
                        pageNumber: pageCounter
                    ).EndPointUri);

                    C.WriteLine($"endPoint: {endPoint}");

                    await page.GoToAsync(endPoint,50000);

                    const int rowCount = 30;

                    // if there are 30 results on page there will be no year
                    try
                    {
                        // loop through each row on page
                        for(int rowCounter = 1; rowCounter < rowCount; rowCounter++)
                        {
                            string rowSelector = $".rgMasterTable #LeaderBoard1_dg1_ctl00__{rowCounter} .grid_line_regular";
                            await page.WaitForSelectorAsync(rowSelector);

                            // get all cell values for each row
                            JToken allValuesInRow = await page.EvaluateFunctionAsync(@"(rowSelector) =>
                            {
                                const cells = Array.from(document.querySelectorAll(rowSelector));
                                    return cells.map(cells =>
                                    {
                                        const cellValue =cells.textContent;
                                        return `${cellValue}`;
                                    });
                            }", rowSelector);
                            FanGraphsHitter fgHitter = CreateFanGraphsHitterInstance(allValuesInRow);
                            fgHitterList.Add(fgHitter);
                            PrintFanGraphsHitterBasicDetails(fgHitter);
                        }
                    }
                    // if there are less than 30 results on page, it'll error
                    catch(Exception ex) { C.WriteLine($"Exception Message: {ex.Message}");}
                }
            }
            return fgHitterList;
        }

        #endregion FANGRAPHS HITTER - PRIMARY METHODS - SCRAPE HTML ------------------------------------------------------------





        #region FANGRAPHS HITTER - SUPPORT METHODS - SCRAPE HTML ------------------------------------------------------------


        // STATUS [ July 8, 2019 ] : this works
        /// <summary>
        ///     Gets a specific value from within the html of the page
        /// </summary>
        /// <param name="page">
        ///     a PuppeteerSharp.Page
        /// </param>
        /// <param name="selector">
        ///     CSS selector which can be attained by inspecting the html in Chrome / Firefox
        /// </param>
        /// <remarks>
        ///     This is used to get the number of pages to scrape and / or rows on page to scrape
        /// </remarks>
        /// <example>
        ///     int numberOfPagesToScrape = await GetIntFromPage(page, _pageCountSelector);
        /// </example>
        private async Task<int> GetIntFromPageAsync(Page page, string selector)
        {
            JToken intJToken = await page.EvaluateFunctionAsync(@"(selector) =>
            {
                const htmlElement = Array.from(document.querySelectorAll(selector));
                    return htmlElement.map(htmlElement =>
                    {
                        const intToGet = htmlElement.textContent;
                        return `${intToGet}`;
                    });
            }", selector);
            int intToGet = Convert.ToInt32(value: intJToken[0], provider: CultureInfo.CurrentCulture);
            return intToGet;
        }


        // STATUS [ July 8, 2019 ] : this works
        /// <summary>
        ///     Gets the number of rows on page to scrape
        /// </summary>
        /// <param name="page">todo: describe page parameter on GetNumberOfPagesToScape</param>
        /// <param name="endPoint">todo: describe endPoint parameter on GetNumberOfPagesToScape</param>
        /// <example>
        ///     int numberOfPagesToScrape = await GetNumberOfPagesToScape(page, endPoint);
        /// </example>
        private async Task<int> GetNumberOfPagesToScapeAsync(Page page, string endPoint)
        {
            await page.GoToAsync(endPoint,10000000);
            int numberOfPagesToScrape = await GetNumberOfPagesToScapeAsync(page);
            return numberOfPagesToScrape;
        }


        // used by 'GetNumberOfPagesToScrape()' method
        private string _pageCountSelector = "#LeaderBoard1_dg1_ctl00 > thead:nth-child(2) > tr:nth-child(1) > td:nth-child(1) > div:nth-child(1) > div:nth-child(5) > strong:nth-child(2)";


        // STATUS [ July 8, 2019 ] : this works
        /// <summary>
        ///     Gets the number of rows on page to scrape
        /// </summary>
        /// <param name="page">todo: describe page parameter on GetNumberOfPagesToScape</param>
        /// <example>
        ///     int numberOfPagesToScrape = await GetNumberOfPagesToScape(page);
        /// </example>
        private async Task<int> GetNumberOfPagesToScapeAsync(Page page)
        {
            int numberOfPagesToScrape = await GetIntFromPageAsync(page, _pageCountSelector);
            return numberOfPagesToScrape;
        }


        // private string _rowCountSelector = "#LeaderBoard1_dg1_ctl00 > thead:nth-child(2) > tr:nth-child(1) > td:nth-child(1) > div:nth-child(1) > div:nth-child(5) > strong:nth-child(1)";

        // private async Task<int> GetNumberOfRowsOnPage(Page page)
        // {
        //     int numberOfRowsInTable = await GetIntFromPage(page, _rowCountSelector);
        //     return numberOfRowsInTable;
        // }


        #endregion FANGRAPHS HITTER - SUPPORT METHODS - SCRAPE HTML ------------------------------------------------------------





        #region FANGRAPHS HITTER - SUPPORT METHODS - CREATE FANGRAPHS HITTER INSTANCE ------------------------------------------------------------

        // STATUS [ July 8, 2019 ] : this works
        public FanGraphsHitter CreateFanGraphsHitterInstance(JToken allValuesInRow)
        {
            FanGraphsHitter fgHitter = new FanGraphsHitter
            {
                FanGraphsRowNumber              =(int)allValuesInRow[0],
                FanGraphsName                   =(string)allValuesInRow[1],
                FanGraphsTeam                   =(string)allValuesInRow[2],
                Age                             =(int)allValuesInRow[3],
                GamesPlayed                     =(int)allValuesInRow[4],
                AtBats                          =(int)allValuesInRow[5],
                PlateAppearances                =(int)allValuesInRow[6],
                Runs                            =(int)allValuesInRow[7],
                HomeRuns                        =(int)allValuesInRow[8],
                RunsBattedIn                    =(int)allValuesInRow[9],
                StolenBases                     =(int)allValuesInRow[10],
                Walks                           =(int)allValuesInRow[11],
                BattingAverage                  =(decimal)allValuesInRow[12],
                WalkPercentage                  =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[13]),
                StrikeoutPercentage             =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[14]),
                WalksPerStrikeout               =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[15]),
                Iso                             =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[16]),
                Babip                           =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[17]),
                FlyBallPercentage               =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[18]),
                PullPercentage                  =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[19]),
                SoftPercentage                  =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[20]),
                HardPercentage                  =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[21]),
                wRcPlus                         =(int)allValuesInRow[22],
                OSwingPercentage                =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[23]),
                ZContactPercentage              =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[24]),
                SwingingStrikePercentage        =ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[25]),
                WalkPercentagePlus              =(int)allValuesInRow[26],
                StrikeoutPercentagePlus         =(int)allValuesInRow[27],
                OnBasePercentagePlus            =(int)allValuesInRow[28],
                SluggingPercentagePlus          =(int)allValuesInRow[29],
                IsoPlus                         =(int)allValuesInRow[30],
                BabipPlus                       =(int)allValuesInRow[31],
                LinedrivePercentagePlus         =(int)allValuesInRow[32],
                GroundBallPercentagePlus        =(int)allValuesInRow[33],
                FlyBallPercentagePlus           =(int)allValuesInRow[34],
                HomeRunPerFlyBallPercentagePlus =(int)allValuesInRow[35],
                PullPercentagePlus              =(int)allValuesInRow[36],
                CenterPercentagePlus            =(int)allValuesInRow[37],
                OppoPercentagePlus              =(int)allValuesInRow[38],
                SoftPercentagePlus              =(int)allValuesInRow[39],
                MediumPercentagePlus            =(int)allValuesInRow[40],
                HardPercentagePlus              =(int)allValuesInRow[41],
            };
            return fgHitter;
        }


        // STATUS [ July 8, 2019 ] : this works
        /// <summary>
        ///     Some cells have % symbol in them; this removes it and just gives the number back
        ///     e.g., 13.1% becomes 13.1
        ///     Helper method for: 'CreateFanGraphsHitterInstance(JToken allValuesInRow)' method
        /// </summary>
        /// <param name="token">todo: describe token parameter on ConvertCellWithPercentageSymbolToDecimal</param>
        /// <remarks>
        ///     See: https://stackoverflow.com/questions/2171615/how-to-convert-percentage-string-to-double
        /// </remarks>
        /// <example>
        ///     int numberOfPagesToScrape = await GetNumberOfPagesToScape(page);
        /// </example>
        public decimal ConvertCellWithPercentageSymbolToDecimal(JToken token)
        {
            string[] dataToConvert = token.ToString().Split('%');
            decimal decimalValue   = decimal.Parse(dataToConvert[0], NumberStyles.None, CultureInfo.InvariantCulture);
            return decimalValue;
        }

        #endregion CREATE INSTANCE OF FANGRAPHS HITTER ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

        public void PrintFanGraphsHitterBasicDetails(FanGraphsHitter hitter)
        {
            C.WriteLine($"#    | {hitter.FanGraphsRowNumber}");
            C.WriteLine($"NAME | {hitter.FanGraphsName}");
            C.WriteLine($"TEAM | {hitter.FanGraphsTeam}\n");
        }


        public void PrintFanGraphsHitterBasicDetails(JToken token)
        {
            C.WriteLine($"{token[0]}\t {token[1]}\t {token[2]}\n");
        }


        public void PrintFanGraphsHitterDetails(FanGraphsHitter hitter)
        {
            C.WriteLine($"1  | NAME:         {hitter.FanGraphsName}");
            C.WriteLine($"0  | #:            {hitter.FanGraphsRowNumber}");
            C.WriteLine($"2  | TEAM:         {hitter.FanGraphsTeam}");
            C.WriteLine($"3  | AGE:          {hitter.Age}");
            C.WriteLine($"4  | GAMES PLAYED: {hitter.GamesPlayed}");
            C.WriteLine($"5  | AT BATS:      {hitter.AtBats}");
            C.WriteLine($"6  | PA:           {hitter.PlateAppearances}");
            C.WriteLine($"7  | RUNS:         {hitter.Runs}");
            C.WriteLine($"8  | HOME RUNS:    {hitter.HomeRuns}");
            C.WriteLine($"9  | RBI:          {hitter.RunsBattedIn}");
            C.WriteLine($"10 | SBs:          {hitter.StolenBases}");
            C.WriteLine($"11 | BBs:          {hitter.Walks}");
            C.WriteLine($"12 | AVG:          {hitter.BattingAverage}");
            C.WriteLine($"13 | BB%:          {hitter.WalkPercentage}");
            C.WriteLine($"14 | K%:           {hitter.StrikeoutPercentage}");
            C.WriteLine($"15 | BB/K:         {hitter.WalksPerStrikeout}");
            C.WriteLine($"16 | ISO:          {hitter.Iso}");
            C.WriteLine($"17 | BABIP:        {hitter.Babip}");
            C.WriteLine($"18 | FB%:          {hitter.FlyBallPercentage}");
            C.WriteLine($"19 | Pull%:        {hitter.PullPercentage}");
            C.WriteLine($"20 | Soft%:        {hitter.SoftPercentage}");
            C.WriteLine($"21 | Hard%:        {hitter.HardPercentage}");
            C.WriteLine($"22 | wRC+:         {hitter.wRcPlus}");
            C.WriteLine($"23 | OSwing%:      {hitter.OSwingPercentage}");
            C.WriteLine($"24 | ZContact%:    {hitter.ZContactPercentage}");
            C.WriteLine($"25 | SwStr%:       {hitter.SwingingStrikePercentage}");
            C.WriteLine($"26 | BB%+:         {hitter.WalkPercentagePlus}");
            C.WriteLine($"27 | K%+:          {hitter.StrikeoutPercentagePlus}");
            C.WriteLine($"28 | OBP%+:        {hitter.OnBasePercentagePlus}");
            C.WriteLine($"29 | SLG%+:        {hitter.SluggingPercentagePlus}");
            C.WriteLine($"30 | ISO+:         {hitter.IsoPlus}");
            C.WriteLine($"31 | BABIP+:       {hitter.BabipPlus}");
            C.WriteLine($"32 | LD%+:         {hitter.LinedrivePercentagePlus}");
            C.WriteLine($"33 | GB%+:         {hitter.GroundBallPercentagePlus}");
            C.WriteLine($"34 | FB%+:         {hitter.FlyBallPercentagePlus}");
            C.WriteLine($"35 | HR%+:         {hitter.HomeRunPerFlyBallPercentagePlus}");
            C.WriteLine($"36 | Pull%+:       {hitter.PullPercentagePlus}");
            C.WriteLine($"37 | Center%+:     {hitter.CenterPercentagePlus}");
            C.WriteLine($"38 | Oppo%+:       {hitter.OppoPercentagePlus}");
            C.WriteLine($"39 | Soft%+:       {hitter.SoftPercentagePlus}");
            C.WriteLine($"40 | Medium%+:     {hitter.MediumPercentagePlus}");
            C.WriteLine($"41 | Hard%+:       {hitter.HardPercentagePlus}");
            C.WriteLine();
        }


        public void PrintFanGraphsHittersDetailsFromList(List<FanGraphsHitter> fgHitterList)
        {
            foreach(FanGraphsHitter hitter in fgHitterList)
            {
                C.WriteLine($"0  | #: {hitter.FanGraphsRowNumber}");
                C.WriteLine($"1  | NAME: {hitter.FanGraphsName}");
                C.WriteLine($"2  | TEAM: {hitter.FanGraphsTeam}");
                C.WriteLine($"3  | AGE: {hitter.Age}");
                C.WriteLine($"4  | GAMES PLAYED: {hitter.GamesPlayed}");
                C.WriteLine($"5  | AT BATS: {hitter.AtBats}");
                C.WriteLine($"6  | PA: {hitter.PlateAppearances}");
                C.WriteLine($"7  | RUNS: {hitter.Runs}");
                C.WriteLine($"8  | HOME RUNS: {hitter.HomeRuns}");
                C.WriteLine($"9  | RBI: {hitter.RunsBattedIn}");
                C.WriteLine($"10 | SBs: {hitter.StolenBases}");
                C.WriteLine($"11 | BBs: {hitter.Walks}");
                C.WriteLine($"12 | AVG: {hitter.BattingAverage}");
                C.WriteLine($"13 | BB%: {hitter.WalkPercentage}");
                C.WriteLine($"14 | K%: {hitter.StrikeoutPercentage}");
                C.WriteLine($"15 | BB/K: {hitter.WalksPerStrikeout}");
                C.WriteLine($"16 | ISO: {hitter.Iso}");
                C.WriteLine($"17 | BABIP: {hitter.Babip}");
                C.WriteLine($"18 | FB%: {hitter.FlyBallPercentage}");
                C.WriteLine($"19 | Pull%: {hitter.PullPercentage}");
                C.WriteLine($"20 | Soft%: {hitter.SoftPercentage}");
                C.WriteLine($"21 | Hard%: {hitter.HardPercentage}");
                C.WriteLine($"22 | wRC+: {hitter.wRcPlus}");
                C.WriteLine($"23 | OSwing%: {hitter.OSwingPercentage}");
                C.WriteLine($"24 | ZContact%: {hitter.ZContactPercentage}");
                C.WriteLine($"25 | SwStr%: {hitter.SwingingStrikePercentage}");
                C.WriteLine($"26 | BB%+: {hitter.WalkPercentagePlus}");
                C.WriteLine($"27 | K%+: {hitter.StrikeoutPercentagePlus}");
                C.WriteLine($"28 | OBP%+: {hitter.OnBasePercentagePlus}");
                C.WriteLine($"29 | SLG%+: {hitter.SluggingPercentagePlus}");
                C.WriteLine($"30 | ISO+: {hitter.IsoPlus}");
                C.WriteLine($"31 | BABIP+: {hitter.BabipPlus}");
                C.WriteLine($"32 | LD%+: {hitter.LinedrivePercentagePlus}");
                C.WriteLine($"33 | GB%+: {hitter.GroundBallPercentagePlus}");
                C.WriteLine($"34 | FB%+: {hitter.FlyBallPercentagePlus}");
                C.WriteLine($"35 | HR%+: {hitter.HomeRunPerFlyBallPercentagePlus}");
                C.WriteLine($"36 | Pull%+: {hitter.PullPercentagePlus}");
                C.WriteLine($"37 | Center%+: {hitter.CenterPercentagePlus}");
                C.WriteLine($"38 | Oppo%+: {hitter.OppoPercentagePlus}");
                C.WriteLine($"39 | Soft%+: {hitter.SoftPercentagePlus}");
                C.WriteLine($"40 | Medium%+: {hitter.MediumPercentagePlus}");
                C.WriteLine($"41 | Hard%+: {hitter.HardPercentagePlus}");
                C.WriteLine();
            }
        }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}







// // var inputBoxSelector = "#LeaderBoard1_dg1_ctl00_ctl02_ctl00_PageSizeComboBox_Input";
// var itemNumberSelector = "#LeaderBoard1_dg1_ctl00 > thead:nth-child(2) > tr:nth-child(1) > td:nth-child(1) > div:nth-child(1) > div:nth-child(5) > strong:nth-child(1)";

// JToken rowNumberFromPageSizeDropdown = await page.EvaluateFunctionAsync(@"(itemNumberSelector) =>
// {
//     const rows = Array.from(document.querySelectorAll(itemNumberSelector));
//         return rows.map(rows =>
//         {
//             const numberOfRows = rows.textContent;
//             return `${numberOfRows}`;
//         });
// }", itemNumberSelector);

// _helpers.Dig(rowNumberFromPageSizeDropdown);
// int numberOfRowsInTable = Convert.ToInt32(rowNumberFromPageSizeDropdown[0]);
// C.WriteLine($"numberOfRowsInTable: {numberOfRowsInTable}");




                    // Stopwatch sw1 = Stopwatch.StartNew();
                    // sw1.Stop();
                    // var swTimeElapsed = sw1.Elapsed.Seconds;
                    // C.WriteLine($"stopwatch.swTimeElapsed: {swTimeElapsed} seconds");








