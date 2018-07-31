using System;

namespace BaseballScraper.Models
{
    public class TwitterTweet
    {
        public string TweetContent { get; set; }
        public TwitterUser TwitterUser { get; set; }
        public DateTime TimeOfTweet { get; set; }
    }
}
