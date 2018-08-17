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

namespace BaseballScraper.Controllers
{
#pragma warning disable CS0414, CS0219
    public class YahooController: Controller
    {
        private const string fanContent = "fantasy_content";
        private const string yT         = "team";
        private static String Start     = "STARTED";
        private static String Complete  = "COMPLETED";
        private BaseballScraperContext _context;
        private readonly AirtableConfiguration _airtableConfig;
        private readonly TwitterConfiguration _twitterConfig;
        private readonly YahooConfiguration _yahooConfig;
        private readonly BaseballScraper.Controllers.YahooAuthController _yahooAuthController;
        private readonly TheGameIsTheGameConfiguration _theGameConfig;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<YahooController> _log;
        private static ApiEndPoints _endPoints;
        private static ApiEndPoints endPoints = new ApiEndPoints();
        public  List<string> messagesList     = new List<string>();
        public YahooController(
            IOptions<AirtableConfiguration> airtableConfig,
            IOptions<TwitterConfiguration> twitterConfig,
            IOptions<YahooConfiguration> yahooConfig,
            YahooAuthController yahooAuthController,
            IHttpContextAccessor contextAccessor,
            BaseballScraperContext context,
            ILogger<YahooController> log,
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


        public int CheckSession()
        {
            int? sessionId = HttpContext.Session.GetInt32("sessionid");

            if(sessionId == null)
            {
                return 0;
            }
            return (int)sessionId;
        }


        [HttpGet]
        [Route("/yahoohome")]
        public IActionResult ViewYahooHomePage()
        {
            var sessionInfoDictionary = CreateSessionInfoDictionary();

            if(CheckSession() == 0)
            {
                Console.WriteLine();
                Console.WriteLine("NEW SESSION IS NEEDED");
                ViewBag.SessionIdExists = " NO session exists";

                // return RedirectToAction("setsessioninfo");
            }
            else
            {
                Console.WriteLine("SESSION ALREADY IN PROGRESS");
                ViewBag.AuthCodeBag     = sessionInfoDictionary["authcode"];
                ViewBag.SessionIdExists = " YES session exists";
                ViewBag.Now             = DateTime.Now;
            }
            return View("yahoohome");
        }

        public Dictionary<string, string> CreateSessionInfoDictionary()
        {
            Dictionary<string, string> sessionInfoDictionary = new Dictionary<string, string>();

            try
            {
                if(CheckSession() == 0)
                {
                    // Console.WriteLine("dictionary created; but nothing to add to it");
                    string consoleMessage = "dictionary created; but nothing to add to it";
                    messagesList.Add(consoleMessage);
                }

                var authCodeCheck     = _contextAccessor.HttpContext.Session.GetString("authorizationcode");
                var accessTokenCheck  = HttpContext.Session.GetString("accesstoken");
                var refreshTokenCheck = HttpContext.Session.GetString("refreshtoken");
                var yahooGuidCheck    = HttpContext.Session.GetString("yahooguid");
                var sessionIdCheck    = HttpContext.Session.GetInt32("sessionid").ToString();

                sessionInfoDictionary.Add("authcode", authCodeCheck);
                sessionInfoDictionary.Add("accesstoken", accessTokenCheck);
                sessionInfoDictionary.Add("refreshtoken", refreshTokenCheck);
                sessionInfoDictionary.Add("yahooguid", yahooGuidCheck);
                sessionInfoDictionary.Add("sessionid", sessionIdCheck);

                bool printDictionaryItems = false;

                if(printDictionaryItems == true)
                {
                    int itemCount = 1;
                    foreach(var item in sessionInfoDictionary)
                    {
                        var setKey = item.Key;
                        Console.WriteLine($"----- #{itemCount} {setKey} ------");
                        Console.WriteLine(item.Value);
                        Console.WriteLine();
                        itemCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine("BASE MESSAGE");
                Console.WriteLine(ex.Message);
                // Console.WriteLine("INNER EXCEPTION");
                // Console.WriteLine(ex.InnerException);
                // Console.WriteLine("--------------------");
                Console.WriteLine();
            }
            return sessionInfoDictionary;
        }





        [HttpGet]
        [Route("/yahoo/team/teambase/queryuri")]
        public string SetUriToQuery()
        {
            Start.ThisMethod();

            var leagueKey = _theGameConfig.LeagueKey;

            int teamId        = 1;
            int weekNumber    = 18;
            int[] weekNumbers = new int[] { 1 , 2, 3};

            // Extensions.Spotlight("Enter Team Number:");
            // string teamNumberToGetString = Console.ReadLine();
            // int    teamId       = Int32.Parse(teamNumberToGetString);

            // this is not working correctly
            // endpoint is showing as "http://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.1/matchups;weeks=System.Int32[]"
            var uriTeamSelectedMatchups = endPoints.TeamSelectedMatchupsEndPoint(leagueKey, teamId, weekNumbers).EndPointUri;

            // these are for paul goldschmidt
            string pKey = "378.p.8967";
            string pId  = "8967";

            return uriTeamSelectedMatchups;
        }


        [Route("/yahoo/team/teambase/listof10")]
        public List<Uri> CreateListOfUrisForAllTeams()
        {
            List<Uri> teamUris = new List<Uri>();

            int numTeamsInLeague = 10;

            for(var teamId = 1; teamId <= numTeamsInLeague; teamId++)
            {
                var uri = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamId}?");
                teamUris.Add(uri);
            }

            return teamUris;
        }


        [Route("/yahoo/execute")]
        public void ExecuteRequestResponseProcess()
        {
            Start.ThisMethod();
            int teamId      = 1;
            var uriTeamBase = endPoints.TeamBaseEndPoint(_theGameConfig.LeagueKey, teamId).EndPointUri;

            GenerateWebRequest(uriTeamBase);
        }

        public void GenerateWebRequest(string uri)
        {
            Start.ThisMethod();

            // pull access token from session
            // access token is a long mix of letters and numbers;
            // this is generated through the yahooauth controller
            string accessToken = _contextAccessor.HttpContext.Session.GetString("accesstoken");

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            // set the authorization header of the request; bearer token plus access token
            request.Headers["Authorization"] = "Bearer " + accessToken;

            // set the method of the request header
            request.Method = "GET";

            GetResponseFromServer(request);
        }


        public void GetResponseFromServer(HttpWebRequest request)
        {
            Start.ThisMethod();

            string serverResponse = "";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                serverResponse = reader.ReadToEnd();
            }
            // serverResponse comes back as xml in string format
            TranslateServerResponseToXml(serverResponse);
        }

        // generate xml from serverResponse string
        public void TranslateServerResponseToXml (string serverResponse)
        {
            Start.ThisMethod();
            // DOC type ---> System.Xml.XmlDocument
            XmlDocument doc       = new XmlDocument();
            XmlReader   xmlReader = XmlReader.Create(new System.IO.StringReader(serverResponse));

            doc.LoadXml(serverResponse);
            doc.Dig();

            GenerateYahooResourceJObject(doc);
        }

        public JObject GenerateYahooResourceJObject(XmlDocument doc)
        {
            Start.ThisMethod();
            // convert the xml to json
            string json = JsonConvert.SerializeXmlNode(doc);

            // clean the json up
            // type ---> Newtonsoft.Json.Linq.JObject
            JObject resourceJson = JObject.Parse(json);
            Extensions.PrintJObjectItems(resourceJson);

            return resourceJson;
        }


        // generate json for any given yahoo resource (e.g., game, league, player, team, etc.)
        [HttpGet]
        [Route("/yahoo/resource/json")]
        public JObject GenerateYahooResourceJson()
        {
            Start.ThisMethod();

            // pull access token from session
            // access token is a long mix of letters and numbers;
            // this is generated through the yahooauth controller
            string accessToken = _contextAccessor.HttpContext.Session.GetString("accesstoken");

            // pull in uri from 'SetUriToQuery' method
            var uri = SetUriToQuery();

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            // set the authorization header of the request; bearer token plus access token
            request.Headers["Authorization"] = "Bearer " + accessToken;

            // set the method of the request header
            request.Method = "GET";

            string serverResponse = "";

            //  RESPONSE TYPE ---> System.Net.HttpWebResponse
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // serverResponse comes back as xml in string format
                serverResponse = reader.ReadToEnd();
            }

            // DOC type ---> System.Xml.XmlDocument
            XmlDocument doc       = new XmlDocument();
            XmlReader   xmlReader = XmlReader.Create(new System.IO.StringReader(serverResponse));

            // generate xml from serverResponse string
            doc.LoadXml(serverResponse);
            doc.Dig();

            // convert the xml to json
            string json = JsonConvert.SerializeXmlNode(doc);

            // clean the json up
            // type ---> Newtonsoft.Json.Linq.JObject
            JObject resourceToJson = JObject.Parse(json);

            return resourceToJson;
        }



