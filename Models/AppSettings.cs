// https://stackoverflow.com/questions/47294020/reading-appsettings-from-asp-net-core-webapi

namespace BaseballScraper.Models
{
    public class AppSettings
    {
        public YahooConfiguration YahooA { get; set; }
        public YahooConfiguration YahooB { get; set; }
        public TwitterConfiguration TwConfig { get; set; }
    }

    public class YahooConfiguration
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string AuthCodeA { get; set; }
        public string AuthCodeB { get; set; }
        public string AuthCodeC { get; set; }
    }


    public class TwitterConfiguration
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string AccessLevel { get; set; }
    }
}