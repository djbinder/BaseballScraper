using System.Collections.Generic;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using C = System.Console;
using HtmlAgilityPack;

#pragma warning disable CS0219, CS0414, CS1570, CS1572, CS1573, CS1584, CS1587, CS1591, CS1658, CS1998, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006, MA0016
namespace BaseballScraper.Controllers.RotoWorld
{
    [Route("api/rotoworld/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RotoWorldForumsController : Controller
    {
        private readonly Helpers _helpers;


        public RotoWorldForumsController(Helpers helpers)
        {
            _helpers = helpers;
        }

        public RotoWorldForumsController(){}

        /*
            https://127.0.0.1:5001/api/rotoworld/rotoworldforums/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
            await GetRotoWorldForumPostsAsync(10);
        }


        public class ForumPost
        {
            // * Title example : Mookie Betts 2019 Outlook
            public string PostTitle { get; set; }
            public string PostUrl   { get; set; }
        }

        
        // STATUS [ September 18, 2019 ] : this works
        // * Get the title and url for each post in Rotoworld Forum Page
        // * numberOfPagesToScrape is how many links you want to go through 
        // * See (Baseball) : http://forums.rotoworld.com/forum/4-fantasy-baseball-talk/
        // * See (Football) : http://forums.rotoworld.com/forum/2-fantasy-football-talk/
        // * Example : await GetRotoWorldForumPostsAsync(10);
        public async Task<List<ForumPost>> GetRotoWorldForumPostsAsync(int numberOfPagesToScrape)
        {
            _helpers.OpenMethod(1);

            HtmlWeb web = new HtmlWeb();
            List<ForumPost> allPosts = new List<ForumPost>();

            // * Count = 25; One for each topic on the page
            string postTitleAndUrlPaths = "//ol/li/div/h4/span/a";
            await Task.Run(() => LoopThroughTopicsAndLinks(web, numberOfPagesToScrape, postTitleAndUrlPaths, allPosts));

            return allPosts;
        } 


        // STATUS [ September 18, 2019 ] : this works
        // * Forum Topics HTML Structure (as of September 18, 2019)
        //   > ol     | The full table                       |  ipsDataList ipsDataList_zebra ipsClear cForumTopicTable  cTopicList 
        //   > li     | Each full top row                    |  ipsDataItem ipsDataItem_responsivePhoto 
        //   > div    | Everything up to the Replies column  |  ipsDataItem_main
        //   > h4     | Pin icon, topic title, page numbers  |  ipsDataItem_title ipsContained_container
        //   > span   | Topic title, page numbers            |  ipsType_break ipsContained
        //   > a href | this is the link                     |  N/A
        //   > span   | this is the actual text that appears |  N/A
        public void LoopThroughTopicsAndLinks([FromQuery] HtmlWeb web, [FromQuery] int numberOfPagesToScrape, [FromQuery] string postTitleAndUrlPaths, [FromQuery] List<ForumPost> allPosts)
        {
            for(int pageCounter = 1; pageCounter <= numberOfPagesToScrape; pageCounter++)
            {
                string baseballForumUri = $"http://forums.rotoworld.com/forum/4-fantasy-baseball-talk/page/{pageCounter}";
                // string footballForumUri = $"http://forums.rotoworld.com/forum/2-fantasy-football-talk/page/{pageCounter}";

                HtmlDocument htmlDoc = web.Load(baseballForumUri);
                
                // ol > li > div > h4 > span > a
                HtmlNodeCollection allLinks = htmlDoc.DocumentNode.SelectNodes(postTitleAndUrlPaths);

                foreach(HtmlNode link in allLinks)
                {
                    ForumPost forumPost = new ForumPost();

                    string postUrl  = link.Attributes["href"].Value;
                    string postTitle = link.InnerText.Trim();

                    forumPost.PostTitle = postTitle;
                    forumPost.PostUrl   = postUrl;

                    allPosts.Add(forumPost);
                }
            }
        }



        #region PRINTING PRESS ------------------------------------------------------------


        public void PrintPostTitlesAndUrls(List<ForumPost> allPosts)
        {
            foreach(ForumPost post in allPosts)
            {
                C.WriteLine($"{post.PostTitle}");
                C.WriteLine($"{post.PostUrl}\n");
            }
        }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}