        [Route("yahoo/team/stats/model")]
        public YahooTeamStats CreateYahooTeamStatsModel()
        {
            Start.ThisMethod();

            JObject teamStatsJson = GenerateYahooResourceJson();

            YahooTeamStats tS = new YahooTeamStats();

            try {
                tS.Season     = teamStatsJson["fantasy_content"]["team"]["team_stats"]["season"].ToString();
                tS.WeekNumber = "No week number; these numbers are for the full season";
            }
            catch {
                tS.WeekNumber = teamStatsJson["fantasy_content"]["team"]["team_stats"]["week"].ToString();
                tS.Season     = "No season / year; these numbers are for one week";
            }
            finally {
                Console.WriteLine("there is no season OR week; something is broken");
            }

            // tS.CoverageType = teamStatsJson["fantasy_content"]["team"]["team_stats"]["coverage_type"].ToString();

            int currentStatId = 0;

            tS.HitsAtBatsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.RunsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.HomeRunsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.RbiTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.StolenBasesTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.WalksTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.BattingAverageTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.InningsPitchedTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.WinsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.StrikeoutsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.SavesTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.HoldsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.EraTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            tS.WhipTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            // currentStatId++;
            // tS.TeamPoints = teamStatsJson["fantasy_content"]["team"]["team_points"]["total"].ToString();

            tS.Dig();

            // int statsCount = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"];

            Complete.ThisMethod();
            return tS;
        }


