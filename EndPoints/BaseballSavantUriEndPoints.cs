using System;
using System.Globalization;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006, MA0007
namespace BaseballScraper.EndPoints
{
    public class BaseballSavantUriEndPoints
    {
        private static readonly string baseUri = "https://baseballsavant.mlb.com/statcast_search";
        private static readonly string baseUriLeaderBoard = "https://baseballsavant.mlb.com/statcast_leaderboard?";
        private static readonly string baseUriExpectedStatistics = "https://baseballsavant.mlb.com/expected_statistics?";


        public class BaseballSavantUriEndPoint
        {
            public string BaseUri { get; set; }
            public string EndPoint { get; set; }
            public string EndPointUri { get { return BaseUri + EndPoint; }}
        }


        public class BaseballSavantPitcherEndPoints
        {
            public BaseballSavantUriEndPoint AllSpCswSingleDay(int year, int monthNumber, int dayNumber)
            {
                return new BaseballSavantUriEndPoint
                {
                    BaseUri  = baseUri,
                    EndPoint = $"/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=foul%5C.%5C.tip%7Cswinging%5C.%5C.pitchout%7Cswinging%5C.%5C.strike%7Cswinging%5C.%5C.strike%5C.%5C.blocked%7C&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea={year}%7C&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt={year}-{monthNumber}-{dayNumber}&game_date_lt={year}-{monthNumber}-{dayNumber}&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=50&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_pas=0"
                };
            }

            public BaseballSavantUriEndPoint AllSpCswRangeOfDays(int year, int startMonth, int startDay, int endMonth, int endDay)
            {
                return new BaseballSavantUriEndPoint
                {
                    BaseUri  = baseUri,
                    EndPoint = $"/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=foul%5C.%5C.tip%7Cswinging%5C.%5C.pitchout%7Cswinging%5C.%5C.strike%7Cswinging%5C.%5C.strike%5C.%5C.blocked%7C&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea={year}%7C&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt={year}-{startMonth}-{startDay}&game_date_lt={year}-{endMonth}-{endDay}&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=50&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_pas=0"
                };
            }


            public BaseballSavantUriEndPoint AllSpCswFullSeason(int year)
            {
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


        public class BaseballSavantHitterEndPoints
        {
            public BaseballSavantUriEndPoint HitterEndPoint(int year = 0, int minPlateAppearances = 0)
            {
                int currentYear = DateTime.Now.Year;
                string yearString;

                yearString = year == 0 ? currentYear.ToString(CultureInfo.InvariantCulture) : year.ToString(CultureInfo.InvariantCulture);

                return new BaseballSavantUriEndPoint
                {
                    BaseUri = baseUri,
                    EndPoint = $"/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea={yearString}%7C&hfSit=&player_type=batter&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt=&game_date_lt=&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=0&min_results=0&group_by=name&sort_col=xwoba&player_event_sort=h_launch_speed&sort_order=desc&min_pas={minPlateAppearances}&"
                };
            }


            // * See: https://baseballsavant.mlb.com/statcast_leaderboard?year=2019&abs=45&player_type=resp_batter_id
            // * Model is ExitVelocityAndBarrelsHitter
            public BaseballSavantUriEndPoint HitterExitVelocityAndBarrelsEndPoint(int year, int minAtBats)
            {
                return new BaseballSavantUriEndPoint
                {
                    BaseUri = baseUriLeaderBoard,
                    EndPoint = $"year={year}&abs={minAtBats}&player_type=resp_batter_id"
                };
            }

            // * See: https://baseballsavant.mlb.com/statcast_leaderboard?year=2019&abs=45&player_type=resp_batter_id
            // * Targets Csv link instead of html link
            // * Model is ExitVelocityAndBarrelsHitter
            public BaseballSavantUriEndPoint HitterExitVelocityAndBarrelsEndPoint_Csv(int year, int minAtBats)
            {
                return new BaseballSavantUriEndPoint
                {
                    BaseUri = baseUriLeaderBoard,
                    EndPoint = $"year={year}&abs={minAtBats}&player_type=resp_batter_id&csv=true"
                };
            }


            public BaseballSavantUriEndPoint HitterExpectedStatisticsEndPoint(int year, int minPlateAppearances, BaseballSavantPositionEnum position = BaseballSavantPositionEnum.All)
            {
                // Console.WriteLine($"position: {position}");
                return new BaseballSavantUriEndPoint
                {
                    BaseUri = baseUriExpectedStatistics,
                    EndPoint = $"type=batter&year={year}&position={position}&team=&min={minPlateAppearances}"
                };
            }

            public BaseballSavantUriEndPoint HitterExpectedStatisticsEndPoint_Csv(int year, int minPlateAppearances, BaseballSavantPositionEnum position = BaseballSavantPositionEnum.All)
            {
                string positionString = string.Empty;
                positionString = position == BaseballSavantPositionEnum.All ? "" : position.ToString();

                // Console.WriteLine($"positionString: {positionString}");
                return new BaseballSavantUriEndPoint
                {
                    BaseUri = baseUriExpectedStatistics,
                    EndPoint = $"type=batter&year={year}&position={positionString}&team=&min={minPlateAppearances}&csv=true"
                };
            }




            public enum BaseballSavantPositionEnum
            {
                All,
                Pitcher          = 1,
                Catcher          = 2,
                FirstBase        = 3,
                SecondBase       = 4,
                ThirdBase        = 5,
                Shortstop        = 6,
                LeftField        = 7,
                CenterField      = 8,
                RightField       = 9,
                DesignatedHitter = 10
            }
        }





    }
}


/* Hitter endpoint example

    https://baseballsavant.mlb.com/statcast_search/csv?all=true&hfPT=&hfAB=&hfBBT=&hfPR=&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea=2019%7C&hfSit=&player_type=batter&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt=2019-06-16&game_date_lt=&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=0&min_results=0&group_by=name&sort_col=xwoba&player_event_sort=h_launch_speed&sort_order=desc&min_pas=50&;

    https://baseballsavant.mlb.com/statcast_search?hfPT=&hfAB=&hfBBT=&hfPR=&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea=2019%7C&hfSit=&player_type=batter&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt=&game_date_lt=&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=0&min_results=0&group_by=name&sort_col=xwoba&player_event_sort=h_launch_speed&sort_order=desc&min_pas=30#results

*/



/* Pitcher endpoint example

https://baseballsavant.mlb.com/statcast_search?hfPT=&hfAB=&hfBBT=&hfPR=foul%5C.%5C.tip%7Cswinging%5C.%5C.pitchout%7Cswinging%5C.%5C.strike%7Cswinging%5C.%5C.strike%5C.%5C.blocked%7C&hfZ=&stadium=&hfBBL=&hfNewZones=&hfGT=R%7C&hfC=&hfSea=2019%7C&hfSit=&player_type=pitcher&hfOuts=&opponent=&pitcher_throws=&batter_stands=&hfSA=&game_date_gt=2019-05-27&game_date_lt=2019-05-27&hfInfield=&team=&position=&hfOutfield=&hfRO=&home_road=&hfFlag=&hfPull=&metric_1=&hfInn=&min_pitches=50&min_results=0&group_by=name&sort_col=pitches&player_event_sort=h_launch_speed&sort_order=desc&min_pas=0


*/
