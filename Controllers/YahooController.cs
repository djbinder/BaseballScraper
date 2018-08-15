using System;
using System.IO;
using System.Net;
using System.Xml;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BaseballScraper.Models.Yahoo;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace BaseballScraper.Controllers
{
#pragma warning disable CS0414, CS0219
    public class YahooController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        private readonly AirtableConfiguration _airtableConfig;
        private readonly TwitterConfiguration _twitterConfig;
        private readonly YahooConfiguration _yahooConfig;
        private readonly BaseballScraper.Controllers.YahooAuthController _yahooAuthController;

        private readonly IHttpContextAccessor _contextAccessor;



        public YahooController(IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig, IOptions<YahooConfiguration> yahooConfig, YahooAuthController yahooAuthController, IHttpContextAccessor contextAccessor)
        {
            _airtableConfig      = airtableConfig.Value;
            _twitterConfig       = twitterConfig.Value;
            _yahooConfig         = yahooConfig.Value;
            _yahooAuthController = yahooAuthController;
            _contextAccessor     = contextAccessor;
        }

        // public string GetDataFromSession()
        // {
        //     var authCode = _contextAccessor.HttpContext.Session.GetString("authorizationcode");
        //     authCode.Intro("auth code in main Y controller");

        //     return authCode;
        // }


        [HttpGet]
        [Route("/yahoohome")]
        public IActionResult ViewYahooHomePage()
        {
            Start.ThisMethod();

            try
            {
                // _yahooAuthController.CheckYahooSession();
                var authCodeCheck     = _contextAccessor.HttpContext.Session.GetString("authorizationcode");
                var accessTokenCheck  = HttpContext.Session.GetString("accesstoken");
                var refreshTokenCheck = HttpContext.Session.GetString("refreshtoken");
                var yahooGuidCheck    = HttpContext.Session.GetString("yahooguid");

                Dictionary<string, string> sessionInfoDictionary = new Dictionary<string, string>();
                    sessionInfoDictionary.Add("authcode", authCodeCheck);
                    sessionInfoDictionary.Add("accesstoken", accessTokenCheck);
                    sessionInfoDictionary.Add("refreshtoken", refreshTokenCheck);
                    sessionInfoDictionary.Add("yahooguid", yahooGuidCheck);

                int itemCount = 1;
                foreach(var item in sessionInfoDictionary)
                {
                    Console.WriteLine($"----- SESSION ITEM {itemCount} -------------------------------------------");
                    Console.WriteLine(item.Key);
                    Console.WriteLine(item.Value);
                    Console.WriteLine();

                    itemCount++;
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine("BASE MESSAGE");
                Console.WriteLine(ex.Message);
                Console.WriteLine("INNER EXCEPTION");
                Console.WriteLine(ex.InnerException);
                Console.WriteLine("--------------------");
                Console.WriteLine();
            }



            return View("yahoohome");
        }


        [HttpGet]
        [Route("/yahoo/team/teambase/queryuri")]
        public Uri SetUriToQuery()
        {
            Start.ThisMethod();

            int teamNumberToGet = 1;
            int weekNumber      = 18;

            // for just teambase
            var uriTeamBase = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}?");

            // for stats
            Uri uriStatsA = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/stats;type=week;week=2");
            var uriStatsB = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/stats");
            var uriStatsC = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/stats;type=week;week={weekNumber}");

            // for standings
            var uriStandings = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/standings");

            // roster
            var uriRoster = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/roster;week={weekNumber}");


            // draft results
            var uriDraftResults = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/draftresults");

            // matchups
            var uriMatchups = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/matchups;weeks=1,3,6");

            // Complete.ThisMethod();

            return uriTeamBase;
        }


        [Route("/yahoo/team/teambase/listof10")]
        public List<Uri> CreateListOfUrisForAllTeams()
        {
            Start.ThisMethod();

            HttpContext.Session.SetInt32("djb2", 2);
            var djb = HttpContext.Session.GetInt32("djb2");
            djb.Intro("djb2");

            TempData["Id"] = 12;
            Console.WriteLine(TempData["Id"]);

            _yahooAuthController.CheckYahooSession();

            bool NeedToLoop = true;

            List<Uri> teamUris = new List<Uri>();

            int numTeamsInLeague = 10;

            for(var teamNumberToGet = 1; teamNumberToGet <= numTeamsInLeague; teamNumberToGet++)
            {
                teamNumberToGet.Intro("team number");
                // for just teambase
                var uri = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}?");
                Console.WriteLine(uri);

                teamUris.Add(uri);
            }

            Console.WriteLine(teamUris.Count);

            Complete.ThisMethod();

            return teamUris;
        }





        [HttpGet]
        [Route("/yahoo/team/teambase/json")]
        public JObject CreateTeamBaseJObject()
        {
            Start.ThisMethod();

            string initialAccessTokenCheck = _contextAccessor.HttpContext.Session.GetString("accesstoken");
            initialAccessTokenCheck.Intro("initial check");

            // returns Access Token, Token Type(i.e., bearer), Expires In (i.e., 3600), Refresh Token (unique), and XOAuth Yahoo Guid(unique)
            // JObject accessTokenResponseJObject = _yahooAuthController.CreateYahooAccessTokenResponseJObject();

            // call the GetYahooAccessTokenResponse method
                // 1. web browser will launch and take user to ---> https://api.login.yahoo.com/oauth2/request_auth?...
                // 2. click "Agree" in browser and a 7 digit code will appear; this code is the "&code=xyz" part of request_auth
                // 3. terminal will be asking for this code; copy and paste from browser and hit enter
                // 4. rest of the method runs
            // AccessTokenResponse accessTokenResponse = GetYahooAccessTokenResponse(accessTokenResponseJObject);
            // returns Access Token, Token Type(i.e., bearer), Expires In (i.e., 3600), Refresh Token (unique), and XOAuth Yahoo Guid(unique)
            // AccessTokenResponse accessTokenResponse = _yahooAuthController.GetYahooAccessTokenResponse();
            // AccessTokenResponse accessTokenResponse = _yahooAuthController.GetYahooAccessTokenResponse(accessTokenResponseJObject);

            // access token is a long mix of letters and numbers;
            // string accessToken      = accessTokenResponse.AccessToken;
            string accessToken = _contextAccessor.HttpContext.Session.GetString("accesstoken");

            // if(accessToken == accessTokenCheck)
            // {
            //     Console.WriteLine("this is GOOOD!!!!!!!");
            // }

            // else {
            //     Console.WriteLine("this is BAD");
            //     accessToken.Intro("access token");
            //     accessTokenCheck.Intro("access token check");
            // }

            // pull in uri from 'SetUriToQuery' method
            var uri = SetUriToQuery();

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            request.Headers["Authorization"] = "Bearer " + accessToken;

            request.Method = "GET";

            string responseFromServer = "";

            //  RESPONSE TYPE ---> System.Net.HttpWebResponse
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                responseFromServer = reader.ReadToEnd();
            }

            // DOC type ---> System.Xml.XmlDocument
            XmlDocument doc       = new XmlDocument();
            XmlReader   xmlReader = XmlReader.Create(new System.IO.StringReader(responseFromServer));

            // responseFromServer comes back as xml in string format
            doc.LoadXml(responseFromServer);
            doc.Dig();

            // convert the xml to json
            string json = JsonConvert.SerializeXmlNode(doc);

            // clean the json up
            // type ---> Newtonsoft.Json.Linq.JObject
            JObject responseToJson = JObject.Parse(json);
            // Extensions.PrintJObjectItems(responseToJson);

            Complete.ThisMethod();
            return responseToJson;
        }







        [Route("yahoo/team/testing")]
        public void YahooTeamTesting()
        {
            Start.ThisMethod();

            JObject teamStatsJson = CreateTeamBaseJObject();

            Extensions.PrintJObjectItems(teamStatsJson);

            Complete.ThisMethod();
        }




        [Route("yahoo/team/stats/model")]
        public YahooTeamStats CreateYahooTeamStatsModel()
        {
            Start.ThisMethod();

            JObject teamStatsJson = CreateTeamBaseJObject();

            YahooTeamStats teamStats = new YahooTeamStats();

            try {
                teamStats.Season     = teamStatsJson["fantasy_content"]["team"]["team_stats"]["season"].ToString();
                teamStats.WeekNumber = "No week number; these numbers are for the full season";
            }
            catch {
                teamStats.WeekNumber = teamStatsJson["fantasy_content"]["team"]["team_stats"]["week"].ToString();
                teamStats.Season     = "No season / year; these numbers are for one week";
            }
            finally {
                Console.WriteLine("there is no season OR week; something is broken");
            }

            // teamStats.CoverageType = teamStatsJson["fantasy_content"]["team"]["team_stats"]["coverage_type"].ToString();

            int currentStatId = 0;

            teamStats.HitsAtBatsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.RunsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.HomeRunsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.RbiTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.StolenBasesTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.WalksTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.BattingAverageTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.InningsPitchedTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.WinsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.StrikeoutsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.SavesTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.HoldsTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.EraTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            currentStatId++;
            teamStats.WhipTotal = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"][currentStatId]["value"].ToString();

            // currentStatId++;
            // teamStats.TeamPoints = teamStatsJson["fantasy_content"]["team"]["team_points"]["total"].ToString();

            teamStats.Dig();

            // int statsCount = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"];

            Complete.ThisMethod();
            return teamStats;
        }



        [Route("/yahoo/team/teambase/model")]
        public YahooTeamBase CreateYahooTeamBaseModel ()
        {
            Start.ThisMethod();

            JObject responseToJson = CreateTeamBaseJObject();

            YahooTeamBase teamBase = new YahooTeamBase();

            Extensions.Spotlight("trying to build teambase");
            teamBase.Key                   = responseToJson["fantasy_content"]["team"]["team_key"].ToString();
            teamBase.TeamName              = responseToJson["fantasy_content"]["team"]["name"].ToString();
            teamBase.TeamId                = (int?)responseToJson["fantasy_content"]["team"]["team_id"];
            teamBase.IsOwnedByCurrentLogin = (int?)responseToJson["fantasy_content"]["team"]["is_owned_by_current_login"];
            teamBase.Url                   = responseToJson["fantasy_content"]["team"]["url"].ToString();
            teamBase.WaiverPriority        = (int?)responseToJson["fantasy_content"]["team"]["waiver_priority"];
            teamBase.NumberOfMoves         = (int?)responseToJson["fantasy_content"]["team"]["number_of_moves"];
            teamBase.NumberOfTrades        = (int?)responseToJson["fantasy_content"]["team"]["number_of_trades"];
            teamBase.LeagueScoringType     = responseToJson["fantasy_content"]["team"]["league_scoring_type"].ToString();
            teamBase.HasDraftGrade         = responseToJson["fantasy_content"]["team"]["has_draft_grade"].ToString();

            // team logo
            teamBase.TeamLogo.Size = responseToJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["size"].ToString();
            teamBase.TeamLogo.Url  = responseToJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["url"].ToString();

            // roster adds
            teamBase.RosterAdds.CoverageType  = responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_type"].ToString();
            teamBase.RosterAdds.CoverageValue = responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_value"].ToString();
            teamBase.RosterAdds.Value         = responseToJson["fantasy_content"]["team"]["roster_adds"]["value"].ToString();

            // managers
            teamBase.TeamManager.ManagerId      = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["manager_id"].ToString();
            teamBase.TeamManager.NickName       = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["nickname"].ToString();
            teamBase.TeamManager.Guid           = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["guid"].ToString();
            teamBase.TeamManager.IsCommissioner = (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_commissioner"];
            teamBase.TeamManager.IsCurrentLogin = (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_current_login"];
            teamBase.TeamManager.Email          = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["email"].ToString();
            teamBase.TeamManager.ImageUrl       = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["image_url"].ToString();

            teamBase.Dig();

            Complete.ThisMethod();

            return teamBase;
        }




        [Route("/yahoo/team/teambase/hashtable")]
        public Hashtable CreateYahooTeamBaseHashTable ()
        {
            Start.ThisMethod();

            JObject responseToJson = CreateTeamBaseJObject();

            Hashtable teamHashTable = new Hashtable();
                teamHashTable.Add("Key", responseToJson["fantasy_content"]["team"]["team_key"].ToString());
                teamHashTable.Add("TeamId", (int?)responseToJson["fantasy_content"]["team"]["team_id"]);
                teamHashTable.Add("Name", responseToJson["fantasy_content"]["team"]["name"].ToString());
                teamHashTable.Add("Is Owned By Current Login?", (int?)responseToJson["fantasy_content"]["team"]["is_owned_by_current_login"]);
                teamHashTable.Add("Url", responseToJson["fantasy_content"]["team"]["url"].ToString());
                teamHashTable.Add("Team Logo", responseToJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["url"].ToString());
                teamHashTable.Add("Waiver Priority", (int?)responseToJson["fantasy_content"]["team"]["waiver_priority"]);
                teamHashTable.Add("Number of Moves", (int?)responseToJson["fantasy_content"]["team"]["number_of_moves"]);
                teamHashTable.Add("Number of Trades", (int?)responseToJson["fantasy_content"]["team"]["number_of_trades"]);
                teamHashTable.Add("Coverage Type", responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_type"].ToString());
                teamHashTable.Add("Coverage Value", responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_value"].ToString());
                teamHashTable.Add("Value", responseToJson["fantasy_content"]["team"]["roster_adds"]["value"].ToString());
                teamHashTable.Add("League Scoring Type", responseToJson["fantasy_content"]["team"]["league_scoring_type"].ToString());
                teamHashTable.Add("Has Draft Grade?", responseToJson["fantasy_content"]["team"]["has_draft_grade"].ToString());

                teamHashTable.Add("Manager Id", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["manager_id"].ToString());
                teamHashTable.Add("NickName", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["nickname"].ToString());
                teamHashTable.Add("Guid", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["guid"].ToString());
                teamHashTable.Add("Is Commish?", (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_commissioner"]);
                teamHashTable.Add("Is Current Login?", (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_current_login"]);
                teamHashTable.Add("Email", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["email"].ToString());
                teamHashTable.Add("Image Url", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["image_url"].ToString());

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
        //     string responseFromServer = "";

        //     try
        //     {
        //         // changes 'HaveResponse' of HttpWebRequest to 228
        //         using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        //         {
        //             // Get the response stream ___ READER ---> System.IO.StreamReader
        //             StreamReader reader = new StreamReader(response.GetResponseStream());

        //             responseFromServer = reader.ReadToEnd();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex.Message); // if error ---> 'The remote server returned an error: (400) Bad Request.'
        //         ex.SetContext("response from server", responseFromServer);
        //     }

        //     // RESPONSE TO JSON ---> Newtonsoft.Json.Linq.JObject
        //     JObject responseToJson = JObject.Parse(responseFromServer);

        //     // PrintJObjectItems(responseToJson);

        //     // responseToJson.Dig();

        //     // REQUEST ---> following are added through beginning to end: ContentLength, HaveResponse, Method, ContentType
        //     Complete.ThisMethod();
        //     return responseToJson;
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
//     string       responseFromServer = reader.ReadToEnd();
//     // Console.WriteLine(responseFromServer);

//     reader.Close();
//     response.Close();

//     Complete.ThisMethod();
//     return Content(responseFromServer);
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

