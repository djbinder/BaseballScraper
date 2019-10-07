using System;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using static BaseballScraper.Infrastructure.PostmanMethods;
using BaseballScraper.Models.MlbStatsApi;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.MlbStatsApiControllers
{
    [Route("api/mlbstatsapi/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MlbStatsApiGameController: ControllerBase
    {
        private readonly Helpers             _helpers;
        private readonly ApiInfrastructure   _apiInfrastructure;
        private readonly MlbDataApiEndPoints _endPoints;
        private readonly PostmanMethods      _postman;


        public MlbStatsApiGameController(Helpers helpers, ApiInfrastructure apiInfrastructure, MlbDataApiEndPoints endPoints, PostmanMethods postman)
        {
            _helpers           = helpers;
            _apiInfrastructure = apiInfrastructure;
            _endPoints         = endPoints;
            _postman           = postman;
        }

        public MlbStatsApiGameController(){}



        // See: https://statsapi.mlb.com/api/v1/schedule?sportId=1&date=5%2F16%2F2016
        [Route("test")]
        public void MlbStatsApiTesting()
        {
            _helpers.StartMethod();

            var endPoint    = _endPoints.AllGamesForDateEndPoint(5,16,2016);
            var endPointUri = endPoint.EndPointUri;

            const string type = "MlbStatsApiEndPoints_AllGamesDate";

            PostmanResponse pmResponse     = _postman.CreatePostmanRequestGetResponse(endPointUri, type);
            IRestResponse fullResponse     = pmResponse.Response;

            // long fullResponseContentLength = fullResponse.ContentLength;

            string content    = fullResponse.Content;
            int contentLength = content.Length;

            AllGamesForDate allGamesDate = fullResponse as AllGamesForDate;

            Console.WriteLine(fullResponse.ContentLength);
            Console.WriteLine(contentLength);
            Console.WriteLine(allGamesDate.Copyright);
        }
    }
}
