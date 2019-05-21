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


namespace BaseballScraper.Infrastructure
{
    // https://medium.com/@williamchislett/writing-to-google-sheets-api-using-net-and-a-services-account-91ee7e4a291
    #pragma warning disable CS0414, CS0219, CS1591
    public class GoogleSheetsConnector
    {
        private readonly Helpers _h = new Helpers();

        // Change this if you're accessing Drive or Docs
        // _scopes: System.String[]
        // _scopes[i]: https://www.googleapis.com/auth/spreadsheets
        private readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string _applicationName = "Baseball Scraper";
        private SheetsService _sheetsService;


        public SheetsService ConnectToGoogle()
        {
            GoogleCredential credential;

            // Put your credentials json file in the root of the solution and make sure copy to output dir property is set to always copy
            // stream: the path to the json file within the project
            using (FileStream stream = new FileStream("Configuration/googleCredentials.json",FileMode.Open, FileAccess.Read))
            {
                // credential: Google.Apis.Auth.OAuth2.GoogleCredential+ServiceAccountGoogleCredential
                credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);

                // a bunch of information about the credential object
                // JObject credentialJObject = JObject.Parse(credential.ToJson());
            }

            // Create Google Sheets API service.
            // sheetsService: Google.Apis.Sheets.v4.SheetsService
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                // sheetsService.HttpClientInitializer: Google.Apis.Auth.OAuth2.GoogleCredential+ServiceAccountGoogleCredential
                HttpClientInitializer = credential, ApplicationName = _applicationName
            });

            // a bunch of information about the initializer object
            // JObject initializerJObject = JObject.Parse(_sheetsService.HttpClientInitializer.ToJson());

            return _sheetsService;
        }


        // Pass in your data as a list of a list (2-D lists are equivalent to the 2-D spreadsheet structure)
        // reference: https://developers.google.com/resources/api-libraries/documentation/sheets/v4/csharp/latest/classGoogle_1_1Apis_1_1Sheets_1_1v4_1_1Data_1_1ValueRange.html#acfb630a3039066c9c9e1764c7c90c29e
        public string UpdateData(List<IList<object>> data)
        {
            String range = "test_sheet!A1:Y";
            string valueInputOption = "USER_ENTERED";

            // The new values to apply to the spreadsheet.
            List<ValueRange> updateData = new List<ValueRange>();

            // dataValueRange: Google.Apis.Sheets.v4.Data.ValueRange
            // dataValueRange.Range: test_sheet!A2
            // dataValueRange.Values: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
            ValueRange dataValueRange = new ValueRange
            {
                MajorDimension = "COLUMNS",
                Range = range,
                Values = data,
            };

            updateData.Add(dataValueRange);

            // requestBody: Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest
            // requestBody.ValueInputOption: USER_ENTERED
            // requestBody.Data: System.Collections.Generic.List`1[Google.Apis.Sheets.v4.Data.ValueRange]
            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = valueInputOption,
                Data = updateData
            };

            var _spreadsheetId = SelectGoogleSheetToRead("SheetsTestDoc","SpreadsheetId");

            // request: Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+BatchUpdateRequest
            var request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, _spreadsheetId);

            // response: Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
            // To do this async --> Data.BatchUpdateValuesResponse response = await request.ExecuteAsync();
            BatchUpdateValuesResponse response = request.Execute();
            LogSpreadsheetUpdateDetails(response);

            // string serializedObject = JsonConvert.SerializeObject(response);
            return JsonConvert.SerializeObject(response);
        }


        // Pass in your data as a list of a list (2-D lists are equivalent to the 2-D spreadsheet structure)
        /// <example>
        ///     var data = ReadDataFromSheetRange(string documentName, string tabName, string range)
        ///     UpdateData(data, "A1:Z1000","SfbbPlayerIdMap","1qKnB5z6qVeHD_eSGFJdJsWKh2muJt_-Cn-ZzdjuSbDw")
        /// </example>
        public string UpdateData(IList<IList<object>> data, String range, String jsonGroupName, string spreadSheetId)
        {
            string valueInputOption = "USER_ENTERED";

            // The new values to apply to the spreadsheet.
            List<ValueRange> updateData = new List<ValueRange>();

            // dataValueRange: Google.Apis.Sheets.v4.Data.ValueRange
            // dataValueRange.Range: test_sheet!A2
            // dataValueRange.Values: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
            ValueRange dataValueRange = new ValueRange
            {
                MajorDimension = "COLUMNS",
                Range = range,
                Values = data
            };

            updateData.Add(dataValueRange);

            // requestBody: Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest
            // requestBody.ValueInputOption: USER_ENTERED
            // requestBody.Data: System.Collections.Generic.List`1[Google.Apis.Sheets.v4.Data.ValueRange]
            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = valueInputOption,
                Data = updateData
            };

            var _spreadsheetId = SelectGoogleSheetToRead(jsonGroupName,spreadSheetId);

            // request: Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+BatchUpdateRequest
            var request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, _spreadsheetId);

            // response: Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
            // To do this async --> Data.BatchUpdateValuesResponse response = await request.ExecuteAsync();
            BatchUpdateValuesResponse response = request.Execute();
            LogSpreadsheetUpdateDetails(response);

            // string serializedObject = JsonConvert.SerializeObject(response);
            return JsonConvert.SerializeObject(response);
        }


        public string UpdateColumn(List<object> list, String sheetName, String column, int startingRow)
        {
            String range = $"{sheetName}!{column}{startingRow}:{column}";
            // Console.WriteLine($"range is: {range}");
            string valueInputOption = "USER_ENTERED";

            // The new values to apply to the spreadsheet.
            List<ValueRange> updateData = new List<ValueRange>();

            List<IList<object>> data = new List<IList<object>>
            {
                list
            };

            // dataValueRange: Google.Apis.Sheets.v4.Data.ValueRange
            // dataValueRange.Range: test_sheet!A2
            // dataValueRange.Values: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
            ValueRange dataValueRange = new ValueRange
            {
                MajorDimension = "COLUMNS",
                Range = range,
                Values = data
            };

            updateData.Add(dataValueRange);

            // requestBody: Google.Apis.Sheets.v4.Data.BatchUpdateValuesRequest
            // requestBody.ValueInputOption: USER_ENTERED
            // requestBody.Data: System.Collections.Generic.List`1[Google.Apis.Sheets.v4.Data.ValueRange]
            BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = valueInputOption,
                Data = updateData
            };

            var _spreadsheetId = SelectGoogleSheetToRead("SheetsTestDoc","SpreadsheetId");

            // request: Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+BatchUpdateRequest
            var request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, _spreadsheetId);

            // response: Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
            // To do this async --> Data.BatchUpdateValuesResponse response = await request.ExecuteAsync();
            BatchUpdateValuesResponse response = request.Execute();
            LogSpreadsheetUpdateDetails(response);

            // string serializedObject = JsonConvert.SerializeObject(response);
            return JsonConvert.SerializeObject(response);
        }


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

            // stream.Name --> the absolute file path of the json file that the stream is reading
            using (FileStream stream = new FileStream("Configuration/gSheetNames.json",FileMode.Open, FileAccess.Read))
            {
                using(var reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();

                    // json --> all the data from the json file; shows all first-level and second-level data / key-value pairs
                    // json.Count --> the number of json groups in the file (i.e., the number of first-level json items in the file)
                    // you can't index on this
                    // foreach loop
                        // Keys are the first-level names
                        // Values are all second-level key value pairs
                    JObject json = JObject.Parse(content);
                        JToken jsonGroup = json[jsonGroupName];
                        JToken jTokenValue = jsonGroup[jsonItemName];
                        string returnValue = jTokenValue.ToString();
                        // Console.WriteLine($"returnValue: {returnValue}");

                    return returnValue;
                }
            }
        }



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



        public void LogSpreadsheetUpdateDetails(BatchUpdateValuesResponse response)
        {
            Console.WriteLine();
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
    }
}
