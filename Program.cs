using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

using BaseballScraper.Client;
using BaseballScraper.Configuration;
using BaseballScraper.Controllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Mappers;
using BaseballScraper.Models;
using BaseballScraper.Scrapers;

using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;

using OAuth;

// using BaseballScraper.Web

using HtmlAgilityPack;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using Npoi.Mapper;

using RDotNet;


namespace BaseballScraper
{
    public class Program
    {

        private System.Data.DataTable table = new System.Data.DataTable ("ParentTable");

        private static String _start = "STARTED";
        private static String _complete = "COMPLETED";
        public static string Start { get => _start; set => _start = value; }
        public static string Complete { get => _complete; set => _complete = value; }
        private const string RequestUrl = "https://api.login.yahoo.com/oauth/v2/get_request_token";
        private const string UserAuthorizeUrl = "https://api.login.yahoo.com/oauth/v2/request_auth";
        private const string AccessUrl = "https://api.login.yahoo.com/oauth/v2/get_token";
        private const string ConsumerKey = "[dj0yJmk9ckl2Z0V6YTdpaDZtJmQ9WVdrOVJGbEpaMUZ1TmpRbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD1hMw--";
        private const string ClientId = "dj0yJmk9ckl2Z0V6YTdpaDZtJmQ9WVdrOVJGbEpaMUZ1TmpRbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD1hMw--";

        private const string HmacSecert = "[66b4934e332c8100df465c4531b53b9f353d9955";
        private const string ClientSecrent = "66b4934e332c8100df465c4531b53b9f353d9955";
        private static string _authenticateUrl = "https://api.login.yahoo.com/oauth/v2/request_auth?oauth_token=";
        private const string ApiUrl = "http://fantasysports.yahooapis.com/fantasy/v2/";



        public static void Main (string[] args)
        {
            Console.WriteLine ($"Version: {Environment.Version}");


            // DataTabler dt = new DataTabler();
            // dt.PublishTable();



            // table.Dig();

            // var NPOIMapper = new NPOIMapper ();

            // int HTMLScrapeOn = 0;
            // if (HTMLScrapeOn == 1)
            // {
            //     var HTMLScrape = new HtmlAgilityPackScraper ();
            //     List<string> ListOfUrls = HtmlAgilityPackScraper.Get_Urls_OfPages_ToScrape ().ToList ();
            //     // var Count_OfUrls = ListOfUrls.Count ();
            //     // Count_OfUrls.Intro ("url count");
            //     // ListOfUrls.Dig ();
            //     HtmlAgilityPackScraper.HitterCrawler ();
            // }

            CreateWebHostBuilder (args).Build ().Run ();
        }

