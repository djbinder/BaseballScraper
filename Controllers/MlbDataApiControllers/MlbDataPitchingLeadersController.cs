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
    /// <summary> Provides two options on how to generate leading pitcher model
        /// <item> OPTION 1 - endpoint parameters defined within the method </item>
        /// <item> OPTION 2 - endpoint parameters passed as parameters to method </item>
    /// </summary>


    #pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
    [Route("api/mlb")]
    [ApiController]
    public class MlbDataPitchingLeadersController: Controller
    {
        private readonly Helpers _h           = new Helpers();
        private readonly ApiInfrastructure _a = new ApiInfrastructure();

        private static readonly MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();
        private static readonly PostmanMethods _postman        = new PostmanMethods();



        #region STEP 1 ------------------------------------------------------------

            // STATUS: this works
            // STEP 1
            /// <summary> OPTION 1: Get the current seasons pitching leaders; Endpoint parameters defined within method </summary>
            /// <remarks> Parameters for 'PitchingLeadersEndPoint' are defined within the method </remarks>
            /// <returns> A list of instantiated 'LeadingPitching' for 'numberToReturn' number of pitchers </returns>
            public PitchingLeaders CreatePitchingLeadersModel ()
            {
                // retrieve the 'PitchingLeaders' end point
                    // param 1: number of pitchers to include in search
                    // param 2: season that you want to query
                    // param 3: stat that you would like to sort by
                var newEndPoint = _endPoints.PitchingLeadersEndPoint(200, "2018", "era");

                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PitchingLeaders");
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

                var response = postmanResponse.Response;

                JObject leadersJObject = _a.CreateModelJObject(response);

                JToken leadersJToken = _a.CreateModelJToken(leadersJObject, "PitchingLeaders");

                // This will return an object that contains multiple 'LeadingPitcher'; The number returned depends on first parameter passed when retrieving the 'PitchingLeadersEndPoint' (see above)
                PitchingLeaders newPitchingLeadersInstance = new PitchingLeaders();

                _a.CreateInstanceOfModel(leadersJToken, newPitchingLeadersInstance, "PitchingLeaders");

                LeadingPitcher newLeadingPitcherInstance = new LeadingPitcher();
                _a.CreateMultipleInstancesOfModelByLooping(leadersJToken, newLeadingPitcherInstance, "LeadingPitcher");

                return newPitchingLeadersInstance;
            }


            // STATUS: this works
            // STEP 1
            /// <summary> OPTION 2: Get the current seasons pitching leaders; Endpoint parameters passed as parameters to method </summary>
            /// <remarks> Parameters for 'PitchingLeadersEndPoint' (i.e. numberToReturn, year, sortColumn) are passed as parameters to the method </remarks>
            ///     <param name="numberToReturn"> The number of pitchers to return in the results (e.g. 50 would show you the top 50 leaders) </param>
            ///     <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
            ///     <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., Era, Wins, etc) </param>
            ///         <see> View 'LeadingPitcher' model for options that you can sort by for this method
            /// <returns> A list of instantiated 'LeadingPitching' for 'numberToReturn' number of pitchers </returns>
            public PitchingLeaders CreatePitchingLeadersModel (int numberToReturn, string year, string sortColumn)
            {
                // retrieve the 'PitchingLeaders' end point
                    // param 1: number of pitchers to include in search
                    // param 2: season that you want to query
                    // param 3: stat that you would like to sort by
                var newEndPoint = _endPoints.PitchingLeadersEndPoint(numberToReturn, year, sortColumn);

                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "PitchingLeaders");
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

                var response = postmanResponse.Response;

                JObject leadersJObject = _a.CreateModelJObject(response);

                JToken leadersJToken = _a.CreateModelJToken(leadersJObject, "PitchingLeaders");

                // This will return an object that contains multiple 'LeadingPitcher'; The number returned depends on first parameter passed when retrieving the 'PitchingLeadersEndPoint' (see above)
                PitchingLeaders newPitchingLeadersInstance = new PitchingLeaders();

                _a.CreateInstanceOfModel(leadersJToken, newPitchingLeadersInstance, "PitchingLeaders");

                LeadingPitcher newLeadingPitcherInstance = new LeadingPitcher();
                _a.CreateMultipleInstancesOfModelByLooping(leadersJToken, newLeadingPitcherInstance, "LeadingPitcher");

                return newPitchingLeadersInstance;
            }

        #endregion STEP 1 ------------------------------------------------------------



        #region STEP 2 ------------------------------------------------------------

            // STATUS: this works
            // STEP 2
            /// <summary> OPTION 1: Get pitchin leaders w parameters </summary>
            /// <param name="numberToReturn"> OPTION 1: The number of pitchers to return in the results (e.g. 50 would show you the top 50 leaders) </param>
            /// <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
            /// <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., era, wins etc) </param>
            /// <see> View 'LeadingPitcher' model for options that you can sort by for this method </see>
            public async Task GetPitchingLeadersAsync(int numberToReturn, string year, string sortColumn)
            {
                await Task.Run(() => { CreatePitchingLeadersModel(numberToReturn, year, sortColumn); });
            }


            // STATUS: this works
            // STEP 2
            /// <summary> OPTION 2: Get pitchin leaders w filters defined within method </summary>
            public async Task GetPitchingLeadersAsync()
            {
                await Task.Run(() => { CreatePitchingLeadersModel(); });
            }

        #endregion STEP 2 ------------------------------------------------------------



        #region STEP 3 ------------------------------------------------------------

            // STATUS: this works
            // STEP 3
            /// <summary> OPTION 1: Initiate retrieval of mlb pitching leaders for current season </summary>
            /// <param name="numberToReturn"> The number of pitchers to return in the results (e.g. 50 would show you the top 50 leaders) </param>
            /// <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
            /// <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., era, wins etc) </param>
            /// <see> View 'LeadingPitcher' model for options that you can sort by for this method </see>
            /// <example> https://127.0.0.1:5001/api/mlb/pitchingleaders </example>
            /// <returns> Pitching leaders for current season </returns>
            [HttpGet]
            [Route("pitchingleaders/{year}")]
            public async Task<IActionResult> ViewPitchingLeadersAsync(int numberToReturn, string year, string sortColumn)
            {
                await GetPitchingLeadersAsync(5, "2018", "era");
                string currently = "retrieving Pitching leaders";
                return Content($"CURRENT TASK: {currently}");
            }


            // STATUS: this works
            // STEP 3
            /// <summary> OPTION 2: Initiate retrieval of mlb Pitching leaders for current season </summary>
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

        #endregion STEP 3 ------------------------------------------------------------



        #region HELPERS ------------------------------------------------------------

            public Dictionary<string,string>[] CreatePitchingLeadersDictionary(PitchingLeaders leaders)
            {
                _h.StartMethod();

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

                _h.Spotlight("begin writing pitching leaders");
                _a.ReturnJsonFromObject(pitchingLeaders);

                _h.CompleteMethod();

                return leadersDictionary;
            }

        #endregion HELPERS ------------------------------------------------------------


    }
}
