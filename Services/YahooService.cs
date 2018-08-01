using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using YahooFantasyWrapper;
using YahooFantasyWrapper.Client;

using YahooFantasyWrapper.Configuration;

using YahooFantasyWrapper.Infrastructure;

using YahooFantasyWrapper.Models;

using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using BaseballScraper.Models;
using BaseballScraper.Models.Yahoo;
using Newtonsoft.Json.Converters;

namespace BaseballScraper.Services
{
    public class YahooService
    {
        // private readonly IYahooAuthClient _client;
        // private readonly IYahooFantasyClient _fantasy;


        // private static string requestUrl = "https://api.login.yahoo.com/oauth/v2/get_request_token";

        // private static string userAuthorizeUrl = "https://api.login.yahoo.com/oauth/v2/request_auth";

        // private static string accessUrl = "https://api.login.yahoo.com/oauth/v2/get_token";

        // private static string authenticateUrl = "https://api.login.yahoo.com/oauth/v2/request_auth?oauth_token=";

        // private static string consumerKey = "[dj0yJmk9blAzdzNJU3lBRktkJmQ9WVdrOVl6WnVNVzFDTXpBbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD05OQ--";

        // private static string clientId = "dj0yJmk9blAzdzNJU3lBRktkJmQ9WVdrOVl6WnVNVzFDTXpBbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD05OQ--";

        // private static string consumer_key = "dj0yJmk9blAzdzNJU3lBRktkJmQ9WVdrOVl6WnVNVzFDTXpBbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD05OQ--";

        // private static string clientSecret = "1be55f24f0c7ae6d87238a7a8f5fc99d73051203";

        // private static string consumer_secret = "1be55f24f0c7ae6d87238a7a8f5fc99d73051203";

        // private static string base_Url = "http://fantasysports.yahooapis.com/";

        public YahooService()
        {

        }
        // public YahooService(IYahooAuthClient client, IYahooFantasyClient fantasyClient)
        // {
        //     _client  = client;
        //     _fantasy = fantasyClient;
        // }

        // public static string GetUserDataFromYahoo(string requestEndPoint, string token, string tokenSecret)
        // {
        //     var data = String.Empty;
        //     var uri  = new Uri(requestEndPoint);
        //     string url, param;
        //     var oAuth     = new OAuthBase();
        //     var nonce     = oAuth.GenerateNonce();
        //     var timeStamp = oAuth.GenerateTimeStamp();
        //     var signature = oAuth.GenerateSignature(
        //         uri,
        //         consumer_key,
        //         consumer_secret,
        //         token,
        //         tokenSecret,
        //         "GET",
        //         timeStamp,
        //         nonce,
        //         OAuthBase.SignatureTypes.HMACSHA1,
        //         out url,
        //         out param);
        //                             data                 = String.Format("{0}?{1}&oauth_signature={2}", url, param, signature);
        //                         var requestParametersUrl = String.Format("{0}?{1}&oauth_signature={2}", url, param, signature);
        //                         var request              = WebRequest.Create(requestParametersUrl);
        //     using (var response = request.GetResponse())
        //     using (Stream dataStream = response.GetResponseStream())
        //     using (StreamReader reader = new StreamReader(dataStream))
        //     {
        //         data = reader.ReadToEnd();
        //     }
        //     return data;
        // }


        // public static void SetX ()
        // {

        //     var request =
        //         new RestRequest("game/{gameType}/players;start={start};count={count}", Method.GET);
        //     request.AddUrlSegment("gameType", _gameType);
        //     request.AddUrlSegment("start", start.ToString());
        //     request.AddUrlSegment("count", count.ToString());
        //     request.AddJsonParam();

        //     var response    = _client.Execute(request);
        //     var json        = JObject.Parse(response.Content);
        //     var playersJson = json["fantasy_content"]["game"][1]["players"];

        //     // Remove the count element
        //     playersJson.Last.Remove();

        //     var players = JsonConvert.DeserializeObject<Dictionary<string, Player>>(
        //         playersJson.ToString(), new JsonPlayerConverter());

        // }


    }


    public class JsonPlayerConverter: CustomCreationConverter<BaseballScraper.Models.Player>
    {
        public override BaseballScraper.Models.Player Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public BaseballScraper.Models.Player Create(Type objectType, JObject obj)
        {
            var array   = obj["player"][0];
            var content = array.Children<JObject>();

            var player = new BaseballScraper.Models.Player();
            foreach (var prop in player.GetType().GetProperties())
            {
                var attr        = prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false).FirstOrDefault();
                var propName    = ((JsonPropertyAttribute)attr).PropertyName;
                var jsonElement = content.FirstOrDefault(c => c.Properties()
                                        .Any(p => p.Name == propName));
                var value = jsonElement.GetValue(propName);
                var type  = prop.PropertyType;

                if (type == typeof(string))
                {
                    prop.SetValue(player, (string)value, null);
                }
                else if (type  == typeof(PlayerName))
                {
                    var playerName = JsonConvert.DeserializeObject<PlayerName>(value.ToString());
                    prop.SetValue(player, (PlayerName)playerName, null);
                }
                else if (type == typeof(BaseballScraper.Models.Yahoo.Headshot))
                {
                    var headshot = JsonConvert.DeserializeObject<BaseballScraper.Models.Yahoo.Headshot>(value.ToString());
                    prop.SetValue(player, headshot, null);
                }
                else if (type == typeof(BaseballScraper.Models.Yahoo.ByeWeeks))
                {
                    var byeWeeks = JsonConvert.DeserializeObject<BaseballScraper.Models.Yahoo.ByeWeeks>(value.ToString());
                    prop.SetValue(player, byeWeeks, null);
                }
                else if (type == typeof(List<EligiblePosition>))
                {
                    var eligiblePositions = JsonConvert.DeserializeObject<List<EligiblePosition>>(value.ToString());
                    prop.SetValue(player, eligiblePositions, null);
                }
            }
            return player;
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var obj    = JObject.Load(reader);
            var target = Create(objectType, obj);
            serializer.Populate(obj.CreateReader(), target);

            return target;
        }
    }
}




