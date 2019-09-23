using System.Collections.Generic;

#pragma warning disable MA0016, MA0048
namespace BaseballScraper.Models.BrooksBaseball
{
    public class BrooksBaseballPitcher
    {
        public string PlayerName                            { get; set; }
        public int? PlayerId                                { get; set; }
        public BrooksBaseballPitcherProfile PitcherProfile  { get; set; }

        public BrooksBaseballPitcher()
        {
            PitcherProfile = new BrooksBaseballPitcherProfile();
        }
    }




    #region PITCHER PROFILE ------------------------------------------------------------


    // NOTES as of [ September 10, 2019 ]
    // * PITCHER PROFILE : Each Pitcher has 6 tabs of data:
    // *    1) [ 1.0 ] Landing Page
    // *    2) [ 2.0 ] Tabular Data
    // *    3) [ 3.0 ] Velo and Movement
    // *    4) [ 4.0 ] Usage and Outcomes
    // *    5) [ 5.0 ] Scatter Charts
    // *    6) [ 6.0 ] Zone Profile
    // * Example: http://bit.ly/2kaDOr4
    // * The class 'PitcherProfile' is used to capture info on these tabs
    // * Within each tab there are different filters / splits etc. you can select


    public class BrooksBaseballPitcherProfile
    {
        // * [ 1.0 | Tab 1 ] Landing Page
        public PitcherLandingPage PitcherLandingPage { get; set; }

        // * [ 2.0 > Tab 2 ] Tabular Data
        public PitchTabularData PitchTabularData     { get; set; }


        public BrooksBaseballPitcherProfile()
        {
            PitcherLandingPage = new PitcherLandingPage();
            PitchTabularData   = new PitchTabularData();
        }
    }


    /* ------------------------------------------------------ */
    /*                                                        */
    /*             [    TAB 1 : LANDING PAGE    ]             */
    /*                                                        */
    /* ------------------------------------------------------ */

    // 1.0
    // * [ Tab 1 ] : Landing Page
    public partial class PitcherLandingPage
    {
        public string PitchRepertoireAtAGlance         { get; set; }
        public string PitchesComparedToOtherPitchers   { get; set; }

    }



    /* ------------------------------------------------------ */
    /*                                                        */
    /*             [    TAB 2 : TABULAR DATA    ]             */
    /*                                                        */
    /* ------------------------------------------------------ */


    // 2.0
    // * [ Tab 2 ] Tabular Data
    // * Tabular data has 7 Table Types:
    // *    1) [ 2.1 ] Trajectory and Movement
    // *    2) [ 2.2 ] Pitch Usage
    // *    3) [ 2.3 ] Pitch Outcomes
    // *    4) [ 2.4 ] Sabermetric Outcomes
    // *    5) [ 2.5 ] Results and Averages
    // *    6) [ 2.6 ] Batted Balls
    // *    7) [ 2.7 ] Game Logs
    public partial class PitchTabularData
    {
        // 2.1 : Tabular Data > Trajectory and Movement
        public PitchTabularData_TrajectoryAndMovement PitchTabularData_TrajectoryAndMovement { get; set; }

        public PitchTabularData()
        {
            PitchTabularData_TrajectoryAndMovement = new PitchTabularData_TrajectoryAndMovement();
        }
    }


    // 2.1 : Tabular Data > Trajectory and Movement
    // * [ Tab        : 2 ] : Tabular Data
    // * [ Table Type : 1 ] : Trajectory and Movement
    // * Trajectory and Movement has 4 Comparison Modes
    // *    1) [ 2.1.1 ] None (i.e., NoComparison)
    // *    2) [ 2.1.2 ] ZScore
    // *    3) [ 2.1.3 ] PitchIQ
    // *    4) [ 2.1.4 ] Scout
    // * Each Comparison Mode has 8 Metrics
    // *    1) [ 2.1.X.1 ] Pitch Type     | PitchType
    // *    2) [ 2.1.X.2 ] Count          | Number of Pitches
    // *    3) [ 2.1.X.3 ] Freq           | Frequency Thrown
    // *    4) [ 2.1.X.4 ] Velo           | PitchVelocity
    // *    5) [ 2.1.X.5 ] pfx HMov (in.) | HorizontalMovement
    // *    6) [ 2.1.X.6 ] pfx VMov (in.) | VerticalMovement
    // *    7) [ 2.1.X.7 ] H. Rel   (ft.) | HorizontalReleasePoint
    // *    8) [ 2.1.X.8 ] V. Rel   (ft.) | VerticalReleasePoint
    public partial class PitchTabularData_TrajectoryAndMovement
    {
        public PitchTabularData_NoComparison PitchTabularData_NoComparison  { get; set; }
        public PitchTabularData_ZScore       PitchTabularData_ZScore        { get; set; }
        public PitchTabularData_PitchIQ      PitchTabularData_PitchIQ       { get; set; }
        public PitchTabularData_Scout        PitchTabularData_Scout         { get; set; }


