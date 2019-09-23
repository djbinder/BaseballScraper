using BaseballScraper.Infrastructure;
using BaseballScraper.Models.ConfigurationModels;
using BaseballScraper.Models.Twitter;
using LinqToTwitter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#pragma warning disable CS0219, CS0414, CS1998, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006, MA0016
namespace BaseballScraper.Controllers
{
    [Route("api/twitter/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TwitterController: ControllerBase
    {
        private readonly Helpers               _helpers;
        private readonly TwitterConfiguration  _twitterConfig;
        private readonly AirtableConfiguration _airtableConfig;


        // MongoDbConfiguration includes:
        // 1) TweetsCollectionName 2) ConnectionString 3) DatabaseName
        private readonly MongoDbServicer _mongoDbServicer;


        public TwitterController(Helpers helpers, IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig, MongoDbServicer mongoDbServicer)
        {
            _helpers         = helpers;
            _airtableConfig  = airtableConfig.Value;
            _twitterConfig   = twitterConfig.Value;
            _mongoDbServicer = mongoDbServicer;
        }


        public TwitterController() {}

        public SingleUserAuthorizer AuthorizeTwitterUser()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey       = _twitterConfig.ConsumerKey,
                    ConsumerSecret    = _twitterConfig.ConsumerSecret,
                    AccessToken       = _twitterConfig.AccessToken,
                    AccessTokenSecret = _twitterConfig.AccessTokenSecret,
                },
            };
            return auth;
        }


        // https://127.0.0.1:5001/api/twitter/twitter/test
        [Route("test")]
        public void TwitterControllerTesting()
        {
            SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();
        }


        // https://127.0.0.1:5001/api/twitter/twitter/test/async
        [Route("test/async")]
        public async Task TwitterControllerTestingAsync()
        {
            SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();
        }


        /*
        SUMMARY:
            # A few methods included here that use LinqToTwitter
            # TwitterTweetinviController has many more ways to get Twitter data
         */


        #region TIMELINES and TWEETS ------------------------------------------------------------


            // STATUS [ June 25, 2019 ]: this works
            /// <summary>
            ///     Search all of Twitter to find most recent tweets that include 'searchString' parameter
            ///     Set to return last 100 relevant tweets which is the max possible
            /// </summary>
            /// <remarks>
            ///     Uses LinqToTwitter
            ///     See: https://github.com/JoeMayo/LinqToTwitter/wiki/Searching-Twitter
            /// </remarks>
            /// <param name="searchString">
            ///     The string that you would like to search twitter for
            /// </param>
            /// <example>
            ///     string fullName = "Anthony Rizzo";
            ///     var taskSearch = await ExecuteTwitterStringSearch(fullName);
            /// </example>
            public async Task<List<TwitterStatus>> ExecuteTwitterStringSearch (string searchString)
            {
                SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();

                TwitterContext twitterCtx = new TwitterContext(authorizedUser);

                Search searchResponse =
                    await
                        (from search in twitterCtx.Search
                            where search.Type == SearchType.Search &&
                                  search.Query == searchString &&
                                  search.Count == 100
                        select search)
                        .SingleOrDefaultAsync();

                List<TwitterStatus> twitterStatus = CreateNewStatusListForEach(searchResponse);

                PrintTwitterStatuses(twitterStatus);
                twitterCtx.Dispose();
                return twitterStatus;
            }


            // STATUS [ June 25, 2019 ]: this works
            // OPTION 1: Create List<TwitterStatus> FOR EACH
            /// <summary>
            ///     Create a List of TwitterStatus given LinqToTwitter Search FOR EACH
            /// </summary>
            /// <remarks>
            ///     Uses LinqToTwitter
            ///     See: https://github.com/JoeMayo/LinqToTwitter/wiki/Searching-Twitter
            /// </remarks>
            /// <param name="searchResponse">
            ///     Search from LinqToTwitter
            /// </param>
            /// <example>
            ///     var twitterStatus = CreateNewStatusListForEach(searchResponse);
            /// </example>
            public List<TwitterStatus> CreateNewStatusListForEach(Search searchResponse)
            {
                List<TwitterStatus> allStatuses = new List<TwitterStatus>();

                foreach(Status status in searchResponse.Statuses)
                {
                    TwitterStatus twitterStatus = new TwitterStatus
                    {
                        ScreenName     = status.User.ScreenNameResponse,
                        CreatedAt      = status.CreatedAt,
                        Text           = status.Text,
                        UserId         = (int)status.UserID,
                        StatusType     = (int)status.Type,
                        StatusIdString = status.StatusID,
                        FullText       = status.ExtendedTweet.FullText,
                    };
                    allStatuses.Add(twitterStatus);
                }
                return allStatuses;
            }


            // STATUS [ June 25, 2019 ]: this works
            // OPTION 2: Create List<TwitterStatus> FROM COUNT
            /// <summary>
            ///     Create a List of TwitterStatus given LinqToTwitter Search FROM COUNT
            /// </summary>
            /// <remarks>
            ///     Uses LinqToTwitter
            ///     See: https://github.com/JoeMayo/LinqToTwitter/wiki/Searching-Twitter
            /// </remarks>
            /// <param name="SearchResponse">
            ///     Search from LinqToTwitter
            /// </param>
            /// <example>
            ///     var twitterStatus = CreateNewStatusListFromCount(searchResponse);
            /// </example>
            public List<TwitterStatus> CreateNewStatusListFromCount(Search SearchResponse)
            {
                int searchResponseCount = SearchResponse.Statuses.Count;

                List<TwitterStatus> statuses = new List<TwitterStatus>();
                for (var i = 0; i <=searchResponseCount - 1; i++)
                {
                    TwitterStatus newStatus = new TwitterStatus
                    {
                        ScreenName     = SearchResponse.Statuses[i].User.ScreenNameResponse,
                        StatusType     = (int)SearchResponse.Statuses[i].Type,
                        UserId         = (int)SearchResponse.Statuses[i].UserID,
                        CreatedAt      = SearchResponse.Statuses[i].CreatedAt,
                        StatusIdString = SearchResponse.Statuses[i].StatusID,
                        Text           = SearchResponse.Statuses[i].Text,
                    };
                    statuses.Add(newStatus);
                }
                // PrintTwitterResponse(statuses);
                return statuses;
            }


