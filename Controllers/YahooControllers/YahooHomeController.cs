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
using BaseballScraper.Infrastructure;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooHomeController: ControllerBase
    {
        private readonly Helpers _h = new Helpers();

        // private readonly YahooConfiguration _yahooConfig;
        private readonly BaseballScraperContext _context;
        private static YahooApiEndPoints endPoints = new YahooApiEndPoints();
        private readonly BaseballScraper.Controllers.YahooControllers.YahooAuthController _yahooAuthController;
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private readonly IHttpContextAccessor _contextAccessor;

        // private static readonly IOptions<YahooConfiguration> _yOptions;
        // private static readonly IHttpContextAccessor _cAccessor;

        // private readonly YahooAuthController _yAuth = new YahooAuthController(_yOptions, _cAccessor);

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
        [Route("")]
        public void ViewYahooHomePage()
        {
            _h.StartMethod();

            var initialAuthCodeCheck = HttpContext.Session.GetString("authorizationcode");
            Console.WriteLine($"HOME ViewYahooHomePage > initialAuthCodeCheck: {initialAuthCodeCheck} ");

            // _yahooAuthController.CheckYahooSession();

            // var aTR = _yahooAuthController.GetYahooAccessTokenResponse();
            // Console.WriteLine(aTR.AccessToken);
            // Console.WriteLine();

            CreateYahooScoreboard();
            // return Content(currently);
        }


        // STEP 1
        /// <summary> Generates the web request using a yahoo api endpoint. This is followed by the method GetResponseFromServer.static It is ultimately called within GenerateYahooResourceJObject Method </summary>
        /// <param name="uri"></param>
        /// <returns> HttpWebRequest request </returns>
        public HttpWebRequest GenerateWebRequest(string uri)
        {
            _h.StartMethod();
            Console.WriteLine($"uri: {uri}");
            Console.WriteLine();

            // pull access token from session; this is generated through the yahooauth controller
            // access token is a long mix of letters and numbers;
            string accessToken = _contextAccessor.HttpContext.Session.GetString("accesstoken");
            Console.WriteLine($"HOME GenerateWebRequest > accessToken: {accessToken}");
            Console.WriteLine();

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            // set the authorization header of the request; bearer token plus access token
            request.Headers["Authorization"] = "Bearer " + accessToken;

            // set the method of the request header
            request.Method = "GET";

            _h.CompleteMethod();
            return request;
        }

        // STEP 2
        /// <summary> Follows GenerateWebRequest method. It receives the response from the request and returns a string of xml.static This is followed by the TranslateServerResponseToXml Method. It is ultimatelly called within GenerateYahooResourceJObject Method</summary>
        /// <param name="request"></param>
        /// <returns> serverResponse string; that looks like Xml </returns>
        public string GetResponseFromServer(HttpWebRequest request)
        {
            _h.StartMethod();
            string serverResponse = "";
            Console.WriteLine($"HOME GetResponseFromServer > request: {request}");
            Console.WriteLine("---");
            Console.WriteLine($"{request.Credentials}");
            Console.WriteLine("---");

            Console.WriteLine($"{request.Headers}");
            Console.WriteLine("---");
            Console.WriteLine();

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                serverResponse = reader.ReadToEnd();
            }
            _h.CompleteMethod();
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
            _h.StartMethod();
            // DOC type ---> System.Xml.XmlDocument
            XmlDocument doc       = new XmlDocument();
            XmlReader   xmlReader = XmlReader.Create(new System.IO.StringReader(serverResponse));

            doc.LoadXml(serverResponse);
            // doc.Dig();

            _h.CompleteMethod();
            return doc;
        }

        // STEP 4 --> calls methods of Steps 1, 2, 3
        /// <summary> generate json for any given yahoo resource (e.g., game, league, player, team, etc.)</summary>
        /// <param name="uri"> uri will typically be generated from an endpoint within YahooApiEndPoints.cs</param>
        /// <returns> json / a jobject of the resource type requested</returns>
        public JObject GenerateYahooResourceJObject(string uri)
        {
            _h.StartMethod();
            Console.WriteLine($"HOME GenerateYahooResourceJObject > uri: {uri}");

            HttpWebRequest request = GenerateWebRequest(uri);
            Console.WriteLine($"HOME GenerateYahooResourceJObject > request: {request}");
            Console.WriteLine();

            string serverResponse = GetResponseFromServer(request);

            XmlDocument doc = TranslateServerResponseToXml(serverResponse);

            // convert the xml to json
            string json = JsonConvert.SerializeXmlNode(doc);
            Console.WriteLine("json");
            Console.WriteLine(json);
            Console.WriteLine();

            // clean the json up
            JObject resourceJson = JObject.Parse(json);

            _h.CompleteMethod();
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



        public void CreateYahooScoreboard ()
        {
            _h.StartMethod();

            // retrieve the league key from user secrets / yahoo league config
            string leagueKey = _theGameConfig.LeagueKey;
            Console.WriteLine($"leagueKey: {leagueKey}");

            var uriLeagueScoreboard = endPoints.LeagueSeasonScoreboardEndPoint(leagueKey).EndPointUri;
            Console.WriteLine($"HOME CreateYahooScoreboard > uriLeagueScoreboard: {uriLeagueScoreboard}");

            JObject leagueScoreboard = GenerateYahooResourceJObject(uriLeagueScoreboard);

            _h.PrintJObjectItems(leagueScoreboard);
        }
    }
}
