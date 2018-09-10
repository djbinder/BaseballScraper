using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BaseballScraper.Models.Lahman;
using CsvHelper;
using CsvHelper.Configuration;

namespace BaseballScraper.Infrastructure
{
    // reference: https://joshclose.github.io/CsvHelper/reading/#getting-all-records
    public class CsvHandler
    {
        private readonly Helpers _h = new Helpers();


        public interface ITypeConverter
        {
            string ConvertToString( object value, IWriterRow row, MemberMapData memberMapData );
            string ConvertFromString( string text, IReaderRow row, MemberMapData memberMapData );
        }


        #region READ CSV ------------------------------------------------------------

            // STATUS: this works
            // example: _cH.ReadCsv("BaseballData/Lahman/Teams_test.csv");
            public void ReadCsv(string csvFilePath)
            {
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    var csvReader = new CsvReader( fileReader );

                    csvReader.Read();
                    csvReader.ReadHeader();

                    // total teams in Lahman is 2685
                    var records = csvReader.GetRecords<LahmanTeam>();
                    // int count   = 1;
                    // foreach(var record in records)
                    // {
                    //     Console.WriteLine(count);
                    //     Console.WriteLine($"Record: {record}");
                    //     Console.WriteLine(record.name);
                    //     count++;
                    // }
                    EnumerateOverRecordsDynamic(records);
                }
            }

            // STATUS: this works
            // example: await _cH.ReadCsvAsync("BaseballData/Lahman/Teams_test.csv");
            public async Task ReadCsvAsync(string csvFilePath)
            {
                using(TextReader fileReader = File.OpenText(csvFilePath))
                {
                    var csvReader = new CsvReader( fileReader );
                    // csvReader.Configuration.RegisterClassMap<LahmanTeamMap>();

                    RegisterMapForClass(csvReader, typeof(LahmanTeamMap));

                    await csvReader.ReadAsync();

                    csvReader.ReadHeader();

                    var records = csvReader.GetRecords<LahmanTeam>();
                    int count   = 1;
                    foreach(var record in records)
                    {
                        if(record.DidTeamWinWorldSeries == "Y")
                        {
                            Console.WriteLine($"{record.Year} : {record.TeamName}");
                        }
                        count++;
                    }
                    // EnumerateOverRecordsDynamic(records);
                }
            }


            public void RegisterMapForClass(CsvReader csvReader, Type modelType)
            {
                var mapClass = csvReader.Configuration.RegisterClassMap(modelType);
            }

        #endregion READ CSV ------------------------------------------------------------



        #region RECORDS ------------------------------------------------------------

            // // STATUS: this works
            // // example:
            //     // var records = csvReader.GetRecords<TeamTestModel>();
            //     // EnumerateOverRecords(records);
            // public void EnumerateOverRecords(IEnumerable<LahmanTeam> records)
            // {
            //     // RECORDS --> System.Collections.Generic.IEnumerable<TeamTestModel> records
            //     foreach(var record in records)
            //     {
            //         Console.WriteLine($"TEAM NAME: {record.name}");
            //         Console.WriteLine($"STADIUM: {record.park}");
            //         Console.WriteLine();
            //     }
            // }

            // // STATUS: this works
            // // example:
            //     // var recordsEnumerator = csvReader.EnumerateRecords( newTeam );
            //     //E numerateOverRecords(recordsEnumerator, 1);
            // public void EnumerateOverRecords(IEnumerable<TeamTestModel> recordsEnumerator, int loopCount)
            // {
            //     var getEnumerator = recordsEnumerator.GetEnumerator();

            //     while(getEnumerator.MoveNext())
            //     {
            //         Console.WriteLine($"Count: {loopCount}");
            //         loopCount++;
            //     }
            // }

            // STATUS: this works
            // example:
                // var dynamicRecords = csvReader.GetRecords<dynamic>();
                // EnumerateOverRecordsDynamic(dynamicRecords);
            public void EnumerateOverRecordsDynamic(IEnumerable<dynamic> dynamicRecords)
            {
                // DYNAMIC RECORDS --> CsvHelper.CsvReader+<GetRecords>d__63`1[System.Object]
                // DYNAMIC RECORDS --> System.Collections.Generic.IEnumerable<dynamic> dynamicRecords
                // DYNAMIC RECORD type --> System.Dynamic.ExpandoObject
                foreach(var dynamicRecord in dynamicRecords)
                {
                    Console.WriteLine(dynamicRecord);
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
                field.Intro("field int");
                return field;
            }

        #endregion HELPERS ------------------------------------------------------------
    }
}