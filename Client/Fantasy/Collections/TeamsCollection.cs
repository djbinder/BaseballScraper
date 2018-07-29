using System.Collections.Generic;
using System.Threading.Tasks;

using BaseballScraper.Models;

namespace BaseballScraper.Client
{
    /// <summary>
    /// https://developer.yahoo.com/fantasysports/guide/#teams-collection
    /// With the Teams API, you can obtain information from a collection of teams simultaneously.
    /// The teams collection is qualified in the URI by a particular league to obtain information about teams within the league,
    /// or by a particular user (and optionally, a game) to obtain information about the teams owned by the user.
    /// Each element beneath the Teams Collection will be a Team Resource
    /// </summary>
    public class TeamsCollectionManager
    {

        public async Task<List<Team>> GetTeams (string[] teamKeys, EndpointSubResourcesCollection subresources, string AccessToken)
        {
            return await Utils.GetCollection<Team> (ApiEndpoints.TeamsEndPoint (teamKeys, subresources), AccessToken, "team");
        }


        public async Task<List<League>> GetLeagueTeams (string AccessToken, string[] leagueKeys = null, EndpointSubResourcesCollection subresources = null)
        {
            return await Utils.GetCollection<League> (ApiEndpoints.TeamsLeagueEndPoint (leagueKeys, subresources), AccessToken, "league");
        }


        public async Task<List<Game>> GetUserGamesTeams (string AccessToken, string[] gameKeys = null, EndpointSubResourcesCollection subresources = null)
        {
            return await Utils.GetCollection<Game> (ApiEndpoints.TeamsUserGamesEndPoint (gameKeys, subresources), AccessToken, "game");
        }
    }
}
