using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using Vereyon.Web;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Yahoo;
using System.Linq;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{

    #region OVERVIEW ------------------------------------------------------------

        /// <summary> Scrapes Yahoo transaction trend data </summary>
        /// <list> INDEX
        ///     <item> Set search date <a cref="YahooTransactionTrendsController.SetSearchDate(string, string, string)" /> </item>
        ///     <item> Set search date as today <a cref="YahooTransactionTrendsController.SetSearchDateAsToday()" /> </item>
        ///     <item> Generate list <a cref="YahooTransactionTrendsController.GenerateList(HtmlDocument)" /> </item>
        ///     <item> Get trends for today, all positions <a cref="YahooTransactionTrendsController.GetTrendsForTodayAllPositions" /> </item>
        ///     <item> Get trends for today, one position <a cref="YahooTransactionTrendsController.GetTrendsForTodayOnePosition(string)" /> </item>
        ///     <item> Get trends for specific date, all positions <a cref="YahooTransactionTrendsController.GetTrendsForDateAllPositions(string, string, string)" /> </item>
        ///     <item> Get trends for specific date, one position <a cref="YahooTransactionTrendsController.GetTrendsForDateOnePosition(string, string, string, string)" /> </item>
        /// </list>
        /// <list> RESOURCES
        ///     <item> https://baseball.fantasysports.yahoo.com/f1/buzzindex </item>
        ///     <item> https://baseball.fantasysports.yahoo.com/b1/buzzindex </item>
        /// </list>

    #endregion OVERVIEW ------------------------------------------------------------



    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooTransactionTrendsController : ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private readonly GoogleSheetsConnector _gSC = new GoogleSheetsConnector();


        [Route("practice")]
        public void YahooPractice()
        {
            _h.StartMethod();

            ConnectYahooTrendsToGoogleSheets();

            List<IList<YahooTransactionTrendsPlayer>> primaryList = new List<IList<YahooTransactionTrendsPlayer>>();

            List<YahooTransactionTrendsPlayer> trendsList = GetTrendsForTodayAllPositions();

            primaryList.Add(trendsList);
            var convertedList = _gSC.ConvertIListOfAnyTypeToObjectType(primaryList);

            // AddTrendsToGoogleSheets();


            // _gSC.UpdateData(convertedList);
        }


        [Route("practice/async")]
        public async Task YahooPracticeAsync()
        {
            _h.StartMethod();
            await AddTrendsToGoogleSheetsAsync("YAHOO_TRENDS","A1:Z1000","CoreCalculator");
            _h.CompleteMethod();
        }



        [HttpGet("trends")]
        public void ViewYahooTransactionTrends()
        {
            _h.StartMethod();
        }


        public void ConnectYahooTrendsToGoogleSheets()
        {
            _gSC.ConnectToGoogle();
        }


        #region SET DATES FOR SEARCHES ------------------------------------------------------------

            private string urlTrendsAllPlayersForDateFootball;

            // STATUS: this works
            /// <summary> Sets a query url for a specific date </summary>
            /// <param name="fourDigitYear"> string year e.g. "2018" </param>
            /// <param name="twoDigitMonth"> string month e.g. "10" </param>
            /// <param name="twoDigitDay"> string day e.g. "08" </param>
            /// <example> var urlToScrape = SetSearchDate("2018", "10" "01"); </example>
            /// <returns> A concatenated string of the url to scrape </returns>
            public string SetSearchDate(string fourDigitYear, string twoDigitMonth, string twoDigitDay)
            {
                urlTrendsAllPlayersForDateFootball = $"https://football.fantasysports.yahoo.com/f1/buzzindex?sort=BI_S&src=combined&bimtab=A&date={fourDigitYear}-{twoDigitMonth}-{twoDigitDay}";
                return urlTrendsAllPlayersForDateFootball;
            }


            // STATUS: this works
            /// <summary> Sets date as today's date to be added to search query url </summary>
            /// <returns> A concatenated string of the url to scrape </returns>
            public string SetSearchDateAsToday()
            {
                var currentDate = DateTime.Today;
                var currentYear = currentDate.Year.ToString();

                int currentMonthInt = currentDate.Month;
                var currentMonth = currentDate.Month.ToString();

                // if the month is less than 10, it is shortened to single digit(e.g. 9 instead 09). If less than 10, this added the zero back in front of the single number
                if(currentMonthInt < 10)
                    currentMonth = $"0{currentMonth}";

                int currentDayInt = currentDate.Day;
                var currentDay = currentDate.Day.ToString();

                // if the day is less than 10, it is shortened to single digit(e.g. 9 instead 09). If less than 10, this added the zero back in front of the single number
                if(currentDayInt < 10)
                    currentDay = $"0{currentDay}";

                var urlToScrape = SetSearchDate(currentYear, currentMonth, currentDay);

                return urlToScrape;
            }

        #endregion SET DATES FOR SEARCHES ------------------------------------------------------------



        #region LIST GENERATOR ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Generates a list of trends given a specific Html page </summary>
            /// <remarks> This method is used by all methods in the 'Get Trends' Region </remarks>
            /// <param name="thisUrlsHtml"> The html to scrape </param>
            /// <returns> A list of trends of YahooTransactionTrendsPlayers </returns>
            public List<YahooTransactionTrendsPlayer> GenerateList(HtmlDocument thisUrlsHtml)
            {
                List<YahooTransactionTrendsPlayer> listOfPlayers = new List<YahooTransactionTrendsPlayer>();

                HtmlNodeCollection transactionTrendTable = thisUrlsHtml.DocumentNode.SelectNodes("//table[contains(@class, 'Tst-table Table')]");

                foreach(HtmlNode tableNodes in transactionTrendTable)
                {
                    // TABLE BODY --> 2 ( <thead> & <tbody> )
                    foreach(HtmlNode tableBody in tableNodes.SelectNodes("tbody"))
                    {
                        var tableRowNumber = 1;

                        // TABLE ROW --> 50 (one row for each player)
                        foreach(HtmlNode tableRow in tableBody.SelectNodes("tr"))
                        {
                            YahooTransactionTrendsPlayer yPlayer = new YahooTransactionTrendsPlayer();

                            int dataCellNumber = 1;

                            // DATA CELL --> 6 ( Columns for: 'Player', 'Drops', 'Adds', 'Trades', 'Total', 'Adds - Drops')
                            foreach(HtmlNode dataCell in tableRow.SelectNodes("td"))
                            {
                                var nameXpath = $"//*[@id='buzzindex']/div/table/tbody/tr[{tableRowNumber}]/td[1]/div/div/div[1]/div/a";

                                if(dataCellNumber == 1)
                                {
                                    HtmlNodeCollection playerNamePath = dataCell.SelectNodes(nameXpath);
                                    foreach(HtmlNode playerName in playerNamePath)
                                    {
                                        yPlayer.YahooPlayerName = playerName.InnerText;
                                    }
                                }

                                if(dataCellNumber == 2)
                                    yPlayer.YahooPlayerDrops = dataCell.InnerText;

                                if(dataCellNumber == 3)
                                    yPlayer.YahooPlayerAdds = dataCell.InnerText;

                                if(dataCellNumber == 4)
                                    yPlayer.YahooPlayerTrades = dataCell.InnerText;

                                if(dataCellNumber == 5)
                                    yPlayer.YahooTransactionsTotal = dataCell.InnerText;

                                dataCellNumber++;
                            }
                            listOfPlayers.Add(yPlayer);
                            tableRowNumber++;
                        }
                    }
                }
                // PrintYahooTrendPlayers(listOfPlayers);
                return listOfPlayers;
            }

        #endregion LIST GENERATOR ------------------------------------------------------------



        #region GET TRENDS ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Get trends for all positions for today's dates </summary>
            /// <example> GetTrendsForTodayAllPositions(); </example>
            /// <returns> A list of YahooTransactionTrendsPlayer --> Name, Drops, Adds, Trades, Total </returns>
            public List<YahooTransactionTrendsPlayer> GetTrendsForTodayAllPositions()
            {
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = "https://baseball.fantasysports.yahoo.com/b1/buzzindex";
                // var urlToScrape = SetSearchDateAsToday();
                // Console.WriteLine($"Url To Scrape: {urlToScrape}");

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<YahooTransactionTrendsPlayer> listOfPlayers = GenerateList(thisUrlsHtml);

                Console.WriteLine($"total count: {listOfPlayers.Count}");

                PrintYahooTrendPlayers(listOfPlayers);
                return listOfPlayers;
            }


            // STATUS: this works
            /// <summary> Get trends for one position for today's date </summary>
            /// <param name="positionShort"> Shortened position string ("1B", "2B", "3B", "SS", "OF", "P", "SP", "RP", "C", "Util")</param>
            /// <example> GetTrendsForTodayOnePosition("QB"); </example>
            /// <returns> A list of YahooTransactionTrendsPlayer --> Name, Drops, Adds, Trades, Total </returns>
            public List<YahooTransactionTrendsPlayer> GetTrendsForTodayOnePosition(string positionShort)
            {
                _h.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = $"{SetSearchDateAsToday()}&pos={positionShort}";
                Console.WriteLine($"Url To Scrape: {urlToScrape}");

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<YahooTransactionTrendsPlayer> listOfPlayers = GenerateList(thisUrlsHtml);

                // PrintYahooTrendPlayers(listOfPlayers);
                return listOfPlayers;
            }


            // STATUS: this works
            /// <summary> Get trends for all positions for a specific date </summary>
            /// <param name="fourDigitYear"> string year e.g. "2018" </param>
            /// <param name="twoDigitMonth"> string month e.g. "10" </param>
            /// <param name="twoDigitDay"> string day e.g. "08" </param>
            /// <example> GetTrendsForDateAllPositions("2018", "10", "01"); </example>
            /// <returns> A list of YahooTransactionTrendsPlayer --> Name, Drops, Adds, Trades, Total </returns>
            public List<YahooTransactionTrendsPlayer> GetTrendsForDateAllPositions(string fourDigitYear, string twoDigitMonth, string twoDigitDay)
            {
                _h.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = SetSearchDate(fourDigitYear, twoDigitMonth, twoDigitDay);
                Console.WriteLine($"Url To Scrape: {urlToScrape}");

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<YahooTransactionTrendsPlayer> listOfPlayers = GenerateList(thisUrlsHtml);

                return listOfPlayers;
            }


            // STATUS: this works
            /// <summary> Get trends for one position for a specific date </summary>
            /// <param name="fourDigitYear"> string year e.g. "2018" </param>
            /// <param name="twoDigitMonth"> string month e.g. "10" </param>
            /// <param name="twoDigitDay"> string day e.g. "08" </param>
            /// <param name="positionShort"> Shortened position string ("1B", "2B", "3B", "SS", "OF", "P", "SP", "RP", "C", "Util")</param>
            /// <example> GetTrendsForDateOnePosition("2018", "10", "01", "QB"); </example>
            /// <returns> A list of YahooTransactionTrendsPlayer --> Name, Drops, Adds, Trades, Total </returns>
            public List<YahooTransactionTrendsPlayer> GetTrendsForDateOnePosition(string fourDigitYear, string twoDigitMonth, string twoDigitDay, string positionShort)
            {
                _h.StartMethod();
                HtmlWeb htmlWeb = new HtmlWeb();

                var urlToScrape = $"{SetSearchDate(fourDigitYear, twoDigitMonth, twoDigitDay)}&pos={positionShort}";
                Console.WriteLine($"Url To Scrape: {urlToScrape}");

                HtmlDocument thisUrlsHtml = htmlWeb.Load(urlToScrape);

                List<YahooTransactionTrendsPlayer> listOfPlayers = GenerateList(thisUrlsHtml);

                return listOfPlayers;
            }

        #endregion GET TRENDS ------------------------------------------------------------



        #region GENERATE GOOGLE SHEETS LISTS ------------------------------------------------------------


            /// <example>
            /// AddTrendsToGoogleSheets("YAHOO_TRENDS","A1:Z1000","CoreCalculator")
            /// </example>
            public void AddTrendsToGoogleSheets(string tabName, string range, string jsonGroupName)
            {
                ConnectYahooTrendsToGoogleSheets();
                List<YahooTransactionTrendsPlayer> allTrendInfo = GetTrendsForTodayAllPositions();

                // instantiate a list foreach column in the transaction trends api
                // each list is associated with a particular column in the report
                List<object> playerNames = new List<object>() { "Player Name" };
                List<object> playerDrops = new List<object>() { "Drops" };
                List<object> playerAdds = new List<object>() { "Adds" };
                List<object> playerTrades = new List<object>() { "Trades" };
                List<object> playerTransactionsTotals = new List<object>() { "Total "};

                // add the player's values to each of the column lists
                foreach(YahooTransactionTrendsPlayer player in allTrendInfo)
                {
                    playerNames.Add(player.YahooPlayerName);
                    playerDrops.Add(player.YahooPlayerDrops);
                    playerAdds.Add(player.YahooPlayerAdds);
                    playerTrades.Add(player.YahooPlayerTrades);
                    playerTransactionsTotals.Add(player.YahooTransactionsTotal);
                }

                // add all columns lists to a list to create a list of lists
                List<IList<object>> listOfLists = new List<IList<object>>
                {
                    playerNames,
                    playerDrops,
                    playerAdds,
                    playerTrades,
                    playerTransactionsTotals
                };

                // Write that data in listOfLists to YAHOO_TRENDS tab in range A1: Z1000 in the CoreCalculator group / sheet
                // _gSC.WriteGoogleSheetColumns(listOfLists, "YAHOO_TRENDS","A1:Z1000","CoreCalculator");
                _gSC.WriteGoogleSheetColumns(listOfLists, tabName, range, jsonGroupName);
            }


            public Task DoAsyncTest(string item)
            {
                Task.Delay(1000);
                Console.WriteLine($"item: {item}");
                return Task.CompletedTask;
            }

            /// <example>
            /// await AddTrendsToGoogleSheetsAsync("YAHOO_TRENDS","A1:Z1000","CoreCalculator")
            /// </example>
            public async Task AddTrendsToGoogleSheetsAsync(string tabName, string range, string jsonGroupName)
            {
                ConnectYahooTrendsToGoogleSheets();
                List<YahooTransactionTrendsPlayer> allTrendInfo = GetTrendsForTodayAllPositions();

                // instantiate a list foreach column in the transaction trends api
                // each list is associated with a particular column in the report
                List<object> playerNames = new List<object>() { "Player Name" };
                List<object> playerDrops = new List<object>() { "Drops" };
                List<object> playerAdds = new List<object>() { "Adds" };
                List<object> playerTrades = new List<object>() { "Trades" };
                List<object> playerTransactionsTotals = new List<object>() { "Total "};

                // add the player's values to each of the column lists
                foreach(YahooTransactionTrendsPlayer player in allTrendInfo)
                {
                    playerNames.Add(item: player.YahooPlayerName);
                    playerDrops.Add(item: player.YahooPlayerDrops);
                    playerAdds.Add(item: player.YahooPlayerAdds);
                    playerTrades.Add(item: player.YahooPlayerTrades);
                    playerTransactionsTotals.Add(item: player.YahooTransactionsTotal);
                }

                // add all columns lists to a list to create a list of lists
                // Write that data in listOfLists to YAHOO_TRENDS tab in range A1: Z1000 in the CoreCalculator group / sheet
                await _gSC.WriteGoogleSheetColumnsAsync(new List<IList<object>>
                    {
                        playerNames,
                        playerDrops,
                        playerAdds,
                        playerTrades,
                        playerTransactionsTotals
                    }, tabName, range, jsonGroupName);
            }


        #endregion GENERATE GOOGLE SHEETS LISTS ------------------------------------------------------------




        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintYahooTrendPlayers(List<YahooTransactionTrendsPlayer> listOfPlayers)
            {
                foreach(var p in listOfPlayers)
                {
                    Console.WriteLine(p.YahooPlayerName);
                    Console.WriteLine(p.YahooPlayerDrops);
                    Console.WriteLine(p.YahooPlayerAdds);
                    Console.WriteLine(p.YahooPlayerTrades);
                    Console.WriteLine(p.YahooTransactionsTotal);
                    Console.WriteLine();
                }
            }

        #endregion PRINTING PRESS ------------------------------------------------------------


    }
}
