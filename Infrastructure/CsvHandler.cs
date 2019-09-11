using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Scripting.Utils;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using C = System.Console;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE0063, IDE0067, IDE1006
namespace BaseballScraper.Infrastructure
{
    public class CsvHandler
    {
        private readonly Helpers _helpers;

        private IEnumerable<dynamic> records;


        public CsvHandler(Helpers helpers)
        {
            _helpers = helpers;
        }

        public CsvHandler()
        {

        }


        // List of CSV tools:
        // * https://toolslick.com/generation/code/class-from-csv
        // * https://joshclose.github.io/CsvHelper/reading/#getting-all-records


        #region AUTOMATED CSV DOWNLOAD ------------------------------------------------------------

            // STATUS [ July 25, 2019 ] : this works
            // Go to Url, click link where CSV is located, CSV downloads
            // * Not sure if this works for any website I would want but it works with FanGraphs CSVs
            // * It sleeps at the end to give the file time to save to local downloads folder
            // See: https://www.c-sharpcorner.com/blogs/how-to-get-the-latest-file-from-a-folder-by-using-c-sharp
            // See: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-get-information-about-files-folders-and-drives
            public async Task ClickLinkToDownloadCsvFile(string url, string csvLinkCssSelector)
            {
                _helpers.OpenMethod(3);
                var options = new LaunchOptions { Headless = false };
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

                using (var browser = await Puppeteer.LaunchAsync(options))
                using (var page    = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url, 1000000);
                    await page.WaitForSelectorAsync(csvLinkCssSelector);
                    await page.ClickAsync(csvLinkCssSelector);
                    Thread.Sleep(5000);
                }
            }


            // STATUS [ July 25, 2019 ] : this works
            // * Find last updated file on folder
            // * Move a file from one folder to another on local machine
            // * Rename file when it is moved by appending a date string with year, month, day
            public void MoveCsvFileToFolder(string currentFilePath, string filePathToSaveCsv, string reportType, int month = 0, int year = 0, int day = 0)
            {
                _helpers.OpenMethod(1);
                string fileName      = string.Empty;
                string targetPath    = filePathToSaveCsv;

                if (Directory.Exists(currentFilePath))
                {
                    var fileInfo         = new DirectoryInfo(currentFilePath).GetFiles();
                    DateTime lastUpdated = DateTime.MinValue;

                    foreach(FileInfo file in fileInfo)
                    {
                        if(file.LastWriteTime > lastUpdated)
                        {
                            lastUpdated = file.LastWriteTime;
                            fileName = file.Name;
                        }
                    }

                    DateTime today = DateTime.Now;
                    if(year  == 0) { year  = today.Year;  }
                    if(month == 0) { month = today.Month; }
                    if(day   == 0) { day   = today.Day;   }

                    string sourceFile = Path.Combine(currentFilePath, fileName);
                    fileName          = $"{reportType}_{month}_{day}_{year}.csv";
                    string destFile   = Path.Combine(targetPath, fileName);

                    File.Copy(sourceFile, destFile, true);
                }
                else
                {
                    C.WriteLine("Source path does not exist!");
                }
            }


            // STATUS [ August 13, 2019 ] : this works
            public string GetPathToLastUpdatedFileInFolder(string folderPath)
            {
                _helpers.OpenMethod(1);
                string fileName = string.Empty;
                if (Directory.Exists(folderPath))
                {
                    var fileInfo = new DirectoryInfo(folderPath).GetFiles();
                    DateTime lastUpdated = DateTime.MinValue;

                    DateTime createTime = new DateTime();
                    DateTime lastAccessTime = new DateTime();

                    foreach(FileInfo file in fileInfo)
                    {
                        createTime = file.CreationTime;
                        lastAccessTime = file.LastAccessTime;

                        if(file.LastAccessTime > lastUpdated)
                        {
                            lastUpdated = file.LastAccessTime;
                            fileName = file.Name;
                        }
                    }
                }
                string filePath = Path.Combine(folderPath, fileName);
                return filePath;
            }


