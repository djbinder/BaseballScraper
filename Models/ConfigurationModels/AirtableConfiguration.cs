namespace BaseballScraper.Models.ConfigurationModels
{
    public class AirtableConfiguration
    {
        public string ApiKey               { get; set; }
        public string TableName            { get; set; }
        public string Base                 { get; set; }
        public string Link                 { get; set; }

        public string AuthenticationString { get; set; }
        public string PostmanToken         { get; set; }
    }
}
