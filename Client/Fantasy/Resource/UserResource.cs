using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

using BaseballScraper.Infrastructure;
using BaseballScraper.Models;

namespace BaseballScraper.Client
{
    /// <summary>
    /// https://developer.yahoo.com/fantasysports/guide/#user-resource
    /// With the User API, you can retrieve fantasy information for a particular Yahoo! user. Most usefully, you can see which games a user is playing,
    /// and which leagues they belong to and teams that they own within those games.
    /// Because you can currently only view user information for the logged in user, you would generally want to use the Users collection,
    /// passing along the use_login flag, instead of trying to request a User resource directly from the URI.
    /// </summary>
    public class UserResourceManager
    {

        public async Task<User> GetUser (string AccessToken)
        {
            return await Utils.GetResource<User> (ApiEndpoints.UserGamesEndPoint, AccessToken, "user");
        }


        public async Task<User> GetUserGameLeagues (string AccessToken, string[] gameKeys = null, EndpointSubResourcesCollection subresources = null)
        {
            return await Utils.GetResource<User> (ApiEndpoints.UserGameLeaguesEndPoint (gameKeys, subresources), AccessToken, "user");
        }


        public async Task<User> GetUserGamesTeamsEndPoint (string AccessToken, string[] gameKeys = null, EndpointSubResourcesCollection subresources = null)
        {
            return await Utils.GetResource<User> (ApiEndpoints.UserGamesTeamsEndPoint (gameKeys, subresources), AccessToken, "user");
        }

    }
}
