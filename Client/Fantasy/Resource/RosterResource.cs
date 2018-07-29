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
    /// https://developer.yahoo.com/fantasysports/guide/#roster-resource
    /// Players on a team are organized into rosters corresponding to certain weeks, in NFL, or certain dates, in MLB, NBA, and NHL.
    /// Each player on a roster will be assigned a position if they’re in the starting lineup, or will be on the bench.
    /// You can only receive credit for stats accumulated by players in your starting lineup.
    ///
    /// You can use this API to edit your lineup by PUTting up new positions for the players on a roster.
    /// You can also add/drop players from your roster by `POSTing new transactions <#transactions-collection-POST>`__ to the league’s transactions collection.
    /// </summary>
    public class RosterResourceManager
    {

        public async Task<Roster> GetPlayers (string teamKey, int? week, DateTime? date, string AccessToken)
        {
            return await Utils.GetResource<Roster> (ApiEndpoints.RosterEndPoint (teamKey, week, date), AccessToken, "roster");
        }
    }
}
