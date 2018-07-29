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
    /// https://developer.yahoo.com/fantasysports/guide/#league-resource
    /// When users join a Fantasy Football, Baseball, Basketball, or Hockey draft and trade game,
    /// they are organized into leagues with a limited number of friends or other Yahoo! users, with each user managing a Team.
    /// With the League API, you can obtain the league related information, like the league name, the number of teams, the draft status, et cetera.
    /// Leagues only exist in the context of a particular Game, although you can request a League Resource as the base of your URI by using the global ````.
    /// A particular user can only retrieve data for private leagues of which they are a member, or for public leagues.
    /// </summary>
    public class LeagueResourceManager
    {

        public async Task<League> GetMeta (string leagueKey, string AccessToken)
        {
            return await Utils.GetResource<League> (ApiEndpoints.LeagueEndPoint (leagueKey, EndpointSubResources.MetaData), AccessToken, "league");
        }

        public async Task<League> GetSettings (string leagueKey, string AccessToken)
        {
            return await Utils.GetResource<League> (ApiEndpoints.LeagueEndPoint (leagueKey, EndpointSubResources.Settings), AccessToken, "league");
        }

        public async Task<League> GetStandings (string leagueKey, string AccessToken)
        {
            return await Utils.GetResource<League> (ApiEndpoints.LeagueEndPoint (leagueKey, EndpointSubResources.Standings), AccessToken, "league");
        }


        public async Task<League> GetScoreboard (string leagueKey, string AccessToken, int?[] weeks = null)
        {
            return await Utils.GetResource<League> (ApiEndpoints.LeagueEndPoint (leagueKey, EndpointSubResources.Scoreboard, weeks), AccessToken, "league");
        }

        public async Task<League> GetTeams (string leagueKey, string AccessToken)
        {
            return await Utils.GetResource<League> (ApiEndpoints.LeagueEndPoint (leagueKey, EndpointSubResources.Teams), AccessToken, "league");
        }


        public async Task<League> GetDraftResults (string leagueKey, string AccessToken)
        {
            return await Utils.GetResource<League> (ApiEndpoints.LeagueEndPoint (leagueKey, EndpointSubResources.DraftResults), AccessToken, "league");
        }


        public async Task<League> GetTransactions (string leagueKey, string AccessToken)
        {
            return await Utils.GetResource<League> (ApiEndpoints.LeagueEndPoint (leagueKey, EndpointSubResources.Transactions), AccessToken, "league");
        }
    }
}
