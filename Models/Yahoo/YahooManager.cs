using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    [XmlRoot (ElementName = "managers", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class YahooManager
    {

        public string ManagerId { get; set; }

        public string ManagerFullName { get; set; }

        public string ManagerFirstName { get; set; }

        public string ManagerLastName { get; set; }

        public string ManagerEmail { get; set; }

        public string Fullname
        {
            get { return string.Concat(ManagerFirstName, " ", ManagerLastName); }
        }
    }
}




