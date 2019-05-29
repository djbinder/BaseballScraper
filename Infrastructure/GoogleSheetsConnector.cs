using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Infrastructure
{
    // https://medium.com/@williamchislett/writing-to-google-sheets-api-using-net-and-a-services-account-91ee7e4a291
    public class GoogleSheetsConnector
    {
        private readonly Helpers _h = new Helpers();

        // Change this if you're accessing Drive or Docs
        // _scopes: System.String[]
        // _scopes[i]: https://www.googleapis.com/auth/spreadsheets
        private readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };

        private readonly string _applicationName = "Baseball Scraper";

        private SheetsService _sheetsService;

        private readonly string _googleSheetsUrlBase = "https://docs.google.com/spreadsheets/d/";


        public SheetsService ConnectToGoogle()
        {
            _h.StartMethod();
            GoogleCredential credential;

            // Put your credentials json file in the root of the solution and make sure copy to output dir property is set to always copy
            // stream: the path to the json file within the project
            using (FileStream stream = new FileStream("Configuration/googleCredentials.json",FileMode.Open, FileAccess.Read))
            {
                // credential: Google.Apis.Auth.OAuth2.GoogleCredential+ServiceAccountGoogleCredential
                credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
                // PrintGoogleCredentialDetails(credential);
            }

            // Create Google Sheets API service.
            // sheetsService: Google.Apis.Sheets.v4.SheetsService
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                // sheetsService.HttpClientInitializer: Google.Apis.Auth.OAuth2.GoogleCredential+ServiceAccountGoogleCredential
                HttpClientInitializer = credential, ApplicationName = _applicationName
            });
            // PrintGoogleSheetServiceInitializerDetails(_sheetsService);

            return _sheetsService;
        }



        // Pass in your data as a list of a list (2-D lists are equivalent to the 2-D spreadsheet structure)
        // reference: https://developers.google.com/resources/api-libraries/documentation/sheets/v4/csharp/latest/classGoogle_1_1Apis_1_1Sheets_1_1v4_1_1Data_1_1ValueRange.html#acfb630a3039066c9c9e1764c7c90c29e
        /// <example>
        ///     private static readonly GoogleSheetsConnector _gSC = new GoogleSheetsConnector();
        ///     _gSC.UpdateGoogleSheetRows(listOfLists,"FG_SP_MASTER_IMPORT","A3:DB1000","CoreCalculator");
        /// </example>
        /// <remarks>
            /// View "FgSpMasterReportController" > "ScrapePitchersAndCreateList" for an example of this in practice
        /// </remarks>
        public string WriteGoogleSheetRows(IList<IList<object>> data, String sheetName, String range, String jsonGroupName)
        {
            _h.StartMethod();

            ConnectToGoogle();

            // The new values to apply to the spreadsheet.
            // "ValueRange": Data within a range of the spreadsheet.
            List<ValueRange> updateData = new List<ValueRange>();

            // data will fill across a row instead of down a column
            string majorDimension = "ROWS";

            // MajorDimension: Direction to write data; either "ROWS" (i.e., write horizontally) or "COLUMNS" (i.e., write vertically)
                // Defaults to "ROWS" if not explicitly set
            // Range: The range the values cover, in A1 notation.
                // Example: "test_sheet!A2" or "test_sheet!A2:D1000"
            // Values: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
                // this is where all the data that will populate the gSheet
                // The data that was read or to be written. This is an array of arrays, the outer array representing all the data and each inner array representing a major dimension. Each item in the inner array corresponds with one cell.
            ValueRange dataValueRange = new ValueRange
            {
                MajorDimension = majorDimension,
                Range = $"{sheetName}!{range}",
                Values = data
            };

            // add "dataValueRange" to List<ValueRange> updateData
            updateData.Add(dataValueRange);

            string valueInputOption = "USER_ENTERED";

            // requestBody: "The request for updating more than one range of values in a spreadsheet."
            // ValueInputOption: "How the input data should be interpreted."
            // Data: "The new values to apply to the spreadsheet"
                // Type: System.Collections.Generic.List`1[Google.Apis.Sheets.v4.Data.ValueRange]
                // this is where all the data that will populate the gSheet
            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = valueInputOption,
                Data = updateData
            };

            // helper function to console log all data points to be added to the sheet
            // PrintRequestBodyData(requestBody);

            // "SpreadsheetId" is the name of the field in the gSheetNames.json file
            var spreadsheetId = SelectGoogleSheetToRead(jsonGroupName,"SpreadsheetId");

            // helper function to print everything to console
            PrintUpdateRangeDetails(sheetName, range, jsonGroupName, spreadsheetId);

            // request: "Sets values in one or more ranges of a spreadsheet. The caller must specify the spreadsheet ID, a valueInputOption, and one or more ValueRanges."
                // Type: Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+BatchUpdateRequest
            SpreadsheetsResource.ValuesResource.BatchUpdateRequest request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            // response: "The response when updating a range of values in a spreadsheet"
                // Type: Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
            // To do this async --> Data.BatchUpdateValuesResponse response = await request.ExecuteAsync();
            BatchUpdateValuesResponse response = request.Execute();

            // helper function to console log all the response details
            LogSpreadsheetUpdateDetails(response);

            // string serializedObject = JsonConvert.SerializeObject(response);
            return JsonConvert.SerializeObject(response);
        }


        public async System.Threading.Tasks.Task<string> WriteGoogleSheetRowsAsync(IList<IList<object>> data, String sheetName, String range, String jsonGroupName)
        {
            _h.StartMethod();

            ConnectToGoogle();

            // The new values to apply to the spreadsheet.
            // "ValueRange": Data within a range of the spreadsheet.
            List<ValueRange> updateData = new List<ValueRange>();

            // data will fill across a row instead of down a column
            string majorDimension = "ROWS";

            // MajorDimension: Direction to write data; either "ROWS" (i.e., write horizontally) or "COLUMNS" (i.e., write vertically)
                // Defaults to "ROWS" if not explicitly set
            // Range: The range the values cover, in A1 notation.
                // Example: "test_sheet!A2" or "test_sheet!A2:D1000"
            // Values: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
                // this is where all the data that will populate the gSheet
                // The data that was read or to be written. This is an array of arrays, the outer array representing all the data and each inner array representing a major dimension. Each item in the inner array corresponds with one cell.
            ValueRange dataValueRange = new ValueRange
            {
                MajorDimension = majorDimension,
                Range = $"{sheetName}!{range}",
                Values = data
            };

            // add "dataValueRange" to List<ValueRange> updateData
            updateData.Add(dataValueRange);

            string valueInputOption = "USER_ENTERED";

            // requestBody: "The request for updating more than one range of values in a spreadsheet."
            // ValueInputOption: "How the input data should be interpreted."
            // Data: "The new values to apply to the spreadsheet"
                // Type: System.Collections.Generic.List`1[Google.Apis.Sheets.v4.Data.ValueRange]
                // this is where all the data that will populate the gSheet
            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = valueInputOption,
                Data = updateData
            };

            // helper function to console log all data points to be added to the sheet
            // PrintRequestBodyData(requestBody);

            // "SpreadsheetId" is the name of the field in the gSheetNames.json file
            var spreadsheetId = SelectGoogleSheetToRead(jsonGroupName,"SpreadsheetId");

            // helper function to print everything to console
            PrintUpdateRangeDetails(sheetName, range, jsonGroupName, spreadsheetId);

            // request: "Sets values in one or more ranges of a spreadsheet. The caller must specify the spreadsheet ID, a valueInputOption, and one or more ValueRanges."
                // Type: Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+BatchUpdateRequest
            SpreadsheetsResource.ValuesResource.BatchUpdateRequest request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            // response: "The response when updating a range of values in a spreadsheet"
                // Type: Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
            BatchUpdateValuesResponse response = await request.ExecuteAsync();
            // BatchUpdateValuesResponse response = request.Execute();

            // helper function to console log all the response details
            LogSpreadsheetUpdateDetails(response);

            // string serializedObject = JsonConvert.SerializeObject(response);
            return JsonConvert.SerializeObject(response);
        }



        /// <example>
            /// _gSC.WriteGoogleSheetColumns(listOfLists, "YAHOO_TRENDS","A1:Z1000","CoreCalculator");
        /// </example>
        public string WriteGoogleSheetColumns(IList<IList<object>> data, String sheetName, String range, String jsonGroupName)
        {
            _h.StartMethod();

            ConnectToGoogle();

            // The new values to apply to the spreadsheet.
            // "ValueRange": Data within a range of the spreadsheet.
            List<ValueRange> updateData = new List<ValueRange>();

            // data will fill across a row instead of down a column
            string majorDimension = "COLUMNS";

            // MajorDimension: Direction to write data; either "ROWS" (i.e., write horizontally) or "COLUMNS" (i.e., write vertically)
                // Defaults to "ROWS" if not explicitly set
            // Range: The range the values cover, in A1 notation.
                // Example: "test_sheet!A2" or "test_sheet!A2:D1000"
            // Values: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
                // this is where all the data that will populate the gSheet
                // The data that was read or to be written. This is an array of arrays, the outer array representing all the data and each inner array representing a major dimension. Each item in the inner array corresponds with one cell.
            ValueRange dataValueRange = new ValueRange
            {
                MajorDimension = majorDimension,
                Range = $"{sheetName}!{range}",
                Values = data
            };

            // add "dataValueRange" to List<ValueRange> updateData
            updateData.Add(dataValueRange);

            string valueInputOption = "USER_ENTERED";

            // requestBody: "The request for updating more than one range of values in a spreadsheet."
            // ValueInputOption: "How the input data should be interpreted."
            // Data: "The new values to apply to the spreadsheet"
                // Type: System.Collections.Generic.List`1[Google.Apis.Sheets.v4.Data.ValueRange]
                // this is where all the data that will populate the gSheet
            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = valueInputOption,
                Data = updateData
            };

            // helper function to console log all data points to be added to the sheet
            // PrintRequestBodyData(requestBody);

            // "SpreadsheetId" is the name of the field in the gSheetNames.json file
            var spreadsheetId = SelectGoogleSheetToRead(jsonGroupName,"SpreadsheetId");

            // helper function to print everything to console
            PrintUpdateRangeDetails(sheetName, range, jsonGroupName, spreadsheetId);

            // request: "Sets values in one or more ranges of a spreadsheet. The caller must specify the spreadsheet ID, a valueInputOption, and one or more ValueRanges."
                // Type: Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+BatchUpdateRequest
            SpreadsheetsResource.ValuesResource.BatchUpdateRequest request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            // response: "The response when updating a range of values in a spreadsheet"
                // Type: Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
            // To do this async --> Data.BatchUpdateValuesResponse response = await request.ExecuteAsync();
            BatchUpdateValuesResponse response = request.Execute();

            // helper function to console log all the response details
            LogSpreadsheetUpdateDetails(response);

            // string serializedObject = JsonConvert.SerializeObject(response);
            return JsonConvert.SerializeObject(response);
        }


        /// <example>
        ///     // await _gSC.WriteGoogleSheetColumnsAsync(listOfLists, "YAHOO_TRENDS","A1:Z1000","CoreCalculator");
        /// </example>
        public async System.Threading.Tasks.Task<string> WriteGoogleSheetColumnsAsync(IList<IList<object>> data, String sheetName, String range, String jsonGroupName)
        {
            _h.StartMethod();

            ConnectToGoogle();

            // The new values to apply to the spreadsheet.
            // "ValueRange": Data within a range of the spreadsheet.
            List<ValueRange> updateData = new List<ValueRange>();

            // data will fill across a row instead of down a column
            string majorDimension = "COLUMNS";

            // MajorDimension: Direction to write data; either "ROWS" (i.e., write horizontally) or "COLUMNS" (i.e., write vertically)
                // Defaults to "ROWS" if not explicitly set
            // Range: The range the values cover, in A1 notation.
                // Example: "test_sheet!A2" or "test_sheet!A2:D1000"
            // Values: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
                // this is where all the data that will populate the gSheet
                // The data that was read or to be written. This is an array of arrays, the outer array representing all the data and each inner array representing a major dimension. Each item in the inner array corresponds with one cell.
            ValueRange dataValueRange = new ValueRange
            {
                MajorDimension = majorDimension,
                Range = $"{sheetName}!{range}",
                Values = data
            };

            // add "dataValueRange" to List<ValueRange> updateData
            updateData.Add(dataValueRange);

            string valueInputOption = "USER_ENTERED";

            // requestBody: "The request for updating more than one range of values in a spreadsheet."
            // ValueInputOption: "How the input data should be interpreted."
            // Data: "The new values to apply to the spreadsheet"
                // Type: System.Collections.Generic.List`1[Google.Apis.Sheets.v4.Data.ValueRange]
                // this is where all the data that will populate the gSheet
            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = valueInputOption,
                Data = updateData
            };

            // helper function to console log all data points to be added to the sheet
            // PrintRequestBodyData(requestBody);

            // "SpreadsheetId" is the name of the field in the gSheetNames.json file
            var spreadsheetId = SelectGoogleSheetToRead(jsonGroupName,"SpreadsheetId");

            // helper function to print everything to console
            PrintUpdateRangeDetails(sheetName, range, jsonGroupName, spreadsheetId);

            // request: "Sets values in one or more ranges of a spreadsheet. The caller must specify the spreadsheet ID, a valueInputOption, and one or more ValueRanges."
                // Type: Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+BatchUpdateRequest
            SpreadsheetsResource.ValuesResource.BatchUpdateRequest request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, spreadsheetId);

            // response: "The response when updating a range of values in a spreadsheet"
                // Type: Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
            BatchUpdateValuesResponse response = await request.ExecuteAsync();
            // BatchUpdateValuesResponse response = request.Execute();

            // helper function to console log all the response details
            LogSpreadsheetUpdateDetails(response);

            // string serializedObject = JsonConvert.SerializeObject(response);
            return JsonConvert.SerializeObject(response);
        }










        // public string UpdateColumn(List<object> list, String sheetName, String column, int startingRow)
        // {
        //     ConnectToGoogle();
        //     String range = $"{sheetName}!{column}{startingRow}:{column}";
        //     // Console.WriteLine($"range is: {range}");
        //     string valueInputOption = "USER_ENTERED";

        //     // The new values to apply to the spreadsheet.
        //     List<ValueRange> updateData = new List<ValueRange>();

        //     List<IList<object>> data = new List<IList<object>>
        //     {
        //         list
        //     };

        //     // dataValueRange: Google.Apis.Sheets.v4.Data.ValueRange
        //     // dataValueRange.Range: test_sheet!A2
        //     // dataValueRange.Values: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
        //     ValueRange dataValueRange = new ValueRange
        //     {
        //         MajorDimension = "COLUMNS",
        //         Range = range,
        //         Values = data
        //     };

        //     updateData.Add(dataValueRange);

        //     // requestBody: Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest
        //     // requestBody.ValueInputOption: USER_ENTERED
        //     // requestBody.Data: System.Collections.Generic.List`1[Google.Apis.Sheets.v4.Data.ValueRange]
        //     BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
        //     {
        //         ValueInputOption = valueInputOption,
        //         Data = updateData
        //     };

        //     // var _spreadsheetId = SelectGoogleSheetToRead("SheetsTestDoc","SpreadsheetId");
        //     var _spreadsheetId = SelectGoogleSheetToRead("CoreCalculator","SpreadsheetId");

        //     // request: Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+BatchUpdateRequest
        //     var request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, _spreadsheetId);

        //     // response: Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
        //     // To do this async --> Data.BatchUpdateValuesResponse response = await request.ExecuteAsync();
        //     BatchUpdateValuesResponse response = request.Execute();
        //     LogSpreadsheetUpdateDetails(response);

        //     // string serializedObject = JsonConvert.SerializeObject(response);
        //     return JsonConvert.SerializeObject(response);
        // }


        // Example:
            // string sfbbMapDocName = "SfbbPlayerIdMap";
            // _gSC.ReadDataFromSheetRange(sfbbMapDocName,"SFBB_PLAYER_ID_MAP","A1:A2284");
        public IList<IList<Object>> ReadDataFromSheetRange(string documentName, string tabName, string range)
        {
            // _h.StartMethod();

            // this references information in the 'gSheetNames.json' file
            var spreadSheetId = SelectGoogleSheetToRead(documentName, "SpreadsheetId");
            // Console.WriteLine($"Spreadsheet Name: {documentName}");
            // Console.WriteLine($"Tab Name: {tabName}");
            // Console.WriteLine($"Spreadsheet Id: {spreadSheetId}");

            // this concatenates 1) tab name 2) ! 3) cell range to read
            range = $"{tabName}!{range}";
            // Console.WriteLine($"google range: {range}");

            var service = ConnectToGoogle();

            // request: Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+GetRequest
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadSheetId,range);

            // response: Google.Apis.Sheets.v4.Data.ValueRange
            ValueRange response = request.Execute();

            // allRows: this is equivalent to all rows with data (i.e., IList of rows); Each row is also a list itself made up of the actual data within the cells of the row
            // allRows.Count = the number of rows with data
            // e.g., allRows[0].Count is the number of cells in the first row with data in the cells
            IList<IList<Object>> allRows = response.Values;
            // int numberOfRows = allRows.Count;

            if (allRows != null && allRows.Count > 0)
            {
                // int maxColumnsWithData = 0;
                // foreach(var x in allRows)
                // {
                //     if(x.Count > maxColumnsWithData)
                //     {
                //         maxColumnsWithData = x.Count;
                //     }
                //     Console.WriteLine($"Max column: {maxColumnsWithData}");
                // }

                int rowCounter = 1;
                foreach (var row in allRows)
                {
                    // Console.WriteLine($"ROW: {rowCounter}");
                    if(row.Count == 0)
                    {
                        Console.WriteLine($"No data found in row {rowCounter}.");
                    }

                    else
                    {
                        foreach(var cell in row)
                        {
                            // Console.WriteLine($"{cell}");
                        }
                    }
                    // Console.WriteLine();
                    rowCounter++;
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            return allRows;
        }


        // Example:
            // var documentName = "SfbbPlayerIdMap";
            // var spreadSheetId = SelectGoogleSheetToRead(documentName, "SpreadsheetId");
        public string SelectGoogleSheetToRead(string jsonGroupName, string jsonItemName)
        {
            // _h.StartMethod();
            ConnectToGoogle();
            // Console.WriteLine($"jSonGroupName: {jsonGroupName}");
            // Console.WriteLine($"jSonItemName: {jsonItemName}");

            // stream.Name --> the absolute file path of the json file that the stream is reading
            using (FileStream stream = new FileStream("Configuration/gSheetNames.json",FileMode.Open, FileAccess.Read))
            {
                using(var reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    // Console.WriteLine($"content: {content}");

                    // json --> all the data from the json file; shows all first-level and second-level data / key-value pairs
                    // json.Count --> the number of json groups in the file (i.e., the number of first-level json items in the file)
                    // you can't index on this
                    // foreach loop
                        // Keys are the first-level names
                        // Values are all second-level key value pairs
                    JObject json = JObject.Parse(content);
                    // Console.WriteLine($"json: {json}");

                        JToken jsonGroup = json[jsonGroupName];
                        // Console.WriteLine($"jsonGroup: {jsonGroup}");

                        var tabSpreadsheetId = jsonGroup["Tabs"]["FG_SP_MASTER_IMPORT"]["SpreadsheetId"];


                        Console.WriteLine($"tabSpreadsheetId: {tabSpreadsheetId}");

                        JToken jTokenValue = jsonGroup[jsonItemName];
                        string returnValue = jTokenValue.ToString();
                        // Console.WriteLine($"returnValue: {returnValue}");

                    return returnValue;
                }
            }
        }

        public List<object> CreateListOfSheetHeaders(string[] headerStrings)
        {
            List<object> headers = new List<object>();

            Console.WriteLine($"HeaderStrings Length: {headerStrings.Length}");
            Console.WriteLine($"headerStrings: {headerStrings}");

            for(var i = 0; i < headerStrings.Length; i++)
            {
                Console.WriteLine(headerStrings[i]);
                headers.Add(headerStrings);
            }
            return headers;
        }


        #region CONVERTERS ------------------------------------------------------------


            public List<object> ConvertListOfAnyTypeToObjectType<T>(List<T> listOfAnyType)
            {
                var convertedList = listOfAnyType.ConvertAll(x => (object)x);
                return convertedList;
            }

            public IList<object> ConvertIListOfAnyTypeToObjectType<T>(List<T> listOfAnyType)
            {
                var convertedList = listOfAnyType.ConvertAll(x => (object)x);
                return convertedList;
            }

        #endregion CONVERTERS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------


            public void LogSpreadsheetUpdateDetails(BatchUpdateValuesResponse response)
            {
                Console.WriteLine();
                _h.Spotlight("UPDATE OUTCOMES");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine($"SPREADSHEET ID:         | {response.SpreadsheetId}");
                Console.WriteLine($"# of SHEETS updated:    | {response.TotalUpdatedSheets}");
                Console.WriteLine($"# of COLUMNS updated:   | {response.TotalUpdatedColumns}");
                Console.WriteLine($"# of ROWS updated:      | {response.TotalUpdatedRows}");
                Console.WriteLine($"# of CELLS updated:     | {response.TotalUpdatedCells}");
                Console.WriteLine($"response.ETag:          | {response.ETag}");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
            }


            public void PrintUpdateRangeDetails(String sheetName, String range, String jsonGroupName, String spreadsheetId)
            {
                Console.WriteLine();
                _h.Spotlight("UPDATE TARGET");
                Console.WriteLine($"{_googleSheetsUrlBase}{spreadsheetId}/{sheetName}{range}");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine($"JSON GROUP NAME:    | {jsonGroupName}");
                Console.WriteLine($"SPREADSHEET ID:     | {spreadsheetId}");
                Console.WriteLine($"SHEET NAME:         | {sheetName}");
                Console.WriteLine($"RANGE:              | {range}");
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
            }


            public void PrintRequestBodyData(BatchUpdateValuesRequest requestBody)
            {
                // Count = 1;
                IList<ValueRange> requestBodyData = requestBody.Data;

                Console.WriteLine();
                _h.Spotlight("DATA TO BE ADDED");
                Console.WriteLine("-------------------------------------------------------");

                foreach(ValueRange vR in requestBodyData)
                {
                    // listOfLists: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
                    // count = the # of lists within the listOfLists
                    IList<IList<object>> listOfLists = vR.Values;

                    int listCount = 1;
                    // list: System.Collections.Generic.List`1[System.Object]
                    foreach(IList<object> list in listOfLists)
                    {
                        Console.WriteLine($"LIST: {listCount}");

                        int dataCounter = 1;
                        foreach(object data in list)
                        {
                            Console.WriteLine($"{dataCounter}. {data}");
                            dataCounter++;
                        }

                        Console.WriteLine();
                        listCount++;
                    }
                }
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
            }


            public void PrintGoogleCredentialDetails(GoogleCredential credential)
            {
                Console.WriteLine();
                _h.Spotlight("GOOGLE CREDENTIAL INFO");
                Console.WriteLine("-------------------------------------------------------");

                // a bunch of information about the credential object
                JObject credentialJObject = JObject.Parse(credential.ToJson());
                Console.WriteLine($"credentialJObject: {credentialJObject}");

                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
            }


            public void PrintGoogleSheetServiceInitializerDetails(SheetsService sheetsService)
            {
                Console.WriteLine();
                _h.Spotlight("SHEET SERVICE INITIALIZER INFO");
                Console.WriteLine("-------------------------------------------------------");

                // a bunch of information about the initializer object
                JObject initializerJObject = JObject.Parse(sheetsService.HttpClientInitializer.ToJson());

                Console.WriteLine($"initializerJObject: {initializerJObject}");

                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine();
            }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}




