using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AirtableApiClient;
using BaseballScraper.Models.ConfigurationModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using C = System.Console;



#pragma warning disable CC0068, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE0063, IDE1006, MA0016
namespace BaseballScraper.Infrastructure
{
    public class AirtableManager
    {
        private readonly Helpers               _helpers;
        private readonly AirtableConfiguration _airtableConfig;
        private readonly AirtableConfiguration _spRankingsConfiguration;
        private readonly AirtableConfiguration _authorsConfiguration;
        private readonly AirtableConfiguration _websitesConfiguration;
        private readonly AirtableConfiguration _baseballScraperConfiguration;

        // private string spRankings = "SpRankings";


        public AirtableManager(Helpers helpers, IOptions<AirtableConfiguration> airtableConfig, IOptionsSnapshot<AirtableConfiguration> options)
        {
            _helpers                      = helpers;
            _airtableConfig               = airtableConfig.Value;
            _spRankingsConfiguration      = options.Get("SpRankings");
            _authorsConfiguration         = options.Get("Authors");
            _websitesConfiguration        = options.Get("Websites");
            _baseballScraperConfiguration = options.Get("BaseballScraperConfig");
        }

        public AirtableManager(){}





        #region GET TABLE CONFIGURATION INFO ------------------------------------------------------------

            public AirtableConfiguration GetConfigurationInformationForTable(AirtableConfiguration configuration)
            {
                _airtableConfig.AuthenticationString = configuration.AuthenticationString;
                _airtableConfig.Base                 = configuration.Base;
                _airtableConfig.Link                 = configuration.Link;
                _airtableConfig.PostmanToken         = configuration.PostmanToken;
                _airtableConfig.TableName            = configuration.TableName;
                return _airtableConfig;
            }


            public AirtableConfiguration GetSpRankingsTableConfiguration()
            {
                _airtableConfig.AuthenticationString = _spRankingsConfiguration.AuthenticationString;
                _airtableConfig.Base                 = _spRankingsConfiguration.Base;
                _airtableConfig.Link                 = _spRankingsConfiguration.Link;
                _airtableConfig.PostmanToken         = _spRankingsConfiguration.PostmanToken;
                _airtableConfig.TableName            = _spRankingsConfiguration.TableName;
                return _airtableConfig;
            }


            /// <summary>
            ///     * Get configuration for Baseball Scraper project that is stored in airtable
            ///     * This allows access to the table data
            ///     * The table data is centralized config data for this app (i.e., it is not baseball data)
            /// </summary>
            /// <remarks>
            ///     To get this working in other tables
            ///       1) private readonly AirtableManager _airtableManager;
            ///       2) DI into the class
            ///       3) Call by something like: var thisConfig = _airtableManager.GetBaseballScraperConfiguration();
            /// </remarks>
            public AirtableConfiguration GetBaseballScraperConfiguration()
            {
                _airtableConfig.AuthenticationString = _baseballScraperConfiguration.AuthenticationString;
                _airtableConfig.Base                 = _baseballScraperConfiguration.Base;
                _airtableConfig.Link                 = _baseballScraperConfiguration.Link;
                _airtableConfig.PostmanToken         = _baseballScraperConfiguration.PostmanToken;
                _airtableConfig.TableName            = _baseballScraperConfiguration.TableName;
                return _airtableConfig;
            }

        #endregion GET TABLE CONFIGURATION INFO ------------------------------------------------------------



        /*
            Task<AirtableListRecordsResponse> AirtableBase.ListRecords(
                string tableName,
                string offset = null,
                IEnumerable<string> fields = null,
                string filterByFormula = null,
                int? maxRecords = null,
                int? pageSize = null,
                IEnumerable<Sort> sort = null,
                string view = null
            )
        */


        // * Dummy data
        private readonly string _testRecordId           = "rec7yJqKs5Ht3I7j3";
        private readonly int    _testMaxRecordsToGet    = 1;
        private readonly string _testAuthorIdString     = "recZmhlaw6k4Vfzwz";
        private readonly string _authorFieldName        = "Author";
        private readonly string _datePublishedFieldName = "Publish Date";
        private readonly List<string> testFields = new List<string> { "Title", "Record_Id" };






        #region GET ALL RECORDS FOR TABLE ------------------------------------------------------------


