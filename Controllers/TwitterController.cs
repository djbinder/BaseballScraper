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
using BaseballScraper.Infrastructure;

namespace BaseballScraper.Controllers
{
    #pragma warning disable CS0414
    [Route("api/twitter")]
    [ApiController]
    public class TwitterController: Controller
    {
        private Helpers _h = new Helpers();
        private readonly TwitterConfiguration _twitterConfig;
        private readonly AirtableConfiguration _airtableConfig;


        public TwitterController(IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig)
        {
            _airtableConfig = airtableConfig.Value;
            _twitterConfig  = twitterConfig.Value;
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


        /// <summary>
        // FILE NOTE: provides three options on how to initialize method
            // OPTION 1 - search string defined within the method
            // OPTION 2 - search string is passed to the method as a parameter (i.e., 'searchString')
            // OPTION 3 - search string is passed as a parameter within the url
        /// </summary>


        // STATUS: this works
        // OPTION 1: search string defined within the method
        /// <summary> Scrapes twitter to find most recent tweets that include 'searchString' that is defined within the method </summary>
        /// <remarks> In Option 1, there are no parameters passed into the method. To change what you are searching for, modify the variable 'searchString' within the method itself </remarks>
        /// <example> https://127.0.0.1:5001/api/twitter/playersearch </example>
        /// <returns> A list of tweets </returns>
        [HttpGet]
        [Route("playersearch")]
        public async Task TwitterStringSearch ()
        {
            // AUTHORIZED USER ---> LinqToTwitter.SingleUserAuthorizer
            var authorizedUser = AuthorizeTwitterUser();

            var twitterConsumerKey = _twitterConfig.ConsumerKey;
            _h.Intro(twitterConsumerKey, "consumer key");

            // TWITTER CTX ---> LinqToTwitter.TwitterContext
            var twitterCtx = new TwitterContext(authorizedUser);

            // string to search twitter for
            string searchString = "Anthony Rizzo";

            // SEARCH RESPONSE ---> LinqToTwitter.Search
            var searchResponse = 
                await
                    (from search in twitterCtx.Search
                    where search.Type == SearchType.Search &&
                    search.Query == searchString
                    select search)
                    .SingleOrDefaultAsync();

            if (searchResponse != null && searchResponse.Statuses != null)
            {
                searchResponse.Statuses.ForEach(tweet =>
                        Console.WriteLine(
                        "User: {0}, Tweet: {1}",
                        tweet.User.ScreenNameResponse,
                        tweet.Text)
                        );

                int searchResponseCount = searchResponse.Statuses.Count();
                _h.Intro(searchResponseCount, "search response count");

                List<TwitterStatus> statuses = new List<TwitterStatus>();

                for (var i = 0; i <=searchResponseCount - 1; i++)
                {
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
                PrintTwitterResponse(statuses);
            }
        }

        // STATUS: this works
        // OPTION 2: search string is passed to the method as a parameter (i.e., 'searchString')
        /// <summary> Scrapes twitter to find most recent tweets that include 'searchString' parameter </summary>
        /// <remarks> In Option 2, there is one parameter passed into the method. To change what you are searching for, modify the parameter when calling the method </remarks>
        /// <param name="searchString"> The string that you would like to search twitter for </param>
        /// <example> TwitterStringSearch("anthony rizzo"); </example>
        /// <returns> A list of tweets </returns>
        public async Task TwitterStringSearch (String searchString)
        {
            // AUTHORIZED USER ---> LinqToTwitter.SingleUserAuthorizer
            var authorizedUser = AuthorizeTwitterUser();

            var twitterConsumerKey = _twitterConfig.ConsumerKey;
            _h.Intro(twitterConsumerKey, "consumer key");

            // TWITTER CTX ---> LinqToTwitter.TwitterContext
            var twitterCtx = new TwitterContext(authorizedUser);

            // SEARCH RESPONSE ---> LinqToTwitter.Search
            var searchResponse = 
                await
                    (from search in twitterCtx.Search
                    where search.Type == SearchType.Search &&
                    search.Query == searchString
                    select search)
                    .SingleOrDefaultAsync();

            if (searchResponse != null && searchResponse.Statuses != null)
            {
                searchResponse.Statuses.ForEach(tweet =>
                        Console.WriteLine(
                        "User: {0}, Tweet: {1}",
                        tweet.User.ScreenNameResponse,
                        tweet.Text)
                        );

                int searchResponseCount = searchResponse.Statuses.Count();
                _h.Intro(searchResponseCount, "search response count");

                List<TwitterStatus> statuses = new List<TwitterStatus>();

                for (var i = 0; i <=searchResponseCount - 1; i++)
                {
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
                // PrintTwitterResponse(statuses);
            }
        }

        // OPTION 2B: View for Option 2
        /// <summary> This allows viewing / testing of Option 2; The method is called and a string is passed as a parameter </summary>
        /// <returns> A view and a list of tweets </returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> ViewTwitterStringSearchResults()
        {
            string searchString = "Anthony Rizzo";
            await TwitterStringSearch(searchString);

            string currently = $"SEARCHING FOR STRING: {searchString}";
            return Content(currently);
        }



        // TODO: Figure out how to differentiate Option 2 and Option 3 so that 'blankString' is not needed to differentiate the two
        // OPTION 3: search string is passed as a parameter within the url
        /// <summary> Scrapes twitter to find most recent tweets that include 'searchString' parameter </summary>
        /// <remarks> In Option 3, there are two parameters called into method. To change what you are searching for, modify the end of the url </remarks>
        /// <param name="searchString"> The string that you would like to search twitter for </param>
        /// <param name="blankString"> This parameter doesn't actually do anything; needed to make this method different than the previous; there is probably a better way to do this </param>
        /// <example> https://127.0.0.1:5001/api/twitter/playersearch/anthony+rizzo </example>
        /// <returns> A list of tweets </returns>
        [HttpGet]
        [Route("playersearch/{searchString}")]
        public async Task TwitterStringSearch (string searchString, string blankString)
        {
            // AUTHORIZED USER ---> LinqToTwitter.SingleUserAuthorizer
            var authorizedUser = AuthorizeTwitterUser();

            var twitterConsumerKey = _twitterConfig.ConsumerKey;
            _h.Intro(twitterConsumerKey, "consumer key");

            // TWITTER CTX ---> LinqToTwitter.TwitterContext
            var twitterCtx = new TwitterContext(authorizedUser);

            // SEARCH RESPONSE ---> LinqToTwitter.Search
            var searchResponse = 
                await
                    (from search in twitterCtx.Search
                    where search.Type == SearchType.Search &&
                    search.Query == searchString
                    select search)
                    .SingleOrDefaultAsync();

            if (searchResponse != null && searchResponse.Statuses != null)
            {
                searchResponse.Statuses.ForEach(tweet =>
                        Console.WriteLine(
                        "User: {0}, Tweet: {1}",
                        tweet.User.ScreenNameResponse,
                        tweet.Text)
                        );

                int searchResponseCount = searchResponse.Statuses.Count();
                _h.Intro(searchResponseCount, "search response count");

                List<TwitterStatus> statuses = new List<TwitterStatus>();

                for (var i = 0; i <=searchResponseCount - 1; i++)
                {
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
                PrintTwitterResponse(statuses);
            }
        }


        private void PrintTwitterResponse(List<TwitterStatus> statuses)
        {
            int xCount = 1;
            foreach(var item in statuses)
            {
                _h.Spotlight("next status");
                _h.Intro(xCount, "status #");
                _h.Intro(item.ScreenName, "screen name");
                _h.Intro(item.StatusType, "status type");
                _h.Intro(item.UserId, "user id");
                _h.Intro(item.CreatedAt, "created at");
                _h.Intro(item.StatusIdString, "status id string");
                _h.Intro(item.Text, "text");

                xCount++;
            }
        }
    }
}