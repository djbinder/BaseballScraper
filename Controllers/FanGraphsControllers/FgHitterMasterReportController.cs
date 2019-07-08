

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.FanGraphs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using static BaseballScraper.EndPoints.FanGraphsUriEndPoints;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.FanGraphsControllers
{
    [Route("api/fangraphs/[controller]")]
    [ApiController]
    public class FgHitterMasterReportController : ControllerBase
    {
        private readonly Helpers _h;
        private readonly GoogleSheetsConnector _gSC;

        private readonly FanGraphsUriEndPoints _fgEndPoints;

        private readonly static DateTime? _dateTimeNow = DateTime.Now;
        readonly int _currentYear = _dateTimeNow.Value.Year;

        // private readonly string _fgHittersReportLink = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2019&month=0&season1=2019&ind=0";

        // private readonly string _fgHitterMasterReportLink = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=c%2c3%2c4%2c5%2c6%2c12%2c11%2c13%2c21%2c14%2c23%2c34%2c35%2c36%2c40%2c41%2c45%2c206%2c209%2c211%2c61%2c102%2c106%2c110%2c289%2c290%2c291%2c292%2c293%2c294%2c295%2c296%2c297%2c298%2c299%2c300%2c301%2c302%2c303%2c304&season=2019&month=0&season1=2019&ind=0&team=0&rost=0&age=0&filter=&players=0&startdate=&enddate=&page=1_50";

        public FgHitterMasterReportController(Helpers h, GoogleSheetsConnector gSC, FanGraphsUriEndPoints fgEndPoints)
        {
            _h = h;
            _gSC = gSC;
            _fgEndPoints = fgEndPoints;
        }

        public FgHitterMasterReportController(){}



        /*
            https://127.0.0.1:5001/api/fangraphs/fghittermasterreport/hitters
        */
        [HttpGet("hitters")]
        public void TestFgHitterMasterReportController()
        {
            _h.StartMethod();
        }




        /*
            https://127.0.0.1:5001/api/fangraphs/fghittermasterreport/hitters/async
        */
        [HttpGet("hitters/async")]
        public async Task TestFgHitterMasterReportControllerAsync()
        {
            _h.StartMethod();
            await ScrapeMasterHittersReport();
        }





        #region SCRAPE FANGRAPHS HITTER HTML ------------------------------------------------------------



            public async Task ScrapeMasterHittersReport(PositionEnum positionEnum = PositionEnum.All, int minPlateAppearances = 0, string league="all", int year = 0)
            {
                LaunchOptions options = new LaunchOptions { Headless = true };
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

                using (Browser browser = await Puppeteer.LaunchAsync(options))
                using (Page page = await browser.NewPageAsync())
                {
                    int pageNumber = 1;
                    var endPoint = (_fgEndPoints.FgHitterMasterReport(
                        positionEnum: positionEnum,
                        minPlateAppearances: minPlateAppearances,
                        league: league,
                        year: year,
                        pageNumber: pageNumber
                    ).EndPointUri);


                    Console.WriteLine($"endPoint: {endPoint}");

                    await page.GoToAsync(endPoint,10000000);

                    int numberOfPagesToScrape = await GetNumberOfPagesToScape(page);

                    int numberOfRowsInTable = await GetNumberOfRowsOnPage(page);
                    List<FanGraphsHitter> fgHitterList = new List<FanGraphsHitter>();

                    // var totalRows = 5;

                    for(var rowCounter = 0; rowCounter < numberOfRowsInTable; rowCounter++)
                    {
                        string rowSelector = $".rgMasterTable #LeaderBoard1_dg1_ctl00__{rowCounter} .grid_line_regular";
                        await page.WaitForSelectorAsync(rowSelector);

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
            }


            private async Task<int> GetIntFromPage(Page page, string selector)
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

                _h.Dig(intJToken);
                int intToGet = Convert.ToInt32(intJToken[0]);
                Console.WriteLine($"intToGet: {intToGet}");
                return intToGet;
            }

            private string _itemNumberSelector = "#LeaderBoard1_dg1_ctl00 > thead:nth-child(2) > tr:nth-child(1) > td:nth-child(1) > div:nth-child(1) > div:nth-child(5) > strong:nth-child(1)";

            private async Task<int> GetNumberOfRowsOnPage(Page page)
            {
                int numberOfRowsInTable = await GetIntFromPage(page, _itemNumberSelector);
                Console.WriteLine($"numberOfRowsInTable: {numberOfRowsInTable}");
                return numberOfRowsInTable;
                // var inputBoxSelector = "#LeaderBoard1_dg1_ctl00_ctl02_ctl00_PageSizeComboBox_Input";
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

                // _h.Dig(rowNumberFromPageSizeDropdown);
                // int numberOfRowsInTable = Convert.ToInt32(rowNumberFromPageSizeDropdown[0]);
                // Console.WriteLine($"numberOfRowsInTable: {numberOfRowsInTable}");
                // return numberOfRowsInTable;
            }


            private string _pageSelector = "#LeaderBoard1_dg1_ctl00 > thead:nth-child(2) > tr:nth-child(1) > td:nth-child(1) > div:nth-child(1) > div:nth-child(5) > strong:nth-child(2)";


            private async Task<int> GetNumberOfPagesToScape(Page page)
            {
                int numberOfPagesToScrape = await GetIntFromPage(page, _pageSelector);
                Console.WriteLine($"numberOfPagesToScrape: {numberOfPagesToScrape}");
                return numberOfPagesToScrape;
                // JToken pageNumberJToken = await page.EvaluateFunctionAsync(@"(_pageSelector) =>
                // {
                //     const pages = Array.from(document.querySelectorAll(_pageSelector));
                //         return pages.map(pages =>
                //         {
                //             const numberOfPages = pages.textContent;
                //             return `${numberOfPages}`;
                //         });
                // }", _pageSelector);

                // _h.Dig(pageNumberJToken);
                // int numberOfPagesToScrape = Convert.ToInt32(pageNumberJToken[0]);
                // Console.WriteLine($"numberOfPagesToScrape: {numberOfPagesToScrape}");
                // return numberOfPagesToScrape;
            }


        #endregion SCRAPE FANGRAPHS HITTER HTML ------------------------------------------------------------





        #region CREATE INSTANCE OF FANGRAPHS HITTER ------------------------------------------------------------


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
                    HardPercentagePlus              =(int)allValuesInRow[41]
                };
                return fgHitter;
            }


            // some cells have % symbol in them; this removes it and just gives the number back
            // e.g., 13.1% becomes 13.1
            // https://stackoverflow.com/questions/2171615/how-to-convert-percentage-string-to-double
            public decimal ConvertCellWithPercentageSymbolToDecimal(JToken token)
            {
                var dataToConvert = token.ToString().Split('%');
                var decimalValue = decimal.Parse(dataToConvert[0]);
                return decimalValue;
            }


        #endregion CREATE INSTANCE OF FANGRAPHS HITTER ------------------------------------------------------------







        // NOT NEEDED; JUST PRACTICE
        // string linkToDownload = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2019&month=0&season1=2019&ind=0";
        // string outputFileName = ""scratch/puppeteer_test2.pdf";
        public async Task DownloadWebPageAsPDF(string linkToDownLoad, string outputFileName)
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
            var page = await browser.NewPageAsync();

            await page.GoToAsync(linkToDownLoad);

            await page.PdfAsync(outputFileName);
        }




        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintFanGraphsHitterBasicDetails(FanGraphsHitter hitter)
            {
                Console.WriteLine($"#    | {hitter.FanGraphsRowNumber}");
                Console.WriteLine($"NAME | {hitter.FanGraphsName}");
                Console.WriteLine($"TEAM | {hitter.FanGraphsTeam}\n");
            }

            public void PrintFanGraphsHitterBasicDetails(JToken token)
            {
                Console.WriteLine($"{token[0]}\t {token[1]}\t {token[2]}\n");
            }


            public void PrintFanGraphsHitterDetails(FanGraphsHitter hitter)
            {
                Console.WriteLine($"1  | NAME:         {hitter.FanGraphsName}");
                Console.WriteLine($"0  | #:            {hitter.FanGraphsRowNumber}");
                Console.WriteLine($"2  | TEAM:         {hitter.FanGraphsTeam}");
                Console.WriteLine($"3  | AGE:          {hitter.Age}");
                Console.WriteLine($"4  | GAMES PLAYED: {hitter.GamesPlayed}");
                Console.WriteLine($"5  | AT BATS:      {hitter.AtBats}");
                Console.WriteLine($"6  | PA:           {hitter.PlateAppearances}");
                Console.WriteLine($"7  | RUNS:         {hitter.Runs}");
                Console.WriteLine($"8  | HOME RUNS:    {hitter.HomeRuns}");
                Console.WriteLine($"9  | RBI:          {hitter.RunsBattedIn}");
                Console.WriteLine($"10 | SBs:          {hitter.StolenBases}");
                Console.WriteLine($"11 | BBs:          {hitter.Walks}");
                Console.WriteLine($"12 | AVG:          {hitter.BattingAverage}");
                Console.WriteLine($"13 | BB%:          {hitter.WalkPercentage}");
                Console.WriteLine($"14 | K%:           {hitter.StrikeoutPercentage}");
                Console.WriteLine($"15 | BB/K:         {hitter.WalksPerStrikeout}");
                Console.WriteLine($"16 | ISO:          {hitter.Iso}");
                Console.WriteLine($"17 | BABIP:        {hitter.Babip}");
                Console.WriteLine($"18 | FB%:          {hitter.FlyBallPercentage}");
                Console.WriteLine($"19 | Pull%:        {hitter.PullPercentage}");
                Console.WriteLine($"20 | Soft%:        {hitter.SoftPercentage}");
                Console.WriteLine($"21 | Hard%:        {hitter.HardPercentage}");
                Console.WriteLine($"22 | wRC+:         {hitter.wRcPlus}");
                Console.WriteLine($"23 | OSwing%:      {hitter.OSwingPercentage}");
                Console.WriteLine($"24 | ZContact%:    {hitter.ZContactPercentage}");
                Console.WriteLine($"25 | SwStr%:       {hitter.SwingingStrikePercentage}");
                Console.WriteLine($"26 | BB%+:         {hitter.WalkPercentagePlus}");
                Console.WriteLine($"27 | K%+:          {hitter.StrikeoutPercentagePlus}");
                Console.WriteLine($"28 | OBP%+:        {hitter.OnBasePercentagePlus}");
                Console.WriteLine($"29 | SLG%+:        {hitter.SluggingPercentagePlus}");
                Console.WriteLine($"30 | ISO+:         {hitter.IsoPlus}");
                Console.WriteLine($"31 | BABIP+:       {hitter.BabipPlus}");
                Console.WriteLine($"32 | LD%+:         {hitter.LinedrivePercentagePlus}");
                Console.WriteLine($"33 | GB%+:         {hitter.GroundBallPercentagePlus}");
                Console.WriteLine($"34 | FB%+:         {hitter.FlyBallPercentagePlus}");
                Console.WriteLine($"35 | HR%+:         {hitter.HomeRunPerFlyBallPercentagePlus}");
                Console.WriteLine($"36 | Pull%+:       {hitter.PullPercentagePlus}");
                Console.WriteLine($"37 | Center%+:     {hitter.CenterPercentagePlus}");
                Console.WriteLine($"38 | Oppo%+:       {hitter.OppoPercentagePlus}");
                Console.WriteLine($"39 | Soft%+:       {hitter.SoftPercentagePlus}");
                Console.WriteLine($"40 | Medium%+:     {hitter.MediumPercentagePlus}");
                Console.WriteLine($"41 | Hard%+:       {hitter.HardPercentagePlus}");
                Console.WriteLine();
            }


            public void PrintFanGraphsHittersDetailsFromList(List<FanGraphsHitter> fgHitterList)
            {
                foreach(var hitter in fgHitterList)
                {
                    Console.WriteLine($"0  | #: {hitter.FanGraphsRowNumber}");
                    Console.WriteLine($"1  | NAME: {hitter.FanGraphsName}");
                    Console.WriteLine($"2  | TEAM: {hitter.FanGraphsTeam}");
                    Console.WriteLine($"3  | AGE: {hitter.Age}");
                    Console.WriteLine($"4  | GAMES PLAYED: {hitter.GamesPlayed}");
                    Console.WriteLine($"5  | AT BATS: {hitter.AtBats}");
                    Console.WriteLine($"6  | PA: {hitter.PlateAppearances}");
                    Console.WriteLine($"7  | RUNS: {hitter.Runs}");
                    Console.WriteLine($"8  | HOME RUNS: {hitter.HomeRuns}");
                    Console.WriteLine($"9  | RBI: {hitter.RunsBattedIn}");
                    Console.WriteLine($"10 | SBs: {hitter.StolenBases}");
                    Console.WriteLine($"11 | BBs: {hitter.Walks}");
                    Console.WriteLine($"12 | AVG: {hitter.BattingAverage}");
                    Console.WriteLine($"13 | BB%: {hitter.WalkPercentage}");
                    Console.WriteLine($"14 | K%: {hitter.StrikeoutPercentage}");
                    Console.WriteLine($"15 | BB/K: {hitter.WalksPerStrikeout}");
                    Console.WriteLine($"16 | ISO: {hitter.Iso}");
                    Console.WriteLine($"17 | BABIP: {hitter.Babip}");
                    Console.WriteLine($"18 | FB%: {hitter.FlyBallPercentage}");
                    Console.WriteLine($"19 | Pull%: {hitter.PullPercentage}");
                    Console.WriteLine($"20 | Soft%: {hitter.SoftPercentage}");
                    Console.WriteLine($"21 | Hard%: {hitter.HardPercentage}");
                    Console.WriteLine($"22 | wRC+: {hitter.wRcPlus}");
                    Console.WriteLine($"23 | OSwing%: {hitter.OSwingPercentage}");
                    Console.WriteLine($"24 | ZContact%: {hitter.ZContactPercentage}");
                    Console.WriteLine($"25 | SwStr%: {hitter.SwingingStrikePercentage}");
                    Console.WriteLine($"26 | BB%+: {hitter.WalkPercentagePlus}");
                    Console.WriteLine($"27 | K%+: {hitter.StrikeoutPercentagePlus}");
                    Console.WriteLine($"28 | OBP%+: {hitter.OnBasePercentagePlus}");
                    Console.WriteLine($"29 | SLG%+: {hitter.SluggingPercentagePlus}");
                    Console.WriteLine($"30 | ISO+: {hitter.IsoPlus}");
                    Console.WriteLine($"31 | BABIP+: {hitter.BabipPlus}");
                    Console.WriteLine($"32 | LD%+: {hitter.LinedrivePercentagePlus}");
                    Console.WriteLine($"33 | GB%+: {hitter.GroundBallPercentagePlus}");
                    Console.WriteLine($"34 | FB%+: {hitter.FlyBallPercentagePlus}");
                    Console.WriteLine($"35 | HR%+: {hitter.HomeRunPerFlyBallPercentagePlus}");
                    Console.WriteLine($"36 | Pull%+: {hitter.PullPercentagePlus}");
                    Console.WriteLine($"37 | Center%+: {hitter.CenterPercentagePlus}");
                    Console.WriteLine($"38 | Oppo%+: {hitter.OppoPercentagePlus}");
                    Console.WriteLine($"39 | Soft%+: {hitter.SoftPercentagePlus}");
                    Console.WriteLine($"40 | Medium%+: {hitter.MediumPercentagePlus}");
                    Console.WriteLine($"41 | Hard%+: {hitter.HardPercentagePlus}");
                    Console.WriteLine();
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

// _h.Dig(rowNumberFromPageSizeDropdown);
// int numberOfRowsInTable = Convert.ToInt32(rowNumberFromPageSizeDropdown[0]);
// Console.WriteLine($"numberOfRowsInTable: {numberOfRowsInTable}");


// var endPoint = (_fgEndPoints.FgHitterMasterReport(positionEnum: PositionEnum.Catcher, year: 2017).EndPointUri);

                    // Stopwatch sw1 = Stopwatch.StartNew();
                    // sw1.Stop();
                    // var swTimeElapsed = sw1.Elapsed.Seconds;
                    // Console.WriteLine($"stopwatch.swTimeElapsed: {swTimeElapsed} seconds");




                    // System.Threading.Thread.Sleep(62000);

                    // 11285
                    // await page.GoToAsync(_fgEndPoints.FgHitterMasterReport("c",100, 2019).EndPointUri,60000);

                    // 7058
                    // await page.GoToAsync(_fgHitterMasterReportLink);

                    // 7783
                    // var endPoint = (_fgEndPoints.FgHitterMasterReport("c",100,2019).EndPointUri);






// foreach(var hitter in fgHitterList)
//                 {
//                     Console.WriteLine($"0  | #: {hitter.FanGraphsRowNumber}");
//                     Console.WriteLine($"1  | NAME: {hitter.FanGraphsName}");
//                     Console.WriteLine($"2  | TEAM: {hitter.FanGraphsTeam}");
//                     Console.WriteLine($"3  | AGE: {hitter.Age}");

//                     Console.WriteLine($"4  | GAMES PLAYED: {hitter.GamesPlayed}");
//                     Console.WriteLine($"5  | AT BATS: {hitter.AtBats}");
//                     Console.WriteLine($"6  | PA: {hitter.PlateAppearances}");

//                     Console.WriteLine($"7  | RUNS: {hitter.Runs}");
//                     Console.WriteLine($"8  | HOME RUNS: {hitter.HomeRuns}");
//                     Console.WriteLine($"9  | RBI: {hitter.RunsBattedIn}");
//                     Console.WriteLine($"10 | SBs: {hitter.StolenBases}");
//                     Console.WriteLine($"11 | BBs: {hitter.Walks}");
//                     Console.WriteLine($"12 | AVG: {hitter.BattingAverage}");

//                     Console.WriteLine($"13 | BB%: {hitter.WalkPercentage}");
//                     Console.WriteLine($"14 | K%: {hitter.StrikeoutPercentage}");
//                     Console.WriteLine($"15 | BB/K: {hitter.WalksPerStrikeout}");
//                     Console.WriteLine($"16 | ISO: {hitter.Iso}");
//                     Console.WriteLine($"17 | BABIP: {hitter.Babip}");

//                     Console.WriteLine($"18 | FB%: {hitter.FlyBallPercentage}");
//                     Console.WriteLine($"19 | Pull%: {hitter.PullPercentage}");
//                     Console.WriteLine($"20 | Soft%: {hitter.SoftPercentage}");
//                     Console.WriteLine($"21 | Hard%: {hitter.HardPercentage}");
//                     Console.WriteLine($"22 | wRC+: {hitter.wRcPlus}");


//                     Console.WriteLine($"23 | OSwing%: {hitter.OSwingPercentage}");
//                     Console.WriteLine($"24 | ZContact%: {hitter.ZContactPercentage}");
//                     Console.WriteLine($"25 | SwStr%: {hitter.SwingingStrikePercentage}");


//                     Console.WriteLine($"26 | BB%+: {hitter.WalkPercentagePlus}");
//                     Console.WriteLine($"27 | K%+: {hitter.StrikeoutPercentagePlus}");

//                     Console.WriteLine($"28 | OBP%+: {hitter.OnBasePercentagePlus}");
//                     Console.WriteLine($"29 | SLG%+: {hitter.SluggingPercentagePlus}");
//                     Console.WriteLine($"30 | ISO+: {hitter.IsoPlus}");
//                     Console.WriteLine($"31 | BABIP+: {hitter.BabipPlus}");

//                     Console.WriteLine($"32 | LD%+: {hitter.LinedrivePercentagePlus}");
//                     Console.WriteLine($"33 | GB%+: {hitter.GroundBallPercentagePlus}");
//                     Console.WriteLine($"34 | FB%+: {hitter.FlyBallPercentagePlus}");
//                     Console.WriteLine($"35 | HR%+: {hitter.HomeRunPerFlyBallPercentagePlus}");

//                     Console.WriteLine($"36 | Pull%+: {hitter.PullPercentagePlus}");
//                     Console.WriteLine($"37 | Center%+: {hitter.CenterPercentagePlus}");
//                     Console.WriteLine($"38 | Oppo%+: {hitter.OppoPercentagePlus}");

//                     Console.WriteLine($"39 | Soft%+: {hitter.SoftPercentagePlus}");
//                     Console.WriteLine($"40 | Medium%+: {hitter.MediumPercentagePlus}");
//                     Console.WriteLine($"41 | Hard%+: {hitter.HardPercentagePlus}");


//                     Console.WriteLine();
//                 }




// fgHitter.FanGraphsRowNumber = (int)allValuesInRow[0];
// fgHitter.FanGraphsName = (string)allValuesInRow[1];
// fgHitter.FanGraphsTeam = (string)allValuesInRow[2];
// fgHitter.Age = (int)allValuesInRow[3];

// fgHitter.GamesPlayed = (int)allValuesInRow[4];
// fgHitter.AtBats = (int)allValuesInRow[5];
// fgHitter.PlateAppearances = (int)allValuesInRow[6];

// fgHitter.Runs = (int)allValuesInRow[7];
// fgHitter.HomeRuns = (int)allValuesInRow[8];
// fgHitter.RunsBattedIn = (int)allValuesInRow[9];
// fgHitter.StolenBases = (int)allValuesInRow[10];
// fgHitter.Walks = (int)allValuesInRow[11];
// fgHitter.BattingAverage = (decimal)allValuesInRow[12];

// fgHitter.WalkPercentage = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[13]);
// fgHitter.StrikeoutPercentage = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[14]);
// fgHitter.WalksPerStrikeout = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[15]);
// fgHitter.Iso = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[16]);
// fgHitter.Babip = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[17]);

// fgHitter.FlyBallPercentage = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[18]);
// fgHitter.PullPercentage = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[19]);
// fgHitter.SoftPercentage = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[20]);
// fgHitter.HardPercentage = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[21]);
// fgHitter.wRcPlus = (int)allValuesInRow[22];


// fgHitter.OSwingPercentage = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[23]);
// fgHitter.ZContactPercentage = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[24]);
// fgHitter.SwingingStrikePercentage = ConvertCellWithPercentageSymbolToDecimal(allValuesInRow[25]);

// fgHitter.WalkPercentagePlus = (int)allValuesInRow[26];
// fgHitter.StrikeoutPercentagePlus = (int)allValuesInRow[27];
// fgHitter.OnBasePercentagePlus = (int)allValuesInRow[28];
// fgHitter.SluggingPercentagePlus = (int)allValuesInRow[29];
// fgHitter.IsoPlus = (int)allValuesInRow[30];
// fgHitter.BabipPlus = (int)allValuesInRow[31];
// fgHitter.LinedrivePercentagePlus = (int)allValuesInRow[32];
// fgHitter.GroundBallPercentagePlus = (int)allValuesInRow[33];
// fgHitter.FlyBallPercentagePlus = (int)allValuesInRow[34];
// fgHitter.HomeRunPerFlyBallPercentagePlus = (int)allValuesInRow[35];
// fgHitter.PullPercentagePlus = (int)allValuesInRow[36];
// fgHitter.CenterPercentagePlus = (int)allValuesInRow[37];
// fgHitter.OppoPercentagePlus = (int)allValuesInRow[38];
// fgHitter.SoftPercentagePlus = (int)allValuesInRow[39];
// fgHitter.MediumPercentagePlus = (int)allValuesInRow[40];
// fgHitter.HardPercentagePlus = (int)allValuesInRow[41];
