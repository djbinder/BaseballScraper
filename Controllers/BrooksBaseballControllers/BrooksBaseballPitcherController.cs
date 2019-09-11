using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using BaseballScraper.Infrastructure;
using System.Collections.Generic;
using C = System.Console;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.BrooksBaseball;


#pragma warning disable CS0219, CS0414, CS1570, CS1572, CS1573, CS1584, CS1587, CS1591, CS1658, CS1998, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.BrooksBaseballControllers
{
    [Route("api/brooks/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BrooksBaseballPitcherController : ControllerBase
    {
        private readonly Helpers                 _helpers;
        private readonly BrooksBaseballEndPoints _endPoints;
        private readonly BrooksBaseballUtilitiesController _brooksBaseballUtilitiesController;

        public BrooksBaseballPitcherController(Helpers helpers, BrooksBaseballEndPoints endPoints, BrooksBaseballUtilitiesController brooksBaseballUtilitiesController)
        {
            _helpers   = helpers;
            _endPoints = endPoints;
            _brooksBaseballUtilitiesController = brooksBaseballUtilitiesController;
        }


        /*
            https://127.0.0.1:5001/api/brooks/brooksbaseballpitcher/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
        }



        #region TESTING / TEST DATA ------------------------------------------------------------

        // * Example Link (Chris Paddack): http://bit.ly/2kaDOr4
        // * This is Paddack's player id ( used in Brooks Baseball url )
        static int intForTesting = 663978;

        public BrooksBaseballPitcher Test()
        {
            var metricsNoComparison      = CreateList_TabularDataMetrics_NoComparison(intForTesting);
            var metricsZScoreComparison  = CreateList_TabularDataMetrics_ZScoreComparison(intForTesting);
            var metricsPitchIQComparison = CreateList_TabularDataMetrics_PitchIQComparison(intForTesting);
            var metricsScoutComparison   = CreateList_TabularDataMetrics_ScoutComparison(intForTesting);

            var pitcher = InstantiateNewBrooksBaseballPitcher(
                metricsNoComparison,
                metricsZScoreComparison,
                metricsPitchIQComparison,
                metricsScoutComparison
            );

            _helpers.Dig(pitcher);
            return pitcher;
        }

        #endregion TESTING / TEST DATA ------------------------------------------------------------





        #region TABULAR DATA > TRAJECTORY AND MOVEMENT - LISTS ------------------------------------------------------------

        // STATUS [ September 10, 2019 ] : Y
        // * TABULAR DATA > TRAJECTORY AND MOVEMENT > NO COMPARISON (NONE)
        // * var metricsNoComparison = CreateList_TabularDataMetrics_NoComparison(intForTesting);
        public List<PitchTabularData_Metric> CreateList_TabularDataMetrics_NoComparison(int playerId)
        {
            string noComparisonEndPoint                       = TabularData_NoComparisonEndPoint(playerId);
            IEnumerable<HtmlNode> allRowsNoComparison         = _brooksBaseballUtilitiesController.GetAllTableRows(noComparisonEndPoint);
            List<PitchTabularData_Metric> metricsNoComparison = ScrapeMetricsForEachPitch(allRowsNoComparison);
            return metricsNoComparison;
        }

        // STATUS [ September 10, 2019 ] : Y
        // * TABULAR DATA > TRAJECTORY AND MOVEMENT > Z SCORE COMPARISON
        // * var metricsZScoreComparison = CreateList_TabularDataMetrics_ZScoreComparison(intForTesting);
        public List<PitchTabularData_Metric> CreateList_TabularDataMetrics_ZScoreComparison(int playerId)
        {
            string zScoreComparisonEndPoint                       = TabularData_ZScoreComparisonEndPoint(playerId);
            IEnumerable<HtmlNode> allRowsZScoreComparison         = _brooksBaseballUtilitiesController.GetAllTableRows(zScoreComparisonEndPoint);
            List<PitchTabularData_Metric> metricsZScoreComparison = ScrapeMetricsForEachPitch(allRowsZScoreComparison);
            return metricsZScoreComparison;
        }

        // STATUS [ September 10, 2019 ] : Y
        // * TABULAR DATA > TRAJECTORY AND MOVEMENT > PITCH IQ COMPARISON
        // * var metricsPitchIQComparison = CreateList_TabularDataMetrics_PitchIQComparison(intForTesting);
        public List<PitchTabularData_Metric> CreateList_TabularDataMetrics_PitchIQComparison(int playerId)
        {
            string pitchIQComparisonEndPoint                       = TabularData_PitchIQComparisonEndPoint(playerId);
            IEnumerable<HtmlNode> allRowsPitchIQComparison         = _brooksBaseballUtilitiesController.GetAllTableRows(pitchIQComparisonEndPoint);
            List<PitchTabularData_Metric> metricsPitchIQComparison = ScrapeMetricsForEachPitch(allRowsPitchIQComparison);
            return metricsPitchIQComparison;
        }

        // STATUS [ September 10, 2019 ] : Y
        // * TABULAR DATA > TRAJECTORY AND MOVEMENT > SCOUT COMPARISON
        // * var metricsScoutComparison = CreateList_TabularDataMetrics_ScoutComparison(intForTesting);
        public List<PitchTabularData_Metric> CreateList_TabularDataMetrics_ScoutComparison(int playerId)
        {
            string scoutComparisonEndPoint                       = TabularData_ScoutComparisonEndPoint(playerId);
            IEnumerable<HtmlNode> allRowsScoutComparison         = _brooksBaseballUtilitiesController.GetAllTableRows(scoutComparisonEndPoint);
            List<PitchTabularData_Metric> metricsScoutComparison = ScrapeMetricsForEachPitch(allRowsScoutComparison);
            return metricsScoutComparison;
        }

        #endregion TABULAR DATA > TRAJECTORY AND MOVEMENT - LISTS ------------------------------------------------------------





        #region TABULAR DATA > TRAJECTORY AND MOVEMENT - ENDPOINTS ------------------------------------------------------------

        // STATUS [ September 10, 2019 ] : Y
        // * TABULAR DATA > TRAJECTORY AND MOVEMENT > NO COMPARISON (NONE)
        public string TabularData_NoComparisonEndPoint(int playerId)
        {
            string tableType      = _endPoints.TableType(BrooksBaseballEndPoints.TableTypeEnum.TrajectoryAndMovement);
            string comparisonType = _endPoints.ComparisonType(BrooksBaseballEndPoints.ComparisonTypeEnum.NoComparison);

            string endPoint_NoComparison = _endPoints.TabularData_EndPoint(
                playerId,
                tableType,
                comparisonType
            ).EndPointUri;

            C.WriteLine($"endPoint_NoComparison : {endPoint_NoComparison}");
            return endPoint_NoComparison;
        }

        // STATUS [ September 10, 2019 ] : Y
        // * TABULAR DATA > TRAJECTORY AND MOVEMENT > Z SCORE COMPARISON
        public string TabularData_ZScoreComparisonEndPoint(int playerId)
        {
            string tableType      = _endPoints.TableType(BrooksBaseballEndPoints.TableTypeEnum.TrajectoryAndMovement);
            string comparisonType = _endPoints.ComparisonType(BrooksBaseballEndPoints.ComparisonTypeEnum.ZScoreComparison);

            string endPoint_ZScore = _endPoints.TabularData_EndPoint(
                playerId,
                tableType,
                comparisonType
            ).EndPointUri;

            C.WriteLine($"endPoint_NoComparison : {endPoint_ZScore}");
            return endPoint_ZScore;
        }

        // STATUS [ September 10, 2019 ] : Y
        // * TABULAR DATA > TRAJECTORY AND MOVEMENT > PITCH IQ COMPARISON
        public string TabularData_PitchIQComparisonEndPoint(int playerId)
        {
            string tableType      = _endPoints.TableType(BrooksBaseballEndPoints.TableTypeEnum.TrajectoryAndMovement);
            string comparisonType = _endPoints.ComparisonType(BrooksBaseballEndPoints.ComparisonTypeEnum.PitchIQComparison);

            string endPoint_PitchIQ = _endPoints.TabularData_EndPoint(
                playerId,
                tableType,
                comparisonType
            ).EndPointUri;

            C.WriteLine($"endPoint_NoComparison : {endPoint_PitchIQ}");
            return endPoint_PitchIQ;
        }

        // STATUS [ September 10, 2019 ] : Y
        // * TABULAR DATA > TRAJECTORY AND MOVEMENT > SCOUT COMPARISON
        public string TabularData_ScoutComparisonEndPoint(int playerId)
        {
            string tableType      = _endPoints.TableType(BrooksBaseballEndPoints.TableTypeEnum.TrajectoryAndMovement);
            string comparisonType = _endPoints.ComparisonType(BrooksBaseballEndPoints.ComparisonTypeEnum.ScoutComparison);

            string endPoint_ScoutComparison = _endPoints.TabularData_EndPoint(
                playerId,
                tableType,
                comparisonType
            ).EndPointUri;

            C.WriteLine($"endPoint_NoComparison : {endPoint_ScoutComparison}");
            return endPoint_ScoutComparison;
        }


        #endregion TABULAR DATA > TRAJECTORY AND MOVEMENT - ENDPOINTS ------------------------------------------------------------





        #region TABULAR DATA > TRAJECTORY AND MOVEMENT - WORKER METHODS ------------------------------------------------------------

        // * Column Headers Are:
        // * > Pitch Type, Count, Freq, Velo (mph), pfx HMov (in.), pfx VMov (in.), H. Rel (ft.), V. Rel(ft.)

        // * Note: these are not specific for any Comparison Mode type;
        // * They work for all Comparison Mode
        // *    Comparison Types: 1) NoComparison 2) ZScore 3) PitchIQ 4) Scout


        // STATUS [ September 10, 2019 ] : this works
        // * endPoints will come from various brooks end points
        // * Works with Tabular Data > Trajectory and Movement Tables > all Comparison Mode Types
        // public IEnumerable<HtmlNode> GetAllTableRows(string endPoint)
        // {
        //     HtmlWeb htmlWeb = new HtmlWeb ();
        //     HtmlDocument htmlWeb1 = htmlWeb.Load (endPoint);

        //     // * The # of rows will vary for pitcher
        //     // * First row is headers, all additional rows are for each pitch the pitcher throws
        //     // * E.g., if pitcher throws 3 pitchers then allTableRows count = 4
        //     IEnumerable<HtmlNode> allTableRows = from table in htmlWeb1.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
        //         from head in table.SelectNodes("thead").Cast<HtmlNode>()
        //         from row in head.SelectNodes("tr").Cast<HtmlNode>()
        //         select row;

        //     // int tableRowsCount = allTableRows.Count();
        //     return allTableRows;
        // }


        // STATUS [ September 10, 2019 ] : this works
        // * Works with Tabular Data > Trajectory and Movement Tables > all Comparison Mode Types
        public List<PitchTabularData_Metric> ScrapeMetricsForEachPitch(IEnumerable<HtmlNode> allTableRows)
        {
            List<PitchTabularData_Metric> metricsForEachPitch = new List<PitchTabularData_Metric>();

            int tableRowsCount = allTableRows.Count();
            for(var rowCounter = 0; rowCounter <= tableRowsCount - 1; rowCounter++)
            {
                // * Represents a row in the table
                // * The count is = to number of cells / child nodes in the row
                // * For each pitch, there are 8 children (i.e., one cell for each data value)
                HtmlNodeCollection currentRowNodeCollection = allTableRows.ElementAt(rowCounter).ChildNodes;

                // * Row 1 (index 0) is column header names
                // * All other rows (i.e., rows > 0) are for each pitch
                if(rowCounter > 0)
                {
                    var newInstance = InstantiatePitchTabularData_Metric(currentRowNodeCollection);
                    metricsForEachPitch.Add(newInstance);
                }
            }
            return metricsForEachPitch;
        }

        #endregion TABULAR DATA > TRAJECTORY AND MOVEMENT - WORKER METHODS ------------------------------------------------------------





        #region MODEL INSTANTIATION ------------------------------------------------------------

        // STATUS [ September 10, 2019 ] : this works but incomplete
        public BrooksBaseballPitcher InstantiateNewBrooksBaseballPitcher([FromQuery]List<PitchTabularData_Metric> metricsForEachPitch_NoComparison, [FromQuery]List<PitchTabularData_Metric> metricsForEachPitch_ZScoreComparison, [FromQuery]List<PitchTabularData_Metric> metricsForEachPitch_PitchIQ, [FromQuery]List<PitchTabularData_Metric> metricsForEachPitch_Scout)
        {
            // BrooksBaseballPitcher > BrooksBaseballPitcherProfile > PitchTabularData > PitchTabularData_TrajectoryAndMovement
            PitchTabularData_TrajectoryAndMovement pitchTabularData_TrajectoryAndMovement = InstantiatePitchTabularData_TrajectoryAndMovement(
                metricsForEachPitch_NoComparison,
                metricsForEachPitch_ZScoreComparison,
                metricsForEachPitch_PitchIQ,
                metricsForEachPitch_Scout
            );

            // BrooksBaseballPitcher > BrooksBaseballPitcherProfile > PitchTabularData
            PitchTabularData pitchTabularData = new PitchTabularData
            {
                PitchTabularData_TrajectoryAndMovement = pitchTabularData_TrajectoryAndMovement
            };

            // BrooksBaseballPitcher > BrooksBaseballPitcherProfile
            BrooksBaseballPitcherProfile pitcherProfile = new BrooksBaseballPitcherProfile
            {
                PitchTabularData = pitchTabularData
            };

            // BrooksBaseballPitcher
            BrooksBaseballPitcher brooksBaseballPitcher = new BrooksBaseballPitcher
            {
                PitcherProfile = pitcherProfile
            };

            // _helpers.Dig(brooksBaseballPitcher);
            return brooksBaseballPitcher;
        }


        // STATUS [ September 10, 2019 ] : this works
        // * BrooksBaseballPitcher > BrooksBaseballPitcherProfile > PitchTabularData > PitchTabularData_TrajectoryAndMovement
        public PitchTabularData_TrajectoryAndMovement InstantiatePitchTabularData_TrajectoryAndMovement(
            [FromQuery]List<PitchTabularData_Metric> metricsForEachPitch_NoComparison,
            [FromQuery]List<PitchTabularData_Metric> metricsForEachPitch_ZScoreComparison,
            [FromQuery]List<PitchTabularData_Metric> metricsForEachPitch_PitchIQ,
            [FromQuery]List<PitchTabularData_Metric> metricsForEachPitch_Scout)
        {
            // * BrooksBaseballPitcher > BrooksBaseballPitcherProfile > PitchTabularData > PitchTabularData_TrajectoryAndMovement > (1) PitchTabularData_NoComparison
            PitchTabularData_NoComparison pitcherNoComparison = new PitchTabularData_NoComparison
            {
                MetricsForEachPitch = metricsForEachPitch_NoComparison
            };

            // * BrooksBaseballPitcher > BrooksBaseballPitcherProfile > PitchTabularData > PitchTabularData_TrajectoryAndMovement > (2) PitchTabularData_ZScore
            PitchTabularData_ZScore pitcherZScoreComparison = new PitchTabularData_ZScore
            {
                MetricsForEachPitch = metricsForEachPitch_ZScoreComparison
            };

            // * BrooksBaseballPitcher > BrooksBaseballPitcherProfile > PitchTabularData > PitchTabularData_TrajectoryAndMovement > (3) PitchTabularData_PitchIQ
            PitchTabularData_PitchIQ pitcherPitchIQComparison = new PitchTabularData_PitchIQ
            {
                MetricsForEachPitch = metricsForEachPitch_PitchIQ
            };

            // * BrooksBaseballPitcher > BrooksBaseballPitcherProfile > PitchTabularData > PitchTabularData_TrajectoryAndMovement > (4) PitchTabularData_Scout
            PitchTabularData_Scout pitcherScoutComparison = new PitchTabularData_Scout
            {
                MetricsForEachPitch = metricsForEachPitch_Scout
            };

            // * BrooksBaseballPitcher > BrooksBaseballPitcherProfile > PitchTabularData > PitchTabularData_TrajectoryAndMovement
            return new PitchTabularData_TrajectoryAndMovement
            {
                PitchTabularData_NoComparison = pitcherNoComparison,
                PitchTabularData_ZScore       = pitcherZScoreComparison,
                PitchTabularData_PitchIQ      = pitcherPitchIQComparison,
                PitchTabularData_Scout        = pitcherScoutComparison
            };
        }


        // STATUS [ September 10, 2019 ] : this works
        // * Instantiate new InstantiatePitchTabularData_NoComparisonInstance
        // * The "FrequencyThrown_NoComparison" contains a % which is removed before instantiating new instance
        // * BrooksBaseballPitcher > BrooksBaseballPitcherProfile > PitchTabularData > PitchTabularData_TrajectoryAndMovement > all Comparison Types
        // *    Comparison Types: 1) NoComparison 2) ZScore 3) PitchIQ 4) Scout
        public PitchTabularData_Metric InstantiatePitchTabularData_Metric(HtmlNodeCollection currentRowNodeCollection)
        {
            string freqStringWithPercentageSymbol = currentRowNodeCollection.ElementAt(2).InnerText;
            string splitString = freqStringWithPercentageSymbol.Split('%').First();
            double frequencyThrownDouble = double.Parse(splitString);

            return new PitchTabularData_Metric
            {
                PitchType              = currentRowNodeCollection.ElementAt(0).InnerText,
                NumberOfPitches        = int.Parse(currentRowNodeCollection.ElementAt(1).InnerText),
                FrequencyThrown        = frequencyThrownDouble,
                PitchVelocity          = double.Parse(currentRowNodeCollection.ElementAt(3).InnerText),
                HorizontalMovement     = double.Parse(currentRowNodeCollection.ElementAt(4).InnerText),
                VerticalMovement       = double.Parse(currentRowNodeCollection.ElementAt(5).InnerText),
                HorizontalReleasePoint = double.Parse(currentRowNodeCollection.ElementAt(6).InnerText),
                VerticalReleasePoint   = double.Parse(currentRowNodeCollection.ElementAt(7).InnerText)
            };
        }


        #endregion MODEL INSTANTIATION ------------------------------------------------------------

    }
}
