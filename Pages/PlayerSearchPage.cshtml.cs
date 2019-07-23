using System;
using System.Collections.Generic;
using System.Linq;
using BaseballScraper.Controllers.AGGREGATORS;
using BaseballScraper.Controllers.MlbDataApiControllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static BaseballScraper.Controllers.PlayerControllers.PlayerBaseController;
using C = System.Console;

#pragma warning disable CS0414, CS0219, CS1591, IDE0052
namespace BaseballScraper.Pages
{
    public class PlayerSearchPage : PageModel
    {
        private readonly Helpers                      _helpers;
        private readonly MlbDataPlayerTeamsController _pT;
        private readonly PlayerBaseFromGoogleSheet    _playerBaseFromGoogleSheet;
        private readonly LaunchCoreSpSitesController  _launchCoreSpSitesController;
        private readonly MlbDataSeasonHittingStatsController _mlbDataSeasonHittingStatsController;


        public SfbbPlayerBase SfbbPlayer          { get; set; }
        public List<SelectListItem> SelectOptions { get; set; }
        public List<int> PlayerSeasons            { get; set; }



        [BindProperty(SupportsGet = true)]
        public string PlayerName { get; set; }


        // these all connect to fields in the .cshtml
        [TempData] public string Games { get; set; }
        [TempData] public string PlateAppearances { get; set; }
        [TempData] public string AtBats { get; set; }
        [TempData] public string Hits { get; set; }
        [TempData] public string Runs { get; set; }
        [TempData] public string HomeRuns { get; set; }
        [TempData] public string RBIs { get; set; }
        [TempData] public string SBs { get; set; }
        [TempData] public string Walks { get; set; }
        [TempData] public string AVG { get; set; }



        public PlayerSearchPage(Helpers helpers, MlbDataPlayerTeamsController pT, PlayerBaseFromGoogleSheet playerBaseFromGoogleSheet, LaunchCoreSpSitesController  launchCoreSpSitesController, MlbDataSeasonHittingStatsController mlbDataSeasonHittingStatsController)
        {
            _helpers                             = helpers;
            _pT                                  = pT;
            _playerBaseFromGoogleSheet           = playerBaseFromGoogleSheet;
            _launchCoreSpSitesController         = launchCoreSpSitesController;
            _mlbDataSeasonHittingStatsController = mlbDataSeasonHittingStatsController;
        }



        /*
            https://127.0.0.1:5001/player_search
            note: url structure is defined on the corresponding .cshtml page next to the "@page" tag in the first row
        */
        public IActionResult OnGet()
        {
            C.WriteLine();
            CreatePlayerListSelectOptions();
            SelectPlayerToSearch();
            return Page();
        }


        // Create list of a player names to be used as dropdown options in .cshtml page
        private void CreatePlayerListSelectOptions()
        {
            List<IList<object>> allPlayerBasesFromGoogleSheets = _playerBaseFromGoogleSheet.GetAllPlayerBaseObjectsFromGoogleSheet("B7:B2333").ToList();

            SelectOptions = allPlayerBasesFromGoogleSheets.Select(player =>
                new SelectListItem
                {
                    Value   = player[0].ToString(),
                    Text    = player[0].ToString(),
                })
                .OrderBy(y => y.Text).ToList();
        }


        private void SelectPlayerToSearch()
        {
            IEnumerable<SfbbPlayerBase> playerBases = _playerBaseFromGoogleSheet.GetAllPlayerBasesFromGoogleSheet("A7:AQ2333");

            if (!string.IsNullOrEmpty(PlayerName))
            {
                playerBases = playerBases.Where(player => player.PLAYERNAME == PlayerName);
            }

            string mlbId = "519203";
            var onePlayersBase =
                from bases in playerBases
                where bases.MLBID == mlbId
                select bases;

            SfbbPlayer         = onePlayersBase.First();
            string firstName   = SfbbPlayer.FIRSTNAME;
            string lastName    = SfbbPlayer.LASTNAME;
            string mlbPlayerId = SfbbPlayer.MLBID;
            C.WriteLine($"firstName: {firstName}\t lastName: {lastName}\t mlbPlayerId: {mlbPlayerId}");

            var hitter = _mlbDataSeasonHittingStatsController.CreateHitterSeasonStatsInstance("2019", mlbPlayerId);

            Games            = hitter.Games;
            PlateAppearances = hitter.PlateAppearances;
            AtBats           = hitter.AtBats;
            Hits             = hitter.Hits;
            Runs             = hitter.Runs;
            HomeRuns         = hitter.HomeRuns;
            RBIs             = hitter.RBIs;
            SBs              = hitter.StolenBases;
            Walks            = hitter.Walks;
            AVG              = hitter.BattingAverage;

            _launchCoreSpSitesController.LaunchAllPagesInChromeForPlayer(firstName, lastName);
        }




        private void PrintSeasons()
        {
            PlayerSeasons.ForEach((season) => C.WriteLine($"season: {season}"));
        }

        private void PrintPlayerInfo(string playerFullName)
        {
            C.WriteLine($"SEARCHING FOR: {playerFullName}");
        }
    }
}


// <h5>Active Seasons</h5>
// <ul>@foreach(var year in Model.PlayerSeasons)
//     {
//         <li>@year</li>
//     }
// </ul>
