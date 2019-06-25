using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Twitter;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BaseballScraper.Infrastructure;






#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
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

        // MongoDbConfiguration includes:
        //      1) TweetsCollectionName 2) ConnectionString 3) DatabaseName
        private readonly MongoDbServicer _mongoDbServicer;



        public TwitterController(IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig, MongoDbServicer mongoDbServicer)
        {
            _airtableConfig = airtableConfig.Value;
            _twitterConfig  = twitterConfig.Value;
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
                    AccessTokenSecret = _twitterConfig.AccessTokenSecret
                }
            };
            return auth;
        }





        // https://127.0.0.1:5001/api/twitter/twitter/test/async
        // [Route("test/async")]
        // public async Task MlbStatsApiTesting()
        // {
        //     SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();


        //     string listOwner = "mr_baseball";
        //     string listName = "baseball";
        //     string searchTerm = "rizzo";
        //     string fullName = "Anthony Rizzo";

        //     // var taskSearch = ExecuteTwitterStringSearch(fullName);


        // }





        #region MONGO DB ------------------------------------------------------------

            [HttpPost]
            public ActionResult<TwitterStatus> Create(TwitterStatus twitterStatus)
            {
                _mongoDbServicer.Create(twitterStatus);
                return CreatedAtRoute("GetTwitterStatus", new { id = twitterStatus.Id.ToString() }, twitterStatus);
            }

        #endregion MONGO DB ------------------------------------------------------------



        #region LINQ TO TWITTER ------------------------------------------------------------


            public async Task ExecuteTwitterRawStringSearch()
            {
                SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();

                var twitterConsumerKey = _twitterConfig.ConsumerKey;

                TwitterContext twitterCtx = new TwitterContext(authorizedUser);

                var savedSearches =
                await
                    (from search in twitterCtx.SavedSearch
                     where search.Type == SavedSearchType.Searches
                     select search)
                    .ToListAsync();

                if (savedSearches != null)
                    savedSearches.ForEach(
                        search => Console.WriteLine("Search: " + search.Query));

                var firstSavedSearch = savedSearches[0].Query;
                Console.WriteLine($"firstSavedSearch: {firstSavedSearch}");

                string unencodedStatus = firstSavedSearch.ToString();
                string encodedStatus = Uri.EscapeDataString(unencodedStatus);
                string queryString = "search/tweets.json?q=" + encodedStatus;
                // string queryString = "search?f=tweets.json?q=boyd" + encodedStatus;
                Console.WriteLine($"encodedStatus: {encodedStatus}\n");
                Console.WriteLine($"queryString: {queryString}");

                var rawResult =
                    await
                    (from raw in twitterCtx.RawQuery
                    where raw.QueryString == queryString
                    select raw)
                    .SingleOrDefaultAsync();

                if (rawResult != null)
                    Console.WriteLine(
                        "Response from Twitter: \n\n" + rawResult.Response);


                // Console.WriteLine(rawResult);
                var rawResultResponse = rawResult.Response;

                // foreach(var r in rawResult.Response)
                // {
                //     Console.WriteLine(r);
                // }


            }


            // STATUS [ June 5, 2019 ]: this works
            /// <summary>
            ///     Scrapes twitter to find most recent tweets that include 'searchString' parameter
            /// </summary>
            /// <param name="searchString">
            ///     The string that you would like to search twitter for
            /// </param>
            /// <example>
            ///
            /// </example>
            public async Task ExecuteTwitterStringSearch (string searchString)
            {
                SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();

                var twitterConsumerKey = _twitterConfig.ConsumerKey;

                TwitterContext twitterCtx = new TwitterContext(authorizedUser);

                LinqToTwitter.Search searchResponse =
                    await
                        (from search in twitterCtx.Search
                        where search.Type == SearchType.Search &&
                        search.Query == searchString
                        select search)
                        .SingleOrDefaultAsync();

                List<TwitterStatus> twitterStatus = CreateNewStatusListForEach(searchResponse);

                foreach(TwitterStatus status in twitterStatus)
                {
                    Create(status);
                }
            }


            // STATUS [ June 5, 2019 ]: this works
            public List<TwitterStatus> CreateNewStatusListForEach(LinqToTwitter.Search searchResponse)
            {
                List<TwitterStatus> allStatuses = new List<TwitterStatus>();

                foreach(LinqToTwitter.Status status in searchResponse.Statuses)
                {
                    TwitterStatus twitterStatus = new TwitterStatus
                    {
                        ScreenName = status.User.ScreenNameResponse,
                        CreatedAt = status.CreatedAt,
                        Text = status.Text,
                        UserId = (int)status.UserID,
                        StatusType = (int)status.Type,
                        StatusIdString = status.StatusID,
                        FullText = status.ExtendedTweet.FullText
                    };
                    allStatuses.Add(twitterStatus);
                    // PrintTwitterResponse(allStatuses);
                }
                return allStatuses;
            }


            // STATUS [ June 5, 2019 ]: this works
            // example: var secondList = CreateNewStatusListFromCount(searchResponse);
            public List<TwitterStatus> CreateNewStatusListFromCount(LinqToTwitter.Search SearchResponse)
            {
                int searchResponseCount = SearchResponse.Statuses.Count();
                // _h.Intro(searchResponseCount, "search response count");

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
                // PrintTwitterResponse(statuses);
                return statuses;
            }


        #endregion LINQ TO TWITTER ------------------------------------------------------------





        #region TWITTER LISTS ------------------------------------------------------------


            // STATUS [ June 5, 2019 ]: this works
            // await GetListsForUser("buster_ESPN");
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

                PrintUserListInfo(listItems);
            }


            // // STATUS [ June 5, 2019 ]: this DOES NOT work
            // public async Task GetTweetsFromTwitterListAsync(string listOwnerUserName, string listName, string searchString)
            // {
            //     SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();

            //     var twitterConsumerKey = _twitterConfig.ConsumerKey;

            //     // LinqToTwitter.TwitterQueryable`1[LinqToTwitter.List]
            //     TwitterContext twitterCtx = new TwitterContext(authorizedUser);

            //     string ownerScreenName = "mr_baseball";
            //     string slug = "baseball";
            //     int maxStatuses = 5;
            //     int lastStatusCount = 0;
            //     // last tweet processed on previous query
            //     ulong sinceID = 1141115792018165760;
            //     ulong maxID;
            //     int count = 10;
            //     var statusList = new List<Status>();

            //     // only count
            //     var listResponse =
            //         await
            //         (from list in twitterCtx.List
            //             where list.Type == ListType.Statuses &&
            //                 list.OwnerScreenName == ownerScreenName &&
            //                 list.Slug == slug &&
            //                 list.Count == count
            //             select list)
            //         .SingleOrDefaultAsync();

            //     Console.WriteLine($"listResponse.Count 1: {listResponse.Count}");

            //     if (listResponse != null && listResponse.Statuses != null)
            //     {
            //         List<Status> newStatuses = listResponse.Statuses;
            //         Console.WriteLine($"newStatusesCount 1: {newStatuses.Count}");

            //         // first tweet processed on current query
            //         maxID = newStatuses.Min(status => status.StatusID) - 1;
            //         statusList.AddRange(newStatuses);

            //         do
            //         {
            //             // now add sinceID and maxID
            //             listResponse =
            //                 await
            //                 (from list in twitterCtx.List
            //                 where list.Type == ListType.Statuses &&
            //                     list.OwnerScreenName == ownerScreenName &&
            //                     list.Slug == slug &&
            //                     list.Count == count &&
            //                     list.SinceID == sinceID &&
            //                     list.MaxID == maxID
            //                 select list)
            //                 .SingleOrDefaultAsync();

            //             if (listResponse == null)
            //                 break;

            //             Console.WriteLine($"listResponse.Count 2: {listResponse.Count}");

            //             newStatuses = listResponse.Statuses;
            //             Console.WriteLine($"newStatusesCount 2: {newStatuses.Count}");

            //             // first tweet processed on current query
            //             maxID = newStatuses.Min(status => status.StatusID) - 1;
            //             statusList.AddRange(newStatuses);
            //             Console.WriteLine($"statusList.Count 1: {statusList.Count}");

            //             lastStatusCount = newStatuses.Count;
            //             PrintTwitterListQueryIds(sinceID, maxID, listResponse.SinceID);
            //         }
            //         while (lastStatusCount != 0 && statusList.Count < maxStatuses);

            //         Console.WriteLine($"listResponse.Count 3: {listResponse.Count}");
            //         Console.WriteLine($"newStatusesCount 3: {newStatuses.Count}");

            //         PrintListOfTwitterStatuses(newStatuses);
            //         // PrintListOfTwitterStatuses(statusList);
            //     }
            // }


        #endregion TWITTER LISTS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            private void PrintTwitterListQueryIds(ulong sinceID, ulong previousMaxID, ulong maxID)
            {
                Console.WriteLine($"sinceID: {sinceID} \tpreviousMaxID: {previousMaxID}\t maxID: {maxID}\n");
            }


            private void PrintUserListInfo(List<LinqToTwitter.List> listItems)
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

            private void PrintTwitterStatus(LinqToTwitter.Status Status)
            {
                Console.WriteLine($"@{Status.User.Name} | {Status.CreatedAt} | [{Status.StatusID}]");
                Console.WriteLine(Status.Text);
                Console.WriteLine();
            }


            private void PrintListOfTwitterStatuses(List<LinqToTwitter.Status> listOfStatuses)
            {
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"TWEETS || COUNT: {listOfStatuses.Count}");
                Console.WriteLine("---------------------------------------------");
                int counter = 1;
                foreach(var status in listOfStatuses)
                {
                    Console.WriteLine($"# {counter}");
                    PrintTwitterStatus(status);
                    Console.WriteLine();
                    counter++;
                }
            }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}







            // // STATUS [ June 5, 2019 ]: this works
            // // OPTION 1: search string defined within the method
            // /// <summary>
            // ///     Scrapes twitter to find most recent tweets that include 'searchString' that is defined within the method
            // /// </summary>
            // /// <remarks>
            // ///     In Option 1, there are no parameters passed into the method.
            // ///     To change what you are searching for, modify the variable 'searchString' within the method itself
            // /// </remarks>
            // /// <example>
            // ///     ExecuteTwitterStringSearch()
            // /// </example>
            // /// <returns>
            // ///     A list of tweets
            // /// </returns>
            // // public async Task ExecuteTwitterStringSearch()
            // public async Task<List<Status>> ExecuteTwitterStringSearch()
            // {
            //     SingleUserAuthorizer authorizedUser = AuthorizeTwitterUser();

            //     var twitterConsumerKey = _twitterConfig.ConsumerKey;

            //     TwitterContext twitterCtx = new TwitterContext(authorizedUser);

            //     string searchString = "Anthony Rizzo";

            //     // SEARCH RESPONSE ---> LinqToTwitter.Search
            //     var searchResponse =
            //         await
            //             (from search in twitterCtx.Search
            //             where search.Type == SearchType.Search &&
            //             search.Query == searchString
            //             select search)
            //             .SingleOrDefaultAsync();

            //     // Console.WriteLine($"searchResponse.Count: {searchResponse.Count}\t searchResponse.Statuses.Count: {searchResponse.Statuses.Count}");

            //     List<Status> listOfSearchResponses = new List<Status>();
            //     if (searchResponse != null && searchResponse.Statuses != null)
            //     {
            //         // Console.WriteLine($"RESULTS FOR: {searchString}");
            //         // PrintUserAndResponse(searchResponse);
            //         foreach(var response in searchResponse.Statuses)
            //         {
            //             listOfSearchResponses.Add(response);
            //         }
            //     }
            //     else
            //     {
            //         Console.WriteLine();
            //         Console.WriteLine($"NO SEARCH RESULTS FOR: {searchString}");
            //     }
            //     return listOfSearchResponses;
            // }






