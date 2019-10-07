using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.BaseballMonster
{
    [Route("api/baseballmonster/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BaseballMonsterDailyController : ControllerBase
    {
        private readonly Helpers helpers;


        public BaseballMonsterDailyController(Helpers helpers)
        {
            this.helpers = helpers;
        }

        public BaseballMonsterDailyController(){}



        /*
            https://127.0.0.1:5001/api/baseballmonster/baseballmonsterdaily
        */
        [HttpGet("daily")]
        public void TestBbMonsterDailyController()
        {
            helpers.StartMethod();
        }


        /*
            https://127.0.0.1:5001/api/baseballmonster/baseballmonsterdaily/daily/async
        */
        [HttpGet("daily/async")]
        public async Task TestBbMonsterDailyControllerAsync()
        {
            helpers.StartMethod();
            await DownloadLiveDFSResultsAsync();
        }


        private readonly string _dfsResultsUrl = "https://baseballmonster.com/dfsdailysummary.aspx";


        // STATUS [ July 8, 2019 ] : DOES NOT WORK
        // trying to automatically click button on page to download results Excel file
        public async Task DownloadLiveDFSResultsAsync()
        {
            helpers.StartMethod();
            var options = new LaunchOptions { Headless = true };
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(_dfsResultsUrl);

                const string buttonSelector = ".button-green";
                await page.WaitForSelectorAsync(buttonSelector);
                await page.ClickAsync(buttonSelector);
            }
            helpers.CompleteMethod();
        }

    }
}
