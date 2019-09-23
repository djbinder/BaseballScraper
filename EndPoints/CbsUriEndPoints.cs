namespace BaseballScraper.EndPoints
{
    public class CbsUriEndPoints
    {

        #region ENDPOINT BUILDING BLOCKS ------------------------------------------------------------

        private static readonly string UriBase            = "https://www.cbssports.com/fantasy/";
        private readonly string BaseballUriComponent      = "baseball/";
        private readonly string FootballUriComponent      = "football/";
        private static readonly string TrendsUriComponent = "trends/";

        private readonly string AddedUriComponent        = "added/";
        private readonly string DroppedUriComponent      = "dropped/";
        private readonly string TradedUriComponent       = "traded/";
        private readonly string ViewedUriComponent       = "viewed/";
        private readonly string AllPositionsUriComponent = "all";



        public class CbsUriEndPoint
        {
            public string UriBase
            {
                get => CbsUriEndPoints.UriBase;
            }

            public string SportComponent { get; set; } // 'baseball/' or 'football/


            public string TrendComponent
            {
                get => CbsUriEndPoints.TrendsUriComponent;
            }


            public string TransactionTypeComponent { get; set; } // * "added/", "dropped/", "traded/", "viewed/"


            public string PositionComponent { get; set; }


            // * Example: "https://www.cbssports.com/fantasy/baseball/trends/added/all";
            public string EndPointUri
            {
                get => $"{UriBase}{SportComponent}{TrendComponent}{TransactionTypeComponent}{PositionComponent}";
            }
        }

        #endregion ENDPOINT BUILDING BLOCKS ------------------------------------------------------------





        #region BASEBALL ENDPOINTS ------------------------------------------------------------


        // * Baseball > Added
        // * = "https://www.cbssports.com/fantasy/baseball/trends/added/all";
        public CbsUriEndPoint BaseballMostAddedEndPoint()
        {
            return new CbsUriEndPoint
            {
                SportComponent           = BaseballUriComponent,
                TransactionTypeComponent = AddedUriComponent,
                PositionComponent        = AllPositionsUriComponent,
            };
        }


        // * Baseball > Dropped
        // * = "https://www.cbssports.com/fantasy/baseball/trends/dropped/all";
        public CbsUriEndPoint BaseballMostDroppedEndPoint()
        {
            return new CbsUriEndPoint
            {
                SportComponent           = BaseballUriComponent,
                TransactionTypeComponent = DroppedUriComponent,
                PositionComponent        = AllPositionsUriComponent,
            };
        }


        // * Baseball > Traded
        // * = "https://www.cbssports.com/fantasy/baseball/trends/traded/all";
        public CbsUriEndPoint BaseballMostTradedEndPoint()
        {
            return new CbsUriEndPoint
            {
                SportComponent           = BaseballUriComponent,
                TransactionTypeComponent = TradedUriComponent,
                PositionComponent        = AllPositionsUriComponent,
            };
        }


        // * Baseball > Viewed
        // * = "https://www.cbssports.com/fantasy/baseball/trends/viewed/all";
        public CbsUriEndPoint BaseballMostViewedEndPoint()
        {
            return new CbsUriEndPoint
            {
                SportComponent           = BaseballUriComponent,
                TransactionTypeComponent = ViewedUriComponent,
                PositionComponent        = AllPositionsUriComponent,
            };
        }


        #endregion BASEBALL ENDPOINTS ------------------------------------------------------------





        #region FOOTBALL ENDPOINTS ------------------------------------------------------------


        // * Football > Added
        // * = "https://www.cbssports.com/fantasy/football/trends/added/all";
        public CbsUriEndPoint FootballMostAddedEndPoint()
        {
            return new CbsUriEndPoint
            {
                SportComponent           = FootballUriComponent,
                TransactionTypeComponent = AddedUriComponent,
                PositionComponent        = AllPositionsUriComponent,
            };
        }


        // * Football > Dropped
        // * = "https://www.cbssports.com/fantasy/football/trends/dropped/all";
        public CbsUriEndPoint FootballMostDroppedEndPoint()
        {
            return new CbsUriEndPoint
            {
                SportComponent           = FootballUriComponent,
                TransactionTypeComponent = DroppedUriComponent,
                PositionComponent        = AllPositionsUriComponent,
            };
        }


        #endregion FOOTBALL ENDPOINTS ------------------------------------------------------------
    }
}





// * Baseball EndPoints
// * > "https://www.cbssports.com/fantasy/baseball/trends/added/all";
// * > "https://www.cbssports.com/fantasy/baseball/trends/dropped/all";
// * > "https://www.cbssports.com/fantasy/baseball/trends/viewed/all";
// * > "https://www.cbssports.com/fantasy/baseball/trends/traded/all";


// * Football EndPoints
// * > "https://www.cbssports.com/fantasy/football/trends/added/all";
// * > "https://www.cbssports.com/fantasy/football/trends/dropped/all";