            /// <summary></summary>
            public void MoveMultipleFiles(string sourceDirectoryPath, string destinationDirectoryPath)
            {
                _helpers.OpenMethod(1);
                int countFilesMoved = 1;

                if(Directory.Exists(sourceDirectoryPath))
                {
                    var fileInfo = new DirectoryInfo(sourceDirectoryPath).GetFiles();

                    foreach(FileInfo file in fileInfo)
                    {
                        string fileName = file.Name;

                        var sourceDirectoryPathAndFileName = $"{sourceDirectoryPath}{fileName}";
                        var destinationDirectoryPathAndFileName = $"{destinationDirectoryPath}{fileName}";

                        if(File.Exists(destinationDirectoryPathAndFileName))
                        {
                            C.WriteLine("File Exists. Replacing File");
                            string backupFileName = $"{destinationDirectoryPathAndFileName}_";
                            File.Replace(
                                sourceDirectoryPathAndFileName,
                                destinationDirectoryPathAndFileName,
                                backupFileName
                            );
                        }

                        else
                        {
                            C.WriteLine("File does NOT exist. Moving File");
                            File.Move(
                                sourceDirectoryPathAndFileName,
                                destinationDirectoryPathAndFileName
                            );
                        }

                        countFilesMoved++;
                    }
                }

                else
                {
                    C.WriteLine("File Path Does Not Exist. Creating Path: ");
                    C.WriteLine(sourceDirectoryPath);
                    Directory.CreateDirectory(sourceDirectoryPath);
                }
                C.WriteLine($"\nMOVED {countFilesMoved} FILES\n");
            }


            // STATUS [ August 13, 2019 ] : this works
            public string FindFileInFolder(string fileName, string folderPath)
            {
                _helpers.OpenMethod(1);
                string filePath = string.Empty;
                if (Directory.Exists(folderPath))
                    filePath = Path.Combine(folderPath, fileName);

                return filePath;
            }

            // STATUS [ August 13, 2019 ] : this works
            public void MoveFile(string fullSourceFilePath, string fullDestinationFilePath)
            {
                _helpers.OpenMethod(1);
                File.Move(fullSourceFilePath, fullDestinationFilePath);
            }

            // STATUS [ August 13, 2019 ] : this works
            public void ReplaceFile(string fullSourceFilePath, string fullDestinationFilePath, string backupFileName)
            {
                _helpers.OpenMethod(1);
                File.Replace(fullSourceFilePath, fullDestinationFilePath, backupFileName);
            }


            public bool CheckIfFileExists(string filePath)
            {
                _helpers.OpenMethod(1);
                bool doesFileExist = false;

                if(File.Exists(filePath))
                    doesFileExist = true;

                return doesFileExist;
            }


        #endregion AUTOMATED CSV DOWNLOAD ------------------------------------------------------------





        #region DOWNLOAD CSV ------------------------------------------------------------

            /// <summary>
            ///     Download remote CSV and save it to local location
            /// </summary>
            /// <param name="csvUrl">
            ///     The full url of where the csv is linked / hosted
            ///     Download CSV from this url
            /// </param>
            /// <param name="fullPathWithFileName">
            ///     The name of the file that you want to write to
            ///     Save CSV to location defined by 'targetFileName'
            /// </param>
            /// <example>
            ///     DownloadCsvFromLink("http://crunchtimebaseball.com/master.csv", "BaseballData/PlayerBase/CrunchtimePlayerBaseCsvAutoDownload.csv")
            /// </example>
            public void DownloadCsvFromLink(string csvUrl, string fullPathWithFileName)
            {
                _helpers.OpenMethod(1);
                WebClient webClient = new WebClient();
                {
                    webClient.DownloadFile(csvUrl, fullPathWithFileName);
                }
            }


            public JObject _appSettingsJson = JObject.Parse(File.ReadAllText("Configuration/appsettings.Development.json"));

            public string LocalDownloadsFolderLocation()
            {
                _helpers.OpenMethod(1);
                var downloadsFolderToken = _appSettingsJson["LocalComputerItems"]["DownloadsFolderLocation"];
                string downloadsPath     = downloadsFolderToken.ToString();
                return downloadsPath;
            }

        #endregion DOWNLOAD CSV ------------------------------------------------------------





        #region READ CSV ------------------------------------------------------------

