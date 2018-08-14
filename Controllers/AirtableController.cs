// https://gist.githubusercontent.com/deanebarker/2b4520f290ece96be40436bc5c8915c5/raw/0cf6005f41ac27c46c9ce1f9bdbf8b5faeb62f8d/AirtableGetObjects.cs

using System;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BaseballScraper.Controllers
{

    public class AirtableController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";

        private readonly TwitterConfiguration _twitterConfig;
        private readonly AirtableConfiguration _airtableConfig;
        private readonly YahooConfiguration _yahooConfig;


        public AirtableController(IOptions<AirtableConfiguration> airtableConfig, IOptions<TwitterConfiguration> twitterConfig, IOptions<YahooConfiguration> yahooConfig)
        {
            _airtableConfig = airtableConfig.Value;
            _twitterConfig  = twitterConfig.Value;
            _yahooConfig    = yahooConfig.Value;
        }



        // [Authorize]
        [HttpGet]
        [Route("/airtablekey")]
        public string GetAirtableKey()
        {
            Start.ThisMethod();
            var airtableApiKey = _airtableConfig.ApiKey;

            return airtableApiKey;
        }



        // THIS WORKS
        [HttpGet]
        [Route("/managers")]
        public JObject GetAirtableManagers ()
        {
            Start.ThisMethod();
            string airTableKey = GetAirtableKey();
            airTableKey.Intro("air table key");

            var client = new RestClient($"https://api.airtable.com/v0/appeokc0jzuDMQ31H/YahooManager?api_key={airTableKey}");

            var request = new RestRequest(Method.GET);
            request.AddHeader("Postman-Token", "af978745-112b-40d2-b760-a86945ce4095");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", "Bearer aXJUKynsTUXLVY");

            IRestResponse response = client.Execute(request);
            response.Intro("response");

            var     initialJson    = response.Content;
            JObject structuredJson = JObject.Parse(initialJson);
            structuredJson.Intro("structured json");

            Complete.ThisMethod();
            return structuredJson;

        }
    }
}