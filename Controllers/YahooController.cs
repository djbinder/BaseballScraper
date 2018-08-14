using System;
using System.Net;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.IO;
using System.Diagnostics;
using ObjectPrinter;
using Newtonsoft.Json.Linq;
using System.Xml;
using Newtonsoft.Json;
using BaseballScraper.Models.Yahoo;
using System.Collections;

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


        public YahooController(IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig, IOptions<YahooConfiguration> yahooConfig)
        {
            _airtableConfig = airtableConfig.Value;
            _twitterConfig  = twitterConfig.Value;
            _yahooConfig    = yahooConfig.Value;
        }


        [HttpGet]
        [Route("/yahoo/simulate")]
        public IActionResult SimulateWebRequest()
        {
            Start.ThisMethod();

            Complete.ThisMethod();
            return Content($"Request is: xyz");
        }



        public string SetAccessCode ()
        {
            Start.ThisMethod();

            string requestUrl = $"https://api.login.yahoo.com/oauth2/request_auth?client_id={_yahooConfig.ClientId}&redirect_uri={_yahooConfig.RedirectUri}&response_type=code&language=en-us";

            Process.Start("open", requestUrl);

            Extensions.Spotlight("Enter Code:");
            // get approval code from console input _____ AUTHORIZATION CODE FROM CONSOLE ---> System.String
            // the code comes from https://api.login.yahoo.com/oauth2/authorize
            var authorizationCodeFromConsole = Console.ReadLine();

            authorizationCodeFromConsole.Intro("code entered in console");

            Complete.ThisMethod();

            return authorizationCodeFromConsole;
        }


        [Route("/yahoo/authorize/accesstokenrequest")]
        public HttpWebRequest CreateYahooAccessTokenRequest()
        {
            Start.ThisMethod();

            var consumerKey    = _yahooConfig.ClientId;
            var consumerSecret = _yahooConfig.ClientSecret;
            var redirectUri    = _yahooConfig.RedirectUri;

            var authorizationCodeFromConsole = SetAccessCode();
            authorizationCodeFromConsole.Intro("auth code");

            // Exchange authorization code for Access Token by sending Post Request
            Uri address = new Uri("https://api.login.yahoo.com/oauth2/get_token");

            // Create the web request ___ REQUEST ---> System.Net.HttpWebRequest
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

            // Set type to POST ---> changes 'Method' of HttpWebRequest to 'POST'
            request.Method = "POST";

            // changes 'Content Type' of HttpWebRequest to 'application/x-www-form-urlencoded'
            request.ContentType = "application/x-www-form-urlencoded";

            // produces very Base64Encoded version of consumerKey and yahooClientSecrent(it's long string of letters and numbers) ___ HEADER BYTE ---> System.Byte[] ___ e.g., '"ZGoweUpt...etc."'
            byte[] headerByte = System.Text.Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);

            // converts 'headerByte'; same letters and numbers but new format ___ HEADER STRING ---> System.String ___ e.g., 'ZGoweUpt...etc.'
            string headerString = System.Convert.ToBase64String(headerByte);

            // returns two lines
                // Content-Type: application/x-www-form-urlencoded
                // 'Authorization: Basic <headerString> ___ e.g., <headerString> = ZGoweUpt...etc.
            request.Headers["Authorization"] = "Basic " + headerString;

            // Create the data we want to send ___ DATA ---> System.Text.StringBuilder ___ 'data' is a concatanated string of all of the data.Append items
            StringBuilder data = new StringBuilder();
                data.Append("?client_id=" + consumerKey);
                data.Append("&client_secret=" + consumerSecret);
                data.Append("&redirect_uri=" + redirectUri);
                data.Append("&code=" + authorizationCodeFromConsole);
                data.Append("&grant_type=authorization_code");

            // Create a byte array of the data we want to send ___ BYTE DATA ---> System.Byte[]
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the request headers ___ // changes 'Content Length' of HttpWebRequest to 218
            request.ContentLength = byteData.Length;

            // request.Dig();

            Complete.ThisMethod();

            return request;
        }


        // THIS WORKS
        [HttpGet]
        [Route("/yahoo/authorize/accesstokenjobject")]
        public JObject CreateYahooAccessTokenResponseJObject()
        {
            Start.ThisMethod();

            var consumerKey    = _yahooConfig.ClientId;
            var consumerSecret = _yahooConfig.ClientSecret;
            var redirectUri    = _yahooConfig.RedirectUri;

            var authorizationCodeFromConsole = SetAccessCode();
            authorizationCodeFromConsole.Intro("auth code");

            // Exchange authorization code for Access Token by sending Post Request
            Uri address = new Uri("https://api.login.yahoo.com/oauth2/get_token");

            // Create the web request ___ REQUEST ---> System.Net.HttpWebRequest
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

            // Set type to POST ---> changes 'Method' of HttpWebRequest to 'POST'
            request.Method = "POST";

            // changes 'Content Type' of HttpWebRequest to 'application/x-www-form-urlencoded'
            request.ContentType = "application/x-www-form-urlencoded";

            // produces very Base64Encoded version of consumerKey and yahooClientSecrent(it's long string of letters and numbers) ___ HEADER BYTE ---> System.Byte[] ___ e.g., '"ZGoweUpt...etc."'
            byte[] headerByte = System.Text.Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);

            // converts 'headerByte'; same letters and numbers but new format ___ HEADER STRING ---> System.String ___ e.g., 'ZGoweUpt...etc.'
            string headerString = System.Convert.ToBase64String(headerByte);

            // returns two lines
                // Content-Type: application/x-www-form-urlencoded
                // 'Authorization: Basic <headerString> ___ e.g., <headerString> = ZGoweUpt...etc.
            request.Headers["Authorization"] = "Basic " + headerString;

            // Create the data we want to send ___ DATA ---> System.Text.StringBuilder ___ 'data' is a concatanated string of all of the data.Append items
            StringBuilder data = new StringBuilder();
                data.Append("?client_id=" + consumerKey);
                data.Append("&client_secret=" + consumerSecret);
                data.Append("&redirect_uri=" + redirectUri);
                data.Append("&code=" + authorizationCodeFromConsole);
                data.Append("&grant_type=authorization_code");

            // Create a byte array of the data we want to send ___ BYTE DATA ---> System.Byte[]
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the request headers ___ // changes 'Content Length' of HttpWebRequest to 218
            request.ContentLength = byteData.Length;

            // Write data ___ POST STREAM ---> System.Net.RequestStream
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response
            string responseFromServer = "";

            try
            {
                // changes 'HaveResponse' of HttpWebRequest to 228
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream ___ READER ---> System.IO.StreamReader
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    responseFromServer = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // if error ---> 'The remote server returned an error: (400) Bad Request.'
                ex.SetContext("response from server", responseFromServer);
            }

            // RESPONSE TO JSON ---> Newtonsoft.Json.Linq.JObject
            JObject responseToJson = JObject.Parse(responseFromServer);

            // PrintJObjectItems(responseToJson);

            // responseToJson.Dig();

            // REQUEST ---> following are added through beginning to end: ContentLength, HaveResponse, Method, ContentType
            Complete.ThisMethod();
            return responseToJson;
        }


        [Route("/yahoo/authorize/accesstokenresponse")]
        public AccessTokenResponse GetYahooAccessTokenResponse (JObject AccessTokenResponseJson)
        {
            Start.ThisMethod();

            JObject responseToJson = AccessTokenResponseJson;

            // NEW ACCESS TOKEN RESPONSE ---> BaseballScraper.Models.Configuration.AccessTokenResponse
            AccessTokenResponse newAccessTokenResponse = new AccessTokenResponse()
            {
                // generated / variable: a VERY long string of letters and numbers
                AccessToken = responseToJson["access_token"].ToString(),
                // constant: bearer
                TokenType = responseToJson["token_type"].ToString(),
                // constant: 3600
                ExpiresIn = responseToJson["expires_in"].ToString(),
                // mine: refresh_token
                RefreshToken = responseToJson["refresh_token"].ToString(),
                // mine: yahoo guid
                XOAuthYahooGuid = responseToJson["xoauth_yahoo_guid"].ToString(),
            };

            // mine
            string refreshToken = newAccessTokenResponse.RefreshToken;
                HttpContext.Session.SetString("refresh token", refreshToken);
                var      refreshTokenCheck = HttpContext.Session.GetString("refresh token");
                TempData["SessionToken"]   = refreshToken;
                ViewData["SessionToken"]   = refreshToken;

            // generated / variable
            string accessToken = newAccessTokenResponse.AccessToken;
                HttpContext.Session.SetString("access token", accessToken);

            Complete.ThisMethod();

            return newAccessTokenResponse;
        }




        // returns 'access_token', 'refresh_token', 'expires_in', 'token_type', 'xoauth_yahoo_guid'
        public void PrintJObjectItems(JObject JObjectToPrint)
        {
            Start.ThisMethod();

            var responseToJson = JObjectToPrint;

            Extensions.Spotlight("access token response");

            foreach(var jsonItem in responseToJson)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"{jsonItem.Key.ToUpper()}");
                Console.ResetColor();
                Console.WriteLine(jsonItem.Value);
                Console.WriteLine();
            }

            Complete.ThisMethod();
        }



        // [HttpGet]
        // [Route("/yahoo/team/teamstats")]
        // public IActionResult GetTeamStats()
        // {
        //     Start.ThisMethod();
        //     JObject             teamStatsJObject    = CreateYahooAccessTokenResponseJObject();
        //     AccessTokenResponse accessTokenResponse = GetYahooAccessTokenResponse(teamStatsJObject);

        //     Complete.ThisMethod();


        // }





        [HttpGet]
        [Route("/yahoo/team/teambase")]
        public JObject CreateTeamBaseJObject(Uri uri)
        {
            Start.ThisMethod();

            JObject jObject = CreateYahooAccessTokenResponseJObject();

            // call the GetYahooAccessToken method
                // 1. web browser will launch and take user to ---> https://api.login.yahoo.com/oauth2/request_auth?...
                // 2. click "Agree" in browser and a 7 digit code will appear; this code is the "&code=xyz" part of request_auth
                // 3. terminal will be asking for this code; copy and paste from browser and hit enter
                // 4. rest of the method runs
            AccessTokenResponse accessTokenResponse = GetYahooAccessTokenResponse(jObject);

            string accessToken = accessTokenResponse.AccessToken;

            // int teamNumberToGet = 1;

            // var uri = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/stats;type=week;week=2");
            // var uri = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}?");

            HttpWebRequest  request          = WebRequest.Create(uri) as HttpWebRequest;
            request.Headers["Authorization"] = "Bearer " + accessToken;
                            request.Method   = "GET";

            string responseFromServer = "";

            //  RESPONSE TYPE ---> System.Net.HttpWebResponse
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                responseFromServer = reader.ReadToEnd();
            }

            XmlDocument doc       = new XmlDocument();
            XmlReader   xmlReader = XmlReader.Create(new System.IO.StringReader(responseFromServer));

            // responseFromServer comes back as xml
            doc.LoadXml(responseFromServer);

            // convert the xml to json
            string json = JsonConvert.SerializeXmlNode(doc);

            // clean the json up
            JObject responseToJson = JObject.Parse(json);
            PrintJObjectItems(responseToJson);

            return responseToJson;
        }


        [Route("yahoo/team/stats/model")]
        public YahooTeamStats CreateYahooTeamStatsModel()
        {
            Start.ThisMethod();

            int teamNumberToGet = 1;
            int weekNumber      = 18;

            var uri = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/standings");
            // var uri = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/stats");
            // var uri = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}/stats;type=week;week={weekNumber}");

            JObject teamStatsJson = CreateTeamBaseJObject(uri);

            YahooTeamStats teamStats = new YahooTeamStats();

            try {
                teamStats.Season     = teamStatsJson["fantasy_content"]["team"]["team_stats"]["season"].ToString();
                teamStats.WeekNumber = "No week number; these numbers are for the full season";
            }
            catch {
                teamStats.WeekNumber = teamStatsJson["fantasy_content"]["team"]["team_stats"]["week"].ToString();
                teamStats.Season     = "No season / year; these numbers are for one week";
            }
            // finally {
            //     Console.WriteLine("there is no season OR week; something is broken");
            // }

            teamStats.CoverageType = teamStatsJson["fantasy_content"]["team"]["team_stats"]["coverage_type"].ToString();

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

            currentStatId++;
            teamStats.TeamPoints = teamStatsJson["fantasy_content"]["team"]["team_points"]["total"].ToString();

            teamStats.Dig();

            // int statsCount = teamStatsJson["fantasy_content"]["team"]["team_stats"]["stats"]["stat"];

            Complete.ThisMethod();
            return teamStats;
        }






        [Route("/yahoo/team/teambase/model")]
        public YahooTeamBase CreateYahooTeamBaseModel ()
        {
            Start.ThisMethod();

            int teamNumberToGet = 1;

            var uri = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}?");

            JObject responseToJson = CreateTeamBaseJObject(uri);

            YahooTeamBase teamBase = new YahooTeamBase();

            Extensions.Spotlight("trying to build teambase");
            teamBase.Key                   = responseToJson["fantasy_content"]["team"]["team_key"].ToString();
            teamBase.Name                  = responseToJson["fantasy_content"]["team"]["name"].ToString();
            teamBase.TeamId                = (int?)responseToJson["fantasy_content"]["team"]["team_id"];
            teamBase.IsOwnedByCurrentLogin = (int?)responseToJson["fantasy_content"]["team"]["is_owned_by_current_login"];
            teamBase.Url                   = responseToJson["fantasy_content"]["team"]["url"].ToString();
            teamBase.TeamLogo              = responseToJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["url"].ToString();
            teamBase.WaiverPriority        = (int?)responseToJson["fantasy_content"]["team"]["waiver_priority"];
            teamBase.NumberOfMoves         = (int?)responseToJson["fantasy_content"]["team"]["number_of_moves"];
            teamBase.NumberOfTrades        = (int?)responseToJson["fantasy_content"]["team"]["number_of_trades"];
            teamBase.CoverageType          = responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_type"].ToString();
            teamBase.CoverageValue         = responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_value"].ToString();
            teamBase.Value                 = responseToJson["fantasy_content"]["team"]["roster_adds"]["value"].ToString();
            teamBase.LeagueScoringType     = responseToJson["fantasy_content"]["team"]["league_scoring_type"].ToString();
            teamBase.HasDraftGuide         = responseToJson["fantasy_content"]["team"]["has_draft_grade"].ToString();
            teamBase.ManagerId             = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["manager_id"].ToString();
            teamBase.NickName              = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["nickname"].ToString();
            teamBase.Guid                  = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["guid"].ToString();
            teamBase.IsCommissioner        = (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_commissioner"];
            teamBase.IsCurrentLogin        = (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_current_login"];
            teamBase.Email                 = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["email"].ToString();
            teamBase.ImageUrl              = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["image_url"].ToString();
            teamBase.Dig();

            Complete.ThisMethod();

            return teamBase;
        }




        [Route("/yahoo/team/teambase/hashtable")]
        public Hashtable CreateYahooTeamBaseHashTable (JObject TeamBaseJson)
        {
            Start.ThisMethod();

            JObject responseToJson = TeamBaseJson;

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



        [HttpGet]
        [Route("/yahoo/refreshtoken")]
        public IActionResult GetYahooRefreshToken()
        {
            Start.ThisMethod();

            var consumerKey    = _yahooConfig.ClientId;
            var consumerSecret = _yahooConfig.ClientSecret;
            var redirectUri    = _yahooConfig.RedirectUri;
            var refreshToken   = HttpContext.Session.GetString("session token");

            refreshToken.Intro("refresh token");

            Uri address = new Uri("https://api.login.yahoo.com/oauth2/get_token");

            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

            request.Method      = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            byte[] headerByte = System.Text.Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);

            string headerString = System.Convert.ToBase64String(headerByte);

            request.Headers["Authorization"] = "Basic " + headerString;

            StringBuilder data = new StringBuilder();
            data.Append("?client_id=" + consumerKey);
            data.Append("&client_secret=" + consumerSecret);
            data.Append("&grant_type=refresh_token");
            data.Append("&redirect_uri=" + redirectUri);
            data.Append("&refresh_token=" + refreshToken);

            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            request.ContentLength = byteData.Length;

            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            string responseFromServer = "";

            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    responseFromServer = reader.ReadToEnd();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ex.SetContext("response from server", responseFromServer);
            }

            try {
                JObject responseToJson = JObject.Parse(responseFromServer);

                Extensions.Spotlight("refresh token response");
                foreach(var jsonItem in responseToJson)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"{jsonItem.Key.ToUpper()}");
                    Console.ResetColor();
                    Console.WriteLine(jsonItem.Value);
                    Console.WriteLine();
                }
            }

            catch (Exception ex)
            {
                ex.Message.Intro("error message");
                ex.SetContext("response from server", responseFromServer);
            }

            Complete.ThisMethod();
            return RedirectToAction("viewnflgames");
        }
    }
}

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