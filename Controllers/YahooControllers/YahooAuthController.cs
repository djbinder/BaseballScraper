using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.ConfigurationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using ObjectPrinter;



#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE0063, IDE0067, IDE1006, MA0051
namespace BaseballScraper.Controllers.YahooControllers
{
    [Route("api/yahoo/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class YahooAuthController: ControllerBase
    {
        private readonly Helpers              _helpers;
        private readonly YahooConfiguration   _yahooConfig;
        private readonly IHttpContextAccessor _contextAccessor;
        private static readonly JsonHandler.NewtonsoftJsonHandlers _newtonHandler = new JsonHandler.NewtonsoftJsonHandlers();

        public static readonly string yahooConfigFilePath = "Configuration/yahooConfig.json";

        private string _accessTokenKey  = "accesstoken";
        private string _tokenTypeKey    = "tokentype";
        private string _expiresInKey    = "expiresin";
        private string _refreshTokenKey = "refreshtoken";
        private string _yahooguidKey    = "yahooguid";
        private string _authCodeKey     = "authorizationcode";
        private string _sessionIdKey    = "sessionid";



        public YahooAuthController(Helpers helpers, IOptions<YahooConfiguration> yahooConfig, IHttpContextAccessor contextAccessor)
        {
            _helpers         = helpers;
            _yahooConfig     = yahooConfig.Value;
            _contextAccessor = contextAccessor;
        }

        public YahooAuthController() {}



        [Route("test")]
        public void ViewYahooAuthHomePage()
        {
            _helpers.StartMethod();
            // bool userHasRefreshToken = CheckIfUserHasExistingRefreshToken();
            PrintYahooConfigInfo();
            var code = GenerateUserAuthorizationCode();
            // ExchangeRefreshTokenForNewAccessTokenJObject();
        }




        #region CHECK IF USER HAS ALREADY AUTHORIZED THE APP ------------------------------------------------------------


            // STATUS [ June 8, 2019 ] : this works
            /// <summary>
            ///     Check if user has existing refresh token
            ///     If they do have an RT, they have already authorized the app; If they do not they must authorize it
            ///         PATH 1: Authorize for first time
            ///         PATH 2: User is already authorized
            /// </summary>
            private bool CheckIfUserHasExistingRefreshToken()
            {
                var refreshTokenCheck = _yahooConfig.RefreshToken;
                Console.WriteLine($"refreshTokenCheck: {refreshTokenCheck}");

                bool userHasRefreshToken;

                // User DOES NOT have a refresh token
                if(refreshTokenCheck == null)
                {
                    userHasRefreshToken = false;
                    // _helpers.Highlight("### USER DOES NOT HAVE EXISTING REFRESH TOKEN ###");
                    return userHasRefreshToken;
                }
                // User DOES have a refresh token
                else
                {
                    userHasRefreshToken = true;
                    // _helpers.Highlight("### USER HAS EXISTING REFRESH TOKEN ###");
                    return userHasRefreshToken;
                }
            }


            // STATUS [ June 8, 2019 ] : this works but not sure if needed
            public YahooConfiguration GetYahooConfigInfo()
            {
                return _newtonHandler.Convert(yahooConfigFilePath, _yahooConfig.GetType()) as YahooConfiguration;
            }


        #endregion CHECK IF USER HAS ALREADY AUTHORIZED THE APP ------------------------------------------------------------





        // PATH 1: Authorize for first time
        // See steps 1 - 4 here: https://developer.yahoo.com/oauth2/guide/flows_authcode/
        #region AUTHORIZE FIRST TIME USER OF YAHOO APP ------------------------------------------------------------


            // STATUS [ June 8, 2019 ] : this works
            // STEP 1
            /// <summary>
            /// * Triggers browser to open window asking user to authorize usage of their data in the app.
            /// * When the user approves, they will receive a short authorization code.
            /// * This code should then be entered in the terminal
            /// * This is applicable to a first time user (i.e., a user w/o a refresh token)
            /// </summary>
            /// <returns>
            ///     AuthorizationCode which is a string entered in the console
            /// </returns>
            private string GenerateUserAuthorizationCode ()
            {
                // _helpers.StartMethod();
                string requestUrl = $"https://api.login.yahoo.com/oauth2/request_auth?client_id={_yahooConfig.ClientId}&redirect_uri={_yahooConfig.RedirectUri}&response_type=code&language=en-us";

                // get approval code from console input
                // the code comes from https://api.login.yahoo.com/oauth2/authorize
                Process.Start("open", requestUrl);
                    _helpers.Spotlight("Enter Code:");
                    string authorizationCodeFromConsole = Console.ReadLine();
                    _helpers.Intro(authorizationCodeFromConsole, "code entered in console");

                SetSessionAuthorizationCode(authorizationCodeFromConsole);
                // _helpers.CompleteMethod();
                return authorizationCodeFromConsole;
            }



            // STATUS [ June 8, 2019 ] : this works
            // STEP 2
            /// <summary>
            ///     Generate the 'request' for authorization / Then convert it to a JObject
            ///     This is for brand new users as it calls the 'GenerateUserAuthorizationCode()' method
            ///     This is applicable to a first time user (i.e., a user w/o a refresh token)
            /// </summary>
            /// <returns> The HttpWebRequest for authorization converted to a JObject </returns>
            private JObject CreateYahooAccessTokenResponseJObject()
            {
                // _helpers.StartMethod();
                // consumerKey and consumerSecret are unique to each yahoo app; Here, they are called from Secrets/ Config File
                string consumerKey    = _yahooConfig.ClientId;
                string consumerSecret = _yahooConfig.ClientSecret;
                string redirectUri    = _yahooConfig.RedirectUri;

                // generated from users input into console; it's a seven character string
                string authorizationCodeFromConsole = GenerateUserAuthorizationCode();

                // Exchange authorization code for Access Token by sending Post Request
                Uri address = new Uri("https://api.login.yahoo.com/oauth2/get_token");

                // Create the web request ___ REQUEST ---> System.Net.HttpWebRequest
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                // _helpers.Dig(request);

                // Set type to POST ---> changes 'Method' of HttpWebRequest to 'POST'
                request.Method = "POST";

                // changes 'Content Type' of HttpWebRequest to 'application/x-www-form-urlencoded'
                request.ContentType = "application/x-www-form-urlencoded";

                // produces very Base64Encoded version of consumerKey and yahooClientSecrent(it's long string of letters and numbers) ___ HEADER BYTE ---> System.Byte[] ___ e.g., '"ZGoweUpt...etc."'
                byte[] headerByte = Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);

                // converts 'headerByte'; same letters and numbers but new format
                // e.g., 'ZGoweUpt...etc.'
                string headerString = Convert.ToBase64String(headerByte);

                // returns two lines
                    // Content-Type: application/x-www-form-urlencoded
                    // 'Authorization: Basic <headerString> ___ e.g., <headerString> = ZGoweUpt...etc.
                request.Headers["Authorization"] = "Basic " + headerString;

                // Create the data we want to send
                // 'data' is a concatanated string of all of the data.Append items
                StringBuilder data = new StringBuilder();
                    data.Append("?client_id=").Append(consumerKey);
                    data.Append("&client_secret=").Append(consumerSecret);
                    data.Append("&redirect_uri=").Append(redirectUri);
                    data.Append("&code=").Append(authorizationCodeFromConsole);
                    data.Append("&grant_type=authorization_code");

                // Create a byte array of the data we want to send ___ BYTE DATA ---> System.Byte[]
                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                // Set the content length in the request headers
                // changes 'Content Length' of HttpWebRequest to 218
                request.ContentLength = byteData.Length;

                // Write data to stream
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.WriteAsync(byteData, 0, byteData.Length);
                }

                // Get response
                string responseFromServer = "";
                Task<string> responseFromServerTask;

                try
                {
                    // changes 'HaveResponse' of HttpWebRequest to 228
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        // responseFromServer = reader.ReadToEnd();
                        responseFromServerTask = reader.ReadToEndAsync();
                        reader.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    // if error ---> 'The remote server returned an error: (400) Bad Request.'
                    // Console.WriteLine(ex.Message);
                    ex.SetContext("response from server", responseFromServer);
                }

                JObject responseToJson = JObject.Parse(responseFromServer);
                // _helpers.Dig(responseToJson);

                // REQUEST ---> following are added through beginning to end: ContentLength, HaveResponse, Method, ContentType
                // _helpers.CompleteMethod();
                return responseToJson;
            }


            // STATUS [ June 8, 2019 ] : this works
            // STEP 2B: not required. But can be used if you just want to generate the request and not convert to a JObject
            /// <summary>
            /// * Generate the HttpWebRequest for authorization
            /// * This is for brand new users as it calls the 'GenerateUserAuthorizationCode()' method
            /// * This is applicable to a first time user (i.e., a user w/o a refresh token)
            /// </summary>
            private HttpWebRequest CreateYahooAccessTokenRequest()
            {
                _helpers.StartMethod();

                // consumerKey and consumerSecret are unique to each yahoo app; Here, they are called from Secrets/ Config File
                string consumerKey    = _yahooConfig.ClientId;
                string consumerSecret = _yahooConfig.ClientSecret;
                string redirectUri    = _yahooConfig.RedirectUri;

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
                byte[] headerByte = Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);

                // converts 'headerByte'; same letters and numbers but new format ___ HEADER STRING ---> System.String ___ e.g., 'ZGoweUpt...etc.'
                string headerString = Convert.ToBase64String(headerByte);

                // returns two lines
                // 1) Content-Type: application/x-www-form-urlencoded
                // 2) 'Authorization: Basic <headerString> ___ e.g., <headerString> = ZGoweUpt...etc.
                request.Headers["Authorization"] = "Basic " + headerString;

                // Create the data we want to send ___ DATA ---> System.Text.StringBuilder ___ 'data' is a concatanated string of all of the data.Append items
                StringBuilder data = new StringBuilder();
                    data.Append("?client_id=").Append(consumerKey);
                    data.Append("&client_secret=").Append(consumerSecret);
                    data.Append("&redirect_uri=").Append(redirectUri);
                    data.Append("&code=").Append(authorizationCodeFromConsole);
                    data.Append("&grant_type=authorization_code");

                // Create a byte array of the data we want to send
                byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());

