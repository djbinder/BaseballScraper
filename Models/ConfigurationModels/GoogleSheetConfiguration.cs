using System;
using System.Runtime.Serialization;

namespace BaseballScraper.Models.ConfigurationModels
{
    [DataContract]
    public class GoogleSheetConfiguration : IGoogleSheetConfiguration
    {
        [DataMember(Name="DocumentName")]
        public string DocumentName { get; set; }


        [DataMember(Name="Link")]
        public Uri Link { get; set; }


        [DataMember(Name="SpreadsheetId")]
        public string SpreadsheetId { get; set; }


        [DataMember(Name="Range")]
        public string Range { get; set; }


        [DataMember(Name="TabName")]
        public string TabName { get; set; }


        [DataMember(Name="GId")]
        public string GId { get; set; }


        [DataMember(Name="WorkbookName")]
        public string WorkbookName { get; set; }
    }


    public interface IGoogleSheetConfiguration
    {
        string DocumentName { get; set; }

        Uri Link { get; set; }

        string SpreadsheetId { get; set; }

        string Range { get; set; }

        string TabName { get; set; }

        string GId { get; set; }

        string WorkbookName { get; set; }
    }
}
