using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Twitter;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BaseballScraper.Infrastructure;
using System.Diagnostics;




#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers
{

    #region OVERVIEW ------------------------------------------------------------

        /// <summary> Provides three options on how to initialize method
        ///     OPTION 1 - search string defined within the method
        ///     OPTION 2 - search string is passed to the method as a parameter (i.e., 'searchString')
        ///     OPTION 3 - search string is passed as a parameter within the url
        /// </summary>
        /// <list> INDEX
        ///     <item> Execute Twitter string search [Option 1] <see cref="TwitterController.ExecuteTwitterStringSearch()" /> </item>
        ///     <item> Execute Twitter string search [Option 2] <see cref="TwitterController.ExecuteTwitterStringSearch(string)" /> </item>
        ///     <item> View Twitter search results <see cref="TwitterController.ViewTwitterStringSearchResults()" /> </item>
        // ///     <item> Execute Twitter string search [Option 3] <see cref="TwitterController.ExecuteTwitterStringSearch(string, string )" /> </item>
        /// </list>


    #endregion OVERVIEW ------------------------------------------------------------


    [Route("api/twitter/[controller]")]
    [ApiController]
    public class TwitterController: ControllerBase
    {
        private readonly Helpers _h = new Helpers();
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


        [Route("test/async")]
        public async Task MlbStatsApiTesting()
        {
            _h.StartMethod();

            SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();
            var twitterConsumerKey = _twitterConfig.ConsumerKey;


            // await ExecuteTwitterStringSearch();
            string searchString = "Rizzo /djbinder/lists/cubs";
            await ExecuteTwitterStringSearch(searchString);
            await GetListsForUser("djbinder");
            // await GetTweetsFromTwitterListAsync("djbinder","baseball","Rizzo");
        }



        #region OPTION 1 ------------------------------------------------------------

            // STATUS [ June 5, 2019 ]: this works
            // OPTION 1: search string defined within the method
            /// <summary> Scrapes twitter to find most recent tweets that include 'searchString' that is defined within the method </summary>
            /// <remarks> In Option 1, there are no parameters passed into the method. To change what you are searching for, modify the variable 'searchString' within the method itself </remarks>
            /// <example> ExecuteTwitterStringSearch() </example>
            /// <returns> A list of tweets </returns>
            public async Task ExecuteTwitterStringSearch()
            {
                SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();

                var twitterConsumerKey = _twitterConfig.ConsumerKey;

                TwitterContext twitterCtx = new TwitterContext(authorizedUser);

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

                if (searchResponse.Count != 0 && searchResponse.Statuses.Count != 0)
                {
                    Console.WriteLine($"RESULTS FOR: {searchString}");
                    // Console.WriteLine(searchResponse.Count);
                    // Console.WriteLine(searchResponse.Statuses.Count);
                    PrintUserAndResponse(searchResponse);
                    var newList = CreateNewStatusListForEach(searchResponse);
                }

                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"NO SEARCH RESULTS FOR: {searchString}");
                }
            }

        #endregion OPTION 1 ------------------------------------------------------------



        #region OPTION 2 ------------------------------------------------------------

            // STATUS [ June 5, 2019 ]: this works
            // OPTION 2: search string is passed to the method as a parameter (i.e., 'searchString')
            /// <summary> Scrapes twitter to find most recent tweets that include 'searchString' parameter </summary>
            /// <remarks> In Option 2, there is one parameter passed into the method. To change what you are searching for, modify the parameter when calling the method </remarks>
            /// <param name="searchString"> The string that you would like to search twitter for </param>
            /// <example> ExecuteTwitterStringSearch("anthony rizzo"); </example>
            /// <returns> A list of tweets </returns>
            public async Task ExecuteTwitterStringSearch (String searchString)
            {
                SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();

                var twitterConsumerKey = _twitterConfig.ConsumerKey;

                TwitterContext twitterCtx = new TwitterContext(authorizedUser);

                // SEARCH RESPONSE ---> LinqToTwitter.Search
                var searchResponse =
                    await
                        (from search in twitterCtx.Search
                        where search.Type == SearchType.Search &&
                        search.Query == searchString
                        select search)
                        .SingleOrDefaultAsync();

                // if (searchResponse != null && searchResponse.Statuses != null)
                if (searchResponse.Count != 0 && searchResponse.Statuses.Count != 0)
                {
                    Console.WriteLine($"RESULTS FOR: {searchString}");
                    // Console.WriteLine(searchResponse.Count);
                    // Console.WriteLine(searchResponse.Statuses.Count);
                    PrintUserAndResponse(searchResponse);
                    var newList = CreateNewStatusListForEach(searchResponse);
                }

                else
                {
                    Console.WriteLine();
                    Console.WriteLine($"NO SEARCH RESULTS FOR: {searchString}");
                }
            }


            // // OPTION 2B: View for Option 2
            // /// <summary> This allows viewing / testing of Option 2; The method is called and a string is passed as a parameter </summary>
            // /// <returns> A view and a list of tweets </returns>
            // public async Task<IActionResult> ViewTwitterStringSearchResults()
            // {
            //     string searchString = "Anthony Rizzo";
            //     await ExecuteTwitterStringSearch(searchString);
            //     string currently = $"SEARCHING FOR STRING: {searchString}";
            //     return Content(currently);
            // }

        #endregion OPTION 2 ------------------------------------------------------------


        // STATUS [ June 5, 2019 ]: this works
        public async Task GetListsForUser(string userName)
        {
            SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();

            var twitterConsumerKey = _twitterConfig.ConsumerKey;

            TwitterContext twitterCtx = new TwitterContext(authorizedUser);

            var listItems =
                await
                    (from list in twitterCtx.List
                    where list.Type == ListType.List &&
                            list.ScreenName == userName
                    select list).ToListAsync();


            Console.WriteLine($"# of List: {listItems.Count}");
            foreach(var item in listItems)
            {
                Console.WriteLine(item.FullName);
                Console.WriteLine(item.SlugResponse);
                Console.WriteLine(item.ListIDResponse);
                Console.WriteLine(item.Uri);
            }

        }


        // STATUS [ June 5, 2019 ]: this DOES NOT work
        public async Task GetTweetsFromTwitterListAsync(string ListOwnerUserName, string ListName, string SearchString)
        {
            SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();

            var twitterConsumerKey = _twitterConfig.ConsumerKey;

            TwitterContext twitterCtx = new TwitterContext(authorizedUser);
            Console.WriteLine(twitterCtx.List);



            int maxTweetsToGet = 5;
            int lastStatusCount = 0;

            // last tweet processed on previous query
            // ulong sinceID = 1136047872758169600;
            ulong sinceID = 1;
            // ulong sinceID = 204251866668871681;
            ulong maxID;
            // int count = 30;
            // var statusList = new List<Status>();

            var combinedSearchResults = new List<Status>();

            List<Status> searchResponse =
                await
                    (from search in twitterCtx.Search
                        where search.Type == SearchType.Search &&
                              search.Query == SearchString &&
                              search.Count == maxTweetsToGet &&
                              search.SinceID == sinceID
                        select search.Statuses)
                    .SingleOrDefaultAsync();

            combinedSearchResults.AddRange(searchResponse);
            ulong previousMaxID = ulong.MaxValue;

            do
            {
                // one less than the newest id you've just queried
                maxID = searchResponse.Min(status => status.StatusID) - 1;
                Console.WriteLine($"sinceID: {sinceID}");
                Console.WriteLine($"previousMaxID: {previousMaxID}");
                Console.WriteLine($"maxID: {maxID}");

                // Debug.Assert(maxID < previousMaxID);
                previousMaxID = maxID;

                searchResponse =
                    await
                    (from search in twitterCtx.Search
                    where search.Type == SearchType.Search &&
                        search.Query == SearchString &&
                        search.Count == maxTweetsToGet &&
                        search.MaxID == maxID &&
                        search.SinceID == sinceID
                    select search.Statuses)
                    .SingleOrDefaultAsync();

                combinedSearchResults.AddRange(searchResponse);
            }

            while (searchResponse.Any());

            foreach(var y in searchResponse)
            {
                Console.WriteLine($"y: {y}");
                Console.WriteLine($"y.Scopes: {y.Scopes}");
            }

            Console.WriteLine();
            foreach(var x in combinedSearchResults)
            {
                // Console.WriteLine($"x: {x}");
                // Console.WriteLine($"x.Scopes: {x.Scopes}");

                Console.WriteLine($"ScreenName: {x.User.ScreenNameResponse}");
                Console.WriteLine($"Tweet: {x.Text}");
                Console.WriteLine();
            }

            // combinedSearchResults.ForEach(tweet =>
            // Console.WriteLine(
            //     "\n  User: {0} ({1})\n  Tweet: {2}",
            //     tweet.User.ScreenNameResponse,
            //     tweet.User.UserIDResponse,
            //     tweet.Text));





            // only count
            // List listResponse =
            //     await
            //     (from list in twitterCtx.List
            //      where list.Type == ListType.Statuses &&
            //            list.OwnerScreenName == ListOwnerUserName &&
            //            list.Slug == ListName &&
            //            list.Count == count
            //      select list)
            //     .SingleOrDefaultAsync();

            // Console.WriteLine($"listResponse.Count 1: {listResponse.Count}");

            // if (listResponse != null && listResponse.Statuses != null)
            // {
            //     List<Status> newStatuses = listResponse.Statuses;
            //     Console.WriteLine($"newStatuses.Count 1: {newStatuses.Count}");

            //     // first tweet processed on current query
            //     maxID = newStatuses.Min(status => status.StatusID) - 1;
            //     Console.WriteLine($"maxID: {maxID}");
            //     statusList.AddRange(newStatuses);

            //     do
            //     {
            //         // now add sinceID and maxID
            //         listResponse =
            //             await
            //             (from list in twitterCtx.List
            //              where list.Type == ListType.Statuses &&
            //                    list.OwnerScreenName == ListOwnerUserName &&
            //                    list.Slug == ListName &&
            //                    list.SinceID == sinceID &&
            //                    list.MaxID == maxID &&
            //                    list.Count == count
            //              select list)
            //             .SingleOrDefaultAsync();



            //         if (listResponse == null)
            //             break;

            //         Console.WriteLine($"listResponse.Count 2: {listResponse.Count}");

            //         newStatuses = listResponse.Statuses;
            //         Console.WriteLine($"newStatuses.Count 2: {newStatuses.Count}");


            //         // first tweet processed on current query
            //         maxID = newStatuses.Min(status => status.StatusID) - 1;
            //         statusList.AddRange(newStatuses);

            //         lastStatusCount = newStatuses.Count;
            //     }

            //     while (lastStatusCount != 0 && statusList.Count < maxTweetsToGet);

            //     Console.WriteLine($"listResponse.Count 3: {listResponse.Count}");
            //     Console.WriteLine($"newStatuses.Count 3: {newStatuses.Count}");

            //     Console.WriteLine(listResponse.FullName);
            //     Console.WriteLine(listResponse.IncludeEntities);
            //     Console.WriteLine(listResponse.Mode);
            //     Console.WriteLine(listResponse.Name);
            //     Console.WriteLine(listResponse.Type);


            //     foreach(var resp in listResponse.Statuses)
            //     {
            //         Console.WriteLine(resp);
            //     }




            //     for (int i = 0; i < statusList.Count; i++)
            //     {
            //         Status status = statusList[i];
            //         // PrintTwitterStatus(status,i);
            //     }

            // }
        }




        // #region OPTION 3 ------------------------------------------------------------

        //     // TODO: Figure out how to differentiate Option 2 and Option 3 so that 'blankString' is not needed to differentiate the two
        //     // OPTION 3: search string is passed as a parameter within the url
        //     /// <summary> Scrapes twitter to find most recent tweets that include 'searchString' parameter </summary>
        //     /// <remarks> In Option 3, there are two parameters called into method. To change what you are searching for, modify the end of the url </remarks>
        //     /// <param name="searchString"> The string that you would like to search twitter for </param>
        //     /// <param name="blankString"> This parameter doesn't actually do anything; needed to make this method different than the previous; there is probably a better way to do this </param>
        //     /// <example> https://127.0.0.1:5001/api/twitter/playersearch/anthony+rizzo </example>
        //     /// <returns> A list of tweets </returns>
        //     // [HttpGet]
        //     // [Route("playersearch/{searchString}")]
        //     public async Task ExecuteTwitterStringSearch (string searchString, string blankString)
        //     {
        //         // AUTHORIZED USER ---> LinqToTwitter.SingleUserAuthorizer
        //         var authorizedUser = AuthorizeTwitterUser();

        //         var twitterConsumerKey = _twitterConfig.ConsumerKey;

        //         // TWITTER CTX ---> LinqToTwitter.TwitterContext
        //         var twitterCtx = new TwitterContext(authorizedUser);

        //         // SEARCH RESPONSE ---> LinqToTwitter.Search
        //         var searchResponse =
        //             await
        //                 (from search in twitterCtx.Search
        //                 where search.Type == SearchType.Search &&
        //                 search.Query == searchString
        //                 select search)
        //                 .SingleOrDefaultAsync();

        //         if (searchResponse != null && searchResponse.Statuses != null)
        //         {
        //             PrintUserAndResponse(searchResponse);
        //             var newList = CreateNewStatusListForEach(searchResponse);
        //         }
        //     }

        // #endregion OPTION 3 ------------------------------------------------------------






        #region CREATE NEW STATUS ------------------------------------------------------------

            // STATUS [ June 5, 2019 ]: this works
            public List<TwitterStatus> CreateNewStatusListForEach(LinqToTwitter.Search SearchResponse)
            {
                List<TwitterStatus> allStatuses = new List<TwitterStatus>();

                foreach(var item in SearchResponse.Statuses)
                {
                    PrintTweetAuthorInfo(item.User);

                    TwitterStatus status = new TwitterStatus
                    {
                        ScreenName = item.User.ScreenNameResponse,
                        CreatedAt = item.CreatedAt,
                        Text = item.Text,
                        UserId = (int)item.UserID,
                        StatusType = (int)item.Type,
                        StatusIdString = item.StatusID,

                    };
                    allStatuses.Add(status);
                    PrintTwitterResponse(allStatuses);
                }
                return allStatuses;
            }

            // STATUS [ June 5, 2019 ]: this works
            // example: var secondList = CreateNewStatusListFromCount(searchResponse);
            public List<TwitterStatus> CreateNewStatusListFromCount(LinqToTwitter.Search SearchResponse)
            {
                int searchResponseCount = SearchResponse.Statuses.Count();
                _h.Intro(searchResponseCount, "search response count");

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
                        Text           = SearchResponse.Statuses[i].Text
                    };
                    statuses.Add(newStatus);
                }
                PrintTwitterResponse(statuses);
                return statuses;
            }


        #endregion CREATE NEW STATUS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            private void PrintTwitterResponse(List<TwitterStatus> statuses)
            {
                int xCount = 1;
                foreach(TwitterStatus item in statuses)
                {
                    Console.WriteLine();
                    _h.Spotlight($"# {xCount}");
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


            private void PrintUserAndResponse(LinqToTwitter.Search SearchResponse)
            {
                SearchResponse.Statuses.ForEach(tweet =>
                    Console.WriteLine(
                        "User: {0}, Tweet: {1}",
                        tweet.User.ScreenNameResponse, tweet.Text)
                    );
            }

            public void PrintTweetAuthorInfo(LinqToTwitter.User user)
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


            private void PrintTwitterStatus(Status Status, int counter)
            {
                Console.WriteLine();
                _h.Spotlight($"# {counter}");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine($"--> {Status.User.Name} @ {Status.CreatedAt} [{Status.StatusID}]");
                Console.WriteLine(Status.Text);
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
            }

        #endregion PRINTING PRESS ------------------------------------------------------------


    }
}