        public string CreateYahooTeamBaseModelField(string fieldname)
        {
            return $"resourceJson[fanContent][yT][{fieldname}].ToString()";
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [Route("/yahoo/team/teambase/model")]
        public YahooTeamBase CreateYahooTeamBaseModel ()
        {
            Start.ThisMethod();

            JObject resourceJson = GenerateYahooResourceJson();
            string  pathBase     = $"{resourceJson}['fantasy_content']";

            YahooTeamBase tB = new YahooTeamBase();

            var fieldTest = CreateYahooTeamBaseModelField("team_key");
            fieldTest.Intro("field test");

            // 'fanContent' = 'fantasy_content' within the actual json
            // 'yT' = 'team' within the actual json
            tB.Key                   = resourceJson[fanContent][yT]["team_key"].ToString();
            tB.TeamName              = resourceJson[fanContent][yT]["name"].ToString();
            tB.TeamId                = (int?)resourceJson[fanContent][yT]["team_id"];
            tB.IsOwnedByCurrentLogin = (int?)resourceJson[fanContent][yT]["is_owned_by_current_login"];
            tB.Url                   = resourceJson[fanContent][yT]["url"].ToString();
            tB.WaiverPriority        = (int?)resourceJson[fanContent][yT]["waiver_priority"];
            tB.NumberOfMoves         = (int?)resourceJson[fanContent][yT]["number_of_moves"];
            tB.NumberOfTrades        = (int?)resourceJson[fanContent][yT]["number_of_trades"];
            tB.LeagueScoringType     = resourceJson[fanContent][yT]["league_scoring_type"].ToString();
            tB.HasDraftGrade         = resourceJson[fanContent][yT]["has_draft_grade"].ToString();

            // team logo
            tB.TeamLogo.Size = resourceJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["size"].ToString();
            tB.TeamLogo.Url  = resourceJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["url"].ToString();

            // roster adds
            tB.RosterAdds.CoverageType  = resourceJson["fantasy_content"]["team"]["roster_adds"]["coverage_type"].ToString();
            tB.RosterAdds.CoverageValue = resourceJson["fantasy_content"]["team"]["roster_adds"]["coverage_value"].ToString();
            tB.RosterAdds.Value         = resourceJson["fantasy_content"]["team"]["roster_adds"]["value"].ToString();

            // managers
            tB.TeamManager.ManagerId      = resourceJson["fantasy_content"]["team"]["managers"]["manager"]["manager_id"].ToString();
            tB.TeamManager.NickName       = resourceJson["fantasy_content"]["team"]["managers"]["manager"]["nickname"].ToString();
            tB.TeamManager.Guid           = resourceJson["fantasy_content"]["team"]["managers"]["manager"]["guid"].ToString();
            tB.TeamManager.IsCommissioner = (int?)resourceJson["fantasy_content"]["team"]["managers"]["manager"]["is_commissioner"];
            tB.TeamManager.IsCurrentLogin = (int?)resourceJson["fantasy_content"]["team"]["managers"]["manager"]["is_current_login"];

            try
            {
                tB.TeamManager.Email = resourceJson["fantasy_content"]["team"]["managers"]["manager"]["email"].ToString();
            }
            catch (System.Exception ex)
            {
                tB.TeamManager.Email = "hidden";
                Console.WriteLine($"Email is hidden ---> {ex.Message}");
            }


            tB.TeamManager.ImageUrl = resourceJson["fantasy_content"]["team"]["managers"]["manager"]["image_url"].ToString();

            tB.Dig();

            SaveObjectToDatabase(tB);

            Complete.ThisMethod();

            return tB;
        }


        /// <summary>
        /// take in an instance of a yahoo model and save it to the database
        /// </summary>
        /// <param name="yahoomodel"></param>
        public void SaveObjectToDatabase(Object yahoomodel)
        {
            Start.ThisMethod();

            _context.Add(yahoomodel);
            _context.SaveChanges();
        }




        [Route("/yahoo/team/teambase/hashtable")]
        public Hashtable CreateYahooTeamBaseHashTable ()
        {
            Start.ThisMethod();

            JObject resourceJson = GenerateYahooResourceJson();

            Hashtable teamHashTable = new Hashtable();
                teamHashTable.Add("Key", resourceJson["fantasy_content"]["team"]["team_key"].ToString());
                teamHashTable.Add("TeamId", (int?)resourceJson["fantasy_content"]["team"]["team_id"]);
                teamHashTable.Add("Name", resourceJson["fantasy_content"]["team"]["name"].ToString());
                teamHashTable.Add("Is Owned By Current Login?", (int?)resourceJson["fantasy_content"]["team"]["is_owned_by_current_login"]);
                teamHashTable.Add("Url", resourceJson["fantasy_content"]["team"]["url"].ToString());
                teamHashTable.Add("Team Logo", resourceJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["url"].ToString());
                teamHashTable.Add("Waiver Priority", (int?)resourceJson["fantasy_content"]["team"]["waiver_priority"]);
                teamHashTable.Add("Number of Moves", (int?)resourceJson["fantasy_content"]["team"]["number_of_moves"]);
                teamHashTable.Add("Number of Trades", (int?)resourceJson["fantasy_content"]["team"]["number_of_trades"]);
                teamHashTable.Add("Coverage Type", resourceJson["fantasy_content"]["team"]["roster_adds"]["coverage_type"].ToString());
                teamHashTable.Add("Coverage Value", resourceJson["fantasy_content"]["team"]["roster_adds"]["coverage_value"].ToString());
                teamHashTable.Add("Value", resourceJson["fantasy_content"]["team"]["roster_adds"]["value"].ToString());
                teamHashTable.Add("League Scoring Type", resourceJson["fantasy_content"]["team"]["league_scoring_type"].ToString());
                teamHashTable.Add("Has Draft Grade?", resourceJson["fantasy_content"]["team"]["has_draft_grade"].ToString());

                teamHashTable.Add("Manager Id", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["manager_id"].ToString());
                teamHashTable.Add("NickName", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["nickname"].ToString());
                teamHashTable.Add("Guid", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["guid"].ToString());
                teamHashTable.Add("Is Commish?", (int?)resourceJson["fantasy_content"]["team"]["managers"]["manager"]["is_commissioner"]);
                teamHashTable.Add("Is Current Login?", (int?)resourceJson["fantasy_content"]["team"]["managers"]["manager"]["is_current_login"]);
                teamHashTable.Add("Email", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["email"].ToString());
                teamHashTable.Add("Image Url", resourceJson["fantasy_content"]["team"]["managers"]["manager"]["image_url"].ToString());

            IDictionaryEnumerator _enumerator = teamHashTable.GetEnumerator();

            int _enumeratorCount = 1;

            while (_enumerator.MoveNext())
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(_enumerator.Key.ToString());
                Console.ResetColor();

                if(_enumerator.Value == null)
                {
                    Console.WriteLine("null");
                }

                else
                {
                    Console.WriteLine(_enumerator.Value.ToString());
                }
                Console.WriteLine();
                _enumeratorCount++;
            }

            Complete.ThisMethod();
            return teamHashTable;
        }


