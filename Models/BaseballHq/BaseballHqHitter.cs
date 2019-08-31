// SEE: https://joshclose.github.io/CsvHelper/examples/configuration/class-maps/mapping-duplicate-names/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BaseballScraper.Models.Player;
using CsvHelper.Configuration;
using Ganss.Excel;

namespace BaseballScraper.Models.BaseballHq
{

    public partial class BaseballHqHitter : BaseEntity
    {

    }

    // There are two different CSV reports run / downloaded from Baseball Hq
    // * 1) Year to Date    2) Rest of Season Projection
    // * These are the fields / columns they both have in common
    public partial class BaseballHqReportHitterBase : IBaseEntity
    {
        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface


        // [ForeignKey("SfbbPlayerBase")]
        [Key]
        public int HQID { get; set; }

        [Ganss.Excel.Column("MLBAM ID")]
        public int MlbId { get; set; }

        [Ganss.Excel.Column("Lastname")]
        public string LastName { get; set; }

        [Ganss.Excel.Column("Firstname")]
        public string FirstName { get; set; }

        [Ganss.Excel.Column("Age")]
        public int Age { get; set; }

        [Ganss.Excel.Column("B")]
        public string Bats { get; set; }

        [Ganss.Excel.Column("Pos")]
        public string Position { get; set; }

        [Ganss.Excel.Column("Tm")]
        public string Team { get; set; }


        // public virtual SfbbPlayerBase SfbbPlayerBase { get; set; }

        // [ForeignKey("SfbbPlayerBase")]
        // public string IDPLAYER { get; set; }
    }


    /* --------------------------------------------------------- */
    /* YEAR TO DATE                                              */
    /* --------------------------------------------------------- */
    public partial class HqHitterYearToDate : BaseballHqReportHitterBase
    {
        public virtual SfbbPlayerBase SfbbPlayerBase { get; set; }

        [ForeignKey("SfbbPlayerBase")]
        public string IDPLAYER { get; set; }


        [Ganss.Excel.Column("C")]
        public int CatcherAppearancesYTD { get; set; }

        [Ganss.Excel.Column("1B")]
        public int FirstBaseAppearancesYTD { get; set; }

        [Ganss.Excel.Column("2B")]
        public int SecondBaseAppearancesYTD { get; set; }

        [Ganss.Excel.Column("3B")]
        public int ThirdBaseAppearancesYTD { get; set; }

        [Ganss.Excel.Column("SS")]
        public int ShortstopAppearancesYTD { get; set; }

        [Ganss.Excel.Column("OF")]
        public int OutfielderAppearancesYTD { get; set; }

        [Ganss.Excel.Column("DH")]
        public int DesignatedHitterAppearancesYTD { get; set; }

        [Ganss.Excel.Column("AB")]
        public int AtBatsYTD { get; set; }

        [Ganss.Excel.Column("R")]
        public int RunsYTD { get; set; }

        [Ganss.Excel.Column("H")]
        public int HitsYTD { get; set; }

        [Ganss.Excel.Column("2B")]
        public int DoublesYTD { get; set; }

        [Ganss.Excel.Column("3B")]
        public int TriplesYTD { get; set; }

        [Ganss.Excel.Column("HR")]
        public int HomeRunsYTD { get; set; }

        [Ganss.Excel.Column("RBI")]
        public int RbiYTD { get; set; }

        [Ganss.Excel.Column("BB")]
        public int WalksYTD { get; set; }

        [Ganss.Excel.Column("K")]
        public int StrikeoutsYTD { get; set; }

        [Ganss.Excel.Column("SB")]
        public int StolenBasesYTD { get; set; }

        [Ganss.Excel.Column("CS")]
        public int CaughtStealingYTD { get; set; }

        [Ganss.Excel.Column("AVG")]
        public int BattingAverageYTD { get; set; }

        [Ganss.Excel.Column("OBP")]
        public int OnBasePercentageYTD { get; set; }

        [Ganss.Excel.Column("SLG")]
        public int SluggingPercentageYTD { get; set; }

        [Ganss.Excel.Column("OPS")]
        public int OnBasePlusSluggingYTD { get; set; }

        [Ganss.Excel.Column("BB%")]
        public int BbPercentageYTD { get; set; }

        [Ganss.Excel.Column("Ct%")]
        public int ContactPercentageYTD { get; set; }

        [Ganss.Excel.Column("Eye")]
        public double EyeYTD { get; set; }

