using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System;

namespace BaseballScraper.Models.Yahoo
{

    [XmlRoot (ElementName = "team_logos")]
    public class YahooTeamLogos : IBaseEntity
    {
        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface
        public YahooTeamLogo TeamLogo { get; set; }
    }

    [XmlRoot (ElementName = "team_logo")]
    public class YahooTeamLogo
    {
        [Key]
        public int YahooTeamLogoRecordId { get; set; }
        public int TeamLogoId { get; set; }

        [XmlElement (ElementName = "size")]
        public string Size { get; set; }

        [XmlElement (ElementName = "url")]
        public string Url { get; set; }

    }
}