        // STATUS [ July 13, 2019 ] : this works
        /// <summary>
        ///     Get all the records from a given table
        /// </summary>
        /// <remarks>
        ///     Configuration for each table is setup in Startup.cs and airtableConfiguration.json
        ///     See: https://github.com/ngocnicholas/airtable.net
        /// </remarks>
        /// <param name="tableName">
        ///     Equivalent to the TableName in airtableConfiguration.json
        ///     Equivalent to the tab name in actual airtable
        /// </param>
        /// <param name="tableAuthenticationString">
        ///     Equivalent to the AuthenticationString in airtableConfiguration.json
        /// </param>
        /// <example>
        ///     var listOfRecords = await _atM.GetAllRecordsFromAirtableAsync(_spRankingsConfiguration.TableName, _spRankingsConfiguration.AuthenticationString);
        /// </example>
        public async Task<List<AirtableRecord>> GetAllRecordsFromAirtableAsync(string tableName, string tableAuthenticationString)
            {
                string offset       = null;
                string errorMessage = null;
                List<AirtableRecord> records = new List<AirtableRecord>();

                using (AirtableBase airtableBase = new AirtableBase(_airtableConfig.ApiKey, tableAuthenticationString))
                {
                    Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(
                        tableName: tableName
                    );

                    AirtableListRecordsResponse airResponse = await task.ConfigureAwait(false);

                    if (airResponse.Success)
                    {
                        records.AddRange(airResponse.Records.ToList());
                        offset = airResponse.Offset;
                    }
                    else errorMessage = airResponse.AirtableApiError is AirtableApiException ? airResponse.AirtableApiError.ErrorMessage : "Unknown error";
                }

                if(!string.IsNullOrEmpty(errorMessage))
                    C.WriteLine("ERROR");

                return records;
            }


        #endregion GET ALL RECORDS FOR TABLE ------------------------------------------------------------





        #region GET FILTERED RECORDS FOR TABLE ------------------------------------------------------------


            // STATUS [ July 13, 2019 ] : this works
            /// <summary>
            ///     Get filtered list of records based on value for a column / field
            /// </summary>
            /// <param name="listOfAirtableRecords">
            ///     All records from a table
            /// </param>
            /// <param name="fieldToFilterKey">
            ///     The name of the field / column you want to filter
            /// </param>
            /// <param name="valueToFilterFor">
            ///     The value in the field / column that you want to filter for
            /// </param>
            /// <example>
            ///     string fieldKey = "Record_Id"
            ///     string recordId = 1
            ///     var airtableRecordEnumerable = _atM.GetOneRecordFromAirtable(listOfRecords, "Record_Id", "1");
            /// </example>
            public static IEnumerable<AirtableRecord> GetOneRecordFromAirtable(List<AirtableRecord> listOfAirtableRecords, string fieldToFilterKey, string valueToFilterFor)
            {
                PrintQueryDetails(fieldToFilterKey, valueToFilterFor);
                IEnumerable<AirtableRecord> oneRowIEnumerable =
                    from resp in listOfAirtableRecords
                    where string.Equals(resp.GetField(fieldToFilterKey).ToString(), valueToFilterFor
                        , StringComparison.Ordinal)
                    select resp;
                return oneRowIEnumerable;
            }


            // STATUS [ July 13, 2019 ] : this works
            /// <summary>
            ///     Get filtered list of records based on value for a column / field then sort
            /// </summary>
            /// <param name="listOfAirtableRecords">
            ///     All records from a table
            /// </param>
            /// <param name="fieldToFilterKey">
            ///     The name of the field / column you want to filter
            /// </param>
            /// <param name="valueToFilterFor">
            ///     The value in the field / column that you want to filter for
            /// </param>
            /// <param name="fieldToSortKey">
            ///     For the results of the filter, what column / field do you want to sort by?
            /// </param>
            /// <example>
            ///     var airtableRecordEnumerable = _atM.GetRecordsFromTableWithSort(listOfRecords, "Month", "7", "Title");
            /// </example>
            public static IOrderedEnumerable<AirtableRecord> GetRecordsFromTableWithSort(List<AirtableRecord> listOfAirtableRecords, string fieldToFilterKey, string valueToFilterFor, string fieldToSortKey)
            {
                PrintSortQueryDetails(fieldToFilterKey, valueToFilterFor, fieldToSortKey);
                IOrderedEnumerable<AirtableRecord> sortedQuery =
                    from resp in listOfAirtableRecords
                    where string.Equals(resp.GetField(fieldToFilterKey).ToString(), valueToFilterFor
                        , StringComparison.OrdinalIgnoreCase)
                    orderby resp.GetField(fieldToSortKey).ToString()
                    select resp;
                return sortedQuery;
            }