        [Ganss.Excel.Column("H%")]
        public int HitPercentageYTD { get; set; }

        [Ganss.Excel.Column("PX")]
        public int PxYTD { get; set; }

        [Ganss.Excel.Column("xPx")]
        public int ExpectedPxYTD { get; set; }

        [Ganss.Excel.Column("hctX")]
        public int HctxYTD { get; set; }

        [Ganss.Excel.Column("SPD")]
        public int SpeedYTD { get; set; }

        [Ganss.Excel.Column("RSPD")]
        public int RspdYTD { get; set; }

        [Ganss.Excel.Column("G%")]
        public int GroundBallPercentageYTD { get; set; }

        [Ganss.Excel.Column("L%")]
        public int LineDrivePercentageYTD { get; set; }

        [Ganss.Excel.Column("F%")]
        public int FlyballPercentageYTD { get; set; }

        [Ganss.Excel.Column("XBA")]
        public int XbaYTD { get; set; }

        [Ganss.Excel.Column("RCG")]
        public double RcgYTD { get; set; }

        [Ganss.Excel.Column("RAR")]
        public double RarYTD { get; set; }

        [Ganss.Excel.Column("BPV")]
        public int BpvYTD { get; set; }

        [Ganss.Excel.Column("GB")]
        public int GroundballsYTD { get; set; }

        [Ganss.Excel.Column("GBO")]
        public int GroundballOutsYTD { get; set; }

        [Ganss.Excel.Column("LD")]
        public int LineDrivesYTD { get; set; }

        [Ganss.Excel.Column("LDO")]
        public int LineDriveOutsYTD { get; set; }

        [Ganss.Excel.Column("FB")]
        public int FlyballsYTD { get; set; }

        [Ganss.Excel.Column("FBO")]
        public int FlyballOutsYTD { get; set; }

        [Ganss.Excel.Column("PAvsRH")]
        public int PlateAppearancesVsRightHandersYTD { get; set; }

        [Ganss.Excel.Column("PAvsLH")]
        public int PlateAppearancesVsLeftHandersYTD { get; set; }

        [Ganss.Excel.Column("AVGvsRH")]
        public int BattingAverageVsRightHandersYTD { get; set; }

        [Ganss.Excel.Column("AVGvsLH")]
        public int BattingAverageVsLeftHandersYTD { get; set; }

        [Ganss.Excel.Column("OBPvsRH")]
        public int OnBasePercentageVsRightHanders { get; set; }

        [Ganss.Excel.Column("OBPvsLH")]
        public int OnBasePercentageVsLeftHanders { get; set; }

        [Ganss.Excel.Column("SLGvsRH")]
        public int SluggingPercentageVsRightHanders { get; set; }

        [Ganss.Excel.Column("SLGvsLH")]
        public int SluggingPercentageVsLeftHanders { get; set; }

        [Ganss.Excel.Column("OPSvsRH")]
        public int OpsVsRightHandersYTD { get; set; }

        [Ganss.Excel.Column("OPSvsLH")]
        public int OpsVsLeftHandersYTD { get; set; }

        [Ganss.Excel.Column("PX")]
        public int PxLast31 { get; set; }

        [Ganss.Excel.Column("xPx")]
        public int ExpectedPxLast31 { get; set; }

        [Ganss.Excel.Column("hctX")]
        public int HctxLast31 { get; set; }

        [Ganss.Excel.Column("XBA")]
        public int XbaLast31 { get; set; }

        [Ganss.Excel.Column("RCG")]
        public double RcgLast31 { get; set; }

        [Ganss.Excel.Column("RAR")]
        public double RarLast31 { get; set; }

        [Ganss.Excel.Column("BPV")]
        public int BpvLast31 { get; set; }

        [Ganss.Excel.Column("PX")]
        public int PxLast7 { get; set; }

        [Ganss.Excel.Column("xPx")]
        public int ExpectedPxLast7 { get; set; }

        [Ganss.Excel.Column("hctX")]
        public int HctxLast7 { get; set; }

        [Ganss.Excel.Column("XBA")]
        public int XbaLast7 { get; set; }

        [Ganss.Excel.Column("RCG")]
        public double RcgLast7 { get; set; }

        [Ganss.Excel.Column("RAR")]
        public double RarLast7 { get; set; }

        [Ganss.Excel.Column("BPV")]
        public int BpvLast7 { get; set; }


        // private int? _hqId;

