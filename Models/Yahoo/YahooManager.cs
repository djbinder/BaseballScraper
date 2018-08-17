// DJB WORKING ON

using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    public class YahooManager
    {
        public int TeamManagersId { get; set; }

        [XmlElement (ElementName = "manager_id")]
        public string ManagerId { get; set; }

        [XmlElement (ElementName = "nickname")]
        public string NickName { get; set; }

        [XmlElement (ElementName = "guid")]
        public string Guid { get; set; }

        [XmlElement (ElementName = "is_commissioner")]
        public int? IsCommissioner { get; set; }

        [XmlElement (ElementName = "is_current_login")]
        public int? IsCurrentLogin { get; set; }

        [XmlElement (ElementName = "email")]
        public string Email { get; set; }

        [XmlElement (ElementName = "image_url")]
        public string ImageUrl { get; set; }

    }
}