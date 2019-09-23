using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

#pragma warning disable MA0016
namespace BaseballScraper.Models.ViewModels
{
    public class PlayerSearchViewModel
    {
        [BindProperty(SupportsGet = true)]
        public string PlayerName { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<SelectListItem> PlayersList { get; }

        [BindProperty(SupportsGet = true)]
        public IEnumerable<SelectListItem> PlayersEnumerable { get; set; }

    }

}
