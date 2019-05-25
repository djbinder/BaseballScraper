namespace BaseballScraper.EndPoints
{
    public class FanGraphsUriEndPoints
    {
        private readonly string baseUri = "https://www.fangraphs.com/leaders.aspx?";
        public  string endPointType     = "";


        public class FanGraphsUriEndPoint
        {
            public string BaseUri { get; set; }
            public string EndPoint { get; set; }
            public string EndPointUri { get { return BaseUri + EndPoint; }}
        }


        // https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&team=0&rost=0&age=0&filter=&players=0
        public FanGraphsUriEndPoint HittingLeadersDashboard(int year)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"pos=all&stats=bat&lg=all&qual=y&type=8&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0"
            };
        }
        public FanGraphsUriEndPoint HittingLeadersDashboard(int year, int minPA)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"pos=all&stats=bat&lg=all&qual={minPA}&type=8&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0"
            };
        }

        public FanGraphsUriEndPoint HittingLeadersStandard(int year)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"pos=all&stats=bat&lg=all&qual=y&type=0&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0"
            };
        }

        public FanGraphsUriEndPoint HittingLeadersStandard(int year, int minPA)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"pos=all&stats=bat&lg=all&qual={minPA}&type=0&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0"
            };
        }


        public FanGraphsUriEndPoint PitchingLeadersMasterStatsReportEndPoint(int minInningsPitched, int year, int page, int recordsPerPage)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"pos=all&stats=sta&lg=all&qual={minInningsPitched}&type=c%2c3%2c8%2c13%2c14%2c4%2c11%2c24%2c114%2c6%2c42%2c120%2c121%2c217%2c36%2c37%2c38%2c29%2c30%2c31%2c40%2c48%2c47%2c49%2c50%2c105%2c204%2c292%2c109%2c208%2c296%2c110%2c209%2c297%2c111%2c210%2c298%2c112%2c113%2c218%2c221%2c223%2c43%2c44%2c51%2c139%2c6%2c117%2c45%2c118%2c124%2c62%2c119%2c122%2c123&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0&page={page}_{recordsPerPage}"
            };
        }




        // https://www.fangraphs.com/leaders.aspx?pos=all&stats=sta&lg=all&qual=y&type=c%2c13%2c6%2c42%2c-1%2c120%2c121%2c217%2c38%2c-1%2c43%2c44%2c51%2c-1%2c105%2c109%2c113%2c-1%2c46%2c40%2c218%2c223%2c-1%2c45%2c124%2c122&season=2019&month=0&season1=2019&ind=0&team=0&rost=0&age=0&filter=&players=0&page=1_50
        public FanGraphsUriEndPoint StartingPitchersSimpleReportEndPoint(int year)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"pos=all&stats=sta&lg=all&qual=y&type=c%2c13%2c6%2c42%2c-1%2c120%2c121%2c217%2c38%2c-1%2c43%2c44%2c51%2c-1%2c105%2c109%2c113%2c-1%2c46%2c40%2c218%2c223%2c-1%2c45%2c124%2c122&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0&page=1_50"
            };
        }


    }
}