        // internal static async Task<T> GetResource<T>(EndPoint endPoint, string AccessToken, string lookup)
        // {
        //     return await await ResilientCall(async () =>
        //     {
        //         var           xml        = await Utils.GetResponseData(endPoint, AccessToken);
        //         XmlSerializer serializer = new XmlSerializer(typeof(T));
        //         XElement      xElement   = xml.Descendants(YahooXml.XMLNS + lookup).FirstOrDefault();
        //         if (xElement == null && IsError(xml))
        //             throw new InvalidOperationException(GetErrorMessage(xml));
        //         if (xElement == null)
        //             throw new InvalidOperationException($"Invalid XML returned. {xml}");

        //         var resource = (T)serializer.Deserialize(xElement.CreateReader());
        //         return resource;
        //     });
        // }

        // async static Task<T> ResilientCall<T>(Func<T> block)
        // {
        //     int      currentRetry = 0;
        //     TimeSpan delay        = TimeSpan.FromSeconds(2);

        //     for (; ; )
        //     {
        //         try
        //         {
        //             return block();
        //         }
        //         catch (Exception ex)
        //         {
        //             Trace.TraceError("Operation Exception");

        //             currentRetry++;

        //             // Check if the exception thrown was a transient exception
        //             // based on the logic in the error detection strategy.
        //             // Determine whether to retry the operation, as well as how
        //             // long to wait, based on the retry strategy.
        //             if (currentRetry > 3 || !IsTransient(ex))
        //             {
        //                 // If this isn't a transient error or we shouldn't retry,
        //                 // rethrow the exception.
        //                 throw;
        //             }
        //         }

