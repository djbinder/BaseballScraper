using System;
using System.Globalization;
using System.IO;
using BaseballScraper.Infrastructure;


#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.EndPoints
{
    public class ProjectDirectoryEndPoints
    {
        // Name of the folder that all baseball data is in
        // * this is the relative path of the directory (i.e., at the project level)
        public string BaseballDataDirectory
        {
            get => "BaseballData/";
        }

        public string SEED_DATA_DirectoryName => "00_SEED_DATA/";
        public string SEED_DirectoryRelativePath => $"{BaseballDataDirectory}{SEED_DATA_DirectoryName}";

        // TWO major paths within BaseballDataDirectory
        // 1) READ  > "01_READ"
        // 2) WRITE > "02_WRITE"


        // 1) READ  > "01_READ"
        #region READ DATA FOLDER  ------------------------------------------------------------


        private string READ_DirectoryName => "01_READ/";

        // "BaseballData/01_READ/"
        public string READ_DirectoryRelativePath => $"{BaseballDataDirectory}{READ_DirectoryName}";


        #endregion READ DATA FOLDER  ------------------------------------------------------------




        /* ---------------------------------------------------------------------------------- */
        /*  PATH 2) WRITE > "02_WRITE"                                                        */
        /* ---------------------------------------------------------------------------------- */
        #region WRITE DATA FOLDER  ------------------------------------------------------------

        /* --------------------------------------------------------------- */
        /* WRITE DIRECTORY BUILDING BLOCKS                                 */
        /* --------------------------------------------------------------- */


        // 02_WRITE/
        private string WRITE_DirectoryName => "02_WRITE/";

        // BaseballData/02_WRITE/
        public string WRITE_DirectoryRelativePath => $"{BaseballDataDirectory}{WRITE_DirectoryName}";

        private string HITTER_DirectoryName => "HITTERS/";

        private string PITCHER_DirectoryName => "PITCHERS/";

        private string ARCHIVE_DirectoryName => "_archive/";


        /* --------------------------------------------------------------- */
        /* PLAYER BASE                                                     */
        /* --------------------------------------------------------------- */

        /* ----->  PLAYER BASE : SECTION OVERVIEW  <----- */
            // 1) PLAYER BASE
            // *  A) ARCHIVE
            // *  B) CRUNCH TIME
            // *  C) SFBB


        /* ----->  PLAYER BASE : BUILDING BLOCKS  <----- */

            // PLAYER_BASE/
            private string PlayerBaseWriteDirectoryName
            {
                get => "PLAYER_BASE/";
            }

            // BaseballData/02_WRITE/PLAYER_BASE/
            private string PlayerBaseWriteDirectoryRelativePath
            {
                get => $"{WRITE_DirectoryRelativePath}{PlayerBaseWriteDirectoryName}";
            }


        /* ----->  PLAYER BASE : ARCHIVE <----- */

            // BaseballData/02_WRITE/PLAYER_BASE/_archive/
            public string PlayerBaseWriteArchiveDirectoryRelativePath
            {
                get => $"{PlayerBaseWriteDirectoryRelativePath}{ARCHIVE_DirectoryName}";
            }


        /* ----->  PLAYER BASE : CRUNCH TIME <----- */

            // * URL of the Crunch time source file
            // * Not defined by me
            public string CrunchTimePlayerBaseCsvSourceUrl
            {
                get => "http://crunchtimebaseball.com/master.csv";
            }

            private string CrunchTimeWriteDirectoryName
            {
                get => $"CRUNCH_TIME/";
            }

            // = "BaseballData/02_WRITE/PLAYER_BASE/CRUNCH_TIME/"
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

            // BaseballData/02_WRITE/PLAYER_BASE/SFBB/
            public string SfbbWriteDirectoryRelativePath
            {
                get => $"{PlayerBaseWriteDirectoryRelativePath}{SfbbWriteDirectoryName}";
            }


        /* --------------------------------------------------------------- */
        /* BASEBALL HQ                                                     */
        /* --------------------------------------------------------------- */

        /* <----- HQ : DIRECTORIES / FOLDERS / PATHS -----> */

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


        /* <----- HQ : FILE NAME STRINGS -----> */

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












    }
}
