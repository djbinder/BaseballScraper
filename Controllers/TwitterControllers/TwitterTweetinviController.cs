using System;
using System.Collections.Generic;
using System.Linq;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Models.DTO;
using Tweetinvi.Models.DTO.QueryDTO;
using Tweetinvi.Parameters;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.TwitterControllers
{
    [Route("api/twitter/[controller]")]
    [ApiController]
    public class TwitterTweetinviController : ControllerBase
    {
        private readonly Helpers _h = new Helpers();
        private readonly TwitterConfiguration _twitterConfig;
        // private readonly MongoDbServicer _mongoDbServicer;
        // private readonly TwitterController _twitterController;

        private readonly string ApiUriBase = "https://api.twitter.com/1.1/users/show.json?";
        private readonly string ScreenNameSearchType = "screen_name=";

        public TwitterTweetinviController(TwitterConfiguration twitterConfig)
        {
            _twitterConfig = twitterConfig;
        }


        // https://127.0.0.1:5001/api/twitter/twittertweetinvi/test
        [Route("test")]
        public void TweetinviTesting()
        {
            TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            Auth.SetUserCredentials(_twitterConfig.ConsumerKey, _twitterConfig.ConsumerSecret, _twitterConfig.AccessToken, _twitterConfig.AccessTokenSecret);
        }



        /*
        SUMMARY:
            # There are a lot of different methods in this controller
            # Eventually they need to be better broken out
            # But for now, the best two methods to use are:
                1) GetListOfITweetsInJToken
                2) GetListOfAllTweetsFullTextInJToken
         */




        #region TWITTER USER ------------------------------------------------------------

            // STATUS [ June 24, 2019 ]: not sure if needed or if it works
            // GetJsonStringOfAllUsersTweetObjects("Buster_ESPN");
            public string GetJsonStringOfAllUsersTweetObjects(string screenName)
            {
                var stringOfAllJson = TwitterAccessor.ExecuteGETQueryReturningJson($"{ApiUriBase}{ScreenNameSearchType}{screenName}");
                Console.WriteLine(stringOfAllJson);
                return stringOfAllJson;
            }


            // STATUS [ June 21, 2019 ]: this works
            /// <summary>
            ///     Get a Twitter IUser from screen name / Twitter handle / user name
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get the user
            /// </remarks>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <example>
            ///     var user = GetIUserFromScreenName("Buster_ESPN");
            /// </example>
            /// <returns>
            ///     Tweetinvi.Models.IUser user
            /// </returns>
            public IUser GetIUserFromScreenName(string screenName)
            {
                IUser user = Tweetinvi.User.GetUserFromScreenName(screenName);
                // Console.WriteLine($"user: {user}");
                return user;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Get a IUserDTO from screen name / Twitter handle / user name
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get the user
            ///     https://github.com/linvi/tweetinvi/wiki/Custom-Queries
            /// </remarks>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <example>
            ///     var iUserDto = GetUserDTO("mr_baseball");
            /// </example>
            /// <returns>
            ///     Tweetinvi.Models.DTO.IUserDTO
            /// </returns>
            public IUserDTO GetUserDTO(string screenName)
            {
                IUserDTO userDTO = TwitterAccessor.ExecuteGETQuery<IUserDTO>($"https://api.twitter.com/1.1/users/show.json?screen_name={screenName}");
                // _h.Dig(userDTO);
                return userDTO;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Get a IUser from screen name / Twitter handle / user name
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get the user
            ///     https://github.com/linvi/tweetinvi/wiki/Custom-Queries
            /// </remarks>
            /// <param name="IUserDTO">
            ///     IUserDTO instance of model defined by tweetinvi
            /// </param>
            /// <example>
            ///     var iUserDto = GetUserDTO("mr_baseball");
            ///     var iUser = GetIUserFromUserDTO(iUserDto);
            /// </example>
            /// <returns>
            ///     Tweetinvi.Models.IUser
            /// </returns>
            public IUser GetIUserFromUserDTO(IUserDTO userDTO)
            {
                IUser user = Tweetinvi.User.GenerateUserFromDTO(userDTO);
                // _h.Dig(user);
                return user;
            }


            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Gets an IEnumerable of twitter ids for all followers of a user
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get the user ids
            ///     https://github.com/linvi/tweetinvi/wiki/Custom-Queries
            /// </remarks>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <example>
            ///     var userIds = GetUserIdsForFollowersOfUser("Buster_ESPN");
            /// </example>
            /// <returns>
            ///     IEnumerable<long>
            /// </returns>
            public IEnumerable<long> GetUserIdsForFollowersOfUser(string screenName)
            {
                IEnumerable<long> userIds = TwitterAccessor.ExecuteCursorGETQuery<long, IIdsCursorQueryResultDTO>($"https://api.twitter.com/1.1/followers/ids.json?screen_name={screenName}");
                // _h.Dig(userIds);
                return userIds;
            }


            // not sure if this works or is needed
            // https://github.com/linvi/tweetinvi/wiki/Custom-Queries
            // public string GetUserJson(string screenName)
            // {
            //     var userJson = TwitterAccessor.ExecuteGETQueryReturningJson($"https://api.twitter.com/1.1/users/show.json?screen_name={screenName}");
            //     _h.Dig(userJson);
            //     return userJson;
            // }


        #endregion TWITTER USER ------------------------------------------------------------





        #region TIMELINES and TWEETS ------------------------------------------------------------


            // STATUS [ June 21, 2019 ]: this works
            /// <summary>
            ///     Gets last 40 tweets from personal timeline (i.e., current authenticated user)
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get the timeline
            ///     Note these are NOT tweets by the user; it's the tweets of users the current user follows
            ///     See: https://github.com/linvi/tweetinvi/wiki/Timelines
            /// </remarks>
            /// <example>
            ///     var tweets = GetTweetsFromMyTimeline();
            /// </example>
            /// <returns>
            ///     IEnumerable<Tweetinvi.Models.ITweet>
            /// </returns>
            public IEnumerable<ITweet> GetTweetsFromMyTimeline()
            {
                RunPreCheck();
                var tweets = Timeline.GetHomeTimeline();
                // PrintTweetInfoFromIEnumerableITweet(tweets);
                RunPostCheck();
                return tweets;
            }


            // STATUS [ June 21, 2019 ]: this works
            /// <summary>
            ///     Gets last 40 tweets from one person's timeline (added as 'screenName' parameter)
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get the timeline
            ///     See: https://github.com/linvi/tweetinvi/wiki/Timelines
            /// </remarks>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <example>
            ///     var tweets = GetTweetsFromUsersTimeline("Buster_ESPN");
            /// </example>
            /// <returns>
            ///     IEnumerable<Tweetinvi.Models.ITweet>
            /// </returns>
            public IEnumerable<ITweet> GetTweetsFromUsersTimeline(string screenName)
            {
                var user = GetIUserFromScreenName(screenName);
                var tweets = Timeline.GetUserTimeline(user);
                PrintTweetInfoFromIEnumerableITweet(tweets);
                return tweets;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Gets last X tweets from all Twitter users for 'searchString'
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to execute the search
            ///     See: https://github.com/linvi/tweetinvi/wiki/Searches
            /// </remarks>
            /// <param name="searchString">
            ///     The term or terms that you want to search for
            /// </param>
            /// <example>
            ///     var twitterSearch = SearchTwitterNoFilters(searchString);
            /// </example>
            /// <returns>
            ///     IEnumerable<Tweetinvi.Models.ITweet>
            /// </returns>
            public IEnumerable<ITweet> SearchTwitterNoFilters(string searchString)
            {
                IEnumerable<ITweet> matchingTweets = Search.SearchTweets(searchString);
                PrintTweetInfoFromIEnumerableITweet(matchingTweets);
                return matchingTweets;
            }



            // STATUS [ June 24, 2019 ]: this works
            // OPTION 1: tweet id parameter is long
            /// <summary>
            ///     Gets one tweet by providing tweetId (e.g., 1142590848724557824)
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to execute the search
            ///     See: https://github.com/linvi/tweetinvi/wiki/Tweets
            /// </remarks>
            /// <param name="tweetId">
            ///     The tweed Id of the tweet you want in long form
            /// </param>
            /// <example>
            ///     var tweetIdLong = 1142590848724557824;
            ///     var tweets = GetTweetFromTweetId(tweetIdLong);
            /// </example>
            /// <returns>
            ///     Tweetinvi.Models.ITweet
            /// </returns>
            public ITweet GetTweetFromTweetId(long tweetId)
            {
                ITweet tweet = Tweet.GetTweet(tweetId);
                // PrintTweetInfo(tweet);
                return tweet;
            }



            // STATUS [ June 24, 2019 ]: this works
            // OPTION 2: tweet id parameter is a string
            /// <summary>
            ///     Gets one tweet by providing tweetId (e.g., "1142590848724557824")
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to execute the search
            ///     See: https://github.com/linvi/tweetinvi/wiki/Tweets
            /// </remarks>
            /// <param name="tweetIdString">
            ///     The tweed Id of the tweet you want in string form
            /// </param>
            /// <example>
            ///     var tweetIdString = "1142590848724557824";
            ///     var tweets = GetTweetFromTweetId(tweetIdString);
            /// </example>
            /// <returns>
            ///     Tweetinvi.Models.ITweet
            /// </returns>
            public ITweet GetTweetFromTweetId(string tweetIdString)
            {
                var tweetId = Int64.Parse(tweetIdString);
                ITweet tweet = Tweet.GetTweet(tweetId);
                // PrintTweetInfo(tweet);
                return tweet;
            }


        #endregion TIMELINES and TWEETS ------------------------------------------------------------





        #region LISTS ------------------------------------------------------------


            // public string CreateSearchStringToSearchListFor(string screenName, string listName, string searchString)
            // {
            //     TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            //     string urlBase = "https://api.twitter.com/1.1/search/tweets.json?q=";
            //     string prefix = $"list%3A%40{screenName}%2F{listName}%20";

            //     // "+tweet_mode=extended" = %2Btweet_mode%3Dextended
            //     // "tweet_mode=extended" = tweet_mode%3Dextended
            //     string searchForExtendedTweetsString = " tweet_mode=extended";

            //     var stringEncoded = Uri.EscapeDataString(searchForExtendedTweetsString);
            //     Console.WriteLine($"\nstringEncoded: {stringEncoded}\n");

            //     string fullSearchString = $"{urlBase}{prefix}{searchString}";
            //     // string fullSearchString = $"{urlBase}{prefix}{searchString}{stringEncoded}";
            //     // string fullSearchString = $"{urlBase}{prefix}{searchString}{searchForExtendedTweetsString}";
            //     Console.WriteLine($"fullSearchString: {fullSearchString}");
            //     return fullSearchString;
            // }


            // STATUS [ June 24, 2019 ]: this works


            /// <summary>
            ///     Create search string to execute twitter list search based on searchString
            /// </summary>
            /// <remarks>
            ///     Used within 'GetJObjectOfTweetsFromListFiltered' method
            ///     See for encoding help: https://en.wikipedia.org/wiki/Percent-encoding
            ///     See: https://developer.twitter.com/en/docs/tweets/search/api-reference/get-search-tweets
            /// </remarks>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <param name="listName">
            ///     The name of the list as defined by the user (e.g., 'Baseball')
            /// </param>
            /// <param name="searchString">
            ///     The term or terms that you want to search for
            /// </param>
            /// <param name="numberOfResultsToReturn">
            ///     The number of tweets you want returned
            /// </param>
            /// <example>
            ///     string fullSearchString = CreateSearchStringToSearchListFor("mr_baseball", "baseball", "mookie", 100);
            /// </example>
            /// <returns>
            ///     string
            /// </returns>
            public string CreateSearchStringToSearchListFor(string screenName, string listName, string searchString, int numberOfResultsToReturn)
            {
                string urlBase = "https://api.twitter.com/1.1/search/tweets.json?q=";
                string prefix = $"list%3A%40{screenName}%2F{listName}%20";
                string resultsCount = $"&count={numberOfResultsToReturn}";
                string fullSearchString = $"{urlBase}{prefix}{searchString}{resultsCount}";
                Console.WriteLine($"{fullSearchString}");
                return fullSearchString;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Gets JObject with all tweet info(i.e., text from tweet) from Twitter list based on searchString
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to execute the search
            ///     Uses 'CreateSearchStringToSearchListFor' to generate search string within method
            ///     Used within 'GetJTokenOfAllTweetTextInJObject' method
            ///     See: https://github.com/linvi/tweetinvi/wiki/Custom-Queries
            /// </remarks>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <param name="listName">
            ///     The name of the list as defined by the user (e.g., 'Baseball')
            /// </param>
            /// <param name="searchString">
            ///     The term or terms that you want to search for
            /// </param>
            /// <param name="numberOfResultsToReturn">
            ///     The number of tweets you want returned
            /// </param>
            /// <example>
            ///     JObject jsonObject = GetJObjectOfTweetsFromListFiltered("mr_baseball", "baseball", "mookie", 100);
            /// </example>
            /// <returns>
            ///     JObject
            /// </returns>
            public JObject GetJObjectOfTweetsFromListFiltered(string screenName, string listName, string searchString, int numberOfResultsToReturn)
            {
                TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
                string fullSearchString = CreateSearchStringToSearchListFor(screenName, listName, searchString, numberOfResultsToReturn);
                JObject jsonObject = TwitterAccessor.GetQueryableJsonObjectFromGETQuery(fullSearchString);
                // _h.Dig(jsonObject);
                return jsonObject;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Gets JToken with all tweet info(i.e., text from tweet) from Twitter list based on searchString
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to execute the search
            ///     Uses 'GetJObjectOfTweetsFromListFiltered' to generate JObject within method
            ///     Used within 'GetListOfAllTweetsFullTextInJToken' and 'GetListOfITweetsInJToken' methods
            ///     See: https://github.com/linvi/tweetinvi/wiki/Custom-Queries
            /// </remarks>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <param name="listName">
            ///     The name of the list as defined by the user (e.g., 'Baseball')
            /// </param>
            /// <param name="searchString">
            ///     The term or terms that you want to search for
            /// </param>
            /// <param name="numberOfResultsToReturn">
            ///     The number of tweets you want returned
            /// </param>
            /// <example>
            ///     JToken allStatusesJToken = GetJTokenOfAllTweetTextInJObject("mr_baseball", "baseball", "mookie", 100);
            /// </example>
            /// <returns>
            ///     JToken
            /// </returns>
            public JToken GetJTokenOfAllTweetTextInJObject(string screenName, string listName, string searchString, int numberOfResultsToReturn)
            {
                TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
                JObject jsonObject = GetJObjectOfTweetsFromListFiltered(screenName, listName, searchString, numberOfResultsToReturn);
                JToken allStatusesJToken = jsonObject["statuses"];
                // _h.Dig(allStatusesJToken[0]);
                // Console.WriteLine($"allStatusesJToken.Count: {allStatusesJToken.Count()}");
                return allStatusesJToken;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Gets "full_text" (i.e., text from tweet) from Twitter list based on searchString
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to execute the search
            ///     Uses 'GetJTokenOfAllTweetTextInJObject' to generate JToken within method
            ///     See: https://github.com/linvi/tweetinvi/wiki/Custom-Queries
            /// </remarks>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <param name="listName">
            ///     The name of the list as defined by the user (e.g., 'Baseball')
            /// </param>
            /// <param name="searchString">
            ///     The term or terms that you want to search for
            /// </param>
            /// <param name="numberOfResultsToReturn">
            ///     The number of tweets you want returned
            /// </param>
            /// <example>
            ///     var list = GetListOfAllTweetsFullTextInJToken("mr_baseball", "baseball", "mookie", 100);
            /// </example>
            /// <returns>
            ///     List<string>
            /// </returns>
            public List<string> GetListOfAllTweetsFullTextInJToken(string screenName, string listName, string searchString, int numberOfResultsToReturn)
            {
                TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
                List<string> tweetTextList = new List<string>();
                JToken allStatusesJToken = GetJTokenOfAllTweetTextInJObject(screenName, listName, searchString, numberOfResultsToReturn);

                var numberOfTweets = allStatusesJToken.Children().Count();
                for(var count = 0; count <= numberOfTweets - 1; count++)
                {
                    var sinceIdString = allStatusesJToken[count]["id_str"].ToString();
                    var tweet = GetTweetFromTweetId(sinceIdString);
                    var tweetFullText = tweet.FullText;
                    tweetTextList.Add(tweetFullText);
                    PrintTweetInfo(tweet);
                }
                return tweetTextList;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Gets multiple ITweets from Twitter list based on searchString
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to execute the search
            ///     Uses 'GetJTokenOfAllTweetTextInJObject' to generate JToken within method
            ///     See: https://github.com/linvi/tweetinvi/wiki/Custom-Queries
            /// </remarks>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <param name="listName">
            ///     The name of the list as defined by the user (e.g., 'Baseball')
            /// </param>
            /// <param name="searchString">
            ///     The term or terms that you want to search for
            /// </param>
            /// <param name="numberOfResultsToReturn">
            ///     The number of tweets you want returned
            /// </param>
            /// <example>
            ///     var list = GetListOfITweetsInJToken("mr_baseball", "baseball", "mookie", 100);
            /// </example>
            /// <returns>
            ///     List<Tweetinvi.Models.ITweet>
            /// </returns>
            public List<ITweet> GetListOfITweetsInJToken(string screenName, string listName, string searchString, int numberOfResultsToReturn)
            {
                TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;

                List<ITweet> listOfITweets = new List<ITweet>();
                JToken allStatusesJToken = GetJTokenOfAllTweetTextInJObject(screenName, listName, searchString, numberOfResultsToReturn);

                var numberOfTweets = allStatusesJToken.Children().Count();
                for(var count = 0; count <= numberOfTweets - 1; count++)
                {
                    var sinceIdString = allStatusesJToken[count]["id_str"].ToString();
                    var tweet = GetTweetFromTweetId(sinceIdString);
                    listOfITweets.Add(tweet);
                    PrintTweetInfo(tweet);
                }
                Console.WriteLine($"\nRETURNED {numberOfTweets} tweets");
                return listOfITweets;
            }



            // STATUS [ June 21, 2019 ]: this works
            /// <summary>
            ///     Gets last X? tweets from one Twitter list
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get the list and then tweets in that list
            ///     Note that this does not filter any of the tweets returned (see next method for that)
            ///     See: https://github.com/linvi/tweetinvi/wiki/Twitter-Lists
            /// </remarks>
            /// <param name="listName">
            ///     The name of the list as defined by the user (e.g., 'Baseball')
            /// </param>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <example>
            ///     var tweets = GetAllTweetsFromTwitterList("baseball", "mr_baseball");
            /// </example>
            /// <returns>
            ///     IEnumerable<Tweetinvi.Models.ITweet>
            /// </returns>
            public IEnumerable<ITweet> GetAllTweetsFromTwitterList(string listName, string screenName)
            {
                var list = TwitterList.GetExistingList(listName, screenName);
                var tweets = list.GetTweets();
                PrintTweetInfoFromIEnumerableITweet(tweets);
                return tweets;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Gets last X tweets from one Twitter list based on filters set in search and current sinceId
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get the list, then tweets in that list, and then filters the tweets
            ///     Note that this DOES filter any of the tweets returned (see previous method for no filters)
            ///     See: https://github.com/linvi/tweetinvi/wiki/Twitter-Lists
            /// </remarks>
            /// <param name="listName">
            ///     The name of the list as defined by the user (e.g., 'Baseball')
            /// </param>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <param name="includeRetweets">
            ///     True if you want to include retweets; False if you don't;
            /// </param>
            /// <example>
            ///     var tweets = GetTweetsFromTwitterListAfterSinceId("baseball", "mr_baseball", false);
            /// </example>
            /// <returns>
            ///     IEnumerable<Tweetinvi.Models.ITweet>
            /// </returns>
            public IEnumerable<ITweet> GetTweetsFromTwitterListAfterSinceId(string listName, string screenName, bool includeRetweets)
            {
                ITwitterList list = TwitterList.GetExistingList(listName, screenName);

                var currentSinceId = ReadCurrentSinceIdFromTxtFile();
                // Console.WriteLine($"currentSinceId: {currentSinceId}");

                var tweetsParameters = new GetTweetsFromListParameters()
                {
                    SinceId = currentSinceId,
                    IncludeRetweets = includeRetweets
                };
                IEnumerable<ITweet> tweets = list.GetTweets(tweetsParameters);

                long newSinceId = currentSinceId;
                foreach(ITweet tweet in tweets)
                {
                    if(tweet.Id > newSinceId)
                    {
                        newSinceId = tweet.Id;
                    }
                }
                WriteSinceIdToTxtFile(newSinceId);
                Console.WriteLine($"newSinceId: {newSinceId}");
                // PrintTweetInfoFromIEnumerableITweet(tweets);
                return tweets;
            }



            // STATUS [ June 24, 2019 ]: THIS DOES NOT WORK
            public IEnumerable<ITweet> GetTweetsFromTwitterListAfterDate(string listName, string screenName, bool includeRetweets, DateTime date)
            {
                ITwitterList list = TwitterList.GetExistingList(listName, screenName);
                _h.Dig(list);

                var tweetsParameters = new GetTweetsFromListParameters()
                {
                    // Until = new DateTime(2019,06,23),
                    IncludeRetweets = includeRetweets
                };
                IEnumerable<ITweet> tweets = list.GetTweets(tweetsParameters);
                // PrintTweetInfoFromIEnumerableITweet(tweets);
                return tweets;
            }



            // STATUS [ June 24, 2019 ]: this works but probably not needed
            public ITwitterListDTO GetITwitterListDTO(string listName, string screenName)
            {
                ITwitterList list = TwitterList.GetExistingList(listName, screenName);
                var iTwitterListDTO = list.TwitterListDTO;
                _h.Dig(iTwitterListDTO);
                return iTwitterListDTO;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Gets list of twitter IUsers that are members of a Twitter list
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get ITwitterList list
            ///     See: https://github.com/linvi/tweetinvi/wiki/Twitter-Lists
            /// </remarks>
            /// <param name="listName">
            ///     The name of the list as defined by the user (e.g., 'Baseball')
            /// </param>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <example>
            ///     var members = GetAllMembersOfTwitterList("baseball","mr_baseball");
            /// </example>
            /// <returns>
            ///     IEnumerable<IUser>
            /// </returns>
            public IEnumerable<IUser> GetAllMembersOfTwitterList(string listName, string screenName)
            {
                ITwitterList list = TwitterList.GetExistingList(listName, screenName);
                int listMemberCount = list.MemberCount;
                IEnumerable<IUser> members = list.GetMembers(listMemberCount);
                PrintListMemberInfo(members);
                return members;
            }



            // STATUS [ June 23, 2019 ]: this works
            // not sure if this is really needed - but good to include for testing purposes
            public IEnumerable<IUser> GetSelectedNumberOfMembersOfTwitterList(string listName, string screenName, int countOfMembersToGet)
            {
                ITwitterList list = TwitterList.GetExistingList(listName, screenName);
                IEnumerable<IUser> members = list.GetMembers(countOfMembersToGet);
                // PrintListMemberInfo(members);
                return members;
            }



            // STATUS [ June 23, 2019 ]: this works
            /// <summary>
            ///     Gets list of twitter user ids that are members of a Twitter list
            /// </summary>
            /// <remarks>
            ///     Uses tweetinvi to get all Twitter list members; then filters for just user ids
            ///     Works with support from 'GetAllMembersOfTwitterList' method
            ///     See: https://github.com/linvi/tweetinvi/wiki/Twitter-Lists
            /// </remarks>
            /// <param name="listName">
            ///     The name of the list as defined by the user (e.g., 'Baseball')
            /// </param>
            /// <param name="screenName">
            ///     The screen name / Twitter handle / user name that you want
            /// </param>
            /// <example>
            ///     var listOfLongs = GetAllUserIdsForMembersOfTwitterList("baseball","mr_baseball");
            /// </example>
            /// <returns>
            ///     List<long>
            /// </returns>
            public List<long> GetAllUserIdsForMembersOfTwitterList(string listName, string screenName)
            {
                IEnumerable<IUser> members = GetAllMembersOfTwitterList(listName, screenName);
                List<long> userIds = new List<long>();
                AddUserIdsToList(members, userIds);
                Console.WriteLine($"# of user ids in list: {userIds.Count}");
                return userIds;
            }



            // STATUS [ June 24, 2019 ]: this works
            /// <summary>
            ///     Support method for 'GetAllUserIdsForMembersOfTwitterList' method
            ///     Gets all user ids from given IUsers and adds them to List<long>
            /// </summary>
            public void AddUserIdsToList(IEnumerable<IUser> members, [FromQuery]List<long> userIds)
            {
                foreach(var member in members) { userIds.Add(member.Id); }
            }


        #endregion LISTS ------------------------------------------------------------





        #region MANAGE MAX ID ------------------------------------------------------------


            // STATUS [ June 23, 2019 ]: this works
            /// <summary>
            ///     Writes a given value / long (i.e., sinceId) to text file within Configuration folder
            ///     It also write date and time the value / long is added
            /// </summary>
            /// <remarks>
            ///     This connects to 'twitterSinceId.txt' file
            ///     See: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-write-to-a-text-file
            ///     From Twitter:
            ///         * Returns tweets w/ Ids > (that is, more recent than) the specified ID.
            ///         * There are limits to # of Tweets which can be accessed through the API.
            ///         * If the limit of Tweets is hit, since_id will use oldest ID available.
            ///     See: https://developer.twitter.com/en/docs/tweets/search/api-reference/get-search-tweets
            /// </remarks>
            /// <param name="sinceId">
            ///     Last id of all tweets previously searched
            ///     Future searches should stop once they hit the sinceId since you've already searched those
            /// </param>
            /// <example>
            ///     WriteSinceIdToTxtFile(999999999999111111);
            /// </example>
            public void WriteSinceIdToTxtFile(long sinceId)
            {
                var rightNow = DateTime.Now;
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"Configuration/twittersinceId.txt", true))
                    {
                        file.WriteLine(rightNow);
                        file.WriteLine(sinceId);
                        file.WriteLine();
                    }
            }



            // STATUS [ June 23, 2019 ]: this works
            /// <summary>
            ///     Reads most recent since tweet id from text file within Configuration folder
            /// </summary>
            /// <remarks>
            ///     This connects to 'twitterSinceId.txt' file
            ///     See: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-a-text-file-one-line-at-a-time
            /// </remarks>
            /// <example>
            ///     var currentSinceId = ReadCurrentSinceIdFromTxtFile()
            /// </example>
            public long ReadCurrentSinceIdFromTxtFile()
            {
                long sinceId = 0;
                string sinceIdString = "";
                int counter = 0;
                string line;

                // Read the file and display it line by line.
                System.IO.StreamReader file =
                    new System.IO.StreamReader(@"Configuration/twittersinceId.txt");

                while((line = file.ReadLine()) != null)
                {
                    sinceIdString = line;
                    try
                    {
                        sinceId = Int64.Parse(sinceIdString);
                    }
                    catch { }
                    counter++;
                }
                Console.WriteLine($"final sinceId: {sinceId}");
                file.Close();
                return sinceId;
            }


        #endregion MANAGE MAX ID ------------------------------------------------------------





        #region TWEETINVI EVENTS ------------------------------------------------------------


            // STATUS [ June 21, 2019 ]: Works (I think) but not sure if needed
            /// <summary>
            ///     From tweetinvi:
            ///     "Tweetinvi events are global events that can be used to gain more control and debug over what Tweetinvi does for you."
            /// </summary>
            /// <remarks>
            ///     From tweetinvi:
            ///         * Checks which requests will be performed through Tweetinvi
            ///     More importantly it allows developers to modify or cancel a WebRequest"
            ///     See: https://github.com/linvi/tweetinvi/wiki/Tweetinvi-Events
            /// </remarks>
            public void RunPreCheck()
            {
                TweetinviEvents.QueryBeforeExecute += (sender, args) =>
                {
                    var rateLimits = RateLimit.GetQueryRateLimit(args.QueryURL);
                    if (rateLimits != null)
                    {
                        Console.WriteLine($"\nrateLimits: {rateLimits}"); // 2 - Your code to await for rate limits
                    }
                    var shouldChangeCredentials = true; // 3 - Your strategy to use multiple credentials
                    if (shouldChangeCredentials)
                    {
                        var queryDetails = args.TwitterQuery;
                        Console.WriteLine($"\nqueryDetails: {queryDetails}\n");
                    }
                    // 4 - Cancel your query
                    // args.Cancel = true;
                };
            }


            // STATUS [ June 21, 2019 ]: Works (I think) but not sure if needed
            /// <summary>
            ///     From tweetinvi:
            ///     "Tweetinvi events are global events that can be used to gain more control and debug over what Tweetinvi does for you."
            /// </summary>
            /// <remarks>
            ///     From tweetinvi:
            ///         * Checks if a query was successful and what it returned
            ///     See: https://github.com/linvi/tweetinvi/wiki/Tweetinvi-Events
            /// </remarks>
            public void RunPostCheck()
            {
                TweetinviEvents.QueryAfterExecute += (sender, args) =>
                {
                    Console.WriteLine(sender);
                    Console.WriteLine(sender.GetType());
                    var json = args.ToJson();

                    var success = args.Success;
                    Console.WriteLine($"success: \n{success}");

                    var completedDateTime = args.CompletedDateTime;
                    Console.WriteLine($"completedDateTime: \n{completedDateTime}");
                };
            }


        #endregion TWEETINVI EVENTS ------------------------------------------------------------





        #region ARE THESE NEEDED? ------------------------------------------------------------

            // public IEnumerable<ITweet> SearchTwitter(string searchString)
            // {
            //     Console.WriteLine($"{searchString}");

            //     // CustomQueryParameters // FilterTweetsNotContainingGeoInformation // Filters,
            //     // FormattedCustomQueryParameters // GeoCode // Lang // Locale // sinceId
            //     // SearchQuery // SearchType // Since // SinceId // TweetSearchType // Until
            //     var searchParams = new SearchTweetsParameters(searchString)
            //     {
            //         MaximumNumberOfResults = 5,
            //         SinceId = 399616835892781056,
            //         sinceId = 405001488843284480,
            //         // SearchType = SearchResultType.Popular,
            //         Filters = TweetSearchFilters.Verified,
            //     };

            //     Console.WriteLine($"{searchString}");
            //     Console.WriteLine(searchString.TweetParts().Content);
            //     Console.WriteLine(searchString.TweetParts().Prefix);
            //     Console.WriteLine(searchString.TweetParts().Mentions);

            //     // Console.WriteLine($"searchParams: {searchParams}");
            //     _h.Dig(searchParams);

            //     // IEnumerable<ITweet> matchingTweets = Tweetinvi.Search.SearchTweets(searchString);
            //     IEnumerable<ITweet> matchingTweets = Tweetinvi.Search.SearchTweets(searchParams);
            //     Console.WriteLine($"matchingTweets.Count: {matchingTweets.Count()}\n");

            //     // _h.Dig(matchingTweets.First());

            //     // int count = 1;
            //     // foreach(var tweet in matchingTweets)
            //     // {
            //     //     Console.WriteLine($"\n{count}");
            //     //     Console.WriteLine(tweet.Text);
            //     //     count++;
            //     // }

            //     // Console.WriteLine(matchingTweets.First().Text);

            //     // var enumerator = matchingTweets.GetEnumerator();

            //     // int counter = 1;
            //     // while(enumerator.MoveNext())
            //     // {
            //     //     Console.WriteLine($"COUNTER: {counter}");
            //     //     Console.WriteLine(enumerator);
            //     //     counter++;
            //     // }


            //     // "Id": 1141828823664812038,
            //     // "IdStr": "1141828823664812038"

            //     // Console.WriteLine("-----");
            //     // Console.WriteLine(matchingTweets.First().ExtendedTweet.FullText);
            //     return matchingTweets;
            // }


            // // STATUS [ June 23, 2019 ]: this works but not sure if needed
            // https://github.com/linvi/tweetinvi/wiki/Extended-Tweets-%26-TweetMode
            // public void GetTweetParts()
            // {
            //     TweetinviConfig.CurrentThreadSettings.TweetMode = TweetMode.Extended;
            //     string tweet = "@tweetinviapi Amazing!";
            //     ITweetTextParts tweetParts = tweet.TweetParts();
            //     _h.Dig(tweetParts);

            //     string prefix = tweetParts.Prefix;
            //     string content = tweetParts.Content;
            //     int twitterLength = tweetParts.Content.Length;
            //     Console.WriteLine($"twitterLength: {twitterLength}");
            //     string[] mentions = tweetParts.Mentions;
            // }



        #endregion ARE THESE NEEDED? ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintTweetInfoFromIEnumerableITweet(IEnumerable<ITweet> tweets)
            {
                Console.WriteLine($"Returning {tweets.Count()} Tweets\n");
                foreach(var tweet in tweets)
                {
                    PrintTweetInfo(tweet);
                }
            }


            public void PrintListMemberInfo(IEnumerable<IUser> members)
            {
                foreach(var member in members) { Console.WriteLine($"{member.Id}\t{member.Name}\t{member.ScreenName}");}
            }


            public void PrintTweetInfo(ITweet tweet)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n---------------------------------------------------------");
                Console.WriteLine($"AUTHOR: {tweet.CreatedBy.ScreenName}\t @ {tweet.CreatedAt}");
                Console.WriteLine($"Tweet Id: {tweet.Id}\t Is Retweet? {tweet.IsRetweet}");
                Console.WriteLine($"Characters: {tweet.FullText.Length}\t Truncated? {tweet.Truncated}");
                Console.WriteLine("---------------------------------------------------------");
                Console.ResetColor();
                // Console.WriteLine(tweet.Text);
                Console.WriteLine(tweet.FullText);
                Console.WriteLine(tweet.CreatedBy.Name);
                Console.WriteLine($"---------------------------------------------------------\n");
            }


        #endregion PRINTING PRESS ------------------------------------------------------------


    }
}
