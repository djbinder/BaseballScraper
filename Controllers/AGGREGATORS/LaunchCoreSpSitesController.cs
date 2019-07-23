using AirtableApiClient;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Player;
using C = System.Console;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;
using static BaseballScraper.Controllers.PlayerControllers.PlayerBaseController;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.AGGREGATORS
{
    [Route("api/aggregators/[controller]")]
    [ApiController]
    public class LaunchCoreSpSitesController : ControllerBase
    {
        private readonly Helpers                   _helpers;
        private readonly AirtableManager           _atM;
        private readonly PlayerBaseFromGoogleSheet _playerBaseFromGoogleSheet;
        private readonly AirtableConfiguration     _airtableConfig;
        private readonly PostmanMethods            _postmanMethods;
        private readonly GoogleSheetsConnector     _googleSheetsConnector;
        private readonly GoogleSheetConfiguration  _crunchTimePlayerIdMapConfiguration;
        private readonly GoogleSheetConfiguration  _sfbbPlayerIdMapConfiguration;
        private readonly AirtableConfiguration     _spRankingsConfiguration;
        private readonly AirtableConfiguration     _authorsConfiguration;

        public LaunchCoreSpSitesController
        (
            Helpers helpers,
            AirtableManager atM,
            PlayerBaseFromGoogleSheet playerBaseFromGoogleSheet,
            IOptions<AirtableConfiguration> airtableConfig,
            PostmanMethods postmanMethods,
            GoogleSheetsConnector googleSheetsConnector,
            IOptionsSnapshot<GoogleSheetConfiguration> options,
            IOptionsSnapshot<AirtableConfiguration> airTableOptions
        )
        {
            _helpers                            = helpers;
            _atM                                = atM;
            _playerBaseFromGoogleSheet          = playerBaseFromGoogleSheet;
            _airtableConfig                     = airtableConfig.Value;
            _postmanMethods                     = postmanMethods;
            _googleSheetsConnector              = googleSheetsConnector;
            _crunchTimePlayerIdMapConfiguration = options.Get("CrunchtimePlayerIdMap");
            _sfbbPlayerIdMapConfiguration       = options.Get("SfbbPlayerIdMap");
            _spRankingsConfiguration            = airTableOptions.Get("SpRankings");
            _authorsConfiguration               = airTableOptions.Get("Authors");
        }

        public LaunchCoreSpSitesController() {}




        /*
            https://127.0.0.1:5001/api/aggregators/launchcorespsites/test
        */
        [HttpGet("test")]
        public void TestCoreSpSitesController()
        {
            _helpers.StartMethod();
        }


        /*
            https://127.0.0.1:5001/api/aggregators/launchcorespsites/test
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
            await RetrievePitcherRankingInfoFromDatabase();
        }






        #region PITCHER RANKINGS ------------------------------------------------------------


            public async Task RetrievePitcherRankingInfoFromDatabase()
            {
                _helpers.StartMethod();
                var spRankingTableConfig    = _atM.GetSpRankingsTableConfiguration();
                string airTableKey          = spRankingTableConfig.ApiKey = _airtableConfig.ApiKey;
                string authenticationString = spRankingTableConfig.AuthenticationString;

                var listOfSpRankings =
                    await _atM.GetAllRecordsFromAirtableAsync
                    (
                        _spRankingsConfiguration.TableName,
                        _spRankingsConfiguration.AuthenticationString
                    );

                var listOfAuthors =
                    await _atM.GetAllRecordsFromAirtableAsync
                    (
                        _authorsConfiguration.TableName,
                        _authorsConfiguration.AuthenticationString
                    );

                foreach(var ranking in listOfSpRankings)
                {
                    var authorField = ranking.GetField("Author");
                    JArray fieldJArray = authorField as JArray;
                    string authorId = fieldJArray[0].ToString();
                    C.WriteLine($"authorId: {authorId}");
                }

                foreach (var author in listOfAuthors)
                {
                    // _helpers.Dig(author);
                    C.WriteLine($"author.Id: {author.Id}");
                    var rankingQuery =
                        (from ranking in listOfSpRankings
                            where ((JArray)ranking.GetField("Author")).Select(item => (string)item[0]).ToString() == author.Id
                            select ranking
                            );

                    _helpers.Dig(rankingQuery);
                }
            }

        #endregion PITCHER RANKINGS ------------------------------------------------------------





        #region LAUNCH ALL SITES - PRIMARY METHOD ------------------------------------------------------------


            // STATUS [ July 11, 2019 ] : this works
            /// <summary>
            ///     Launch all websites for an individual in Google Chrome
            ///     Sites include:
            ///         * FanGraphs
            ///         * Baseball Prospectus
            ///         * Rotowire
            ///         * Baseball Savant
            ///         * Baseball HQ
            ///         * Yahoo
            /// </summary>
            /// <remarks>
            ///     Uses google sheet connector
            ///     See: gSheetNames.json for sheet info
            ///     Relies on SfbbPlayerIdMap to pull various ids for the player
            /// </remarks>
            /// <example>
            ///     LaunchAllPagesInChromeForPlayer("tyler","chatwood");
            /// </example>
            public void LaunchAllPagesInChromeForPlayer(string firstName, string lastName)
            {
                firstName = FormatPlayerFirstAndLastName(firstName); // capitalize first letter if needed
                lastName = FormatPlayerFirstAndLastName(lastName);   // capitalize first letter if needed
                string playerName = $"{firstName} {lastName}";

                IEnumerable<SfbbPlayerBase> allPlayerBases = _playerBaseFromGoogleSheet.GetAllPlayerBasesFromGoogleSheet("A7:AQ2284");

                IEnumerable<SfbbPlayerBase> onePlayerBase =
                    from playerBases in allPlayerBases
                    where playerBases.FANGRAPHSNAME == playerName
                    select playerBases;

                int nullCheck = onePlayerBase.Count();
                if(nullCheck == 0)
                {
                    ManageNullPlayerBase(firstName, lastName);
                }

                else
                {
                    SfbbPlayerBase player = onePlayerBase.First();
                    LaunchPlayersFanGraphsPageInChrome(player);
                    LaunchPlayersBaseballProsectusPageInChrome(player);
                    LaunchPlayersRotowirePageInChrome(player);
                    LaunchPlayersBaseballSavantPageInChrome(player);
                    LaunchPlayersBaseballHqPageInChrome(player);
                    LaunchPlayersYahooPageInChrome(player);
                    LaunchPlayersBaseballReferencePageInChrome(player);
                }
            }


            public void ManageNullPlayerBase(string firstName, string lastName)
            {
                C.ForegroundColor = ConsoleColor.Red;
                C.WriteLine($"\n****************************************************************");
                C.WriteLine($"PLAYER BASE DOES NOT EXIST FOR: {firstName} {lastName}");
                C.WriteLine($"See: {_sfbbPlayerIdMapConfiguration.Link}");
                C.WriteLine($"****************************************************************\n");
                C.ResetColor();
            }


        #endregion LAUNCH ALL SITES - PRIMARY METHOD ------------------------------------------------------------





        #region LAUNCH INDIVIDUAL SITES - PRIMARY METHOD ------------------------------------------------------------


            // STATUS [ July 11, 2019 ] : this works
            /// <summary>
            ///     Launch FanGraphs page for player
            /// </summary>
            public void LaunchPlayersFanGraphsPageInChrome(SfbbPlayerBase player)
            {
                var playerFanGraphsId = player.IDFANGRAPHS;
                string urlString = $"https://www.fangraphs.com/statss.aspx?playerid={playerFanGraphsId}";
                C.WriteLine($"FG: {urlString}");
                Process.Start("open", urlString);
            }


            // STATUS [ July 11, 2019 ] : this works
            /// <summary>
            ///     Launch Baseball Prospectus page for player
            /// </summary>
            public void LaunchPlayersBaseballProsectusPageInChrome(SfbbPlayerBase player)
            {
                var playerBaseballProspectusId = player.BPID;
                string urlString = $"https://legacy.baseballprospectus.com/card/{playerBaseballProspectusId}/";
                C.WriteLine($"BB PROSPECTUS: {urlString}");
                Process.Start("open", urlString);
            }


            // STATUS [ July 11, 2019 ] : this works
            /// <summary>
            ///     Launch Rotowire page for player
            /// </summary>
            public void LaunchPlayersRotowirePageInChrome(SfbbPlayerBase player)
            {
                var playerRotowireId = player.ROTOWIREID;
                string urlString = $"https://www.rotowire.com/baseball/player.php?id={playerRotowireId}";
                C.WriteLine($"ROTOWIRE: {urlString}");
                Process.Start("open", urlString);
            }


            // STATUS [ July 11, 2019 ] : this works
            /// <summary>
            ///     Launch Baseball Savant page for player
            /// </summary>
            public void LaunchPlayersBaseballSavantPageInChrome(SfbbPlayerBase player)
            {
                var mlbId = player.MLBID;
                string urlString = $"https://baseballsavant.mlb.com/savant-player/{mlbId}";
                C.WriteLine($"SAVANT: {urlString}");
                Process.Start("open", urlString);
            }


            // STATUS [ July 11, 2019 ] : this works
            /// <summary>
            ///     Launch Baseball HQ page for player
            /// </summary>
            public void LaunchPlayersBaseballHqPageInChrome(SfbbPlayerBase player)
            {
                var playerBaseballHqId = player.HQID;
                var playerPosition = player.POS;

                // the urls are slightly different for pitchers and hitters
                string linkType;
                if(playerPosition == "P")
                    linkType = "pitcherlink";
                else
                    linkType = "batterlink";

                string urlString = $"https://www.baseballhq.com/members/tools/{linkType}/?id={playerBaseballHqId}";
                C.WriteLine($"HQ: {urlString}");
                Process.Start("open", urlString);
            }


            // STATUS [ July 11, 2019 ] : this works
            /// <summary>
            ///     Launch Yahoo page for player
            /// </summary>
            public void LaunchPlayersYahooPageInChrome(SfbbPlayerBase player)
            {
                var playerYahooId = player.YAHOOID;
                string urlString = $"https://sports.yahoo.com/mlb/players/{playerYahooId}/";
                C.WriteLine($"YAHOO: {urlString}");
                Process.Start("open", urlString);
            }


            // STATUS [ July 11, 2019 ] : this works
            /// <summary>
            ///     Launch Baseball Reference page
            /// </summary>
            public void LaunchPlayersBaseballReferencePageInChrome(SfbbPlayerBase player)
            {
                var baseballReferenceId = player.BREFID;
                string lastNameFirstLetter = player.LASTNAME.Substring(0,1).ToLower();
                string urlString = $"https://www.baseball-reference.com/players/{lastNameFirstLetter}/{baseballReferenceId}.shtml";
                C.WriteLine($"BREF: {urlString}");
                Process.Start("open", urlString);
            }


        #endregion LAUNCH INDIVIDUAL SITES - PRIMARY METHOD ------------------------------------------------------------





        #region LAUNCH SITES - SUPPORT METHODS ------------------------------------------------------------

            // STATUS [ July 11, 2019 ] : this works
            /// <summary>
            ///     Capitalizes first letter in player's first and / or last name
            ///     Without this, you will get an error / will not find player in sheet
            /// </summary>
            public string FormatPlayerFirstAndLastName(string str)
            {
                var isFirstLetterCapitalized = char.IsUpper(str, 0);
                if(isFirstLetterCapitalized == false) { str = char.ToUpper(str[0]) + str.Substring(1); }
                return str;
            }

        #endregion LAUNCH SITES - SUPPORT METHODS ------------------------------------------------------------





        // #endregion START ------------------------------------------------------------
    }

}



        // NOT NEEDED; JUST PRACTICE
        // string linkToDownload = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2019&month=0&season1=2019&ind=0";
        // string outputFileName = ""scratch/puppeteer_test2.pdf";
        // public async Task DownloadWebPageAsPDF(string linkToDownLoad, string outputFileName)
        // {
        //     await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
        //     var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        //     {
        //         Headless = true
        //     });
        //     var page = await browser.NewPageAsync();

        //     await page.GoToAsync(linkToDownLoad);

        //     await page.PdfAsync(outputFileName);
        // }