        public PitchTabularData_TrajectoryAndMovement()
        {
            PitchTabularData_NoComparison = new PitchTabularData_NoComparison();
            PitchTabularData_ZScore       = new PitchTabularData_ZScore();
            PitchTabularData_PitchIQ      = new PitchTabularData_PitchIQ();
            PitchTabularData_Scout        = new PitchTabularData_Scout();
        }
    }


    // 2.1.1 : Tabular Data > Trajectory and Movement > None (i.e., NoComparison)
    // * [ Tab             : 2 ] : Tabular Data
    // * [ Table Type      : 1 ] : Trajectory and Movement
    // * [ Comparison Mode : 1 ] : NoComparison
    // * You need one of these for each pitch (e.g., Fastball, Curve, Change, etc.)
    public partial class PitchTabularData_NoComparison
    {
        public List<PitchTabularData_Metric> MetricsForEachPitch { get; set; }
        public PitchTabularData_NoComparison()
        {
            MetricsForEachPitch = new List<PitchTabularData_Metric>();
        }
    }


    // 2.1.2 : Tabular Data > Trajectory and Movement > ZScore
    // * [ Tab             : 2 ] : Tabular Data
    // * [ Table Type      : 1 ] : Trajectory and Movement
    // * [ Comparison Mode : 2 ] : ZScore
    // * You need one of these for each pitch (e.g., Fastball, Curve, Change, etc.)
    public partial class PitchTabularData_ZScore
    {
        public List<PitchTabularData_Metric> MetricsForEachPitch { get; set; }
        public PitchTabularData_ZScore()
        {
            MetricsForEachPitch = new List<PitchTabularData_Metric>();
        }
    }


    // 2.1.3 : Tabular Data > Trajectory and Movement > PitchIQ
    // * [ Tab             : 2 ] : Tabular Data
    // * [ Table Type      : 1 ] : Trajectory and Movement
    // * [ Comparison Mode : 3 ] : PitchIQ
    // * You need one of these for each pitch (e.g., Fastball, Curve, Change, etc.)
    public partial class PitchTabularData_PitchIQ
    {
        public List<PitchTabularData_Metric> MetricsForEachPitch { get; set; }

        public PitchTabularData_PitchIQ()
        {
            MetricsForEachPitch = new List<PitchTabularData_Metric>();
        }
    }


    // 2.1.4 : Tabular Data > Trajectory and Movement > Scout
    // * [ Tab             : 2 ] : Tabular Data
    // * [ Table Type      : 1 ] : Trajectory and Movement
    // * [ Comparison Mode : 4 ] : Scout
    // * You need one of these for each pitch (e.g., Fastball, Curve, Change, etc.)
    public partial class PitchTabularData_Scout
    {
        public List<PitchTabularData_Metric> MetricsForEachPitch { get; set; }

        public PitchTabularData_Scout()
        {
            MetricsForEachPitch = new List<PitchTabularData_Metric>();
        }
    }


    // * Applicable to each Comparison Type :
    // *    Comparison Types: 1) NoComparison 2) ZScore 3) PitchIQ 4) Scout
    public partial class PitchTabularData_Metric
    {
        // [ 2.1.X.1 ] Pitch Type
        public string PitchType              { get; set; }

        // [ 2.1.X.2 ] Count
        public int NumberOfPitches           { get; set; }

        // [ 2.1.X.3 ] Freq
        public double FrequencyThrown        { get; set; }

