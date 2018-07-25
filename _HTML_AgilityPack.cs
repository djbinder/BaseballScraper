using System;
using System.Collections.Generic;
using System.Linq;

using AngleSharp.Extensions;
using AngleSharp.Parser.Html;

using BaseballScraper.Models;

using HtmlAgilityPack;

namespace BaseballScraper.Scrapers
{
    public class _HTML_AgilityPack
    {
        private static String _start    = "STARTED";
        private static String _complete = "COMPLETED";

        public static String Start
        {
            get => _start;
            set => _start = value;
        }

        public static String Complete
        {
            get => _complete;
            set => _complete = value;
        }
        public _HTML_AgilityPack () { }

        public static string Get_XPath_H_Pages_ToScrape ()
        {
            Start.ThisMethod ();

            string XPath = "//*[@id='LeaderBoard1_dg1_ctl00']/thead/tr[1]/td/div/div[5]/strong[2]";

            Complete.ThisMethod ();
            return XPath;
        }

        public static string Set_Pages_ToScrape_InitialUrl ()
        {
            Start.ThisMethod ();

            string InitialUrl = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=1_50";

            Complete.ThisMethod ();
            return InitialUrl;
        }

        public static int Get_Num_OfPages_ToScrape ()
        {
            Start.ThisMethod ();
            Start.ThisMethod ();

            string Url_Where_Scraping_Begins = Set_Pages_ToScrape_InitialUrl ();

            HtmlWeb htmlWeb = new HtmlWeb ();

            //HTML WEB 1 ---> HtmlAgilityPack.HtmlDocument
            var htmlWeb1 = htmlWeb.Load (Url_Where_Scraping_Begins);

            var Num_Pages_ToScrape_XPath = Get_XPath_H_Pages_ToScrape ();

            // TABLE BODY ---> HtmlAgilityPack.HtmlNodeCollection
            var NumberOfHtmlElement = htmlWeb1.DocumentNode.SelectNodes (Num_Pages_ToScrape_XPath);

            string PagesToScrape_AsString = NumberOfHtmlElement[0].InnerText;
            int    PagesToScrape_AsInt32  = Convert.ToInt32 (PagesToScrape_AsString);

            PagesToScrape_AsInt32.Intro ("pages to scrape MARK-A");

            Complete.ThisMethod ();
            return PagesToScrape_AsInt32;
        }

        public static List<string> Get_Urls_OfPages_ToScrape ()
        {
            Start.ThisMethod ();

            int NumberOfPagesToScrape = Get_Num_OfPages_ToScrape ();
            NumberOfPagesToScrape.Intro ("pages to scrape MARK-B");

            List<string> UrlsOfPagesToScrape = new List<string> ();

            for (var i = 1; i <= NumberOfPagesToScrape; i++)
            {
                var FG_H_URL_PreBase = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=";

                var PageNumber = i;

                var FG_H_URL_PostBase = "_50";

                string URLtoScrape = $"{FG_H_URL_PreBase}{i}{FG_H_URL_PostBase}";
                // URLtoScrape.Intro ("url to scrape");

                UrlsOfPagesToScrape.Add (URLtoScrape);
            }

            Complete.ThisMethod ();
            return UrlsOfPagesToScrape;
        }

