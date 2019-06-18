// DJB WORKING ON
// DJB i think this is right

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    // /fantasy/v2/team/{team_key}/standings
    [XmlRoot (ElementName = "team_standings")]
    public class YahooTeamStanding
    {
        [Key]
        public int YahooTeamStandingRecordId { get; set; }

        [XmlElement (ElementName = "rank")]
        public string Rank { get; set; }

        [XmlElement (ElementName = "playoff_seed")]
        public string PlayoffSeed { get; set; }

        // includes wins, losses, ties, winning percentage
        [XmlElement (ElementName = "outcome_totals")]
        public YahooOutcomeTotals OutcomeTotals { get; set; }

        [XmlElement (ElementName = "games_back")]
        public string GamesBack { get; set; }

        public YahooTeamStanding()
        {
            OutcomeTotals = new YahooOutcomeTotals();
        }
    }
}