﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Cbs;

namespace BaseballScraper.Controllers.CbsControllers
{

    #region OVERVIEW ------------------------------------------------------------

        /// <summary> Retrieve Cbs player trends: Most added, most dropped, most viewed, most traded </summary>
        /// <list> INDEX
        ///     <item> Get Most Added / Dropped All <see cref="CbsTransactionTrendsController.GetListOfCbsMostAddedOrDropped(string)" /> </item>
        ///     <item> Get Most Added / Dropped By Position <see cref="CbsTransactionTrendsController.GetListOfCbsMostAddedOrDroppedByPosition(string, string)"/></item>
        ///     <item> Print Most Added / Dropped <see cref="CbsTransactionTrendsController.PrintCbsAddedOrDroppedListOfPlayers(List{CbsMostAddedOrDroppedPlayer})" /> </item>
        ///     <item> Get Most Viewed All <see cref="CbsTransactionTrendsController.GetListOfCbsMostViewedPlayers(string)" /></item>
        ///     <item> Get Most Viewed <see cref="CbsTransactionTrendsController.GetListOfCbsMostViewedPlayersByPosition(string, string)"/></item>
        ///     <item> Get Most Traded All <see cref="CbsTransactionTrendsController.GetListOfCbsMostTradedPlayers(string)" /> </item>
        ///     <item> Get Most Traded By Position <see cref="CbsTransactionTrendsController.GetListOfCbsMostTradedPlayersByPosition(string, string)"/> </item>
        /// <list> RESOURCES
        ///     <item> Most Added: https://www.cbssports.com/fantasy/baseball/trends/added/all </item>
        ///     <item> Most Dropped: https://www.cbssports.com/fantasy/baseball/trends/dropped/all </item>
        ///     <item> Most Viewed: https://www.cbssports.com/fantasy/baseball/trends/viewed/all </item>
        ///     <item> Most Traded https://www.cbssports.com/fantasy/baseball/trends/traded/all </item>
        /// </list>

    #endregion OVERVIEW ------------------------------------------------------------



    [Route("cbs")]
    #pragma warning disable CS0219
    public class CbsTransactionTrendsController : Controller
    {
        private readonly Helpers _h = new Helpers();

        #region URL QUERY STRINGS ------------------------------------------------------------

            // NOTE: these are needed as parameters when calling the various methods in the controller (i.e. DO NOT DELETE THEM)
            private const string urlForMostAddedAll = "https://www.cbssports.com/fantasy/baseball/trends/added/all";
            private const string urlForMostDroppedAll = "https://www.cbssports.com/fantasy/baseball/trends/dropped/all";
            private const string urlForMostAddedByPositionPrefix = "https://www.cbssports.com/fantasy/baseball/trends/added";
            private const string urlForMostDroppedByPositionPrefix = "https://www.cbssports.com/fantasy/baseball/trends/dropped";
            private const string urlForMostViewedAll = "https://www.cbssports.com/fantasy/baseball/trends/viewed/all";
            private const string urlForMostViewedByPositionPrefix = "https://www.cbssports.com/fantasy/baseball/trends/viewed";
            private const string urlForMostTradedAll = "https://www.cbssports.com/fantasy/baseball/trends/traded/all";
            private const string urlForMostTradedByPositionPrefix = "https://www.cbssports.com/fantasy/baseball/trends/traded";
            private const string urlForMostAddedAllFootball = "https://www.cbssports.com/fantasy/football/trends/added/all";
            private const string urlForMostDroppedAllFootball = "https://www.cbssports.com/fantasy/football/trends/dropped/all";

        #endregion URL QUERY STRINGS ------------------------------------------------------------


        [HttpGet]
        [Route("trends")]
        public void CbsMostAdded()
        {
            _h.StartMethod();
        }


        #region MOST ADDED OR DROPPED ------------------------------------------------------------

            // STATUS: this works
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
                HtmlWeb htmlWeb = new HtmlWeb();

                // THIS URLS HTML --> HtmlAgilityPack.HtmlDocument
                var thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<CbsMostAddedOrDroppedPlayer> players = new List<CbsMostAddedOrDroppedPlayer>();

