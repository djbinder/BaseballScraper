#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
using CsvHelper.Configuration;
using BaseballScraper.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System;

namespace BaseballScraper.Models.FanGraphs
{

    public class FanGraphsPitcher : BaseEntity
    {
        public string RecordNumber { get; set; }
        public string FanGraphsName { get; set; }
        public string FanGraphsTeam { get; set; }
        public string FanGraphsAge { get; set; }
        public string GamesStarted { get; set; }
        public string InningsPitched { get; set; }
        public string TotalBattersFaced { get; set; }
        public string Wins { get; set; }
        public string Saves { get; set; }
        public string Strikeouts { get; set; }
        public string Holds { get; set; }
        public string EarnedRunAverage { get; set; }
        public string Whip { get; set; }
        public string StrikeoutPercentage { get; set; }
        public string WalkPercentage { get; set; }
        public string StrikeoutsMinusWalks { get; set; }
        public string StrikeoutsPerNine { get; set; }
        public string WalksPerNine { get; set; }
        public string StrikeoutsDividedByWalks { get; set; }
        public string Balls { get; set; }
        public string Strikes { get; set; }
        public string Pitches { get; set; }
        public string HomeRunsPerNine { get; set; }
        public string GroundBallPercentage { get; set; }
        public string LineDrivePercentage { get; set; }
        public string FlyBallPercentage { get; set; }
        public string InfieldFlyBallPercentage { get; set; }
        public string OSwingPercentage { get; set; }
        public string OSwingPercentagePitchFx { get; set; }
        public string OSwingPercentagePitchInfo { get; set; }
        public string ZContactPercentage { get; set; }
        public string ZContactPercentagePitchFx { get; set; }
        public string ZContactPercentagePitchInfo { get; set; }
        public string ContactPercentage { get; set; }
        public string ContactPercentagePitchFx { get; set; }
        public string ContactPercentagePitchInfo { get; set; }
        public string ZonePercentage { get; set; }
        public string ZonePercentagePitchFx { get; set; }
        public string ZonePercentagePitchInfo { get; set; }
        public string FStrikePercentage { get; set; }
        public string SwingingStrikePercentage { get; set; }
        public string PullPercentage { get; set; }
        public string SoftPercentage { get; set; }
        public string sHardPercentage { get; set; }
        public string Babip { get; set; }
        public string LeftOnBasePercentage { get; set; }
        public string HomeRunsDividedByFlyBalls { get; set; }
        public string FastballVelocityPitchFx { get; set; }
        public string EarnedRunAverageRepeat { get; set; }
        public string EarnedRunAverageMinus { get; set; }
        public string Fip { get; set; }
        public string FipMinus { get; set; }
        public string EarnedRunAverageMinusFip { get; set; }
        public string XFip { get; set; }
        public string XFipMinus { get; set; }
        public string Siera { get; set; }
        public string RunsPerNine { get; set; }

    }


    public class FanGraphsPitcherForWpdiReportSingleDay : FanGraphsPitcherForWpdiReport
    {
        new public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        new public DateTime DateUpdated { get; set; }  // from IBaseEntity interface
        public DateTime DatePitched     { get; set; }
    }


    // public class FanGraphsPitcherForWpdiReportFullSeason : FanGraphsPitcherForWpdiReport
    // {
    //     public int Season { get; set; }
    //     new public DateTime DateCreated { get; set; }  // from IBaseEntity interface
    //     new public DateTime DateUpdated { get; set; }  // from IBaseEntity interface
    // }


    // See: https://www.fangraphs.com/leaders.aspx?pos=all&stats=sta&lg=all&qual=150&type=c,8,13,210,204,205,208,207,111,105,106,109,108&season=2018&month=0&season1=2018&ind=0&team=0&rost=0&age=0&filter=&players=0&startdate=2018-01-01&enddate=2018-12-31
    // See: https://bit.ly/33abnet
    // See: https://bit.ly/2YPh1TU
    public class FanGraphsPitcherForWpdiReport : IBaseEntity
    {
        private readonly CsvHandler _csvHandler = new CsvHandler();


        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface

        // [Key]
        // public int RecordId { get; set; }

        private string _playerYearConcat;

        [Key]
        public string PlayerYearConcat
        {
            get => $"{FanGraphsId}-{Season}";
            set => _playerYearConcat = value;
        }

        public int FanGraphsId            { get; set; }
        public int Season                 { get; set; }
        public string PitcherName         { get; set; }
        public string Team                { get; set; }
        public int GamesStarted           { get; set; }
        public decimal InningsPitched     { get; set; }
        public double Wpdi                { get; set; }
        public double Mpdi                { get; set; }


