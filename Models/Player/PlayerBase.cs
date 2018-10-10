// // SFFB SOURCE --> https://www.smartfantasybaseball.com/tools/
// other source http://crunchtimebaseball.com/baseball_map.html
// For Ganss.Excel (i.e., [Column(names)] ) --> https://github.com/mganss/ExcelMapper

using System;
using CsvHelper.Configuration;
using Ganss.Excel;

namespace BaseballScraper.Models
{
    public class PlayerBase
    {
        [Column("mlb_id")]
        public string MlbId { get; set; }

        [Column("mlb_name")]
        public string MlbName { get; set; }

        [Column("mlb_pos")]
        public string MlbPosition { get; set; }

        [Column("mlb_team")]
        public string MlbTeam { get; set; }

        [Column("mlb_team_long")]
        public string MlbTeamLong { get; set; }

        [Column("bats")]
        public string Bats { get; set; }

        [Column("throws")]
        public string Throws { get; set; }

        [Column("birth_year")]
        public int? BirthYear { get; set; }

        [Column("sfbb_id")]
        public string SfbbPlayerId { get; set; }

        [Column("sfbb_fullname")]
        public string SfbbFullName { get; set; }

        [Column("sfbb_firstname")]
        public string SfbbFirstName { get; set; }

        [Column("sfbb_lastname")]
        public string SfbbLastName { get; set; }

        [Column("sfbb_league")]
        public string SfbbLeague { get; set; }

        [Column("sfbb_birthdate")]
        public int? SfbbBirthDate { get; set; }

        [Column("baseballhq_id")]
        public string BaseballHqPlayerId { get; set; }

        [Column("fantasypros_name")]
        public string FantasyProsName { get; set; }

        [Column("davenport_id")]
        public string DavenportId { get; set; }

        [Column("razzball_name")]
        public string RazzBallName { get; set; }

        [Column("bp_id")]
        public string BaseballProspectusPlayerId { get; set; }

        [Column("bref_id")]
        public string BaseballReferencePlayerId { get; set; }

        [Column("bref_name")]
        public string BaseballReferenceName { get; set; }

        [Column("cbs_id")]
        public string CbsPlayerId { get; set; }

        [Column("cbs_name")]
        public string CbsName { get; set; }

        [Column("cbs_pos")]
        public string CbsPosition { get; set; }

        [Column("espn_id")]
        public string EspnPlayerId { get; set; }

        [Column("espn_name")]
        public string EspnName { get; set; }

        [Column("espn_pos")]
        public string EspnPosition { get; set; }

        [Column("fg_id")]
        public string FanGraphsPlayerId { get; set; }

        [Column("fg_name")]
        public string FanGraphsName { get; set; }

        [Column("fg_pos")]
        public string FanGraphsPosition { get; set; }

        [Column("lahman_id")]
        public string LahmanPlayerId { get; set; }

        [Column("nfbc_id")]
        public string NfbcPlayerId { get; set; }

        [Column("nfbc_name")]
        public string NfbcName { get; set; }

        [Column("nfbc_pos")]
        public string NfbcPosition { get; set; }

        [Column("retro_id")]
        public string RetroPlayerId { get; set; }

        [Column("retro_name")]
        public string RetroName { get; set; }

        [Column("debut")]
        public string MlbDebut { get; set; }

        [Column("yahoo_id")]
        public string YahooPlayerId { get; set; }

        [Column("yahoo_name")]
        public string YahooName { get; set; }

        [Column("yahoo_pos")]
        public string YahooPosition { get; set; }

        [Column("mlb_depth")]
        public string MlbDepth { get; set; }

        [Column("ottoneu_id")]
        public string OttoneuPlayerId { get; set; }

        [Column("ottoneu_name")]
        public string OttoneuName { get; set; }

        [Column("ottoneu_pos")]
        public string OttoneuPosition { get; set; }

        [Column("rotowire_id")]
        public string RotoWirePlayerId { get; set; }

        [Column("rotowire_name")]
        public string RotoWireName { get; set; }

        [Column("rotowire_pos")]
        public string RotoWirePosition  { get; set; }


        public PlayerBase () {}

    }


    // public class PlayerBaseClassMap: ClassMap<PlayerBase>
    // {
    //     public PlayerBaseClassMap()
    //     {
    //         Map(m => m.MlbId).Name("mlb_id");
    //         Map(m => m.MlbPosition).Name("mlb_pos");
    //     }
    // }
}
