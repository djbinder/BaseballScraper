using System;
using System.Collections.Generic;
using System.Linq;

using AngleSharp.Extensions;

using BaseballScraper.Models;

using HtmlAgilityPack;



namespace BaseballScraper.Scrapers
{
    public class HtmlAgilityPackScraper
    {
        private static String _start    = "STARTED";
        private static String _complete = "COMPLETED";

        public static string start { get => _start; set => _start = value; }
        public static string complete { get => _complete; set => _complete = value; }

        public HtmlAgilityPackScraper () { }


        // test comment string
        public static string GetXpathForNumOfPagesToScrape ()
        {
            // start.ThisMethod ();
            string tableBodyXpath = "//*[@id='LeaderBoard1_dg1_ctl00']/thead/tr[1]/td/div/div[5]/strong[2]";

            // complete.ThisMethod ();
            return tableBodyXpath;
        }

        public static string SetInitialUrlToScrape ()
        {
            // start.ThisMethod ();
            string initialUrl = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=1_50";

            // complete.ThisMethod ();
            return initialUrl;
        }

        public static int GetNumOfPagesToScrape ()
        {
            // start.ThisMethod ();
            string urlToBeginScraping = SetInitialUrlToScrape ();

            HtmlWeb htmlWeb = new HtmlWeb ();

            //HTML WEB 1 ---> HtmlAgilityPack.HtmlDocument
            var htmlWeb1 = htmlWeb.Load (urlToBeginScraping);

            // scrape fangraphs page to get number of pages
            var numPagesToScrape = GetXpathForNumOfPagesToScrape ();

            // TABLE BODY ---> HtmlAgilityPack.HtmlNodeCollection
            var numOfHtmlElement = htmlWeb1.DocumentNode.SelectNodes (numPagesToScrape);

            string numOfPagesToScrapeString = numOfHtmlElement[0].InnerText;
            int    numOfPagesToScrapeInt    = Convert.ToInt32 (numOfPagesToScrapeString);

            // complete.ThisMethod ();
            return numOfPagesToScrapeInt;
        }

        public static List<string> GetUrlsOfPagesToScrape ()
        {
            // start.ThisMethod ();
            int numOfPagesToScrape = GetNumOfPagesToScrape ();

            List<string> urlsOfPagesToScrape = new List<string> ();

            for (var i = 1; i <= numOfPagesToScrape; i++)
            {
                var baseOfUrlToScrape = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=";

                var pageNumber = i;

                var numOfRecordsListedOnPage = "_50";

                string urlToScrape = $"{baseOfUrlToScrape}{i}{numOfRecordsListedOnPage}";

                urlsOfPagesToScrape.Add (urlToScrape);
            }

            // complete.ThisMethod ();
            return urlsOfPagesToScrape;
        }

        public static string Get_XPathOf_TableBody_ToScrape ()
        {
            // start.ThisMethod ();
            string tableBodyXpath = "//*[@id='LeaderBoard1_dg1_ctl00']/tbody";

            // complete.ThisMethod ();
            return tableBodyXpath;
        }

        public static string Get_XPathOf_TableRow_ToScrape ()
        {
            // start.ThisMethod ();
            string preForRows    = "//*[@id='LeaderBoard1_dg1_ctl00__";
            string postForRows   = "']";
            int    i             = 0;
            string tableRowXpath = $"{preForRows}{i}{postForRows}";

            // complete.ThisMethod ();
            return tableRowXpath;
        }

        // public static int Get_Num_OfRows_ToScrape_OnPage ()
        // {
        //     start.ThisMethod ();

        //     HtmlNode TableBodyXPath = "//*[@id='LeaderBoard1_dg1_ctl00']/tbody";

        //     foreach (HtmlNode tr in TableBodyXPath.SelectNodes ("//tr"));
        // }

