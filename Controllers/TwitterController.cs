using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Twitter;
using BaseballScraper.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace BaseballScraper
{
    #pragma warning disable CS0414
    public class TwitterController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        private readonly TwitterConfiguration _twitterConfig;
        private readonly AirtableConfiguration _airtableConfig;


        public TwitterController(IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig)
        {
            _airtableConfig = airtableConfig.Value;
            _twitterConfig  = twitterConfig.Value;
        }


        // THIS DOES NOT WORK
        [HttpGet]
        [Route("/consumerkey")]
        public string GetConsumerKey ()
        {
            Start.ThisMethod();
            var twitterConsumerKey = _twitterConfig.ConsumerKey;
            return twitterConsumerKey;
            // return Content($"Consumer Key Is: {twitterConsumerKey}");
        }

        [HttpGet]
        [Route("/consumersecret")]
        public string GetConsumerSecret ()
        {
            Start.ThisMethod();
            var twitterConsumerSecret = _twitterConfig.ConsumerSecret;
            return twitterConsumerSecret;
        }


        [HttpGet]
        [Route("/accesstoken")]
        public string GetAccessToken ()
        {
            Start.ThisMethod();
            var twitterAccessToken = _twitterConfig.AccessToken;
            return twitterAccessToken;
        }

        [HttpGet]
        [Route("/accesstokensecret")]
        public string GetAccessTokenSecret ()
        {
            Start.ThisMethod();
            var twitterAcccessTokenSecret = _twitterConfig.AccessTokenSecret;
            return twitterAcccessTokenSecret;
        }


        public SingleUserAuthorizer AuthorizeTwitterUser()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey       = _twitterConfig.ConsumerKey,
                    ConsumerSecret    = _twitterConfig.ConsumerSecret,
                    AccessToken       = _twitterConfig.AccessToken,
                    AccessTokenSecret = _twitterConfig.AccessTokenSecret

                }
            };
            return auth;
        }


        [HttpGet]
        [Route("/playersearch")]
        public async Task TwitterStringSearch (String searchString)
        {
            Start.ThisMethod();
            //  AUTHORIZED USER ---> LinqToTwitter.SingleUserAuthorizer
            var authorizedUser = AuthorizeTwitterUser();

            var twitterConsumerKey = _twitterConfig.ConsumerKey;
            twitterConsumerKey.Intro("consumer key");

            //  TWITTER CTX ---> LinqToTwitter.TwitterContext
            var twitterCtx = new TwitterContext(authorizedUser);

            // test string
            string searchThis = "Anthony Rizzo";

            //  SEARCH RESPONSE ---> LinqToTwitter.Search
            var searchResponse = 
                await
                    (from search in twitterCtx.Search
                    where search.Type == SearchType.Search &&
                    search.Query == searchThis
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


