using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Configuration;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using C = System.Console;


#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.BaseballHQControllers
{
    public class BaseballHqUtilitiesController
    {
        private readonly Helpers               _helpers;
        private readonly CsvHandler            _csvHandler;
        private readonly BaseballHqCredentials _hqCredentials;


        public BaseballHqUtilitiesController(Helpers helpers, CsvHandler csvHandler, IOptions<BaseballHqCredentials> hqCredentials)
        {
            _helpers    = helpers;
            _csvHandler = csvHandler;
            _hqCredentials = hqCredentials.Value;
        }

        public BaseballHqUtilitiesController(){}



        // Used to open local instance of chrome instead of Chromium
        private readonly string _chromePath = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";

        // Url where all Hq Reports are located
        private readonly string _hqReportsUrl = "https://www.baseballhq.com/members/tools/player_stats_proj";

        // What Hq / Chrome name the file when it's downloaded to local downloads folder
        // private readonly string _downloadedCsvName = "BaseballHQ_M_B_PY.csv";

        // // Folder where we want the Csv downloaded the end up
        // public readonly string _hqTargetWriteFolderPath = "BaseballData/02_Target_Write/BaseballHQ_Target_Write/";


        public string _downloadsFolderPath
        {
            get
            {
                return _csvHandler.LocalDownloadsFolderLocation();
            }
        }



        // STATUS [ August 8, 2019 ] : this works
        // STEP 1: Create chrome page for other methods to use
        // * Launches browser (i.e., Headless = false)
        // * Uses local chrome instance instead of Chromium (i.e., ExecutablePath =_chromePath)
        public async Task<Page> CreateChromePage()
        {
            LaunchOptions options = new LaunchOptions { Headless = false, ExecutablePath =_chromePath };
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            var browser = await Puppeteer.LaunchAsync(options);
            var page    = await browser.NewPageAsync();

            return page;
        }


        // STATUS [ August 8, 2019 ] : this works
        // STEP 2: Automate logging in to Baseball HQ
        // * Login page: https://www.baseballhq.com/user
        // * Selectors are all from login pages html / css
        public async Task LoginToBaseballHq(Page page)
        {
            string userNameSelector    = "#edit-name";
            string passwordSelector    = "#edit-pass";
            string loginButtonSelector = "#edit-submit";

            string hqUserName   = _hqCredentials.UserName;
            string hqPassword   = _hqCredentials.Password;
            string loginPageUrl = _hqCredentials.LoginLink;

            await page.GoToAsync(loginPageUrl, 0);
            await WaitForSelectorClickThenType(page, userNameSelector, hqUserName); // click user name box and enter user name
            await WaitForSelectorClickThenType(page, passwordSelector, hqPassword); // click password box and enter password
            await WaitForSelectorThenClick(page, loginButtonSelector);              // click login button
        }


        // STATUS [ August 8, 2019 ] : this works
        // STEP 3: Go to report page and download specified report to local downloads folder
        // * Thread.Sleep is needed to give the file time to get to downloads folder
        // * If you don't wait, it might not get there in time for the MoveReportToHqFolder method
        public async Task DownloadHqReport(Page page, string reportSelector)
        {
            await page.GoToAsync(_hqReportsUrl);
            await page.ClickAsync(reportSelector);
            Thread.Sleep(5000);
        }


        // STATUS [ August 8, 2019 ] : this works
        // STEP 4: Move report from local downloads to Hq report folder
        // * Hq Reports Url: https://bit.ly/2KnC7Rh
        // * bool is an option to open the file once done
        public async Task MoveReportToHqFolder(bool openFile, string downloadedCsvFileName, string targetWriteFolderPath, string filePrefix)
        {
            string destFilePath = string.Empty;

            string sourceFilePath    = _csvHandler.FindFileInFolder(downloadedCsvFileName, _downloadsFolderPath);

            string todayString = _csvHandler.TodaysDateString();

            C.WriteLine(destFilePath,sourceFilePath,todayString);

            destFilePath   = $"{targetWriteFolderPath}{filePrefix}{todayString}.csv";
            C.WriteLine($"destFilePath: {destFilePath}");

            if(System.IO.File.Exists(sourceFilePath))
            {
                try
                {
                    _csvHandler.MoveFile(sourceFilePath, destFilePath);
                    C.WriteLine("HqUtilitiesController > MoveReportToHqFolder() > MOVING FILE");
                }

                catch
                {
                    _csvHandler.ReplaceFile(sourceFilePath, destFilePath, "BLAH.csv");

                    C.WriteLine("HqUtilitiesController > MoveReportToHqFolder() > REPLACING FILE");
                }

                if(openFile == true)
                {
                    OpenFile(destFilePath);
                }
            }

            else
            {
                _csvHandler.MoveFile(sourceFilePath, destFilePath);
                C.WriteLine("HqUtilitiesController > MoveReportToHqFolder() > FILE DOES NOT EXIST");
            }

        }




        #region HELPERS ------------------------------------------------------------

            // STATUS [ August 8, 2019 ] : this works
            // Click html form box and enter data (used to automate Hq UserName and Password)
            public async Task WaitForSelectorClickThenType(Page page, string cssSelector, string valueToType)
            {
                await page.WaitForSelectorAsync(cssSelector);
                await page.ClickAsync(cssSelector);
                await page.Keyboard.TypeAsync(valueToType);
            }


            // STATUS [ August 8, 2019 ] : this works
            // Click html object (used to click login submit button)
            public async Task WaitForSelectorThenClick(Page page, string cssSelector)
            {
                await page.WaitForSelectorAsync(cssSelector);
                await page.ClickAsync(cssSelector);
            }


            // STATUS [ August 8, 2019 ] : this works
            // Open the file in default program (e.g., Csv opens in Excel)
            public void OpenFile(string fullFilePath)
            {
                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = fullFilePath
                };
                process.StartInfo = processStartInfo;
                process.Start();
            }


            public void HoldProcess()
            {
                C.WriteLine("press any key to continue");
                C.ReadLine();
            }

        #endregion HELPERS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintFileMoveDetails(string sourceFile, string destFile)
            {
                C.WriteLine($"\nMOVING --> {sourceFile}\nTO --> {destFile}");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
