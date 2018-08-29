using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using RestSharp;

namespace BaseballScraper.Infrastructure
{
    public class PostmanMethods
    {
        private Constants _c = new Constants();

        public class PostmanRequest
        {
            public RestClient Client { get; set; }
            public RestRequest Request { get; set; }
        }
        public class PostmanResponse
        {
            public IRestResponse Response { get; set; }
        }

        public PostmanRequest CreatePostmanRequest (MlbDataApiEndPoints.MlbDataEndPoint endPoint, string tokenType)
        {
            // _c.Start.ThisMethod();

            // this gets you Trout
            var endPointUri = endPoint.EndPointUri;

            PostmanRequest postmanRequest = new PostmanRequest()
            {
                Client = new RestClient(endPointUri),

                Request = new RestRequest(Method.GET)
            };

            string postmanToken = PostmanToken(tokenType);
            postmanToken.Intro("postman token");

            postmanRequest.Request.AddHeader("Postman-Token", postmanToken);
            postmanRequest.Request.AddHeader("Cache-Control", "no-cache");

            // _c.Complete.ThisMethod();
            return postmanRequest;

        }

        public PostmanResponse GetPostmanResponse(PostmanRequest request)
        {
            // _c.Start.ThisMethod();
            return new PostmanResponse
            {
                Response = request.Client.Execute(request.Request)
            };
        }

        internal string PostmanToken(string apiType)
        {
            switch(apiType)
            {
                case "PlayerSearch": 
                    return "e49343c5-1a5a-4666-842f-ee67cb1796fc";

                case "PlayerInfo": 
                    return "bbd0ee6c-aa7e-4f56-a501-dd1bb3842307";

                case "ProjectedPitchingStats": 
                    return "410f04b5-78a0-4d88-a7c8-ec4425e45173";

                case "ProjectedHittingStats": 
                    return "bc1efdd8-3fc4-4836-b3c7-2e0364a2c618";

                case "PitchingLeaders": 
                    return "81596fc7-3027-4c1a-8288-8baca8d9a7e3";

                case "HittingLeaders": 
                    return "bc59dbdc-2968-4c03-9fe4-eededd57c016";
            }

            throw new System.Exception("no api type found");
        }
    }
}