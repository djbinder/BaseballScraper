// // SFFB SOURCE --> https://www.smartfantasybaseball.com/tools/
// other source http://crunchtimebaseball.com/baseball_map.html
// For Ganss.Excel (i.e., [Ganss.Excel.Column(names)] ) --> https://github.com/mganss/ExcelMapper

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BaseballScraper.Models.BaseballSavant;
using CsvHelper.Configuration;
using Ganss.Excel;

namespace BaseballScraper.Models.Player
{
    public class PlayerBase
    {
        [Ganss.Excel.Column("mlb_id")]
        public string MlbId { get; set; }

        [Ganss.Excel.Column("mlb_name")]
        public string MlbName { get; set; }

        [Ganss.Excel.Column("mlb_pos")]
        public string MlbPosition { get; set; }

        [Ganss.Excel.Column("mlb_team")]
        public string MlbTeam { get; set; }

        [Ganss.Excel.Column("mlb_team_long")]
        public string MlbTeamLong { get; set; }

        [Ganss.Excel.Column("bats")]
        public string Bats { get; set; }

        [Ganss.Excel.Column("throws")]
        public string Throws { get; set; }

        [Ganss.Excel.Column("birth_year")]
        public int? BirthYear { get; set; }

        [Ganss.Excel.Column("sfbb_id")]
        public string SfbbPlayerId { get; set; }

        [Ganss.Excel.Column("sfbb_fullname")]
        public string SfbbFullName { get; set; }

        [Ganss.Excel.Column("sfbb_firstname")]
        public string SfbbFirstName { get; set; }

        [Ganss.Excel.Column("sfbb_lastname")]
        public string SfbbLastName { get; set; }

        [Ganss.Excel.Column("sfbb_league")]
        public string SfbbLeague { get; set; }

        [Ganss.Excel.Column("sfbb_birthdate")]
        public int? SfbbBirthDate { get; set; }

        [Ganss.Excel.Column("baseballhq_id")]
        public string BaseballHqPlayerId { get; set; }

        [Ganss.Excel.Column("fantasypros_name")]
        public string FantasyProsName { get; set; }

        [Ganss.Excel.Column("davenport_id")]
        public string DavenportId { get; set; }

        [Ganss.Excel.Column("razzball_name")]
        public string RazzBallName { get; set; }

        [Ganss.Excel.Column("bp_id")]
        public string BaseballProspectusPlayerId { get; set; }

        [Ganss.Excel.Column("bref_id")]
        public string BaseballReferencePlayerId { get; set; }

        [Ganss.Excel.Column("bref_name")]
        public string BaseballReferenceName { get; set; }

        [Ganss.Excel.Column("cbs_id")]
        public string CbsPlayerId { get; set; }

        [Ganss.Excel.Column("cbs_name")]
        public string CbsName { get; set; }

        [Ganss.Excel.Column("cbs_pos")]
        public string CbsPosition { get; set; }

        [Ganss.Excel.Column("espn_id")]
        public string EspnPlayerId { get; set; }

        [Ganss.Excel.Column("espn_name")]
        public string EspnName { get; set; }

        [Ganss.Excel.Column("espn_pos")]
        public string EspnPosition { get; set; }

        [Ganss.Excel.Column("fg_id")]
        public string FanGraphsPlayerId { get; set; }

        [Ganss.Excel.Column("fg_name")]
        public string FanGraphsName { get; set; }

        [Ganss.Excel.Column("fg_pos")]
        public string FanGraphsPosition { get; set; }

        [Ganss.Excel.Column("lahman_id")]
        public string LahmanPlayerId { get; set; }

        [Ganss.Excel.Column("nfbc_id")]
        public string NfbcPlayerId { get; set; }

        [Ganss.Excel.Column("nfbc_name")]
        public string NfbcName { get; set; }

        [Ganss.Excel.Column("nfbc_pos")]
        public string NfbcPosition { get; set; }

        [Ganss.Excel.Column("retro_id")]
        public string RetroPlayerId { get; set; }

        [Ganss.Excel.Column("retro_name")]
        public string RetroName { get; set; }

        [Ganss.Excel.Column("debut")]
        public string MlbDebut { get; set; }

        [Ganss.Excel.Column("yahoo_id")]
        public string YahooPlayerId { get; set; }

        [Ganss.Excel.Column("yahoo_name")]
        public string YahooName { get; set; }

        [Ganss.Excel.Column("yahoo_pos")]
        public string YahooPosition { get; set; }

        [Ganss.Excel.Column("mlb_depth")]
        public string MlbDepth { get; set; }

        [Ganss.Excel.Column("ottoneu_id")]
        public string OttoneuPlayerId { get; set; }

        [Ganss.Excel.Column("ottoneu_name")]
        public string OttoneuName { get; set; }

        [Ganss.Excel.Column("ottoneu_pos")]
        public string OttoneuPosition { get; set; }

        [Ganss.Excel.Column("rotowire_id")]
        public string RotoWirePlayerId { get; set; }

        [Ganss.Excel.Column("rotowire_name")]
        public string RotoWireName { get; set; }

        [Ganss.Excel.Column("rotowire_pos")]
        public string RotoWirePosition  { get; set; }


