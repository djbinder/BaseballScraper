namespace BaseballScraper.Models.Lahman
{
    public class LahmanPlayerInfo : BaseEntity
    {
        public string LahmanId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
