// // SFFB SOURCE --> https://www.smartfantasybaseball.com/tools/
// other source http://crunchtimebaseball.com/baseball_map.html

using System;

namespace BaseballScraper.Models
{
    public class PlayerBase
    {
        public int? MlbId { get; set; }
        public string MlbName { get; set; }
        public string MlbPosition { get; set; }
        public string MlbTeam { get; set; }
        public string MlbTeamLong { get; set; }
        public string Bats { get; set; }
        public string Throws { get; set; }
        public int? BirthYear { get; set; }
        public string SfbbPlayerId { get; set; }
        public string SfbbFullName { get; set; }
        public string SfbbFirstName { get; set; }
        public string SfbbLastName { get; set; }
        public string SfbbLeague { get; set; }
        public DateTime? SfbbBirthDate { get; set; }
        public int BaseballHqPlayerId { get; set; }
        public string FantasyProsName { get; set; }
        public string DavenportId { get; set; }
        public string RazzBallName { get; set; }
        public int? BaseballProspectusPlayerId { get; set; }
        public string BaseballReferencePlayerId { get; set; }
        public string BaseballReferenceName { get; set; }
        public int? CbsPlayerId { get; set; }
        public string CbsName { get; set; }
        public string CbsPosition { get; set; }
        public int? EspnPlayerId { get; set; }
        public string EspnName { get; set; }
        public string EspnPosition { get; set; }
        public string FanGraphsPlayerId { get; set; }
        public string FanGraphsName { get; set; }
        public string FanGraphsPosition { get; set; }
        public string LahmanPlayerId { get; set; }
        public string NfbcPlayerId { get; set; }
        public string NfbcName { get; set; }
        public string NfbcPosition { get; set; }
        public string RetroPlayerId { get; set; }
        public string RetroName { get; set; }
        public DateTime? MlbDebut { get; set; }
        public int? YahooPlayerId { get; set; }
        public string YahooName { get; set; }
        public string YahooPosition { get; set; }
        public string MlbDepth { get; set; }
        public string OttoneuPlayerId { get; set; }
        public string OttoneuName { get; set; }
        public string OttoneuPosition { get; set; }
        public string RotoWirePlayerId { get; set; }
        public string RotoWireName { get; set; }
        public string RotoWirePosition  { get; set; }


        public PlayerBase () {}

    }
}
