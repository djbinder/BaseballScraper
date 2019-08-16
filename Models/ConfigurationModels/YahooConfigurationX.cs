using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace BaseballScraper.Models.Configuration
{

    public class YahooConfigurationX
    {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }
    }
}