        public static void HitterCrawler (List<string> url)
        // public static void HitterCrawler (string url)
        {
            string TestURL = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=1_50";

            string TableBodyXPath = " //*[@id='LeaderBoard1_dg1_ctl00']/tbody";

            HtmlWeb htmlWeb = new HtmlWeb ();

            //HTML WEB 1 ---> HtmlAgilityPack.HtmlDocument
            var htmlWeb1 = htmlWeb.Load (TestURL);

            // TABLE BODY ---> HtmlAgilityPack.HtmlNodeCollection
            var TableBody = htmlWeb1.DocumentNode.SelectNodes (TableBodyXPath);

            foreach (var TableBody_Node in TableBody)
            {
                // this can be gotten from Chrome; right-click 'Inspect', view the html for the table, right click on any item(in this case a row) and select Copy > XPath
                string pre_ForRows  = "//*[@id='LeaderBoard1_dg1_ctl00__";
                string post_ForRows = "']";

                // int NumberOfPlayersListed = 1;

                for (var i = 0; i <= 29; i++)
                {
                    // TR FOR EACH PLAYER ---> //*[@id='LeaderBoard1_dg1_ctl00__11']
                    string tr_ForEachPlayer = $"{pre_ForRows}{i}{post_ForRows}";

                    // COUNT --> 1
                    // TABLE BODY NODE FOR EACH PLAYER ---> HtmlAgilityPack.HtmlNodeCollection
                    var TableBody_Node_Row_EachPlayer = TableBody_Node.SelectNodes (tr_ForEachPlayer);

                    foreach (var Player_Item in TableBody_Node_Row_EachPlayer)
                    {
                        // COUNT --> 24
                        var Player_Item_Children_Count = Player_Item.ChildNodes.Count ();

                        //  e.g. --->   12Manny Machado- - -101440245066710.9 %12.5 %.252.310.311.384.563.393152-0.526.3-3.23.9
                        // Player_Item.InnerText.Intro ("inner text");
                        string pre_ForData  = $"{tr_ForEachPlayer}/td[";
                        string post_ForData = "]";

                        List<string> PlayerItems = new List<string> ();

                        int NumberOfColumns = 22;
                        int KeyCount        = 1;

                        for (var j = 1; j <= NumberOfColumns; j++)
                        {
                            // TD FOR EACH PLAYER ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[1]
                            string td_ForEachPlayer = $"{pre_ForData}{j}{post_ForData}";

                            // COUNT --> 1
                            // PLAYERS NODE COLLECTION ---> HtmlAgilityPack.HtmlNodeCollection
                            var PlayersNodeCollection = Player_Item.SelectNodes (td_ForEachPlayer);

                            // go this way if looking for player name or player team
                            if (j == 2 || j == 3)
                            {
                                try
                                {
                                    string post_post = "/a";

                                    // NAME AND TEAM X-PATHS ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[2]/a
                                    string NameAndTeamXPaths = $"{td_ForEachPlayer}{post_post}";

                                    // COUNT --> 1
                                    // NAME AND TEAM ---> HtmlAgilityPack.HtmlNodeCollection
                                    var NameAndTeam = Player_Item.SelectNodes (NameAndTeamXPaths);

                                    foreach (var ActualNumber in NameAndTeam)
                                    {
                                        // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
                                        var NumberToAddToList = ActualNumber.InnerText;
                                        PlayerItems.Add (NumberToAddToList);
                                    }
                                }

                                catch
                                {
                                    string CellIsBlank = "";
                                    PlayerItems.Add (CellIsBlank);
                                    Extensions.Spotlight ("NAME or TEAM is broken");
                                }
                            }

                            else
                            {
                                foreach (var ActualNumber in PlayersNodeCollection)
                                {
                                    // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
                                    var NumberToAddToList = ActualNumber.InnerText;
                                    PlayerItems.Add (NumberToAddToList);
                                }
                            }

                            KeyCount++;
                        }

                        List<FGHitter> Hitters = new List<FGHitter> ();

                        FGHitter NewFGHitter = new FGHitter
                        {

                            Name       = PlayerItems[1],
                            Team       = PlayerItems[2],
                            GP         = PlayerItems[3],
                            PA         = PlayerItems[4],
                            HR         = PlayerItems[5],
                            R          = PlayerItems[6],
                            RBI        = PlayerItems[7],
                            SB         = PlayerItems[8],
                            BB_percent = PlayerItems[9],
                            K_percent  = PlayerItems[10],
                            ISO        = PlayerItems[11],
                            BABIP      = PlayerItems[12],
                            AVG        = PlayerItems[13],
                            OBP        = PlayerItems[14],
                            SLG        = PlayerItems[15],
                            wOBA       = PlayerItems[16],
                            wRC_plus   = PlayerItems[17],
                            BsR        = PlayerItems[18],
                            Off        = PlayerItems[19],
                            Def        = PlayerItems[20],
                            WAR        = PlayerItems[21],
                        };

                        Hitters.Add (NewFGHitter);

                        foreach (var hitter in Hitters)
                        {
                            Console.WriteLine (hitter.Name);
                            Console.WriteLine (hitter.WAR);
                        }
                    }
                }

            }

        }

        public void MainHTML_scrape (string url, string node)
        {
            Start.ThisMethod ();

            HtmlAgilityPack.HtmlWeb      web = new HtmlAgilityPack.HtmlWeb ();
            HtmlAgilityPack.HtmlDocument doc = web.Load (url);

            // HEADER NAMES [@ Line#: 35] ---> System.Collections.Generic.List`1[HtmlAgilityPack.HtmlNode]
            // COUNT --> 1
            var HeaderNames = doc.DocumentNode
                .SelectNodes (node).ToList ();

            foreach (var item in HeaderNames)
            {
                Console.WriteLine (item.InnerText);
            }
        }

