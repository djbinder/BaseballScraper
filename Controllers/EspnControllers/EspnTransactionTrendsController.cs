using System;
using System.Collections.Generic;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers.EspnControllers
{

    #region OVERVIEW ------------------------------------------------------------

        /// <summary> Gets lists of ESPN most added or dropped players (top 25) </summary>
        /// <list> INDEX
        ///     <item> Get most added players <a cref="EspnTransactionTrendsController.GetListOfMostAddedPlayers" /> </item>
        ///     <item> Get most dropped players <a cref="EspnTransactionTrendsController.GetListOfMostDroppedPlayers" /> </item>
        /// </list>
        /// <list> RESOURCES
        ///     <item> http://games.espn.com/flb/addeddropped </item>
        /// </list>

    #endregion OVERVIEW ------------------------------------------------------------



    [Route("espn")]
    public class EspnTransactionTrendsController : Controller
    {
        private readonly Helpers _h = new Helpers();

        [Route("trends")]
        [HttpGet]
        public void ViewEspnTransactionTrends()

        {
            _h.StartMethod();
            GetListOfMostAddedPlayers();
            GetListOfMostDroppedPlayers();
        }


        #region GET LISTS FOR ADDED OR MOST DROPPED ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Gets the top 25 most added players from ESPN </summary>
            /// <returns> A list of EspnTransactionTrendPlayer </returns>
            public List<EspnTransactionTrendPlayer> GetListOfMostAddedPlayers()
            {
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = "http://games.espn.com/flb/addeddropped";

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<EspnTransactionTrendPlayer> listOfAddedPlayers = new List<EspnTransactionTrendPlayer>();

                HtmlNodeCollection transactionTrendTable = thisUrlsHtml.DocumentNode.SelectNodes("//table[contains(@class, 'tableBody')]");

                // Child nodes --> 28
                foreach(HtmlNode tableBody in transactionTrendTable)
                {
                    // Child nodes --> 13
                    foreach(HtmlNode tableRow in tableBody.SelectNodes("tr"))
                    {
                        EspnTransactionTrendPlayer eAddedPlayer = new EspnTransactionTrendPlayer();

                        int dataCellNumber = 1;

                        // the rows in the table are odd; the first row includes BOTH the most added player AND the most dropped player; The most added player is cells 1- - 6; the most dropped player is cells 8 - 13;
                        foreach(HtmlNode dataCell in tableRow.SelectNodes("td"))
                        {
                            if(dataCellNumber == 1)
                                eAddedPlayer.EspnTrendRankNumber = dataCell.InnerText;

                            // player name comes back with team attached to it (split by the comma)
                            // player names comes back with an asterisk if player is on the DL (split by the *)
                            if(dataCellNumber == 2)
                            {
                                string playerNameAndDetails = dataCell.InnerText;
                                string[] splitDetails = playerNameAndDetails.Split('*', ',');
                                var playerName = splitDetails[0];
                                eAddedPlayer.EspnTrendPlayerName = playerName;
                            }

                            if(dataCellNumber == 3)
                                eAddedPlayer.EspnPlayerPosition = dataCell.InnerText;
                            if(dataCellNumber == 4)
                                eAddedPlayer.EspnPlayerAddsLastWeek = dataCell.InnerText;
                            if(dataCellNumber == 5)
                                eAddedPlayer.EspnPlayerAddsCurrentWeek = dataCell.InnerText;
                            if(dataCellNumber == 6)
                                eAddedPlayer.EspnPlayerTrendSevenDayChange = dataCell.InnerText;
                            dataCellNumber++;
                        }
                        listOfAddedPlayers.Add(eAddedPlayer);
                    }
                }
                PrintEspnTrendsList(listOfAddedPlayers);
                return listOfAddedPlayers;
            }


            // STATUS: this works
            /// <summary> Gets the top 25 most dropped players from ESPN </summary>
            /// <returns> A list of EspnTransactionTrendPlayer </returns>
            public List<EspnTransactionTrendPlayer> GetListOfMostDroppedPlayers()
            {
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = "http://games.espn.com/flb/addeddropped";

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<EspnTransactionTrendPlayer> listOfDroppedPlayers = new List<EspnTransactionTrendPlayer>();

                HtmlNodeCollection transactionTrendTable = thisUrlsHtml.DocumentNode.SelectNodes("//table[contains(@class, 'tableBody')]");

                // Child nodes --> 28
                foreach(HtmlNode tableBody in transactionTrendTable)
                {
                    // Child nodes --> 13
                    foreach(HtmlNode tableRow in tableBody.SelectNodes("tr"))
                    {
                        EspnTransactionTrendPlayer eDroppedPlayer = new EspnTransactionTrendPlayer();

                        int dataCellNumber = 1;

                        // the rows in the table are odd; the first row includes BOTH the most added player AND the most dropped player; The most added player is cells 1- - 6; the most dropped player is cells 8 - 13;
                        foreach(HtmlNode dataCell in tableRow.SelectNodes("td"))
                        {
                            if(dataCellNumber == 8)
                                eDroppedPlayer.EspnTrendRankNumber = dataCell.InnerText;

                            // player name comes back with team attached to it (split by the comma)
                            // player names comes back with an asterisk if player is on the DL (split by the *)
                            if(dataCellNumber == 9)
                            {
                                string playerNameAndDetails = dataCell.InnerText;
                                string[] splitDetails = playerNameAndDetails.Split('*', ',');
                                var playerName = splitDetails[0];
                                eDroppedPlayer.EspnTrendPlayerName = playerName;
                            }

                            if(dataCellNumber == 10)
                                eDroppedPlayer.EspnPlayerPosition = dataCell.InnerText;
                            if(dataCellNumber == 11)
                                eDroppedPlayer.EspnPlayerAddsLastWeek = dataCell.InnerText;
                            if(dataCellNumber == 12)
                                eDroppedPlayer.EspnPlayerAddsCurrentWeek = dataCell.InnerText;
                            if(dataCellNumber == 13)
                                eDroppedPlayer.EspnPlayerTrendSevenDayChange = dataCell.InnerText;
                            dataCellNumber++;
                        }
                        listOfDroppedPlayers.Add(eDroppedPlayer);
                    }
                }
                PrintEspnTrendsList(listOfDroppedPlayers);
                return listOfDroppedPlayers;
            }

        #endregion GET LISTS FOR ADDED OR MOST DROPPED ------------------------------------------------------------



        #region HELPERS ------------------------------------------------------------

            public void PrintEspnTrendsList(List<EspnTransactionTrendPlayer> listOfPlayers)
            {
                foreach(var player in listOfPlayers)
                {
                    Console.WriteLine(player.EspnTrendRankNumber);
                    Console.WriteLine(player.EspnTrendPlayerName);
                    Console.WriteLine(player.EspnPlayerPosition);
                    Console.WriteLine(player.EspnPlayerAddsLastWeek);
                    Console.WriteLine(player.EspnPlayerAddsCurrentWeek);
                    Console.WriteLine(player.EspnPlayerTrendSevenDayChange);
                    Console.WriteLine();
                }
            }

        #endregion HELPERS ------------------------------------------------------------
    }
}