        public static IWebHostBuilder CreateWebHostBuilder (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup> ();
    }
}
//
//
//
//
//

//

//

//
// this was all moved to HtmlAgilityPackScraper --> it worked in main before the move
// public static void HitterCrawler (string url)
// {
//     string TableBodyXPath = "//*[@id='LeaderBoard1_dg1_ctl00']/tbody";

//     HtmlWeb htmlWeb = new HtmlWeb ();

//     //HTML WEB 1 ---> HtmlAgilityPack.HtmlDocument
//     var htmlWeb1 = htmlWeb.Load (url);

//     // TABLE BODY ---> HtmlAgilityPack.HtmlNodeCollection
//     var TableBody = htmlWeb1.DocumentNode.SelectNodes (TableBodyXPath);

//     foreach (var TableBody_Node in TableBody)
//     {
//         // this can be gotten from Chrome; right-click 'Inspect', view the html for the table, right click on any item(in this case a row) and select Copy > XPath
//         string pre_ForRows = "//*[@id='LeaderBoard1_dg1_ctl00__";
//         string post_ForRows = "']";

//         // int NumberOfPlayersListed = 1;

//         for (var i = 0; i <= 29; i++)
//         {
//             // TR FOR EACH PLAYER ---> //*[@id='LeaderBoard1_dg1_ctl00__11']
//             string tr_ForEachPlayer = $"{pre_ForRows}{i}{post_ForRows}";

//             // COUNT --> 1
//             // TABLE BODY NODE FOR EACH PLAYER ---> HtmlAgilityPack.HtmlNodeCollection
//             var TableBody_Node_Row_EachPlayer = TableBody_Node.SelectNodes (tr_ForEachPlayer);

//             foreach (var Player_Item in TableBody_Node_Row_EachPlayer)
//             {
//                 // COUNT --> 24
//                 var Player_Item_Children_Count = Player_Item.ChildNodes.Count ();

//                 //  e.g. --->   12Manny Machado- - -101440245066710.9 %12.5 %.252.310.311.384.563.393152-0.526.3-3.23.9
//                 // Player_Item.InnerText.Intro ("inner text");
//                 string pre_ForData = $"{tr_ForEachPlayer}/td[";
//                 string post_ForData = "]";

//                 List<string> PlayerItems = new List<string> ();

//                 int NumberOfColumns = 22;
//                 int KeyCount = 1;

//                 for (var j = 1; j <= NumberOfColumns; j++)
//                 {
//                     // TD FOR EACH PLAYER ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[1]
//                     string td_ForEachPlayer = $"{pre_ForData}{j}{post_ForData}";

//                     // COUNT --> 1
//                     // PLAYERS NODE COLLECTION ---> HtmlAgilityPack.HtmlNodeCollection
//                     var PlayersNodeCollection = Player_Item.SelectNodes (td_ForEachPlayer);

//                     // go this way if looking for player name or player team
//                     if (j == 2 || j == 3)
//                     {
//                         try
//                         {
//                             string post_post = "/a";

//                             // NAME AND TEAM X-PATHS ---> //*[@id='LeaderBoard1_dg1_ctl00__11']/td[2]/a
//                             string NameAndTeamXPaths = $"{td_ForEachPlayer}{post_post}";

//                             // COUNT --> 1
//                             // NAME AND TEAM ---> HtmlAgilityPack.HtmlNodeCollection
//                             var NameAndTeam = Player_Item.SelectNodes (NameAndTeamXPaths);

//                             foreach (var ActualNumber in NameAndTeam)
//                             {
//                                 // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
//                                 var NumberToAddToList = ActualNumber.InnerText;
//                                 PlayerItems.Add (NumberToAddToList);
//                             }
//                         }

//                         catch
//                         {
//                             string CellIsBlank = "";
//                             PlayerItems.Add (CellIsBlank);
//                             Extensions.Spotlight ("NAME or TEAM is broken");
//                         }
//                     }

//                     else
//                     {
//                         foreach (var ActualNumber in PlayersNodeCollection)
//                         {
//                             // e.g. '3.9', '26.3' etc. The players actual numbers for each stats
//                             var NumberToAddToList = ActualNumber.InnerText;
//                             PlayerItems.Add (NumberToAddToList);
//                         }
//                     }

//                     KeyCount++;
//                 }

//                 List<FGHitter> Hitters = new List<FGHitter> ();

//                 FGHitter NewFGHitter = new FGHitter
//                 {

//                     Name = PlayerItems[1],
//                     Team = PlayerItems[2],
//                     GP = PlayerItems[3],
//                     PA = PlayerItems[4],
//                     HR = PlayerItems[5],
//                     R = PlayerItems[6],
//                     RBI = PlayerItems[7],
//                     SB = PlayerItems[8],
//                     BB_percent = PlayerItems[9],
//                     K_percent = PlayerItems[10],
//                     ISO = PlayerItems[11],
//                     BABIP = PlayerItems[12],
//                     AVG = PlayerItems[13],
//                     OBP = PlayerItems[14],
//                     SLG = PlayerItems[15],
//                     wOBA = PlayerItems[16],
//                     wRC_plus = PlayerItems[17],
//                     BsR = PlayerItems[18],
//                     Off = PlayerItems[19],
//                     Def = PlayerItems[20],
//                     WAR = PlayerItems[21],
//                 };

//                 Hitters.Add (NewFGHitter);

//                 foreach (var hitter in Hitters)
//                 {
//                     Console.WriteLine (hitter.Name);
//                     Console.WriteLine (hitter.WAR);
//                 }
//             }
//         }

//     }

// }
