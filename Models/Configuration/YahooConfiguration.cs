// https://stackoverflow.com/questions/47294020/reading-appsettings-from-asp-net-core-webapi

namespace BaseballScraper.Models.Configuration
{
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
}