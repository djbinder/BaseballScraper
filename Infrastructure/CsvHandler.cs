using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Scripting.Utils;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Infrastructure
{
    /// <summary> </summary>
    /// <remarks>
    ///     See: https://joshclose.github.io/CsvHelper/reading/#getting-all-records
    ///     See: https://toolslick.com/generation/code/class-from-csv
    /// </remarks>

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



        #region AUTOMATED CSV DOWNLOAD ------------------------------------------------------------


            // STATUS [ July 25, 2019 ] : this works
            // Go to Url, click link where CSV is located, CSV downloads
            // * Not sure if this works for any website but it works with FanGraphs CSVs
            // See: https://www.c-sharpcorner.com/blogs/how-to-get-the-latest-file-from-a-folder-by-using-c-sharp
            // See: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-get-information-about-files-folders-and-drives
            public async Task ClickLinkToDownloadCsvFile(string url, string csvLinkCssSelector)
            {
                // Console.WriteLine($"\nCSV URL:\n{url}\n");
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
            /// <summary>
            ///     Move a file from one folder to another on local machine
            ///     Rename file when it is moved
            /// </summary>
            public void MoveCsvFileToFolder(string currentFilePath, string filePathToSaveCsv, string reportType, int month = 0, int year = 0, int day = 0)
            {
                string fileName      = string.Empty;
                string targetPath    = filePathToSaveCsv;

                if (Directory.Exists(currentFilePath))
                {
                    var fileInfo = new DirectoryInfo(currentFilePath).GetFiles();
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
                    Console.WriteLine("Source path does not exist!");
                }
            }


            public string TodaysDateString()
            {
                string dateString = string.Empty;
                DateTime today    = DateTime.Now;

                string month      = today.Month.ToString();
                string day        = today.Day.ToString();
                string year       = today.Year.ToString();

                dateString        = $"{month}_{day}_{year}";
                return dateString;
            }

            public string TodaysDateStringComplex()
            {
                string dateString = string.Empty;

                DateTime today    = DateTime.Now;
                    string todayString = today.ToString();
                    string month  = today.Month.ToString();
                    string day    = today.Day.ToString();
                    string year   = today.Year.ToString();
                    string minute = today.Minute.ToString();
                    string hour   = today.Hour.ToString();
                    string second = today.Second.ToString();

                dateString        = $"{month}_{day}_{year}_{hour}_{minute}_{second}";
                return dateString;
            }


            public string GetPathToLastUpdatedFileInFolder(string folderPath)
            {
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
                // Console.WriteLine($"\nFILE TO MOVE:\n{filePath}\n");
                return filePath;
            }



            public string FindFileInFolder(string fileName, string folderPath)
            {
                string filePath = string.Empty;
                if (Directory.Exists(folderPath))
                {
                    filePath = Path.Combine(folderPath, fileName);
                    // Console.WriteLine($"filePath: {filePath}");
                }
                return filePath;
            }


            public void MoveFile(string fullSourceFilePath, string fullDestinationFilePath)
            {
                // File.Copy(fullSourceFilePath, fullDestinationFilePath, true);
                File.Move(fullSourceFilePath, fullDestinationFilePath);
            }


            public void ReplaceFile(string fullSourceFilePath, string fullDestinationFilePath, string backupFileName)
            {
                // string backupFileAppendix = "_backup";
                // string backupFileName = $"HQ_{backupFileAppendix}.csv ";
                File.Replace(fullSourceFilePath, fullDestinationFilePath, backupFileName);
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
            /// <param name="targetFileName">
            ///     The name of the file that you want to write to
            ///     Save CSV to location defined by 'targetFileName'
            /// </param>
            /// <example>
            ///     DownloadCsvFromLink("http://crunchtimebaseball.com/master.csv", "BaseballData/PlayerBase/CrunchtimePlayerBaseCsvAutoDownload.csv")
            /// </example>
            public void DownloadCsvFromLink(string csvUrl, string targetFileName)
            {
                WebClient webClient = new WebClient();
                {
                    webClient.DownloadFile(csvUrl, targetFileName );
                }
            }


            public JObject _appSettingsJson = JObject.Parse(File.ReadAllText("Configuration/appsettings.Development.json"));

            public string LocalDownloadsFolderLocation()
            {
                var downloadsFolderToken = _appSettingsJson["LocalComputerItems"]["DownloadsFolderLocation"];
                string downloadsPath     = downloadsFolderToken.ToString();
                return downloadsPath;
            }


            // Go to the url the crunchtime player base file is located, download the file and save it to the 'BaseballData' folder so it can be read
            // http://crunchtimebaseball.com/baseball_map.html
            // http://crunchtimebaseball.com/baseball_map.html/master.csv
            // https://stackoverflow.com/questions/33649294/c-sharp-downloading-file-with-webclient-and-saving-it
            // public void DownloadCrunchTimePlayerBaseCsvFromLink()
            // {
            //     string crunchTimePlayerBaseCsv = "http://crunchtimebaseball.com/master.csv";

            //     WebClient webClient = new WebClient();
            //     {
            //         webClient.DownloadFile(crunchTimePlayerBaseCsv, "BaseballData/02_Target_Write/PlayerBase_Target_Write/CrunchtimePlayerBaseCsvAutoDownload.csv");
            //     }
            // }

        #endregion DOWNLOAD CSV ------------------------------------------------------------





        #region READ CSV ------------------------------------------------------------

            // STATUS: this works
            /// <summary>
            ///     Reads a csv file, non async
            /// </summary>
            /// <param name="csvFilePath"> The location / path of the file that you want to read </param>
            /// <example> _cH.ReadCsv("BaseballData/Lahman/Teams.csv"); </example>
            public IEnumerable<dynamic> ReadCsvRecords(string csvFilePath, Type modelType, Type modelMapType)
            {
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    var csvReader = new CsvReader( fileReader );
                    RegisterMapForClass(csvReader, modelMapType);

                    csvReader.Read();
                    csvReader.ReadHeader();

                    IEnumerable<object> records      = csvReader.GetRecords(modelType);
                    return records;
                }
            }


            public IList<object> ReadCsvRecordsToList(string csvFilePath, Type modelType, Type modelMapType)
            {
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    var csvReader = new CsvReader( fileReader );
                    RegisterMapForClass(csvReader, modelMapType);

                    csvReader.Read();
                    csvReader.ReadHeader();

                    var records      = csvReader.GetRecords(modelType).ToList();
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


            public IList<object> ReadCsvRecordsToList(string csvFolderPath, string csvFileName, Type modelType, Type modelMapType, bool headersAreInFirstRow)
            {
                _helpers.StartMethod();

                string csvFileFullPath = string.Empty;
                string newCsvFileName = string.Empty;

                string fileLocationFullPath = $"{csvFolderPath}{csvFileName}";
                if(fileLocationFullPath.Contains("csv"))
                {
                    // Console.WriteLine("FILE PATH CONTAINS 'CSV' AT APPENDIX ALREADY");
                    csvFileFullPath = $"{csvFolderPath}{csvFileName}";
                    // Console.WriteLine($"CsvHandler > ReadCsvRecordsToList() A > csvFileFullPath: {csvFileFullPath}");
                    newCsvFileName = $"_{csvFileName}";
                }

                else
                {
                    Console.WriteLine("APPENDING 'CSV' TO FILE NAME");
                    // csvFileFullPath = $"{csvFolderPath}{csvFileName}.csv";
                    Console.WriteLine($"CsvHandler > ReadCsvRecordsToList() B > csvFileFullPath: {csvFileFullPath}");
                    // newCsvFileName = $"_{csvFileName}.csv";
                }

                // IEnumerable<string>
                var preProcessedRecords = File.ReadLines(csvFileFullPath).Skip(1);
                string updatedPath = $"{csvFolderPath}{newCsvFileName}";
                Console.WriteLine(updatedPath);

                // Console.WriteLine($"CsvHandler > ReadCsvRecordsToList() > newFile: {newCsvFileName}");

                WriteValuesAcrossRows(updatedPath, preProcessedRecords);

                using(TextReader fileReader = File.OpenText(updatedPath))
                {
                    var csvReader = new CsvReader( fileReader );
                    RegisterMapForClass(csvReader, modelMapType);

                    csvReader.Configuration.ShouldSkipRecord = row =>
                    {
                        return row[0].StartsWith("(");
                    };

                    csvReader.Read();
                    csvReader.ReadHeader();

                    var records      = csvReader.GetRecords(modelType).ToList();
                    _helpers.CompleteMethod();
                    return records;
                }
            }


            public JObject ReadCsvRecordsToJObject(string csvFilePath, Type modelType, Type modelMapType)
            {
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    var csvReader = new CsvReader( fileReader );
                    RegisterMapForClass(csvReader, modelMapType);

                    csvReader.Read();
                    csvReader.ReadHeader();

                    var records      = csvReader.GetRecords(modelType).ToList();
                    // _h.EnumerateOverRecordsDynamic(records);

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
                // MODEL TYPE type & MODEL MAP TYPE type --> System.RuntimeType
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    CsvReader csvReader = new CsvReader( fileReader );

                    RegisterMapForClass(csvReader, modelMapType);
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
                PrintPathModelMap(csvFilePath, modelType, modelMapType);
                // MODEL TYPE type & MODEL MAP TYPE type --> System.RuntimeType
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    CsvReader csvReader = new CsvReader( fileReader );

                    RegisterMapForClass(csvReader, modelMapType);
                    csvReader.Configuration.DetectColumnCountChanges = true;

                    await csvReader.ReadAsync();
                    csvReader.ReadHeader();

                    // RECORDS type --> CsvHelper.CsvReader+<GetRecords>d__65
                    records = csvReader.GetRecords(modelType);

                    _helpers.EnumerateOverRecordsDynamic(records);

                    foreach(var record in records)
                    {
                        // Console.WriteLine(record);
                        list.Add(record);
                    }
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
                // Console.WriteLine($"CSV HANDLER > RegisterMapForClass > modelType: {modelType}");
                var mapClass = csvReader.Configuration.RegisterClassMap(modelType);
                // Console.WriteLine($"mapClass.ClassType: {mapClass.ClassType} \tmapClass.MemberMaps: {mapClass.MemberMaps}\n");
            }

        #endregion READ CSV ------------------------------------------------------------





        #region WRITE TO CSV ------------------------------------------------------------


            public void WriteValuesAcrossRows(string fullPathOfWriteFile, IEnumerable<string> recordsToWrite)
            {
                using(var stream = new MemoryStream())
                using(var writer = new StreamWriter(fullPathOfWriteFile))
                using(var reader = new StreamReader(stream))
                using(var csv = new CsvWriter(writer))
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
                    // reader.ReadToEnd().Dump();
                }
            }


        #endregion WRITE TO CSV ------------------------------------------------------------





        #region RECORDS ------------------------------------------------------------

            // // STATUS: this works
            // public JObject CreateRecordJObject(string recordString)
            // {
            //     JObject recordJObject = JObject.Parse(recordString);
            //     return recordJObject;
            // }


            // // STATUS: this works
            // public JObject CreateRecordJObject(Object record)
            // {
            //     string  recordString  = record.ToJson();
            //     JObject recordJObject = JObject.Parse(recordString);
            //     return recordJObject;
            // }


            // STATUS: this works
            public void FilterRecordsByKey(JObject obj, string filterCriteria)
            {
                // KEY VALUE PAIR --> KeyValuePair<string, JToken> recordObject
                foreach(var keyValuePair in obj)
                {
                    var key = keyValuePair.Key;
                    if(key == filterCriteria)
                    {
                        var value = keyValuePair.Value;
                        Console.WriteLine($"Value: {value}");
                    }
                }
            }

        #endregion RECORDS ------------------------------------------------------------




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
                double doubleValue = 0.00;
                // double doubleValue = double.Parse(dataToConvert[0]);

                // if the cell has data, parse the data
                try
                {
                    string[] dataToConvert = s.Split('%');
                    doubleValue = double.Parse(dataToConvert[0]);
                }

                // if the cell does not have data, error
                catch
                {
                    Console.WriteLine($"\nIssue converting string to double - likely because no data in csv cell");
                    Console.WriteLine($"In: CsvHandler > ConvertCellWithPercentageSymbolToDouble() Method\n");
                }
                return doubleValue;
            }


            public double? ParseNullableDouble(string val) => double.TryParse(val, out var i) ? (double?) i : null;


            // STATUS [ August 9, 2019 ] : this works
            // If text in a cell has quotation marks around a string, this format removes them
            // * Quotation marks can mess with mapping header rows to model properties
            public void CleanQuotationMarksFromString(string[] values)
            {
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

            // STATUS: this works
            public string GetFieldNameByIndex(CsvReader csvReader, int indexNumber)
            {
                string fieldName = csvReader[indexNumber];
                Console.WriteLine($"field: {fieldName}");
                return fieldName;
            }


            // STATUS: this works
            public int GetFieldPosition(CsvReader csvReader, int indexNumber)
            {
                var field = csvReader.GetField<int>(indexNumber);
                _helpers.Intro(field, "field int");
                return field;
            }

        #endregion HELPERS ------------------------------------------------------------




        #region PRINTING PRESS ------------------------------------------------------------


            public void PrintPathModelMap(string csvFilePath, Type modelType, Type modelMapType)
            {
                Console.WriteLine($"\nPATH: {csvFilePath}");
                Console.WriteLine($"MODEL: {modelType}\nMAP TYPE: {modelMapType}\n");
            }


            public void PrintFileInfoDetails(FileInfo file)
            {
                Console.WriteLine($"\nfileName: {file.FullName}");
                Console.WriteLine($"Last Access: {file.LastAccessTime}");
                Console.WriteLine($"Last Write: {file.LastWriteTime}\n");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------

    }
}
