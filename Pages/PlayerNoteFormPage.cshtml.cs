using System;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseballScraper.Pages
{
    public class PlayerNoteFormPage : PageModel
    {
        private readonly Helpers _helpers;
        private readonly PlayerNoteController _playerNoteController;

        public PlayerNoteFormPage(Helpers helpers, PlayerNoteController playerNoteController)
        {
            _helpers = helpers;
            _playerNoteController = playerNoteController;
        }

        public IActionResult OnGet()
        {
            _helpers.StartMethod();
            Console.WriteLine("PLAYER NOTE FORM PAGE .cshtml .cs");
            // _playerNoteController.CreateNote();

            return Page();
        }
    }
}
