using System;
using System.Collections.Generic;
using System.Linq;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.FanGraphs;
using BaseballScraper.Scrapers;
using BaseballScraper.EndPoints;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers.FanGraphs
{
    [Route("fangraphs")]
    public class FanGraphsHomeController: Controller
    {
        private Constants _c                            = new Constants();
        private static FanGraphsUriEndPoints _endPoints = new FanGraphsUriEndPoints();

        public FanGraphsHomeController() {}


        [HttpGet]
        [Route("")]
        public IActionResult ViewFanGraphsHomePage()
        {
            _c.Start.ThisMethod();

            SetInitialUrlToScrape();

            string test = "test";

            return Content($"test is {test}");
        }

        private string SetInitialUrlToScrape ()
        {
            _c.Start.ThisMethod ();

            // original
            string initialUrl = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=1_50";

            // Complete.ThisMethod ();
            return initialUrl;
        }

        private string GetPathForNumberOfPagesToScrape()
        {
            _c.Start.ThisMethod();

            string tableBodyXpath = "//*[@id='LeaderBoard1_dg1_ctl00']/thead/tr[1]/td/div/div[5]/strong[2]";

            return tableBodyXpath;
        }


        // THIS WORKS
        private int GetNumOfPagesToScrape ()
        {
            _c.Start.ThisMethod ();
            string urlToBeginScraping = SetInitialUrlToScrape ();

            HtmlWeb htmlWeb = new HtmlWeb ();

            //HTML WEB 1 ---> HtmlAgilityPack.HtmlDocument
            var htmlWeb1 = htmlWeb.Load (urlToBeginScraping);

            // scrape fangraphs page to get number of pages
            var numPagesToScrape = GetPathForNumberOfPagesToScrape ();

            // TABLE BODY ---> HtmlAgilityPack.HtmlNodeCollection
            var numOfHtmlElement = htmlWeb1.DocumentNode.SelectNodes (numPagesToScrape);

            string numOfPagesToScrapeString = numOfHtmlElement[0].InnerText;
            int    numOfPagesToScrapeInt    = Convert.ToInt32 (numOfPagesToScrapeString);

            Console.WriteLine($"NUMBER OF PAGES TO SCRAPE IS: {numOfPagesToScrapeInt}");

            // Complete.ThisMethod ();
            return numOfPagesToScrapeInt;
        }

        // THIS WORKS
        private List<string> GetUrlsOfPagesToScrape ()
        {
            _c.Start.ThisMethod ();

            List<string> urlsOfPagesToScrape = new List<string> ();

            int numOfPagesToScrape = GetNumOfPagesToScrape ();

            for (var i = 1; i <= numOfPagesToScrape; i++)
            {
                var baseOfUrlToScrape = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=";

                var pageNumber = i;

                var numOfRecordsListedOnPage = "_50";

                string urlToScrape = $"{baseOfUrlToScrape}{i}{numOfRecordsListedOnPage}";

                urlsOfPagesToScrape.Add (urlToScrape);
            }

            // Complete.ThisMethod ();
            return urlsOfPagesToScrape;
        }

        // THIS WORKS
        private string GetXPathOfTableBodyToScrape ()
        {
            _c.Start.ThisMethod ();
            string tableBodyXpath = "//*[@id='LeaderBoard1_dg1_ctl00']/tbody";

            // Complete.ThisMethod ();
            return tableBodyXpath;
        }


        // THIS WORKS
        public void HitterCrawler ()
        {
            _c.Start.ThisMethod ();

            List<string> listOfUrls = GetUrlsOfPagesToScrape ().ToList ();
            var  numOfUrls          = listOfUrls.Count ();
            numOfUrls.Intro ("url count");

            string tableBodyXpath = GetXPathOfTableBodyToScrape ();

            HtmlWeb htmlWeb = new HtmlWeb ();

            int loopCount = 1;
            foreach (var urlForThisPageInLoop in listOfUrls)
            {
                urlForThisPageInLoop.Intro ("single page url");
                loopCount.Intro ("this is loop number");
                loopCount++;

                var htmlWeb1 = htmlWeb.Load (urlForThisPageInLoop);

                // COUNT ---> 1
                // TABLE BODY ---> HtmlAgilityPack.HtmlNodeCollection
                var tableBody = htmlWeb1.DocumentNode.SelectNodes (tableBodyXpath);

                foreach (var tableBodyNode in tableBody)
                {
                    // this can be gotten from Chrome; right-click 'Inspect', view the html for the table, right click on any item(in this case a row) and select Copy > tableBodyXpath
                    string preForRows  = "//*[@id='LeaderBoard1_dg1_ctl00__";
                    string postForRows = "']";

                    // 52 for first page; 13 for last page
                    int tbNodeChildCount = tableBodyNode.ChildNodes.Count ();
                    // tbNodeChildCount.Intro ("count of child nodes / rows");
                    int adjustedCount = tbNodeChildCount - 2;
                    // adjustedCount.Intro ("adjusted count of child nodes / rows");

                    for (var i = 0; i <= adjustedCount - 1; i++)
                    {
                        // TR FOR EACH PLAYER ---> //*[@id='LeaderBoard1_dg1_ctl00__11']
                        string trForEachPlayer = $"{preForRows}{i}{postForRows}";

                        // COUNT --> 1
                        // TABLE BODY NODE FOR EACH PLAYER ---> HtmlAgilityPack.HtmlNodeCollection
                        var nodeRowEachPlayer = tableBodyNode.SelectNodes (trForEachPlayer);

                        foreach (var playerItem in nodeRowEachPlayer)
                        {
                            // COUNT --> 24
                            var playerItemChildrenCount = playerItem.ChildNodes.Count ();
                            // playerItemChildrenCount.Intro("player item children count");

                            //  e.g. --->   12Manny Machado- - -101440245066710.9 %12.5 %.252.310.311.384.563.393152-0.526.3-3.23.9
                            string preForData  = $"{trForEachPlayer}/td[";
                            string postForData = "]";

                            List<string> playerItems = new List<string> ();

                            int numberOfColumns = 22;
                            int keyCount        = 1;

                            for (var j = 1; j <= numberOfColumns; j++)
                            {
                                // TD FOR EACH PLAYER ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[1]
                                string tdForEachPlayer = $"{preForData}{j}{postForData}";

                                // COUNT --> 1
                                // PLAYERS NODE COLLECTION ---> HtmlAgilityPack.HtmlNodeCollection
                                var playersNodeCollection = playerItem.SelectNodes (tdForEachPlayer);

                                // go this way if looking for player name or player team
                                if (j == 2 || j == 3)
                                {
                                    try
                                    {
                                        // Extensions.Spotlight("MARK A");
                                        string postPost = "/a";

                                        // NAME AND TEAM X-PATHS ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[2]/a
                                        string nameAndTeamTableBodyXpaths = $"{tdForEachPlayer}{postPost}";
                                        // nameAndTeamTableBodyXpaths.Intro("name and team table body x paths");

                                        // COUNT --> 1
                                        // NAME AND TEAM ---> HtmlAgilityPack.HtmlNodeCollection
                                        var nameAndTeam = playerItem.SelectNodes (nameAndTeamTableBodyXpaths);
                                        // nameAndTeam.Intro("name and team");

                                        foreach (var actualNumber in nameAndTeam)
                                        {
                                            // Extensions.Spotlight("MARK B");
                                            // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
                                            var numToAddToList = actualNumber.InnerText;
                                            playerItems.Add (numToAddToList);
                                            // numToAddToList.Intro("num to add to list");
                                        }
                                    }

                                    catch
                                    {
                                        Extensions.Spotlight ("NAME or TEAM is broken");
                                        string cellIsBlank = "";
                                        playerItems.Add (cellIsBlank);
                                    }
                                }

                                else
                                {
                                    // Extensions.Spotlight("MARK D");
                                    foreach (var actualNumber in playersNodeCollection)
                                    {
                                        // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
                                        var numToAddToList = actualNumber.InnerText;
                                        playerItems.Add (numToAddToList);
                                        // numToAddToList.Intro("num to add to list");
                                    }
                                }

                                keyCount++;
                            }

                            List<FGHitter> hitters = new List<FGHitter> ();

                            FGHitter newFGHitter = new FGHitter
                            {
                                FanGraphsName = playerItems[1],
                                FanGraphsTeam = playerItems[2],
                                GP            = playerItems[3],
                                PA            = playerItems[4],
                                HR            = playerItems[5],
                                R             = playerItems[6],
                                RBI           = playerItems[7],
                                SB            = playerItems[8],
                                BB_percent    = playerItems[9],
                                K_percent     = playerItems[10],
                                ISO           = playerItems[11],
                                BABIP         = playerItems[12],
                                AVG           = playerItems[13],
                                OBP           = playerItems[14],
                                SLG           = playerItems[15],
                                wOBA          = playerItems[16],
                                wRC_plus      = playerItems[17],
                                BsR           = playerItems[18],
                                Off           = playerItems[19],
                                Def           = playerItems[20],
                                WAR           = playerItems[21],
                            };

                            hitters.Add (newFGHitter);

                            foreach (var hitter in hitters)
                            {
                                Console.WriteLine (hitter.FanGraphsName);
                                Console.WriteLine (hitter.WAR);
                            }
                        }
                    }
                }
            }
        }
    }
}