        // [ 2.1.X.4 ] Velo
        public double PitchVelocity         { get; set; }

        // [ 2.1.X.5 ] pfx HMov (in.)
        public double HorizontalMovement     { get; set; }

        // [ 2.1.X.6 ] pfx VMov (in.)
        public double VerticalMovement       { get; set; }

        // [ 2.1.X.7 ] H. Rel (ft.)
        public double HorizontalReleasePoint { get; set; }

        // [ 2.1.X.8 ] V. Rel (ft.)
        public double VerticalReleasePoint   { get; set; }
    }


    // 2.2 : Tabular Data > Pitch Usage
    // * [ Tab        : 2 ] : Tabular Data
    // * [ Table Type : 2 ] : Pitch Usage
    public partial class TabularData_PitchUsage
    {
        // TO DO
    }



    // 2.3 : Tabular Data > Pitch Outcomes
    // * [ Tab        : 2 ] : Tabular Data
    // * [ Table Type : 3 ] : Pitch Outcomes
    // * Pitch Outcomes has 13 Metrics
    // *    1) [ 2.3.1  ] Pitch Type | PitchType
    // *    2) [ 2.3.2  ] Count      | Number of Pitches
    // *    3) [ 2.3.3  ] Ball       | BallPercentage
    // *    4) [ 2.3.4  ] Strike     | StrikePercentage
    // *    5) [ 2.3.5  ] Swing      | SwingPercentage
    // *    6) [ 2.3.6  ] Foul       | FoulPercentage
    // *    7) [ 2.3.7  ] Whiffs     | WhiffPercentage
    // *    8) [ 2.3.8  ] BIP        | BallInPlayPercentage
    // *    8) [ 2.3.9  ] GB         | GroundBallPercentage
    // *    8) [ 2.3.10 ] LD         | LineDrivePercentage
    // *    8) [ 2.3.11 ] FB         | FlyBallPercentage
    // *    8) [ 2.3.12 ] PU         | PopUpPercentage
    // *    8) [ 2.3.13 ] HR         | HomeRunPercentage
    public partial class TabularData_PitchOutcomes
    {
        // [ 2.3.1 ]
        public string PitchType            { get; set; }

        // [ 2.3.2 ]
        public int NumberOfPitches         { get; set; }

        // [ 2.3.3 ]
        public double BallPercentage       { get; set; }

        // [ 2.3.4 ]
        public double StrikePercentage     { get; set; }

        // [ 2.3.5 ]
        public double SwingPercentage      { get; set; }

        // [ 2.3.6 ]
        public double FoulPercentage       { get; set; }

        // [ 2.3.7 ]
        public double WhiffPercentage      { get; set; }

        // [ 2.3.8 ]
        public double BallInPlayPercentage { get; set; }

        // [ 2.3.9 ]
        public double GroundBallPercentage { get; set; }

        // [ 2.3.10 ]
        public double LineDrivePercentage  { get; set; }

        // [ 2.3.11 ]
        public double FlyBallPercentage    { get; set; }

        // [ 2.3.12 ]
        public double PopUpPercentage      { get; set; }

        // [ 2.3.13 ]
        public double HomeRunPercentage    { get; set; }
    }



    // NOTE: September 10, 2019
    // * Lots more to add here:
    // *  1) Other Table Types for Tabular Data
    // *  2) All other tabs


    #endregion PITCHER PROFILE ------------------------------------------------------------





}






// // [ 2.1.1.1 ] Pitch Type
// public string PitchType_NoComparison              { get; set; }

// // [ 2.1.1.2 ] Count
// public int NumberOfPitches_NoComparison           { get; set; }

// // [ 2.1.1.3 ] Freq
// public double FrequencyThrown_NoComparison        { get; set; }

// // [ 2.1.1.4 ] Velo
// public double PitchVelocity_NoComparison          { get; set; }

// // [ 2.1.1.5 ] pfx HMov (in.)
// public double HorizontalMovement_NoComparison     { get; set; }

// // [ 2.1.1.6 ] pfx VMov (in.)
// public double VerticalMovement_NoComparison       { get; set; }

// // [ 2.1.1.7 ] H. Rel (ft.)
// public double HorizontalReleasePoint_NoComparison { get; set; }

