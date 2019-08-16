// using System;
// using System.Collections.Generic;
// using System.Linq;
// using HtmlAgilityPack;

// #pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
// namespace BaseballScraper.Infrastructure
// {
//     public class HtmlScraper
//     {
//         // STATUS
//         public void ScrapeHtmlPage(string urlToScrape, string pathOfTableBodyToScrape)
//         {
//             HtmlWeb htmlWeb = new HtmlWeb();

//             var thisUrlsHtml = htmlWeb.Load(urlToScrape);
//         }


//         public List<IEnumerable<HtmlNode>> GetAllTableRowsOnPage(HtmlDocument doc, string urlToScrape)
//         {
//             HtmlWeb htmlWeb = new HtmlWeb();
//             var thisUrlsHtml = htmlWeb.Load(urlToScrape);

//             // ----- ROWS -----
//                 // ALL TABLE ROW NODES IN TABLE
//                     // List<IEnumerable<HtmlNode>> allTableRowNodesInTable
//                     // System.Collections.Generic.List`1[System.Collections.Generic.IEnumerable`1[HtmlAgilityPack.HtmlNode]]
//                     // 'IterateForEach' item => HtmlAgilityPack.HtmlNode+<Descendants>d__125
//                 var allTableRowNodesInTable = thisUrlsHtml.DocumentNode.Descendants("tr")
//                     .Select(y => y.Descendants())
//                     .ToList();

//                 Console.WriteLine("----- ROWS -----");
//                 Console.WriteLine($"Count: {allTableRowNodesInTable.Count()}");
//                 Console.WriteLine($"Type: {allTableRowNodesInTable.GetType()}");
//                 Console.WriteLine();

//             return allTableRowNodesInTable;
//         }


//         public List<IEnumerable<HtmlNode>> GetAllTableCellsOnPage(HtmlDocument doc, string urlToScrape)
//         {
//             HtmlWeb htmlWeb = new HtmlWeb();
//             var thisUrlsHtml = htmlWeb.Load(urlToScrape);

//             // ----- CELLS -----
//                 // ALL TABLE DATA NODES IN TABLE
//                     // Count = 405
//                     // List<IEnumerable<HtmlNode>> allTableDataNodesInTable
//                     // System.Collections.Generic.List`1[System.Collections.Generic.IEnumerable`1[HtmlAgilityPack.HtmlNode]]
//                     // 'IterateForEach' item => HtmlAgilityPack.HtmlNode+<Descendants>d__125
//                 var allTableDataNodesInTable = thisUrlsHtml.DocumentNode.Descendants("td")
//                     .Select(y => y.Descendants())
//                     .ToList();

//                 Console.WriteLine("----- TABLE DATA -----");
//                 Console.WriteLine($"Count: {allTableDataNodesInTable.Count()}");
//                 Console.WriteLine($"Type: {allTableDataNodesInTable.GetType()}");
//                 Console.WriteLine();

//             return allTableDataNodesInTable;
//         }


//     }
// }
