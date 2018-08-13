// // SFFB SOURCE --> https://www.smartfantasybaseball.com/tools/
// other source http://crunchtimebaseball.com/baseball_map.html

using System;

namespace BaseballScraper.Models
{
    public abstract class Player
    {
        public int? DJBid { get; set; }


        // from sfbb spreadsheet
        public string SfbbPlayerId { get; set; }
        public string SfbbFullName { get; set; }
        public string SfbbFirstName { get; set; }
        public string SfbbLastName { get; set; }

        public string SfbbLastCommaFirst
        {
            get { return string.Concat(SfbbLastName, ", ", SfbbFirstName );}
        }
        public string SfbbLeague { get; set; }
        public DateTime? SfbbBirthDate { get; set; }


        // these are from crunchtimebaseball xlsx
        public string Team { get; set; }
        public string Position { get; set; }
        public DateTime? MlbDebut { get; set; }
        public string Bats { get; set; }
        public string Throws { get; set; }
        public string MlbDepth { get; set; }
        public int? YahooId { get; set; }
        public string YahooName { get; set; }
        public string YahooPosition { get; set; }
        public string LahmanId { get; set; }
        public string FanGraphsId { get; set; }
        public string FanGraphsName { get; set; }
        public string FanGraphsPosition { get; set; }
        public int? MlbId { get; set; }
        public string MlbName { get; set; }
        public string MlbPosition { get; set; }
        public string MlbTeam { get; set; }
        public string MlbTeamLong { get; set; }
        public int? CbsId { get; set; }
        public string CbsName { get; set; }
        public string CbsPosition { get; set; }
        public string RetroId { get; set; }
        public string RetroName { get; set; }
        public string BaseballReferenceId { get; set; }
        public string BaseballReferenceName { get; set; }
        public string NfbcId { get; set; }
        public string NfbcName { get; set; }
        public string NfbcPosition { get; set; }
        public int? EspnId { get; set; }
        public string EspnName { get; set; }
        public string EspnPosition { get; set; }
        public string OttoneuId { get; set; }
        public string OttoneuName { get; set; }
        public string OttoneuPosition { get; set; }
        public string RotoWireId { get; set; }
        public string RotoWireName { get; set; }
        public string RotoWirePosition  { get; set; }
        public int? BaseballProspectusId { get; set; }


        // from sfbb
        public int BaseballHqId { get; set; }
        public string FantasyProsName { get; set; }
        public string DavenportId { get; set; }
        public string RazzBallName { get; set; }




    }
}
