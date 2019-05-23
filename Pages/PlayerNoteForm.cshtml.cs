using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseballScraper.Pages
{
    public class PlayerNoteForm : PageModel
    {
        private readonly Helpers _h = new Helpers();

        public IActionResult OnGet()
        {
            _h.StartMethod();
            return Page();
        }
    }
}

// @model BaseballScraper.Models.PlayerNote
