using System;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.EndPoints
{
    public class ProjectDirectoryEndPoints
    {
        // Name of the folder that all baseball data is in
        // * this is the relative path of the directory (i.e., at the project level)
        private string BaseballDataDirectory
        {
            get => "BaseballData/";
        }

        // TWO major paths within BaseballDataDirectory
        // 1) READ  > "01_READ"
        // 2) WRITE > "02_WRITE"


        // 1) READ  > "01_READ"
        #region READ DATA FOLDER  ------------------------------------------------------------

            private string READ_DirectoryName
            {
                get => "01_READ/";
            }


            // "BaseballData/01_READ/"
            private string READ_DirectoryRelativePath
            {
                get => $"{BaseballDataDirectory}{READ_DirectoryName}";
            }

        #endregion READ DATA FOLDER  ------------------------------------------------------------




        // 2) WRITE > "02_WRITE"
        #region WRITE DATA FOLDER  ------------------------------------------------------------

            /* --------------------------------------------------------------- */
            /* READ DIRECTORY BUILDING BLOCKS                                  */
            /* --------------------------------------------------------------- */

            // 02_WRITE/
            private string WRITE_DirectoryName
            {
                get => "02_WRITE/";
            }

            // BaseballData/02_WRITE/
            private string WRITE_DirectoryRelativePath
            {
                get => $"{BaseballDataDirectory}{WRITE_DirectoryName}";
            }

            private string HITTER_DirectoryName
            {
                get => "HITTERS/";
            }

            private string PITCHER_DirectoryName
            {
                get => "PITCHERS/";
            }

            private string ARCHIVE_DirectoryName
            {
                get => "_archive/";
            }


            // READ DIRECTORY SUB-DIRECTORIES
            // 1) PLAYER BASE
            // *  A) ARCHIVE
            // *  B) CRUNCH TIME
            // *  C) SFBB




            /* --------------------------------------------------------------- */
            /* PLAYER BASE                                                     */
            /* --------------------------------------------------------------- */


            // PlayerBase/
            private string PlayerBaseWriteDirectoryName
            {
                get => "PlayerBase/";
            }

            // BaseballData/02_WRITE/PlayerBase/
            private string PlayerBaseWriteDirectoryRelativePath
            {
                get => $"{WRITE_DirectoryRelativePath}{PlayerBaseWriteDirectoryName}";
            }


            /* ----->  PLAYER BASE : ARCHIVE <----- */

            public string PlayerBaseWriteArchiveDirectoryName
            {
                get => "_archive/";
            }

            // BaseballData/02_WRITE/PlayerBase/_archive/
            public string PlayerBaseWriteArchiveDirectoryRelativePath
            {

                get => $"{PlayerBaseWriteDirectoryRelativePath}{PlayerBaseWriteArchiveDirectoryName}";
            }


            /* ----->  PLAYER BASE : CRUNCH TIME <----- */


            // URL of the Crunch time source file
            // * Not defined by me
            public string CrunchTimePlayerBaseCsvSourceUrl
            {
                get => "http://crunchtimebaseball.com/master.csv";
            }

            private string CrunchTimeWriteDirectoryName
            {
                get => $"CRUNCH_TIME/";
            }

            // BaseballData/02_WRITE/PlayerBase/CRUNCH_TIME/
            public string CrunchTimeWriteDirectoryRelativePath
            {
                get => $"{PlayerBaseWriteDirectoryRelativePath}{CrunchTimeWriteDirectoryName}";
            }

            public string CrunchTimeReportFileBaseName
            {
                get => "CrunchTime_Csv";
            }


            /* ----->  PLAYER BASE : SFBB <----- */
            private string SfbbWriteDirectoryName
            {
                get => $"SFBB/";
            }

            // BaseballData/02_WRITE/PlayerBase/SFBB/
            public string SfbbWriteDirectoryRelativePath
            {
                get => $"{PlayerBaseWriteDirectoryRelativePath}{SfbbWriteDirectoryName}";
            }


            /* --------------------------------------------------------------- */
            /* BASEBALL HQ                                                     */
            /* --------------------------------------------------------------- */

            /* <----- DIRECTORIES / FOLDERS / PATHS -----> */

            private string BaseballHqWriteDirectoryName
            {
                get => "BASEBALL_HQ/";
            }

            // BaseballData/02_WRITE/BASEBALL_HQ/
            public string BaseballHqWriteRelativePath
            {
                get => $"{WRITE_DirectoryRelativePath}{BaseballHqWriteDirectoryName}";
            }

            // BaseballData/02_WRITE/BASEBALL_HQ/HITTERS/
            public string BaseballHqHitterWriteRelativePath
            {
                get => $"{BaseballHqWriteRelativePath}{HITTER_DirectoryName}";
            }

            // BaseballData/02_WRITE/BASEBALL_HQ/PITCHERS/
            public string BaseballHqPitcherWriteRelativePath
            {
                get => $"{BaseballHqWriteRelativePath}{PITCHER_DirectoryName}";
            }

            // BaseballData/02_WRITE/BASEBALL_HQ/_archive/
            public string BaseballHqArchiveRelativePath
            {
                get => $"{BaseballHqWriteRelativePath}{ARCHIVE_DirectoryName}";
            }


            /* <----- FILE NAME STRINGS -----> */


            public string BaseballHqHitterReportPrefix
            {
                get => "HqHitterReport_";
            }

            private string BaseballHqHitterYearToDateFileNameIdentifier
            {
                get => "YTD_";
            }

            // "HqHitterReport_YTD_"
            public string BaseballHqHitterYearToDateCsvFileNameBase
            {
                get => $"{BaseballHqHitterReportPrefix}{BaseballHqHitterYearToDateFileNameIdentifier}";
            }

            private string BaseballHqHitterRosProjectionsFileNameIdentifier
            {
                get => "PROJ_";
            }

            // "HqHitterReport_PROJ_"
            public string BaseballHqHitterRosProjectionsCsvFileNameBase
            {
                get => $"{BaseballHqHitterReportPrefix}{BaseballHqHitterRosProjectionsFileNameIdentifier}";
            }

            /* --------------------------------------------------------------- */
            /* BASEBALL SAVANT                                                 */
            /* --------------------------------------------------------------- */


            private string BaseballSavantWriteDirectoryName
            {
                get => "BASEBALL_SAVANT/";
            }

            // private string BaseballSavantHitterWriteDirectoryName
            // {
            //     get => "HITTERS/";
            // }

            // private string BaseballSavantPitcherWriteDirectoryName
            // {
            //     get => "PITCHERS/";
            // }

            // BaseballData/02_WRITE/BASEBALL_SAVANT/
            public string BaseballSavantWriteRelativePath
            {
                get => $"{WRITE_DirectoryRelativePath}{BaseballSavantWriteDirectoryName}";
            }

            // BaseballData/02_WRITE/BASEBALL_SAVANT/HITTERS/
            public string BaseballSavantHitterWriteRelativePath
            {
                get => $"{BaseballSavantWriteRelativePath}{HITTER_DirectoryName}";
            }

            // BaseballData/02_WRITE/BASEBALL_SAVANT/PITCHERS/
            public string BaseballSavantPitcherWriteRelativePath
            {
                get => $"{BaseballSavantWriteRelativePath}{PITCHER_DirectoryName}";
            }

            // BaseballData/02_WRITE/BASEBALL_SAVANT/_archive
            public string BaseballSavantArchiveDirectory
            {
                get => $"{BaseballSavantWriteRelativePath}{ARCHIVE_DirectoryName}";
            }



            /* --------------------------------------------------------------- */
            /* FANGRAPHS                                                       */
            /* --------------------------------------------------------------- */

            private string FanGraphsWriteDirectoryName
            {
                get => "FANGRAPHS/";
            }

            // BaseballData/02_WRITE/FANGRAPHS/
            public string FanGraphsWriteRelativePath
            {
                get => $"{WRITE_DirectoryRelativePath}{FanGraphsWriteDirectoryName}";
            }

            // BaseballData/02_WRITE/FANGRAPHS/HITTERS/
            public string FanGraphsHitterWriteRelativePath
            {
                get => $"{FanGraphsWriteRelativePath}{HITTER_DirectoryName}";
            }

            // BaseballData/02_WRITE/FANGRAPHS/PITCHERS/
            public string FanGraphsPitcherWriteRelativePath
            {
                get => $"{FanGraphsWriteRelativePath}{PITCHER_DirectoryName}";
            }

            // BaseballData/02_WRITE/FANGRAPHS/_archive/
            public string FanGraphsArchiveRelativePath
            {
                get => $"{FanGraphsWriteRelativePath}{ARCHIVE_DirectoryName}";
            }


        #endregion WRITE DATA FOLDER  ------------------------------------------------------------






        #region LOCAL PROGRAMS  ------------------------------------------------------------

            // Path to Chrome Executable on my machine
            public string GoogleChromePath
            {
                get => "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
            }


        #endregion LOCAL PROGRAMS  ------------------------------------------------------------







        public class FileManagerMethods
        {
            // STATUS [ August 13, 2019 ] : this works
            // * Appends data string that includes month, day, year to another string
            // * Helps when downloaded files initially have the same generic name
            // * This basically makes the file unique for the day it was downloaded
            public string TodaysDateString()
            {
                // _helpers.OpenMethod(1);
                string dateString = string.Empty;
                DateTime today    = DateTime.Now;

                string month      = today.Month.ToString();
                string day        = today.Day.ToString();
                string year       = today.Year.ToString();

                dateString        = $"{month}_{day}_{year}";
                return dateString;
            }


            // STATUS [ August 13, 2019 ] : this works
            // * Appends data string that includes month, day, year, minute, hour, second, to another string
            // * Helps when downloaded files initially have the same generic name
            // * This basically makes the file unique for the day it was downloaded
            public string TodaysDateStringComplex()
            {
                // _helpers.OpenMethod(1);
                string dateString = string.Empty;

                DateTime today    = DateTime.Now;
                    string todayString = today.ToString();
                    string month       = today.Month.ToString();
                    string day         = today.Day.ToString();
                    string year        = today.Year.ToString();
                    string minute      = today.Minute.ToString();
                    string hour        = today.Hour.ToString();
                    string second      = today.Second.ToString();

                dateString        = $"{month}_{day}_{year}_{hour}_{minute}_{second}";
                return dateString;
            }
        }



    }
}
