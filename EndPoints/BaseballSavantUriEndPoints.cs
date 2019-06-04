using System;

namespace BaseballScraper.EndPoints
{
    public class BaseballSavantUriEndPoints
    {
        private readonly string baseUri = "https://baseballsavant.mlb.com/statcast_search";
        public  string endPointType     = "";


        public class BaseballSavantUriEndPoint
        {
            public string BaseUri { get; set; }
            public string EndPoint { get; set; }
            public string EndPointUri { get { return BaseUri + EndPoint; }}
        }



        public BaseballSavantUriEndPoint AllSpCswSingleDay(int year, int monthNumber, int dayNumber)
        {
            Console.WriteLine($"AllSpSingleDayCsw Endpoint");
            return new BaseballSavantUriEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=foul%5C.%5C.tip%7Cswinging%5C.%5C.pitchout%7Cswinging%5C.%5C.strike%7Cswinging%5C.%5C.strike%5C.%5C.blocked%7C&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea={year}%7C&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt={year}-{monthNumber}-{dayNumber}&game_date_lt={year}-{monthNumber}-{dayNumber}&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=50&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_pas=0"
            };
        }


        public BaseballSavantUriEndPoint AllSpCswRangeOfDays(int year, int startMonth, int startDay, int endMonth, int endDay)
        {
            Console.WriteLine($"AllSpCswRangeOfDays Endpoint");
            return new BaseballSavantUriEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=foul%5C.%5C.tip%7Cswinging%5C.%5C.pitchout%7Cswinging%5C.%5C.strike%7Cswinging%5C.%5C.strike%5C.%5C.blocked%7C&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea={year}%7C&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt={year}-{startMonth}-{startDay}&game_date_lt={year}-{endMonth}-{endDay}&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=50&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_pas=0"
            };
        }

        public BaseballSavantUriEndPoint AllSpCswFullSeason(int year)
        {
            Console.WriteLine($"AllSpCswFullSeason Endpoint");

            DateTime seasonStart = new DateTime(year, 3, 1);
            int startMonth = seasonStart.Month;
            int startDay = seasonStart.Day;


            DateTime yesterday = DateTime.Today.AddDays(-1);
            int endMonth = yesterday.Month;
            int endDay = yesterday.Day;

            return new BaseballSavantUriEndPoint
            {
                BaseUri  = baseUri,
                EndPoint = $"/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=foul%5C.%5C.tip%7Cswinging%5C.%5C.pitchout%7Cswinging%5C.%5C.strike%7Cswinging%5C.%5C.strike%5C.%5C.blocked%7C&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea={year}%7C&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt={year}-{startMonth}-{startDay}&game_date_lt={year}-{endMonth}-{endDay}&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=50&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_pas=0"
            };
        }
    }
}


//https://baseballsavant.mlb.com/statcast_search?hfPT=&hfAB=&hfBBT=&hfPR=foul%5C.%5C.tip%7Cswinging%5C.%5C.pitchout%7Cswinging%5C.%5C.strike%7Cswinging%5C.%5C.strike%5C.%5C.blocked%7C&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea=2019%7C&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt=2019-05-27&game_date_lt=2019-05-27&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=50&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_pas=0
