using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.FanGraphs;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;


#pragma warning disable CS0219, CS0414, CS1998, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006, MA0051
namespace BaseballScraper.Controllers.FanGraphs
{
    // TO-DO: need to extract hitter info
    [Route("api/fangraphs/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FanGraphsHomeController: ControllerBase
    {
        private readonly Helpers _helpers;

        public FanGraphsHomeController(Helpers helpers)
        {
            _helpers = helpers;
        }



        [HttpGet]
        [Route("")]
        public IActionResult ViewFanGraphsHomePage()
        {
            _helpers.StartMethod();
            return Ok();
        }

        private string SetInitialUrlToScrape ()
        {
            _helpers.StartMethod();

            // original
            const string initialUrl = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=1_50";

            return initialUrl;
        }

        private string GetPathForNumberOfPagesToScrape() => "//*[@id='LeaderBoard1_dg1_ctl00']/thead/tr[1]/td/div/div[5]/strong[2]";

        // {
        //     return "//*[@id='LeaderBoard1_dg1_ctl00']/thead/tr[1]/td/div/div[5]/strong[2]";
        // }


        // THIS WORKS
        private int GetNumOfPagesToScrape ()
        {
            _helpers.StartMethod();
            string urlToBeginScraping = SetInitialUrlToScrape ();

            HtmlWeb htmlWeb       = new HtmlWeb ();
            HtmlDocument htmlWeb1 = htmlWeb.Load (urlToBeginScraping);

            // scrape fangraphs page to get number of pages
            string numPagesToScrape = GetPathForNumberOfPagesToScrape ();

            HtmlNodeCollection numOfHtmlElement = htmlWeb1.DocumentNode.SelectNodes (numPagesToScrape);

            string numOfPagesToScrapeString = numOfHtmlElement[0].InnerText;
            int    numOfPagesToScrapeInt    = Convert.ToInt32(numOfPagesToScrapeString, CultureInfo.CurrentCulture);

            Console.WriteLine($"NUMBER OF PAGES TO SCRAPE IS: {numOfPagesToScrapeInt}");

            return numOfPagesToScrapeInt;
        }

        // THIS WORKS
        private List<string> GetUrlsOfPagesToScrape ()
        {
            _helpers.StartMethod();

            List<string> urlsOfPagesToScrape = new List<string> ();

            int numOfPagesToScrape = GetNumOfPagesToScrape ();

            for (int i = 1; i <= numOfPagesToScrape; i++)
            {
                const string baseOfUrlToScrape = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=";

                int pageNumber = i;

                const string numOfRecordsListedOnPage = "_50";

                string urlToScrape = $"{baseOfUrlToScrape}{i}{numOfRecordsListedOnPage}";

                urlsOfPagesToScrape.Add (urlToScrape);
            }
            return urlsOfPagesToScrape;
        }

        // THIS WORKS
        private string GetXPathOfTableBodyToScrape ()
        {
            return "//*[@id='LeaderBoard1_dg1_ctl00']/tbody";
        }


        // THIS WORKS
        public void HitterCrawler ()
        {
            _helpers.StartMethod();

            List<string> listOfUrls = GetUrlsOfPagesToScrape ().ToList ();
            int numOfUrls          = listOfUrls.Count;
            _helpers.Intro (numOfUrls, "url count");

            string tableBodyXpath = GetXPathOfTableBodyToScrape ();

            HtmlWeb htmlWeb = new HtmlWeb ();

            int loopCount = 1;
            foreach (string urlForThisPageInLoop in listOfUrls)
            {
                _helpers.Intro (urlForThisPageInLoop, "single page url");
                _helpers.Intro (loopCount, "this is loop number");
                loopCount++;

                HtmlDocument htmlWeb1 = htmlWeb.Load (urlForThisPageInLoop);

                // COUNT = 1
                HtmlNodeCollection tableBody = htmlWeb1.DocumentNode.SelectNodes (tableBodyXpath);

                foreach (HtmlNode tableBodyNode in tableBody)
                {
                    // * This can be gotten from Chrome
                    // * Right-click 'Inspect', view the html for the table
                    // * Right-click on any item(in this case a row) and select Copy > tableBodyXpath
                    const string preForRows = "//*[@id='LeaderBoard1_dg1_ctl00__";
                    const string postForRows = "']";

                    // 52 for first page; 13 for last page
                    int tbNodeChildCount = tableBodyNode.ChildNodes.Count;
                    int adjustedCount = tbNodeChildCount - 2;

                    for (int i = 0; i <= adjustedCount - 1; i++)
                    {
                        // TR FOR EACH PLAYER ---> //*[@id='LeaderBoard1_dg1_ctl00__11']
                        string trForEachPlayer = $"{preForRows}{i}{postForRows}";

                        // Count = 1
                        HtmlNodeCollection nodeRowEachPlayer = tableBodyNode.SelectNodes (trForEachPlayer);

                        foreach (HtmlNode playerItem in nodeRowEachPlayer)
                        {
                            // Count = 24
                            int playerItemChildrenCount = playerItem.ChildNodes.Count;

                            //  e.g. --->   12Manny Machado- - -101440245066710.9 %12.5 %.252.310.311.384.563.393152-0.526.3-3.23.9
                            string preForData  = $"{trForEachPlayer}/td[";
                            const string postForData = "]";

                            List<string> playerItems = new List<string> ();

                            const int numberOfColumns = 22;
                            int keyCount        = 1;

                            for (int j = 1; j <= numberOfColumns; j++)
                            {
                                // TD FOR EACH PLAYER ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[1]
                                string tdForEachPlayer = $"{preForData}{j}{postForData}";

                                // Count = 1
                                HtmlNodeCollection playersNodeCollection = playerItem.SelectNodes (tdForEachPlayer);

                                // go this way if looking for player name or player team
                                if (j == 2 || j == 3)
                                {
                                    try
                                    {
                                        const string postPost = "/a";

                                        // NAME AND TEAM X-PATHS ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[2]/a
                                        string nameAndTeamTableBodyXpaths = $"{tdForEachPlayer}{postPost}";

                                        // Count = 1
                                        HtmlNodeCollection nameAndTeam = playerItem.SelectNodes (nameAndTeamTableBodyXpaths);

                                        foreach (HtmlNode actualNumber in nameAndTeam)
                                        {
                                            // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
                                            string numToAddToList = actualNumber.InnerText;
                                            playerItems.Add (numToAddToList);
                                        }
                                    }

                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex);
                                        _helpers.Spotlight("NAME or TEAM is broken");
                                        const string cellIsBlank = "";
                                        playerItems.Add(cellIsBlank);
                                    }
                                }

                                else
                                {
                                    foreach (HtmlNode actualNumber in playersNodeCollection)
                                    {
                                        // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
                                        string numToAddToList = actualNumber.InnerText;
                                        playerItems.Add (numToAddToList);
                                    }
                                }
                                keyCount++;
                            }

                            List<FanGraphsHitter> hitters = new List<FanGraphsHitter> ();

                            FanGraphsHitter newFGHitter = new FanGraphsHitter
                            {
                                FanGraphsName                       = playerItems[1],
                                FanGraphsTeam                       = playerItems[2],
                                GamesPlayed                         = Convert.ToInt32(playerItems[3], CultureInfo.CurrentCulture),
                                PlateAppearances                    = Convert.ToInt32(playerItems[4], CultureInfo.CurrentCulture),
                                HomeRuns                            = Convert.ToInt32(playerItems[5], CultureInfo.CurrentCulture),
                                Runs                                = Convert.ToInt32(playerItems[6], CultureInfo.CurrentCulture),
                                RunsBattedIn                        = Convert.ToInt32(playerItems[7], CultureInfo.CurrentCulture),
                                StolenBases                         = Convert.ToInt32(playerItems[8], CultureInfo.CurrentCulture),
                                WalkPercentage                      = Convert.ToInt32(playerItems[9], CultureInfo.CurrentCulture),
                                StrikeoutPercentage                 = Convert.ToInt32(playerItems[10], CultureInfo.CurrentCulture),
                                Iso                                 = Convert.ToInt32(playerItems[11], CultureInfo.CurrentCulture),
                                Babip                               = Convert.ToInt32(playerItems[12], CultureInfo.CurrentCulture),
                                BattingAverage                      = Convert.ToInt32(playerItems[13], CultureInfo.CurrentCulture),
                                OnBasePercentage                    = Convert.ToInt32(playerItems[14], CultureInfo.CurrentCulture),
                                SluggingPercentage                  = Convert.ToInt32(playerItems[15], CultureInfo.CurrentCulture),
                                wOba                                = Convert.ToInt32(playerItems[16], CultureInfo.CurrentCulture),
                                wRcPlus                             = Convert.ToInt32(playerItems[17], CultureInfo.CurrentCulture),
                                BaseRunningRunsAboveReplacement     = Convert.ToInt32(playerItems[18], CultureInfo.CurrentCulture),
                                Offense                             = Convert.ToInt32(playerItems[19], CultureInfo.CurrentCulture),
                                Defense                             = Convert.ToInt32(playerItems[20], CultureInfo.CurrentCulture),
                                WinsAboveReplacement                = Convert.ToInt32(playerItems[21], CultureInfo.CurrentCulture),
                            };

                            hitters.Add (newFGHitter);

                            foreach (FanGraphsHitter hitter in hitters)
                            {
                                Console.WriteLine (hitter.FanGraphsName);
                                Console.WriteLine (hitter.WinsAboveReplacement);
                            }
                        }
                    }
                }
            }
        }
    }
}
