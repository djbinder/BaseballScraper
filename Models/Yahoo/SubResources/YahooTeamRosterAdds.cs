using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System;

namespace BaseballScraper.Models.Yahoo
{
    [XmlRoot (ElementName = "roster_adds")]
    public class YahooTeamRosterAdds : IBaseEntity
    {
        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface


        [Key]
        public int YahooTeamRosterAddsRecordId { get; set; }

        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "coverage_value")]
        public string CoverageValue { get; set; }

        [XmlElement (ElementName = "value")]
        public string Value { get; set; }
    }
}