        #endregion TIMELINES and TWEETS ------------------------------------------------------------





        #region TWITTER LISTS ------------------------------------------------------------


            // STATUS [ June 25, 2019 ]: this works
            /// <summary>
            ///     Create LinqToTwitter.List
            /// </summary>
            /// <remarks>
            ///     Uses LinqToTwitter
            ///     See: https://github.com/JoeMayo/LinqToTwitter/wiki/List-Entity
            /// </remarks>
            /// <param name="userName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <example>
            ///     var taskSearch = await GetListsForUser("mr_baseball");
            /// </example>
            public async Task<List<List>> GetListsForUser(string userName)
            {
                SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();
                TwitterContext twitterCtx = new TwitterContext(authorizedUser);

                var linqToTwitterListItems =
                    await
                        (from list in twitterCtx.List
                        where list.Type == ListType.List &&
                                list.ScreenName == userName
                        select list).ToListAsync();

                PrintLinqToTwitterListInfo(linqToTwitterListItems);
                twitterCtx.Dispose();
                return linqToTwitterListItems;
            }


        #endregion TWITTER LISTS ------------------------------------------------------------





        #region MONGO DB ------------------------------------------------------------

            // STATUS [ June 25, 2019 ]: this works
            /// <summary>
            ///     Add a TwitterStatus to mongoDb
            /// </summary>
            [HttpPost]
            public ActionResult<TwitterStatus> AddTwitterStatusToMongoDb(TwitterStatus twitterStatus)
            {
                _mongoDbServicer.Create(twitterStatus);
                return CreatedAtRoute("GetTwitterStatus", new { id = twitterStatus.Id }, twitterStatus);
            }

        #endregion MONGO DB ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            private void PrintTwitterListQueryIds(ulong sinceID, ulong previousMaxID, ulong maxID)
            {
                Console.WriteLine($"sinceID: {sinceID} \tpreviousMaxID: {previousMaxID}\t maxID: {maxID}\n");
            }


            private void PrintLinqToTwitterListInfo(List<List> listItems)
            {
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"USER'S TWITTER LISTS INFO || COUNT: {listItems.Count}");
                Console.WriteLine("---------------------------------------------");

                foreach(var item in listItems)
                {
                    Console.WriteLine($"LIST:   {item.FullName}");
                    Console.WriteLine($"SLUG:   {item.SlugResponse}");
                    Console.WriteLine($"ID:     {item.ListIDResponse}");
                    Console.WriteLine($"URI:    {item.Uri}\n");
                }
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine();
            }


            private void PrintTwitterStatuses(List<TwitterStatus> statuses)
            {
                int xCount = 1;
                foreach(TwitterStatus item in statuses)
                {
                    Console.WriteLine();
                    _helpers.Spotlight($"# {xCount}");
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine($"SCREEN NAME         | {item.ScreenName}");
                    Console.WriteLine($"STATUS TYPE         | {item.StatusType}");
                    Console.WriteLine($"USER ID             | {item.UserId}");
                    Console.WriteLine($"CREATED @           | {item.CreatedAt}");
                    Console.WriteLine($"STATUS ID STRING    | {item.StatusIdString}");
                    Console.WriteLine($"TWEET               | {item.Text}");
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine();

                    xCount++;
                }
            }


            private void PrintLinqToTwitterUserAndLinqToTwitterSearch(Search search)
            {
                search.Statuses.ForEach(tweet =>
                    Console.WriteLine(
                        "User: {0}, Tweet: {1}",
                        tweet.User.ScreenNameResponse, tweet.Text)
                    );
            }


            public void PrintLinqToTwitterUserInfo(User user)
            {
                Console.WriteLine("-----USER-----");
                Console.WriteLine($"ScreenNameResponse: {user.ScreenNameResponse}");
                Console.WriteLine($"BannerSizes: {user.BannerSizes}");
                Console.WriteLine($"Categories: {user.Categories}");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine($"Location: {user.Location}");
                Console.WriteLine($"Name: {user.Name}");
                Console.WriteLine($"ScreenName: {user.ScreenName}");
                Console.WriteLine($"ScreenNameList: {user.ScreenNameList}");
                Console.WriteLine($"Type: {user.Type}");
                Console.WriteLine($"UserIDResponse: {user.UserIDResponse}");
                Console.WriteLine($"UserIdList: {user.UserIdList}");
            }


            private void PrintLinqToTwitterStatus(Status Status)
            {
                Console.WriteLine($"@{Status.User.Name} | {Status.CreatedAt} | [{Status.StatusID}]");
                Console.WriteLine(Status.Text);
                Console.WriteLine();
            }


            private void PrintListOfLinqToTwitterStatuses(List<Status> listOfStatuses)
            {
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"TWEETS || COUNT: {listOfStatuses.Count}");
                Console.WriteLine("---------------------------------------------");
                int counter = 1;
                foreach(var status in listOfStatuses)
                {
                    Console.WriteLine($"# {counter}");
                    PrintLinqToTwitterStatus(status);
                    Console.WriteLine();
                    counter++;
                }
            }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
