using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.Controllers.MlbDataApiControllers;
using BaseballScraper.Controllers.Player;

using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaseballScraper.Pages
{
    #pragma warning disable CS0414, CS0219, CS1591, IDE0052
    public class PlayerSearchPage : PageModel
    {
        private readonly Helpers _h = new Helpers();
        private readonly MlbDataSeasonHittingStats _sHS = new MlbDataSeasonHittingStats();
        private readonly MlbDataPlayerTeams _pT = new MlbDataPlayerTeams();

        // private readonly PlayerBaseController _pBC = new PlayerBaseController();

        public SfbbPlayerBase SfbbPlayer { get; set; }
        public List<SelectListItem> SelectOptions { get; set; }
        public List<int> PlayerSeasons { get; set; }


        [BindProperty(SupportsGet = true)]
        public string PlayerName { get; set; }



        public PlayerSearchPage()
        {
        }

        // note: url structure is defined on the corresponding .cshtml page next to the "@page" tag in the first row
        public IActionResult OnGet()
        {
            Console.WriteLine();

            CreatePlayerListSelectOptions();

            SelectPlayerToSearch();


            // _pT.GetListOfPlayersSeasons("493316");
            // _pT.GetTeamsForPlayerAllSeasons("493316");

            // _sHS.GetStatsForSeason();


            return Page();
        }

        private void SelectPlayerToSearch()
        {
            IEnumerable<Models.Player.SfbbPlayerBase> playerBases = PlayerBaseController.PlayerBaseFromGoogleSheet.GetAllPlayerBasesFromGoogleSheet("A5:AQ2289");
            // Console.WriteLine($"playerBases count {playerBases.Count()}");
            // Console.WriteLine($"playerBases count {playerBases.LongCount()}");

            if (!string.IsNullOrEmpty(PlayerName))
            {
                // Console.WriteLine($"PlayerName: {PlayerName}");
                playerBases = playerBases.Where(player => player.PLAYERNAME == PlayerName);
            }

            var playerBasesList = playerBases.ToList();

            SfbbPlayer = playerBasesList[0];

            // Console.WriteLine($"playerBasesList.Count: {playerBasesList.Count}");

            var playerFullName = SfbbPlayer.PLAYERNAME;
            // var playerFullName = playerBasesList[0].PLAYERNAME;
            // Console.WriteLine($"playerFullName: {playerFullName}");

            PlayerSeasons = _pT.GetListOfPlayersSeasons(SfbbPlayer.MLBID);
            PlayerSeasons.ForEach((season) => Console.WriteLine($"season: {season}"));
        }

        private void CreatePlayerListSelectOptions()
        {
            List<IList<object>> allPlayerBasesFromGoogleSheets = PlayerBaseController.PlayerBaseFromGoogleSheet.GetAllPlayerBaseObjectsFromGoogleSheet("B5:B2286").ToList();
            // Console.WriteLine($"google sheets count: {allPlayerBasesFromGoogleSheets.Count}");

            SelectOptions = allPlayerBasesFromGoogleSheets.Select(player =>
                new SelectListItem
                {
                    Value   = player[0].ToString(),
                    Text    = player[0].ToString(),
                })
                .OrderBy(y => y.Text).ToList();
        }
    }
}
