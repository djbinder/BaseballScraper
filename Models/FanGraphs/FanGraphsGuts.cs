namespace BaseballScraper.Models.FanGraphs
{
    // https://www.fangraphs.com/guts.aspx?type=pfh&teamid=0&season=2017
    #pragma warning disable IDE1006, CS1591
    public class FanGraphsGuts
    {
        public class wObaFipConstants
        {
            public int Season { get; set; }
            public int wOba { get; set; }
            public int wObaScale { get; set;}
            public int wWalks { get; set; }
            public int wHitByPitches { get; set; }
            public int wSingles { get; set; }
            public int wDoubles { get; set; }
            public int wTriples { get; set; }
            public int wHomeRuns { get; set; }
            public int RunStolenBases { get; set; }
            public int RunCaughtStealing { get; set; }
            public int RunsPerPlateAppearance{ get; set; }
            public int RW { get; set; }
            public int cFip { get; set; }
        }


        public class ParkFactors
        {
            public int Season { get; set; }
            public string TeamNameShort { get; set; }
            public int FiveYear { get; set; }
            public int ThreeYear { get; set; }
            public int OneYear { get; set; }
            public int ParkFactorSingles { get; set; }
            public int ParkFactorDoubles { get; set; }
            public int ParkFactorTriples { get; set; }
            public int ParkFactorHomeRuns { get; set; }
            public int ParkFactorStrikeouts { get; set; }
            public int ParkFactorWalks { get; set; }
            public int ParkFactorGroundBalls { get; set; }
            public int ParkFactorFlyBalls { get; set; }
            public int ParkFactorLineDrives { get; set; }
            public int ParkFactorInfieldFlyBalls { get; set; }
            public int ParkFactorFip { get; set; }

        }


        public class HandednessParkFactors
        {
            public int Season { get; set; }
            public string TeamNameShort { get; set; }
            public int HpfSinglesAsL { get; set; }
            public int HpfSinglesAsR { get; set; }
            public int HpfDoublesAsL { get; set; }
            public int HpfDoublesAsR { get; set; }
            public int HpfTriplesAsL { get; set; }
            public int HpfTriplesAsR { get; set; }
            public int HpfHomeRunsAsL { get; set; }
            public int HpfHomeRunsAsR { get; set; }
        }
    }
}
