using System;
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

namespace BaseballScraper.Controllers.YahooControllers
{
#pragma warning disable CS0414, CS0219
    public class YahooHomeController: Controller
    {
        private Constants _c = new Constants();
        private BaseballScraperContext _context;
        private static YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private readonly BaseballScraper.Controllers.YahooControllers.YahooAuthController _yahooAuthController;
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private readonly IHttpContextAccessor _contextAccessor;

        // private readonly AirtableConfiguration _airtableConfig;
        // private readonly TwitterConfiguration _twitterConfig;
        // private readonly YahooConfiguration _yahooConfig;
        // private readonly ILogger<YahooHomeController> _log;
        // private static YahooApiEndPoints _endPoints;

        public YahooHomeController(
            YahooAuthController yahooAuthController,
            IHttpContextAccessor contextAccessor,
            BaseballScraperContext context,
            IOptions<TheGameIsTheGameConfiguration> theGameConfig
            // IOptions<AirtableConfiguration> airtableConfig,
            // IOptions<TwitterConfiguration> twitterConfig,
            // IOptions<YahooConfiguration> yahooConfig,
            // ILogger<YahooHomeController> log,
            // YahooApiEndPoints endPoints
            )
        {
            _yahooAuthController = yahooAuthController;
            _contextAccessor     = contextAccessor;
            _context             = context;
            _theGameConfig       = theGameConfig.Value;
            // _airtableConfig      = airtableConfig.Value;
            // _twitterConfig       = twitterConfig.Value;
            // _yahooConfig         = yahooConfig.Value;
            // _log                 = log;
            // _endPoints           = endPoints;
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


        // STEP 1
        /// <summary> Generates the web request using a yahoo api endpoint. This is followed by the method GetResponseFromServer.static It is ultimately called within GenerateYahooResourceJObject Method </summary>
        /// <param name="uri"></param>
        /// <returns> HttpWebRequest request </returns>
        public HttpWebRequest GenerateWebRequest(string uri)
        {
            // _c.Start.ThisMethod();

            // pull access token from session; this is generated through the yahooauth controller
            // access token is a long mix of letters and numbers;
            string accessToken = _contextAccessor.HttpContext.Session.GetString("accesstoken");

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            // set the authorization header of the request; bearer token plus access token
            request.Headers["Authorization"] = "Bearer " + accessToken;

            // set the method of the request header
            request.Method = "GET";

            // _c.Complete.ThisMethod();
            return request;
        }

        // STEP 2
        /// <summary> Follows GenerateWebRequest method. It receives the response from the request and returns a string of xml.static This is followed by the TranslateServerResponseToXml Method. It is ultimatelly called within GenerateYahooResourceJObject Method</summary>
        /// <param name="request"></param>
        /// <returns> serverResponse string; that looks like Xml </returns>
        public string GetResponseFromServer(HttpWebRequest request)
        {
            // _c.Start.ThisMethod();
            string serverResponse = "";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                serverResponse = reader.ReadToEnd();
            }
            // _c.Complete.ThisMethod();
            // serverResponse comes back as xml in string format
            return serverResponse;
        }

        // STEP 3
        /// <summary> Follows GerResponseFromServer method. Generate xml from serverResponse string. This is called within the GenerateYahooResourceJObject Method</summary>
        /// <param name="serverResponse"></param>
        /// <returns> xml document / content from the called  yahoo api endpoint</returns>
        /// <see cref=\"${GenerateYahooResourceJObject}\"/>
        public XmlDocument TranslateServerResponseToXml (string serverResponse)
        {
            // _c.Start.ThisMethod();
            // DOC type ---> System.Xml.XmlDocument
            XmlDocument doc       = new XmlDocument();
            XmlReader   xmlReader = XmlReader.Create(new System.IO.StringReader(serverResponse));

            doc.LoadXml(serverResponse);
            // doc.Dig();

            // _c.Complete.ThisMethod();
            return doc;
        }

        // STEP 4 --> calls methods of Steps 1, 2, 3
        /// <summary> generate json for any given yahoo resource (e.g., game, league, player, team, etc.)</summary>
        /// <param name="uri"> uri will typically be generated from an endpoint within YahooApiEndPoints.cs</param>
        /// <returns> json / a jobject of the resource type requested</returns>
        public JObject GenerateYahooResourceJObject(string uri)
        {
            // _c.Start.ThisMethod();

            HttpWebRequest request = GenerateWebRequest(uri);

            string serverResponse = GetResponseFromServer(request);

            XmlDocument doc = TranslateServerResponseToXml(serverResponse);

            // convert the xml to json
            string json = JsonConvert.SerializeXmlNode(doc);

            // clean the json up
            JObject resourceJson = JObject.Parse(json);

            // _c.Complete.ThisMethod();
            return resourceJson;
        }

        /// <summary> take in an instance of a yahoo model and save it to the database</summary>
        /// <param name="yahoomodel"></param>
        public void SaveObjectToDatabase(Object yahoomodel)
        {
            _c.Start.ThisMethod();

            _context.Add(yahoomodel);
            _context.SaveChanges();
        }


        [Route("yahoo/leaguescoreboard/create")]
        public void CreateYahooScoreboard ()
        {
            _c.Start.ThisMethod();

            // retrieve the league key from user secrets / yahoo league config
            string leagueKey = _theGameConfig.LeagueKey;

            var uriLeagueScoreboard = endPoints.LeagueSeasonScoreboardEndPoint(leagueKey).EndPointUri;

            JObject leagueScoreboard = GenerateYahooResourceJObject(uriLeagueScoreboard);

            Extensions.PrintJObjectItems(leagueScoreboard);
        }
    }
}