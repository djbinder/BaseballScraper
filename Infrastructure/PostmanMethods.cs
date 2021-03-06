﻿using BaseballScraper.EndPoints;
using RestSharp;

#pragma warning disable CC0091, IDE0066
namespace BaseballScraper.Infrastructure
{
    public class PostmanMethods
    {
        /* GENERIC EXAMPLE
            [1] var endPoint = $"https://api.airtable.com/v0/{spRankingTableConfig.AuthenticationString}/{spRankingTableConfig.TableName}?api_key={spRankingTableConfig.ApiKey}";
            [2]var client = new RestClient(endPoint);
            [3]var request = new RestRequest(Method.GET);
            [4]request.AddHeader("Postman-Token", _airtableConfig.PostmanToken);
            [5]request.AddHeader("cache-control", "no-cache");
        */

        #region POSTMAN REQUEST ------------------------------------------------------------

        public class PostmanRequest
        {
            public RestClient Client { get; set; }
            public RestRequest Request { get; set; }
        }

        // STATUS: this works
        // request from MlbDataEndPoint
        public PostmanRequest CreatePostmanRequest (MlbDataApiEndPoints.MlbDataEndPoint endPoint, string tokenType)
        {
            string endPointUri = endPoint.EndPointUri;

            PostmanRequest postmanRequest = new PostmanRequest
            {
                Client  = new RestClient(endPointUri),
                Request = new RestRequest(Method.GET),
            };

            AddRequestHeader(postmanRequest, tokenType);
            return postmanRequest;
        }

        // request from string endPointUri
        public PostmanRequest CreatePostmanRequest(string endPointUri, string postmanToken)
        {
            PostmanRequest postmanRequest = new PostmanRequest
            {
                Client  = new RestClient(endPointUri),
                Request = new RestRequest(Method.GET),
            };

            postmanRequest.Request.AddHeader("Postman-Token", postmanToken);
            postmanRequest.Request.AddHeader("Cache-Control", "no-cache");
            return postmanRequest;
        }


        public PostmanRequest CreatePostmanRequestFromSwitch (string endPointUri, string tokenTypeForSwitch)
        {
            PostmanRequest postmanRequest = new PostmanRequest
            {
                Client  = new RestClient(endPointUri),
                Request = new RestRequest(Method.GET),
            };

            AddRequestHeader(postmanRequest, tokenTypeForSwitch);
            return postmanRequest;
        }


        private void AddRequestHeader(PostmanRequest postmanRequest, string tokenType)
        {
            string postmanToken = PostmanToken(tokenType);
            postmanRequest.Request.AddHeader("Postman-Token", postmanToken);
            postmanRequest.Request.AddHeader("Cache-Control", "no-cache");
        }


        #endregion POSTMAN REQUEST ------------------------------------------------------------





        #region POSTMAN RESPONSE ------------------------------------------------------------

        public class PostmanResponse
        {
            public IRestResponse Response { get; set; }
        }


        // STATUS: this works
        public PostmanResponse GetPostmanResponse(PostmanRequest request)
        {
            return new PostmanResponse
            {
                Response = request.Client.Execute(request.Request),
            };
        }


        public PostmanResponse CreatePostmanRequestGetResponse(string endPointUri, string tokenType)
        {
            PostmanRequest postmanRequest = new PostmanRequest
            {
                Client  = new RestClient(endPointUri),
                Request = new RestRequest(Method.GET),
            };

            AddRequestHeader(postmanRequest, tokenType);

            return new PostmanResponse
            {
                Response = postmanRequest.Client.Execute(postmanRequest.Request),
            };
        }

        #endregion POSTMAN RESPONSE ------------------------------------------------------------





        #region IREST RESPONSE ------------------------------------------------------------

        public IRestResponse CreateIRestResponseFromPostmanResponse(PostmanResponse postmanResponse)
        {
            return postmanResponse.Response;
        }

        public string CreateIRestResponseString(IRestResponse response)
        {
            return response.Content;
        }

        #endregion IREST RESPONSE ------------------------------------------------------------





        #region POSTMAN TOKEN ------------------------------------------------------------

        // STATUS: this works
        /// <summary>
        ///     A switch that returns a postman token depending upon the end type you are use
        /// </summary>
        /// <remarks> Each of the return values were generated by Postman </remarks>
        /// <param name="apiType"> The api method / endpoinot you are querying </param>
        /// <returns> A string that represents an api Postman token</returns>
        public string PostmanToken(string apiType)
        {
            switch (apiType)
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

                case "HitterSeasonStats":
                    return "b037e4c8-be75-4604-ab5b-74f7033a5fb4";

                case "PlayerTeams":
                    return "0d865811-639b-41e2-a5db-93623f323098";

                case "MlbStatsApiEndPoints_AllGamesDate":
                    return "fdc3f291-2428-450a-9eb7-3f45c0f01467";
                default:
                    throw new System.Exception("Unexpected Case");
            }
            throw new System.Exception("no api type found");
        }

        #endregion POSTMAN TOKEN ------------------------------------------------------------

    }
}
