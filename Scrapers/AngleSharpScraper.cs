using System;
using System.Data;
using System.Linq;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Scrapers
{
    public class AngleSharpScraper
    {
        private String _start    = "STARTED";
        private String _complete = "COMPLETED";

        public string Start {get => _start; set => _start = value;}

        public string Complete { get => _complete; set => _complete = value; }


        public void MainAngleScrape (string String)
        {
            Start.ThisMethod ();

            // PARSER ---> AngleSharp.Parser.Html.HtmlParser
            var parser = new HtmlParser ();

            // DOCUMENT ---> AngleSharp.Dom.Html.HtmlDocument
            var document = parser.Parse (String);

            // #region SHOW ITEMS PATH
            bool ShowItemsPath = false;
            if (ShowItemsPath == true)
            {
                foreach (var item in document.All)
                {
                    // ITEM ---> AngleSharp.Dom.Html.HtmlBodyElement
                    item.Intro ("item");
                }
                //Do something with document like the following
                Console.WriteLine ("Serializing the (original) document:");
                // <html><head></head><body><h1>Some example source</h1><p>This is a paragraph element</p></body></html>
                Console.WriteLine (document.DocumentElement.OuterHtml);
                Console.WriteLine ();

                var p             = document.CreateElement ("p");
                    p.TextContent = "This is another paragraph.";

                Console.WriteLine ("Inserting another element in the body ...");
                document.Body.AppendChild (p);
                Console.WriteLine ();

                Console.WriteLine ("Serializing the document again:");
                // <html><head></head><body><h1>Some example source</h1><p>This is a paragraph element</p><p>This is another paragraph.</p></body></html>
                Console.WriteLine (document.DocumentElement.OuterHtml);
                Console.WriteLine ();
            }
            // #endregion

            // #region SHOW ITEMS OPTIONS
            bool ShowItemsOptions = false;
            if (ShowItemsOptions == true)
            {
                // DOCUMENT 2 ---> AngleSharp.Dom.Html.HtmlDocument
                //Generate HTML DOM for the following source code
                var document2 = parser.Parse (String);
                // var document2 = parser.Parse("<ul><li>First element<li>Second element<li>third<li class=bla>Last");
                // document2.Intro("document 2");

                // ELEMENTS ---> AngleSharp.Dom.Collections.HtmlCollection`1[AngleSharp.Dom.IElement]
                //Get all li elements and set the test attribute to the value test
                var elements = document2.QuerySelectorAll ("a").Attr ("test", "test");
                // elements.Intro("elements");

                int ElementsCount = elements.Count ();
                ElementsCount.Intro (ElementsCount.ToString ());

                bool ShowAllOptionsToSelect = true;
                // #region OPTIONS PRINT
                if (ShowAllOptionsToSelect == true)
                {
                    //elements still contains all li elements
                    foreach (var item in elements)
                    {
                        Extensions.Spotlight ("start listing new item options");

                        // ITEM ---> AngleSharp.Dom.Html.HtmlListItemElement
                        item.Intro ("item");
                        // Console.WriteLine();

                        // CHILD NODES ---> AngleSharp.Dom.Collections.NodeList
                        item.ChildNodes.Intro ("child nodes");
                        // Console.WriteLine();

                        // 1)
                        // 2)
                        // 3)
                        // 4) 'bla'
                        item.ClassList.Intro ("class list");
                        // Console.WriteLine();

                        // ATTRIBUTES ---> AngleSharp.Dom.Collections.NamedNodeMap
                        item.Attributes.Intro ("attributes");
                        // Console.WriteLine();

                        // 1) 'First Element'
                        // 2) 'Second Element'
                        // 3) 'third'
                        // 4) 'Last'
                        item.InnerHtml.Intro ("inner html");
                        // Console.WriteLine();

                        // all are blank
                        item.AssignedSlot.Intro ("assigned slot");

                        // all are blank
                        item.Id.Intro ("id");

                        // 1) 'true'
                        // 2) 'true'
                        // 3) 'true'
                        // 4) 'true'
                        item.HasChildNodes.Intro ("has child nodes");

                        // 1) false
                        // 2) false
                        // 3) false
                        // 4) false
                        item.IsFocused.Intro ("is focused");

                        // 1) AngleSharp.Dom.TextNode
                        // 2) AngleSharp.Dom.TextNode
                        // 3) AngleSharp.Dom.TextNode
                        // 4) AngleSharp.Dom.TextNode
                        item.LastChild.Intro ("last child");

                        // all are blank
                        item.LastElementChild.Intro ("last element child");

                        // 1) li
                        // 2) li
                        // 3) li
                        // 4) li
                        item.LocalName.Intro ("local name");

                        // 1) http://www.w3.org/1999/xhtml
                        // 2) http://www.w3.org/1999/xhtml
                        // 3) http://www.w3.org/1999/xhtml
                        // 4) http://www.w3.org/1999/xhtml
                        item.NamespaceUri.Intro ("Name space uri");

                        // 1) AngleSharp.Dom.Html.HtmlListItemElement
                        // 2) AngleSharp.Dom.Html.HtmlListItemElement
                        // 3) AngleSharp.Dom.Html.HtmlListItemElement
                        // 4)
                        item.NextElementSibling.Intro ("next element sibling");

                        // 1) AngleSharp.Dom.Html.HtmlListItemElement
                        // 2) AngleSharp.Dom.Html.HtmlListItemElement
                        // 3) AngleSharp.Dom.Html.HtmlListItemElement
                        // 4)
                        item.NextSibling.Intro ("next sibling");

                        // 1) LI
                        // 2) LI
                        // 3) LI
                        // 4) LI
                        item.NodeName.Intro ("node name");

                        // 1) Element
                        // 2) Element
                        // 3) Element
                        // 4) Element
                        item.NodeType.Intro ("node type");

                        // all are blank
                        item.NodeValue.Intro ("node value");

                        // 1) <li test="test">First element</li>
                        // 2) <li test="test">Second element</li>
                        // 3) <li test="test">third</li>
                        // 4) <li class="bla" test="test">Last</li>
                        item.OuterHtml.Intro ("outer html");

                        // 1) AngleSharp.Dom.Html.HtmlDocument
                        // 2) AngleSharp.Dom.Html.HtmlDocument
                        // 3) AngleSharp.Dom.Html.HtmlDocument
                        // 4) AngleSharp.Dom.Html.HtmlDocument
                        item.Owner.Intro ("owner");

                        // 1) AngleSharp.Dom.Html.HtmlUnorderedListElement
                        // 2) AngleSharp.Dom.Html.HtmlUnorderedListElement
                        // 3) AngleSharp.Dom.Html.HtmlUnorderedListElement
                        // 4) AngleSharp.Dom.Html.HtmlUnorderedListElement
                        item.Parent.Intro ("parent");

                        // 1) AngleSharp.Dom.Html.HtmlUnorderedListElement
                        // 2) AngleSharp.Dom.Html.HtmlUnorderedListElement
                        // 3) AngleSharp.Dom.Html.HtmlUnorderedListElement
                        // 4) AngleSharp.Dom.Html.HtmlUnorderedListElement
                        // item.ParentElement.Intro("parent element");

                        // all are blank
                        item.Prefix.Intro ("pre fix");

                        // 1) AngleSharp.Dom.Html.HtmlListItemElement
                        // 2) AngleSharp.Dom.Html.HtmlListItemElement
                        // 3) AngleSharp.Dom.Html.HtmlListItemElement
                        // 4) AngleSharp.Dom.Html.HtmlListItemElement
                        item.PreviousElementSibling.Intro ("previous element sibling");

                        // 1) AngleSharp.Dom.Html.HtmlListItemElement
                        // 2) AngleSharp.Dom.Html.HtmlListItemElement
                        // 3) AngleSharp.Dom.Html.HtmlListItemElement
                        // 4) AngleSharp.Dom.Html.HtmlListItemElement
                        item.PreviousSibling.Intro ("previous sibling");

                        // all are blank
                        item.ShadowRoot.Intro ("shadow root");

                        // all are blank
                        item.Slot.Intro ("slot");

                        // all are blank
                        item.Style.Intro ("style");

                        // 1) LI
                        // 2) LI
                        // 3) LI
                        // 4) LI
                        item.TagName.Intro ("tag name");

                        // 1) 'First Element'
                        // 2) 'Second Element'
                        // 3) 'third'
                        // 4) 'Last'
                        item.TextContent.Intro ("text content");

                        Extensions.Spotlight ("END");
                        Console.WriteLine ();
                    }

                }
                // #endregion
            }
            // #endregion
        }

        public void CreateTable (string TableId)
        {
            DataTable FG_Hitter_Table = new DataTable ();

            string[] fgHeaders = { "#", "Name", "Team", "G", "PA", "HR", "R", "RBI", "SB", "BB%", "K%", "ISO", "BABIP", "AVG", "OBP", "SLG", "wOBA", "wRC+", "BsR", "Off", "Def", "WAR" };

            foreach (var header in fgHeaders)
            {
                FG_Hitter_Table.Columns.Add (header);
                FG_Hitter_Table.AcceptChanges ();
            }

            DataRow dr = null;

            Console.WriteLine (dr);
        }
    }
}

// this worked in main
// MAIN ANGLE ---> BaseballScraper.Scrapers.AngleSharpScraper
// var AngleScrape = new AngleSharpScraper ();

// bool AngleScrapeOn = false;
// if (AngleScrapeOn == true)
// {
//     string FGstring = "<td class='grid_line_regular'><a href='statss.aspx?playerid=13510&amp;position=3B'>Jose Ramirez</a></td>";
//     AngleScrape.MainAngleScrape (FGstring);
//}