// https://stackoverflow.com/questions/47294020/reading-appsettings-from-asp-net-core-webapi

using System.Runtime.Serialization;

namespace BaseballScraper.Models.ConfigurationModels
{
    [DataContract]
    public class YahooConfiguration
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember(Name="AppID")]
        public string AppId { get; set; }

        [DataMember]
        public string ClientId { get; set; }

        [DataMember]
        public string ClientSecret { get; set; }

        [DataMember]
        public string Base64Encoding { get; set; }

        [DataMember]
        public string ClientPublic { get; set; }

        [DataMember]
        public string RedirectUri { get; set; }

        [DataMember]
        public string RefreshToken { get; set; }

        [DataMember]
        public string XOAuthYahooGuid { get; set; }


        [DataMember]
        public int? ExpiresIn { get; set; } = 3600;

        public string RequestAuthUri { get; set; }
    }


    public class AccessTokenResponse
    {
        // The access token that you can use to make calls for Yahoo user data. The access token has a 1-hour lifetime.
        public string AccessToken { get; set; }

        // The access token that you can use to make calls for Yahoo user data.
        public string TokenType { get; set; }

        // The access token lifetime in seconds.
        public string ExpiresIn { get; set; }

        // The refresh token that you can use to acquire a new access token after the current one expires.
        public string RefreshToken { get; set; }

        // The GUID of the Yahoo user.
        public string XOAuthYahooGuid { get; set; }
    }
}
