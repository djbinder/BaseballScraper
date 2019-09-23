#pragma warning disable CS0219, CS0414, IDE0051, IDE0052
using System;

namespace BaseballScraper.EndPoints
{
    public class BrooksBaseballEndPoints
    {
        private static readonly string UriBase = "http://www.brooksbaseball.net/";


        // * PITCHER PROFILE TAB URI COMPONENTS
        private static readonly string LandingPageTabUriComponent      = "landing.php?";
        private static readonly string TabularDataTabUriComponent      = "tabs.php?";
        private static readonly string VeloAndMovementTabUriComponent  = "velo.php?";
        private static readonly string UsageAndOutcomesTabUriComponent = "outcome.php?";


        private static readonly string PlayerIdUriComponent = "player=";




        // * Pitcher Example profile on Brooks Baseball: http://bit.ly/2kaDOr4
        // * See: BrooksBaseballPitcher.cs to see how things are broken down


        #region TABULAR DATA ------------------------------------------------------------

        // * PITCHER PROFILE > TABULAR DATA



        // * PITCHER PROFILE > TABULAR DATA > TRAJECTORY AND MOVEMENT URI COMPONENTS


        // * = http://www.brooksbaseball.net/tabs.php?player=";
        private static readonly string TabularDataUriBase = $"{UriBase}{TabularDataTabUriComponent}player=";

        public class TabularDataEndPoint
        {
            public static string BaseUri { get => TabularDataUriBase; }
            public int PlayerId          { get; set; }
            public string EndPoint       { get; set; }
            public string EndPointUri    { get { return BaseUri + PlayerId + EndPoint; }}
        }


        /* <----- TABLES TYPES -----> */

        public enum TableTypeEnum
        {
            TrajectoryAndMovement,
            PitchUsage,
            PitchOutcomes,
            SabermetricOutcomes,
            ResultsAndAverages,
            BattedBalls,
            GameLogs,
        }

        // * Applicable to Tabular Data tab
        // * These are defined by Brooks Baseball in the URL
        public string TableType(TableTypeEnum tableTypeEnum)
        {
            switch (tableTypeEnum)
            {
                case TableTypeEnum.TrajectoryAndMovement: return"traj";

                case TableTypeEnum.PitchOutcomes:         return"po";

                case TableTypeEnum.PitchUsage:            return"usage";

                case TableTypeEnum.SabermetricOutcomes:   return"so";

                case TableTypeEnum.ResultsAndAverages:    return"ra";

                case TableTypeEnum.BattedBalls:           return"bb";

                case TableTypeEnum.GameLogs:              return"gl";

                default: break;
            }
            throw new Exception("Table Type not found");
        }



        /* <----- COMPARISON TYPES -----> */

        public enum ComparisonTypeEnum
        {
            NoComparison,       // none
            ZScoreComparison,   // z
            PitchIQComparison,  // piq
            ScoutComparison,    // scout
        }

        public string ComparisonType(ComparisonTypeEnum comparisonTypeEnum)
        {
            switch(comparisonTypeEnum)
            {
                case ComparisonTypeEnum.NoComparison:      return "none";

                case ComparisonTypeEnum.ZScoreComparison:  return "z";

                case ComparisonTypeEnum.PitchIQComparison: return "piq";

                case ComparisonTypeEnum.ScoutComparison:   return "scout";

                default: break;
            }
            throw new Exception("Comparison Type not found");
        }


        // Table types:
        //  1) TrajectoryAndMovement
        //  2) TO-DO
        // Comparison types:
        //  1) NoComparison ("none")
        //  2) ZScoreComparison ("z")
        //  3) PitchIQComparison ("piq")
        //  4) ScoutUriComparison ("scout")
        public TabularDataEndPoint TabularData_EndPoint(int playerId, TableTypeEnum tableType, ComparisonTypeEnum comparisonType)
        {
            var tableTypeString = tableType.ToString();
            var comparisonTypeString = comparisonType.ToString();

            return new TabularDataEndPoint
            {
                EndPoint = $"{TabularDataEndPoint.BaseUri}{playerId}&p_hand=-1&ppos=-1&cn=200&compType={comparisonTypeString}&risp=0&1b=0&2b=0&3b=0&rType=perc&gFilt=&time=month&minmax=ci&var={tableTypeString}&s_type=2&startDate=01/01/2019&endDate=01/01/2020&balls=-1&strikes=-1&b_hand=-1",
            };
        }