        // [ForeignKey("SfbbPlayerBase")]
        // [Key]
        // public int? HQID_
        // {
        //     get => HQID;
        //     set => _hqId = value;
        // }

        // public virtual SfbbPlayerBase SfbbPlayerBase { get; set; }
    }


    /* --------------------------------------------------------- */
    /* YEAR TO DATE                                              */
    /* --------------------------------------------------------- */
    public sealed class HqHitterYearToDateClassMap : ClassMap<HqHitterYearToDate>
    {
        public HqHitterYearToDateClassMap()
        {
            Map( h => h.HQID                                ).Name("PlayerID");
            Map( h => h.MlbId                                     ).Name("MLBAM ID");
            Map( h => h.LastName                                  ).Name("Lastname");
            Map( h => h.FirstName                                 ).Name("Firstname");
            Map( h => h.Age                                       ).Name("Age");
            Map( h => h.Bats                                      ).Name("B");
            Map( h => h.Position                                  ).Name("Pos");
            Map( h => h.Team                                      ).Name("Tm");
            Map( h => h.CatcherAppearancesYTD)          .Index(8  ).Name("C");
            Map( h => h.FirstBaseAppearancesYTD)        .Index(9  ).Name("1B");
            Map( h => h.SecondBaseAppearancesYTD)       .Index(8  ).Name("2B");
            Map( h => h.ThirdBaseAppearancesYTD)        .Index(10 ).Name("3B");
            Map( h => h.ShortstopAppearancesYTD)        .Index(11 ).Name("SS");
            Map( h => h.OutfielderAppearancesYTD)       .Index(12 ).Name("OF");
            Map( h => h.DesignatedHitterAppearancesYTD) .Index(13 ).Name("DH");
            Map( h => h.AtBatsYTD                                 ).Name("AB");
            Map( h => h.RunsYTD                                   ).Name("R");
            Map( h => h.HitsYTD                                   ).Name("H");
            Map( h => h.DoublesYTD                                ).Name("2B");
            Map( h => h.TriplesYTD                                ).Name("3B");
            Map( h => h.HomeRunsYTD                               ).Name("HR");
            Map( h => h.RbiYTD                                    ).Name("RBI");
            Map( h => h.WalksYTD                                  ).Name("BB");
            Map( h => h.StrikeoutsYTD                             ).Name("K");
            Map( h => h.StolenBasesYTD                            ).Name("SB");
            Map( h => h.CaughtStealingYTD                         ).Name("CS");
            Map( h => h.BattingAverageYTD                         ).Name("AVG");
            Map( h => h.OnBasePercentageYTD                       ).Name("OBP");
            Map( h => h.SluggingPercentageYTD                     ).Name("SLG");
            Map( h => h.OnBasePlusSluggingYTD                     ).Name("OPS");
            Map( h => h.BbPercentageYTD                           ).Name("BB%");
            Map( h => h.ContactPercentageYTD                      ).Name("Ct%");
            Map( h => h.EyeYTD                                    ).Name("Eye");
            Map( h => h.HitPercentageYTD                          ).Name("H%");
            Map( h => h.PxYTD                                     ).Name("PX").NameIndex(0);
            Map( h => h.ExpectedPxYTD                             ).Name("xPX").NameIndex(0);
            Map( h => h.HctxYTD                                   ).Name("hctX").NameIndex(0);
            Map( h => h.SpeedYTD                                  ).Name("SPD");
            Map( h => h.RspdYTD                                   ).Name("RSPD");
            Map( h => h.GroundBallPercentageYTD                   ).Name("G%");
            Map( h => h.LineDrivePercentageYTD                    ).Name("L%");
            Map( h => h.FlyballPercentageYTD                      ).Name("F%");
            Map( h => h.XbaYTD                                    ).Name("XBA").NameIndex(0);
            Map( h => h.RcgYTD                                    ).Name("RCG").NameIndex(0);
            Map( h => h.RarYTD                                    ).Name("RAR").NameIndex(0);
            Map( h => h.BpvYTD                                    ).Name("BPV").NameIndex(0);
            Map( h => h.GroundballsYTD                            ).Name("GB");
            Map( h => h.GroundballOutsYTD                         ).Name("GBO");
            Map( h => h.LineDrivesYTD                             ).Name("LD");
            Map( h => h.LineDriveOutsYTD                          ).Name("LDO");
            Map( h => h.FlyballsYTD                               ).Name("FB");
            Map( h => h.FlyballOutsYTD                            ).Name("FBO");
            Map( h => h.PlateAppearancesVsRightHandersYTD         ).Name("PAvsRH");
            Map( h => h.PlateAppearancesVsLeftHandersYTD          ).Name("PAvsLH");
            Map( h => h.BattingAverageVsRightHandersYTD           ).Name("AVGvsRH");
            Map( h => h.BattingAverageVsLeftHandersYTD            ).Name("AVGvsLH");
            Map( h => h.OnBasePercentageVsRightHanders            ).Name("OBPvsRH");
            Map( h => h.OnBasePercentageVsLeftHanders             ).Name("OBPvsLH");
            Map( h => h.SluggingPercentageVsRightHanders          ).Name("SLGvsRH");
            Map( h => h.SluggingPercentageVsLeftHanders           ).Name("SLGvsLH");
            Map( h => h.OpsVsRightHandersYTD                      ).Name("OPSvsRH");
            Map( h => h.OpsVsLeftHandersYTD                       ).Name("OPSvsLH");
            Map( h => h.PxLast31                                  ).Name("PX").NameIndex(1);
            Map( h => h.ExpectedPxLast31                          ).Name("xPX").NameIndex(1);
            Map( h => h.HctxLast31                                ).Name("hctX").NameIndex(1);
            Map( h => h.XbaLast31                                 ).Name("XBA").NameIndex(1);
            Map( h => h.RcgLast31                                 ).Name("RCG").NameIndex(1);
            Map( h => h.RarLast31                                 ).Name("RAR").NameIndex(1);
            Map( h => h.BpvLast31                                 ).Name("BPV").NameIndex(1);
            Map( h => h.PxLast7                                   ).Name("PX").NameIndex(2);
            Map( h => h.ExpectedPxLast7                           ).Name("xPX").NameIndex(2);
            Map( h => h.HctxLast7                                 ).Name("hctX").NameIndex(2);
            Map( h => h.XbaLast7                                  ).Name("XBA").NameIndex(2);
            Map( h => h.RcgLast7                                  ).Name("RCG").NameIndex(2);
            Map( h => h.RarLast7                                  ).Name("RAR").NameIndex(2);
            Map( h => h.BpvLast7                                  ).Name("BPV").NameIndex(2);
        }
    }


