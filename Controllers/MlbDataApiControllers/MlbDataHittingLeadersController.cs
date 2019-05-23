using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.MlbDataApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BaseballScraper.Controllers.MlbDataApiControllers
{

    #region OVERVIEW ------------------------------------------------------------

        /// <summary> Provides two options on how to generate leading hitter model
        ///     OPTION 1 - endpoint parameters passed as parameters to method
        ///     OPTION 2 - endpoint parameters defined within the method
        /// </summary>
        /// <list> INDEX
        ///     <item> Create Hitting Leaders Model (Option 1) <see cref="MlbDataHittingLeadersController.CreateHittingLeadersModel(int, string, string)" /> </item>
        ///     <item> Create Hitting Leaders Model (Option 2) <see cref="MlbDataHittingLeadersController.CreateHittingLeadersModel()" /> </item>
        ///     <item> Get Hitting Leaders Async    (Option 1) <see cref="MlbDataHittingLeadersController.GetHittingLeadersAsync(int, string, string)" /> </item>
        ///     <item> Get Hitting Leaders Async    (Option 2) <see cref="MlbDataHittingLeadersController.GetHittingLeadersAsync()" /> </item>
        ///     <item> View Hitting Leaders Async   (Option 1) <see cref="MlbDataHittingLeadersController.ViewHittingLeadersAsync(int, string, string)" /> </item>
        ///     <item> View Hitting Leaders Async   (Option 2) <see cref="MlbDataHittingLeadersController.ViewHittingLeadersAsync()" /> </item>
        /// </list>

    #endregion OVERVIEW ------------------------------------------------------------



    [Route("api/mlb")]
    [ApiController]
    #pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
    public class MlbDataHittingLeadersController: Controller
    {
        private readonly Helpers _h                            = new Helpers();
        private readonly ApiInfrastructure _a                  = new ApiInfrastructure();
        private static readonly MlbDataApiEndPoints _endPoints = new MlbDataApiEndPoints();
        private static readonly PostmanMethods _postman        = new PostmanMethods();



        #region STEP 1 ------------------------------------------------------------

            // STATUS: this works
            // STEP 1
            /// <summary> OPTION 1: Get the current seasons hitting leaders; Endpoint parameters passed as parameters to method  </summary>
            /// <remarks> Parameters for 'HittingLeadersEndPoint' (i.e. numberToReturn, year, sortColumn) are passed as parameters to the method </remarks>
            /// <param name="numberToReturn"> The number of hitters to return in the results (e.g. 50 would show you the top 50 leaders) </param>
            /// <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
            /// <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., Hr, Rbi etc) </param>
            /// <see> View 'LeadingHitter' model for options that you can sort by for this method </see>
            /// <returns> A list of instantiated 'LeadingHitter' for 'numberToReturn' number of hitters </returns>
            public HittingLeaders CreateHittingLeadersModel (int numberToReturn, string year, string sortColumn)
            {
                // retrieve the 'HittingLeaders' end point
                    // param 1: number of hitters to include in search
                    // param 2: season that you want to query
                    // param 3: stat that you would like to sort by
                var newEndPoint = _endPoints.HittingLeadersEndPoint(numberToReturn, year, sortColumn);
                // var newEndPoint = _endPoints.HittingLeadersEndPoint(5, "2018", "hr");

                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "HittingLeaders");
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

                IRestResponse response = postmanResponse.Response;

                JObject leadersJObject = _a.CreateModelJObject(response);

                JToken leadersJToken = _a.CreateModelJToken(leadersJObject, "HittingLeaders");

                // This will return an object that contains multiple 'LeadingHitter'; The number returned depends on first parameter passed when retrieving the 'HittingLeadersEndPoint' (see above)
                HittingLeaders newHittingLeadersInstance = new HittingLeaders();

                _a.CreateInstanceOfModel(leadersJToken, newHittingLeadersInstance, "HittingLeaders");

                LeadingHitter newLeadingHitterInstance = new LeadingHitter();
                _a.CreateMultipleInstancesOfModelByLooping(leadersJToken, newLeadingHitterInstance, "LeadingHitter");

                return newHittingLeadersInstance;
            }


            // STATUS: this works
            // STEP 1
            /// <summary> OPTION 2: Get the current seasons hitting leaders; Endpoint parameters defined within the method </summary>
            /// <remarks> Parameters for 'HittingLeadersEndPoint' are defined within the method </remarks>
            /// <returns> A list of instantiated 'LeadingHitter' for 'numberToReturn' number of hitters </returns>
            public HittingLeaders CreateHittingLeadersModel ()
            {
                // retrieve the 'HittingLeaders' end point
                    // param 1: number of hitters to include in search
                    // param 2: season that you want to query
                    // param 3: stat that you would like to sort by
                var newEndPoint = _endPoints.HittingLeadersEndPoint(5, "2018", "hr");

                var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "HittingLeaders");
                var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

                IRestResponse response = postmanResponse.Response;

                JObject leadersJObject = _a.CreateModelJObject(response);

                JToken leadersJToken = _a.CreateModelJToken(leadersJObject, "HittingLeaders");

                // This will return an object that contains multiple 'LeadingHitter'; The number returned depends on first parameter passed when retrieving the 'HittingLeadersEndPoint' (see above)
                HittingLeaders newHittingLeadersInstance = new HittingLeaders();

                _a.CreateInstanceOfModel(leadersJToken, newHittingLeadersInstance, "HittingLeaders");

                LeadingHitter newLeadingHitterInstance = new LeadingHitter();
                _a.CreateMultipleInstancesOfModelByLooping(leadersJToken, newLeadingHitterInstance, "LeadingHitter");

                return newHittingLeadersInstance;
            }

        #endregion STEP 1 ------------------------------------------------------------



        #region STEP 2 ------------------------------------------------------------

            // STATUS: this works
            // STEP 2
            /// <summary> OPTION 1: Gets a list of Mlb hitting leaders for a defined season </summary>
            /// <param name="numberToReturn"> OPTION 2: The number of hitters to return in the results (e.g. 50 would show you the top 50 leaders) </param>
            /// <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
            /// <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., Hr, Rbi etc) </param>
            ///         <see> View 'LeadingHitter' model for options that you can sort by for this method </see>
            public async Task GetHittingLeadersAsync(int numberToReturn, string year, string sortColumn)
            {
                await Task.Run(() => { CreateHittingLeadersModel(numberToReturn, year, sortColumn); });
            }


            // STATUS: this works
            // STEP 2
            /// <summary> OPTION 2: Gets a list of Mlb hitting leaders for a defined season; The variables are defined within the 'CreateHittingLeadersModel' function </summary>
            public async Task GetHittingLeadersAsync()
            {
                await Task.Run(() => { CreateHittingLeadersModel(); });
            }

        #endregion STEP 2 ------------------------------------------------------------



        #region STEP 3 ------------------------------------------------------------

            // STATUS: this works
            // STEP 3
            /// <summary> OPTION 1: Initiate retrieval of mlb hitting leaders for current season </summary>
            /// <param name="numberToReturn"> The number of hitters to return in the results (e.g. 50 would show you the top 50 leaders) </param>
            /// <param name="year"> The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season) </param>
            /// <param name="sortColumn"> This is the stat you want to retrieve the leaders for (e.g., Hr, Rbi etc) </param>
            /// <see> View 'LeadingHitter' model for options that you can sort by for this method </see>
            /// <example> https://127.0.0.1:5001/api/mlb/hittingleaders </example>
            /// <returns> Hitting leaders for current season </returns>
            [HttpGet]
            [Route("hittingleaders/{year}")]
            public async Task<IActionResult> ViewHittingLeadersAsync(int numberToReturn, string year, string sortColumn)
            {
                await GetHittingLeadersAsync(numberToReturn, year, sortColumn);
                // await GetHittingLeadersAsync(5, "2018", "hr");
                string currently = "retrieving hitting leaders";
                return Content($"CURRENT TASK: {currently}");
            }


            // STATUS: this works
            // STEP 3
            /// <summary> OPTION 2: Initiate retrieval of mlb hitting leaders for current season </summary>
            /// <example> https://127.0.0.1:5001/api/mlb/hittingleaders </example>
            /// <returns> Hitting leaders for current season </returns>
            [HttpGet]
            [Route("hittingleaders")]
            public async Task<IActionResult> ViewHittingLeadersAsync()
            {
                await GetHittingLeadersAsync();
                string currently = "retrieving hitting leaders";
                return Content($"CURRENT TASK: {currently}");
            }

        #endregion STEP 3 ------------------------------------------------------------


    }
}
