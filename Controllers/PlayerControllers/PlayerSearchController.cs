using System.Collections.Generic;
using System.Threading.Tasks;
using BaseballScraper.Controllers.AGGREGATORS;
using BaseballScraper.Controllers.MlbDataApiControllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Player;
using BaseballScraper.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using C = System.Console;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006, MA0016
namespace BaseballScraper.Controllers.PlayerControllers
{
    [Route("player/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PlayerSearchController : Controller
    {
        private readonly Helpers _helpers;
        // private readonly PlayerBaseFromGoogleSheet    _playerBaseFromGoogleSheet;
        private readonly LaunchCoreSpSitesController  _launchCoreSpSitesController;
        private readonly MlbDataSeasonHittingStatsController _mlbDataSeasonHittingStatsController;


        public PlayerSearchController(Helpers helpers, LaunchCoreSpSitesController  launchCoreSpSitesController, MlbDataSeasonHittingStatsController mlbDataSeasonHittingStatsController)
        {
            _helpers = helpers;
            // _playerBaseFromGoogleSheet = playerBaseFromGoogleSheet;
            _launchCoreSpSitesController         = launchCoreSpSitesController;
            _mlbDataSeasonHittingStatsController = mlbDataSeasonHittingStatsController;
        }

        public PlayerSearchController() {}


        public SfbbPlayerBase SfbbPlayer          { get; set; }
        // public List<SelectListItem> SelectOptions { get; set; }
        public List<int> PlayerSeasons            { get; set; }


        // [BindProperty(SupportsGet = true)]
        // public string PlayerName { get; set; }

        [TempData] public string Games            { get; set; }
        [TempData] public string PlateAppearances { get; set; }
        [TempData] public string AtBats           { get; set; }
        [TempData] public string Hits             { get; set; }
        [TempData] public string Runs             { get; set; }
        [TempData] public string HomeRuns         { get; set; }
        [TempData] public string RBIs             { get; set; }
        [TempData] public string SBs              { get; set; }
        [TempData] public string Walks            { get; set; }
        [TempData] public string AVG              { get; set; }




        /*
            https://127.0.0.1:5001/player/playersearch/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
        }


        /*
            https://127.0.0.1:5001/player/playersearch/async
        */
        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
        }





        private PlayerSearchViewModel _playerSearchViewModel = new PlayerSearchViewModel();


        // STATUS [ July 24, 2019 ]: DOES NOT WORK; ISSUES WITH THE HTML
        // https://nimblegecko.com/using-simple-drop-down-lists-in-ASP-NET-MVC/
        // [HttpGet("player_search")]
        // public IActionResult ViewPlayerSearch()
        // {
        //     C.WriteLine("PlayerSearch");

        //     PlayerSearchViewModel playerSearchViewModel = new PlayerSearchViewModel
        //     {
        //         PlayersEnumerable = CreateListOfPlayerNamesToSelect()
        //     };
        //     return View("PlayerSearch", playerSearchViewModel);
        // }






        #region START ------------------------------------------------------------


        // Create list of a player names to be used as dropdown options in .cshtml page
        // public SelectList CreatePlayerListSelectOptions()
        // {
        //     List<IList<object>> allPlayerBasesFromGoogleSheets = _playerBaseFromGoogleSheet.GetAllPlayerBaseObjectsFromGoogleSheet("B7:B2333").ToList();

        //     var selectOptions = _playerSearchViewModel.PlayersList;
        //     selectOptions = allPlayerBasesFromGoogleSheets.Select(player =>
        //         new SelectListItem
        //         {
        //             Value   = player[0].ToString(),
        //             Text    = player[0].ToString(),
        //         })
        //         .OrderBy(y => y.Text).ToList();
        //     return new SelectList(selectOptions, "Value", "Text");
        // }


        // public IEnumerable<SelectListItem> CreateListOfPlayerNamesToSelect()
        // {
        //     List<IList<object>> allPlayerBasesFromGoogleSheets = _playerBaseFromGoogleSheet.GetAllPlayerBaseObjectsFromGoogleSheet("B7:B2333").ToList();

        //     var selectOptions = _playerSearchViewModel.PlayersList;

        //     selectOptions = allPlayerBasesFromGoogleSheets.Select(player =>
        //         new SelectListItem
        //         {
        //             Value   = player[0].ToString(),
        //             Text    = player[0].ToString(),
        //         })
        //         .OrderBy(y => y.Text).ToList();

        //     return selectOptions;
        // }


        // public IActionResult SearchForPlayer(PlayerSearchViewModel selectedPlayer)
        // [HttpGet("search/{PlayerName}")]
        // public IActionResult SearchForPlayer(string PlayerName, int mlbId)
        // {
        //     _helpers.StartMethod();
        //     // var PlayerName = selectedPlayer.PlayerName;
        //     C.WriteLine($"selectedPlayer: {PlayerName}");
        //     IEnumerable<SfbbPlayerBase> playerBases = _playerBaseFromGoogleSheet.GetAllPlayerBasesFromGoogleSheet("A7:AQ2333");

        //     C.WriteLine(playerBases.Count());

        //     // if (!string.IsNullOrEmpty(PlayerName))
        //     // {
        //     //     C.WriteLine("EMPTY");
        //     //     playerBases = playerBases.Where(player => player.PLAYERNAME == PlayerName);
        //     // }

        //     // rizzo id = "519203";
        //     // string mlbId = "519203";
        //     var onePlayersBase =
        //         from bases in playerBases
        //         where bases.MLBID == mlbId
        //         select bases;

        //     SfbbPlayer         = onePlayersBase.First();
        //     string firstName   = SfbbPlayer.FIRSTNAME;
        //     string lastName    = SfbbPlayer.LASTNAME;
        //     int? mlbPlayerId = SfbbPlayer.MLBID;
        //     C.WriteLine($"firstName: {firstName}\t lastName: {lastName}\t mlbPlayerId: {mlbPlayerId}");

        //     var hitter = _mlbDataSeasonHittingStatsController.CreateHitterSeasonStatsInstance("2019", (int)mlbPlayerId);


        //     Games            = hitter.Games;
        //     PlateAppearances = hitter.PlateAppearances;
        //     AtBats           = hitter.AtBats;
        //     Hits             = hitter.Hits;
        //     Runs             = hitter.Runs;
        //     HomeRuns         = hitter.HomeRuns;
        //     RBIs             = hitter.RBIs;
        //     SBs              = hitter.StolenBases;
        //     Walks            = hitter.Walks;
        //     AVG              = hitter.BattingAverage;

        //     // _launchCoreSpSitesController.LaunchAllPagesInChromeForPlayer(firstName, lastName);
        //     return View("dashboard", "Home");
        // }




        public void PrintSeasons()
        {
            PlayerSeasons.ForEach((season) => C.WriteLine($"season: {season}"));
        }

        private void PrintPlayerInfo(string playerFullName)
        {
            C.WriteLine($"SEARCHING FOR: {playerFullName}");
        }


        #endregion START ------------------------------------------------------------
    }
}



// <h4>Enter Player Name</h4>
//     <form asp-controller="PlayerSearch" asp-action="SearchForPlayer" method="GET">
//     <div class="form-group">
//         @Html.LabelFor(x => x.PlayerName)
//         @Html.DropDownListFor(x => x.PlayerName, Model.PlayersEnumerable,"SELECT PLAYER", new { @class = "form-control"})
//     </div>
//     <input type="submit" value="Search" />
// </form>