    /* --------------------------------------------------------- */
    /* REST OF SEASON PROJECTION                                 */
    /* --------------------------------------------------------- */
    public partial class HqHitterRestOfSeasonProjection : BaseballHqReportHitterBase
    {
        public virtual SfbbPlayerBase SfbbPlayerBase { get; set; }

        [ForeignKey("SfbbPlayerBase")]
        public string IDPLAYER { get; set; }


        [Ganss.Excel.Column("MM Code")]
        public string MmCode { get; set; }

        [Ganss.Excel.Column("MM")]
        public int MM { get; set; }

        [Ganss.Excel.Column("DL")]
        public int DisabledListROS { get; set; }

        [Ganss.Excel.Column("AB")]
        public int AtBatsROS { get; set; }

        [Ganss.Excel.Column("R")]
        public int RunsROS { get; set; }

        [Ganss.Excel.Column("H")]
        public int HitsROS { get; set; }

        [Ganss.Excel.Column("2B")]
        public int DoublesROS { get; set; }

        [Ganss.Excel.Column("3B")]
        public int TriplesROS { get; set; }

        [Ganss.Excel.Column("HR")]
        public int HomeRunsROS { get; set; }

        [Ganss.Excel.Column("RBI")]
        public int RbiROS { get; set; }

        [Ganss.Excel.Column("BB")]
        public int WalksROS { get; set; }

        [Ganss.Excel.Column("K")]
        public int StrikeoutsROS { get; set; }

        [Ganss.Excel.Column("SB")]
        public int StolenBasesROS { get; set; }

        [Ganss.Excel.Column("CS")]
        public int CaughtStealingROS { get; set; }

        [Ganss.Excel.Column("AVG")]
        public int BattingAverageROS { get; set; }

        [Ganss.Excel.Column("OBP")]
        public int OnBasePercentageROS { get; set; }

        [Ganss.Excel.Column("SLG")]
        public int SluggingPercentageROS { get; set; }

        [Ganss.Excel.Column("OPS")]
        public int OnBasePlusSluggingROS { get; set; }

        [Ganss.Excel.Column("12$")]
        public int TwelveDollarValueROS { get; set; }

        [Ganss.Excel.Column("15$")]
        public int FifteenDollarValueROS { get; set; }