// int maxTweetsToGet = 5;
// int lastStatusCount = 0;

// // last tweet processed on previous query
// // ulong sinceID = 1136047872758169600;
// ulong sinceID = 1;
// // ulong sinceID = 204251866668871681;

// ulong maxID;

// // int count = 30;

// var combinedSearchResults = new List<Status>();

// List<Status> searchResponse =
//     await
//         (from search in twitterCtx.Search
//             where search.Type == SearchType.Search &&
//                 search.Query == searchString &&
//                 search.Count == maxTweetsToGet &&
//                 search.SinceID == sinceID
//             select search.Statuses)
//         .SingleOrDefaultAsync();

// combinedSearchResults.AddRange(searchResponse);
// ulong previousMaxID = ulong.MaxValue;

// PrintListOfTwitterStatuses(searchResponse);

// do
// {
//     // one less than the newest id you've just queried
//     maxID = searchResponse.Min(status => status.StatusID) - 1;
//     PrintTwitterListQueryIds(sinceID, previousMaxID, maxID);


//     // Debug.Assert(maxID < previousMaxID);
//     previousMaxID = maxID;

//     searchResponse =
//         await
//         (from search in twitterCtx.Search
//         where search.Type == SearchType.Search &&
//             search.Query == searchString &&
//             search.Count == maxTweetsToGet &&
//             search.MaxID == maxID &&
//             search.SinceID == sinceID
//         select search.Statuses)
//         .SingleOrDefaultAsync();

