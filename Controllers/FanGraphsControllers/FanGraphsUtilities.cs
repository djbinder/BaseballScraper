using System;
using System.IO;
using System.Threading.Tasks;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using C = System.Console;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.FanGraphsControllers
{
    public class FanGraphsUtilities
    {
        private readonly Helpers _helpers;

        private readonly FanGraphsUriEndPoints _endPoints;

        private readonly CsvHandler _csvHandler;

        public FanGraphsUtilities(Helpers helpers, FanGraphsUriEndPoints fanGraphsUriEndPoints, CsvHandler csvHandler)
        {
            _csvHandler = csvHandler;
            _endPoints = fanGraphsUriEndPoints;
            _helpers = helpers;
        }



        #region FANGRAPHS CSV ------------------------------------------------------------

        // 1) Check if CSV Exists
        // 2) Download CSV to Local Downloads folder
        // 3) Move CSV to project folder
        // 4) Get List of pitchers from CSV



        /* --------------------------------------------------------------- */
        /* CURRENT SEASON / IN-SEASON                                      */
        /* --------------------------------------------------------------- */

        // STATUS [ July 31, 2019 ] : this works
        // STEP 0: Check if there is already a Csv file created for today
        // * Returns 'false' if file doesn't exist; returns 'true' if it does
        // * If the file already exists, you do not need to run the report again
        public bool CheckIfCsvFileForTodayExists(string directoryToSearchForFile, string reportPrefix)
        {
            _helpers.OpenMethod(1);

            FileInfo[] fileInfo = new DirectoryInfo(directoryToSearchForFile).GetFiles();

            DateTime today = DateTime.Now;
            int year       = today.Year;
            int month      = today.Month;
            int day        = today.Day;

            string fileName = $"{reportPrefix}_{month}_{day}_{year}.csv";

            bool doesCsvReportExistForToday = false;

            foreach(FileInfo file in fileInfo)
            {
                if(string.Equals(file.Name, fileName, StringComparison.Ordinal))
                    doesCsvReportExistForToday = true;
            }
            return doesCsvReportExistForToday;
        }


        // STATUS [ July 31, 2019 ] : this works
        // STEP 1: set report parameters and download CSV to local downloads folder
        // * Goes to a FanGraphs page and downloads the CSV from that page to local downloads folder
        // * Ultimately the CSV should be moved to project data folder (See MoveCsvToProjectFolder method)
        // * This should not run if a Csv file for current day already exists
        public async Task DownloadCsvToLocalDownloadsAsync(string endPoint)
        {
            _helpers.OpenMethod(3);

            string csvSelector = _endPoints.FanGraphsCsvHtmlSelector();
            await _csvHandler.ClickLinkToDownloadCsvFileAsync(endPoint, csvSelector).ConfigureAwait(false);
        }


        // STATUS [ July 31, 2019 ] : this works
        // STEP 2: move the CSV from local downloads folder to project Target_Write folder
        // * Looks for the file that was last updated in local downloads
        // * This should not run if a Csv file for current day already exists
        // * Once it finds last updated file, it moves and renames the file
        // * Ends up something like: "SpWpdiReport_07_09_2019.csv"
        public void MoveCsvToProjectFolder(string filePathToSaveCsv, string fileNamePrefix, int reportMonth = 0, int reportYear  = 0,int reportDay = 0)
        {
            _helpers.OpenMethod(1);
            string downloadsFolder   = _endPoints.LocalDownloadsFolderLocation();

            _csvHandler.MoveCsvFileToFolder(
                downloadsFolder,
                filePathToSaveCsv,
                fileNamePrefix,
                month:reportMonth,
                year:reportYear,
                day:reportDay
            );
            PrintCsvMoveInfo(downloadsFolder, filePathToSaveCsv, fileNamePrefix, reportMonth, reportDay, reportYear);
        }



        #endregion FANGRAPHS CSV ------------------------------------------------------------




        #region PRINTING PRESS ------------------------------------------------------------

        private void PrintCsvMoveInfo(string movingFromDirectory, string movingToDirectory, string fileNamePrefix, int reportMonth, int reportDay, int reportYear)
        {
            C.WriteLine($"\n--------------------------------------------");
            _helpers.PrintNameSpaceControllerNameMethodName(typeof(FanGraphsSpController));
            C.WriteLine($"MOVING FROM : {movingFromDirectory}");
            C.WriteLine($"MOVING TO   : {movingToDirectory}");
            C.WriteLine($"PREFIX      : {fileNamePrefix}");
            C.WriteLine($"REPORT DATE : {reportMonth}.{reportDay}.{reportYear}");
            C.WriteLine($"--------------------------------------------\n");
        }

        #endregion PRINTING PRESS ------------------------------------------------------------

    }
}

