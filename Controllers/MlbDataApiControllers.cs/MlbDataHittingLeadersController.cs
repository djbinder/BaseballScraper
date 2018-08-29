using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.MlbDataApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BaseballScraper.Controllers.MlbDataApiControllers
{
    [Route("api/mlb")]
    [ApiController]
    public class MlbDataHittingLeadersController: Controller
    {
        private Constants _c                          = new Constants();
        private ApiInfrastructure _a                  = new ApiInfrastructure();
        private static MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();
        private static PostmanMethods _postman        = new PostmanMethods();

        /// <summary>
        // FILE NOTE: provides two options on how to generate leading hitter model
            // OPTION 1 - endpoint parameters defined within the method
            // OPTION 2 - endpoint parameters passed as parameters to method
        /// </summary>


        // OPTION 1: step 1 - endpoint parameters defined within the method
        /// <summary> Get the current seasons hitting leaders </summary>
        /// <remarks> Parameters for 'HittingLeadersEndPoint' are defined within the method </remarks>
        /// <returns> A list of instantiated 'LeadingHitter' for 'numberToReturn' number of hitters </returns>
        public HittingLeaders CreateHittingLeadersModel ()
        {
            _c.Start.ThisMethod();

            // retrieve the 'HittingLeaders' end point
                // param 1: number of hitters to include in search
                // param 2: season that you want to query
                // param 3: stat that you would like to sort by
            var newEndPoint = _endPoints.HittingLeadersEndPoint(5, "2018", "hr");
            // Console.WriteLine(newEndPoint);

            // Postman actions
                // type --> PostmanRequest
                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "HittingLeaders");
                // type --> PostmanResponse
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);
                // type --> IRestResponse
                IRestResponse response = postmanResponse.Response;
                // type --> string
                // var responseToJson = response.Content;

            JObject leadersJObject = _a.CreateModelJObject(response);
            // Extensions.PrintJObjectItems(leadersJObject);

            JToken leadersJToken = _a.CreateModelJToken(leadersJObject, "HittingLeaders");

            // This will return an object that contains multiple 'LeadingHitter'; The number returned depends on first parameter passed when retrieving the 'HittingLeadersEndPoint' (see above)
            HittingLeaders newHittingLeadersInstance = new HittingLeaders();

            _a.CreateInstanceOfModel(leadersJToken, newHittingLeadersInstance, "HittingLeaders");

            LeadingHitter newLeadingHitterInstance = new LeadingHitter();
            _a.CreateMultipleInstancesOfModelByLooping(leadersJToken, newLeadingHitterInstance, "LeadingHitter");

            // _c.Complete.ThisMethod();
            return newHittingLeadersInstance;
        }

        // OPTION 1: Step 2
        public async Task GetHittingLeadersAsync()
        {
            await Task.Run(() => { CreateHittingLeadersModel(); });
        }


        // OPTION 1 : Step 3
        /// <summary> Initiate retrieval of mlb hitting leaders for current season </summary>
        /// <example> https://127.0.0.1:5001/api/mlb/hittingleaders </example>
        /// <returns> Hitting leaders for current season </returns>
        [HttpGet]
        [Route("hittingleaders")]
        public async Task<IActionResult> ViewHittingLeadersAsync()
        {
            // OPTION 1 --> no parameters are provided to the method; search criteria is defined within the Step 1
            await GetHittingLeadersAsync();

            string currently = "retrieving hitting leaders";

            return Content($"CURRENT TASK: {currently}");
        }


        // OPTION 2 : Step 1 - endpoint parameters passed as parameters to method
        /// <summary> Get the current seasons hitting leaders </summary>
        /// <remarks> Parameters for 'HittingLeadersEndPoint' (i.e. numberToReturn, year, sortColumn) are passed as parameters to the method </remarks>
        ///     <param name="numberToReturn"> The number of hitters to return in the results (e.g. 50 would show you the top 50 leaders) </param>
        ///     <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
        ///     <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., Hr, Rbi etc) </param>
        ///         <see> View 'LeadingHitter' model for options that you can sort by for this method </see>
        /// <returns> A list of instantiated 'LeadingHitter' for 'numberToReturn' number of hitters </returns>
        public HittingLeaders CreateHittingLeadersModel (int numberToReturn, string year, string sortColumn)
        {
            _c.Start.ThisMethod();

            // retrieve the 'HittingLeaders' end point
                // param 1: number of hitters to include in search
                // param 2: season that you want to query
                // param 3: stat that you would like to sort by
            var newEndPoint = _endPoints.HittingLeadersEndPoint(5, "2018", "hr");
            // Console.WriteLine(newEndPoint);

            // Postman actions
                // type --> PostmanRequest
                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "HittingLeaders");
                // type --> PostmanResponse
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);
                // type --> IRestResponse
                IRestResponse response = postmanResponse.Response;
                // type --> string
                // var responseToJson = response.Content;

            JObject leadersJObject = _a.CreateModelJObject(response);
            // Extensions.PrintJObjectItems(leadersJObject);

            JToken leadersJToken = _a.CreateModelJToken(leadersJObject, "HittingLeaders");

            // This will return an object that contains multiple 'LeadingHitter'; The number returned depends on first parameter passed when retrieving the 'HittingLeadersEndPoint' (see above)
            HittingLeaders newHittingLeadersInstance = new HittingLeaders();

            _a.CreateInstanceOfModel(leadersJToken, newHittingLeadersInstance, "HittingLeaders");

            LeadingHitter newLeadingHitterInstance = new LeadingHitter();
            _a.CreateMultipleInstancesOfModelByLooping(leadersJToken, newLeadingHitterInstance, "LeadingHitter");

            // _c.Complete.ThisMethod();
            return newHittingLeadersInstance;
        }

        // OPTION 2 : Step 2
        /// <param name="numberToReturn"> The number of hitters to return in the results (e.g. 50 would show you the top 50 leaders) </param>
        /// <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
        /// <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., Hr, Rbi etc) </param>
        ///         <see> View 'LeadingHitter' model for options that you can sort by for this method </see>
        public async Task GetHittingLeadersAsync(int numberToReturn, string year, string sortColumn)
        {
            await Task.Run(() => { CreateHittingLeadersModel(numberToReturn, year, sortColumn); });
        }

        // OPTION 2: Step 3
        /// <summary> Initiate retrieval of mlb hitting leaders for current season </summary>
        /// <param name="numberToReturn"> The number of hitters to return in the results (e.g. 50 would show you the top 50 leaders) </param>
        /// <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
        /// <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., Hr, Rbi etc) </param>
        ///         <see> View 'LeadingHitter' model for options that you can sort by for this method </see>
        /// <example> https://127.0.0.1:5001/api/mlb/hittingleaders </example>
        /// <returns> Hitting leaders for current season </returns>
        [HttpGet]
        [Route("hittingleaders/{year}")]
        public async Task<IActionResult> ViewHittingLeadersAsync(int numberToReturn, string year, string sortColumn)
        {
            // OPTION 2 --> three parameters needed to call the method
            await GetHittingLeadersAsync(5, "2018", "hr");

            string currently = "retrieving hitting leaders";

            return Content($"CURRENT TASK: {currently}");
        }

    }
}