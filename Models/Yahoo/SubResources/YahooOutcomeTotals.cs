using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    // this is correct
    [XmlRoot (ElementName = "outcome_totals")]
    public class YahooOutcomeTotals
    {
        [Key]
        public int YahooOutcomeRecordId { get; set; }

        [XmlElement (ElementName = "wins")]
        public string Wins { get; set; }

        [XmlElement (ElementName = "losses")]
        public string Losses { get; set; }

        [XmlElement (ElementName = "ties")]
        public string Ties { get; set; }

        [XmlElement (ElementName = "percentage")]
        public string Percentage { get; set; }
    }
}