            // STATUS: this works
            /// <summary>
            ///     Reads a csv file, non async
            /// </summary>
            /// <param name="csvFilePath"> The location / path of the file that you want to read </param>
            /// <param name="modelType">  </param>
            /// <param name="modelMapType"> </param>
            /// <example> _cH.ReadCsv("BaseballData/Lahman/Teams.csv"); </example>
            public IEnumerable<dynamic> ReadCsvRecords(string csvFilePath, Type modelType, Type modelMapType)
            {
                _helpers.OpenMethod(1);
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    var csvReader = new CsvReader( fileReader );

                    RegisterMapForClass(
                        csvReader,
                        modelMapType
                    );

                    csvReader.Read();
                    csvReader.ReadHeader();

                    IEnumerable<object> records = csvReader.GetRecords(modelType);
                    return records;
                }
            }


            public IList<object> ReadCsvRecordsToList(string csvFilePath, Type modelType, Type modelMapType)
            {
                _helpers.OpenMethod(1);
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    var csvReader = new CsvReader( fileReader );

                    RegisterMapForClass(
                        csvReader,
                        modelMapType
                    );

                    csvReader.Configuration.TrimOptions       = TrimOptions.Trim;
                    csvReader.Configuration.IgnoreBlankLines  = true;
                    csvReader.Configuration.MissingFieldFound = null;

                    csvReader.Read();
                    csvReader.ReadHeader();

                    var records = csvReader.GetRecords(modelType).ToList();
                    return records;
                }
            }


            // STATUS [ August 9, 2019 ] : this works
            // * Do not read a row in a csv
            // * Example: var preProcessedRecords = File.ReadLines(csvFileFullPath).Skip(1);
            public IEnumerable<string> ReadCsvLinesAndSkipRow(string csvFilePath, int skipRowInt)
            {
                IEnumerable<string> preProcessedRecords = File.ReadLines(csvFilePath).Skip(skipRowInt);
                return preProcessedRecords;
            }


            // STATUS [ August 13, 2019 ] : this works BUT needs to be DRYer
            // * As of right now, used by Baseball Hq Hitter controller
            // * When downloaded CSV from HQ, the first row isn't the headers we need; headers are in second row
            // * This method:
            // *    1) Makes sure files have .csv appendix (makes it idiot proof for me)
            // *    2) Creates a new file (i.e., appends "_" to original file name)
            // *    3) Copies all data except the first row
            // *    4) Pastes copied data to new file (so now headers are in first row)
            // *    5) Creates list of the object from csv
            // *    6) When reading records, ignores last row of data in csv (it's a disclaimer added by bb hq)
            public IList<object> ReadCsvRecordsToList(string csvFolderPath, string csvFileName, Type modelType, Type modelMapType, bool headersAreInFirstRow)
            {
                _helpers.OpenMethod(1);
                string csvFileFullPath      = string.Empty;
                string newCsvFileName       = string.Empty;
                string fileLocationFullPath = $"{csvFolderPath}{csvFileName}";

                /*  STEPS 1 & 2 */
                if(fileLocationFullPath.Contains("csv"))
                {
                    csvFileFullPath = $"{csvFolderPath}{csvFileName}";
                    newCsvFileName  = $"_{csvFileName}";
                }

                string updatedPath = $"{csvFolderPath}{newCsvFileName}";

                /*  STEP 3  */
                IEnumerable<string> preProcessedRecords = File.ReadLines(csvFileFullPath).Skip(1);

                /*  STEP 4  */
                WriteValuesAcrossRows(
                    updatedPath,
                    preProcessedRecords
                );

                /*  STEP 5  */
                using(TextReader fileReader = File.OpenText(updatedPath))
                {
                    var csvReader = new CsvReader( fileReader );
                    RegisterMapForClass(
                        csvReader,
                        modelMapType
                    );

                    /*  STEP 6  */
                    csvReader.Configuration.ShouldSkipRecord = row =>
                    {
                        return row[0].StartsWith("(");
                    };

                    csvReader.Read();
                    csvReader.ReadHeader();

                    var records = csvReader.GetRecords(modelType).ToList();
                    return records;
                }
            }


            // _h.EnumerateOverRecordsDynamic(records);
            public JObject ReadCsvRecordsToJObject(string csvFilePath, Type modelType, Type modelMapType)
            {
                _helpers.OpenMethod(1);
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    var csvReader = new CsvReader( fileReader );

                    RegisterMapForClass(
                        csvReader,
                        modelMapType
                    );

                    csvReader.Read();
                    csvReader.ReadHeader();

                    var records      = csvReader.GetRecords(modelType).ToList();

                    JObject jObject = new JObject
                    {
                        ["rows"] = JToken.FromObject(records)
                    };

                    return jObject;
                }
            }


            // STATUS: this works
            /// <summary>
            ///     Reads a csv file, async
            /// </summary>
            /// <remarks>
            ///     This does not enumerate over the records
            /// </remarks>
            /// <param name="csvFilePath">
            ///     The location / path of the file that you want to read
            /// </param>
            /// <param name="modelType">
            ///     The Lahman class / model that is in the csv file
            /// </param>
            /// <param name="modelMapType">
            ///     The map of the Lahman class / model that is in the csv file
            /// </param>
            /// <example>
            ///     await _cH.ReadCsvRecordsAsync(filePath, typeof(LahmanPeople), typeof(LahmanPeopleMap));
            /// </example>
            public async Task<IEnumerable<dynamic>> ReadCsvRecordsAsync(string csvFilePath, Type modelType, Type modelMapType)
            {
                _helpers.OpenMethod(3);

                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    CsvReader csvReader = new CsvReader( fileReader );

                    RegisterMapForClass(
                        csvReader,
                        modelMapType
                    );

                    csvReader.Configuration.DetectColumnCountChanges = true;

                    await csvReader.ReadAsync();
                    csvReader.ReadHeader();

                    // RECORDS type --> CsvHelper.CsvReader+<GetRecords>d__65
                    records = csvReader.GetRecords(modelType);
                }
                return records;
            }


            public async Task<IEnumerable<object>> ReadCsvRecordsAsyncToList(string csvFilePath, Type modelType, Type modelMapType, List<object> list)
            {
                _helpers.OpenMethod(3);
                bool doesFileExist = CheckIfFileExists(csvFilePath);

                PrintPathModelMap(csvFilePath, doesFileExist, modelType, modelMapType);

                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    CsvReader csvReader = new CsvReader( fileReader );

                    RegisterMapForClass(
                        csvReader,
                        modelMapType
                    );

                    csvReader.Configuration.DetectColumnCountChanges = true;

                    await csvReader.ReadAsync();
                    csvReader.ReadHeader();

                    // RECORDS type --> CsvHelper.CsvReader+<GetRecords>d__65
                    records = csvReader.GetRecords(modelType);

                    int counter = 1;
                    foreach(var record in records)
                    {
                        // C.WriteLine(record);
                        list.Add(record);
                        counter++;
                    }
                    // C.WriteLine($"COUNTER: {counter}");
                }
                return records;
            }


            // STATUS: this works
            /// <summary>
            ///     Register the map for the class within a csv you are trying read
            /// </summary>
            /// <remarks>
            ///     This is required any type you want to use a model map
            /// </remarks>
            /// <param name="csvReader">
            ///     A reader reading a csv file
            /// </param>
            /// <param name="modelType">
            ///     The class / model that is in the csv file
            /// </param>
            public void RegisterMapForClass(CsvReader csvReader, Type modelType)
            {
                // _helpers.OpenMethod(1);
                var mapClass = csvReader.Configuration.RegisterClassMap(modelType);
            }

        #endregion READ CSV ------------------------------------------------------------





        #region WRITE TO CSV ------------------------------------------------------------

            // STATUS [ August 13, 2019 ] : this works
            // * Writes records across rows in a CSV
            // * Each initial data row is one long string; It's comma-delimited but hasn't been split by "," yet
            // * So data only fills on column to start
            // * This splits each row by commas and spreads data across columns
            // * It's basically what Text-to-Columns is in Excel
            public void WriteValuesAcrossRows(string fullPathOfWriteFile, IEnumerable<string> recordsToWrite)
            {
                _helpers.OpenMethod(1);
                using(var stream = new MemoryStream())
                using(var writer = new StreamWriter(fullPathOfWriteFile))
                using(var reader = new StreamReader(stream))
                using(var csv    = new CsvWriter(writer))
                {
                    var preProcessedRecordsList = recordsToWrite.ToList();

                    foreach(var preProcessedRecordString in preProcessedRecordsList)
                    {
                        string[] splitValues = preProcessedRecordString.Split(",");
                        CleanQuotationMarksFromString(splitValues);
                        csv.WriteField(splitValues);
                        csv.NextRecord();
                    }

                    writer.Flush();
                    stream.Position = 0;
                }
            }

        #endregion WRITE TO CSV ------------------------------------------------------------





        #region CELLS ------------------------------------------------------------

            // STATUS [ July 8, 2019 ] : this works
            /// <summary>
            ///     Some cells have % symbol in them; this removes it and just gives the number back
            ///     e.g., 13.1% becomes 13.1
            ///     Helper method for: 'CreateFanGraphsHitterInstance(JToken allValuesInRow)' method
            /// </summary>
            /// <remarks>
            ///     See: https://stackoverflow.com/questions/2171615/how-to-convert-percentage-string-to-double
            /// </remarks>
            /// <example>
            ///     int numberOfPagesToScrape = await GetNumberOfPagesToScape(page);
            /// </example>
            public decimal ConvertCellWithPercentageSymbolToDecimal(JToken token)
            {
                var dataToConvert = token.ToString().Split('%');
                var decimalValue = decimal.Parse(dataToConvert[0]);
                return decimalValue;
            }

            public decimal ConvertCellWithPercentageSymbolToDecimal(string s)
            {
                var dataToConvert = s.Split('%');
                var decimalValue = decimal.Parse(dataToConvert[0]);
                return decimalValue;
            }

            public double ConvertCellWithPercentageSymbolToDouble(string s)
            {
                // _helpers.OpenMethod(1);
                double doubleValue = 0.00;

                // if the cell has data, parse the data
                try
                {
                    string[] dataToConvert = s.Split('%');
                    doubleValue = double.Parse(dataToConvert[0]);
                }

                // if the cell does not have data, error
                catch
                {
                    C.ForegroundColor = ConsoleColor.Red;
                    C.WriteLine($"\nERROR:");
                    C.ResetColor();
                    C.WriteLine($"Issue converting string to double - likely because no data in csv cell");
                    C.WriteLine($"In: CsvHandler > ConvertCellWithPercentageSymbolToDouble() Method\n");
                }
                return doubleValue;
            }


            public double? ParseNullableDouble(string val) => double.TryParse(val, out var i) ? (double?) i : null;
            public int? ParseNullableInt(string val) => int.TryParse(val, out var i) ? (int?) i : null;


            // STATUS [ August 9, 2019 ] : this works
            // If text in a cell has quotation marks around a string, this format removes them
            // * Quotation marks can mess with mapping header rows to model properties
            public void CleanQuotationMarksFromString(string[] values)
            {
                // _helpers.OpenMethod(1);
                foreach(var value in values)
                {
                    if(value.Contains("\""))
                    {
                        var cleanedValue = value.Replace("\"", "");
                        var indexOfValue = values.FindIndex(idx => idx == value);
                        values[indexOfValue] = cleanedValue;
                    }
                }
            }

        #endregion CELLS ------------------------------------------------------------





        #region HELPERS ------------------------------------------------------------


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


            // // STATUS [ September 1, 2019 ] : moved to FileManagerMethods
            // // STATUS [ August 13, 2019 ] : this works
            // // * Appends data string that includes month, day, year, minute, hour, second, to another string
            // // * Helps when downloaded files initially have the same generic name
            // // * This basically makes the file unique for the day it was downloaded
            // public string TodaysDateStringComplex()
            // {
            //     // _helpers.OpenMethod(1);
            //     string dateString = string.Empty;

            //     DateTime today    = DateTime.Now;
            //         string todayString = today.ToString();
            //         string month       = today.Month.ToString();
            //         string day         = today.Day.ToString();
            //         string year        = today.Year.ToString();
            //         string minute      = today.Minute.ToString();
            //         string hour        = today.Hour.ToString();
            //         string second      = today.Second.ToString();

            //     dateString        = $"{month}_{day}_{year}_{hour}_{minute}_{second}";
            //     return dateString;
            // }


            // STATUS: this works
            public string GetFieldNameByIndex(CsvReader csvReader, int indexNumber)
            {
                string fieldName = csvReader[indexNumber];
                return fieldName;
            }


            // STATUS: this works
            public int GetFieldPosition(CsvReader csvReader, int indexNumber)
            {
                var field = csvReader.GetField<int>(indexNumber);
                return field;
            }

        #endregion HELPERS ------------------------------------------------------------







        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintPathModelMap(string csvFilePath, bool doesFileExist, Type modelType, Type modelMapType)
            {
                C.WriteLine($"\n-------------------------------------------------------------------");
                _helpers.PrintNameSpaceControllerNameMethodName(typeof(CsvHandler));
                C.WriteLine($"CSV FILE PATH  : {csvFilePath}");
                C.WriteLine($"FILE EXISTS?   : {doesFileExist}");
                C.WriteLine($"MODEL TYPE     : {modelType}");
                C.WriteLine($"MODEL MAP TYPE : {modelMapType}");
                C.WriteLine($"-------------------------------------------------------------------\n");
            }


            public void PrintFileInfoDetails(FileInfo file)
            {
                C.WriteLine($"\nfileName: {file.FullName}");
                C.WriteLine($"Last Access: {file.LastAccessTime}");
                C.WriteLine($"Last Write: {file.LastWriteTime}\n");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------

    }
}



// 08.30.2019 - not sure if this is needed
// #region RECORDS ------------------------------------------------------------

//     // STATUS: this works
//     public void FilterRecordsByKey(JObject obj, string filterCriteria)
//     {
//         // KEY VALUE PAIR --> KeyValuePair<string, JToken> recordObject
//         foreach(var keyValuePair in obj)
//         {
//             var key = keyValuePair.Key;
//             if(key == filterCriteria)
//             {
//                 var value = keyValuePair.Value;
//                 C.WriteLine($"Value: {value}");
//             }
//         }
//     }

// #endregion RECORDS ------------------------------------------------------------