            // STATUS [ July 13, 2019 ] : this works
            /// <summary>
            ///     Get one record from a given table
            /// </summary>
            /// <remarks>
            ///     Configuration for each table is setup in Startup.cs and airtableConfiguration.json
            ///     See: https://github.com/ngocnicholas/airtable.net
            /// </remarks>
            /// <param name="tableName">
            ///     Equivalent to the TableName in airtableConfiguration.json
            ///     Equivalent to the tab name in actual airtable
            /// </param>
            /// <param name="tableAuthenticationString">
            ///     Equivalent to the AuthenticationString in airtableConfiguration.json
            /// </param>
            /// <param name="recordId">
            ///     The airtable generated record Id
            ///     This is visible thorugh the API but NOT in the actual table
            ///     It is different than things like "Author_Id", "Record_Id", "Website_Id"
            /// </param>
            /// <example>
            ///     var oneRecord = await _atM.GetOneRecordFromAirtableAsync("SpRankings", authenticationString, "rec7yJqKs5Ht3I7j3");
            /// </example>
            public async Task<AirtableRecord> GetOneRecordFromAirtableAsync(string tableName, string tableAuthenticationString, string recordId)
            {
                using (AirtableBase airtableBase = new AirtableBase(_airtableConfig.ApiKey, tableAuthenticationString))
                {
                    Task<AirtableRetrieveRecordResponse> recordTask = airtableBase.RetrieveRecord(tableName, recordId);
                    AirtableRetrieveRecordResponse recordTaskResponse = await recordTask.ConfigureAwait(false);
                    AirtableRecord oneRecord = recordTaskResponse.Record;
                    return oneRecord;
                }
            }


        #endregion GET FILTERED RECORDS FOR TABLE ------------------------------------------------------------





        #region JOIN VALUE SEARCHES ------------------------------------------------------------

            // STATUS [ July 13, 2019 ] : this works
            /// <summary>
            ///     Retrieves the Airtable generated Id for an author based on their name
            ///     Note: the Airtable generated Id is different than "Author_Id"
            /// </summary>
            /// <param name="authorName">todo: describe authorName parameter on GetAuthorIdFromAuthorName</param>
            /// <remarks>
            ///     This can be used when the Author table is used as a field in another table
            ///     When author is referenced on another table, the Api provides the generated id instead of the author name
            ///     If you look at the table, it'll show the author's name but that is not what the api actually provides
            /// </remarks>
            /// <example>
            ///     var authorNameFromId = await _atM.GetAuthorIdFromAuthorName("Eno Sarris");
            /// </example>
            public async Task<string> GetAuthorIdFromAuthorNameAsync(string authorName)
            {
                List<AirtableRecord> authorTableList = await GetAllRecordsFromAirtableAsync(_authorsConfiguration.TableName, _authorsConfiguration.AuthenticationString).ConfigureAwait(false);

                IEnumerable<AirtableRecord> airtableRecordEnumerable = GetOneRecordFromAirtable(authorTableList, "Name", authorName);
                AirtableRecord authorRecord = airtableRecordEnumerable.First();
                string authorAirtableId = authorRecord.Id;

                return authorAirtableId;
            }


            // STATUS [ July 13, 2019 ] : this works
            /// <summary>
            ///     Retrieves the Airtable generated Id for website based on its name
            ///     Note: the Airtable generated Id is different than "Website_Id"
            /// </summary>
            /// <param name="websiteName">todo: describe websiteName parameter on GetWebsiteIdFromWebsiteName</param>
            /// <remarks>
            ///     This can be used when the website table is used as a field in another table
            ///     When the website is referenced on another table, the Api provides the generated id instead of the website name
            ///     If you look at the table, it'll show the websites's name but that is not what the api actually provides
            /// </remarks>
            /// <example>
            ///     var websiteId = await _atM.GetWebsiteIdFromWebsiteName("FanGraphs");
            /// </example>
            public async Task<string> GetWebsiteIdFromWebsiteNameAsync(string websiteName)
            {
                PrintTableConfigurationInfo(_websitesConfiguration.TableName, _websitesConfiguration.AuthenticationString);

            List<AirtableRecord> websiteTableList = await GetAllRecordsFromAirtableAsync(_websitesConfiguration.TableName, _websitesConfiguration.AuthenticationString).ConfigureAwait(false);

            IEnumerable<AirtableRecord> airtableRecordEnumerable  = GetOneRecordFromAirtable(websiteTableList, "Name", websiteName);
            AirtableRecord websiteRecord = airtableRecordEnumerable.First();
            string websiteAirtableId = websiteRecord.Id;

            return websiteAirtableId;
            }


