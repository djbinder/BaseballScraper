using System;
using System.Net;
using System.Collections.Generic;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Web;
using ObjectPrinter;
using Newtonsoft.Json.Linq;
using Nemiro.OAuth;
using Nemiro.OAuth.Clients;
using Serilog;
using NLog;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using BaseballScraper.Models.Yahoo;
using System.Reflection;
using System.Collections;

namespace BaseballScraper.Controllers
{
#pragma warning disable CS0414, CS0219
    public class YahooController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        private readonly TwitterConfiguration _twitterConfig;
        private readonly AirtableConfiguration _airtableConfig;
        private readonly YahooConfiguration _yahooConfig;


        private Uri getRequestTokenUri = new Uri("https://api.login.yahoo.com/oauth/v2/get_request_token");
        private Uri getAccessTokenUri  = new Uri("https://api.login.yahoo.com/oauth/v2/get_token");


        public YahooController(IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig, IOptions<YahooConfiguration> yahooConfig)
        {
            _airtableConfig = airtableConfig.Value;
            _twitterConfig  = twitterConfig.Value;
            _yahooConfig    = yahooConfig.Value;
        }



        // THIS WORKS
        [HttpGet]
        [Route("/yahoo/requestauth")]
        public Dictionary<string, string> GetYahooRequestAuthConfiguration()
        {
            Start.ThisMethod();

            Dictionary<string,string> requestAuthItemsList = new Dictionary<string,string>();

            var yahooConfigName = _yahooConfig.Name;
            var yahooAppId      = _yahooConfig.AppId;
            var consumerKey     = _yahooConfig.ClientId;
            var consumerSecret  = _yahooConfig.ClientSecret;
            var redirectUri     = _yahooConfig.RedirectUri;

            requestAuthItemsList.Add("Name", yahooConfigName);
            requestAuthItemsList.Add("App Id", yahooAppId);
            requestAuthItemsList.Add("Client Id", consumerKey);
            requestAuthItemsList.Add("Client Secret", consumerSecret);
            requestAuthItemsList.Add("Redirect Uri", redirectUri);

            foreach(var item in requestAuthItemsList)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(item.Key);
                Console.ResetColor();
                Console.WriteLine(item.Value);
                Console.WriteLine();
            }

