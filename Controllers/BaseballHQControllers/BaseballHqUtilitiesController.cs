﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.ConfigurationModels;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using C = System.Console;


#pragma warning disable CC0091, CS1998, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.BaseballHQControllers
{
    public class BaseballHqUtilitiesController
    {
        private readonly Helpers               _helpers;
        private readonly CsvHandler            _csvHandler;
        private readonly BaseballHqCredentials _hqCredentials;


        public BaseballHqUtilitiesController(Helpers helpers, CsvHandler csvHandler, IOptions<BaseballHqCredentials> hqCredentials)
        {
            _helpers       = helpers;
            _csvHandler    = csvHandler;
            _hqCredentials = hqCredentials.Value;
        }

        public BaseballHqUtilitiesController(){}


        // Used to open local instance of chrome instead of Chromium
        // Defined by me
        private readonly string _chromePath = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";

        // Url where all Hq Reports are located
        // Defined by HQ
        private readonly string _hqReportsUrl = "https://www.baseballhq.com/members/tools/player_stats_proj";

        // Defined by me
        public string _downloadsFolderPath { get => _csvHandler.LocalDownloadsFolderLocation(); }




        /* -------------------- CONTROLLER OVERVIEW -------------------- */
        //
        // NOTES [ August 13, 2019 ]:
        // * Should be used by other HQ controllers
        // * As of 08.13.2019 used in BaseballHqHitterController
        // * Some of the below should be moved to a more appropriate utility



        #region DOWNLOAD HQ REPORT ------------------------------------------------------------

        // STATUS [ August 8, 2019 ] : this works
        // STEP 1: Create chrome page for other methods to use
        // * Launches browser (i.e., Headless = false)
        // * Uses local chrome instance instead of Chromium (i.e., ExecutablePath =_chromePath)
        // * Should probably be moved to some kind of file manager utility / infrastructure
        public async Task<Page> CreateChromePageAsync()
        {
            LaunchOptions options = new LaunchOptions { Headless = false, ExecutablePath =_chromePath };
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision).ConfigureAwait(false);

            Browser browser = await Puppeteer.LaunchAsync(options).ConfigureAwait(false);
            Page page    = await browser.NewPageAsync().ConfigureAwait(false);

            return page;
        }


        // STATUS [ August 8, 2019 ] : this works
        // STEP 2: Automate logging in to Baseball HQ
        // * Login page: https://www.baseballhq.com/user
        // * Selectors are all from login pages html / css
        public async Task LoginToBaseballHqAsync(Page page)
        {
            const string userNameSelector    = "#edit-name";
            const string passwordSelector    = "#edit-pass";
            const string loginButtonSelector = "#edit-submit";

            string hqUserName   = _hqCredentials.UserName;
            string hqPassword   = _hqCredentials.Password;
            string loginPageUrl = _hqCredentials.LoginLink;

            await page.GoToAsync(loginPageUrl, 0).ConfigureAwait(false);
            await WaitForSelectorClickThenTypeAsync(page, userNameSelector, hqUserName).ConfigureAwait(false); // click user name box and enter user name
            await WaitForSelectorClickThenTypeAsync(page, passwordSelector, hqPassword).ConfigureAwait(false); // click password box and enter password
            await WaitForSelectorThenClickAsync(page, loginButtonSelector).ConfigureAwait(false);              // click login button
        }


        // STATUS [ August 8, 2019 ] : this works
        // STEP 3: Go to report page and download specified report to local downloads folder
        // * Thread.Sleep is needed to give the file time to get to downloads folder
        // * If you don't wait, it might not get there in time for the MoveReportToHqFolder method
        // * Hq Reports Url: https://bit.ly/2KnC7Rh
        // * Should probably be moved to some kind of file manager utility / infrastructure
        public async Task DownloadHqReportAsync(Page page, string reportSelector)
        {
            await page.GoToAsync(_hqReportsUrl).ConfigureAwait(false);
            await page.ClickAsync(reportSelector).ConfigureAwait(false);
            await Task.Delay(5000).ConfigureAwait(false);
        }


        // STATUS [ August 8, 2019 ] : this works
        // STEP 4: Move report from local downloads to Hq report folder
        // * Hq Reports Url: https://bit.ly/2KnC7Rh
        // * bool is an option to open the file once done (e.g., open CSV in Excel)
        // * Note: not really sure what the catch is doing here
        public async Task MoveReportToHqFolderAsync(bool openFile, string downloadedCsvFileName, string targetWriteFolderPath, string filePrefix)
        {
            string destFilePath   = string.Empty;
            string sourceFilePath = _csvHandler.FindFileInFolder(downloadedCsvFileName, _downloadsFolderPath);
            string todayString    = _csvHandler.TodaysDateString();

            destFilePath   = $"{targetWriteFolderPath}{filePrefix}{todayString}.csv";

            if(System.IO.File.Exists(sourceFilePath))
            {
                try
                {
                    _csvHandler.MoveFile(sourceFilePath, destFilePath);
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    _csvHandler.ReplaceFile(sourceFilePath, destFilePath, "BLAH.csv");
                }

                if (openFile)
                {
                    OpenFile(destFilePath);
                }
            }
            else
            {
                _csvHandler.MoveFile(sourceFilePath, destFilePath);
            }
        }


        #endregion DOWNLOAD HQ REPORT ------------------------------------------------------------





        #region HELPERS ------------------------------------------------------------

        // STATUS [ August 8, 2019 ] : this works
        // * Click html form box and enter data (used to automate Hq UserName and Password)
        // * Should probably be moved to some kind of file manager utility / infrastructure
        public async Task WaitForSelectorClickThenTypeAsync(Page page, string cssSelector, string valueToType)
        {
            await page.WaitForSelectorAsync(cssSelector).ConfigureAwait(false);
            await page.ClickAsync(cssSelector).ConfigureAwait(false);
            await page.Keyboard.TypeAsync(valueToType).ConfigureAwait(false);
        }


        // STATUS [ August 8, 2019 ] : this works
        // * Click html object (used to click login submit button)
        // * Should probably be moved to some kind of file manager utility / infrastructure
        public async Task WaitForSelectorThenClickAsync(Page page, string cssSelector)
        {
            await page.WaitForSelectorAsync(cssSelector).ConfigureAwait(false);
            await page.ClickAsync(cssSelector).ConfigureAwait(false);
        }


        // STATUS [ August 8, 2019 ] : this works
        // * Open the file in default program (e.g., Csv opens in Excel)
        // * Should probably be moved to some kind of file manager utility / infrastructure
        public void OpenFile(string fullFilePath)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = fullFilePath,
            };
            process.StartInfo = processStartInfo;
            process.Start();

            process.Dispose();
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