        public static void HitterCrawler ()
        // public static void HitterCrawler (string url)
        {
            start.ThisMethod ();
            List<string> listOfUrls = HtmlAgilityPackScraper.GetUrlsOfPagesToScrape ().ToList ();
            // var  numOfUrls       = listOfUrls.Count ();
            // numOfUrls.Intro ("url count");

            string tableBodyXpath = Get_XPathOf_TableBody_ToScrape ();

            HtmlWeb htmlWeb = new HtmlWeb ();

            int loopCount = 1;
            foreach (var urlForThisPageInLoop in listOfUrls)
            {
                // urlForThisPageInLoop.Intro ("single page url");
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
                            // var Player_Item_Children_Count = playerItem.ChildNodes.Count ();

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
                                        string postPost = "/a";

                                        // NAME AND TEAM X-PATHS ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[2]/a
                                        string nameAndTeamTableBodyXpaths = $"{tdForEachPlayer}{postPost}";

                                        // COUNT --> 1
                                        // NAME AND TEAM ---> HtmlAgilityPack.HtmlNodeCollection
                                        var nameAndTeam = playerItem.SelectNodes (nameAndTeamTableBodyXpaths);

                                        foreach (var actualNumber in nameAndTeam)
                                        {
                                            // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
                                            var numToAddToList = actualNumber.InnerText;
                                            playerItems.Add (numToAddToList);
                                        }
                                    }

                                    catch
                                    {
                                        string cellIsBlank = "";
                                        playerItems.Add (cellIsBlank);
                                        Extensions.Spotlight ("NAME or TEAM is broken");
                                    }
                                }

                                else
                                {
                                    foreach (var actualNumber in playersNodeCollection)
                                    {
                                        // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
                                        var numToAddToList = actualNumber.InnerText;
                                        playerItems.Add (numToAddToList);
                                    }
                                }

                                keyCount++;
                            }

                            List<FGHitter> hitters = new List<FGHitter> ();

                            FGHitter newFGHitter = new FGHitter
                            {
                                FanGraphsName = playerItems[1],
                                Team          = playerItems[2],
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

                            // foreach (var hitter in hitters)
                            // {
                            //     Console.WriteLine (hitter.Name);
                            //     Console.WriteLine (hitter.WAR);
                            // }

                        }
                    }

                }
            }

        }

        // public void MainHTML_scrape (string url, string node)
        // {
        //     start.ThisMethod ();

        //     HtmlAgilityPack.HtmlWeb      web = new HtmlAgilityPack.HtmlWeb ();
        //     HtmlAgilityPack.HtmlDocument doc = web.Load (url);

        //     // HEADER NAMES [@ Line#: 35] ---> System.Collections.Generic.List`1[HtmlAgilityPack.HtmlNode]
        //     // COUNT --> 1
        //     var HeaderNames = doc.DocumentNode
        //         .SelectNodes (node).ToList ();

        //     foreach (var item in HeaderNames)
        //     {
        //         Console.WriteLine (item.InnerText);
        //     }
        // }

        public void HTML_from_String (string html)
        {
            start.ThisMethod ();

            var htmlDoc = new HtmlDocument ();
            htmlDoc.LoadHtml (html);

            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode ("//body");

            Console.WriteLine (htmlBody.OuterHtml);
        }

        public void LoadHTML_FromFile (string FilePath)
        {
            start.ThisMethod ();

            // example of 'FilePath'
            // var path = @"test.html";

            var doc = new HtmlDocument ();
            doc.Load (FilePath);

            var node = doc.DocumentNode.SelectSingleNode ("//body");

            Console.WriteLine (node.OuterHtml);
        }

    }
}

// this made it work in Main[]

// string Sterbenz = "https://www.yellowpages.com/search?search_terms=Sterbenz&geo_location_terms=Chicago%2C+IL";

// string DoctorsPage2 = "https://www.yellowpages.com/search?search_terms=doctor&geo_location_terms=Chicago%2C%20IL&page=2";

// string businessNode = "//a[@class='business-name']";

// // HTML SCRAPE  ---> BaseballScraper.Scrapers.HtmlAgilityPack
// var HTMLScrape = new HtmlAgilityPack();
// bool HTMLScrapeOn = true;
// if(HTMLScrapeOn == true)
// {
//     // HTMLScrape.MainHTML_scrape(YellowPages, businessNode);
//     // HTMLScrape.MainHTML_scrape(Sterbenz, businessNode);
//     // HTMLScrape.MainHTML_scrape(DoctorsPage2, businessNode);
// }
