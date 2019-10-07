using System;
using System.Globalization;
using System.IO;
using BaseballScraper.EndPoints;
using C = System.Console;
using D = System.Diagnostics.Debug;

namespace BaseballScraper.Infrastructure
{
    public class FileHandler
    {
        private readonly Helpers _helpers;
        private readonly ProjectDirectoryEndPoints _projectDirectoryEndPoints;

        public FileHandler(Helpers helpers, ProjectDirectoryEndPoints projectDirectoryEndPoints)
        {
            _helpers = helpers;
            _projectDirectoryEndPoints = projectDirectoryEndPoints;
        }


        public void BuildBaseballDataProjectDirectories()
        {
            // Top-level directores
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.BaseballDataDirectory);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.SEED_DirectoryRelativePath);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.READ_DirectoryRelativePath);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.WRITE_DirectoryRelativePath);

            // Player Base
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.PlayerBaseWriteArchiveDirectoryRelativePath);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.CrunchTimeWriteDirectoryRelativePath);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.SfbbWriteDirectoryRelativePath);

            // Baseball HQ
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.BaseballHqArchiveRelativePath);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.BaseballHqHitterWriteRelativePath);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.BaseballHqPitcherWriteRelativePath);

            // Baseball Savant
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.BaseballSavantArchiveDirectory);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.BaseballSavantHitterWriteRelativePath);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.BaseballSavantPitcherWriteRelativePath);

            // Baseball Savant
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.FanGraphsArchiveRelativePath);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.FanGraphsHitterWriteRelativePath);
            CreateDirectoryIfItDoesNotExist(_projectDirectoryEndPoints.FanGraphsPitcherWriteRelativePath);
        }


        public void CreateDirectoryIfItDoesNotExist(string directoryPath)
        {
            if(!Directory.Exists(directoryPath))
            {
                C.WriteLine($"Creating '{directoryPath}' Directory");
                Directory.CreateDirectory(directoryPath);
            }
        }


        public bool CheckIfFileExists(string filePath)
        {
            _helpers.OpenMethod(1);
            bool doesFileExist = false;

            if(File.Exists(filePath))
                doesFileExist = true;

            return doesFileExist;
        }



        // STATUS [ August 13, 2019 ] : this works
        // * Appends date string that includes month, day, year to another string
        // * Helps when downloaded files initially have the same generic name
        // * This basically makes the file unique for the day it was downloaded
        // * Example : 08_01_2019
        public string TodaysDateString()
        {
            DateTime today    = DateTime.Now;

            string month      = today.Month.ToString(CultureInfo.InvariantCulture);
            string day        = today.Day.ToString(CultureInfo.InvariantCulture);
            string year       = today.Year.ToString(CultureInfo.InvariantCulture);

            // _helpers.OpenMethod(1);
            string dateString = $"{month}_{day}_{year}";
            return dateString;
        }


        // STATUS [ August 13, 2019 ] : this works
        // * Appends data string that includes month, day, year, minute, hour, second, to another string
        // * Helps when downloaded files initially have the same generic name
        // * This basically makes the file unique for the day it was downloaded
        public string TodaysDateStringComplex()
        {
            DateTime today    = DateTime.Now;
                    _ = today.ToString(CultureInfo.InvariantCulture);
                    string month       = today.Month.ToString(CultureInfo.InvariantCulture);
                    string day         = today.Day.ToString(CultureInfo.InvariantCulture);
                    string year        = today.Year.ToString(CultureInfo.InvariantCulture);
                    string minute      = today.Minute.ToString(CultureInfo.InvariantCulture);
                    string hour        = today.Hour.ToString(CultureInfo.InvariantCulture);
                    string second      = today.Second.ToString(CultureInfo.InvariantCulture);

            // _helpers.OpenMethod(1);
            string dateString = $"{month}_{day}_{year}_{hour}_{minute}_{second}";
            return dateString;
        }
    }
}