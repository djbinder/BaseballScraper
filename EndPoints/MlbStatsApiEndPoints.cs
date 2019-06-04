using System;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.EndPoints
{
    public class MlbStatsApiEndPoints
    {
        private readonly string baseUri = $"https://statsapi.mlb.com/api/";
        public  string endPointType     = "";

        public int sportId = 1;
        public string versionOne = "v1";

        public class MlbStatApiEndPoint
        {
            public string BaseUri { get; set; }
            public string EndPoint { get; set; }
            public string EndPointUri { get { return BaseUri + EndPoint; }}
        }


        // public MlbStatApiEndPoint SingleGameEndPoint()
        // {
        //     // endPointType = "search_player_all";

        //     var versionOne = "v1";
        //     var versionOneOne = "v1.1";
        //     var gamePk = "529572";

        //     return new MlbStatApiEndPoint
        //     {
        //         BaseUri  = baseUri,
        //         EndPoint = $"{versionOne}/game/{gamePk}/feed/live"
        //     };
        // }


        // public MlbStatApiEndPoint AllGamesForDateEndPoint(int monthNumber, int dayNumber, int year)
        // {
        //     // endPointType = "search_player_all";
        //     return new MlbStatApiEndPoint
        //     {
        //         BaseUri  = baseUri,
        //         EndPoint = $"{versionOne}/schedule?sportId={sportId}&date={monthNumber}%2F{dayNumber}%2F{year}"
        //     };
        // }
    }
}
