using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObjectPrinter;

namespace BaseballScraper.Controllers.YahooControllers
{
    #pragma warning disable CS0414, CS0219
    public class YahooAuthController: Controller
    {
        private Constants _c = new Constants();
        private readonly YahooConfiguration _yahooConfig;
        private readonly IHttpContextAccessor _contextAccessor;


        public YahooAuthController(IOptions<YahooConfiguration> yahooConfig, IHttpContextAccessor contextAccessor)
        {
            _yahooConfig     = yahooConfig.Value;
            _contextAccessor = contextAccessor;
        }


        // mainly for testing purposes
        [HttpGet]
        [Route("/yahoo/authorize")]
        public IActionResult ViewYahooAuthHomePage()
        {
            _c.Start.ThisMethod();

            return View("yahoohome");
        }


        // STEP 1
        /// <summary> This triggers the browser to open a new window that asks the user to authorize the usage of their data within the app. When the user approves, they will receive a short authorization code. This code should then be entered in the terminal </summary>
        /// <returns> AuthorizationCode which is a string entered in the console </returns>
        [HttpGet]
        [Route("/yahoo/authorize/authorizationcode")]
        public string GenerateUserAuthorizationCode ()
        {
            _c.Start.ThisMethod();

            string requestUrl = $"https://api.login.yahoo.com/oauth2/request_auth?client_id={_yahooConfig.ClientId}&redirect_uri={_yahooConfig.RedirectUri}&response_type=code&language=en-us";

            Process.Start("open", requestUrl);

            Extensions.Spotlight("Enter Code:");
            // get approval code from console input _____ AUTHORIZATION CODE FROM CONSOLE ---> System.String
            // the code comes from https://api.login.yahoo.com/oauth2/authorize
            var authorizationCodeFromConsole = Console.ReadLine();

            authorizationCodeFromConsole.Intro("code entered in console");

            SetSessionAuthorizationCode(authorizationCodeFromConsole);

            // _c.Complete.ThisMethod();
            return authorizationCodeFromConsole;
        }

