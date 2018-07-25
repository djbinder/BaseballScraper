using System;

using IronWebScraper;

namespace BaseballScraper.Scrapers
{

    public class _IronWebScraper
    {
        private String _start    = "STARTED";
        private String _complete = "COMPLETED";

        public string _Start
        {
            get => _start;
            set => _start = value;
        }

        public string Complete
        {
            get => _complete;
            set => _complete = value;
        }
        public _IronWebScraper () { }

        public void MainIronWebScraper ()
        {
            Console.WriteLine ();
            Extensions.Spotlight ("main iron web scraper method started");
            Console.WriteLine ();

            var scraper = new BlogScraper ();
            scraper.Start ();

            Complete.ThisMethod ();
        }
    }

    class BlogScraper: WebScraper
    {
        public string GoGo    = "STARTED";
        public string NowStop = "COMPLETED";

        public override void Init ()
        {
            GoGo.ThisMethod ();

            this.LoggingLevel = WebScraper.LogLevel.All;
            this.Request ("https://blog.scrapinghub.com", Parse);

            NowStop.ThisMethod ();

        }

        public override void Parse (Response response)
        {
            GoGo.ThisMethod ();

            foreach (var title_link in response.Css ("h2.entry-title a"))
            {
                string strTitle = title_link.TextContentClean;
                Scrape (new ScrapedData () { { "Title", strTitle } });
            }

            if (response.CssExists ("div.prev-post > a[href]"))
            {
                var next_page = response.Css ("div.prev-post > a[href]") [0].Attributes["href"];
                this.Request (next_page, Parse);
            }

            NowStop.ThisMethod ();
        }
    }
}

// this worked in main
//// MAIN IRON ---> BaseballScraper.Scrapers._IronWebScraper
// var IronScrape = new _IronWebScraper ();

// bool IronScrapeOn = false;
// if (IronScrapeOn == true)
// {
//     IronScrape.MainIronWebScraper ();
// }