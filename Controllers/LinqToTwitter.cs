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
    [Route("linq/[controller]")]
    [ApiController]
    public class LinqToTwitterController: ControllerBase
    {
        private static String Start        = "STARTED";
        private static String Complete     = "COMPLETED";
        private string _twitterConsumerKey = null;
        public IConfiguration _configuration;
        public string _ConsumerKey { get; set; }


        private readonly TwitterConfiguration _secrets;

        public static TwitterConfiguration _twitConfig;

        public LinqToTwitterController(IOptions<TwitterConfiguration> secrets)
        {
            _secrets = secrets.Value ?? throw new ArgumentException(nameof(secrets));
        }

        // THIS DOES NOT WORK
        [HttpGet]
        public IActionResult ConsumerKey ()
        {
            Start.ThisMethod();

            var twitterConsumerKey = _secrets.ConsumerKey;
            twitterConsumerKey.Intro("consumer key");

            Complete.ThisMethod();
            return Content($"Consumer Key Is: {twitterConsumerKey}");
        }
        public string ConsumerSecret ()
        {
            Start.ThisMethod();

            var twitterConsumerSecret = _secrets.ConsumerSecret;
            twitterConsumerSecret.Intro("consumer key");

            Complete.ThisMethod();
            return twitterConsumerSecret;
        }


        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            if(id == 1)
            {
                var twitterConsumerKey = _secrets.ConsumerKey;
                twitterConsumerKey.Intro("consumer key");
                return Content($"Consumer Key Is: {twitterConsumerKey}");
            }

            if(id == 2)
            {
                var twitterConsumerSecret = _secrets.ConsumerSecret;
                twitterConsumerSecret.Intro("consumer key");
                return Content($"Consumer Secret Is: {twitterConsumerSecret}");
            }

            return Content("djb xyz");
        }


        public SingleUserAuthorizer AuthorizeTwitterUser()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey       = "jC9xha0LjStVYQY6kXXhljBvd",
                    ConsumerSecret    = "XrpXmzl9oN6ziwZR1pbRosb1Ljh44TVNe7gTsgwwM7nT5XCjtX",
                    AccessToken       = "100554390-tqir0ZO3vmYMAxGdZDI9WAfg7tGkE3sd7WUTVzoR",
                    AccessTokenSecret = "81B5NC0rUqo3GUiMmEhEWTxG3DydEzfTVTOWVLa1m2lQk"
                    // ConsumerKey       = _twitterConfiguration.ConsumerKey,
                    // ConsumerSecret    = _twitterConfiguration.ConsumerSecret,
                    // AccessToken       = _twitterConfiguration.AccessToken,
                    // AccessTokenSecret = _twitterConfiguration.AccessTokenSecret

                }
            };
            return auth;
        }

        public async Task TwitterStringSearch (String searchString)
        {
            Start.ThisMethod();
            //  AUTHORIZED USER ---> LinqToTwitter.SingleUserAuthorizer
            var authorizedUser = AuthorizeTwitterUser();

            var twitterConsumerKey = _secrets.ConsumerKey;
            twitterConsumerKey.Intro("consumer key");

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