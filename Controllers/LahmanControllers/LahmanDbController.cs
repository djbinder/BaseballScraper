using System.Collections.Generic;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Lahman;
using Microsoft.AspNetCore.Mvc;


#pragma warning disable CS0414, CS0219, IDE0051, IDE0059, CS1591, IDE0044
namespace BaseballScraper.LahmanControllers
{

    [Route("api/lahman/[controller]")]
    [ApiController]
    public class LahmanDbController: ControllerBase
    {
        private readonly Helpers _h     = new Helpers();





    }
}
