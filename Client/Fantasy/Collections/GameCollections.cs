using System.Collections.Generic;
using System.Threading.Tasks;

using BaseballScraper.Models;

namespace BaseballScraper.Client
{
    /// <summary>
    /// https://developer.yahoo.com/fantasysports/guide/#games-collection
    /// With the Games API, you can obtain information from a collection of games simultaneously.
    /// Each element beneath the Games Collection will be a Game Resource
    /// </summary>
    public class GameCollectionsManager
    {

        public async Task<List<Game>> GetGames (string gameKey, string AccessToken)
        {
            return await Utils.GetCollection<Game> (ApiEndpoints.GameEndPoint (gameKey), AccessToken, "game");
        }


        public async Task<List<Game>> GetGames (string[] gameKeys, string AccessToken, EndpointSubResourcesCollection subresources = null, GameCollectionFilters filters = null)
        {
            return await Utils.GetCollection<Game> (ApiEndpoints.GamesEndPoint (gameKeys, subresources, filters), AccessToken, "game");
        }


        public async Task<List<Game>> GetGamesUsers (string AccessToken, string[] gameKeys = null, EndpointSubResourcesCollection subresources = null, GameCollectionFilters filters = null)
        {
            return await Utils.GetCollection<Game> (ApiEndpoints.GamesUserEndPoint (gameKeys, subresources, filters), AccessToken, "game");
        }
    }
}
