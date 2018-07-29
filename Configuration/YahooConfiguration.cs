using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace BaseballScraper.Configuration
{

    public class YahooConfiguration
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }
    }
}
