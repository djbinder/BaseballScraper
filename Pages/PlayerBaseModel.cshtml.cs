using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.Controllers.Player;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseballScraper.Pages
{
    public class PlayerBaseModel : PageModel
    {
        private Helpers _h = new Helpers();
        private readonly PlayerBaseController _pBC = new PlayerBaseController();

        public PlayerBaseModel() {}

        public IList<PlayerBase> PlayerBases { get; set; }

        public void OnGet()
        {
            _h.StartMethod();
            var start = _pBC.GetAllPlayerBasesForOneMlbTeam("Chicago Cubs");

            PlayerBases = start.ToList();
        }
    }
}