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
    /// https://developer.yahoo.com/fantasysports/guide/#game-resource
    /// With the Game API, you can obtain the fantasy game related information, like the fantasy game name, the Yahoo! game code, and season.
    /// </summary>
    public class GameResourceManager
    {

        public async Task<Game> GetMeta (string gameKey, string AccessToken)
        {
            return await Utils.GetResource<Game> (ApiEndpoints.GameEndPoint (gameKey, EndpointSubResources.MetaData), AccessToken, "game");
        }

        public async Task<Game> GetLeagues (string gameKey, string[] leagueKeys, string AccessToken)
        {
            return await Utils.GetResource<Game> (ApiEndpoints.GameLeaguesEndPoint (gameKey, leagueKeys), AccessToken, "game");
        }

        public async Task<Game> GetPlayers (string gameKey, string[] playerKeys, string AccessToken)
        {
            return await Utils.GetResource<Game> (ApiEndpoints.GamePlayersEndPoint (gameKey, playerKeys), AccessToken, "game");
        }

        public async Task<Game> GetGameWeeks (string gameKey, string AccessToken)
        {
            return await Utils.GetResource<Game> (ApiEndpoints.GameEndPoint (gameKey, EndpointSubResources.GameWeeks), AccessToken, "game");
        }

        public async Task<Game> GetStatCategories (string gameKey, string AccessToken)
        {
            return await Utils.GetResource<Game> (ApiEndpoints.GameEndPoint (gameKey, EndpointSubResources.StatCategories), AccessToken, "game");
        }

        public async Task<Game> GetPositionTypes (string gameKey, string AccessToken)
        {
            return await Utils.GetResource<Game> (ApiEndpoints.GameEndPoint (gameKey, EndpointSubResources.PositionTypes), AccessToken, "game");
        }

        public async Task<Game> GetRosterPositions (string gameKey, string AccessToken)
        {
            return await Utils.GetResource<Game> (ApiEndpoints.GameEndPoint (gameKey, EndpointSubResources.RosterPositions), AccessToken, "game");
        }
    }
}
