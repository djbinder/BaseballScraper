using System;
using System.Diagnostics;
using LinqToTwitter;

namespace BaseballScraper.Models.Twitter
{
    public class TwContext
    {
        private static String _start    = "STARTED";
        private static String _complete = "COMPLETED";
        public static string Start { get => _start; set => _start = value; }
        public static string Complete { get => _complete; set => _complete = value; }

        // aka 'oauth_consumer_key' and 'ConsumerKey'
        private static String _apiKey = "ptSiuGtIpFnCz7nAbzH7EidKQ";

        // aka 'ConsumerSecret'
        private static String _apiSecretKey = "EQLjfoQNg2luDSEIC0j8ehpopi2ACbfdl0tIpumkLCsaGgBAev";

        // aka 'oauth_token', 'AccessToken', and 'OAuthToken'
        private static String _accessToken = "100554390-KFStxew66kJrgIkH8HY6zTaOBOEh9W6Otfe7bDTO";

        // aka 'AccessTokenSecret', 'OAuthTokenSecret'
        private static String _accessTokenSecret = "0yqldliDxx0vadWQ1I2akX3X6tiBqSAiA58lhLpoBcnmM";


        public static string ApiKey { get => _apiKey; set => _apiKey = value; }
        public static string ApiSecretKey { get => _apiSecretKey; set => _apiSecretKey = value; }
        public static string AccessToken { get => _accessToken; set => _accessToken = value; }
        public static string AccessTokenSecret { get => _accessTokenSecret; set => _accessTokenSecret = value; }



        public SingleUserAuthorizer AuthorizeTwitterUser()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey       = ApiKey,
                    ConsumerSecret    = ApiSecretKey,
                    AccessToken       = AccessToken,
                    AccessTokenSecret = AccessTokenSecret
                }
            };
            return auth;
        }


        public PinAuthorizer NewPinAuthorizer()
        {
            Start.ThisMethod();
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey    = ApiKey,
                    ConsumerSecret = ApiSecretKey,
                },


                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin                   = () =>
                {
                    Console.WriteLine(
                        "\nAfter authorizing this application, Twitter " +
                        "will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };

            Console.WriteLine(auth);
            Complete.ThisMethod();
            return auth;
        }
    }
}