        public PlayerBase () {}

    }


    // this references SfbbPlayerIdMap in BaseballScraper/Configuration/gSheetNames.json
    public class SfbbPlayerBase
    {
        // [ForeignKey("PlayerId")]
        public virtual BaseballSavantHitter BaseballSavantHitter { get; set; }

        private int? _baseballSavantHitterForeignKey;


        // [ForeignKey("BaseballSavantHitter")]
        [Ganss.Excel.Column("MLBID")]
        public int? MLBID  { get; set; }

        [ForeignKey("BaseballSavantHitter")]
        public int? BaseballSavantHitterForeignKey
        {
            get
            {
                if(MLBID.ToString() == "")
                {
                    return null;
                }
                else
                {
                    return Int32.Parse(MLBID.ToString());
                }
            }
            // set => _baseballSavantHitterForeignKey = value;
        }



        [Key]
        [Ganss.Excel.Column("IDPLAYER")]
        public string IDPLAYER  { get; set; }

        [Ganss.Excel.Column("PLAYERNAME")]
        public string PLAYERNAME  { get; set; }

        [Ganss.Excel.Column("BIRTHDATE")]
        public string BIRTHDATE  { get; set; }

        [Ganss.Excel.Column("FIRSTNAME")]
        public string FIRSTNAME  { get; set; }

        [Ganss.Excel.Column("LASTNAME")]
        public string LASTNAME  { get; set; }

        [Ganss.Excel.Column("TEAM")]
        public string TEAM  { get; set; }

        [Ganss.Excel.Column("LG")]
        public string LG  { get; set; }

        [Ganss.Excel.Column("POS")]
        public string POS  { get; set; }

        [Ganss.Excel.Column("IDFANGRAPHS")]
        public string IDFANGRAPHS  { get; set; }

        [Ganss.Excel.Column("FANGRAPHSNAME")]
        public string FANGRAPHSNAME  { get; set; }




        [Ganss.Excel.Column("MLBNAME")]
        public string MLBNAME  { get; set; }

        [Ganss.Excel.Column("CBSID")]
        public string CBSID  { get; set; }

        [Ganss.Excel.Column("CBSNAME")]
        public string CBSNAME  { get; set; }

        [Ganss.Excel.Column("RETROID")]
        public string RETROID  { get; set; }

        [Ganss.Excel.Column("BREFID")]
        public string BREFID  { get; set; }

        [Ganss.Excel.Column("NFBCID")]
        public string NFBCID  { get; set; }

        [Ganss.Excel.Column("NFBCNAME")]
        public string NFBCNAME  { get; set; }

        [Ganss.Excel.Column("ESPNID")]
        public string ESPNID  { get; set; }

        [Ganss.Excel.Column("ESPNNAME")]
        public string ESPNNAME  { get; set; }

        [Ganss.Excel.Column("KFFLNAME")]
        public string KFFLNAME  { get; set; }

        [Ganss.Excel.Column("DAVENPORTID")]
        public string DAVENPORTID  { get; set; }

        [Ganss.Excel.Column("BPID")]
        public string BPID  { get; set; }

        [Ganss.Excel.Column("YAHOOID")]
        public string YAHOOID  { get; set; }

        [Ganss.Excel.Column("YAHOONAME")]
        public string YAHOONAME  { get; set; }

        [Ganss.Excel.Column("MSTRBLLNAME")]
        public string MSTRBLLNAME  { get; set; }

        [Ganss.Excel.Column("BATS")]
        public string BATS  { get; set; }

        [Ganss.Excel.Column("THROWS")]
        public string THROWS  { get; set; }

        [Ganss.Excel.Column("FANPROSNAME")]
        public string FANPROSNAME  { get; set; }

        [Ganss.Excel.Column("LASTCOMMAFIRST")]
        public string LASTCOMMAFIRST  { get; set; }

        [Ganss.Excel.Column("ROTOWIREID")]
        public string ROTOWIREID  { get; set; }

        [Ganss.Excel.Column("FANDUELNAME")]
        public string FANDUELNAME  { get; set; }

        [Ganss.Excel.Column("FANDUELID")]
        public string FANDUELID  { get; set; }

        [Ganss.Excel.Column("DRAFTKINGSNAME")]
        public string DRAFTKINGSNAME  { get; set; }

        [Ganss.Excel.Column("OTTONEUID")]
        public string OTTONEUID  { get; set; }

        [Ganss.Excel.Column("HQID")]
        public string HQID  { get; set; }

        [Ganss.Excel.Column("RAZZBALLNAME")]
        public string RAZZBALLNAME  { get; set; }

        [Ganss.Excel.Column("FANTRAXID")]
        public string FANTRAXID  { get; set; }

        [Ganss.Excel.Column("FANTRAXNAME")]
        public string FANTRAXNAME  { get; set; }

        [Ganss.Excel.Column("ROTOWIRENAME")]
        public string ROTOWIRENAME  { get; set; }

        [Ganss.Excel.Column("ALLPOS")]
        public string ALLPOS  { get; set; }

        [Ganss.Excel.Column("NFBCLASTFIRST")]
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
