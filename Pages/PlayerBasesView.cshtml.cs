using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.Controllers.Player;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseballScraper.Pages
{
    public class PlayerBasesView : PageModel
    {
        private Helpers _h = new Helpers();
        private readonly PlayerBaseController _pBC = new PlayerBaseController();

        public PlayerBasesView() {}

        public IEnumerable<PlayerBase> ListOfPlayerBases { get; set; }

        public IActionResult OnGet()
        {
            _h.StartMethod();
            var listOfBases = _pBC.GetAllPlayerBasesForOneMlbTeam("Chicago Cubs");

            ListOfPlayerBases = listOfBases;

            return Page();
        }
    }
}