        #endregion JOIN VALUE SEARCHES ------------------------------------------------------------





        #region GET VALUES FROM FIELD ------------------------------------------------------------


            // STATUS [ July 13, 2019 ] : this works
            /// <summary>
            ///     Get all the values for one field / column in a table
            /// </summary>
            /// <remarks>
            ///     Some field values are strings and others are JArrays
            ///     If the field refers to another table, it'll be a JArray
            /// </remarks>
            /// <param name="listOfAirtableRecords"></param>
            /// <param name="fieldName">
            ///     The name of the field you want to get values for
            /// </param>
            /// <example>
            ///     var listOfRecords = await _atM.GetAllRecordsFromAirtableAsync(_spRankingsConfiguration.TableName, _spRankingsConfiguration.AuthenticationString);
            ///     var titles = _atM.GetOneFieldForListOfRecords(listOfRecords, "Title");
            ///     var authors = _atM.GetOneFieldForListOfRecords(listOfRecords, "Author");
            /// </example>
            public static List<string> GetAllValuesForOneField(List<AirtableRecord> listOfAirtableRecords, string fieldName)
            {
                List<string> returnValues = new List<string>();
                foreach(AirtableRecord record in listOfAirtableRecords)
                {
                    Dictionary<string,object> recordFields = record.Fields;

                    KeyValuePair<string, object> singleFieldKvp = recordFields.Single(field => string.Equals(field.Key, fieldName, StringComparison.Ordinal));
                    string returnValue  = string.Empty;
                    Type fieldType      = singleFieldKvp.Value.GetType();

                    if(fieldType == typeof(string))
                    {
                        returnValue = singleFieldKvp.Value.ToString();
                        returnValues.Add(returnValue);
                    }

                    else if(fieldType == typeof(JArray))
                    {
                        JArray fieldToJArray = singleFieldKvp.Value as JArray;
                        returnValue = fieldToJArray[0].ToString();
                        returnValues.Add(returnValue);
                    }

                    else
                    {
                        C.WriteLine("it's not a string OR a JArray; ERROR!!!");
                    }
                }
                return returnValues;
            }


        #endregion GET VALUES FROM FIELD ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            private static void PrintRecordsFromAirtableList(List<AirtableRecord> listOfAirtableRecords)
            {
                listOfAirtableRecords.ForEach((record) => C.WriteLine($"record: {record}"));
            }

            private static void PrintFieldValuesFromList(List<string> returnValues)
            {
                returnValues.ForEach((returnValue) => C.WriteLine($"returnValue: {returnValue}"));
            }

            private static void PrintKeyValuesPairs(Dictionary<string,object> recordFields)
            {
                foreach(KeyValuePair<string,object> kvp in recordFields)
                {
                    C.WriteLine($"KEY: {kvp.Key}\t VALUE: {kvp.Value}");
                }
            }

            private static void PrintSortQueryDetails(string fieldToQueryKey, string fieldToQueryValue, string fieldToSortKey)
            {
                C.WriteLine($"QUERY FIELD KEY: {fieldToQueryKey}\t FILTER FOR: {fieldToQueryValue}\t SORT BY: {fieldToSortKey}");
            }

            private static void PrintQueryDetails(string fieldToQueryKey, string filterForValue)
            {
                C.WriteLine($"QUERY FIELD KEY: {fieldToQueryKey}\t FILTER FOR: {filterForValue}");
            }

            private static void PrintTableConfigurationInfo(string tableName, string authString)
            {
                C.WriteLine($"TABLE: {tableName}\t AUTH STRING: {authString}");
            }

        #endregion PRINTING PRESS ------------------------------------------------------------



    }
}
