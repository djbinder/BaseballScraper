using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BaseballScraper.Models;
using BaseballScraper.Models.Yahoo;
using BaseballScraper.Models.Configuration;
using System.Threading;

namespace BaseballScraper.Controllers.YahooControllers
{
#pragma warning disable CS0414, CS0219
    public class YahooHomeController: Controller
    {

        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        private BaseballScraperContext _context;
        private readonly AirtableConfiguration _airtableConfig;
        private readonly TwitterConfiguration _twitterConfig;
        private readonly YahooConfiguration _yahooConfig;
        private readonly BaseballScraper.Controllers.YahooControllers.YahooAuthController _yahooAuthController;
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<YahooHomeController> _log;
        private static ApiEndPoints _endPoints;
        private static ApiEndPoints endPoints = new ApiEndPoints();

        public YahooHomeController(
            IOptions<AirtableConfiguration> airtableConfig,
            IOptions<TwitterConfiguration> twitterConfig,
            IOptions<YahooConfiguration> yahooConfig,
            YahooAuthController yahooAuthController,
            IHttpContextAccessor contextAccessor,
            BaseballScraperContext context,
            ILogger<YahooHomeController> log,
            IOptions<TheGameIsTheGameConfiguration> theGameConfig,
            ApiEndPoints endPoints)
        {
            _airtableConfig      = airtableConfig.Value;
            _twitterConfig       = twitterConfig.Value;
            _yahooConfig         = yahooConfig.Value;
            _yahooAuthController = yahooAuthController;
            _contextAccessor     = contextAccessor;
            _context             = context;
            _log                 = log;
            _theGameConfig       = theGameConfig.Value;
            _endPoints           = endPoints;
        }




        [HttpGet]
        [Route("/yahoohome")]
        public IActionResult ViewYahooHomePage()
        {
            // var sessionInfoDictionary = _yahooAuthController.CreateSessionInfoDictionary();

            // if(_yahooAuthController.CheckSession() == 0)
            // {
            //     Console.WriteLine();
            //     Console.WriteLine("NEW SESSION IS NEEDED");
            //     ViewBag.SessionIdExists = " NO session exists";
            //     // return RedirectToAction("setsessioninfo");
            // }
            // else
            // {
            //     Console.WriteLine("SESSION ALREADY IN PROGRESS");
            //     ViewBag.AuthCodeBag     = sessionInfoDictionary["authcode"];
            //     ViewBag.SessionIdExists = " YES session exists";
            //     ViewBag.Now             = DateTime.Now;
            // }
            return View("yahoohome");
        }



        public HttpWebRequest GenerateWebRequest(string uri)
        {
            // Start.ThisMethod();

            // pull access token from session; this is generated through the yahooauth controller
            // access token is a long mix of letters and numbers;
            string accessToken = _contextAccessor.HttpContext.Session.GetString("accesstoken");

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            // set the authorization header of the request; bearer token plus access token
            request.Headers["Authorization"] = "Bearer " + accessToken;

            // set the method of the request header
            request.Method = "GET";

            // Complete.ThisMethod();
            return request;
        }

        public string GetResponseFromServer(HttpWebRequest request)
        {
            // Start.ThisMethod();
            string serverResponse = "";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                serverResponse = reader.ReadToEnd();
            }
            // Complete.ThisMethod();
            // serverResponse comes back as xml in string format
            return serverResponse;
        }


        // generate xml from serverResponse string
        public XmlDocument TranslateServerResponseToXml (string serverResponse)
        {
            // Start.ThisMethod();
            // DOC type ---> System.Xml.XmlDocument
            XmlDocument doc       = new XmlDocument();
            XmlReader   xmlReader = XmlReader.Create(new System.IO.StringReader(serverResponse));

            doc.LoadXml(serverResponse);
            // Extensions.Spotlight("translate xml dig");
            // doc.Dig();

            // Complete.ThisMethod();
            return doc;
        }

        /// <summary> generate json for any given yahoo resource (e.g., game, league, player, team, etc.)</summary>
        /// <param name="uri"> uri will typically be generated from an endpoint within ApiEndPoints.cs</param>
        /// <returns> json / a jobject of the resource type requested</returns>
        public JObject GenerateYahooResourceJObject(string uri)
        {
            // Start.ThisMethod();

            HttpWebRequest request = GenerateWebRequest(uri);

            string serverResponse = GetResponseFromServer(request);

            XmlDocument doc = TranslateServerResponseToXml(serverResponse);

            // convert the xml to json
            string json = JsonConvert.SerializeXmlNode(doc);

            // clean the json up
            JObject resourceJson = JObject.Parse(json);

            // Complete.ThisMethod();
            return resourceJson;
        }

        /// <summary> take in an instance of a yahoo model and save it to the database</summary>
        /// <param name="yahoomodel"></param>
        public void SaveObjectToDatabase(Object yahoomodel)
        {
            Start.ThisMethod();

            _context.Add(yahoomodel);
            _context.SaveChanges();
        }

        [Route("yahoo/leaguescoreboard/create")]
        public void CreateYahooScoreboard ()
        {
            Start.ThisMethod();

            // retrieve the league key from user secrets / yahoo league config
            string leagueKey = _theGameConfig.LeagueKey;

            var uriLeagueScoreboard = endPoints.LeagueSeasonScoreboardEndPoint(leagueKey).EndPointUri;

            JObject leagueScoreboard = GenerateYahooResourceJObject(uriLeagueScoreboard);

            Extensions.PrintJObjectItems(leagueScoreboard);
        }
    }
}