        //         // Wait to retry the operation.
        //         // Consider calculating an exponential delay here and
        //         // using a strategy best suited for the operation and fault.
        //         await Task.Delay(delay);
        //     }
        // }


        // private static bool IsTransient(Exception ex)
        // {
        //     var webException = ex as WebException;
        //     if (webException != null)
        //     {
        //         // If the web exception contains one of the following status values
        //         // it might be transient.
        //         return new[] {WebExceptionStatus.ConnectionClosed,
        //           WebExceptionStatus.Timeout,
        //           WebExceptionStatus.RequestCanceled }.
        //                 Contains(webException.Status);
        //     }

        //     // Additional exception checking logic goes here.
        //     return false;
        // }

        // private static bool IsError(XDocument xml)
        // {
        //     return string.Equals(xml.Root.Name.LocalName, "error", StringComparison.OrdinalIgnoreCase);
        // }


        // private static string GetErrorMessage(XDocument xml)
        // {
        //     var result =
        //         from e in xml.Root.Elements()
        //         where e.Name.LocalName == "description"
        //         select e.Value;

        //     return result.FirstOrDefault() ?? "Unknown XML";
        // }



    }
}



















        // //THIS WORKS
        // [HttpGet]
        // [Route("/yahoo/authorize/accesstokenjobject")]
        // public JObject CreateYahooAccessTokenResponseJObject()
        // {
        //     Start.ThisMethod();

        //     var consumerKey    = _yahooConfig.ClientId;
        //     var consumerSecret = _yahooConfig.ClientSecret;
        //     var redirectUri    = _yahooConfig.RedirectUri;

        //     var authorizationCodeFromConsole = GetUserAuthorizationCode();
        //     authorizationCodeFromConsole.Intro("auth code");

        //     // Exchange authorization code for Access Token by sending Post Request
        //     Uri address = new Uri("https://api.login.yahoo.com/oauth2/get_token");

        //     // Create the web request ___ REQUEST ---> System.Net.HttpWebRequest
        //     HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

        //     // Set type to POST ---> changes 'Method' of HttpWebRequest to 'POST'
        //     request.Method = "POST";

        //     // changes 'Content Type' of HttpWebRequest to 'application/x-www-form-urlencoded'
        //     request.ContentType = "application/x-www-form-urlencoded";

        //     // produces very Base64Encoded version of consumerKey and yahooClientSecrent(it's long string of letters and numbers) ___ HEADER BYTE ---> System.Byte[] ___ e.g., '"ZGoweUpt...etc."'
        //     byte[] headerByte = System.Text.Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);

        //     // converts 'headerByte'; same letters and numbers but new format ___ HEADER STRING ---> System.String ___ e.g., 'ZGoweUpt...etc.'
        //     string headerString = System.Convert.ToBase64String(headerByte);

        //     // returns two lines
        //         // Content-Type: application/x-www-form-urlencoded
        //         // 'Authorization: Basic <headerString> ___ e.g., <headerString> = ZGoweUpt...etc.
        //     request.Headers["Authorization"] = "Basic " + headerString;

        //     // Create the data we want to send ___ DATA ---> System.Text.StringBuilder ___ 'data' is a concatanated string of all of the data.Append items
        //     StringBuilder data = new StringBuilder();
        //         data.Append("?client_id=" + consumerKey);
        //         data.Append("&client_secret=" + consumerSecret);
        //         data.Append("&redirect_uri=" + redirectUri);
        //         data.Append("&code=" + authorizationCodeFromConsole);
        //         data.Append("&grant_type=authorization_code");

        //     // Create a byte array of the data we want to send ___ BYTE DATA ---> System.Byte[]
        //     byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

        //     // Set the content length in the request headers ___ // changes 'Content Length' of HttpWebRequest to 218
        //     request.ContentLength = byteData.Length;

        //     // Write data ___ POST STREAM ---> System.Net.RequestStream
        //     using (Stream postStream = request.GetRequestStream())
        //     {
        //         postStream.Write(byteData, 0, byteData.Length);
        //     }

        //     // Get response
        //     string serverResponse = "";

        //     try
        //     {
        //         // changes 'HaveResponse' of HttpWebRequest to 228
        //         using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        //         {
        //             // Get the response stream ___ READER ---> System.IO.StreamReader
        //             StreamReader reader = new StreamReader(response.GetResponseStream());

        //             serverResponse = reader.ReadToEnd();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex.Message); // if error ---> 'The remote server returned an error: (400) Bad Request.'
        //         ex.SetContext("response from server", serverResponse);
        //     }

        //     // RESPONSE TO JSON ---> Newtonsoft.Json.Linq.JObject
        //     JObject resourceJson = JObject.Parse(serverResponse);

        //     // PrintJObjectItems(resourceJson);

        //     // resourceJson.Dig();

        //     // REQUEST ---> following are added through beginning to end: ContentLength, HaveResponse, Method, ContentType
        //     Complete.ThisMethod();
        //     return resourceJson;
        // }








