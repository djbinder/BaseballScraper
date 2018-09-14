// http://www.seanlahman.com/files/database/readme58.txt

using CsvHelper.Configuration;

namespace BaseballScraper.Models.Lahman
{
    public class LahmanParks
    {
        public string ParkKey { get; set; }
        public string ParkName { get; set; }
        public string ParkAlias { get; set; }
        public string ParkCity { get; set; }
        public string ParkState { get; set; }
        public string ParkCountry { get; set; }
    }

    public class LahmanParksClassMap: ClassMap<LahmanParks>
    {
        public LahmanParksClassMap()
        {
            Map(m => m.ParkKey).Name("park.key");
            Map(m => m.ParkName).Name("park.name");
            Map(m => m.ParkAlias).Name("park.alias");
            Map(m => m.ParkCity).Name("city");
            Map(m => m.ParkState).Name("state");
            Map(m => m.ParkCountry).Name("country");
        }
    }

}