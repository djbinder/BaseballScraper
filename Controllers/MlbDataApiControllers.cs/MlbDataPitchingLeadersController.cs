using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Models.MlbDataApi;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.Controllers.MlbDataApiControllers.cs
{
    [Route("api/mlb")]
    [ApiController]
    public class MlbDataPitchingLeadersController: Controller
    {
        private Constants _c         = new Constants();
        private ApiInfrastructure _a = new ApiInfrastructure();

        private static MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();

        private static PostmanMethods _postman = new PostmanMethods();

        private static MlbDataApiHomeController _home = new MlbDataApiHomeController();


        // https://127.0.0.1:5001/api/mlb/leaders/pitchingleaders
        [HttpGet]
        [Route("pitchingleaders")]
        public async Task<IActionResult> ViewPitchingLeadersAsync()
        {
            await GetPitchingLeadersAsync();

            return Content($"Hello");
        }

        public async Task GetPitchingLeadersAsync()
        {
            await Task.Run(() => { CreatePitchingLeadersModel(); });
        }

        /// <summary> Get the current seasons pitching leaders </summary>
        /// <returns> A list of instantiated 'LeadingPitching' for x number of pitchers </returns>
        public PitchingLeaders CreatePitchingLeadersModel ()
        {
            _c.Start.ThisMethod();

            // retrieve the 'PitchingLeaders' end point
                // param 1: number of pitchers to include in search
                // param 2: season that you want to query
                // param 3: stat that you would like to sort by
            var newEndPoint = _endPoints.PitchingLeadersEndPoint(3, "2018", "era");
            // Console.WriteLine(newEndPoint);

            // Postman actions
                // type --> PostmanRequest
                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PitchingLeaders");
                // type --> PostmanResponse
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);
                // type --> IRestResponse
                var response = postmanResponse.Response;
                // type --> string
                var responseToJson = response.Content;

            JObject obj = CreatePitchingLeadersJObject(responseToJson);
            // Extensions.PrintJObjectItems(obj);

            JToken leadersToken = CreatePitchingLeadersJToken(obj);
            CreateLeadingPitcherModel(leadersToken);

            var responseToJsonString = responseToJson.ToString();
            // Console.WriteLine(responseToJsonString);

            // type = BaseballScraper.Models.MlbDataApi.PitchingLeaders
            var pitchingLeaders = PitchingLeaders.FromJson(responseToJson);

            _c.Complete.ThisMethod();
            return pitchingLeaders;
        }

        public JObject CreatePitchingLeadersJObject(string response)
        {
            _c.Start.ThisMethod();
            JObject obj = JObject.Parse(response);
            // Console.WriteLine($"OBJ: {obj}");
            return obj;
        }

        public JToken CreatePitchingLeadersJToken(JObject obj)
        {
            _c.Start.ThisMethod();
            JToken leadersToken = obj["leader_pitching_repeater"]["leader_pitching_mux"]["queryResults"]["row"];
            // Console.WriteLine($"TOKEN: {leadersToken}");
            return leadersToken;
        }

        public Dictionary<string,string>[] CreatePitchingLeadersDictionary(PitchingLeaders leaders)
        {
            _c.Start.ThisMethod();

            // System.Collections.Generic.Dictionary`2[System.String,System.String][]
            // The lengths of 'row' is equal to the first parameter passed when creating the endpoint
            var leadersDictionary = leaders.LeaderPitchingRepeater.LeaderPitchingMux.QueryResults.Row;

            // // R type = System.Collections.Generic.Dictionary`2[System.String,System.String]
            // // R.KEYS ---> System.Collections.Generic.Dictionary`2+KeyCollection[System.String,System.String]
            // // R.VALUES ---> System.Collections.Generic.Dictionary`2+ValueCollection[System.String,System.String]
            // int count = 1;
            // foreach (var r in leadersDictionary)
            // {
            //     Console.WriteLine($"Leader #: {count}");
            //     foreach (KeyValuePair<string, string> pair in r)
            //     {
            //         Console.WriteLine("{0}, {1}", pair.Key, pair.Value);
            //     }
            //     count++;
            // }
            string dictString = leadersDictionary.ToString();
            // Console.WriteLine(dictString);

            MemoryStream mS = new MemoryStream(Encoding.UTF8.GetBytes(dictString));

            PitchingLeaders pitchingLeaders = new PitchingLeaders();

            DataContractJsonSerializer pitchingLeadersJsonSerializer = new DataContractJsonSerializer(pitchingLeaders.GetType());

            // pitchingLeaders = pitchingLeadersJsonSerializer.ReadObject(mS) as PitchingLeaders;
            mS.Close();

            Extensions.Spotlight("begin writing pitching leaders");
            _a.ReturnJsonFromObject(pitchingLeaders);

            _c.Complete.ThisMethod();

            return leadersDictionary;
        }

        public LeadingPitcher CreateLeadingPitcherModel(JToken leadersToken)
        {
            _c.Start.ThisMethod();

            LeadingPitcher lP = new LeadingPitcher();

            foreach(var leaderObject in leadersToken)
            {
                // Console.WriteLine(leaderObject.GetType());
                // Console.WriteLine("check");

                string leaderObjectToString = leaderObject.ToString();

                MemoryStream mS = new MemoryStream(Encoding.UTF8.GetBytes(leaderObjectToString));

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(lP.GetType());

                lP = serializer.ReadObject(mS) as LeadingPitcher;

                mS.Close();

                Extensions.Spotlight("begin writing from object");
                _a.ReturnJsonFromObject(lP);
            }

            return lP;
        }
    }
}