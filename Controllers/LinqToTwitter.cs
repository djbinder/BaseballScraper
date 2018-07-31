using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Net.Http;
using Microsoft.AspNetCore.Identity;
using BaseballScraper.Models;
using BaseballScraper.Services.Security.Extensions;
using BaseballScraper.Services.Security;
using Newtonsoft.Json;
using BaseballScraper.Models.Twitter;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BaseballScraper
{
    public class LinqToTwitterController: Controller
    {
        private TwitterContext _twitterContext;
        private TwContext _twContext;

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

        private readonly AppIdentitySettings _identity;

        public LinqToTwitterController(IOptions<AppIdentitySettings> appSettingsAccessor)
        {
            _identity = appSettingsAccessor.Value;
        }

        public static SingleUserAuthorizer AuthorizeTwitterUser()
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

        public async Task TwitterStringSearch (String searchString)
        {
            //  AUTHORIZED USER ---> LinqToTwitter.SingleUserAuthorizer
            var authorizedUser = AuthorizeTwitterUser();

            //  TWITTER CTX ---> LinqToTwitter.TwitterContext
            var twitterCtx = new TwitterContext(authorizedUser);

            //  SEARCH RESPONSE ---> LinqToTwitter.Search
            var searchResponse = 
                await
                    (from search in twitterCtx.Search
                    where search.Type == SearchType.Search &&
                    search.Query == searchString
                    select search)
                    .SingleOrDefaultAsync();

            if (searchResponse != null && searchResponse.Statuses != null)
            {
                // searchResponse.Statuses.ForEach(tweet =>
                //         Console.WriteLine(
                //         "User: {0}, Tweet: {1}",
                //         tweet.User.ScreenNameResponse,
                //         tweet.Text)
                //         );

                int searchResponseCount = searchResponse.Statuses.Count();
                searchResponseCount.Intro("search response count");

                List<TwitterStatus> statuses = new List<TwitterStatus>();

                for (var i = 0; i <=searchResponseCount - 1; i++)
                {
                    Extensions.Spotlight("start of new loop");
                    i.Intro("loop number");
                    TwitterStatus newStatus = new TwitterStatus
                    {
                        ScreenName     = searchResponse.Statuses[i].ScreenName,
                        StatusType     = (int)searchResponse.Statuses[i].Type,
                        UserId         = (int)searchResponse.Statuses[i].UserID,
                        CreatedAt      = searchResponse.Statuses[i].CreatedAt,
                        StatusIdString = searchResponse.Statuses[i].StatusID,
                        Text           = searchResponse.Statuses[i].Text

                    };

                    statuses.Add(newStatus);
                }

                // int xCount = 1;
                // foreach(var item in statuses)
                // {
                //     Extensions.Spotlight("next status");
                //     xCount.Intro("status #");
                //     item.ScreenName.Intro("screen name");
                //     item.StatusType.Intro("status type");
                //     item.UserId.Intro("user id");
                //     item.CreatedAt.Intro("created at");
                //     item.StatusIdString.Intro("status id string");
                //     item.Text.Intro("text");

                //     xCount++;
                // }
            }
        }
    }
}





// to get 'TwitterStringSearch' to work
// try {
//     linqToTwitter.TwitterStringSearch("Anthony Rizzo").Wait();
// }

// catch (Exception ex)
// {
//     Console.WriteLine(ex);
//     Console.WriteLine("error");
// }