        public void HTML_from_String (string html)
        {
            Start.ThisMethod ();

            var htmlDoc = new HtmlDocument ();
            htmlDoc.LoadHtml (html);

            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode ("//body");

            Console.WriteLine (htmlBody.OuterHtml);
        }

        public void HTML_from_Web (string url)
        // public void HTML_from_Web (string url, string XPath)
        {
            Start.ThisMethod ();
            HtmlWeb web = new HtmlWeb ();

            // var htmlDoc = web.Load(url);
            HtmlDocument htmlDoc = web.Load (url);

            string Lindor = "//*[@id='LeaderBoard1_dg1_ctl00__0']";

            var node = htmlDoc.DocumentNode.SelectSingleNode ("//body/form/div/div/span/div/table/thead/tr/th");

            // NODE STRING [@ Line#: 77] ---> HtmlAgilityPack.HtmlNode
            var nodeString = node.ToString ();
            nodeString.Intro ("node string");

            var parser = new HtmlParser ();
            var doc2   = parser.Parse (nodeString);

            var elements      = doc2.QuerySelectorAll ("a");
            int elementsCount = elements.Count ();
            elementsCount.Intro (elementsCount.ToString ());

            foreach (var subnode in node.Descendants ())
            {
                subnode.Intro ("subnode");
                subnode.Name.Intro ("name");
                subnode.InnerHtml.Intro ("inner html");
                subnode.OuterHtml.Intro ("out html");
            }

            int    StartingPoint = 0;
            int    DataItem      = 0;
            string PlayerData    = $"*[@id='LeaderBoard1_dg1_ctl00__{StartingPoint}']/td[{DataItem}]";

            var TableHTML = htmlDoc.DocumentNode.SelectSingleNode (PlayerData);

            // for(var i = 0; i <=30; i++)
            // {
            //     var Data = $"*[@id='LeaderBoard1_dg1_ctl00__{i}']/td[{DataItem}]";
            //     Data.Intro("data");

            //     var DataHTML = htmlDoc.DocumentNode.SelectSingleNode(Data);
            //     DataHTML.Intro("data html");
            // }

            var TableHTMLX = htmlDoc.DocumentNode.SelectSingleNode (Lindor);
            foreach (var tableNodes in TableHTMLX.DescendantsAndSelf ())
            {
                Console.WriteLine (tableNodes.InnerHtml);
            }

            // Console.WriteLine("Node Name: " + node.Name + "\n" + node.InnerHtml);
        }

        public void LoadHTML_FromFile (string FilePath)
        {
            Start.ThisMethod ();

            // example of 'FilePath'
            // var path = @"test.html";

            var doc = new HtmlDocument ();
            doc.Load (FilePath);

            var node = doc.DocumentNode.SelectSingleNode ("//body");

            Console.WriteLine (node.OuterHtml);
        }

        // public static async Task HitterCrawler (string url)
        // {
        //     var URLtoScrape = url;
        //     var httpclient = new HttpClient();
        //     var html = await httpclient.GetStringAsync(URLtoScrape);

        //     var htmldocument = new HtmlDocument();
        //     htmldocument.LoadHtml(html);

        //     var HitterTable = htmldocument.DocumentNode.Descendants("table")
        //         .Where(x => x.GetAttributeValue("id", "")
        //         .Equals("LeaderBoard1_dg1_ctl00"))
        //         .ToList();

        //     // var ListHitter = new List<FanGraphsHitter>();

        //     foreach(var table in HitterTable)
        //     {
        //         var xA = table.Descendants("tr").FirstOrDefault().InnerText;
        //         xA.Intro("x a");
        //     }
        // }

    }
}

// this made it work in Main[]

// string Sterbenz = "https://www.yellowpages.com/search?search_terms=Sterbenz&geo_location_terms=Chicago%2C+IL";

// string DoctorsPage2 = "https://www.yellowpages.com/search?search_terms=doctor&geo_location_terms=Chicago%2C%20IL&page=2";

// string businessNode = "//a[@class='business-name']";

// // HTML SCRAPE  ---> BaseballScraper.Scrapers._HTML_AgilityPack
// var HTMLScrape = new _HTML_AgilityPack();
// bool HTMLScrapeOn = true;
// if(HTMLScrapeOn == true)
// {
//     // HTMLScrape.MainHTML_scrape(YellowPages, businessNode);
//     // HTMLScrape.MainHTML_scrape(Sterbenz, businessNode);
//     // HTMLScrape.MainHTML_scrape(DoctorsPage2, businessNode);
// }