            return requestAuthItemsList;
        }


        [HttpGet]
        [Route("/yahoo/simulate")]
        public IActionResult SimulateWebRequest()
        {
            Start.ThisMethod();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("Logs//BaseballScraperLog.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Hello, world!");
            Log.Information("The time is {Now}", DateTime.Now);

            Log.CloseAndFlush();

            var config = new NLog.Config.LoggingConfiguration();

            var logfile    = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            Complete.ThisMethod();
            return Content($"Request is: xyz");
        }




        [HttpGet]
        [Route("/yahoo/authorizationcode")]
        public IActionResult GetAuthorizationCode()
        {
            Start.ThisMethod();

            string requestUrl = $"https://api.login.yahoo.com/oauth2/request_auth?client_id={_yahooConfig.ClientId}&redirect_uri={_yahooConfig.RedirectUri}&response_type=code&language=en-us";

            Process.Start("open", requestUrl);

            requestUrl.Intro("request url");

            // REQUEST ---> System.Net.HttpWebRequest
            WebRequest request = WebRequest.Create(requestUrl);
            // request.Dig();

            // REQUEST.CREDENTIALS ---> System.Net.SystemNetworkCredential
            request.Credentials = CredentialCache.DefaultCredentials;

            // RESPONSE ---> System.Net.HttpWebResponse
            WebResponse response = request.GetResponse();

            response.ResponseUri.Intro("response uri");
            response.ResponseUri.Authority.Intro("authority");
            response.ResponseUri.Host.Intro("host");
            response.ResponseUri.LocalPath.Intro("local path");
            response.ResponseUri.UserInfo.Intro("user info");
            response.ResponseUri.OriginalString.Intro("original string");
            response.ResponseUri.PathAndQuery.Intro("path and query");
            response.ResponseUri.Scheme.Intro("scheme");
            response.ResponseUri.Segments.Intro("segments");

            response.Headers[3].Intro("cookie");

            response.Headers[4].Intro("age");                   // AGE ---> 0
            response.Headers[10].Intro("date");                 // DATE ---> e.g., 'Tue, 07 Aug 2018 23:03:19 GMT'
            response.Headers[11].Intro("connection");           // CONNECTION ---> close
            response.Headers[14].Intro("expires");              // EXPIRES ---> 0
            response.Headers[15].Intro("content-type");         // CONTENT-TYPE ---> text/html; charset=utf-8
            response.Headers[16].Intro("content-length");       // CONTENT-LENGTH ---> 100765

            // returns as 'OK'
            Console.WriteLine (((HttpWebResponse)response).StatusDescription);

            // response.Dig();

            Stream       postStream         = response.GetResponseStream();
            StreamReader reader             = new StreamReader(postStream);
            string       responseFromServer = reader.ReadToEnd();
            // Console.WriteLine(responseFromServer);

            reader.Close();
            response.Close();

            Complete.ThisMethod();
            return Content(responseFromServer);
        }


        // THIS WORKS
        [HttpGet]
        [Route("/yahoo/accesstoken")]
        public AccessTokenResponse GetYahooAccessToken()
        {
            Start.ThisMethod();

            var consumerKey    = _yahooConfig.ClientId;
            var consumerSecret = _yahooConfig.ClientSecret;
            var redirectUri    = _yahooConfig.RedirectUri;

            string requestUrl = $"https://api.login.yahoo.com/oauth2/request_auth?client_id={_yahooConfig.ClientId}&redirect_uri={_yahooConfig.RedirectUri}&response_type=code&language=en-us";

            Process.Start("open", requestUrl);

            Extensions.Spotlight("Enter Code:");
            // get approval code from console input _____ AUTHORIZATION CODE FROM CONSOLE ---> System.String
            // the code comes from https://api.login.yahoo.com/oauth2/authorize
            var authorizationCodeFromConsole = Console.ReadLine();
            authorizationCodeFromConsole.Intro("code entered in console");

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

            // returns 'access_token', 'refresh_token', 'expires_in', 'token_type', 'xoauth_yahoo_guid'
            Extensions.Spotlight("access token response");
            foreach(var jsonItem in responseToJson)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"{jsonItem.Key.ToUpper()}");
                Console.ResetColor();
                Console.WriteLine(jsonItem.Value);
                Console.WriteLine();
            }

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

            // REQUEST ---> following are added through beginning to end: ContentLength, HaveResponse, Method, ContentType
            Complete.ThisMethod();
            return newAccessTokenResponse;
        }

        [HttpGet]
        [Route("/yahoo/apiquerybase")]
        public IActionResult GetTeamStats(Uri QueryUri)
        {
            return Content("asdfasdf");
        }


        [HttpGet]
        [Route("/yahoo/teambase")]
        public YahooTeamBase GetTeamBase()
        {
            Start.ThisMethod();

            // call the GetYahooAccessToken method
                // 1. web browser will launch and take user to ---> https://api.login.yahoo.com/oauth2/request_auth?...
                // 2. click "Agree" in browser and a 7 digit code will appear; this code is the "&code=xyz" part of request_auth
                // 3. terminal will be asking for this code; copy and paste from browser and hit enter
                // 4. rest of the method runs
            AccessTokenResponse accessTokenResponse = GetYahooAccessToken();

            string accessToken = accessTokenResponse.AccessToken;

            int teamNumberToGet = 2;

            var uri = new Uri($"https://fantasysports.yahooapis.com/fantasy/v2/team/378.l.26189.t.{teamNumberToGet}?");
            // var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/league/378.l.26189/teams");
            // var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/league/378.l.26189/settings");
            // var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/league/378.l.26189");

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;

            // set request headers to be "Bearer <accesstoken>"
            request.Headers["Authorization"] = "Bearer " + accessToken;

            // set request method to 'GET'
            request.Method = "GET";

            string responseFromServer = "";

            //  RESPONSE TYPE ---> System.Net.HttpWebResponse
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                responseFromServer = reader.ReadToEnd();
            }

            // responseFromServer.Dig();

            XmlDocument doc       = new XmlDocument();
            XmlReader   xmlReader = XmlReader.Create(new System.IO.StringReader(responseFromServer));

            while (xmlReader.Read())
            {
                Console.WriteLine(xmlReader.Value);
                if(xmlReader.NodeType == XmlNodeType.Element)
                {
                    Console.WriteLine(xmlReader.LocalName);
                }
            }

            // responseFromServer comes back as xml
            doc.LoadXml(responseFromServer);

            // convert the xml to json
            string json = JsonConvert.SerializeXmlNode(doc);

            // clean the json up
            JObject responseToJson = JObject.Parse(json);
            // responseToJson.Dig();

            // int jCount = responseToJson.Count;
            // jCount.Intro("j count");

            // // instantiate a new instance of team and add the class items
            YahooTeamBase teamBase = new YahooTeamBase();

            // try
            // {
            //     Extensions.Spotlight("trying to build teambase");
            //     teamBase.Key                   = responseToJson["fantasy_content"]["team"]["team_key"].ToString();
            //     teamBase.Name                  = responseToJson["fantasy_content"]["team"]["name"].ToString();
            //     teamBase.TeamId                = (int?)responseToJson["fantasy_content"]["team"]["team_id"];
            //     teamBase.IsOwnedByCurrentLogin = (int?)responseToJson["fantasy_content"]["team"]["is_owned_by_current_login"];
            //     teamBase.Url                   = responseToJson["fantasy_content"]["team"]["url"].ToString();
            //     teamBase.TeamLogo              = responseToJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["url"].ToString();
            //     teamBase.WaiverPriority        = (int?)responseToJson["fantasy_content"]["team"]["waiver_priority"];
            //     teamBase.NumberOfMoves         = (int?)responseToJson["fantasy_content"]["team"]["number_of_moves"];
            //     teamBase.NumberOfTrades        = (int?)responseToJson["fantasy_content"]["team"]["number_of_trades"];
            //     teamBase.CoverageType          = responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_type"].ToString();
            //     teamBase.CoverageValue         = responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_value"].ToString();
            //     teamBase.Value                 = responseToJson["fantasy_content"]["team"]["roster_adds"]["value"].ToString();
            //     teamBase.LeagueScoringType     = responseToJson["fantasy_content"]["team"]["league_scoring_type"].ToString();
            //     teamBase.HasDraftGuide         = responseToJson["fantasy_content"]["team"]["has_draft_grade"].ToString();
            //     teamBase.ManagerId             = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["manager_id"].ToString();
            //     teamBase.NickName              = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["nickname"].ToString();
            //     teamBase.Guid                  = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["guid"].ToString();
            //     teamBase.IsCommissioner        = (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_commissioner"];
            //     teamBase.IsCurrentLogin        = (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_current_login"];
            //     teamBase.Email                 = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["email"].ToString();
            //     teamBase.ImageUrl              = responseToJson["fantasy_content"]["team"]["managers"]["manager"]["image_url"].ToString();
            //     teamBase.Dig();
            // }

            // catch (System.Exception)
            // {
            //     Extensions.Spotlight("broke during teambase");
            //     throw;
            // }

            CreateYahooTeamBaseModel(responseToJson);

            CreateYahooTeamBaseHasTable(responseToJson);



            #region hashtable
            // try
            // {
            //     Extensions.Spotlight("trying to build team hash table");
            //     Hashtable teamHashTable = new Hashtable();
            //     teamHashTable.Add("Key", responseToJson["fantasy_content"]["team"]["team_key"].ToString());
            //     teamHashTable.Add("TeamId", (int?)responseToJson["fantasy_content"]["team"]["team_id"]);
            //     teamHashTable.Add("Name", responseToJson["fantasy_content"]["team"]["name"].ToString());
            //     teamHashTable.Add("Is Owned By Current Login?", (int?)responseToJson["fantasy_content"]["team"]["is_owned_by_current_login"]);
            //     teamHashTable.Add("Url", responseToJson["fantasy_content"]["team"]["url"].ToString());
            //     teamHashTable.Add("Team Logo", responseToJson["fantasy_content"]["team"]["team_logos"]["team_logo"]["url"].ToString());
            //     teamHashTable.Add("Waiver Priority", (int?)responseToJson["fantasy_content"]["team"]["waiver_priority"]);
            //     teamHashTable.Add("Number of Moves", (int?)responseToJson["fantasy_content"]["team"]["number_of_moves"]);
            //     teamHashTable.Add("Number of Trades", (int?)responseToJson["fantasy_content"]["team"]["number_of_trades"]);
            //     teamHashTable.Add("Coverage Type", responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_type"].ToString());
            //     teamHashTable.Add("Coverage Value", responseToJson["fantasy_content"]["team"]["roster_adds"]["coverage_value"].ToString());
            //     teamHashTable.Add("Value", responseToJson["fantasy_content"]["team"]["roster_adds"]["value"].ToString());
            //     teamHashTable.Add("League Scoring Type", responseToJson["fantasy_content"]["team"]["league_scoring_type"].ToString());
            //     teamHashTable.Add("Has Draft Grade?", responseToJson["fantasy_content"]["team"]["has_draft_grade"].ToString());

            //     teamHashTable.Add("Manager Id", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["manager_id"].ToString());
            //     teamHashTable.Add("NickName", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["nickname"].ToString());
            //     teamHashTable.Add("Guid", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["guid"].ToString());
            //     teamHashTable.Add("Is Commish?", (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_commissioner"]);
            //     teamHashTable.Add("Is Current Login?", (int?)responseToJson["fantasy_content"]["team"]["managers"]["manager"]["is_current_login"]);
            //     teamHashTable.Add("Email", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["email"].ToString());
            //     teamHashTable.Add("Image Url", responseToJson["fantasy_content"]["team"]["managers"]["manager"]["image_url"].ToString());

            //     Extensions.Spotlight("dan");

            //     IDictionaryEnumerator _enumerator = teamHashTable.GetEnumerator();

            //     int _enumeratorCount = 1;

            //     while (_enumerator.MoveNext())
            //     {
            //         Console.ForegroundColor = ConsoleColor.DarkMagenta;
            //         Console.WriteLine(_enumerator.Key.ToString());
            //         Console.ResetColor();

            //         if(_enumerator.Value == null)
            //         {
            //             Console.WriteLine("null");
            //         }

            //         else
            //         {
            //             Console.WriteLine(_enumerator.Value.ToString());
            //         }
            //         Console.WriteLine();
            //         _enumeratorCount++;
            //     }
            // }

            // catch (System.Exception)
            // {
            //     Extensions.Spotlight("broke during team hash table");
            //     throw;
            // }
            #endregion

            return teamBase;
        }

        public YahooTeamBase CreateYahooTeamBaseModel (JObject TeamBaseJson)
        {
            Start.ThisMethod();

            JObject responseToJson = TeamBaseJson;

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


        public Hashtable CreateYahooTeamBaseHasTable (JObject TeamBaseJson)
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
        [Route("/yahoo/signature")]
        public string GetSignature()
        {
            Console.Title = "Signature";
            Start.ThisMethod();

            AccessTokenResponse accessTokenResponse = GetYahooAccessToken();
            accessTokenResponse.Dig();

            string accessToken = accessTokenResponse.AccessToken;
            accessToken.Intro("access token");

            var consumerKey    = _yahooConfig.ClientId;
            var consumerSecret = _yahooConfig.ClientSecret;
            var redirectUri    = _yahooConfig.RedirectUri;
            var refreshToken   = HttpContext.Session.GetString("refresh token");
            refreshToken.Intro("refresh token | signature");

            var nullRefreshToken = "";

            var data = String.Empty;
            string url, param;

            var uri = new Uri("https://social.yahooapis.com/v1/user/abcdef123/profile?format=json");
            // var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/league/378.l.26189");

            // WORKS
            // var uri = new Uri("http://fantasysports.yahooapis.com/fantasy/v2/game/223/leagues;league_keys=223.l.431");

            // THESE DO NOT WORK
            // var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/users;use_login=1");

            // THESE ALL WORK; note https vs http in some
            // MLB 2018 game_key and game_id both = 378;
            // var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/game/378");
            // var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/game/nfl/game_weeks");
            // var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/game/223");
            // var uri = new Uri("https://fantasysports.yahooapis.com/fantasy/v2/game/mlb");

            // O AUTH ---> BaseballScraper.OAuthBase
            var oAuth = new OAuthBase();

            // this changes with each request; System.String; NONCE ---> e.g., 4260699
            var nonce = oAuth.GenerateNonce();

            // this changes with each request; System.String; TIME STAMP ---> e.g, 1533611257
            var timeStamp = oAuth.GenerateTimeStamp();

            Console.WriteLine($"NONCE: {nonce}   TIMESTAMP: {timeStamp}");
            Console.WriteLine();

            // this changes with each request; System.String; SIGNATURE ---> e.g., M8jxmDTTEGwkyf8Ey1qAtnIpnRQ=
            var signature = oAuth.GenerateSignature(
                uri,
                consumerKey,
                consumerSecret,
                accessToken,
                string.Empty,
                "GET",
                timeStamp,
                nonce,
                OAuthBase.SignatureTypes.HMACSHA1,
                out url,
                out param
            );

            // concatantes url, oauth_consumer_key, nonce, method, time_stamp, version, signature
            data = String.Format("{0}?{1}&oauth_signature={2}", url, param, signature);

            Console.WriteLine($"URL: {url}");
            Console.WriteLine();
            Console.WriteLine($"PARAMS: {param}");
            Console.WriteLine();
            Console.WriteLine($"SIGNATURE: {signature}");
            Console.WriteLine();

            // REQUEST ---> System.Net.HttpWebRequest
            HttpWebRequest request = WebRequest.Create(data) as HttpWebRequest;

            // Extensions.Spotlight("request 1");
            // request.Dig();

            request.ContentType = "application/x-www-form-urlencoded";

            byte [] headerByte = System.Text.Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);
                headerByte.Dig();

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("HEADER BYTE");
                Console.ResetColor();
                Console.WriteLine(headerByte);
                Console.WriteLine();
                Console.WriteLine(headerByte.Length);

            // MINE: ZGoweUp....
            string headerString = System.Convert.ToBase64String(headerByte);

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("HEADER STRING");
                Console.ResetColor();
                Console.WriteLine(headerString);
                Console.WriteLine();

            // Bearer ZGoweUp....
            request.Headers["Authorization"] = "Bearer " + accessToken;
            // request.Headers["Authorization"] = "Basic " + headerString;

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("AUTHORIZATION");
                Console.ResetColor();
                Console.WriteLine(request.Headers["Authorization"]);
                Console.WriteLine();

            string requestRequestUri = request.RequestUri.ToString();

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("REQUEST URI");
                Console.ResetColor();
                Console.WriteLine(requestRequestUri);
                Console.WriteLine();

            // var xD = WebRequest.Create(string.Format($"{url}?{param}&oauth_signature={signature}")).GetResponse();
            // Console.WriteLine(xD);


            // request has added 'ContentType', 'Authorization' (both inf Headers)
                // Extensions.Spotlight("request 2");
                // request.Dig();

            // RESPONSE ---> System.Net.HttpWebResponse
            // DATA STREAM ---> System.Net.Http.HttpConnection+ContentLengthReadStream
            // READER ---> System.IO.StreamReader
            // 'have response' of 'request' is set to true
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response == null)
                {
                    return "NOT WORKING";
                }
                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream == null)
                    {
                        return "NOT WORKING 2";
                    }

                    using (StreamReader reader = new StreamReader(dataStream, Encoding.UTF8))
                    {
                        data = reader.ReadToEnd();

                        Extensions.Spotlight("response");
                        Console.WriteLine($"STATUS DESCRIPTION: {((HttpWebResponse)response).StatusDescription}");
                        Console.WriteLine($"STATUS CODE: {((HttpWebResponse)response).StatusCode}");
                        // response.Dig();
                    }
                }
            }

            // JObject responseToJson = JObject.Parse(responseFromServer);
            // responseToJson.Dig();

            Complete.ThisMethod();
            // DATA ---> is the returned XML as a string
            return data;
        }


        // public async Task<AccessAndSessionInfo> GetAccessAsync()
        // {

        // }





        // help from: https://github.com/scottiemc7/FBF.Yahoo/blob/master/FBF.Yahoo/OAuth.cs
        // https://api.login.yahoo.com/oauth/v2/get_request_token?oauth_nonce={0}&oauth_timestamp={1}&oauth_consumer_key={2}&oauth_version=1.0&xoauth_lang_pref=en-us&oauth_callback=oob&oauth_signature_method=HMAC-SHA1
        public static string getRequestTokenUriFormat = "https://api.login.yahoo.com/oauth/v2/get_request_token?" +
            "oauth_nonce={0}&" +                    // nonce                        (1 of 3)
            "oauth_timestamp={1}&" +                // timestamp                    (1 of 3)
            "oauth_consumer_key={2}&" +             // oauth consumer key           (1 of 2)
            "oauth_version=1.0&" +                  // oauth version                (1 of 3)
            "xoauth_lang_pref=en-us&" +             // oauth language preference    (1 of 1)
            "oauth_callback=oob&" +                 // oauth callback               (1 of 1)
            "oauth_signature_method=HMAC-SHA1";     // oauth signature method       (1 of 3)


        // https://api.login.yahoo.com/oauth/v2/get_token?oauth_nonce={0}&oauth_timestamp={1}&oauth_consumer_key={2}&oauth_version=1.0&oauth_verifier={3}&oauth_token={4}&oauth_signature_method=HMAC-SHA1
        public static string getAccessTokenUriFormat = "https://api.login.yahoo.com/oauth/v2/get_token?" +
            "oauth_nonce={0}&" +                    // nonce                        (2 of 3)    // nonce
            "oauth_timestamp={1}&" +                // timestamp                    (2 of 3)    // timestamp
            "oauth_consumer_key={2}&" +             // oauth consumer key           (2 of 2)    // consumerKey
            "oauth_version=1.0&" +                  // oauth version                (2 of 3)    // 1.0
            "oauth_verifier={3}&" +                 // oauth verifier               (1 of 1)
            "oauth_token={4}&" +                    // oauth token                  (1 of 2)    // session token
            "oauth_signature_method=HMAC-SHA1";     // oauth signature method       (2 of 3)    // HMAC-SHA1


        // oauth_consumer_key={0}&oauth_nonce={1}&oauth_timestamp={2}&oauth_token={3}&oauth_version=1.0&oauth_signature_method=HMAC-SHA1
        public static string getOAuthQueryStringFormat = "oauth_consumer_key={0}&" +
            "oauth_nonce={1}&" +                    // nonce                        (3 of 3)
            "oauth_timestamp={2}&" +                // timestamp                    (3 of 3)
            "oauth_token={3}&" +                    // oauth token                  (2 of 2)
            "oauth_version=1.0&" +                  // oauth version                (3 of 3)
            "oauth_signature_method=HMAC-SHA1";     // oauth signature method       (3 of 3)







        [HttpGet]
        [Route("/nflgames")]
        public IActionResult ViewNflGames()
        {
            Start.ThisMethod();

            var refreshToken = HttpContext.Session.GetString("session token");
            // refreshToken.Intro("refresh token");

            Uri nflUrlAddress = new Uri("http://fantasysports.yahooapis.com/fantasy/v2/users;use_login=1/games");

            HttpWebRequest request = WebRequest.Create(nflUrlAddress) as HttpWebRequest;

            request.Method = "GET";

            // REQUEST.HEADERS ---> Authorization: Bearer ANYbXltBbZxNep6uw2zNPWeOs8rmhkRR.LxBqqpRb4dV2NTtfA--
            // REQUEST.HEADERS ["AUTHORIZATION"] ---> Bearer ANYbXltBbZxNep6uw2zNPWeOs8rmhkRR.LxBqqpRb4dV2NTtfA--
            request.Headers["Authorization"] = "Bearer " + refreshToken;

            // request.Dig();

            return Redirect("http://fantasysports.yahooapis.com/fantasy/v2/users;use_login=1/games");
        }


        [HttpGet]
        [Route("/yahoo/openmlbpage")]
        public void OpenMlbPage()
        {
            Start.ThisMethod();

            string refreshToken = HttpContext.Session.GetString("session refresh token");
            refreshToken.Intro("session refresh token");

            string accessToken = HttpContext.Session.GetString("session access token");
            accessToken.Intro("session access token");

            string sessionAccessSecret = _yahooConfig.ClientSecret;
            sessionAccessSecret.Intro("access secret");

            string signatureString = GetSignature();
            signatureString.Intro("mlb singature string");

            try {
                string mlbUrl = "http://fantasysports.yahooapis.com/fantasy/v2/game/mlb";
                Process.Start("open", mlbUrl);
            }

            catch {
                string mlbUrl = "https://fantasysports.yahooapis.com/fantasy/v2/game/mlb";
                Process.Start("open", mlbUrl);
            }

            finally {
                Extensions.Spotlight("neither mlb link worked");
            }

            Complete.ThisMethod();
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