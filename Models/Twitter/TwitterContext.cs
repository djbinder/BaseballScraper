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