        public double Apercentage { get; set; }
        public double Bpercentage { get; set; }
        public double Cpercentage { get; set; }
        public double Dpercentage { get; set; }
        public double Epercentage { get; set; }
        public double Fpercentage { get; set; }


        public double OutcomeApercentage { get; set; }
        public double OutcomeBpercentage { get; set; }
        public double OutcomeCpercentage { get; set; }
        public double OutcomeDpercentage { get; set; }
        public double OutcomeEpercentage { get; set; }
        public double OutcomeFpercentage { get; set; }



        // for mPDI, C, D, and E, are 0 for all pitchers so do not need them
        public double OutcomeApercentage_mPDI { get; set; }
        public double OutcomeBpercentage_mPDI { get; set; }
        public double OutcomeFpercentage_mPDI { get; set; }


        public string ZonePercentageString { get; set; }
        private double _zonePercentage;
        public double ZonePercentage
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(ZonePercentageString)/10;
            }

            set
            {
                _zonePercentage = value;
            }
        }


        public string OSwingPercentageString   { get; set; }
        private double _oSwingPercentage;
        public double OSwingPercentage
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(OSwingPercentageString)/10;
            }
            set
            {
                _oSwingPercentage = value;
            }
        }


        public string ZSwingPercentageString   { get; set; }
        private double _zSwingPercentage;
        public double ZSwingPercentage
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(ZSwingPercentageString)/10;
            }
            set
            {
                _zSwingPercentage = value;
            }
        }

        public string ZContactPercentageString { get; set; }
        private double _zContactPercentage;
        public double ZContactPercentage
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(ZContactPercentageString)/10;
            }
            set
            {
                _zContactPercentage = value;
            }
        }


        public string OContactPercentageString { get; set; }
        private double _oContactPercentage;
        public double OContactPercentage
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(OContactPercentageString)/10;
            }
            set
            {
                _oContactPercentage = value;
            }
        }


        public string ZonePercentageStringPfx { get; set; }
        private double _zonePercentagePfx;
        public double ZonePercentagePfx
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(ZonePercentageStringPfx)/10;
            }
            set
            {
                _zonePercentagePfx = value;
            }
        }

        public string OSwingPercentageStringPfx   { get; set; }
        private double _oSwingPercentagePfx;
        public double OSwingPercentagePfx
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(OSwingPercentageStringPfx)/10;
            }
            set
            {
                _oSwingPercentagePfx = value;
            }
        }


        public string ZSwingPercentageStringPfx   {get; set; }
        private double _zSwingPercentagePfx;
        public double ZSwingPercentagePfx
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(ZSwingPercentageStringPfx)/10;
            }
            set
            {
                _zSwingPercentagePfx = value;
            }
        }

        public string ZContactPercentageStringPfx {get; set; }
        private double _zContactPercentagePfx;
        public double ZContactPercentagePfx
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(ZContactPercentageStringPfx)/10;
            }
            set
            {
                _zContactPercentagePfx = value;
            }
        }


        public string OContactPercentageStringPfx { get; set; }
        private double _oContactPercentagePfx;
        public double OContactPercentagePfx
        {
            get
            {
                return _csvHandler.ConvertCellWithPercentageSymbolToDouble(OContactPercentageStringPfx)/10;
            }
            set
            {
                _oContactPercentagePfx = value;
            }
        }
    }


    public sealed class WpdiReportClassMap: ClassMap<FanGraphsPitcherForWpdiReport>
    {
        public WpdiReportClassMap()
        {
            Map( m => m.PitcherName                 ).Name("Name");
            Map( m => m.Team                        ).Name("Team");
            Map( m => m.GamesStarted                ).Name("GS");
            Map( m => m.InningsPitched              ).Name("IP");
            Map( m => m.FanGraphsId                 ).Name("playerid");
            Map( m => m.ZonePercentageString        ).Name("Zone%");
            Map( m => m.OSwingPercentageString      ).Name("O-Swing%");
            Map( m => m.ZSwingPercentageString      ).Name("Z-Swing%");
            Map( m => m.ZContactPercentageString    ).Name("Z-Contact%");
            Map( m => m.OContactPercentageString    ).Name("O-Contact%");
            Map( m => m.ZonePercentageStringPfx     ).Name("Zone% (pfx)");
            Map( m => m.OSwingPercentageStringPfx   ).Name("O-Swing% (pfx)");
            Map( m => m.ZSwingPercentageStringPfx   ).Name("Z-Swing% (pfx)");
            Map( m => m.ZContactPercentageStringPfx ).Name("Z-Contact% (pfx)");
            Map( m => m.OContactPercentageStringPfx ).Name("O-Contact% (pfx)");
        }
    }
}
