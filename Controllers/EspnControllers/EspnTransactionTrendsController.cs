using System;
using System.Collections.Generic;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.Espn;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS0219, CS0414, CS1998, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.EspnControllers
{

    [Route("api/espn/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EspnTransactionTrendsController : Controller
    {
        private readonly Helpers _h = new Helpers();
        private readonly string espnMostAddedTableHtmlSelectorString = "#espn-analytics > div > div.jsx-1612911345.shell-container > div.page-container.cf > div.layout.is-full > div > div > div.InnerLayout.flex.flex-auto.flex-wrap > div:nth-child(1) > div > section > table > tbody > tr > td > div > div > div.Table2__shadow-scroller > table";

        private readonly string espnMostAddedTableHtmlXPathString = "//*[@id='espn-analytics']/div/div[5]/div[2]/div[1]/div/div/div[2]/div[1]/div/section/table/tbody/tr/td/div/div/div[2]/table";

        private readonly string espnMostAddedTableHtmlJsPathString = "document.querySelector('#espn-analytics > div > div.jsx-1612911345.shell-container > div.page-container.cf > div.layout.is-full > div > div > div.InnerLayout.flex.flex-auto.flex-wrap > div:nth-child(1) > div > section > table > tbody > tr > td > div > div > div.Table2__shadow-scroller > table')";




        // https://theathletic.com/533969/2018/09/19/exploring-the-hidden-fantasy-tools-on-yahoo-espn-and-cbs/

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

                var urlToScrape = "http://fantasy.espn.com/baseball/addeddropped";
                // var urlToScrape = "http://games.espn.com/flb/addeddropped";

                // thisUrlsHtml --> HtmlAgilityPack.HtmlDocument
                HtmlDocument thisUrlsHtmlAndScripts = htmlWeb.Load(urlToScrape);


                List<EspnTransactionTrendPlayer> listOfAddedPlayers = new List<EspnTransactionTrendPlayer>();

                // htmlChildNodes has 2 children: 1) #comment 2) html
                HtmlNodeCollection thisUrlsNodeCollection = thisUrlsHtmlAndScripts.DocumentNode.ChildNodes;

                HtmlNode thisUrlsHtmlNode = thisUrlsNodeCollection[1];

                HtmlNodeCollection htmlHeadAndBodyNodeCollection = thisUrlsHtmlNode.ChildNodes;

                HtmlNode bodyNode = htmlHeadAndBodyNodeCollection[1];
                // Console.WriteLine($"htmlBodyNode: {bodyNode.ChildNodes}");

                HtmlNodeCollection bodyNodeChildrenCollection = bodyNode.ChildNodes;

                int childCount = 0;

                foreach(var child in bodyNodeChildrenCollection)
                {
                    if(childCount == 0)
                    {
                        // Console.WriteLine(child);
                        // Console.WriteLine(child.Name);
                        // Console.WriteLine("CHILD NODES");
                        // Console.WriteLine(child.ChildNodes);

                        foreach(var child2 in child.ChildNodes)
                        {
                            Console.WriteLine(child2.Name);
                            Console.WriteLine(child2.XPath);
                            Console.WriteLine(child2.InnerText);
                        }
                    }
                    childCount++;
                }









                HtmlNode transactionTrendTable = thisUrlsHtmlAndScripts.DocumentNode.SelectSingleNode("//*[@id='espn-analytics']/div/div[5]/div[2]/footer/p[1]/a[2]");
                // HtmlNodeCollection transactionTrendTable = thisUrlsHtmlAndScripts.DocumentNode.SelectNodes("//table[contains(@class, 'Table2__table-scroller Table2__table')]");
                // Console.WriteLine($"transactionTrendTable: {transactionTrendTable}");
                // Console.WriteLine($"transactionTrendTable: {transactionTrendTable.InnerText}");
            //     Console.WriteLine($"transactionTrendTable.Count: {transactionTrendTable.Count}");

            //     // Child nodes --> 28
                // foreach(HtmlNode tableBody in transactionTrendTable)
                // {
                //     Console.WriteLine($"tableBody > ChildNodes: {tableBody}");
                //     int rowCount = 1;


            //         // Child nodes --> 13
            //         foreach(HtmlNode tableRow in tableBody.SelectNodes("tr"))
            //         {
            //             EspnTransactionTrendPlayer eAddedPlayer = new EspnTransactionTrendPlayer();

            //             int dataCellNumber = 1;

            //             // the first two rows are table headers, not player data; So you don't want to get them.
            //             if(rowCount > 2)
            //             {
            //                 // the rows in the table are odd; the first row includes BOTH the most added player AND the most dropped player; The most added player is cells 1- - 6; the most dropped player is cells 8 - 13;
            //                 foreach(HtmlNode dataCell in tableRow.SelectNodes("td"))
            //                 {
            //                     if(dataCellNumber == 1)
            //                         eAddedPlayer.EspnTrendRankNumber = dataCell.InnerText;

            //                     // player name comes back with team attached to it (split by the comma)
            //                     // player names comes back with an asterisk if player is on the DL (split by the *)
            //                     if(dataCellNumber == 2)
            //                     {
            //                         string playerNameAndDetails = dataCell.InnerText;
            //                         string[] splitDetails = playerNameAndDetails.Split('*', ',');
            //                         var playerName = splitDetails[0];
            //                         eAddedPlayer.EspnTrendPlayerName = playerName;
            //                     }

            //                     if(dataCellNumber == 3)
            //                         eAddedPlayer.EspnPlayerPosition = dataCell.InnerText;
            //                     if(dataCellNumber == 4)
            //                         eAddedPlayer.EspnPlayerAddsLastWeek = dataCell.InnerText;
            //                     if(dataCellNumber == 5)
            //                         eAddedPlayer.EspnPlayerAddsCurrentWeek = dataCell.InnerText;
            //                     if(dataCellNumber == 6)
            //                         eAddedPlayer.EspnPlayerTrendSevenDayChange = dataCell.InnerText;
            //                     dataCellNumber++;
            //                 }
            //                 listOfAddedPlayers.Add(eAddedPlayer);
            //             }
            //             rowCount++;
            //         }
            //     }
            //     // PrintEspnTrendsList(listOfAddedPlayers);
            return listOfAddedPlayers;
            }


            // STATUS: this works
            /// <summary> Gets the top 25 most dropped players from ESPN </summary>
            /// <returns> A list of EspnTransactionTrendPlayer </returns>
            public List<EspnTransactionTrendPlayer> GetListOfMostDroppedPlayers()
            {
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = "http://fantasy.espn.com/baseball/addeddropped";
                // var urlToScrape = "http://games.espn.com/flb/addeddropped";

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
