using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using NScrape;
using NScrape.Forms;



namespace BaseballScraper.Scrapers
{
    public class _NScrape : Scraper
    {
        private String _start = "STARTED";
        private String _complete = "COMPLETED";

        public string Start {
            get => _start;
            set => _start = value;
        }

        public string Complete {
            get => _complete;
            set => _complete = value;
        }


        public _NScrape( string html ) : base( html )
        {

        }

        public string GetConditions()
        {
            Start.ThisMethod();

            // NODE ---> HtmlAgilityPack.HtmlNode
            var node = HtmlDocument.DocumentNode.Descendants().SingleOrDefault( n => n.Attributes.Contains( "class" ) && n.Attributes["class"].Value == "myforecast-current" );
            node.Intro("node");

            if ( node != null )
            {
                Extensions.Spotlight("get conditions --> mark 1 | NODE IS NOT NULL");
                return node.InnerText;
            }

            throw new ScrapeException( "Could not scrape conditions.", Html );
        }

        public string GetTemperatureF()
        {
            Start.ThisMethod();

            var node = HtmlDocument.DocumentNode.Descendants().SingleOrDefault( n => n.Attributes.Contains( "class" ) && n.Attributes["class"].Value == "myforecast-current-lrg" );

            if ( node != null )
            {
                Extensions.Spotlight("get temperature --> mark 1 | NODE IS NOT NULL");
                return node.InnerText.Replace( "&deg;", "°" );
            }

            throw new ScrapeException( "Could not scrape temperature.", Html );
        }

        public string GetTemperatureC()
        {
            Start.ThisMethod();

            var node = HtmlDocument.DocumentNode.Descendants().SingleOrDefault( n => n.Attributes.Contains( "class" ) && n.Attributes["class"].Value == "myforecast-current-sm" );

            if ( node != null )
            {
                return node.InnerText.Replace( "&deg;", "°" );
            }

            throw new ScrapeException( "Could not scrape temperature.", Html );
        }


        public string FanGraphsHitters()
        {
            Start.ThisMethod();

            var node = HtmlDocument.DocumentNode.Descendants().SingleOrDefault( n => n.Attributes.Contains( "class" ) && n.Attributes["class"].Value == "myforecast-current-sm" );

            if ( node != null )
            {
                return node.InnerText.Replace( "&deg;", "°" );
            }

            throw new ScrapeException( "Could not scrape temperature.", Html );
        }


        public WebClient CreateNewWebClient ()
        {
            var webClient = new WebClient();
            return webClient;
        }


        public _NScrape CreateScraper ()
        {
            var NewWebClient = CreateNewWebClient();
            // FORM --> NScrape.Forms.BasicHtmlForm
            var form = new BasicHtmlForm( NewWebClient );
            // form.Intro("form");

            form.Load( new Uri( "http://www.weather.gov/" ), new KeyValuePair<string, string>( "name", "getForecast" ) );

            form.InputControls.Single( c => c.Name == "inputstring" ).Value = "chicago, il";

            var response = form.Submit();
            var scraper = new _NScrape( ( ( HtmlWebResponse )response ).Html );
            scraper.Intro("scraper");

            return scraper;
        }




    }
}




// this is what worked in Main[]

// //N SCRAPE ---> BaseballScraper.Scrapers._NScrape
// var NScrape = new _NScrape(YellowPages);
// bool NScrapeOn = false;
// if(NScrapeOn == true)
// {
//     // NEW SCRAPER [@ Line#: 70] ---> BaseballScraper.Scrapers._NScrape
//     var NewScraper = NScrape.CreateScraper();
//     NewScraper.Intro("new NewScraper");

//     // CONDITIONS ---> e.g., 'Partly Cloudy'
//     var conditions = NewScraper.GetConditions();
//     conditions.Intro("conditions");

//     // TEMPERATURE F ---> e.g., '64°F'
//     var temperatureF = NewScraper.GetTemperatureF();
//     temperatureF.Intro("temperature F");

//     // TEMPERATURE C ---> 28°C
//     var temperatureC = NewScraper.GetTemperatureC();
//     temperatureC.Intro("temperature C");
// }