// this works - do I need it?
// [HttpGet]
// [Route("/yahoo/authorizationcode")]
// public IActionResult GetAuthorizationCode()
// {
//     Start.ThisMethod();

//     string requestUrl = $"https://api.login.yahoo.com/oauth2/request_auth?client_id={_yahooConfig.ClientId}&redirect_uri={_yahooConfig.RedirectUri}&response_type=code&language=en-us";

//     Process.Start("open", requestUrl);

//     requestUrl.Intro("request url");

//     // REQUEST ---> System.Net.HttpWebRequest
//     WebRequest request = WebRequest.Create(requestUrl);
//     // request.Dig();

//     // REQUEST.CREDENTIALS ---> System.Net.SystemNetworkCredential
//     request.Credentials = CredentialCache.DefaultCredentials;

//     // RESPONSE ---> System.Net.HttpWebResponse
//     WebResponse response = request.GetResponse();

//     response.ResponseUri.Intro("response uri");
//     response.ResponseUri.Authority.Intro("authority");
//     response.ResponseUri.Host.Intro("host");
//     response.ResponseUri.LocalPath.Intro("local path");
//     response.ResponseUri.UserInfo.Intro("user info");
//     response.ResponseUri.OriginalString.Intro("original string");
//     response.ResponseUri.PathAndQuery.Intro("path and query");
//     response.ResponseUri.Scheme.Intro("scheme");
//     response.ResponseUri.Segments.Intro("segments");

//     response.Headers[3].Intro("cookie");

//     response.Headers[4].Intro("age");                   // AGE ---> 0
//     response.Headers[10].Intro("date");                 // DATE ---> e.g., 'Tue, 07 Aug 2018 23:03:19 GMT'
//     response.Headers[11].Intro("connection");           // CONNECTION ---> close
//     response.Headers[14].Intro("expires");              // EXPIRES ---> 0
//     response.Headers[15].Intro("content-type");         // CONTENT-TYPE ---> text/html; charset=utf-8
//     response.Headers[16].Intro("content-length");       // CONTENT-LENGTH ---> 100765

