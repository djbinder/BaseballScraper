using BaseballScraper.Client;

namespace BaseballScraper
{
    public class YahooService
    {
        private readonly IYahooAuthClient _client;
        private readonly IYahooFantasyClient _fantasy;

        public YahooService (IYahooAuthClient client, IYahooFantasyClient fantasyClient)
        {
            _client  = client;
            _fantasy = fantasyClient;
        }


        public void GetURL ()
        {
            _client.GetLoginLinkUri ();
        }



    }
}
