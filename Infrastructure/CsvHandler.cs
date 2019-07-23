using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CsvHelper;
using Newtonsoft.Json.Linq;


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

        #endregion DOWNLOAD CSV ------------------------------------------------------------





        #region READ CSV ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Reads a csv file, non async </summary>
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
            ///     Register the map for the class within a csv you are trying read
            /// </summary>
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
                _helpers.Intro(field, "field int");
                return field;
            }

        #endregion HELPERS ------------------------------------------------------------

    }
}
