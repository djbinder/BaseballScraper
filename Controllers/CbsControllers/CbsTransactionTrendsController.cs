﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Cbs;
using BaseballScraper.EndPoints;


#pragma warning disable CC0068, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006, MA0016
namespace BaseballScraper.Controllers.CbsControllers
{

    [Route("api/cbs/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CbsTransactionTrendsController : ControllerBase
    {
        private readonly Helpers         _helpers;
        private readonly CbsUriEndPoints _endPoints;

        public CbsTransactionTrendsController(Helpers helpers, CbsUriEndPoints endPoints)
        {
            _helpers   = helpers;
            _endPoints = endPoints;
        }

        public CbsTransactionTrendsController() {}


        // See: https://theathletic.com/533969/2018/09/19/exploring-the-hidden-fantasy-tools-on-yahoo-espn-and-cbs/



        // * = "https://www.cbssports.com/fantasy/baseball/trends/added/all";
        private string BaseballMostAddedEndPoint
        {
            get => _endPoints.BaseballMostAddedEndPoint().EndPointUri;
        }

        // * = "https://www.cbssports.com/fantasy/baseball/trends/dropped/all";
        private string BaseballMostDroppedEndPoint
        {
            get => _endPoints.BaseballMostDroppedEndPoint().EndPointUri;
        }

        // * = "https://www.cbssports.com/fantasy/baseball/trends/traded/all";
        private string BaseballMostTradedEndPoint
        {
            get => _endPoints.BaseballMostTradedEndPoint().EndPointUri;
        }

        // * = "https://www.cbssports.com/fantasy/baseball/trends/viewed/all";
        private string BaseballMosViewedEndPoint
        {
            get => _endPoints.BaseballMostViewedEndPoint().EndPointUri;
        }




        /*
            https://127.0.0.1:5001/api/cbs/cbstransactiontrends/test
        */
        [HttpGet("test")]
        public void CbsMostAdded()
        {
        }



        #region MOST ADDED OR DROPPED ------------------------------------------------------------

            // STATUS: May 7, 2019 --> this works
            /// <summary> Returns a list of the most added or dropped players according to Cbs trends; Does not filter by position </summary>
            /// <remarks>
            ///     CbsRankCurrentWeek - % of leagues that have the player on a roster this week
            ///     CbsRankPreviousWeek - % of leagues that have the player on a roster last week
            /// </remarks>
            /// <param name="urlToScrape"> The url of the Cbs roster trends; Should be either the url for most added or most dropped </param>
            /// <example> Most Added --> GetListOfCbsMostAddedOrDropped(urlForMostAddedAll); </example>
            /// <example> Most Dropped --> GetListOfCbsMostAddedOrDropped(urlForMostDroppedAll); </example>
            /// <returns> A list of most added or dropped players</returns>
            public List<CbsMostAddedOrDroppedPlayer> GetListOfCbsMostAddedOrDropped(string urlToScrape)
            {
                _helpers.StartMethod();

                HtmlWeb htmlWeb           = new HtmlWeb();
                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<CbsMostAddedOrDroppedPlayer> players = new List<CbsMostAddedOrDroppedPlayer>();

                const string tableBase = "//*[@id='TableBase']/div/div/table";

                foreach (HtmlNode table in thisUrlsHtml.DocumentNode.SelectNodes(tableBase))
                {
                    // tBody.Name == 'tbody'
                    HtmlNode tBody = table.ChildNodes[2];

                    HtmlNodeCollection tBodyRowNodes = tBody.ChildNodes;

                    // tBodyRowNodesCount == 100
                    // int tBodyRowNodesCount = tBodyRowNodes.Count;

                    int tBodyRowCount = 1;

                    // * There are 100 rows
                    foreach(HtmlNode row in tBody.SelectNodes("tr"))
                    {
                        CbsMostAddedOrDroppedPlayer player = new CbsMostAddedOrDroppedPlayer();

                        HtmlNodeCollection playerNamePositionTeamNode = row.ChildNodes[0].ChildNodes;

                        // * See comments at end of controller for example of the HTML
                        string playerNamePositionTeamHtmlInRow = row.ChildNodes[0].InnerHtml;

                        // * Player name team and position are in the same node to start
                        // * This goes through that node to get subnode for the player's name
                        foreach(HtmlNode node in playerNamePositionTeamNode)
                        {
                            int secondNodeCount = 1;
                            foreach(HtmlNode secondNode in node.ChildNodes)
                            {
                                if(secondNodeCount == 2)
                                {
                                    string playerName = secondNode.InnerText;
                                    player.CbsRosterTrendPlayerName = playerName;
                                }
                                secondNodeCount++;
                            }
                        }

                        player.CbsRankPreviousWeek = row.ChildNodes[2].InnerText.Trim();
                        player.CbsRankCurrentWeek  = row.ChildNodes[4].InnerText.Trim();

                        player.CbsDifferenceBetweenCurrentWeekAndPreviousWeek = row.ChildNodes[6].InnerText.Trim();

                        players.Add(player);
                        tBodyRowCount++;
                    }
                }
                PrintCbsAddedOrDroppedListOfPlayers(players);
                return players;
            }


            // STATUS: this works
            /// <summary> Returns a list of the most added or dropped players according to Cbs trends filtered by position </summary>
            /// <remarks>
            ///     CbsRankCurrentWeek - % of leagues that have the player on a roster this week
            ///     CbsRankPreviousWeek - % of leagues that have the player on a roster last week
            /// </remarks>
            /// <param name="urlToScrapePrefix"> The url of the Cbs roster trends; Should be either the url for most added or most dropped </param>
            /// <param name="position"> Position type: 1B, 2B, 3B, SS, OF, C, DH, SP, RP </param>
            /// <example> Most Added --> GetListOfCbsMostAddedOrDroppedByPosition(urlForMostAddedByPositionPrefix,"1B"); </example>
            /// <example> Most Dropped --> GetListOfCbsMostAddedOrDroppedByPosition(urlForMostDroppedByPositionPrefix,"1B"); </example>
            /// <returns> A list of most added or dropped players for one position </returns>
            public List<CbsMostAddedOrDroppedPlayer> GetListOfCbsMostAddedOrDroppedByPosition(string urlToScrapePrefix, string position)
            {
                _helpers.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                string urlToScrape = $"{urlToScrapePrefix}/{position}";
                Console.WriteLine(urlToScrape);

                // THIS URLS HTML --> HtmlAgilityPack.HtmlDocument
                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<CbsMostAddedOrDroppedPlayer> players = new List<CbsMostAddedOrDroppedPlayer>();

                foreach(HtmlNode table in thisUrlsHtml.DocumentNode.SelectNodes("//table"))
                {
                    foreach(HtmlNode row in table.SelectNodes("tr"))
                    {
                        CbsMostAddedOrDroppedPlayer player = new CbsMostAddedOrDroppedPlayer();

                        int cellNumber = 1;
                        foreach(HtmlNode cell in row.SelectNodes("td"))
                        {
                            if(cellNumber == 1)
                                player.CbsRosterTrendPlayerName = cell.InnerText;
                            if(cellNumber == 2)
                                player.CbsRankPreviousWeek = cell.InnerText;
                            if(cellNumber == 3)
                                player.CbsRankCurrentWeek = cell.InnerText;
                            if(cellNumber == 4)
                                player.CbsDifferenceBetweenCurrentWeekAndPreviousWeek = cell.InnerText;

                            cellNumber++;
                        }
                        players.Add(player);
                    }
                }
                PrintCbsAddedOrDroppedListOfPlayers(players);
                return players;
            }


        #endregion MOST ADDED OR DROPPED ------------------------------------------------------------





        #region MOST VIEWED ------------------------------------------------------------


            // STATUS: this works
            /// <summary> Returns a list of the most viewed players according to Cbs trends; Does not filter by position </summary>
            /// <param name="urlToScrape"> The url of the Cbs roster trends for most viewed </param>
            /// <example> GetListOfCbsMostViewedPlayers(urlForMostViewedAll); </example>
            /// <returns> A list of most viewed players </returns>
            public List<CbsMostViewedPlayer> GetListOfCbsMostViewedPlayers(string urlToScrape)
            {
                _helpers.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<CbsMostViewedPlayer> listOfViewedPlayers = new List<CbsMostViewedPlayer>();

                foreach(HtmlNode table in thisUrlsHtml.DocumentNode.SelectNodes("//table"))
                {
                    foreach(HtmlNode row in table.SelectNodes("tr"))
                    {
                        CbsMostViewedPlayer player = new CbsMostViewedPlayer();

                        int cellNumber = 1;
                        foreach(HtmlNode cell in row.SelectNodes("td"))
                        {
                            if(cellNumber == 1)
                                player.CbsRosterTrendPlayerName = cell.InnerText;
                            if(cellNumber == 2)
                                player.CbsRecentViews = cell.InnerText;
                            if(cellNumber == 3)
                                player.CbsTodaysViews = cell.InnerText;

                            cellNumber++;
                        }
                        listOfViewedPlayers.Add(player);
                    }
                }
                // PrintListOfMostViewedPlayers(listOfViewedPlayers);
                return listOfViewedPlayers;
            }


            // STATUS: this works
            /// <summary> Returns a list of the most viewed players according to Cbs trends for one position </summary>
            /// <param name="urlToScrapePrefix"> The url of the Cbs roster trends for most viewed </param>
            /// <param name="position"> Position type: 1B, 2B, 3B, SS, OF, C, DH, SP, RP </param>
            /// <example> GetListOfCbsMostViewedPlayersByPosition(urlForMostViewedByPositionPrefix, "1B"); </example>
            /// <returns> A list of most viewed players for one position </returns>
            public List<CbsMostViewedPlayer> GetListOfCbsMostViewedPlayersByPosition(string urlToScrapePrefix, string position)
            {
                _helpers.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                string urlToScrape = $"{urlToScrapePrefix}/{position}";
                Console.WriteLine(urlToScrape);

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<CbsMostViewedPlayer> listOfViewedPlayers = new List<CbsMostViewedPlayer>();

                foreach(HtmlNode table in thisUrlsHtml.DocumentNode.SelectNodes("//table"))
                {
                    foreach(HtmlNode row in table.SelectNodes("tr"))
                    {
                        CbsMostViewedPlayer player = new CbsMostViewedPlayer();

                        int cellNumber = 1;
                        foreach(HtmlNode cell in row.SelectNodes("td"))
                        {
                            if(cellNumber == 1)
                                player.CbsRosterTrendPlayerName = cell.InnerText;
                            if(cellNumber == 2)
                                player.CbsRecentViews = cell.InnerText;
                            if(cellNumber == 3)
                                player.CbsTodaysViews = cell.InnerText;

                            cellNumber++;
                        }
                        listOfViewedPlayers.Add(player);
                    }
                }
                // PrintListOfMostViewedPlayers(listOfViewedPlayers);
                return listOfViewedPlayers;
            }


        #endregion MOST VIEWED ------------------------------------------------------------





        #region MOST TRADED ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Returns a list of the most traded players according to Cbs trends; Does not filter by position </summary>
            /// <param name="urlToScrape"> The url of the Cbs roster trends for most traded </param>
            /// <example> GetListOfCbsMostTradedPlayers(urlForMostTradedAll); </example>
            /// <returns> A list of most traded players </returns>
            public List<CbsMostTradedPlayer> GetListOfCbsMostTradedPlayers(string urlToScrape)
            {
                _helpers.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<CbsMostTradedPlayer> listOfTradedPlayers = new List<CbsMostTradedPlayer>();

                foreach(HtmlNode table in thisUrlsHtml.DocumentNode.SelectNodes("//table"))
                {
                    foreach(HtmlNode row in table.SelectNodes("tr"))
                    {
                        CbsMostTradedPlayer player = new CbsMostTradedPlayer();

                        int cellNumber = 1;
                        foreach(HtmlNode cell in row.SelectNodes("td"))
                        {
                            if(cellNumber == 1)
                                player.CbsRosterTrendPlayerName = cell.InnerText;
                            if(cellNumber == 2)
                                player.CbsNumberOfTrades = cell.InnerText;

                            cellNumber++;
                        }
                        listOfTradedPlayers.Add(player);
                    }
                }
                PrintListOfMostTradedPlayers(listOfTradedPlayers);
                return listOfTradedPlayers;
            }


            // STATUS: this works
            /// <summary> Returns a list of the most traded players according to Cbs trends for one position </summary>
            /// <param name="urlToScrapePrefix"> The url of the Cbs roster trends for most traded </param>
            /// <param name="position"> Position type: 1B, 2B, 3B, SS, OF, C, DH, SP, RP </param>
            /// <example> GetListOfCbsMostTradedPlayersByPosition(urlForMostTradedByPositionPrefix, "1B"); </example>
            /// <returns> A list of most traded players for one position </returns>
            public List<CbsMostTradedPlayer> GetListOfCbsMostTradedPlayersByPosition(string urlToScrapePrefix, string position)
            {
                _helpers.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                string urlToScrape = $"{urlToScrapePrefix}/{position}";
                Console.WriteLine(urlToScrape);

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<CbsMostTradedPlayer> listOfTradedPlayers = new List<CbsMostTradedPlayer>();

                foreach(HtmlNode table in thisUrlsHtml.DocumentNode.SelectNodes("//table"))
                {
                    foreach(HtmlNode row in table.SelectNodes("tr"))
                    {
                        CbsMostTradedPlayer player = new CbsMostTradedPlayer();

                        int cellNumber = 1;
                        foreach(HtmlNode cell in row.SelectNodes("td"))
                        {
                            if(cellNumber == 1)
                                player.CbsRosterTrendPlayerName = cell.InnerText;
                            if(cellNumber == 2)
                                player.CbsNumberOfTrades = cell.InnerText;

                            cellNumber++;
                        }
                        listOfTradedPlayers.Add(player);
                    }
                }
                // PrintListOfMostTradedPlayers(listOfTradedPlayers);
                return listOfTradedPlayers;
            }

        #endregion MOST TRADED ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------


            private void PrintCbsAddedOrDroppedListOfPlayers(List<CbsMostAddedOrDroppedPlayer> players)
            {
                int playerNumber = 1;
                foreach(CbsMostAddedOrDroppedPlayer p in players)
                {
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine($"Player Number: {playerNumber}");
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine(p.CbsRosterTrendPlayerName);
                    Console.WriteLine(p.CbsRankPreviousWeek);
                    Console.WriteLine(p.CbsRankCurrentWeek);
                    Console.WriteLine(p.CbsDifferenceBetweenCurrentWeekAndPreviousWeek);
                    Console.WriteLine();
                    playerNumber++;
                }
            }

            private void PrintListOfMostTradedPlayers(List<CbsMostTradedPlayer> listOfTradedPlayers)
            {
                foreach(CbsMostTradedPlayer player in listOfTradedPlayers)
                {
                    Console.WriteLine(player.CbsRosterTrendPlayerName);
                    Console.WriteLine(player.CbsNumberOfTrades);
                    Console.WriteLine();
                }
            }


            private void PrintListOfMostViewedPlayers(List<CbsMostViewedPlayer> listOfViewedPlayers)
            {
                foreach(CbsMostViewedPlayer player in listOfViewedPlayers)
                {
                    Console.WriteLine(player.CbsRosterTrendPlayerName);
                    Console.WriteLine(player.CbsRecentViews);
                    Console.WriteLine(player.CbsTodaysViews);
                    Console.WriteLine();
                }
            }


        #endregion PRINTING PRESS ------------------------------------------------------------

    }
}





// EXAMPLE HTML
// html for player's full name, player's abbreviated name, position, and team
    // [1] html structure - abbreviated name
        // <span class="CellPlayerName--short">
            // <a href = "/mlb/players/playerpage/1757975/jake-odorizzi" class="">J.Odorizzi</a>
            // <span class="CellPlayerName-position"> SP </span>
            // <span class="CellPlayerName-team">• MIN </span>
        // </span>
    // [2] html structure - full name
        // <span class="CellPlayerName--long">
            // <a href = "/mlb/players/playerpage/1757975/jake-odorizzi" class="">Jake Odorizzi</a>
            // <span class="CellPlayerName-position"> SP </span>
            // <span class="CellPlayerName-team">• MIN </span>
        // </span>






// * NOTE: these are needed as parameters when calling the various methods in the controller (i.e. DO NOT DELETE THEM)
// private const string urlForMostAddedAll = "https://www.cbssports.com/fantasy/baseball/trends/added/all";
// private const string urlForMostDroppedAll = "https://www.cbssports.com/fantasy/baseball/trends/dropped/all";
// private const string urlForMostAddedByPositionPrefix = "https://www.cbssports.com/fantasy/baseball/trends/added";
// private const string urlForMostDroppedByPositionPrefix = "https://www.cbssports.com/fantasy/baseball/trends/dropped";
// private const string urlForMostViewedAll = "https://www.cbssports.com/fantasy/baseball/trends/viewed/all";
// private const string urlForMostViewedByPositionPrefix = "https://www.cbssports.com/fantasy/baseball/trends/viewed";
// private const string urlForMostTradedAll = "https://www.cbssports.com/fantasy/baseball/trends/traded/all";
// private const string urlForMostTradedByPositionPrefix = "https://www.cbssports.com/fantasy/baseball/trends/traded";
// private const string urlForMostAddedAllFootball = "https://www.cbssports.com/fantasy/football/trends/added/all";
// private const string urlForMostDroppedAllFootball = "https://www.cbssports.com/fantasy/football/trends/dropped/all";

// private const string cbsUrlForMostAddedAllBaseball = "https://www.cbssports.com/fantasy/baseball/trends/added/all";