// // [ 2.1.1.8 ] V. Rel (ft.)
// public double VerticalReleasePoint_NoComparison   { get; set; }





// // 2.1.2 : Tabular Data > Trajectory and Movement > ZScore
// // * [ Tab             : 2 ] : Tabular Data
// // * [ Table Type      : 1 ] : Trajectory and Movement
// // * [ Comparison Mode : 2 ] : ZScore
// // * You need one of these for each pitch (e.g., Fastball, Curve, Change, etc.)
// public partial class PitchTabularData_ZScore
// {
// // [ 2.1.2.1 ] Pitch Type
// public string PitchType_ZScore              { get; set; }

// // [ 2.1.2.2 ] Count
// public int NumberOfPitches_ZScore           { get; set; }

// // [ 2.1.2.3 ] Freq
// public double FrequencyThrown_ZScore        { get; set; }

// // [ 2.1.2.4 ] Velo
// public double PitchVelocity_ZScore          { get; set; }

// // [ 2.1.2.5 ] pfx HMov (in.)
// public double HorizontalMovement_ZScore     { get; set; }

// // [ 2.1.2.6 ] pfx VMov (in.)
// public double VerticalMovement_ZScore       { get; set; }

// // [ 2.1.2.7 ] H. Rel (ft.)
// public double HorizontalReleasePoint_ZScore { get; set; }

// // [ 2.1.2.8 ] V. Rel (ft.)
// public double VerticalReleasePoint_ZScore   { get; set; }
// }


// // 2.1.3 : Tabular Data > Trajectory and Movement > PitchIQ
// // * [ Tab             : 2 ] : Tabular Data
// // * [ Table Type      : 1 ] : Trajectory and Movement
// // * [ Comparison Mode : 3 ] : PitchIQ
// // * You need one of these for each pitch (e.g., Fastball, Curve, Change, etc.)
// public partial class PitchTabularData_PitchIQ
// {
// // [ 2.1.3.1 ] Pitch Type
// public string PitchType_PitchIQ              { get; set; }

// // [ 2.1.3.2 ] Count
// public int NumberOfPitches_PitchIQ           { get; set; }

// // [ 2.1.3.3 ] Freq
// public double FrequencyThrown_PitchIQ        { get; set; }

// // [ 2.1.3.4 ] Velo
// public double PitchVelocity_PitchIQ          { get; set; }

// // [ 2.1.3.5 ] pfx HMov (in.)
// public double HorizontalMovement_PitchIQ     { get; set; }

// // [ 2.1.3.6 ] pfx VMov (in.)
// public double VerticalMovement_PitchIQ       { get; set; }

// // [ 2.1.3.7 ] H. Rel (ft.)
// public double HorizontalReleasePoint_PitchIQ { get; set; }

// // [ 2.1.3.8 ] V. Rel (ft.)
// public double VerticalReleasePoint_PitchIQ   { get; set; }
// }


// // 2.1.4 : Tabular Data > Trajectory and Movement > Scout
// // * [ Tab             : 2 ] : Tabular Data
// // * [ Table Type      : 1 ] : Trajectory and Movement
// // * [ Comparison Mode : 4 ] : Scout
// // * You need one of these for each pitch (e.g., Fastball, Curve, Change, etc.)
// public partial class PitchTabularData_Scout
// {
// // [ 2.1.4.1 ] Pitch Type
// public string PitchType_Scout              { get; set; }

// // [ 2.1.4.2 ] Count
// public int NumberOfPitches_Scout           { get; set; }

// // [ 2.1.4.3 ] Freq
// public double FrequencyThrown_Scout        { get; set; }

// // [ 2.1.4.4 ] Velo
// public double PitchVelocity_Scout          { get; set; }

// // [ 2.1.4.5 ] pfx HMov (in.)
// public double HorizontalMovement_Scout     { get; set; }

// // [ 2.1.4.6 ] pfx VMov (in.)
// public double VerticalMovement_Scout       { get; set; }

// // [ 2.1.4.7 ] H. Rel (ft.)
// public double HorizontalReleasePoint_Scout { get; set; }

// // [ 2.1.4.8 ] V. Rel (ft.)
// public double VerticalReleasePoint_Scout   { get; set; }
// }
