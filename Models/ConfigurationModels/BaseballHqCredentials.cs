namespace BaseballScraper.Models.ConfigurationModels
{
    public class BaseballHqCredentials : IBaseballHqCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string LoginLink { get; set; }
    }

    public interface IBaseballHqCredentials
    {
        string UserName { get; set; }
        string Password { get; set; }

        string LoginLink { get; set; }
    }
}
