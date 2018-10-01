// http://www.seanlahman.com/files/database/readme58.txt

using CsvHelper.Configuration;

namespace BaseballScraper.Models.Lahman
{
    public class LahmanPeople
    {
        public string LahmanPlayerId { get; set; }
        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }
        public string BirthCountry { get; set; }
        public string BirthState { get; set; }
        public string BirthCity { get; set; }
        public int? DeathYear { get; set; }
        public int? DeathMonth { get; set; }
        public int? DeathDay { get; set; }
        public string DeathCountry { get; set; }
        public string DeathState { get; set; }
        public string DeathCity { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NameFirstLast { get; set; }
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public string Bats { get; set; }
        public string Throws { get; set; }
        public string Debut { get; set; }
        public string FinalGame { get; set; }
        public string RetroPlayerId { get; set; }
        public string BaseballReferencePlayerId { get; set; }
    }


    public sealed class LahmanPeopleClassMap: ClassMap<LahmanPeople>
    {
        public LahmanPeopleClassMap()
        {
            Map(m => m.LahmanPlayerId).Name("playerID");
            Map(m => m.BirthYear).Name("birthYear");
            Map(m => m.BirthMonth).Name("birthMonth");
            Map(m => m.BirthDay).Name("birthDay");
            Map(m => m.BirthCountry).Name("birthCountry");
            Map(m => m.BirthState).Name("birthState");
            Map(m => m.BirthCity).Name("birthCity");
            Map(m => m.DeathYear).Name("deathYear");
            Map(m => m.DeathMonth).Name("deathMonth");
            Map(m => m.DeathDay).Name("deathDay");
            Map(m => m.DeathCountry).Name("deathCountry");
            Map(m => m.DeathState).Name("deathState");
            Map(m => m.DeathCity).Name("deathCity");
            Map(m => m.FirstName).Name("nameFirst");
            Map(m => m.LastName).Name("nameLast");
            Map(m => m.NameFirstLast).Name("nameGiven");
            Map(m => m.Weight).Name("weight");
            Map(m => m.Height).Name("height");
            Map(m => m.Bats).Name("bats");
            Map(m => m.Throws).Name("throws");
            Map(m => m.Debut).Name("debut");
            Map(m => m.FinalGame).Name("finalGame");
            Map(m => m.RetroPlayerId).Name("retroID");
            Map(m => m.BaseballReferencePlayerId).Name("bbrefID");
        }
    }
}