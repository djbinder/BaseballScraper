﻿using System;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BaseballScraper.Models;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Infrastructure;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooApiRequestController: ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private static YahooApiEndPoints _endPoints = new YahooApiEndPoints();
        private readonly BaseballScraperContext _context;
        private readonly BaseballScraper.Controllers.YahooControllers.YahooAuthController _yahooAuthController;
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private readonly IHttpContextAccessor _contextAccessor;

        public static readonly string yahooConfigFilePath = "Configuration/yahooConfig.json";
        public static readonly string theGameConfigFilePath = "Configuration/theGameIsTheGameConfig.json";

        public static readonly JsonHandler.NewtonsoftJsonHandlers _newtonHandler = new JsonHandler.NewtonsoftJsonHandlers();



        public YahooApiRequestController(YahooAuthController yahooAuthController, IHttpContextAccessor contextAccessor, BaseballScraperContext context,IOptions<TheGameIsTheGameConfiguration> theGameConfig)
        {
            _yahooAuthController = yahooAuthController;
            _contextAccessor     = contextAccessor;
            _context             = context;
            _theGameConfig       = theGameConfig.Value;
        }

        public YahooApiRequestController() {}




        [HttpGet]
        [Route("")]
        public void ViewYahooHomePage()
        {
            _h.StartMethod();

            var x = _yahooAuthController.ExchangeRefreshTokenForNewAccessToken();

            string leagueKey = GetTheGameIsTheGameLeagueKey();
            Console.WriteLine($"leagueKey: {leagueKey}");

            var uriLeagueScoreboard = _endPoints.LeagueSeasonScoreboardEndPoint(leagueKey).EndPointUri;
            Console.WriteLine($"HOME CreateYahooScoreboard > uriLeagueScoreboard: {uriLeagueScoreboard}");

            JObject leagueScoreboard = GenerateYahooResourceJObject(uriLeagueScoreboard);
            // _h.Dig(leagueScoreboard);
        }


        #region GENERATE YAHOO LEAGUE KEY ------------------------------------------------------------

            // STATUS [ June 7, 2019 ] : this works
            // STEP 1
            /// <summary>
            ///     The yahoo game id for mlb changes each season
            ///     This method gets the id for the current year
            ///     Method is ultimately called in 'GetTheGameIsTheGameLeagueKey' method
            /// </summary>
            /// <returns>
            ///     A string that is three numbers
            ///     e.g., 378 OR 388
            /// </returns>
            public string GetYahooMlbGameKeyForThisYear()
            {
                var gameLink = "https://fantasysports.yahooapis.com/fantasy/v2/game/mlb";
                var gameObject = GenerateYahooResourceJObject(gameLink);
                var gameKey = gameObject["fantasy_content"]["game"]["game_key"].ToString();

                return gameKey;
            }

            // STATUS [ June 7, 2019 ] : this works
            // STEP 2
            /// <summary>
            ///     Each yahoo league has a unique id
            ///     This method gets the id for the league you want data from
            ///     The league key suffix is in a config file
            ///     Method is ultimately called in 'GetTheGameIsTheGameLeagueKey' method
            /// </summary>
            /// <returns>
            ///     A string of lowercase L + the league id
            ///     e.g., l.1234 OR l.679
            /// </returns>
            public string GetTheGameIsTheGameLeagueKeySuffix()
            {
                TheGameIsTheGameConfiguration theGameConfig = new TheGameIsTheGameConfiguration();
                Type theGameConfigType = theGameConfig.GetType();

                var configObject = _newtonHandler.DeserializeJsonFromFile(theGameConfigFilePath, theGameConfigType) as TheGameIsTheGameConfiguration;

                string leagueKeySuffix = configObject.LeagueKeySuffix;

                return leagueKeySuffix;
            }


            // STATUS [ June 7, 2019 ] : this works
            // STEP 3
            /// <summary>
            ///     To get league day you need the league key
            ///     The league key is a combo of the mlb game key and the league suffix
            ///     Methods calls the two previous methods (STEP 1 and STEP 2)
            /// </summary>
            /// <returns>
            ///     A string that is the Y! mlb game key and the league suffix
            ///     e.g., 378.l.1234 OR 388.l.679
            /// </returns>
            public string GetTheGameIsTheGameLeagueKey()
            {
                // _h.StartMethod();
                var gameKey = GetYahooMlbGameKeyForThisYear();
                var theGameKeySuffix = GetTheGameIsTheGameLeagueKeySuffix();
                var leagueKey = $"{gameKey}{theGameKeySuffix}";
                // PrintLeagueKeyDetails(gameKey,theGameKeySuffix,leagueKey);
                return leagueKey;
            }


        #endregion GENERATE YAHOO LEAGUE KEY ------------------------------------------------------------


        // STEP 1
        /// <summary>
        ///     Generates the web request using a yahoo api endpoint.
        ///     This is followed by the method GetResponseFromServer
        ///     It is ultimately called within GenerateYahooResourceJObject Method
        /// </summary>
        /// <param name="uri"></param>
        /// <returns> HttpWebRequest request </returns>
        public HttpWebRequest GenerateWebRequest(string uri)
        {
            // _h.StartMethod();
            // Console.WriteLine($"uri: {uri}");

            var aTR = _yahooAuthController.GetYahooAccessTokenResponse();
            var accessToken = aTR.AccessToken;
            // Console.WriteLine($"HOME GenerateWebRequest > accessToken: {accessToken}");

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            // set the authorization and method headers of the request
            request.Headers["Authorization"] = "Bearer " + accessToken;
            request.Method = "GET";

            return request;
        }

        // STEP 2
        /// <summary>
        ///     Follows GenerateWebRequest method.
        ///     It receives the response from the request and returns a string of xml.static
        ///     This is followed by the TranslateServerResponseToXml Method.
        ///     It is ultimatelly called within GenerateYahooResourceJObject Method
        /// </summary>
        /// <param name="request"></param>
        /// <returns> serverResponse string; that looks like Xml </returns>
        public string GetResponseFromServer(HttpWebRequest request)
        {
            // _h.StartMethod();
            string serverResponse = "";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                serverResponse = reader.ReadToEnd();
                return serverResponse;
            }
            // _h.CompleteMethod();
        }

        // STEP 3
        /// <summary>
        ///     Follows GerResponseFromServer method.
        ///     Generate xml from serverResponse string.
        ///     This is called within the GenerateYahooResourceJObject Method
        /// </summary>
        /// <param name="serverResponse"></param>
        /// <returns> xml document / content from the called  yahoo api endpoint</returns>
        /// <see cref=\"${GenerateYahooResourceJObject}\"/>
        public XmlDocument TranslateServerResponseToXml (string serverResponse)
        {
            // _h.StartMethod();

            XmlDocument doc       = new XmlDocument();
            XmlReader   xmlReader = XmlReader.Create(new System.IO.StringReader(serverResponse));

            doc.LoadXml(serverResponse);

            // _h.CompleteMethod();
            return doc;
        }

        // STEP 4 --> calls methods of Steps 1, 2, 3
        /// <summary>
        ///     Generate json for any given yahoo resource (e.g., game, league, player, team, etc.)
        /// </summary>
        /// <param name="uri"> uri will typically be generated from an endpoint within YahooApiEndPoints.cs</param>
        /// <returns> json / a jobject of the resource type requested</returns>
        public JObject GenerateYahooResourceJObject(string uri)
        {
            // _h.StartMethod();
            // Console.WriteLine($"HOME GenerateYahooResourceJObject > uri: {uri}");

            // System.Net.HttpWebRequest
            HttpWebRequest request = GenerateWebRequest(uri);

            string serverResponse = GetResponseFromServer(request);

            XmlDocument doc = TranslateServerResponseToXml(serverResponse);

            // convert the xml to json
            string json = JsonConvert.SerializeXmlNode(doc);

            // clean the json up
            JObject resourceJson = JObject.Parse(json);

            // _h.CompleteMethod();
            return resourceJson;
        }

        /// <summary> take in an instance of a yahoo model and save it to the database</summary>
        /// <param name="yahoomodel"></param>
        public void SaveObjectToDatabase(Object yahoomodel)
        {
            _h.StartMethod();

            _context.Add(yahoomodel);
            _context.SaveChanges();
        }











        #region PRINTING PRESS ------------------------------------------------------------


            public void PrintHttpWebRequestDetails(HttpWebRequest request)
            {
                Console.WriteLine($"HOME GetResponseFromServer > request: {request}");
                Console.WriteLine("---");
                Console.WriteLine($"{request.Credentials}");
                Console.WriteLine("---");

                Console.WriteLine($"{request.Headers}");
                Console.WriteLine("---");
                Console.WriteLine();
            }


            public void PrintLeagueKeyDetails(string GameKey, string Suffix, string LeagueKey)
            {
                Console.WriteLine();
                Console.WriteLine("------------------------------------");
                Console.WriteLine("### LEAGUE KEY INFO ###");
                Console.WriteLine("------------------------------------");
                Console.WriteLine($"LEAGUE KEY        | {LeagueKey}");
                Console.WriteLine($"GAME KEY          | {GameKey}");
                Console.WriteLine($"LEAGUE KEY SUFFIX | {Suffix}");
                Console.WriteLine("------------------------------------");
                Console.WriteLine();
            }



        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}