using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    public class YahooDraft
    {
    // /fantasy/v2/team/{team_key}/draftresults
    public class YahooTeamDraftResult
    {
        [XmlElement (ElementName = "@count")]
        public int? NumberOfPlayersDrafted { get; set; }

        public IList<YahooTeamDraftPick> DraftPicks { get; set; }

        public YahooTeamDraftResult()
        {
            DraftPicks = new List<YahooTeamDraftPick>();
        }
    }

    public class YahooTeamDraftPick
    {
        [XmlElement (ElementName = "pick")]
        public string PickNumber { get; set; }

        [XmlElement (ElementName = "round")]
        public string RoundNumber { get; set; }

        [XmlElement (ElementName = "team_key")]
        public string DraftingTeamsKey { get; set; }

        [XmlElement (ElementName = "player_key")]
        public string PlayerKey { get; set; }
    }
    }
}