        public TabularDataEndPoint TabularData_EndPoint(int playerId, string tableType, string comparisonType)
        {
            return new TabularDataEndPoint
            {
                PlayerId = playerId,
                EndPoint = $"&p_hand=-1&ppos=-1&cn=200&compType={comparisonType}&risp=0&1b=0&2b=0&3b=0&rType=perc&gFilt=&time=month&minmax=ci&var={tableType}&s_type=2&startDate=01/01/2019&endDate=01/01/2020&balls=-1&strikes=-1&b_hand=-1",
            };
        }

        public TabularDataEndPoint TabularData_PitchOutcomes_EndPoint(int playerId)
        {
            return new TabularDataEndPoint
            {
                PlayerId = playerId,
                EndPoint = $"&p_hand=-1&ppos=-1&cn=200&compType=none&gFilt=&time=month&minmax=ci&var=po&s_type=2&startDate=01/01/2019&endDate=01/01/2020&balls=-1&strikes=-1&b_hand=-1",
            };
        }




// http://www.brooksbaseball.net/tabs.php?player=663978&p_hand=-1&ppos=-1&cn=200&compType=none&gFilt=&time=month&minmax=ci&var=po&s_type=2&startDate=01/01/2019&endDate=01/01/2020&balls=-1&strikes=-1&b_hand=-1


        //
        // * [ 1 ]
        // * "None" Example:
        // * http://www.brooksbaseball.net/tabs.php?player=663978&p_hand=-1&ppos=-1&cn=200&compType=none&risp=0&1b=0&2b=0&3b=0&rType=perc&gFilt=&time=month&minmax=ci&var=traj&s_type=2&startDate=01/01/2019&endDate=01/01/2020&balls=-1&strikes=-1&b_hand=-1
        // * /tabs.php?...compType=none...var=traj
        //
        //
        // * [ 2 ]
        // * "ZScore" Example:
        // * http://www.brooksbaseball.net/tabs.php?player=663978&p_hand=-1&ppos=-1&cn=200&compType=z&risp=0&1b=0&2b=0&3b=0&rType=perc&gFilt=&time=month&minmax=ci&var=traj&s_type=2&startDate=01/01/2019&endDate=01/01/2020&balls=-1&strikes=-1&b_hand=-1
        // * /tabs.php?...compType=z...var=traj
        //
        //
        // * [ 3 ]
        // * "Pitch IQ" Example:
        // * http://www.brooksbaseball.net/tabs.php?player=663978&p_hand=-1&ppos=-1&cn=200&compType=piq&risp=0&1b=0&2b=0&3b=0&rType=perc&gFilt=&time=month&minmax=ci&var=traj&s_type=2&startDate=01/01/2019&endDate=01/01/2020&balls=-1&strikes=-1&b_hand=-1
        // * /tabs.php?...compType=piq...var=traj
        //
        //
        // * [ 4 ]
        // * "Scout" Example:
        // * http://www.brooksbaseball.net/tabs.php?player=663978&p_hand=-1&ppos=-1&cn=200&compType=scout&risp=0&1b=0&2b=0&3b=0&rType=perc&gFilt=&time=month&minmax=ci&var=traj&s_type=2&startDate=01/01/2019&endDate=01/01/2020&balls=-1&strikes=-1&b_hand=-1
        // * /tabs.php?...compType=scout...var=traj



        #endregion TABULAR DATA ------------------------------------------------------------
    }



}
