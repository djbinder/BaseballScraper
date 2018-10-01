using System;
using System.Linq;
using HtmlAgilityPack;

namespace BaseballScraper.Infrastructure
{
    #pragma warning disable CS0219
    public class HtmlScraper
    {
        public void ScrapeHtmlPage()
        {
            HtmlWeb htmlWeb = new HtmlWeb();

            string urlToScrape = "https://www.cbssports.com/fantasy/football/trends/added/all";
            var thisUrlsHtml = htmlWeb.Load(urlToScrape);
            Console.WriteLine(thisUrlsHtml);

            var doc = thisUrlsHtml;

            var dataPath = "//*[@id='layoutRailRight']/div[1]/table/tbody/tr[3]";
            var dataSelector = "#layoutRailRight > div.column1 > table > tbody > tr:nth-child(3)";

            var tableClass = "data compact";
            var trClass1 = "row1";
            var trClass2 = "row2";
            var trAlign = "right";
            var tdFirstCellAlign = "left";
            var tdAllOtherCellsAlign = "center";
            var playerNameLinkPath = "//*[@id='layoutRailRight']/div[1]/table/tbody/tr[3]/td[1]/a";
            var tableTitlePath = "//*[@id='layoutRailRight']/div[1]/table/tbody/tr[1]";
            var pathOfTableBodyToScrape = "//*[@id='layoutRailRight']/div[1]/table/tbody";

            var thisTablesBody = thisUrlsHtml.DocumentNode.SelectNodes(pathOfTableBodyToScrape);

            // int nodeCount = 0;
            // foreach(var node in htmlNodes)
            // {
            //     nodeCount++;
            // }

            // Console.WriteLine(nodeCount);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(urlToScrape);

            var value = htmlDoc.DocumentNode
                .SelectNodes("//td/a")
                .First()
                .Attributes["value"].Value;

            Console.WriteLine(value);

            var nodes = thisUrlsHtml.DocumentNode.Descendants("td")
                .Select(y => y.Descendants()
                .Where(x => x.Attributes["class"].Value == trClass1))
                .ToList();

            Console.WriteLine(nodes.Count());

            int rowCount = 0;

            foreach(var player in nodes)
            {
                rowCount++;

            }

            Console.WriteLine(rowCount);
        }
    }
}