        // <input asp-for="PlayerName"class="form-control"/>
        //     @Html.LabelFor(x => x.PlayerName)
        //     @Html.DropDownListFor(x => x.PlayerName, Model.PlayersEnumerable,"SELECT PLAYER", new { @class = "form-control"})




/*
<h4>Enter Player Name</h4>
<form asp-controller="PlayerSearch" asp-action="SearchForPlayer" method="POST">
    <div class="form-group">
            @Html.LabelFor(x => x.PlayerName)
            @Html.DropDownListFor(x => x.PlayerName, Model.PlayersEnumerable,"SELECT PLAYER", new { @class = "form-control", @type="text"})
    </div>
    <div class="form-group">
        <input type="submit" value="Search" />
    </div>
</form>
 */




/*

<h4>Enter Player Name</h4>
<form asp-controller="PlayerSearch" asp-action="SearchForPlayer" method="POST">
    <div class="form-group">
        <input type="text" list="players" asp-for="PlayerName"/>
        <datalist id="players" asp-for="PlayerName">
            @foreach(var item in Model.PlayersEnumerable)
            {
                <option>@Html.DisplayFor(modelItem => item.Value)</option>
            }
        </datalist>
    </div>
    <div class="form-group">
        <input type="submit" value="Search" />
    </div>
</form>

*/



/*
<h4>Enter Player Name</h4>
<form asp-controller="PlayerSearch" asp-action="SearchForPlayer" method="POST">
    <div class="form-group">
        <label asp-for="PlayerName" class="control-label"></label>
        <input list="players" asp-for="PlayerName"/>
        <datalist id="players">
            @foreach(var item in Model.PlayersEnumerable)
            {
                <option>@Html.DisplayFor(modelItem => item.Value)</option>
            }
        </datalist>
    </div>
    <div class="form-group">
        <input type="submit" value="Search" />
    </div>
</form>

 */
