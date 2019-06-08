using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObjectPrinter;



#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006, CS0168
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    public class YahooAuthController: ControllerBase
    {
        private readonly Helpers _h = new Helpers();

        private readonly YahooConfiguration _yahooConfig;

        private readonly TheGameIsTheGameConfiguration _theGameConfig = new TheGameIsTheGameConfiguration();

        private static YahooApiEndPoints endPoints = new YahooApiEndPoints();

        private readonly YahooApiRequestController _yHomeController = new YahooApiRequestController();

        private readonly IHttpContextAccessor _contextAccessor;

        private readonly JsonHandler _jsonHandler = new JsonHandler();
        public static readonly JsonHandler.NewtonsoftJsonHandlers _newtonHandler = new JsonHandler.NewtonsoftJsonHandlers();

        public ISession _sessionX;


        // public readonly YahooTeamBaseController _yahooTeamBaseController = new YahooTeamBaseController();

        public static readonly string yahooConfigFilePath = "Configuration/yahooConfig.json";

        public string _accessTokenKey = "accesstoken";
        public string _tokenTypeKey = "tokentype";
        public string _expiresInKey = "expiresin";
        public string _refreshTokenKey = "refreshtoken";
        public string _yahooguidKey = "yahooguid";
        public string _authCodeKey = "authorizationcode";
        public string _sessionIdKey = "sessionid";
        public string _sessionIdStringKey = "sessionid_string";
        public string _sessionIdIntKey = "sessionid_int";


        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-2.1
        public const string SessionKeyName = "_Name";
        public const string SessionKeyAge = "_Age";
        const string SessionKeyTime = "_Time";

        public string SessionInfo_Name { get; private set; }
        public string SessionInfo_Age { get; private set; }
        public string SessionInfo_CurrentTime { get; private set; }
        public string SessionInfo_SessionTime { get; private set; }
        public string SessionInfo_MiddlewareValue { get; private set; }




        public YahooAuthController(IOptions<YahooConfiguration> yahooConfig, IHttpContextAccessor contextAccessor)
        {
            _yahooConfig     = yahooConfig.Value;
            _contextAccessor = contextAccessor;
        }

        public YahooAuthController() {}



        [Route("test")]
        public void ViewYahooAuthHomePage()
        {
            _h.StartMethod();
            GetYahooAccessTokenResponse();
        }

        [Route("test2")]
        public void ViewYahooAuthHomePage2()
        {
            _h.StartMethod();
            ExchangeRefreshTokenForNewAccessToken();
        }

        public YahooConfiguration GetYahooConfigInfo()
        {
            var yJson = _newtonHandler.Convert(yahooConfigFilePath, _yahooConfig.GetType()) as YahooConfiguration;
            Console.WriteLine(yJson.AppId);
            return yJson;
        }


        // STEP 1
        /// <summary>
        ///     Triggers browser to open window asking user to authorize usage of their data in the app.
        ///     When the user approves, they will receive a short authorization code.
        ///     This code should then be entered in the terminal
        /// </summary>
        /// <returns>
        ///     AuthorizationCode which is a string entered in the console
        /// </returns>
        public string GenerateUserAuthorizationCode ()
        {
            // _h.StartMethod();

            string requestUrl = $"https://api.login.yahoo.com/oauth2/request_auth?client_id={_yahooConfig.ClientId}&redirect_uri={_yahooConfig.RedirectUri}&response_type=code&language=en-us";

            // get approval code from console input
            // the code comes from https://api.login.yahoo.com/oauth2/authorize
            Process.Start("open", requestUrl);
                _h.Spotlight("Enter Code:");
                string authorizationCodeFromConsole = Console.ReadLine();
                // _h.Intro(authorizationCodeFromConsole, "code entered in console");

            SetSessionAuthorizationCode(authorizationCodeFromConsole);

            // _h.CompleteMethod();
            return authorizationCodeFromConsole;
        }


        // STEP 2
        /// <summary>
        ///     Generate the 'request' for authorization / Then convert it to a JObject
        /// </summary>
        /// <returns> The HttpWebRequest for authorization converted to a JObject </returns>
        public JObject CreateYahooAccessTokenResponseJObject()
        {
            // _h.StartMethod();

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
            // _h.Dig(request);

            // Set type to POST ---> changes 'Method' of HttpWebRequest to 'POST'
            request.Method = "POST";

            // changes 'Content Type' of HttpWebRequest to 'application/x-www-form-urlencoded'
            request.ContentType = "application/x-www-form-urlencoded";

            // produces very Base64Encoded version of consumerKey and yahooClientSecrent(it's long string of letters and numbers) ___ HEADER BYTE ---> System.Byte[] ___ e.g., '"ZGoweUpt...etc."'
            byte[] headerByte = System.Text.Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);

            // converts 'headerByte'; same letters and numbers but new format
            // e.g., 'ZGoweUpt...etc.'
            string headerString = System.Convert.ToBase64String(headerByte);

            // returns two lines
                // Content-Type: application/x-www-form-urlencoded
                // 'Authorization: Basic <headerString> ___ e.g., <headerString> = ZGoweUpt...etc.
            request.Headers["Authorization"] = "Basic " + headerString;

            // Create the data we want to send
            // 'data' is a concatanated string of all of the data.Append items
            StringBuilder data = new StringBuilder();
                data.Append("?client_id=" + consumerKey);
                data.Append("&client_secret=" + consumerSecret);
                data.Append("&redirect_uri=" + redirectUri);
                data.Append("&code=" + authorizationCodeFromConsole);
                data.Append("&grant_type=authorization_code");

            // Create a byte array of the data we want to send ___ BYTE DATA ---> System.Byte[]
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the request headers
            // changes 'Content Length' of HttpWebRequest to 218
            request.ContentLength = byteData.Length;

            // Write data to stream
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
                    // Get the response stream
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    responseFromServer = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                // if error ---> 'The remote server returned an error: (400) Bad Request.'
                // Console.WriteLine(ex.Message);
                ex.SetContext("response from server", responseFromServer);
            }

            JObject responseToJson = JObject.Parse(responseFromServer);
            _h.Dig(responseToJson);

            // REQUEST ---> following are added through beginning to end: ContentLength, HaveResponse, Method, ContentType
            // _h.CompleteMethod();
            return responseToJson;
        }


        // STEP 2B: not required. But can be used if you just want to generate the request and not convert to a JObject
        public HttpWebRequest CreateYahooAccessTokenRequest()
        {
            _h.StartMethod();

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

            // _h.CompleteMethod();
            _h.Dig(request);
            return request;
        }



        public HttpWebRequest ExchangeRefreshTokenForNewAccessToken()
        {
            var consumerKey    = _yahooConfig.ClientId;
            var consumerSecret = _yahooConfig.ClientSecret;
            var redirectUri    = _yahooConfig.RedirectUri;

            // var refreshTokenCheck = HttpContext.Session.GetString(_refreshTokenKey);
            var refreshTokenCheck = "INSERT REFRESH TOKEN HERE";
            Console.WriteLine(refreshTokenCheck);
            var grantType = "refresh_token";

            // var authorizationCodeFromConsole = GenerateUserAuthorizationCode();
            Uri address = new Uri("https://api.login.yahoo.com/oauth2/get_token");
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] headerByte = System.Text.Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);
            string headerString = System.Convert.ToBase64String(headerByte);
            request.Headers["Authorization"] = "Basic " + headerString;

            StringBuilder data = new StringBuilder();
                data.Append("?client_id=" + consumerKey);
                data.Append("&client_secret=" + consumerSecret);
                data.Append("&redirect_uri=" + redirectUri);
                data.Append("&refresh_token=" + refreshTokenCheck);
                data.Append("&grant_type=" + grantType);

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
                ex.SetContext("response from server", responseFromServer);
            }

            JObject responseToJson = JObject.Parse(responseFromServer);
            _h.Dig(responseToJson);
            // _h.Dig(request);
            return request;
        }



        // STEP 3
        /// <summary>
        ///     Retrieve response from Yahoo.
        ///     Create a new instance of AccessTokenResponse from yahoo response
        /// </summary>
        /// <returns> A new AccessTokenResponse that includes: AccessToken, TokenType, ExpiresIn, RefreshToken, XOAuthYahooGuid</returns>
        public AccessTokenResponse GetYahooAccessTokenResponse ()
        {
            // CheckYahooSession();

            // _h.StartMethod();
            JObject responseToJson = CreateYahooAccessTokenResponseJObject();

            // BaseballScraper.Models.Configuration.AccessTokenResponse
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

            // PrintAccessTokenResponseDetails(newAccessTokenResponse);
            // set the items in session
            SetSessionAccessTokenItems(newAccessTokenResponse);

            // _h.CompleteMethod();
            return newAccessTokenResponse;
        }








        #region MANAGE SESSION ------------------------------------------------------------





            /// <summary>
            ///     Takes the authorization code and sets it within session
            ///     This is called within the GenerateUserAuthorizationCode method
            ///     This method / step is not required
            /// </summary>
            public void SetSessionAuthorizationCode(string authorizationCodeFromConsole)
            {
                // _h.StartMethod();
                // Console.WriteLine($"AUTH > SetSessionAuthorizationCode: {authorizationCodeFromConsole}");
                var authCodeCheck = "";

                try
                {
                    HttpContext.Session.SetString(_authCodeKey, authorizationCodeFromConsole);
                        authCodeCheck = HttpContext.Session.GetString(_authCodeKey);
                        // _h.Intro(authCodeCheck, "setting auth code as");

                    HttpContext.Session.SetInt32(_sessionIdKey,1);

                        int? sessionId = HttpContext.Session.GetInt32(_sessionIdKey);
                        _h.Intro(sessionId, "setting session code as");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR IN: YahooAuthController.SetSessionAuthorizationCode()");
                    // PrintExceptionMessageInfo(ex);
                }
            }



            public void SetSessionAccessTokenItems(AccessTokenResponse newAccessTokenResponse)
            {
                try
                {
                    HttpContext.Session.SetString(_accessTokenKey, newAccessTokenResponse.AccessToken);
                    HttpContext.Session.SetString(_tokenTypeKey, newAccessTokenResponse.TokenType);
                    // HttpContext.Session.SetString(_expiresInKey, newAccessTokenResponse.ExpiresIn);
                    HttpContext.Session.SetString(_refreshTokenKey, newAccessTokenResponse.RefreshToken);
                    HttpContext.Session.SetString(_yahooguidKey, newAccessTokenResponse.XOAuthYahooGuid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR IN: YahooAuthController.SetSessionAccessTokenItems()");
                    // PrintExceptionMessageInfo(ex);
                }

                // CheckYahooSession();
            }

            public void GetSessionAccessTokenItems()
            {
                try
                {
                    var authCodeCheck     = HttpContext.Session.GetString(_authCodeKey);
                    var accessTokenCheck  = HttpContext.Session.GetString(_accessTokenKey);
                    var refreshTokenCheck = HttpContext.Session.GetString(_refreshTokenKey);
                    var yahooGuidCheck    = HttpContext.Session.GetString(_yahooguidKey);
                    // var sessionIdCheck    = HttpContext.Session.GetInt32(_sessionIdKey).ToString();
                    int? sessionIdCheck    = HttpContext.Session.GetInt32(_sessionIdKey);
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("ERROR IN: YahooAuthController.GetSessionAccessTokenItems()");
                    // PrintExceptionMessageInfo(ex);
                }

                // CheckYahooSession();
            }

            public void PersistSession()
            {
                _h.Highlight("### PERSISTING SESSION ###");
                var authCodeCheck     = HttpContext.Session.GetString(_authCodeKey);
                var accessTokenCheck  = HttpContext.Session.GetString(_accessTokenKey);
                var refreshTokenCheck = HttpContext.Session.GetString(_refreshTokenKey);
                var yahooGuidCheck    = HttpContext.Session.GetString(_yahooguidKey);
                // var sessionIdCheck    = HttpContext.Session.GetInt32(_sessionIdKey).ToString();
                int? sessionIdCheck    = HttpContext.Session.GetInt32(_sessionIdKey);

                HttpContext.Session.SetString(_authCodeKey, authCodeCheck);
                HttpContext.Session.SetString(_accessTokenKey, accessTokenCheck);
                HttpContext.Session.SetString(_refreshTokenKey, refreshTokenCheck);
                HttpContext.Session.SetString(_yahooguidKey, yahooGuidCheck);
                // HttpContext.Session.SetString(_sessionIdKey, sessionIdCheck);
                HttpContext.Session.SetInt32(_sessionIdKey, (int)sessionIdCheck);
            }



            // TODO: Confirm whether or not this works
            public void CheckYahooSession(HttpResponseMessage response = null)
            {
                int? sessionId = HttpContext.Session.GetInt32(_sessionIdKey);
                Console.WriteLine($"sessionId {sessionId}");

                if(sessionId == 0)
                {
                    Console.WriteLine("sessionId = 0");
                }
                if(sessionId == null)
                {
                    Console.WriteLine("sessionId = null");
                }
                try
                {
                    if(HttpContext.Session.GetString(_authCodeKey) == null)
                    {
                        // Console.WriteLine("### NO CURRENT SESSION ###");
                        var initialAuthCodeCheck = HttpContext.Session.GetString(_authCodeKey);
                        // _h.Intro(initialAuthCodeCheck, "initial auth code check");
                    }

                    else
                    {
                        // Console.WriteLine("### SESSION IN PROGRESS ###");
                        PersistSession();
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine("ERROR IN: YahooAuthController.CheckYahooSession() > ERROR 1");
                    // PrintExceptionMessageInfo(ex);

                    try
                    {
                        _h.Spotlight("trying more...");
                        throw new RequestFailedException(String.Format("HTTP error. Request Message - '{0}', Content - {1}, Status Code - {2}", response.RequestMessage, response.Content != null ? response.Content.ReadAsStringAsync().Result : "", response.StatusCode), response);
                    }

                    catch (Exception ex2)
                    {
                        Console.WriteLine("ERROR IN: YahooAuthController.CheckYahooSession() > ERROR 2");
                        // PrintExceptionMessageInfo(ex2);
                    }
                }
            }


            public Dictionary<string, string> CreateSessionInfoDictionary()
            {
                List       <string> messagesList                 = new List<string>();
                Dictionary<string, string> sessionInfoDictionary = new Dictionary<string, string>();

                try
                {
                    if(CheckSession() == 0)
                    {
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


            public int CheckSession()
            {
                int? sessionId;
                try
                {
                    sessionId = HttpContext.Session.GetInt32(_sessionIdKey);
                    _h.Highlight($"### SESSION EXISTS ### > id = {sessionId}");
                    return (int)sessionId;
                }
                catch(Exception ex)
                {
                    _h.Highlight("### SESSION DOES NOT EXIST ###");
                    // PrintExceptionMessageInfo(ex);
                    return 0;
                }
            }

            public int CheckSessionId()
            {
                int? sessionId = HttpContext.Session.GetInt32("sessionid");
                Console.WriteLine($"CheckSessionId() > {sessionId}");

                if(sessionId == null)
                {
                    Console.WriteLine($"CHECK SESSION ID IS NULL!!!!");
                    return 0;
                }
                Console.WriteLine($"CHECK SESSION ID: {sessionId}");
                return (int)sessionId;
            }


        #endregion MANAGE SESSION ------------------------------------------------------------



            // try{
            //     int? sessionId = HttpContext.Session.GetInt32(_sessionIdKey);
            //     if(sessionId > 0)
            //     {
            //         Console.WriteLine(" NOT NULL ");
            //     }
            //     else Console.WriteLine(" NULL ");
            // }
            // catch








        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintKeySecretAndRedirectUri(string consumerKey, string consumerSecret, string redirectUri)
            {
                Console.WriteLine($"consumerKey: {consumerKey}");
                Console.WriteLine($"consumerSecret: {consumerSecret}");
                Console.WriteLine($"redirectUri: {redirectUri}");
            }


            public void PrintAccessTokenResponseDetails(AccessTokenResponse ATR)
            {
                Console.WriteLine();
                Console.WriteLine("------------------------------------");
                Console.WriteLine("### ACCESS TOKEN RESPONSE DETAILS ###");
                Console.WriteLine("------------------------------------");
                Console.WriteLine("ACCESS TOKEN");
                Console.WriteLine(ATR.AccessToken);
                Console.WriteLine();
                Console.WriteLine("TOKEN TYPE");
                Console.WriteLine(ATR.TokenType);
                Console.WriteLine();
                Console.WriteLine("EXPIRES IN");
                Console.WriteLine(ATR.ExpiresIn);
                Console.WriteLine();
                Console.WriteLine("REFRESH TOKEN");
                Console.WriteLine(ATR.RefreshToken);
                Console.WriteLine();
                Console.WriteLine("XO AUTH YAHOO GUID");
                Console.WriteLine(ATR.XOAuthYahooGuid);
                Console.WriteLine();
                Console.WriteLine("------------------------------------");
                Console.WriteLine();
            }


            public void PrintExceptionMessageInfo(Exception ex)
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine();
            }


            public void PrintYahooConfigInfo()
            {
                Console.WriteLine(_yahooConfig.Name);
                Console.WriteLine(_yahooConfig.ClientId);
                Console.WriteLine(_yahooConfig.ClientSecret);
                Console.WriteLine(_yahooConfig.RequestAuthUri);
                Console.WriteLine(_yahooConfig.AppId);
                Console.WriteLine();
            }


            public class RequestFailedException: Exception
            {
                HttpResponseMessage Response;

                public RequestFailedException(): base() { }

                public RequestFailedException(string message): base(message) { }

                public RequestFailedException(string message, HttpResponseMessage response): base(message) { Response = response; }

                public RequestFailedException(string message, Exception innerException): base(message, innerException) { }
            }

        #endregion PRINTING PRESS ------------------------------------------------------------






        #region OTHER SESSION THINGS



            // public static class SessionExtensions
            // {


            //     public static void Set<T>(this ISession session, string key, T value)
            //     {
            //         session.SetString(key, JsonConvert.SerializeObject(value));
            //     }


            //     public static T Get<T>(this ISession session, string key)
            //     {
            //         var value = session.GetString(key);
            //         return value == null ? default(T) :
            //             JsonConvert.DeserializeObject<T>(value);
            //     }

            // }


            // public void OnGet()
            // {
            //     if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
            //     {
            //         HttpContext.Session.SetString(SessionKeyName, "The Doctor");
            //         HttpContext.Session.SetInt32(SessionKeyAge, 773);
            //     }
            // }


            // public static void OnGetDateTime()
            // {
            //     if (HttpContext.Session.Get<DateTime>(SessionKeyTime) == default(DateTime))
            //     {
            //         HttpContext.Session.Set<DateTime>(SessionKeyTime, currentTime);
            //     }
            // }

            // public static void SetObjectAsJson(this ISession session, string key, object value)
            // {
            //     _h.StartMethod();
            //     session.SetString(key, JsonConvert.SerializeObject(value));
            //     CompleteMethod();
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
