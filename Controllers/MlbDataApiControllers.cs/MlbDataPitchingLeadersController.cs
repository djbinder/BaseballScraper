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


        /// <summary>
        // FILE NOTE: provides two options on how to generate leading hitter model
            // OPTION 1 - endpoint parameters defined within the method
            // OPTION 2 - endpoint parameters passed as parameters to method
        /// </summary>


        // OPTION 1 : Step 1 - endpoint parameters defined within method
        /// <summary> Get the current seasons pitching leaders </summary>
        /// <remarks> Parameters for 'PitchingLeadersEndPoint' are defined within the method </remarks>
        /// <returns> A list of instantiated 'LeadingPitching' for 'numberToReturn' number of pitchers </returns>
        public PitchingLeaders CreatePitchingLeadersModel ()
        {
            // _c.Start.ThisMethod();

            // retrieve the 'PitchingLeaders' end point
                // param 1: number of pitchers to include in search
                // param 2: season that you want to query
                // param 3: stat that you would like to sort by
            var newEndPoint = _endPoints.PitchingLeadersEndPoint(200, "2018", "era");
            // Console.WriteLine(newEndPoint);

            // Postman actions
                // type --> PostmanRequest
                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PitchingLeaders");
                // type --> PostmanResponse
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);
                // type --> IRestResponse
                var response = postmanResponse.Response;
                // type --> string
                // var responseToJson = response.Content;

            JObject leadersJObject = _a.CreateModelJObject(response);
            // Extensions.PrintJObjectItems(leadersJObject);

            JToken leadersJToken = _a.CreateModelJToken(leadersJObject, "PitchingLeaders");

            // This will return an object that contains multiple 'LeadingPitcher'; The number returned depends on first parameter passed when retrieving the 'PitchingLeadersEndPoint' (see above)
            PitchingLeaders newPitchingLeadersInstance = new PitchingLeaders();

            _a.CreateInstanceOfModel(leadersJToken, newPitchingLeadersInstance, "PitchingLeaders");

            LeadingPitcher newLeadingPitcherInstance = new LeadingPitcher();
            _a.CreateMultipleInstancesOfModelByLooping(leadersJToken, newLeadingPitcherInstance, "LeadingPitcher");

            // _c.Complete.ThisMethod();
            return newPitchingLeadersInstance;
        }

        // OPTION 1 : Step 2
        public async Task GetPitchingLeadersAsync()
        {
            await Task.Run(() => { CreatePitchingLeadersModel(); });
        }

        // OPTION 1 : Step 3
        /// <summary> Initiate retrieval of mlb Pitching leaders for current season </summary>
        /// <example> https://127.0.0.1:5001/api/mlb/pitchingleaders </example>
        /// <returns> Pitching leaders for current season </returns>
        [HttpGet]
        [Route("pitchingleaders")]
        public async Task<IActionResult> ViewPitchingLeadersAsync()
        {
            await GetPitchingLeadersAsync();

            string currently = "retrieving pitching leaders";

            return Content($"CURRENT TASK: {currently}");
        }



        // OPTION 2 - endpoint parameters passed as parameters to method
        /// <summary> Get the current seasons pitching leaders </summary>
        /// <remarks> Parameters for 'PitchingLeadersEndPoint' (i.e. numberToReturn, year, sortColumn) are passed as parameters to the method </remarks>
        ///     <param name="numberToReturn"> The number of pitchers to return in the results (e.g. 50 would show you the top 50 leaders) </param>
        ///     <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
        ///     <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., Era, Wins, etc) </param>
        ///         <see> View 'LeadingPitcher' model for options that you can sort by for this method
        /// <returns> A list of instantiated 'LeadingPitching' for 'numberToReturn' number of pitchers </returns>
        public PitchingLeaders CreatePitchingLeadersModel (int numberToReturn, string year, string sortColumn)
        {
            // _c.Start.ThisMethod();

            // retrieve the 'PitchingLeaders' end point
                // param 1: number of pitchers to include in search
                // param 2: season that you want to query
                // param 3: stat that you would like to sort by
            var newEndPoint = _endPoints.PitchingLeadersEndPoint(numberToReturn, year, sortColumn);
            // Console.WriteLine(newEndPoint);

            // Postman actions
                // type --> PostmanRequest
                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PitchingLeaders");
                // type --> PostmanResponse
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);
                // type --> IRestResponse
                var response = postmanResponse.Response;
                // type --> string
                // var responseToJson = response.Content;

            JObject leadersJObject = _a.CreateModelJObject(response);
            // Extensions.PrintJObjectItems(leadersJObject);

            JToken leadersJToken = _a.CreateModelJToken(leadersJObject, "PitchingLeaders");

            // This will return an object that contains multiple 'LeadingPitcher'; The number returned depends on first parameter passed when retrieving the 'PitchingLeadersEndPoint' (see above)
            PitchingLeaders newPitchingLeadersInstance = new PitchingLeaders();

            _a.CreateInstanceOfModel(leadersJToken, newPitchingLeadersInstance, "PitchingLeaders");

            LeadingPitcher newLeadingPitcherInstance = new LeadingPitcher();
            _a.CreateMultipleInstancesOfModelByLooping(leadersJToken, newLeadingPitcherInstance, "LeadingPitcher");

            // _c.Complete.ThisMethod();
            return newPitchingLeadersInstance;
        }



        // OPTION 2 : Step 2
        /// <param name="numberToReturn"> The number of pitchers to return in the results (e.g. 50 would show you the top 50 leaders) </param>
        /// <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
        /// <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., era, wins etc) </param>
        ///         <see> View 'LeadingPitcher' model for options that you can sort by for this method </see>
        public async Task GetPitchingLeadersAsync(int numberToReturn, string year, string sortColumn)
        {
            await Task.Run(() => { CreatePitchingLeadersModel(numberToReturn, year, sortColumn); });
        }


        // OPTION 2: Step 3
        /// <summary> Initiate retrieval of mlb pitching leaders for current season </summary>
        /// <param name="numberToReturn"> The number of pitchers to return in the results (e.g. 50 would show you the top 50 leaders) </param>
        /// <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
        /// <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., era, wins etc) </param>
        ///         <see> View 'LeadingPitcher' model for options that you can sort by for this method </see>
        /// <example> https://127.0.0.1:5001/api/mlb/pitchingleaders </example>
        /// <returns> Pitching leaders for current season </returns>
        [HttpGet]
        [Route("pitchingleaders/{year}")]
        public async Task<IActionResult> ViewHittingLeadersAsync(int numberToReturn, string year, string sortColumn)
        {
            // OPTION 2 --> three parameters needed to call the method
            await GetPitchingLeadersAsync(5, "2018", "era");

            string currently = "retrieving Pitching leaders";

            return Content($"CURRENT TASK: {currently}");
        }



        public Dictionary<string,string>[] CreatePitchingLeadersDictionary(PitchingLeaders leaders)
        {
            _c.Start.ThisMethod();

            // System.Collections.Generic.Dictionary`2[System.String,System.String][]
            // The lengths of 'row' is equal to the first parameter passed when creating the endpoint
            var leadersDictionary = leaders.LeaderPitchingRepeater.LeaderPitchingMux.QueryResults.Row;

            // R type = System.Collections.Generic.Dictionary`2[System.String,System.String]
            // R.KEYS ---> System.Collections.Generic.Dictionary`2+KeyCollection[System.String,System.String]
            // R.VALUES ---> System.Collections.Generic.Dictionary`2+ValueCollection[System.String,System.String]
            int count = 1;
            foreach (var r in leadersDictionary)
            {
                Console.WriteLine($"Leader #: {count}");
                foreach (KeyValuePair<string, string> pair in r)
                {
                    Console.WriteLine("{0}, {1}", pair.Key, pair.Value);
                }
                count++;
            }
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
    }
}