//     combinedSearchResults.AddRange(searchResponse);
// }


// while (searchResponse.Any());
// // PrintListOfTwitterStatuses(searchResponse);

// // foreach(var y in searchResponse)
// // {
// //     Console.WriteLine($"y: {y}");
// //     Console.WriteLine($"y.Scopes: {y.Scopes}");
// // }

// // Console.WriteLine();
// // foreach(var x in combinedSearchResults)
// // {
// //     // Console.WriteLine($"x: {x}");
// //     // Console.WriteLine($"x.Scopes: {x.Scopes}");

// //     Console.WriteLine($"ScreenName: {x.User.ScreenNameResponse}");
// //     Console.WriteLine($"Tweet: {x.Text}");
// //     Console.WriteLine();
// // }

// // combinedSearchResults.ForEach(tweet =>
// // Console.WriteLine(
// //     "\n  User: {0} ({1})\n  Tweet: {2}",
// //     tweet.User.ScreenNameResponse,
// //     tweet.User.UserIDResponse,
// //     tweet.Text));





// // only count
// // List listResponse =
// //     await
// //     (from list in twitterCtx.List
// //      where list.Type == ListType.Statuses &&
// //            list.OwnerScreenName == ListOwnerUserName &&
// //            list.Slug == ListName &&
// //            list.Count == count
// //      select list)
// //     .SingleOrDefaultAsync();

