using System;
using System.Diagnostics;

namespace BaseballScraper.EndPoints
{
    public class FanGraphsUriEndPoints
    {
        private readonly string baseUri = "https://www.fangraphs.com/leaders.aspx?";
        // private static readonly string _baseUri = "https://www.fangraphs.com/leaders.aspx?";
        public string endPointType = "";


        public class FanGraphsUriEndPoint
        {
            public string BaseUri { get; set; }
            public string EndPoint { get; set; }
            public string EndPointUri { get { return BaseUri + EndPoint; } }
        }



        #region FANGRAPHS SP ENDPOINTS ------------------------------------------------------------


        // https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&team=0&rost=0&age=0&filter=&players=0
        public FanGraphsUriEndPoint HittingLeadersDashboard(int year)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri = baseUri,
                EndPoint = $"pos=all&stats=bat&lg=all&qual=y&type=8&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0"
            };
        }

        public FanGraphsUriEndPoint HittingLeadersDashboard(int year, int minPA)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri = baseUri,
                EndPoint = $"pos=all&stats=bat&lg=all&qual={minPA}&type=8&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0"
            };
        }

        public FanGraphsUriEndPoint HittingLeadersStandard(int year)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri = baseUri,
                EndPoint = $"pos=all&stats=bat&lg=all&qual=y&type=0&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0"
            };
        }

        public FanGraphsUriEndPoint PitchingLeadersMasterStatsReportEndPoint(int minInningsPitched, int year, int page, int recordsPerPage)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri = baseUri,
                EndPoint = $"pos=all&stats=sta&lg=all&qual={minInningsPitched}&type=c%2c3%2c8%2c13%2c14%2c4%2c11%2c24%2c114%2c6%2c42%2c120%2c121%2c217%2c36%2c37%2c38%2c29%2c30%2c31%2c40%2c48%2c47%2c49%2c50%2c105%2c204%2c292%2c109%2c208%2c296%2c110%2c209%2c297%2c111%2c210%2c298%2c112%2c113%2c218%2c221%2c223%2c43%2c44%2c51%2c139%2c6%2c117%2c45%2c118%2c124%2c62%2c119%2c122%2c123&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0&page={page}_{recordsPerPage}"
            };
        }

        // https://www.fangraphs.com/leaders.aspx?pos=all&stats=sta&lg=all&qual=y&type=c%2c13%2c6%2c42%2c-1%2c120%2c121%2c217%2c38%2c-1%2c43%2c44%2c51%2c-1%2c105%2c109%2c113%2c-1%2c46%2c40%2c218%2c223%2c-1%2c45%2c124%2c122&season=2019&month=0&season1=2019&ind=0&team=0&rost=0&age=0&filter=&players=0&page=1_50
        public FanGraphsUriEndPoint StartingPitchersSimpleReportEndPoint(int year)
        {
            return new FanGraphsUriEndPoint
            {
                BaseUri = baseUri,
                EndPoint = $"pos=all&stats=sta&lg=all&qual=y&type=c%2c13%2c6%2c42%2c-1%2c120%2c121%2c217%2c38%2c-1%2c43%2c44%2c51%2c-1%2c105%2c109%2c113%2c-1%2c46%2c40%2c218%2c223%2c-1%2c45%2c124%2c122&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0&page=1_50"
            };
        }


        #endregion FANGRAPHS SP ENDPOINTS ------------------------------------------------------------





        #region FANGRAPHS HITTER ENDPOINTS ------------------------------------------------------------


            // private readonly string _fgHitterMasterReportLink = "https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=c%2c3%2c4%2c5%2c6%2c12%2c11%2c13%2c21%2c14%2c23%2c34%2c35%2c36%2c40%2c41%2c45%2c206%2c209%2c211%2c61%2c102%2c106%2c110%2c289%2c290%2c291%2c292%2c293%2c294%2c295%2c296%2c297%2c298%2c299%2c300%2c301%2c302%2c303%2c304&season=2019&month=0&season1=2019&ind=0&team=0&rost=0&age=0&filter=&players=0&startdate=&enddate=&page=1_50";

            private readonly string _fgHitterMasterReportUrlAppendix = "pos=all&stats=bat&lg=all&qual=y&type=c%2c3%2c4%2c5%2c6%2c12%2c11%2c13%2c21%2c14%2c23%2c34%2c35%2c36%2c40%2c41%2c45%2c206%2c209%2c211%2c61%2c102%2c106%2c110%2c289%2c290%2c291%2c292%2c293%2c294%2c295%2c296%2c297%2c298%2c299%2c300%2c301%2c302%2c303%2c304&season=2019&month=0&season1=2019&ind=0&team=0&rost=0&age=0&filter=&players=0&startdate=&enddate=&page=1_50";

            public FanGraphsUriEndPoint FgHitterMasterReport()
            {
                return new FanGraphsUriEndPoint
                {
                    BaseUri = baseUri,
                    EndPoint = _fgHitterMasterReportUrlAppendix
                };
            }


            private readonly static DateTime? rightNow = DateTime.Now;
            readonly int currentYear = rightNow.Value.Year;



            // public FanGraphsUriEndPoint FgHitterMasterReport(string position = "all", int minPlateAppearances = 0, string league="all", int year = 0, PositionEnum positionEnum = PositionEnum.All)
            public FanGraphsUriEndPoint FgHitterMasterReport(PositionEnum positionEnum = PositionEnum.All, int minPlateAppearances = 0, string league="all", int year = 0, int pageNumber=1)
            {
                object convertedPlateAppearances;

                if (minPlateAppearances == 0) { convertedPlateAppearances = "y"; }
                else { convertedPlateAppearances = minPlateAppearances; }

                if (year == 0) { year = currentYear; }
                else {}

                var position = PositionString(positionEnum);

                PrintHitterSearchParameters(position, convertedPlateAppearances, league, year, positionEnum);

                return new FanGraphsUriEndPoint
                {
                    BaseUri = baseUri,
                    EndPoint = $"pos={position}&stats=bat&lg={league}&qual={convertedPlateAppearances}&type=c%2c3%2c4%2c5%2c6%2c12%2c11%2c13%2c21%2c14%2c23%2c34%2c35%2c36%2c40%2c41%2c45%2c206%2c209%2c211%2c61%2c102%2c106%2c110%2c289%2c290%2c291%2c292%2c293%2c294%2c295%2c296%2c297%2c298%2c299%2c300%2c301%2c302%2c303%2c304&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0&startdate=&enddate=&page={pageNumber}_30"
                };
            }


            public FanGraphsUriEndPoint HittingLeadersStandard(int year, int minPA)
            {
                return new FanGraphsUriEndPoint
                {
                    BaseUri = baseUri,
                    EndPoint = $"pos=all&stats=bat&lg=all&qual={minPA}&type=0&season={year}&month=0&season1={year}&ind=0&team=0&rost=0&age=0&filter=&players=0"
                };
            }


            public enum PositionEnum
            {
                All,
                Catcher,
                FirstBase,
                SecondBase,
                Shortstop,
                ThirdBase,
                Outfield,
                DesignatedHitter,
                NoPosition
            }


            public string PositionString(PositionEnum position)
            {
                switch (position)
                {
                    case PositionEnum.All:
                        return "all";
                    case PositionEnum.Catcher:
                        return "c";
                    case PositionEnum.FirstBase:
                        return "1b";
                    case PositionEnum.SecondBase:
                        return "2b";
                    case PositionEnum.Shortstop:
                        return "ss";
                    case PositionEnum.ThirdBase:
                        return "3b";
                    case PositionEnum.Outfield:
                        return "of";
                    case PositionEnum.DesignatedHitter:
                        return "dh";
                    case PositionEnum.NoPosition:
                        return "np";
                }
                throw new System.Exception("PositionEnum not found");
            }


            public string PositionCleaned(string position)
            {
                string finalString;
                switch(position)
                {
                    case "ALL":
                    case "All":
                    case "all":
                        finalString = "all";
                        return finalString;

                    case "1B":
                    case "1b":
                        finalString = "1b";
                        return finalString;

                    case "2B":
                    case "2b":
                        finalString = "2b";
                        return finalString;

                    case "SS":
                    case "Ss":
                    case "ss":
                        finalString = "ss";
                        return finalString;

                    case "3B":
                    case "3b":
                        finalString = "3b";
                        return finalString;

                    case "OF":
                    case "Of":
                    case "of":
                        finalString = "of";
                        return finalString;

                    case "DH":
                    case "Dh":
                    case "dh":
                        finalString = "dh";
                        return finalString;

                    case "NP":
                    case "Np":
                    case "np":
                        finalString = "np";
                        return finalString;
                }
                throw new System.Exception("Position not able to be cleaned; it doesn't exist in the switch");
            }


        #endregion FANGRAPHS HITTER ENDPOINTS ------------------------------------------------------------




        #region PRINTING PRESS ------------------------------------------------------------


        public void PrintHitterSearchParameters(string position, object minPlateAppearances, string league, int year, PositionEnum positionEnum)
        {
            Console.WriteLine($"\n[ SEARCH PARAMETERS ]");
            Console.WriteLine($"Position: {position}\t| Qualified?: {minPlateAppearances}\t| League: {league}\t| Season: {year}\t| Position Enum: {positionEnum}\n");


            // Console.WriteLine($"position: {position}");
            // Console.WriteLine($"positionEnum: {positionEnum}");
            // Console.WriteLine($"positionString: {PositionString(positionEnum)}");
        }



        public void PrintPositionEnumOptions()
        {
            var enumOptions = Enum.GetNames(typeof(PositionEnum));

            Console.WriteLine($"\nOPTIONS FOR POSITION ENUM------------------------------------------");
            int counter = 1;
            foreach(var option in enumOptions)
            {
                Console.WriteLine($"{counter} {option}");
                counter++;
            }
        }


        #endregion PRINTING PRESS ------------------------------------------------------------

    }
}