                // * Set the content length in the request headers
                // * Changes 'Content Length' of HttpWebRequest to 218
                request.ContentLength = byteData.Length;

                return request;
            }


        #endregion AUTHORIZE FIRST TIME USER OF YAHOO APP ------------------------------------------------------------





        // PATH 2: User is already authorized
        // See step 5 here: https://developer.yahoo.com/oauth2/guide/flows_authcode/
        #region RE-AUTHORIZE EXISTING USER OF YAHOO APP ------------------------------------------------------------


            // STATUS [ June 8, 2019 ] : this works
            /// <summary>
            ///     If user has a refresh token, this is the path
            ///     Yahoo API auth process step 5: 'Exchange refresh token for new access token'
            ///     Yahoo description: After the access token expires, you can use the refresh token, which has a long lifetime, to get a new access token.
            ///     For notes / comments on this method check it's 'sister' method (CreateYahooAccessTokenResponseJObject) in
            /// </summary>
            private JObject ExchangeRefreshTokenForNewAccessTokenJObject()
            {
                string consumerKey       = _yahooConfig.ClientId;
                string consumerSecret    = _yahooConfig.ClientSecret;
                string redirectUri       = _yahooConfig.RedirectUri;
                string grantType         = "refresh_token";
                string refreshTokenCheck = _yahooConfig.RefreshToken;
                Console.WriteLine(refreshTokenCheck);

                Uri address = new Uri("https://api.login.yahoo.com/oauth2/get_token");

                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                request = ExchangeRefreshTokenRequestAppender(
                    request,
                    consumerKey,
                    consumerSecret
                );

                StringBuilder data = ExchangeRefreshTokenStringbuilder(
                    consumerKey,
                    consumerSecret,
                    redirectUri,
                    refreshTokenCheck,
                    grantType
                );

                byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());

                request.ContentLength = byteData.Length;

                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.WriteAsync(byteData, 0, byteData.Length);
                }

                string responseFromServer = "";
                Task<string> responseFromServerTask;

                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        // responseFromServer = reader.ReadToEnd();
                        responseFromServerTask = reader.ReadToEndAsync();
                        reader.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    ex.SetContext("response from server", responseFromServer);
                }

                JObject responseToJson = JObject.Parse(responseFromServer);
                return responseToJson;
            }


            // STATUS [ June 8, 2019 ] : this works
            /// <summary>
            ///     This is a condensed version of 'ExchangeRefreshTokenForNewAccessTokenJObject()' and returns a HttpWebRequest instead of a JObject
            ///     If user has a refresh token, this is the path
            ///     Yahoo API auth process step 5: 'Exchange refresh token for new access token'
            ///     Yahoo description: After the access token expires, you can use the refresh token, which has a long lifetime, to get a new access token.
            ///     For notes / comments on this method check it's 'sister' method (CreateYahooAccessTokenResponseJObject) in
            /// </summary>
            private HttpWebRequest ExchangeRefreshTokenForNewAccessToken()
            {
                string consumerKey       = _yahooConfig.ClientId;
                string consumerSecret    = _yahooConfig.ClientSecret;
                string redirectUri       = _yahooConfig.RedirectUri;
                string grantType         = "refresh_token";
                string refreshTokenCheck = _yahooConfig.RefreshToken;
                // Console.WriteLine(refreshTokenCheck);

                Uri address = new Uri("https://api.login.yahoo.com/oauth2/get_token");

                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                request = ExchangeRefreshTokenRequestAppender(
                    request,
                    consumerKey,
                    consumerSecret
                );

                StringBuilder data = ExchangeRefreshTokenStringbuilder(
                    consumerKey,
                    consumerSecret,
                    redirectUri,
                    refreshTokenCheck,
                    grantType
                );

                byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());

                request.ContentLength = byteData.Length;
                return request;
            }


            // STATUS [ June 8, 2019 ] : this works
            /// <summary>
            ///     Stringbuilder for:
            ///     1) ExchangeRefreshTokenForNewAccessTokenJObject()
            ///     2) ExchangeRefreshTokenForNewAccessToken()
            /// </summary>
            private StringBuilder ExchangeRefreshTokenStringbuilder(string consumerKey, string consumerSecret, string redirectUri, string refreshTokenCheck, string grantType)
            {
                StringBuilder data = new StringBuilder();
                    data.Append("?client_id=").Append(consumerKey);
                    data.Append("&client_secret=").Append(consumerSecret);
                    data.Append("&redirect_uri=").Append(redirectUri);
                    data.Append("&refresh_token=").Append(refreshTokenCheck);
                    data.Append("&grant_type=").Append(grantType);

                return data;
            }


            // STATUS [ June 8, 2019 ] : this works
            /// <summary>
            ///     Appends multiple things to HttpWebRequest for methods:
            ///     1) ExchangeRefreshTokenForNewAccessTokenJObject()
            ///     2) ExchangeRefreshTokenForNewAccessToken()
            /// </summary>
            private HttpWebRequest ExchangeRefreshTokenRequestAppender(HttpWebRequest request, string consumerKey, string consumerSecret)
            {
                request.Method      = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] headerByte   = Encoding.UTF8.GetBytes(consumerKey+":"+consumerSecret);
                string headerString = Convert.ToBase64String(headerByte);
                request.Headers["Authorization"] = "Basic " + headerString;

                return request;
            }


        #endregion RE-AUTHORIZE EXISTING USER OF YAHOO APP ------------------------------------------------------------





        #region GENERATE ACCESS TOKEN RESPONSE ------------------------------------------------------------


            // NOTE: this is ultimately the method that gets called by other controllers to ensure user has authorized the app
            // STATUS [ June 8, 2019 ] : this works
            // STEP 3
            /// <summary>
            ///     Retrieve response from Yahoo.
            ///     Create a new instance of AccessTokenResponse from yahoo response
            /// </summary>
            /// <returns> A new AccessTokenResponse that includes: AccessToken, TokenType, ExpiresIn, RefreshToken, XOAuthYahooGuid</returns>
            public AccessTokenResponse GetYahooAccessTokenResponse ()
            {
                // _helpers.StartMethod();
                JObject responseToJson;

                bool userHasRefreshToken = CheckIfUserHasExistingRefreshToken();
                // userHasRefreshToken = false;
                Console.WriteLine($"User has refresh token?: {userHasRefreshToken}");

                if(userHasRefreshToken == false)
                {
                    responseToJson = CreateYahooAccessTokenResponseJObject();
                    AccessTokenResponse finalAccessTokenResponse = GenerateYahooAccessTokenResponse(responseToJson);
                    _helpers.Dig(finalAccessTokenResponse);
                    return finalAccessTokenResponse;
                }

                if(userHasRefreshToken == true)
                {
                    responseToJson = ExchangeRefreshTokenForNewAccessTokenJObject();
                    AccessTokenResponse finalAccessTokenResponse = GenerateYahooAccessTokenResponse(responseToJson);
                    return finalAccessTokenResponse;
                }

                else
                {
                    Console.WriteLine("ERROR IN: YahooAuthController.GetYahooAccessTokenResponse()");
                    return null;
                }
            }


            // STATUS [ June 8, 2019 ] : this works
            /// <summary>
            ///     Receives a JObject, takes that information, and turns it into an AccessTokenResponse
            ///     Called 2x within:  'GetYahooAccessTokenResponse()'
            /// </summary>
            private AccessTokenResponse GenerateYahooAccessTokenResponse (JObject responseToJson)
            {
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
                SetSessionAccessTokenItems(newAccessTokenResponse);
                return newAccessTokenResponse;
            }


        #endregion GENERATE ACCESS TOKEN RESPONSE ------------------------------------------------------------





        #region MANAGE SESSION ------------------------------------------------------------


            /// <summary>
            ///     Takes the authorization code and sets it within session
            ///     This is called within the GenerateUserAuthorizationCode method
            ///     This method / step is not required
            /// </summary>
            public void SetSessionAuthorizationCode(string authorizationCodeFromConsole)
            {
                // _helpers.StartMethod();
                var authCodeCheck = "";

                try
                {
                    HttpContext.Session.SetString(_authCodeKey, authorizationCodeFromConsole);
                        authCodeCheck = HttpContext.Session.GetString(_authCodeKey);

                    HttpContext.Session.SetInt32(_sessionIdKey,1);
                        int? sessionId = HttpContext.Session.GetInt32(_sessionIdKey);
                }
                catch(Exception ex)
                {
                    // Console.WriteLine("ERROR IN: YahooAuthController.SetSessionAuthorizationCode()");
                    PrintExceptionMessageInfo(ex);
                }
            }


            public void SetSessionAccessTokenItems(AccessTokenResponse newAccessTokenResponse)
            {
                try
                {
                    HttpContext.Session.SetString(_accessTokenKey, newAccessTokenResponse.AccessToken);
                    HttpContext.Session.SetString(_tokenTypeKey, newAccessTokenResponse.TokenType);
                    HttpContext.Session.SetString(_expiresInKey, newAccessTokenResponse.ExpiresIn);
                    HttpContext.Session.SetString(_refreshTokenKey, newAccessTokenResponse.RefreshToken);
                    HttpContext.Session.SetString(_yahooguidKey, newAccessTokenResponse.XOAuthYahooGuid);
                }
                catch
                {
                    // Console.WriteLine();
                    // Console.WriteLine("ERROR IN: YahooAuthController.SetSessionAccessTokenItems()");
                    // PrintExceptionMessageInfo(ex);
                }
            }


            public void PersistSession()
            {
                // _helpers.Highlight("### PERSISTING SESSION ###");
                var authCodeCheck     = HttpContext.Session.GetString(_authCodeKey);
                var accessTokenCheck  = HttpContext.Session.GetString(_accessTokenKey);
                var refreshTokenCheck = HttpContext.Session.GetString(_refreshTokenKey);
                var yahooGuidCheck    = HttpContext.Session.GetString(_yahooguidKey);
                // var sessionIdCheck    = HttpContext.Session.GetInt32(_sessionIdKey).ToString();
                int? sessionIdCheck   = HttpContext.Session.GetInt32(_sessionIdKey);

                HttpContext.Session.SetString(_authCodeKey, authCodeCheck);
                HttpContext.Session.SetString(_accessTokenKey, accessTokenCheck);
                HttpContext.Session.SetString(_refreshTokenKey, refreshTokenCheck);
                HttpContext.Session.SetString(_yahooguidKey, yahooGuidCheck);
                // HttpContext.Session.SetString(_sessionIdKey, sessionIdCheck);
                HttpContext.Session.SetInt32(_sessionIdKey, (int)sessionIdCheck);
            }


        #endregion MANAGE SESSION ------------------------------------------------------------





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


            private void PrintExceptionMessageInfo(Exception ex)
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine();
            }


            private void PrintYahooConfigInfo()
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


    }
}
