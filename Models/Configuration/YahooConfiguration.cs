// https://stackoverflow.com/questions/47294020/reading-appsettings-from-asp-net-core-webapi

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BaseballScraper.Models.Configuration
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


        public int? ExpiresIn { get; set; } = 3600;
        public string TokenType { get; set; } = "bearer";
        public string RequestUriBase { get; set; }
        public string RequestAuthUri { get; set; }
        public string GetTokenBase { get; set; }

        // public string ResponseType { get; set; }
        // public string Language { get; set; }
        // public Guid RequestId { get; set; }

    }

    public class YahooAuthParameters
    {
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string ResponseType { get; set; }
        public string Language { get; set; }
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


    public class Signature
    {
        public string Nonce { get; set; }
        public string TimeStamp { get; set; }
        public string FullSignature { get; set; }
    }


    public class GetTokenResponse
    {
        public string FormHtml { get; set; }

        [DataMember(Name = "TokenSecret")]
        public string TokenSecret { get; set; }

        [DataMember(Name = "Duration")]
        public string Duration { get; set; }

        [DataMember(Name = "AuthenticationUrl")]
        public string AuthenticationUrl { get; set; }
    }

    public class GetTokenRequest
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonProperty("response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        public Guid RequestId { get; set;  }
    }
}
