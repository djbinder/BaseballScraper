// DJB WORKING ON
// DJB: this is correct

using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    public class YahooManagers
    {
        public YahooManager Manager { get; set; }
    }

    public class YahooManager : IBaseEntity
    {
        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface

        [Key]
        public int YahooManagerRecordId { get; set; }

        [XmlElement (ElementName = "manager_id")]
        public string ManagerId { get; set; }

        [XmlElement (ElementName = "nickname")]
        public string NickName { get; set; }

        [XmlElement (ElementName = "guid")]
        public string Guid { get; set; }

        [XmlElement (ElementName = "is_commissioner")]
        public string IsCommissioner { get; set; }

        [XmlElement (ElementName = "is_current_login")]
        public string IsCurrentLogin { get; set; }

        [XmlElement (ElementName = "email")]
        public string Email { get; set; }

        [XmlElement (ElementName = "image_url")]
        public string ImageUrl { get; set; }

    }
}