        [Ganss.Excel.Column("BB%")]
        public int BbPercentageROS { get; set; }

        [Ganss.Excel.Column("Ct%")]
        public int ContactPercentageROS { get; set; }

        [Ganss.Excel.Column("Eye")]
        public double EyeROS { get; set; }

        [Ganss.Excel.Column("H%")]
        public int HitPercentageROS { get; set; }

        [Ganss.Excel.Column("PX")]
        public int PxROS { get; set; }

        [Ganss.Excel.Column("SPD")]
        public int SpeedROS { get; set; }

        [Ganss.Excel.Column("RSPD")]
        public int RspdROS { get; set; }

        [Ganss.Excel.Column("G%")]
        public int GroundBallPercentageROS { get; set; }

        [Ganss.Excel.Column("L%")]
        public int LineDrivePercentageROS { get; set; }

        [Ganss.Excel.Column("F%")]
        public int FlyballPercentageROS { get; set; }

        [Ganss.Excel.Column("XBA")]
        public int XbaROS { get; set; }

        [Ganss.Excel.Column("BA")]
        public int BA_ROS { get; set; }

        [Ganss.Excel.Column("RCG")]
        public double RcgROS { get; set; }

        [Ganss.Excel.Column("RAR")]
        public double RarROS { get; set; }

        [Ganss.Excel.Column("BPV")]
        public int BpvROS { get; set; }
    }

    /* --------------------------------------------------------- */
    /* REST OF SEASON PROJECTION                                 */
    /* --------------------------------------------------------- */
    public sealed class HqHitterRestOfSeasonProjectionClassMap : ClassMap<HqHitterRestOfSeasonProjection>
    {
        public HqHitterRestOfSeasonProjectionClassMap()
        {
            Map( h => h.HQID              ).Name("PlayerID");
            Map( h => h.MlbId                   ).Name("MLBAM ID");
            Map( h => h.LastName                ).Name("Lastname");
            Map( h => h.FirstName               ).Name("Firstname");
            Map( h => h.Age                     ).Name("Age");
            Map( h => h.Bats                    ).Name("B");
            Map( h => h.Position                ).Name("Pos");
            Map( h => h.Team                    ).Name("Tm");
            Map( h => h.MmCode                  ).Name("MM Code");
            Map( h => h.MM                      ).Name("MM");
            Map( h => h.DisabledListROS         ).Name("DL");
            Map( h => h.AtBatsROS               ).Name("AB");
            Map( h => h.RunsROS                 ).Name("R");
            Map( h => h.HitsROS                 ).Name("H");
            Map( h => h.DoublesROS              ).Name("2B");
            Map( h => h.TriplesROS              ).Name("3B");
            Map( h => h.HomeRunsROS             ).Name("HR");
            Map( h => h.RbiROS                  ).Name("RBI");
            Map( h => h.WalksROS                ).Name("BB");
            Map( h => h.StrikeoutsROS           ).Name("K");
            Map( h => h.StolenBasesROS          ).Name("SB");
            Map( h => h.CaughtStealingROS       ).Name("CS");
            Map( h => h.BattingAverageROS       ).Name("AVG");
            Map( h => h.OnBasePercentageROS     ).Name("OBP");
            Map( h => h.SluggingPercentageROS   ).Name("SLG");
            Map( h => h.OnBasePlusSluggingROS   ).Name("OPS");
            Map( h => h.TwelveDollarValueROS    ).Name("12$");
            Map( h => h.FifteenDollarValueROS   ).Name("15$");
            Map( h => h.BbPercentageROS         ).Name("BB%");
            Map( h => h.ContactPercentageROS    ).Name("Ct%");
            Map( h => h.EyeROS                  ).Name("Eye");
            Map( h => h.HitPercentageROS        ).Name("H%");
            Map( h => h.PxROS                   ).Name("PX");
            Map( h => h.SpeedROS                ).Name("SPD");
            Map( h => h.RspdROS                 ).Name("RSPD");
            Map( h => h.GroundBallPercentageROS ).Name("G%");
            Map( h => h.LineDrivePercentageROS  ).Name("L%");
            Map( h => h.FlyballPercentageROS    ).Name("F%");
            Map( h => h.XbaROS                  ).Name("XBA");
            Map( h => h.RcgROS                  ).Name("RCG");
            Map( h => h.RarROS                  ).Name("RAR");
            Map( h => h.BpvROS                  ).Name("BPV");
        }
    }
}
