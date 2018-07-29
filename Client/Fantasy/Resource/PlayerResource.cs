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
    /// https://developer.yahoo.com/fantasysports/guide/#player-resource
    /// With the Player API, you can obtain the player (athlete) related information, such as their name, professional team, and eligible positions.
    /// The player is identified in the context of a particular game, and can be requested as the base of your URI by using the global ````.
    /// </summary>
    public class PlayerResourceManager
    {

        public async Task<Player> GetMeta (string playerKey, string AccessToken)
        {
            return await Utils.GetResource<Player> (ApiEndpoints.PlayerEndPoint (playerKey, EndpointSubResources.MetaData), AccessToken, "game");
        }

        public async Task<Player> GetStats (string playerKey, string AccessToken)
        {
            return await Utils.GetResource<Player> (ApiEndpoints.PlayerEndPoint (playerKey, EndpointSubResources.Stats), AccessToken, "game");
        }


        public async Task<Player> GetOwnership (string[] playerKeys, string leagueKeys, string AccessToken)
        {
            return await Utils.GetResource<Player> (ApiEndpoints.PlayerOwnershipEndPoint (playerKeys, leagueKeys), AccessToken, "game");
        }


        public async Task<Player> GetPercentOwned (string playerKey, string AccessToken)
        {
            return await Utils.GetResource<Player> (ApiEndpoints.PlayerEndPoint (playerKey, EndpointSubResources.PercentOwned), AccessToken, "game");
        }


        public async Task<Player> GetDraftAnalysis (string playerKey, string AccessToken)
        {
            return await Utils.GetResource<Player> (ApiEndpoints.PlayerEndPoint (playerKey, EndpointSubResources.DraftAnalysis), AccessToken, "game");
        }
    }
}
