// // SFFB SOURCE --> https://www.smartfantasybaseball.com/tools/
// other source http://crunchtimebaseball.com/baseball_map.html
// For Ganss.Excel (i.e., [Column(names)] ) --> https://github.com/mganss/ExcelMapper

using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration;
using Ganss.Excel;

namespace BaseballScraper.Models.Player
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


    // this references SfbbPlayerIdMap in BaseballScraper/Configuration/gSheetNames.json
    public class SfbbPlayerBase
    {
        [Key]
        [Column("IDPLAYER")]
        public string IDPLAYER  { get; set; }

        [Column("PLAYERNAME")]
        public string PLAYERNAME  { get; set; }

        [Column("BIRTHDATE")]
        public string BIRTHDATE  { get; set; }

        [Column("FIRSTNAME")]
        public string FIRSTNAME  { get; set; }

        [Column("LASTNAME")]
        public string LASTNAME  { get; set; }

        [Column("TEAM")]
        public string TEAM  { get; set; }

        [Column("LG")]
        public string LG  { get; set; }

        [Column("POS")]
        public string POS  { get; set; }

        [Column("IDFANGRAPHS")]
        public string IDFANGRAPHS  { get; set; }

        [Column("FANGRAPHSNAME")]
        public string FANGRAPHSNAME  { get; set; }

        [Column("MLBID")]
        public string MLBID  { get; set; }

        [Column("MLBNAME")]
        public string MLBNAME  { get; set; }

        [Column("CBSID")]
        public string CBSID  { get; set; }

        [Column("CBSNAME")]
        public string CBSNAME  { get; set; }

        [Column("RETROID")]
        public string RETROID  { get; set; }

        [Column("BREFID")]
        public string BREFID  { get; set; }

        [Column("NFBCID")]
        public string NFBCID  { get; set; }

        [Column("NFBCNAME")]
        public string NFBCNAME  { get; set; }

        [Column("ESPNID")]
        public string ESPNID  { get; set; }

        [Column("ESPNNAME")]
        public string ESPNNAME  { get; set; }

        [Column("KFFLNAME")]
        public string KFFLNAME  { get; set; }

        [Column("DAVENPORTID")]
        public string DAVENPORTID  { get; set; }

        [Column("BPID")]
        public string BPID  { get; set; }

        [Column("YAHOOID")]
        public string YAHOOID  { get; set; }

        [Column("YAHOONAME")]
        public string YAHOONAME  { get; set; }

        [Column("MSTRBLLNAME")]
        public string MSTRBLLNAME  { get; set; }

        [Column("BATS")]
        public string BATS  { get; set; }

        [Column("THROWS")]
        public string THROWS  { get; set; }

        [Column("FANPROSNAME")]
        public string FANPROSNAME  { get; set; }

        [Column("LASTCOMMAFIRST")]
        public string LASTCOMMAFIRST  { get; set; }

        [Column("ROTOWIREID")]
        public string ROTOWIREID  { get; set; }

        [Column("FANDUELNAME")]
        public string FANDUELNAME  { get; set; }

        [Column("FANDUELID")]
        public string FANDUELID  { get; set; }

        [Column("DRAFTKINGSNAME")]
        public string DRAFTKINGSNAME  { get; set; }

        [Column("OTTONEUID")]
        public string OTTONEUID  { get; set; }

        [Column("HQID")]
        public string HQID  { get; set; }

        [Column("RAZZBALLNAME")]
        public string RAZZBALLNAME  { get; set; }

        [Column("FANTRAXID")]
        public string FANTRAXID  { get; set; }

        [Column("FANTRAXNAME")]
        public string FANTRAXNAME  { get; set; }

        [Column("ROTOWIRENAME")]
        public string ROTOWIRENAME  { get; set; }

        [Column("ALLPOS")]
        public string ALLPOS  { get; set; }

        [Column("NFBCLASTFIRST")]
        public string NFBCLASTFIRST  { get; set; }
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
