namespace BaseballScraper.Models.Configuration
{
    public class TheGameIsTheGameConfiguration
    {
        // this is defined by Yahoo; it changes each year
        // https://developer.yahoo.com/fantasysports/guide/#game-resource
        // this can be generated using the 'GetYahooMlbGameKeyForThisYear()' method
        public string YahooGameKey { get; set; }

        // e.g., "l.12345"
        // this is unique to each yahoo league and is typically 4-5 numbers; it is preceeded by a lowercase L that ultimately separates the YahooGameKey and the league's league id
        // this is set in 'theGameIsTheGameConfig.json' file
        // it needs to be changed each season
        public string LeagueKeySuffix { get; set; }


        // this is the combination of YahooGameKey and LeagueKeySuffix
        // e.g., 223.l.431
        // e.g., 338.l.1234
        public string LeagueKey { get; set; }
    }
}
