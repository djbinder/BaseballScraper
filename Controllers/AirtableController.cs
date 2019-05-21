//REFERENCE:
    // https://gist.githubusercontent.com/deanebarker/2b4520f290ece96be40436bc5c8915c5/raw/0cf6005f41ac27c46c9ce1f9bdbf8b5faeb62f8d/AirtableGetObjects.cs

using System;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;



namespace BaseballScraper.Controllers
{
    #pragma warning disable CS0414, CS0219, CS1591
    [Route("api/airtable")]
    [ApiController]
    public class AirtableController: Controller
    {
        private readonly Helpers _h = new Helpers();
        private readonly AirtableConfiguration _airtableConfig;

        public AirtableController(IOptions<AirtableConfiguration> airtableConfig)
        {
            _airtableConfig = airtableConfig.Value;
        }


        /// <summary> Retrieves managers listed in league manager database </summary>
        /// <example> https://127.0.0.1:5001/api/airtable/managers </example>
        /// <returns> ManagerFullName, ManagerFirstName, ManagerListName, TeamIds, etc.</returns>
        [HttpGet]
        [Route("managers")]
        public JObject GetAirtableManagers ()
        {
            _h.StartMethod();
            // string airTableKey = GetAirtableKey();
            string airTableKey = _airtableConfig.ApiKey;
            Console.WriteLine($"AIR TABLE KEY IS: {airTableKey}");

            string tableName = "TgManagers";

            var client = new RestClient($"https://api.airtable.com/v0/appeokc0jzuDMQ31H/{tableName}?api_key={airTableKey}");

            var request = new RestRequest(Method.GET);
            request.AddHeader("Postman-Token", "af978745-112b-40d2-b760-a86945ce4095");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Authorization", "Bearer aXJUKynsTUXLVY");

            IRestResponse response = client.Execute(request);
            _h.Intro(response, "response");

            var     initialJson    = response.Content;
            JObject structuredJson = JObject.Parse(initialJson);
            _h.Intro(structuredJson, "structured json");

            _h.CompleteMethod();
            return structuredJson;

        }
    }
}
