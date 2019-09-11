
using System.Collections.Generic;
using System.Linq;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.BrooksBaseball;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS0219, CS0414, CS1570, CS1572, CS1573, CS1584, CS1587, CS1591, CS1658, CS1998, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.BrooksBaseballControllers
{
    public class BrooksBaseballUtilitiesController : Controller
    {
        private readonly Helpers _helpers;

        public BrooksBaseballUtilitiesController(Helpers helpers)
        {
            _helpers = helpers;
        }

        // * Column Headers Are:
        // * > Pitch Type, Count, Freq, Velo (mph), pfx HMov (in.), pfx VMov (in.), H. Rel (ft.), V. Rel(ft.)

        // * Note: these are not specific for any Comparison Mode type;
        // * They work for all Comparison Mode
        // *    Comparison Types: 1) NoComparison 2) ZScore 3) PitchIQ 4) Scout


        // STATUS [ September 10, 2019 ] : this works
        // * endPoints will come from various brooks end points
        // * Works with Tabular Data > Trajectory and Movement Tables > all Comparison Mode Types
        public IEnumerable<HtmlNode> GetAllTableRows(string endPoint)
        {
            HtmlWeb htmlWeb = new HtmlWeb ();
            HtmlDocument htmlWeb1 = htmlWeb.Load (endPoint);

            // * The # of rows will vary for pitcher
            // * First row is headers, all additional rows are for each pitch the pitcher throws
            // * E.g., if pitcher throws 3 pitchers then allTableRows count = 4
            IEnumerable<HtmlNode> allTableRows = from table in htmlWeb1.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
                from head in table.SelectNodes("thead").Cast<HtmlNode>()
                from row in head.SelectNodes("tr").Cast<HtmlNode>()
                select row;

            // int tableRowsCount = allTableRows.Count();
            return allTableRows;
        }
    }
}
