using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.MlbDataApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using C = System.Console;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.MlbDataApiControllers
{

    #region OVERVIEW ------------------------------------------------------------

    /// <summary>
    ///     Generate instance of MlbDataHittingLeader
    /// </summary>
    /// <list> INDEX
    ///     <item> Create Hitting Leaders Model<see cref="CreateHittingLeadersModel(int, string, string)" /> </item>
    ///     <item> Get Hitting Leaders Async   <see cref="GetHittingLeadersAsync(int, string, string)" /> </item>
    ///     <item> View Hitting Leaders Async  <see cref="ViewHittingLeadersAsync(int, string, string)" /> </item>
    /// </list>

    #endregion OVERVIEW ------------------------------------------------------------




    [Route("api/mlb/[controller]")]
    [ApiController]
    public class MlbDataHittingLeadersController: ControllerBase
    {
        private readonly Helpers             _helpers;
        private readonly ApiInfrastructure   _apiInfrastructure;
        private readonly MlbDataApiEndPoints _endPoints;
        private readonly PostmanMethods      _postman;


        public MlbDataHittingLeadersController(Helpers helpers, ApiInfrastructure apiInfrastructure, MlbDataApiEndPoints endPoints, PostmanMethods postman)
        {
            _helpers           = helpers;
            _apiInfrastructure = apiInfrastructure;
            _endPoints         = endPoints;
            _postman           = postman;
        }

        public MlbDataHittingLeadersController(){}



        /*
            https://127.0.0.1:5001/api/mlb/mlbdatahittingleaders/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();

        }



        #region STEP 1 ------------------------------------------------------------

            // STATUS [ July 15, 2019 ] : this works
            // STEP 1
            /// <summary>
            ///     Get the current seasons hitting leaders; Endpoint parameters passed as parameters to method
            /// </summary>
            /// <remarks>
            ///     Parameters for 'HittingLeadersEndPoint' (i.e. numberToReturn, year, sortColumn) are passed as parameters to the method
            ///     See: https://appac.github.io/mlb-data-api-docs/#reports-hitting-leaders-get
            /// </remarks>
            /// <param name="numberToReturn">
            ///     The number of hitters to return in the results (e.g. 50 would show you the top 50 leaders)
            /// </param>
            /// <param name="year">
            ///     The year that you want to retrieve the leaders for (e.g. 2018 gets you leaders for 2018 mlb season)
            /// </param>
            /// <param name="sortColumn">
            ///     This is the stat you want to retrieve the leaders for (e.g., Hr, Rbi etc)
            ///     See: View 'LeadingHitter' model for options that you can sort by for this method
            /// </param>
            /// <example>
            ///     var hittingLeaders = CreateHittingLeadersModel(5, "2019", "hr");
            /// </example>
            /// <returns>
            ///     A list of instantiated 'LeadingHitter' for 'numberToReturn' number of hitters
            /// </returns>
            public List<LeadingHitter> CreateHittingLeadersModel (int numberToReturn, string year, string sortColumn)
            {
                var newEndPoint        = _endPoints.HittingLeadersEndPoint(numberToReturn, year, sortColumn);
                C.WriteLine(newEndPoint);
                var postmanRequest     = _postman.CreatePostmanRequest(newEndPoint, "HittingLeaders");
                var postmanResponse    = _postman.GetPostmanResponse(postmanRequest);
                IRestResponse response = postmanResponse.Response;

                JObject leadersJObject  = _apiInfrastructure.CreateModelJObject(response);
                JToken leadersJToken    = _apiInfrastructure.CreateModelJToken(leadersJObject, "HittingLeaders");
                int jTokenChildrenCount = leadersJToken.Count<object>();
                C.WriteLine($"jTokenChildrenCount: {jTokenChildrenCount}");

                List<LeadingHitter> listOfPlayers     = new List<LeadingHitter>();
                LeadingHitter newLeadingHitterInstance = new LeadingHitter();

                for(var counter = 0; counter < jTokenChildrenCount; counter++)
                {
                    newLeadingHitterInstance =
                        _apiInfrastructure.CreateInstanceOfModel
                        (
                            leadersJToken[counter],
                            newLeadingHitterInstance,
                            "LeadingHitter"
                        ) as LeadingHitter;

                    listOfPlayers.Add(newLeadingHitterInstance);
                }

                foreach(var player in listOfPlayers)
                {
                    C.WriteLine(player.NameLastCommaFirst);
                }
                return listOfPlayers;
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
                string currently = "retrieving hitting leaders";
                return Content($"CURRENT TASK: {currently}");
            }




        #endregion STEP 3 ------------------------------------------------------------


    }
}





// // STATUS: this works
// // STEP 1
// /// <summary> OPTION 2: Get the current seasons hitting leaders; Endpoint parameters defined within the method </summary>
// /// <remarks> Parameters for 'HittingLeadersEndPoint' are defined within the method </remarks>
// /// <returns> A list of instantiated 'LeadingHitter' for 'numberToReturn' number of hitters </returns>
// public HittingLeaders CreateHittingLeadersModel ()
// {
//     // retrieve the 'HittingLeaders' end point
//         // param 1: number of hitters to include in search
//         // param 2: season that you want to query
//         // param 3: stat that you would like to sort by
//     var newEndPoint = _endPoints.HittingLeadersEndPoint(5, "2018", "hr");

//     var postmanRequest = _postman.CreatePostmanRequest(newEndPoint, "HittingLeaders");
//     var postmanResponse = _postman.GetPostmanResponse(postmanRequest);

//     IRestResponse response = postmanResponse.Response;

//     JObject leadersJObject = _a.CreateModelJObject(response);

//     JToken leadersJToken = _a.CreateModelJToken(leadersJObject, "HittingLeaders");

//     // This will return an object that contains multiple 'LeadingHitter'; The number returned depends on first parameter passed when retrieving the 'HittingLeadersEndPoint' (see above)
//     HittingLeaders newHittingLeadersInstance = new HittingLeaders();

//     _a.CreateInstanceOfModel(leadersJToken, newHittingLeadersInstance, "HittingLeaders");

//     LeadingHitter newLeadingHitterInstance = new LeadingHitter();
//     _a.CreateMultipleInstancesOfModelByLooping(leadersJToken, newLeadingHitterInstance, "LeadingHitter");

//     return newHittingLeadersInstance;
// }




// // STATUS: this works
// // STEP 2
// /// <summary> OPTION 2: Gets a list of Mlb hitting leaders for a defined season; The variables are defined within the 'CreateHittingLeadersModel' function </summary>
// public async Task GetHittingLeadersAsync()
// {
//     await Task.Run(() => { CreateHittingLeadersModel(); });
// }



// // STATUS: this works
// // STEP 3
// /// <summary> OPTION 2: Initiate retrieval of mlb hitting leaders for current season </summary>
// /// <example> https://127.0.0.1:5001/api/mlb/hittingleaders </example>
// /// <returns> Hitting leaders for current season </returns>
// [HttpGet]
// [Route("hittingleaders")]
// public async Task<IActionResult> ViewHittingLeadersAsync()
// {
//     await GetHittingLeadersAsync();
//     string currently = "retrieving hitting leaders";
//     return Content($"CURRENT TASK: {currently}");
// }
