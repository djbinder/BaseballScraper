﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BaseballScraper.Models.Lahman;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;

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
        // private CsvReader csvReader;
        private IEnumerable<dynamic> records;

        public interface ITypeConverter
        {
            string ConvertToString( object value, IWriterRow row, MemberMapData memberMapData );
            string ConvertFromString( string text, IReaderRow row, MemberMapData memberMapData );
        }



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
                    int recordsCount = _h.CountRecords(records);

                    // Run EnumerateOverRecordsDynamic to loop through records
                    // _h.EnumerateOverRecordsDynamic(records);

                    return records;
                }
            }


            // STATUS: this works
            /// <summary> Reads a csv file, async </summary>
            /// <remarks> This does not enumerate over the records </remarks>
            ///     <seealso cref="Helpers.EnumerateOverRecords(System.Collections.Generic.IEnumerable{object})" />
            ///     <seealso cref="Helpers.EnumerateOverRecordsDynamic(System.Collections.Generic.IEnumerable{dynamic})" />
            ///     <seealso cref="Helpers.EnumerateOverRecordsObject(System.Collections.Generic.IEnumerable{object})" />
            /// <param name="csvFilePath"> The location / path of the file that you want to read </param>
            /// <param name="modelType"> The Lahman class / model that is in the csv file </param>
            ///     <example> typeof(LahmanPeople) </example>
            /// <param name="modelMapType"> The map of the Lahman class / model that is in the csv file </param>
            ///     <example> typeof(LahmanPeopleMap) </example>
            /// <example> await _cH.ReadCsvRecordsAsync(filePath, typeof(LahmanPeople), typeof(LahmanPeopleMap)); </example>
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
                    foreach(var record in records)
                    {
                        Console.WriteLine(record);
                    }

                    int recordsCount = _h.CountRecords(records);

                    // Run EnumerateOverRecordsDynamic to loop through records
                    _h.EnumerateOverRecordsDynamic(records);
                    _h.EnumerateOverRecordsObject(records);
                }
                return records;
            }


            // STATUS: this works
            /// <summary> Register the map for the class within a csv you are trying read </summary>
            /// <remarks> This is required any type you want to use a model map </remarks>
            /// <param name="csvReader"> A reader reading a csv file </param>
            /// <param name="modelType"> The Lahman class / model that is in the csv file </param>
            public void RegisterMapForClass(CsvReader csvReader, Type modelType)
            {
                var mapClass = csvReader.Configuration.RegisterClassMap(modelType);
            }

        #endregion READ CSV ------------------------------------------------------------



        #region RECORDS ------------------------------------------------------------

            // STATUS: this works
            public JObject CreateRecordJObject(string recordString)
            {
                JObject recordJObject = JObject.Parse(recordString);
                return recordJObject;
            }


            // STATUS: this works
            public JObject CreateRecordJObject(Object record)
            {
                string  recordString  = record.ToJson();

                JObject recordJObject = JObject.Parse(recordString);
                return recordJObject;
            }


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
