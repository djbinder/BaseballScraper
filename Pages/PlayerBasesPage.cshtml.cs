using System.Collections.Generic;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace BaseballScraper.Pages
{
    public class PlayerBasesPage : PageModel
    {
        private readonly Helpers _helpers;
        private readonly PlayerBaseController _playerBaseController;
        private readonly PlayerBaseController.PlayerBaseFromExcel _playerBaseFromExcel;

        public PlayerBasesPage(Helpers helpers, PlayerBaseController playerBaseController, PlayerBaseController.PlayerBaseFromExcel playerBaseFromExcel)
        {
            _helpers = helpers;
            _playerBaseController = playerBaseController;
            _playerBaseFromExcel = playerBaseFromExcel;
        }

        public IEnumerable<PlayerBase> ListOfPlayerBases { get; set; }

        public IActionResult OnGet()
        {
            _helpers.StartMethod();
            // var listOfBases = PlayerBaseController.PlayerBaseFromExcel.GetAllPlayerBasesForOneMlbTeam("Chicago Cubs");
            // ListOfPlayerBases = listOfBases;

            return Page();
        }
    }
}