        // STEP 1B (this step is not required)
        /// <summary> Takes the authorization code and sets it within session / This is called within the GenerateUserAuthorizationCode method </summary>
        /// <returns> Void </returns>
        public void SetSessionAuthorizationCode(string authorizationCodeFromConsole)
        {
            var authCodeCheck = "";
            try {
                HttpContext.Session.SetString("authorizationcode", authorizationCodeFromConsole);
                    authCodeCheck = HttpContext.Session.GetString("authorizationcode");
                    authCodeCheck.Intro("setting auth code as");

                HttpContext.Session.SetInt32("sessionid", 1);
                    int? sessionid = HttpContext.Session.GetInt32("sessionid");
                    sessionid.Intro("setting session code as");
            }
            catch (Exception ex)
            {
                Console.WriteLine("error setting session in SetSessionAuthorizationCode");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }


        // STEP 2
        /// <summary> Generate the 'request' for authorization / Then convert it to a JObject </summary>
        /// <returns> The HttpWebRequest for authorization converted to a JObject </returns>
        [HttpGet]
        [Route("/yahoo/authorize/accesstokenjobject")]
        public JObject CreateYahooAccessTokenResponseJObject()
        {
            _c.Start.ThisMethod();

            // consumerKey and consumerSecret are unique to each yahoo app; Here, they are called from Secrets/ Config File
            var consumerKey    = _yahooConfig.ClientId;
            var consumerSecret = _yahooConfig.ClientSecret;
            var redirectUri    = _yahooConfig.RedirectUri;

            // generated from users input into console; it's a seven character string
            var authorizationCodeFromConsole = GenerateUserAuthorizationCode();

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

            // REQUEST ---> following are added through beginning to end: ContentLength, HaveResponse, Method, ContentType
            // _c.Complete.ThisMethod();
            return responseToJson;
        }


        // STEP 2B: not required. But can be used if you just want to generate the request and not convert to a JObject
        [Route("/yahoo/authorize/accesstokenrequest")]
        public HttpWebRequest CreateYahooAccessTokenRequest()
        {
            _c.Start.ThisMethod();

            // consumerKey and consumerSecret are unique to each yahoo app; Here, they are called from Secrets/ Config File
            var consumerKey    = _yahooConfig.ClientId;
            var consumerSecret = _yahooConfig.ClientSecret;
            var redirectUri    = _yahooConfig.RedirectUri;

            // Generates the Authorization Code that is needed for the request
            var authorizationCodeFromConsole = GenerateUserAuthorizationCode();

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

            // _c.Complete.ThisMethod();
            return request;
        }

        // STEP 3
        /// <summary> Retrieve response from Yahoo. Create a new instance of AccessTokenResponse from yahoo response </summary>
        /// <returns> A new AccessTokenResponse --> includes AccessToken, TokenType, ExpiresIn, RefreshToken, XOAuthYahooGuid</returns>
        [HttpGet]
        [Route("/yahoo/authorize/accesstokenresponse")]
        public AccessTokenResponse GetYahooAccessTokenResponse ()
        {
            _c.Start.ThisMethod();

            JObject responseToJson = CreateYahooAccessTokenResponseJObject();

            // NEW ACCESS TOKEN RESPONSE ---> BaseballScraper.Models.Configuration.AccessTokenResponse
            AccessTokenResponse newAccessTokenResponse = new AccessTokenResponse()
            {
                // generated / variable: a VERY long string of letters and numbers
                AccessToken = responseToJson["access_token"].ToString(),
                // constant: bearer
                TokenType = responseToJson["token_type"].ToString(),
                // constant: 3600
                ExpiresIn = responseToJson["expires_in"].ToString(),
                // unique: refresh_token
                RefreshToken = responseToJson["refresh_token"].ToString(),
                // unique to each yahoo user: yahoo guid
                XOAuthYahooGuid = responseToJson["xoauth_yahoo_guid"].ToString(),
            };

            // set the items in session
            SetSessionAccessTokenItems(newAccessTokenResponse);

            // _c.Complete.ThisMethod();
            return newAccessTokenResponse;
        }

        // STEP 4
        /// <summary> Take access token response and set the various items in session </summary>
        [Route("/yahoo/authorize/setsessionaccesstoken")]
        public void SetSessionAccessTokenItems(AccessTokenResponse newAccessTokenResponse)
        {
            try {
                HttpContext.Session.SetString("accesstoken", newAccessTokenResponse.AccessToken);
                HttpContext.Session.SetString("tokentype", newAccessTokenResponse.TokenType);
                HttpContext.Session.SetString("expiresin", newAccessTokenResponse.ExpiresIn);
                HttpContext.Session.SetString("refreshtoken", newAccessTokenResponse.RefreshToken);
                HttpContext.Session.SetString("yahooguid", newAccessTokenResponse.XOAuthYahooGuid);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error setting session in GetYahooAccessTokenResponse");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }


        // TODO: Confirm whether or not this works
        [HttpGet]
        [Route("/yahoo/authorize/checkyahoosession")]
        public ActionResult CheckYahooSession(HttpResponseMessage response = null)
        {
            _c.Start.ThisMethod();

            var initialAuthCodeCheck = HttpContext.Session.GetString("authorizationcode");
            initialAuthCodeCheck.Intro("initial auth code check");

            try {
                if(HttpContext.Session.GetString("authorizationcode") == null)
                {
                    Console.WriteLine("there is no session");
                }

                else
                {
                    Console.WriteLine("there is a session already");
                    var authCodeCheck     = HttpContext.Session.GetString("authorizationcode");
                    var accessTokenCheck  = HttpContext.Session.GetString("accesstoken");
                    var refreshTokenCheck = HttpContext.Session.GetString("refreshtoken");
                    var yahooGuidCheck    = HttpContext.Session.GetString("yahooguid");
                    var sessionIdCheck    = HttpContext.Session.GetInt32("sessionid").ToString();

                    Dictionary<string, string> sessionInfoDictionary = new Dictionary<string, string>();
                        sessionInfoDictionary.Add("authcode", authCodeCheck);
                        sessionInfoDictionary.Add("accesstoken", accessTokenCheck);
                        sessionInfoDictionary.Add("refreshtoken", refreshTokenCheck);
                        sessionInfoDictionary.Add("yahooguid", yahooGuidCheck);
                        sessionInfoDictionary.Add("sessionid", sessionIdCheck);

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
            }

            catch (Exception ex)
            {
                Console.WriteLine("error setting session in CheckYahooSession");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.StackTrace);

                try {
                    Extensions.Spotlight("trying more...");
                    throw new RequestFailedException(String.Format("HTTP error. Request Message - '{0}', Content - {1}, Status Code - {2}", response.RequestMessage, response.Content != null ? response.Content.ReadAsStringAsync().Result : "", response.StatusCode), response);
                }

                catch (Exception ex2)
                {
                    Console.WriteLine("error setting session in CheckYahooSession trying to throw");
                    Console.WriteLine(ex2.Message);
                    Console.WriteLine(ex2.InnerException);
                }
            }
            return Content("content");
        }


        public class RequestFailedException: Exception
        {
            HttpResponseMessage Response;

            public RequestFailedException(): base() { }

            public RequestFailedException(string message): base(message) { }

            public RequestFailedException(string message, HttpResponseMessage response): base(message) { Response = response; }

            public RequestFailedException(string message, Exception innerException): base(message, innerException) { }
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

        public Dictionary<string, string> CreateSessionInfoDictionary()
        {
            List       <string> messagesList                 = new List<string>();
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






        #region refresh token
        // [HttpGet]
        // [Route("/yahoo/refreshtoken")]
        // public IActionResult GetYahooRefreshToken()
        // {
        //     _c.Start.ThisMethod();

        //     var consumerKey    = _yahooConfig.ClientId;
        //     var consumerSecret = _yahooConfig.ClientSecret;
        //     var redirectUri    = _yahooConfig.RedirectUri;
        //     var refreshToken   = HttpContext.Session.GetString("session token");

        //     refreshToken.Intro("refresh token");

        //     Uri address = new Uri("https://api.login.yahoo.com/oauth2/get_token");

        //     HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

        //     request.Method      = "POST";
        //     request.ContentType = "application/x-www-form-urlencoded";

        //     byte[] headerByte = System.Text.Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);

        //     string headerString = System.Convert.ToBase64String(headerByte);

        //     request.Headers["Authorization"] = "Basic " + headerString;

        //     StringBuilder data = new StringBuilder();
        //     data.Append("?client_id=" + consumerKey);
        //     data.Append("&client_secret=" + consumerSecret);
        //     data.Append("&grant_type=refresh_token");
        //     data.Append("&redirect_uri=" + redirectUri);
        //     data.Append("&refresh_token=" + refreshToken);

        //     byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

        //     request.ContentLength = byteData.Length;

        //     using (Stream postStream = request.GetRequestStream())
        //     {
        //         postStream.Write(byteData, 0, byteData.Length);
        //     }

        //     string responseFromServer = "";

        //     try
        //     {
        //         using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        //         {
        //             StreamReader reader = new StreamReader(response.GetResponseStream());

        //             responseFromServer = reader.ReadToEnd();
        //         }
        //     }

        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex.Message);
        //         ex.SetContext("response from server", responseFromServer);
        //     }

        //     try {
        //         JObject responseToJson = JObject.Parse(responseFromServer);

        //         Extensions.Spotlight("refresh token response");
        //         foreach(var jsonItem in responseToJson)
        //         {
        //             Console.ForegroundColor = ConsoleColor.DarkMagenta;
        //             Console.WriteLine($"{jsonItem.Key.ToUpper()}");
        //             Console.ResetColor();
        //             Console.WriteLine(jsonItem.Value);
        //             Console.WriteLine();
        //         }
        //     }

        //     catch (Exception ex)
        //     {
        //         ex.Message.Intro("error message");
        //         ex.SetContext("response from server", responseFromServer);
        //     }

        //     Complete.ThisMethod();
        //     return RedirectToAction("viewnflgames");
        // }
        #endregion


        #region OTHER SESSION THINGS
            // public static void SetObjectAsJson(this ISession session, string key, object value)
            // {
            //     _c.Start.ThisMethod();
            //     session.SetString(key, JsonConvert.SerializeObject(value));
            //     Complete.ThisMethod();
            // }

            // public static async Task Set<T>(this ISession session, string key, T value)
            // {
            //     if (!session.IsAvailable)
            //     await session.LoadAsync();
            //     session.SetString(key, JsonConvert.SerializeObject(value));
            // }

            // public static async Task<T> Get<T>(this ISession session,string key)
            // {
            //     if (!session.IsAvailable)
            //     await session.LoadAsync();
            //     var value = session.GetString(key);
            //     return value == null ? default(T):
            //                         JsonConvert.DeserializeObject<T>(value);
            // }
        #endregion

    }
}