//     // returns as 'OK'
//     Console.WriteLine (((HttpWebResponse)response).StatusDescription);

//     // response.Dig();

//     Stream       postStream         = response.GetResponseStream();
//     StreamReader reader             = new StreamReader(postStream);
//     string       serverResponse = reader.ReadToEnd();
//     // Console.WriteLine(serverResponse);

//     reader.Close();
//     response.Close();

//     Complete.ThisMethod();
//     return Content(serverResponse);
// }



// this works. do i need it?
// [HttpGet]
// [Route("/yahoo/requestauth")]
// public Dictionary<string, string> GetYahooRequestAuthConfiguration()
// {
//     Start.ThisMethod();

//     Dictionary<string,string> requestAuthItemsList = new Dictionary<string,string>();

//     var yahooConfigName = _yahooConfig.Name;
//     var yahooAppId      = _yahooConfig.AppId;
//     var consumerKey     = _yahooConfig.ClientId;
//     var consumerSecret  = _yahooConfig.ClientSecret;
//     var redirectUri     = _yahooConfig.RedirectUri;

//     requestAuthItemsList.Add("Name", yahooConfigName);
//     requestAuthItemsList.Add("App Id", yahooAppId);
//     requestAuthItemsList.Add("Client Id", consumerKey);
//     requestAuthItemsList.Add("Client Secret", consumerSecret);
//     requestAuthItemsList.Add("Redirect Uri", redirectUri);

//     foreach(var item in requestAuthItemsList)
//     {
//         Console.ForegroundColor = ConsoleColor.DarkMagenta;
//         Console.WriteLine(item.Key);
//         Console.ResetColor();
//         Console.WriteLine(item.Value);
//         Console.WriteLine();
//     }

//     return requestAuthItemsList;
// }




        // [HttpGet]
        // [Route("/yahoo/team/printxml")]
        // public IActionResult PrintXml()
        // {
        //     Start.ThisMethod();

        //     XStreamingElement root = new XStreamingElement("Root",
        //         from el in CreateTeamBaseXml("Source.xml")
        //         select new XElement("Item",
        //             new XElement("Customer", (string)el.Parent.Element("Name")),
        //             new XElement(el.Element("Key"))
        //         )
        //     );

        //     root.Save("Test.xml");
        //     // Console.WriteLine(System.IO.File.ReadAllText("Test.xml"));

        //     Complete.ThisMethod();

        //     return Content($"String is");
        // }





        // [HttpGet]
        // [Route("/yahoo/team/createxml")]
        // public static IEnumerable<XElement> CreateTeamBaseXml(string response)
        // {
        //     Start.ThisMethod();

        //     // response ---> this is a string of the xml
        //     response.Intro("response");

        //     // intro DOC ---> System.Xml.XmlDocument
        //     XmlDocument doc = new XmlDocument();

        //     // intro XMLREADER ---> System.Xml.XmlTextReaderImpl
        //     XmlReader xmlReader = XmlReader.Create(new System.IO.StringReader(response));

        //     using(XmlReader reader = XmlReader.Create(response))
        //     {
        //         XElement name = null;
        //         XElement item = null;

        //         reader.MoveToContent();

        //         // MARK A
        //         while (reader.Read())
        //         {
        //             if (reader.NodeType == XmlNodeType.Element && reader.Name == "Customer")
        //             {
        //                 // move to Name element
        //                 while (reader.Read())
        //                 {
        //                     if (reader.NodeType == XmlNodeType.Element && reader.Name == "Name")
        //                     {
        //                         name = XElement.ReadFrom(reader) as XElement;
        //                         break;
        //                     }
        //                 }

        //                 // loop through Item elements
        //                 while (reader.Read())
        //                 {
        //                     // nodetypes both equal 'EndElement'
        //                     if (reader.NodeType == XmlNodeType.EndElement)
        //                     {
        //                         break;
        //                     }

        //                     if (reader.NodeType == XmlNodeType.Element && reader.Name == "Item")
        //                     {
        //                         item = XElement.ReadFrom(reader) as XElement;

        //                         if (item != null)
        //                         {
        //                             XElement tempRoot = new XElement("Root",
        //                                 new XElement(name)
        //                             );

        //                             tempRoot.Add(item);
        //                             yield return item;
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //     }

        //     // doc.Dig();
        //     // xmlReader.Dig();
        //     // response.Dig();

        //     Complete.ThisMethod();
        // }
