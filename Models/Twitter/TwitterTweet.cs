using System;

namespace BaseballScraper.Models.Twitter
{
    public class TwitterTweet
    {
        public string TweetContent { get; set; }
        public TwitterUser TwitterUser { get; set; }
        public DateTime TimeOfTweet { get; set; }
    }
}
