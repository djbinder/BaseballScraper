using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;

using BaseballScraper.Configuration;

namespace BaseballScraper.Client
{
    public class BeforeAfterRequestArgs
    {
        /// <summary>
        /// Client instance.
        /// </summary>
        internal HttpClient Client { get; set; }

        /// <summary>
        /// Request instance.
        /// </summary>
        internal HttpRequestMessage Request { get; set; }

        /// <summary>
        /// Response instance.
        /// </summary>
        internal string Response { get; set; }

        /// <summary>
        /// Values received from service.
        /// </summary>
        internal NameValueCollection Parameters { get; set; }

        /// <summary>
        /// Client configuration.
        /// </summary>
        internal YahooConfiguration Configuration { get; set; }
    }
}
