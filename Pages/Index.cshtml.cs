using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.Controllers.Player;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaseballScraper.Pages
{
    public class IndexModel : PageModel
    {
        private Helpers _h = new Helpers();
        private readonly PlayerBaseController _pBC = new PlayerBaseController();
        public void OnGet()
        {
            _h.StartMethod();

        }
    }
}
