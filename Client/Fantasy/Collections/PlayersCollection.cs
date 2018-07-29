using System.Collections.Generic;
using System.Threading.Tasks;

using BaseballScraper.Models;

namespace BaseballScraper.Client
{
    /// <summary>
    /// https://developer.yahoo.com/fantasysports/guide/#players-collection
    /// With the Players API, you can obtain information from a collection of players simultaneously.
    /// To obtains general players information, the players collection can be qualified in the URI by a particular game, league or team.
    /// To obtain specific league or team related information, the players collection is qualified by the relevant league or team.
    /// Each element beneath the Players Collection will be a Player Resource
    /// </summary>
    public class PlayersCollectionManager
    {

        public async Task<List<Player>> GetPlayers (string[] playerKeys, EndpointSubResourcesCollection subresources, string AccessToken)
        {
            return await Utils.GetCollection<Player> (ApiEndpoints.PlayersEndPoint (playerKeys, subresources), AccessToken, "player");
        }


        public async Task<List<League>> GetLeaguePlayers (string[] leagueKeys, string AccessToken, EndpointSubResourcesCollection subresources = null)
        {
            return await Utils.GetCollection<League> (ApiEndpoints.PlayersLeagueEndPoint (leagueKeys, subresources), AccessToken, "league");
        }


        public async Task<List<Team>> GetTeamPlayers (string AccessToken, string[] teamKeys = null, EndpointSubResourcesCollection subresources = null)
        {
            return await Utils.GetCollection<Team> (ApiEndpoints.PlayersTeamEndPoint (teamKeys, subresources), AccessToken, "team");
        }
    }
}
