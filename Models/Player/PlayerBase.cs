// For Ganss.Excel (i.e., [Ganss.Excel.Column(names)] ) --> https://github.com/mganss/ExcelMapper

using System.ComponentModel.DataAnnotations;
using BaseballScraper.Models.BaseballSavant;
using BaseballScraper.Models.BaseballHq;
using CsvHelper.Configuration;
using System;

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



    // * CrunchTime Map : http://crunchtimebaseball.com/baseball_map.html
    // * CrunchTime CSV : http://crunchtimebaseball.com/master.csv
    public class CrunchTimePlayerBase : IBaseEntity
    {
        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface

        [Key]
        public int MlbId           { get; set; }
        public string MlbName      { get; set; }
        public string MlbPos       { get; set; }
        public string MlbTeam      { get; set; }
        public string MlbTeamLong  { get; set; }
        public string Bats         { get; set; }
        public string Throws       { get; set; }
        public int BirthYear       { get; set; }
        public int BpId            { get; set; }
        public string BrefId       { get; set; }
        public string BrefName     { get; set; }
        public int? CbsId          { get; set; }
        public string CbsName      { get; set; }
        public string CbsPos       { get; set; }
        public int? EspnId         { get; set; }
        public string EspnName     { get; set; }
        public string EspnPos      { get; set; }
        public string FgId         { get; set; }
        public string FgName       { get; set; }
        public string FgPos        { get; set; }
        public string LahmanId     { get; set; }
        public int? NfbcId         { get; set; }
        public string NfbcName     { get; set; }
        public string NfbcPos      { get; set; }
        public string RetroId      { get; set; }
        public string RetroName    { get; set; }
        public int Debut           { get; set; }
        public int? YahooId        { get; set; }
        public string YahooName    { get; set; }
        public int? OttoneuId      { get; set; }
        public string OttoneuName  { get; set; }
        public string OttoneuPos   { get; set; }
        public int? RotowireId     { get; set; }
        public string RotowireName { get; set; }
        public string RotowirePos  { get; set; }
    }

    public class CrunchTimePlayerBaseClassMap : ClassMap<CrunchTimePlayerBase>
    {
        public CrunchTimePlayerBaseClassMap()
        {
            Map(m => m.MlbId).Name("mlb_id");
            Map(m => m.MlbName).Name("mlb_name");
            Map(m => m.MlbPos).Name("mlb_pos");
            Map(m => m.MlbTeam).Name("mlb_team");
            Map(m => m.MlbTeamLong).Name("mlb_team_long");
            Map(m => m.Bats).Name("bats");
            Map(m => m.Throws).Name("throws");
            Map(m => m.BirthYear).Name("birth_year");
            Map(m => m.BpId).Name("bp_id");
            Map(m => m.BrefId).Name("bref_id");
            Map(m => m.BrefName).Name("bref_name");
            Map(m => m.CbsId).Name("cbs_id");
            Map(m => m.CbsName).Name("cbs_name");
            Map(m => m.CbsPos).Name("cbs_pos");
            Map(m => m.EspnId).Name("espn_id");
            Map(m => m.EspnName).Name("espn_name");
            Map(m => m.EspnPos).Name("espn_pos");
            Map(m => m.FgId).Name("fg_id");
            Map(m => m.FgName).Name("fg_name");
            Map(m => m.FgPos).Name("fg_pos");
            Map(m => m.LahmanId).Name("lahman_id");
            Map(m => m.NfbcId).Name("nfbc_id");
            Map(m => m.NfbcName).Name("nfbc_name");
            Map(m => m.NfbcPos).Name("nfbc_pos");
            Map(m => m.RetroId).Name("retro_id");
            Map(m => m.RetroName).Name("retro_name");
            Map(m => m.Debut).Name("debut");
            Map(m => m.YahooId).Name("yahoo_id");
            Map(m => m.YahooName).Name("yahoo_name");
            Map(m => m.OttoneuId).Name("ottoneu_id");
            Map(m => m.OttoneuName).Name("ottoneu_name");
            Map(m => m.OttoneuPos).Name("ottoneu_pos");
            Map(m => m.RotowireId).Name("rotowire_id");
            Map(m => m.RotowireName).Name("rotowire_name");
            Map(m => m.RotowirePos).Name("rotowire_pos");
        }
    }


    // this references SfbbPlayerIdMap in BaseballScraper/Configuration/gSheetNames.json
    // * SFBB Player Id Map : http://bit.ly/2UdNAGy
    public class SfbbPlayerBase : IBaseEntity
    {
        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface



        public virtual XstatsHitter XstatsHitter { get; set; }
        public virtual ExitVelocityAndBarrelsHitter ExitVelocityAndBarrelsHitter { get; set; }
        public virtual HqHitterRestOfSeasonProjection HqHitterRestOfSeasonProjection { get; set; }
        public virtual HqHitterYearToDate HqHitterYearToDate { get; set; }


        public int? MLBID  { get; set; }
        public int? HQID { get; set; }



        // [Ganss.Excel.Column("IDPLAYER")]
        [Key]
        public string IDPLAYER                    { get; set; }

        public string PLAYERNAME                  { get; set; }

        public string BIRTHDATE                   { get; set; }

        public string FIRSTNAME                   { get; set; }

        public string LASTNAME                    { get; set; }

        public string TEAM                        { get; set; }

        public string LG                          { get; set; }

        public string POS                         { get; set; }

        public string IDFANGRAPHS                 { get; set; }

        public string FANGRAPHSNAME               { get; set; }

        public string MLBNAME                     { get; set; }

        public string CBSID                       { get; set; }

        public string CBSNAME                     { get; set; }

        public string RETROID                     { get; set; }

        public string BREFID                      { get; set; }

        public string NFBCID                      { get; set; }

        public string NFBCNAME                    { get; set; }

        public string ESPNID                      { get; set; }

        public string ESPNNAME                    { get; set; }

        public string KFFLNAME                    { get; set; }

        public string DAVENPORTID                 { get; set; }

        public string BPID                        { get; set; }

        public string YAHOOID                     { get; set; }

        public string YAHOONAME                   { get; set; }

        public string MSTRBLLNAME                 { get; set; }

        public string BATS                        { get; set; }

        public string THROWS                      { get; set; }

        public string FANTPROSNAME                { get; set; }

        public string LASTCOMMAFIRST              { get; set; }

        public string ROTOWIREID                  { get; set; }

        public string FANDUELNAME                 { get; set; }

        public string FANDUELID                   { get; set; }

        public string DRAFTKINGSNAME              { get; set; }

        public string OTTONEUID                   { get; set; }

        public string RAZZBALLNAME                { get; set; }

        public string FANTRAXID                   { get; set; }

        public string FANTRAXNAME                 { get; set; }

        public string ROTOWIRENAME                { get; set; }

        public string ALLPOS                      { get; set; }

        public string NFBCLASTFIRST               { get; set; }

    }

    public sealed class SfbbPlayerBaseClassMap : ClassMap<SfbbPlayerBase>
    {
        public SfbbPlayerBaseClassMap()
        {
            Map(m => m.IDPLAYER).Name("IDPLAYER");
            Map(m => m.PLAYERNAME).Name("PLAYERNAME");
            Map(m => m.BIRTHDATE).Name("BIRTHDATE");
            Map(m => m.FIRSTNAME).Name("FIRSTNAME");
            Map(m => m.LASTNAME).Name("LASTNAME");
            Map(m => m.TEAM).Name("TEAM");
            Map(m => m.LG).Name("LG");
            Map(m => m.POS).Name("POS");
            Map(m => m.IDFANGRAPHS).Name("IDFANGRAPHS");
            Map(m => m.MLBID).Name("MLBID");
            Map(m => m.MLBNAME).Name("MLBNAME");
            Map(m => m.CBSID).Name("CBSID");
            Map(m => m.CBSNAME).Name("CBSNAME");
            Map(m => m.RETROID).Name("RETROID");
            Map(m => m.BREFID).Name("BREFID");
            Map(m => m.NFBCID).Name("NFBCID");
            Map(m => m.NFBCNAME).Name("NFBCNAME");
            Map(m => m.ESPNID).Name("ESPNID");
            Map(m => m.ESPNNAME).Name("ESPNNAME");
            Map(m => m.KFFLNAME).Name("KFFLNAME");
            Map(m => m.DAVENPORTID).Name("DAVENPORTID");
            Map(m => m.BPID).Name("BPID");
            Map(m => m.YAHOOID).Name("YAHOOID");
            Map(m => m.YAHOONAME).Name("YAHOONAME");
            Map(m => m.MSTRBLLNAME).Name("MSTRBLLNAME");
            Map(m => m.BATS).Name("BATS");
            Map(m => m.THROWS).Name("THROWS");
            Map(m => m.FANTPROSNAME).Name("FANTPROSNAME");
            Map(m => m.LASTCOMMAFIRST).Name("LASTCOMMAFIRST");
            Map(m => m.ROTOWIREID).Name("ROTOWIREID");
            Map(m => m.FANDUELNAME).Name("FANDUELNAME");
            Map(m => m.FANDUELID).Name("FANDUELID");
            Map(m => m.DRAFTKINGSNAME).Name("DRAFTKINGSNAME");
            Map(m => m.OTTONEUID).Name("OTTONEUID");
            Map(m => m.HQID).Name("HQID");
            Map(m => m.RAZZBALLNAME).Name("RAZZBALLNAME");
            Map(m => m.FANTRAXID).Name("FANTRAXID");
            Map(m => m.FANTRAXNAME).Name("FANTRAXNAME");
            Map(m => m.ROTOWIRENAME).Name("ROTOWIRENAME");
            Map(m => m.ALLPOS).Name("ALLPOS");
            Map(m => m.NFBCLASTFIRST).Name("NFBCLASTFIRST");
        }
    }
}
