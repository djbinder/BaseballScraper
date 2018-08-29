using System;
using System.Collections.Generic;
using System.Linq;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.FanGraphs;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers.FanGraphs
{
    [Route("fangraphs/sp")]
    public class FanGraphsStartingPitcherController: Controller
    {
        private Constants _c                            = new Constants();
        private static FanGraphsUriEndPoints _endPoints = new FanGraphsUriEndPoints();
        private string pathToGetNumberOfPagesToScrape   = "//*[@id='LeaderBoard1_dg1_ctl00']/thead/tr[1]/td/div/div[5]/strong[2]";
        private string pathOfTableToScrape              = "//*[@id='LeaderBoard1_dg1_ctl00']/tbody";

        public FanGraphsStartingPitcherController() {}


        [HttpGet]
        [Route("")]
        public IActionResult ViewFanGraphsStartingPitcherPage()
        {
            _c.Start.ThisMethod();

            PitcherCrawler();

            string action = "viewing FG starting pitcher page";

            return Content($"CURRENTLY: {action}");
        }


        public string SetInitialUrlToScrape(int minInningsPitched, int year, int page, int recordsPerPage)
        {
            _c.Start.ThisMethod();

            var newEndPoint = _endPoints.PitchingLeadersMasterStatsReportEndPoint(minInningsPitched, year, page, recordsPerPage);
            // Console.WriteLine($"NEW ENDPOINT: {newEndPoint}");

            string uriToBeginScraping = newEndPoint.EndPointUri.ToString();

            return uriToBeginScraping;
        }

        public int GetNumberOfPagesToScrape(int minInningsPitched, int year, int page, int recordsPerPage)
        {
            string uriToBeginScraping = SetInitialUrlToScrape(minInningsPitched, year, page, recordsPerPage);

            HtmlWeb htmlWeb       = new HtmlWeb();
            var     htmlWebLoaded = htmlWeb.Load(uriToBeginScraping);

            var htmlElement = htmlWebLoaded.DocumentNode.SelectNodes(pathToGetNumberOfPagesToScrape);

            string numberOfPagesToScrapeString = htmlElement[0].InnerText;
            int    numberOfPagesToScrapeInt    = Convert.ToInt32(numberOfPagesToScrapeString);

            Console.WriteLine($"NUMBER OF PAGES TO SCRAPE IS: {numberOfPagesToScrapeInt}");

            return numberOfPagesToScrapeInt;
        }

        private List<string> GetUrlsOfPagesToScrape()
        {
            _c.Start.ThisMethod();

            List<string> urlsOfPagesToScrape = new List<string> ();

            int minInningsPitched = 170;
            int year              = 2018;
            int recordsPerPage    = 50;

            int numberOfPagesToScrape = GetNumberOfPagesToScrape (minInningsPitched, year, 1, recordsPerPage);

            for (var i = 1; i <= numberOfPagesToScrape; i++)
            {
                var urlToScrape = _endPoints.PitchingLeadersMasterStatsReportEndPoint(minInningsPitched, year, i, recordsPerPage);

                var urlToScrapeEndPointUri = urlToScrape.EndPointUri;

                // Console.WriteLine($"URL TO SCRAPE # {i}: {urlToScrapeEndPointUri}");

                string urlToScrapeEndPointUriToString = urlToScrapeEndPointUri.ToString();

                urlsOfPagesToScrape.Add (urlToScrapeEndPointUri);
            }
            return urlsOfPagesToScrape;
        }


        public void PitcherCrawler ()
        {
            _c.Start.ThisMethod ();

            List<string> listOfUrls = GetUrlsOfPagesToScrape ().ToList ();

            HtmlWeb htmlWeb = new HtmlWeb ();

            // int loopCount = 1;
            foreach (var urlForThisPageInLoop in listOfUrls)
            {
                // Console.WriteLine($"THIS IS LOOP NUMBER: {loopCount} and THE URL IS {urlForThisPageInLoop}");
                // loopCount++;

                var htmlWebLoad = htmlWeb.Load (urlForThisPageInLoop);

                // TABLE BODY NODE COLLECTION --> HtmlAgilityPack.HtmlNodeCollection (COUNT --> 1)
                var tableBodyNodeCollection = htmlWebLoad.DocumentNode.SelectNodes (pathOfTableToScrape);

                foreach (var tableBodyNode in tableBodyNodeCollection)
                {
                    // this can be gotten from Chrome; right-click 'Inspect', view the html for the table, right click on any item(in this case a row) and select Copy > tableBodyXpath
                    string preForRows  = "//*[@id='LeaderBoard1_dg1_ctl00__";
                    string postForRows = "']";

                    // 52 for first page; 13 for last page
                    int tableBodyNodeChildCount = tableBodyNode.ChildNodes.Count ();
                    // tableBodyNodeChildCount.Intro ("count of child nodes / rows");
                    int adjustedCount = tableBodyNodeChildCount - 2;
                    // Console.WriteLine($"ADJUSTED COUNT: {adjustedCount}");

                    for (var i = 0; i <= adjustedCount - 1; i++)
                    {
                        // TR FOR EACH PLAYER --> //*[@id='LeaderBoard1_dg1_ctl00__11']
                        string tableRowForEachPlayer = $"{preForRows}{i}{postForRows}";

                        // NODE ROW FOR EACH PLAYER ---> HtmlAgilityPack.HtmlNodeCollection (COUNT --> 1)
                        var nodeRowForEachPlayer = tableBodyNode.SelectNodes (tableRowForEachPlayer);

                        foreach (var playerItem in nodeRowForEachPlayer)
                        {
                            // PLAYER ITEM CHILDREN COUNT --> counts the total number columns listed in the table
                            var playerItemChildrenCount = playerItem.ChildNodes.Count ();
                            // Console.WriteLine($"PLAYER ITEM CHILDREN COUNT: {playerItemChildrenCount}");

                            // e.g. --> 12Manny Machado- - -101440245066710.9 %12.5 %.252.310.311.384.563.393152-0.526.3-3.23.9
                            string prePathForData  = $"{tableRowForEachPlayer}/td[";
                            string postPathForData = "]";

                            List<string> playerItems = new List<string> ();

                            int numberOfColumns = playerItemChildrenCount - 2;
                            // int numberOfColumns = 57;
                            int keyCount = 1;

                            for (var j = 1; j <= numberOfColumns; j++)
                            {
                                // TD FOR EACH PLAYER ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[1]
                                string tdForEachPlayer = $"{prePathForData}{j}{postPathForData}";

                                // PLAYERS NODE COLLECTION ---> HtmlAgilityPack.HtmlNodeCollection (COUNT --> 1)
                                var playersNodeCollection = playerItem.SelectNodes (tdForEachPlayer);

                                // go this way if looking for player name or player team
                                if (j == 2 || j == 3)
                                {
                                    try
                                    {
                                        // Extensions.Spotlight("MARK A");
                                        string postPost = "/a";

                                        // NAME AND TEAM TABLE BODY X-PATHS --> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[2]/a
                                        string nameAndTeamTableBodyXpaths = $"{tdForEachPlayer}{postPost}";

                                        // NAME AND TEAM --> HtmlAgilityPack.HtmlNodeCollection (COUNT --> 1)
                                        var nameAndTeamNodeCollection = playerItem.SelectNodes (nameAndTeamTableBodyXpaths);

                                        foreach (var nameOrTeamNode in nameAndTeamNodeCollection)
                                        {
                                            // Extensions.Spotlight("MARK B");
                                            var nameOrTeamValue = nameOrTeamNode.InnerText;
                                            playerItems.Add (nameOrTeamValue);
                                            Console.WriteLine($"NAME OR TEAM: {nameOrTeamValue}");
                                        }
                                    }

                                    catch
                                    {
                                        // if the team is empty, the 'try' will not work; this happens in cases where a player has played for multiple teams within a season; It shows up as '---' in their table data
                                        Extensions.Spotlight ("NAME or TEAM is broken");
                                        string cellIsBlank = "";
                                        playerItems.Add (cellIsBlank);
                                    }
                                }

                                else
                                {
                                    // Extensions.Spotlight("MARK D");
                                    foreach (var statValueNode in playersNodeCollection)
                                    {
                                        // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
                                        var statValue = statValueNode.InnerText;
                                        playerItems.Add (statValue);
                                        // Console.WriteLine($"STAT: {statValue}");
                                    }
                                }
                                keyCount++;
                            }

                            // List<FGHitter> hitters = new List<FGHitter> ();

                            // FGHitter newFGHitter = new FGHitter
                            // {
                            //     FanGraphsName = playerItems[1],
                            //     FanGraphsTeam = playerItems[2],
                            //     GP            = playerItems[3],
                            //     PA            = playerItems[4],
                            //     HR            = playerItems[5],
                            //     R             = playerItems[6],
                            //     RBI           = playerItems[7],
                            //     SB            = playerItems[8],
                            //     BB_percent    = playerItems[9],
                            //     K_percent     = playerItems[10],
                            //     ISO           = playerItems[11],
                            //     BABIP         = playerItems[12],
                            //     AVG           = playerItems[13],
                            //     OBP           = playerItems[14],
                            //     SLG           = playerItems[15],
                            //     wOBA          = playerItems[16],
                            //     wRC_plus      = playerItems[17],
                            //     BsR           = playerItems[18],
                            //     Off           = playerItems[19],
                            //     Def           = playerItems[20],
                            //     WAR           = playerItems[21],
                            // };

                            // hitters.Add (newFGHitter);

                            // foreach (var hitter in hitters)
                            // {
                            //     Console.WriteLine (hitter.FanGraphsName);
                            //     Console.WriteLine (hitter.WAR);
                            // }
                        string listToString = playerItems.ToString();
                        Console.WriteLine($"PLAYER ITEMS STRING: {listToString}");
                        }

                    }
                }
            }
        }
    }
}