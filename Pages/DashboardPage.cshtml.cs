using System;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseballScraper.Pages
{
    public class DashboardPage : PageModel
    {
        private readonly Helpers _helpers;

        public DashboardPage(Helpers helpers)
        {
            _helpers = helpers;
        }

        public IActionResult OnGet()
        {
            _helpers.StartMethod();
            Console.WriteLine("DASHBOARD PAGE .cshtml .cs");
            return Page();
        }
    }
}