// // Console.WriteLine($"listResponse.Count 1: {listResponse.Count}");

// // if (listResponse != null && listResponse.Statuses != null)
// // {
// //     List<Status> newStatuses = listResponse.Statuses;
// //     Console.WriteLine($"newStatuses.Count 1: {newStatuses.Count}");

// //     // first tweet processed on current query
// //     maxID = newStatuses.Min(status => status.StatusID) - 1;
// //     Console.WriteLine($"maxID: {maxID}");
// //     statusList.AddRange(newStatuses);

// //     do
// //     {
// //         // now add sinceID and maxID
// //         listResponse =
// //             await
// //             (from list in twitterCtx.List
// //              where list.Type == ListType.Statuses &&
// //                    list.OwnerScreenName == ListOwnerUserName &&
// //                    list.Slug == ListName &&
// //                    list.SinceID == sinceID &&
// //                    list.MaxID == maxID &&
// //                    list.Count == count
// //              select list)
// //             .SingleOrDefaultAsync();



// //         if (listResponse == null)
// //             break;

// //         Console.WriteLine($"listResponse.Count 2: {listResponse.Count}");

// //         newStatuses = listResponse.Statuses;
// //         Console.WriteLine($"newStatuses.Count 2: {newStatuses.Count}");


// //         // first tweet processed on current query
// //         maxID = newStatuses.Min(status => status.StatusID) - 1;
// //         statusList.AddRange(newStatuses);

// //         lastStatusCount = newStatuses.Count;
// //     }

// //     while (lastStatusCount != 0 && statusList.Count < maxTweetsToGet);

// //     Console.WriteLine($"listResponse.Count 3: {listResponse.Count}");
// //     Console.WriteLine($"newStatuses.Count 3: {newStatuses.Count}");

// //     Console.WriteLine(listResponse.FullName);
// //     Console.WriteLine(listResponse.IncludeEntities);
// //     Console.WriteLine(listResponse.Mode);
// //     Console.WriteLine(listResponse.Name);
// //     Console.WriteLine(listResponse.Type);


// //     foreach(var resp in listResponse.Statuses)
// //     {
// //         Console.WriteLine(resp);
// //     }




// //     for (int i = 0; i < statusList.Count; i++)
// //     {
// //         Status status = statusList[i];
// //         // PrintTwitterStatus(status,i);
// //     }

// // }
