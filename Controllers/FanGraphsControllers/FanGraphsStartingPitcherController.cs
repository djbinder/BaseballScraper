using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.FanGraphs;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers.FanGraphs
{
    #region OVERVIEW ------------------------------------------------------------

        /// <summary> </summary>
        /// <list> INDEX
        ///     <item> View Page <see cref="FanGraphsStartingPitcherController.ViewFanGraphsStartingPitcherPage()"/>
        ///     <item> Set Initial Url to Scrape <see cref="FanGraphsStartingPitcherController.SetInitialUrlToScrape(int, int, int, int)"/> </item>
        ///     <item> Get Number of Pages to Scrape <see cref="FanGraphsStartingPitcherController.GetNumberOfPagesToScrape(int, int, int, int)"/> </item>
        ///     <item> Get Urls of Pages to Scrape <see cref="FanGraphsStartingPitcherController.GetUrlsOfPagesToScrape()"/> </item>
        ///     <item> Get Urls of Pages to Scrape <see cref="FanGraphsStartingPitcherController.GetUrlsOfPagesToScrape(int, int, int)"/> </item>
        ///     <item> Scrape Pitchers and Create List <see cref="FanGraphsStartingPitcherController.ScrapePitchersAndCreateList()"/> </item>
        ///     <item> Count the Nodes Children <see cref="FanGraphsStartingPitcherController.CountTheNodesChildren(HtmlNode)"/> </item>
        ///     <item> Get Table Header Values <see cref="FanGraphsStartingPitcherController.GetTableHeaderValues(string)"/> </item>
        /// </list>
        ///
        /// <list> RESOURCES
        ///     <item> FanGraphs custom report this scraper targets: https://bit.ly/2N2somI </item>
        /// </list>

    #endregion OVERVIEW ------------------------------------------------------------


    [Route("fangraphs")]
    public class FanGraphsStartingPitcherController: Controller
    {
        private Helpers _h                              = new Helpers();
        private static FanGraphsUriEndPoints _endPoints = new FanGraphsUriEndPoints();
        private string pathToGetNumberOfPagesToScrape   = "//*[@id='LeaderBoard1_dg1_ctl00']/thead/tr[1]/td/div/div[5]/strong[2]";
        private string pathOfTableBodyToScrape          = "//*[@id='LeaderBoard1_dg1_ctl00']/tbody";
        private string pathOfTableHeaderToScrape        = "//*[@id='LeaderBoard1_dg1_ctl00']/thead/tr[2]";

        public FanGraphsStartingPitcherController() {}

        [HttpGet]
        [Route("sp")]
        public void ViewFanGraphsStartingPitcherPage()
        {

        }



        #region SETUP PRIOR TO SCRAPING ------------------------------------------------------------

            // STATUS: this works
            // Step 1:
            /// <summary> This defines the first url to scrape; At times, you may need to loop through multiple urls and the url defined here is the first url in the loop </summary>
            /// <param name="minInningsPitched"> The minimum number of innings pitched a pitcher needs to be included in the results of the scrape </param>
            /// <param name="year"> The Mlb season year </param>
            /// <param name="page"> The page to being scraping; this typically will be one. But if you want to start on page 2 (for example), just set 'page' to 2 </param>
            /// <param name="recordsPerPage"> The number of rows in the table; Standard options include 30, 50 , 100; This will ultimately influence the total number of urls and their tables to scrape </param>
            /// <returns> A string url </returns>
            public string SetInitialUrlToScrape(int minInningsPitched, int year, int page, int recordsPerPage)
            {
                var    newEndPoint        = _endPoints.PitchingLeadersMasterStatsReportEndPoint(minInningsPitched, year, page, recordsPerPage);
                string uriToBeginScraping = newEndPoint.EndPointUri.ToString();
                return uriToBeginScraping;
            }


            // STATUS: this works
            // Step 2:
            /// <summary> This counts the number of urls that will be scraped; It examines a specific html element on the fangraphs html page that shows the number of pages </summary>
            /// <example> '70 items in 3 page' --> the '3' is what this method is looking for </example>
            /// <param name="minInningsPitched"> The minimum number of innings pitched a pitcher needs to be included in the results of the scrape </param>
            /// <param name="year"> The Mlb season year </param>
            /// <param name="page"> The page to being scraping; this typically will be one. But if you want to start on page 2 (for example), just set 'page' to 2 </param>
            /// <param name="recordsPerPage"> The number of rows in the table; Standard options include 30, 50 , 100; This will ultimately influence the total number of urls and their tables to scrape </param>
            /// <returns> A number of the number of pages to scrape as part of the loop </returns>
            public int GetNumberOfPagesToScrape(int minInningsPitched, int year, int page, int recordsPerPage)
            {
                string uriToBeginScraping = SetInitialUrlToScrape(minInningsPitched, year, page, recordsPerPage);

                HtmlWeb htmlWeb  = new HtmlWeb();
                var     urisHtml = htmlWeb.Load(uriToBeginScraping);

                var htmlElement = urisHtml.DocumentNode.SelectNodes(pathToGetNumberOfPagesToScrape);

                string numberOfPagesToScrapeString = htmlElement[0].InnerText;
                int    numberOfPagesToScrapeInt    = Convert.ToInt32(numberOfPagesToScrapeString);

                bool showGuardRails = false;
                if(showGuardRails == true)
                {
                    _h.Spotlight("----- GET NUMBER OF PAGES TO SCRAPE -----");
                    Console.WriteLine($"NUMBER OF PAGES TO SCRAPE IS: {numberOfPagesToScrapeInt}");
                    Console.WriteLine();
                }
                return numberOfPagesToScrapeInt;
            }


            // STATUS: this works
            // Step 3 [1 of 2 Options]
            /// <summary> OPTION 1: variables defined within the method (i.e minInningsPitched, year, page, recordsPerPage ). Retrieves all urls of pages that will be scraped and adds them to a list  </summary>
            /// <example> '70 items in 3 page' --> the '3' is what this method is looking for </example>
            /// <param name="minInningsPitched"> The minimum number of innings pitched a pitcher needs to be included in the results of the scrape </param>
            /// <param name="year"> The Mlb season year </param>
            /// <param name="page"> The page to being scraping; this typically will be one. But if you want to start on page 2 (for example), just set 'page' to 2 </param>
            /// <param name="recordsPerPage"> The number of rows in the table; Standard options include 30, 50 , 100; This will ultimately influence the total number of urls and their tables to scrape </param>
            /// <returns> A list of strings representing urls to be scraped </returns>
            private List<string> GetUrlsOfPagesToScrape()
            {
                List<string> urlsOfPagesToScrape = new List<string> ();

                int minInningsPitched = 170;
                int year              = 2018;
                int recordsPerPage    = 50;

                int numberOfPagesToScrape = GetNumberOfPagesToScrape (minInningsPitched, year, 1, recordsPerPage);

                for (var i = 1; i <= numberOfPagesToScrape; i++)
                {
                    var urlToScrape = _endPoints.PitchingLeadersMasterStatsReportEndPoint(minInningsPitched, year, i, recordsPerPage);

                    var urlToScrapeEndPointUri = urlToScrape.EndPointUri;

                    string urlToScrapeEndPointUriToString = urlToScrapeEndPointUri.ToString();
                    urlsOfPagesToScrape.Add (urlToScrapeEndPointUri);

                    bool showGuardRails = false;
                    if(showGuardRails == true)
                    {
                        _h.Spotlight("----- GET URLS OF PAGES TO SCRAPE -----");
                        Console.WriteLine($"URL TO SCRAPE # {i}: {urlToScrapeEndPointUri}");
                        Console.WriteLine();
                    }
                }
                return urlsOfPagesToScrape;
            }


            // STATUS: this works
            // Step 3 [2 of 2 Options]
            /// <summary> OPTION 2: parameters are passed into the  method (i.e minInningsPitched, year, page, recordsPerPage ). Retrieves all urls of pages that will be scraped and adds them to a list  </summary>
            /// <example> '70 items in 3 page' --> the '3' is what this method is looking for </example>
            /// <param name="minInningsPitched"> The minimum number of innings pitched a pitcher needs to be included in the results of the scrape </param>
            /// <param name="year"> The Mlb season year </param>
            /// <param name="page"> The page to being scraping; this typically will be one. But if you want to start on page 2 (for example), just set 'page' to 2 </param>
            /// <param name="recordsPerPage"> The number of rows in the table; Standard options include 30, 50 , 100; This will ultimately influence the total number of urls and their tables to scrape </param>
            /// <returns> A list of strings representing urls to be scraped </returns>
            private List<string> GetUrlsOfPagesToScrape(int minInningsPitched, int year, int recordsPerPage)
            {
                List<string> urlsOfPagesToScrape = new List<string> ();

                int numberOfPagesToScrape = GetNumberOfPagesToScrape (minInningsPitched, year, 1, recordsPerPage);

                for (var i = 1; i <= numberOfPagesToScrape; i++)
                {
                    var urlToScrape = _endPoints.PitchingLeadersMasterStatsReportEndPoint(minInningsPitched, year, i, recordsPerPage);

                    var urlToScrapeEndPointUri = urlToScrape.EndPointUri;

                    string urlToScrapeEndPointUriToString = urlToScrapeEndPointUri.ToString();
                    urlsOfPagesToScrape.Add (urlToScrapeEndPointUri);

                    bool showGuardRails = false;
                    if(showGuardRails == true)
                    {
                        _h.Spotlight("----- GET URLS OF PAGES TO SCRAPE -----");
                        Console.WriteLine($"URL TO SCRAPE # {i}: {urlToScrapeEndPointUri}");
                        Console.WriteLine();
                    }
                }
                return urlsOfPagesToScrape;
            }

        #endregion SETUP PRIOR TO SCRAPING ------------------------------------------------------------



        #region SCRAPING ------------------------------------------------------------

            // STATUS: this works
            // Step 4
            /// <summary> Scrape the pitchers table and get all their data </summary>
            /// <remarks> Any XPath can be pulled from Chrome; right-click 'Inspect', view the html for the table, right click on any item and select Copy > tableBodyXpath </remarks>
            [Route("scrape")]
            public List<FanGraphsPitcher> ScrapePitchersAndCreateList ()
            {
                List<string> listOfUrlsToLoopThrough = GetUrlsOfPagesToScrape ().ToList ();
                int  numberOfUrlsToScrape            = listOfUrlsToLoopThrough.Count();
                Console.WriteLine($"THERE IS {numberOfUrlsToScrape} tables to scrape");

                HtmlWeb htmlWeb = new HtmlWeb ();

                List<FanGraphsPitcher> listOfFgPitchers = new List<FanGraphsPitcher>();

                int urlNumber = 1;

                foreach (string thisUrl in listOfUrlsToLoopThrough)
                {
                    Console.WriteLine($"THIS IS URL {urlNumber}/{numberOfUrlsToScrape}");
                    urlNumber++;

                    var thisUrlsHtml = htmlWeb.Load (thisUrl);

                    GetTableHeaderValues(thisUrl);

                    // PATH OF TABLE TO SCRAPE --> "//*[@id='LeaderBoard1_dg1_ctl00']/tbody";
                    HtmlNodeCollection thisTablesBody = thisUrlsHtml.DocumentNode.SelectNodes (pathOfTableBodyToScrape);

                    // HTML --> <table id= 'LeaderBoard1_dg1_ctl00'
                    foreach (HtmlNode thisTable in thisTablesBody)
                    {
                        // NUMBER OF ROWS IN THIS TABLE --> equal to the number of rows/player records returned + 2;
                        int numberOfRowsInThisTable = CountTheNodesChildren(thisTable);

                        // NUMBER OF ROWS TO SCRAPE IN THIS TABLE --> you only want player data, so remove the header and footer
                        int numberOfRowsToScrapeInThisTable = numberOfRowsInThisTable - 2;

                        for (var row = 0; row <= numberOfRowsToScrapeInThisTable - 1; row++)
                        {
                            // THIS PLAYERS TABLE ROW PATH return example --> //*[@id='LeaderBoard1_dg1_ctl00__11']
                                // HTML --> <tr id='LeaderBoard1_dg1_ctl00__0'
                            string thisPlayersTableRowPath = $"//*[@id='LeaderBoard1_dg1_ctl00__{row}']";

                            // COUNT --> 1; i.e., each player has one row/record)
                            HtmlNodeCollection thisPlayersTableRowNodeCollection = thisTable.SelectNodes (thisPlayersTableRowPath);

                            foreach (HtmlNode playerItem in thisPlayersTableRowNodeCollection)
                            {
                                // TOTAL NUMBER OF COLUMNS IN TABLE --> counts the total number columns listed in the table
                                var totalNumberOfColumnsInTable = playerItem.ChildNodes.Count ();

                                int numberOfColumnsToScrape = totalNumberOfColumnsInTable - 2;

                                List<string> playerItems = new List<string> ();

                                // Begin looping through every column in the table
                                for (var column = 1; column <= numberOfColumnsToScrape; column++)
                                {
                                    // THIS STATS TABLE ROW PATH return example --> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[1]
                                    string thisStatsTableRowPath = $"{thisPlayersTableRowPath}/td[{column}]";

                                    // COUNT --> 1
                                    HtmlNodeCollection playersNodeCollection = playerItem.SelectNodes (thisStatsTableRowPath);

                                    // If the column is player name or team name, go this way; Column 2 is the player's team and Column 3 is the player's team
                                    // Player name and team name are hyperlinks; because of this the html structure is slightly different than other data cells so a unique approach is needed
                                    if (column == 2 || column == 3)
                                    {
                                        try
                                        {
                                            // NAME AND TEAM NODE COLLECTION PATH return example --> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[2]/a
                                            string nameAndTeamNodeCollectionPath = $"{thisStatsTableRowPath}/a";

                                            // COUNT --> 1
                                            HtmlNodeCollection nameAndTeamNodeCollection = playerItem.SelectNodes (nameAndTeamNodeCollectionPath);

                                            foreach (HtmlNode nameOrTeamValueNode in nameAndTeamNodeCollection)
                                            {
                                                // NAME OR TEAM VALUE --> this is where you get the player's actual name and team name
                                                var nameOrTeamValue = nameOrTeamValueNode.InnerText;
                                                playerItems.Add (nameOrTeamValue);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            // error description: if the team is empty, the 'try' will not work; this happens in cases where a player has played for multiple teams within a season; It shows up as '---' in their table data
                                            Console.WriteLine($"Cell is blank: {ex.Message}");
                                            string cellIsBlank = "";
                                            playerItems.Add (cellIsBlank);
                                        }
                                    }
                                    else
                                    {
                                        foreach (HtmlNode statValueNode in playersNodeCollection)
                                        {
                                            // STAT VALUE --> The players actual numbers for each stat; the numerical value
                                                // return example --> 3.9', '26.3' etc.
                                            var statValue = statValueNode.InnerText;
                                            playerItems.Add (statValue);
                                        }
                                    }
                                }
                                CreateNewFanGraphsPitcher(playerItems);
                                listOfFgPitchers.Add(CreateNewFanGraphsPitcher(playerItems));
                            }
                        }
                    }
                }
                return listOfFgPitchers;
            }

        #endregion SCRAPING ------------------------------------------------------------



        #region HELPERS ------------------------------------------------------------

            // STATUS: this works
            public int CountTheNodesChildren(HtmlNode node)
            {
                int countOfChildren = node.ChildNodes.Count ();
                return countOfChildren;
            }


            // STATUS: this works
            /// <summary> Scrapes the headers of the table to get the stat names (e.g., ERA, WHIP, etc.) </summary>
            /// <param name="thisUrl"> The url of the table you are scraping </param>
            public void GetTableHeaderValues (string thisUrl)
            {
                HtmlWeb htmlWeb = new HtmlWeb ();

                var thisUrlsHtml = htmlWeb.Load (thisUrl);

                // THIS TABLES HEADER type --> HtmlAgilityPack.HtmlNodeCollection (COUNT --> 1)
                // <thead> Xpath --> //*[@id="LeaderBoard1_dg1_ctl00"]/thead/tr[2]";
                    // Note: The 2 in '/tr[2]' is hard coded into the variable; the first row in the header (i.e., '/tr[1]') is the navigation part of the header; We don't want that row. We want the second row which actually has the header values
                var thisTablesHeader = thisUrlsHtml.DocumentNode.SelectNodes(pathOfTableHeaderToScrape);

                foreach(HtmlNode thisHeader in thisTablesHeader)
                {
                    int numberOfHeaderColumns = thisTablesHeader.First().ChildNodes.Count();
                    // numberOfHeaderColumns.Intro("number of header columns");
                    int numberOfHeaderColumnsToScrape = numberOfHeaderColumns - 2;

                    for(var headerColumn = 1; headerColumn <= numberOfHeaderColumnsToScrape; headerColumn++)
                    {
                        // HEADER STAT NAME PATH return example --> //*[@id='LeaderBoard1_dg1_ctl00']/thead/tr[2]/th[16]
                        var headerStatNamePath = $"{pathOfTableHeaderToScrape}/th[{headerColumn}]";

                        HtmlNodeCollection headerStatNameDataCell = thisHeader.SelectNodes(headerStatNamePath);
                        foreach(var headerStatName in headerStatNameDataCell)
                        {
                            // INNER TEXT --> this is the value that you want; the name of the stat (e.g., ERA, WHIP, etc.)
                            var innerText = headerStatName.InnerText;
                            // Console.WriteLine($"STAT NAME: {headerStatName.InnerText}");
                        }
                    }
                }
            }

        #endregion HELPERS ------------------------------------------------------------



        #region TESTING HELP ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Instantiates a new instance of class FanGraphsPitcher </summary>
            /// <param name="playerItems"> A list of values for each of the stats / properties of the FanGraphsPitcher class </param>
            /// <returns> New instance of FanGraphsPitcher class </returns>
            public FanGraphsPitcher CreateNewFanGraphsPitcher(List<string> playerItems)
            {
                int count = 1;

                FanGraphsPitcher newFanGraphsPitcher = new FanGraphsPitcher
                {
                    RecordNumber              = playerItems[0],
                    FanGraphsName             = playerItems[count++],
                    FanGraphsTeam             = playerItems[count++],
                    FanGraphsAge              = playerItems[count++],
                    GamesStarted              = playerItems[count++],
                    InningsPitched            = playerItems[count++],
                    TotalBattersFaced         = playerItems[count++],
                    Wins                      = playerItems[count++],
                    Saves                     = playerItems[count++],
                    Strikeouts                = playerItems[count++],
                    Holds                     = playerItems[count++],
                    EarnedRunAverage          = playerItems[count++],
                    Whip                      = playerItems[count++],
                    StrikeoutPercentage       = playerItems[count++],
                    WalkPercentage            = playerItems[count++],
                    StrikeoutsMinusWalks      = playerItems[count++],
                    StrikeoutsPerNine         = playerItems[count++],
                    WalksPerNine              = playerItems[count++],
                    StrikeoutsDividedByWalks  = playerItems[count++],
                    Balls                     = playerItems[count++],
                    Strikes                   = playerItems[count++],
                    Pitches                   = playerItems[count++],
                    HomeRunsPerNine           = playerItems[count++],
                    GroundBallPercentage      = playerItems[count++],
                    LineDrivePercentage       = playerItems[count++],
                    FlyBallPercentage         = playerItems[count++],
                    InfieldFlyBallPercentage  = playerItems[count++],
                    OSwingPercentage          = playerItems[count++],
                    OSwingPercentagePitchFx   = playerItems[count++],
                    OSwingPercentagePitchInfo = playerItems[count++],
                    ZContactPercentage        = playerItems[count++],
                    ZContactPercentagePitchFx = playerItems[count++],
                    ZContactPercentagePitchInfo = playerItems[count++],
                    ContactPercentage         = playerItems[count++],
                    ContactPercentagePitchFx  = playerItems[count++],
                    ContactPercentagePitchInfo= playerItems[count++],
                    ZonePercentage            = playerItems[count++],
                    ZonePercentagePitchFx     = playerItems[count++],
                    ZonePercentagePitchInfo   = playerItems[count++],
                    FStrikePercentage         = playerItems[count++],
                    SwingingStrikePercentage  = playerItems[count++],
                    PullPercentage            = playerItems[count++],
                    SoftPercentage            = playerItems[count++],
                    sHardPercentage           = playerItems[count++],
                    Babip                     = playerItems[count++],
                    LeftOnBasePercentage      = playerItems[count++],
                    HomeRunsDividedByFlyBalls = playerItems[count++],
                    FastballVelocityPitchFx   = playerItems[count++],
                    EarnedRunAverageRepeat    = playerItems[count++],
                    EarnedRunAverageMinus     = playerItems[count++],
                    Fip                       = playerItems[count++],
                    FipMinus                  = playerItems[count++],
                    EarnedRunAverageMinusFip  = playerItems[count++],
                    XFip                      = playerItems[count++],
                    XFipMinus                 = playerItems[count++],
                    Siera                     = playerItems[count++],
                    RunsPerNine               = playerItems[count++],
                };

                Console.WriteLine($"NAME: {newFanGraphsPitcher.FanGraphsName}");

                // PrintFanGraphsPitcher(newFanGraphsPitcher);
                // Extensions.PrintKeysAndValues(newFanGraphsPitcher);

                return newFanGraphsPitcher;
            }

        #endregion TESTING HELP ------------------------------------------------------------


    }
}