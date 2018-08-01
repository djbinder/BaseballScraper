using System;

using IronWebScraper;

namespace BaseballScraper.Scrapers
{
    public class IronScraper
    {
        // private static String Start    = "STARTED";
        // private static String Complete = "COMPLETED";
        public IronScraper () { }

        // public void MainIronWebScraper ()
        // {
        //     Console.WriteLine ();
        //     Extensions.Spotlight ("main iron web scraper method started");
        //     Console.WriteLine ();

        //     var scraper = new BlogScraper ();
        //     scraper.Start ();

        //     Complete.ThisMethod ();
        // }
    }

    // class BlogScraper: WebScraper
    // {
    //     public override void Init ()
    //     {
    //         this.LoggingLevel = WebScraper.LogLevel.All;
    //         this.Request ("https://blog.scrapinghub.com", Parse);
    //     }

    //     public override void Parse (Response response)
    //     {
    //         foreach (var title_link in response.Css ("h2.entry-title a"))
    //         {
    //             string strTitle = title_link.TextContentClean;
    //             Scrape (new ScrapedData () { { "Title", strTitle } });
    //         }

    //         if (response.CssExists ("div.prev-post > a[href]"))
    //         {
    //             var next_page = response.Css ("div.prev-post > a[href]") [0].Attributes["href"];
    //             this.Request (next_page, Parse);
    //         }
    //     }
    // }
}

// this worked in main
//// MAIN IRON ---> BaseballScraper.Scrapers._IronWebScraper
// var IronScrape = new _IronWebScraper ();

// bool IronScrapeOn = false;
// if (IronScrapeOn == true)
// {
//     IronScrape.MainIronWebScraper ();
// }
