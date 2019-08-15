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
        private static readonly Helpers _h = new Helpers();

        private static readonly MlbDataApiEndPoints _mlbDataApiEndPoints = new MlbDataApiEndPoints();

        private static readonly PostmanMethods _pmMethods = new PostmanMethods();

        // private static string _endpoint;



        [Route("test")]
        public void MlbStatsApiTesting()
        {
            _h.StartMethod();

            var endPoint = _mlbDataApiEndPoints.AllGamesForDateEndPoint(5,16,2016);
            var endPointUri = endPoint.EndPointUri;

            // BaseballScraper.EndPoints.MlbDataApiEndPoints+MlbDataEndPoint
            Console.WriteLine(endPoint);

            // https://statsapi.mlb.com/api/v1/schedule?sportId=1&date=5%2F16%2F2016
            Console.WriteLine(endPointUri);


            var type = "MlbStatsApiEndPoints_AllGamesDate";

            PostmanResponse pmResponse = _pmMethods.CreatePostmanRequestGetResponse(endPointUri, type);

            IRestResponse fullResponse = pmResponse.Response;
            var fullResponseContentLength = fullResponse.ContentLength;
            Console.WriteLine(fullResponse.ContentLength);

            string content = fullResponse.Content;
            var contentLength = content.Length;
            Console.WriteLine(contentLength);


            var allGamesDate = fullResponse as AllGamesForDate;
            Console.WriteLine(allGamesDate.Copyright);



            // Console.WriteLine(pmResponse.Response);

        }

    }
}
