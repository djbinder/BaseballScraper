using System.Collections.Generic;
using System.Threading.Tasks;

using BaseballScraper.Models;

namespace BaseballScraper.Client
{
    /// <summary>
    /// https://developer.yahoo.com/fantasysports/guide/#leagues-collection
    /// With the Leagues API, you can obtain information from a collection of leagues simultaneously.
    /// Each element beneath the Leagues Collection will be a League Resource
    /// </summary>
    public class LeaguesCollectionManager
    {

        public async Task<List<League>> GetLeagues (string AccessToken, string[] leagueKeys = null, EndpointSubResourcesCollection subresources = null)
        {
            return await Utils.GetCollection<League> (ApiEndpoints.LeaguesEndPoint (leagueKeys, subresources), AccessToken, "league");
        }
    }
}
