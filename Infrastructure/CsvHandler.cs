using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BaseballScraper.Models.BaseballSavant;
using BaseballScraper.Models.Lahman;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Infrastructure
{
    /// <summary> </summary>
    /// <list> RESOURCES
    ///     <item> Csv Helper
    ///         <description> https://joshclose.github.io/CsvHelper/reading/#getting-all-records </description>
    ///     </item>
    ///     <item> Generate Class From CSV
    ///         <description> https://toolslick.com/generation/code/class-from-csv </description>
    ///     </item>
    /// </list>
    public class CsvHandler
    {
        private readonly Helpers _h = new Helpers();

        private IEnumerable<dynamic> records;




        #region DOWNLOAD CSV ------------------------------------------------------------

            /// <example>
            ///     DownloadCsvFromLink("http://crunchtimebaseball.com/master.csv", "BaseballData/PlayerBase/CrunchtimePlayerBaseCsvAutoDownload.csv")
            /// </example>
            /// <param name="csvUrl">
            ///     this is the full url of where the csv is linked / hosted
            /// </param>
            /// <param name="targetFileName">
            ///     this is the name of the file that you want to write to
            /// </param>
            public void DownloadCsvFromLink(string csvUrl, string targetFileName)
            {
                // _h.StartMethod();
                WebClient webClient = new WebClient();
                {
                    // 1) download csv from csvUrl; 2) take that csv and save it to location defined by 'targetFileName'
                    webClient.DownloadFile(csvUrl, targetFileName );
                }
            }

        #endregion DOWNLOAD CSV ------------------------------------------------------------





        #region READ CSV ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Reads a csv file, non async </summary>
            /// <param name="csvFilePath"> The location / path of the file that you want to read </param>
            /// <example> _cH.ReadCsv("BaseballData/Lahman/Teams.csv"); </example>
            public IEnumerable<dynamic> ReadCsvRecords(string csvFilePath, Type modelType, Type modelMapType)
            {
                // _h.StartMethod();
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    var csvReader = new CsvReader( fileReader );
                    RegisterMapForClass(csvReader, modelMapType);

                    csvReader.Read();
                    csvReader.ReadHeader();

                    // Returns an IEnumerable<T> of records.
                    var records      = csvReader.GetRecords(modelType);
                    // int recordsCount = _h.CountRecords(records);

                    // Run EnumerateOverRecordsDynamic to loop through records
                    // _h.EnumerateOverRecordsDynamic(records);

                    return records;
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
                // _h.StartMethod();
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
                    // Console.WriteLine($"records.GetType: {records.GetType()}");

                    // foreach(var record in records)
                    // {
                    //     // Console.WriteLine($"record: {record}");
                    //     var spCsw = record as StartingPitcherCsw;
                    //     // Console.WriteLine($"player name: {spCsw.PlayerName}");
                    // }

                    // int recordsCount = _h.CountRecords(records);
                    // Console.WriteLine($"recordsCount: {recordsCount}");

                    // Run EnumerateOverRecordsDynamic to loop through records
                    // _h.EnumerateOverRecordsDynamic(records);
                    // _h.EnumerateOverRecordsObject(records);
                }
                return records;
            }

            public async Task<IEnumerable<dynamic>> ReadCsvRecordsAsyncToList(string csvFilePath, Type modelType, Type modelMapType, List<dynamic> list)
            {
                // _h.StartMethod();
                // MODEL TYPE type & MODEL MAP TYPE type --> System.RuntimeType
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    CsvReader csvReader = new CsvReader( fileReader );

                    // Console.WriteLine($"CSV HANDLER > ReadCsvRecordsAsyncToList > modelType: {modelType}");
                    RegisterMapForClass(csvReader, modelMapType);
                    csvReader.Configuration.DetectColumnCountChanges = true;

                    await csvReader.ReadAsync();
                    csvReader.ReadHeader();

                    // RECORDS type --> CsvHelper.CsvReader+<GetRecords>d__65
                    records = csvReader.GetRecords(modelType);

                    foreach(var record in records)
                    {
                        // Console.WriteLine(record);
                        list.Add(record);
                    }

                    // int recordsCount = _h.CountRecords(records);
                    // Console.WriteLine($"recordsCount: {recordsCount}");
                    // Run EnumerateOverRecordsDynamic to loop through records
                    // _h.EnumerateOverRecordsDynamic(records);
                    // _h.EnumerateOverRecordsObject(records);
                }
                return records;
            }



            // STATUS: this works
            /// <summary>
            ///     Register the map for the class within a csv you are trying read </summary>
            /// <remarks>
            ///     This is required any type you want to use a model map
            /// </remarks>
            /// <param name="csvReader">
            ///     A reader reading a csv file
            /// </param>
            /// <param name="modelType">
            ///     The Lahman class / model that is in the csv file
            /// </param>
            public void RegisterMapForClass(CsvReader csvReader, Type modelType)
            {
                // _h.StartMethod();
                // Console.WriteLine($"CSV HANDLER > RegisterMapForClass > modelType: {modelType}");
                var mapClass = csvReader.Configuration.RegisterClassMap(modelType);
                // Console.WriteLine($"mapClass.ClassType: {mapClass.ClassType} \tmapClass.MemberMaps: {mapClass.MemberMaps}\n");
            }

        #endregion READ CSV ------------------------------------------------------------



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
                _h.Intro(field, "field int");
                return field;
            }

        #endregion HELPERS ------------------------------------------------------------

    }
}