                foreach(HtmlNode table in thisUrlsHtml.DocumentNode.SelectNodes("//table"))
                {
                    int rowCount = 1;
                    foreach(HtmlNode row in table.SelectNodes("tr"))
                    {
                        CbsMostAddedOrDroppedPlayer player = new CbsMostAddedOrDroppedPlayer();

                        // the first two rows are table headers, not player data; So you don't want to get them.
                        if(rowCount > 2)
                        {
                            int cellNumber = 1;
                            foreach(HtmlNode cell in row.SelectNodes("td"))
                            {
                                // player name cell (ie cellNumber 1) comes back like -> Smallwood, Wendell RB PHI &nbsp;
                                // this code cleans that text up to return firstName lastName (e.g. Wendell Smallwood)
                                if(cellNumber == 1)
                                {
                                    string playerNameAndDetails = cell.InnerText;

                                    string[] playersLastName = playerNameAndDetails.Split(',');
                                    string[] playersFirstName = playersLastName[1].Trim().Split(' ');
                                    var playerName = $"{playersFirstName[0]} {playersLastName[0]}";

                                    player.CbsRosterTrendPlayerName = playerName;
                                }
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
                        rowCount++;
                    }
                }
                // PrintCbsAddedOrDroppedListOfPlayers(players);
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
                _h.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = $"{urlToScrapePrefix}/{position}";
                Console.WriteLine(urlToScrape);

                // THIS URLS HTML --> HtmlAgilityPack.HtmlDocument
                var thisUrlsHtml = htmlWeb.Load(urlToScrape);

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

            // STATUS: this works
            /// <summary> Print the lists from previous two methods </summary>
            /// <param name="players"> A list of added or dropped players</param>
            public void PrintCbsAddedOrDroppedListOfPlayers(List<CbsMostAddedOrDroppedPlayer> players)
            {
                int playerNumber = 1;
                foreach(var p in players)
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

        #endregion MOST ADDED OR DROPPED ------------------------------------------------------------



        #region MOST VIEWED ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Returns a list of the most viewed players according to Cbs trends; Does not filter by position </summary>
            /// <param name="urlToScrape"> The url of the Cbs roster trends for most viewed </param>
            /// <example> GetListOfCbsMostViewedPlayers(urlForMostViewedAll); </example>
            /// <returns> A list of most viewed players </returns>
            public List<CbsMostViewedPlayer> GetListOfCbsMostViewedPlayers(string urlToScrape)
            {
                _h.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                // THIS URLS HTML --> HtmlAgilityPack.HtmlDocument
                var thisUrlsHtml = htmlWeb.Load(urlToScrape);

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
                foreach(var player in listOfViewedPlayers)
                {
                    Console.WriteLine(player.CbsRosterTrendPlayerName);
                    Console.WriteLine(player.CbsRecentViews);
                    Console.WriteLine(player.CbsTodaysViews);
                    Console.WriteLine();
                }

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
                _h.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = $"{urlToScrapePrefix}/{position}";
                Console.WriteLine(urlToScrape);

                // THIS URLS HTML --> HtmlAgilityPack.HtmlDocument
                var thisUrlsHtml = htmlWeb.Load(urlToScrape);

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
                foreach(var player in listOfViewedPlayers)
                {
                    Console.WriteLine(player.CbsRosterTrendPlayerName);
                    Console.WriteLine(player.CbsRecentViews);
                    Console.WriteLine(player.CbsTodaysViews);
                    Console.WriteLine();
                }

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
                _h.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                // THIS URLS HTML --> HtmlAgilityPack.HtmlDocument
                var thisUrlsHtml = htmlWeb.Load(urlToScrape);

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
                foreach(var player in listOfTradedPlayers)
                {
                    Console.WriteLine(player.CbsRosterTrendPlayerName);
                    Console.WriteLine(player.CbsNumberOfTrades);
                    Console.WriteLine();
                }
                return listOfTradedPlayers;
            }


            // STATUS: this works
            /// <summary> Returns a list of the most traded players according to Cbs trends for one position </summary>
            /// <param name="urlToScrape"> The url of the Cbs roster trends for most traded </param>
            /// <param name="position"> Position type: 1B, 2B, 3B, SS, OF, C, DH, SP, RP </param>
            /// <example> GetListOfCbsMostTradedPlayersByPosition(urlForMostTradedByPositionPrefix, "1B"); </example>
            /// <returns> A list of most traded players for one position </returns>
            public List<CbsMostTradedPlayer> GetListOfCbsMostTradedPlayersByPosition(string urlToScrapePrefix, string position)
            {
                _h.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = $"{urlToScrapePrefix}/{position}";
                Console.WriteLine(urlToScrape);

                // THIS URLS HTML --> HtmlAgilityPack.HtmlDocument
                var thisUrlsHtml = htmlWeb.Load(urlToScrape);

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
                foreach(var player in listOfTradedPlayers)
                {
                    Console.WriteLine(player.CbsRosterTrendPlayerName);
                    Console.WriteLine(player.CbsNumberOfTrades);
                    Console.WriteLine();
                }
                return listOfTradedPlayers;
            }

        #endregion MOST TRADED ------------------------------------------------------------


    }
}