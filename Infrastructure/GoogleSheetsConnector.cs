using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.Models.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;
using C = System.Console;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Infrastructure
{
    // https://medium.com/@williamchislett/writing-to-google-sheets-api-using-net-and-a-services-account-91ee7e4a291
    public class GoogleSheetsConnector
    {
        private readonly Helpers                  _helpers;
        private readonly string[]                 _scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string                   _applicationName = "Baseball Scraper";
        private SheetsService                     _sheetsService;
        private readonly string                   _googleSheetsUrlBase = "https://docs.google.com/spreadsheets/d/";
        private GoogleSheetConfiguration          _googleSheetConfiguration;
        private readonly GoogleSheetConfiguration _crunchTimePlayerIdMapConfiguration;
        private readonly GoogleSheetConfiguration _sfbbPlayerIdMapConfiguration;

        private string sfbbPlayerIdMap       = "SfbbPlayerIdMap";
        private string crunchtimePlayerIdMap = "CrunchtimePlayerIdMap";



        public GoogleSheetsConnector(Helpers helpers, GoogleSheetConfiguration googleSheetConfiguration, IOptionsSnapshot<GoogleSheetConfiguration> options)
        {
            _helpers                            = helpers;
            _googleSheetConfiguration           = googleSheetConfiguration;
            _crunchTimePlayerIdMapConfiguration = options.Get(crunchtimePlayerIdMap);
            _sfbbPlayerIdMapConfiguration       = options.Get(sfbbPlayerIdMap);
        }

        public GoogleSheetsConnector() {}



        // STATUS [ July 12, 2019 ] : this works
        /// <summary>
        ///     Gets all configuration information from SffbPlayerIdMap from gSheetNames.json configuration file
        /// </summary>
        /// <remarks>
        ///     See: gSheetNames.json, GoogleSheetsConnector.cs, GoogleSheetConfiguration.cs
        ///     Inserted into Dependency Injection in Startup.cs
        /// </remarks>
        /// <example>
        ///     [1] public class GenericController
        ///     [2] private readonly GoogleSheetConfiguration _sfbbPlayerIdMapConfiguration;
        ///     [3] public GenericController(IOptionsSnapshot < GoogleSheetConfiguration > options)
        ///     {
        ///         _sfbbPlayerIdMapConfiguration = options.Get("SfbbPlayerIdMap");
        ///     }
        ///     [4] var sfbbDocumentName = _sfbbPlayerIdMapConfiguration.DocumentName;
        /// </example>
        public GoogleSheetConfiguration GetSfbbPlayerIdMapConfiguration()
        {
            _googleSheetConfiguration.DocumentName  = _sfbbPlayerIdMapConfiguration.DocumentName;
            _googleSheetConfiguration.GId           = _sfbbPlayerIdMapConfiguration.GId;
            _googleSheetConfiguration.Link          = _sfbbPlayerIdMapConfiguration.Link;
            _googleSheetConfiguration.Range         = _sfbbPlayerIdMapConfiguration.Range;
            _googleSheetConfiguration.SpreadsheetId = _sfbbPlayerIdMapConfiguration.SpreadsheetId;
            _googleSheetConfiguration.TabName       = _sfbbPlayerIdMapConfiguration.TabName;
            _googleSheetConfiguration.WorkbookName  = _sfbbPlayerIdMapConfiguration.WorkbookName;
            return _googleSheetConfiguration;
        }


        // STATUS [ July 12, 2019 ] : this works
        /// <summary>
        ///     Gets all configuration information from CrunchtimePlayerIdMap from gSheetNames.json configuration file
        /// </summary>
        /// <remarks>
        ///     See: gSheetNames.json, GoogleSheetsConnector.cs, GoogleSheetConfiguration.cs
        ///     Inserted into Dependency Injection in Startup.cs
        /// </remarks>
        /// <example>
        ///     [1] public class GenericController
        ///     [2] private readonly GoogleSheetConfiguration _crunchTimePlayerIdMapConfiguration;
        ///     [3] public GenericController(IOptionsSnapshot < GoogleSheetConfiguration > options)
        ///     {
        ///         _crunchTimePlayerIdMapConfiguration = options.Get("CrunchtimePlayerIdMap");
        ///     }
        ///     [4] var crunchtimeDocumentName = _crunchTimePlayerIdMapConfiguration.DocumentName;
        /// </example>
        public GoogleSheetConfiguration GetCrunchTimePlayerIdMapConfiguration()
        {
            _googleSheetConfiguration.DocumentName  = _crunchTimePlayerIdMapConfiguration.DocumentName;
            _googleSheetConfiguration.GId           = _crunchTimePlayerIdMapConfiguration.GId;
            _googleSheetConfiguration.Link          = _crunchTimePlayerIdMapConfiguration.Link;
            _googleSheetConfiguration.Range         = _crunchTimePlayerIdMapConfiguration.Range;
            _googleSheetConfiguration.SpreadsheetId = _crunchTimePlayerIdMapConfiguration.SpreadsheetId;
            _googleSheetConfiguration.TabName       = _crunchTimePlayerIdMapConfiguration.TabName;
            _googleSheetConfiguration.WorkbookName  = _crunchTimePlayerIdMapConfiguration.WorkbookName;
            return _googleSheetConfiguration;
        }





        #region CONNECT TO GOOGLE API ------------------------------------------------------------

            // Add credentials json file to project root folder
            // Make sure copy to output dir property is set to always copy
            public SheetsService ConnectToGoogle()
            {
                GoogleCredential credential;

                using (FileStream stream = new FileStream("Configuration/googleCredentials.json",FileMode.Open, FileAccess.Read))
                {
                    // credential: Google.Apis.Auth.OAuth2.GoogleCredential+ServiceAccountGoogleCredential
                    credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
                }

                // Create Google Sheets API service.
                // Type is Google.Apis.Sheets.v4.SheetsService
                _sheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    // Type is Google.Apis.Auth.OAuth2.GoogleCredential+ServiceAccountGoogleCredential
                    HttpClientInitializer = credential, ApplicationName = _applicationName
                });

                // PRINTERS
                // PrintGoogleCredentialDetails(credential);
                // PrintGoogleSheetServiceInitializerDetails(_sheetsService);
                return _sheetsService;
            }

        #endregion CONNECT TO GOOGLE API ------------------------------------------------------------





        #region CONSTANTS FOR SHEETS ------------------------------------------------------------


            // Data will fill across a row instead of down a column
            private string _rowsMajorDimension = "ROWS";


            // Data will fill down a column instead of across a row
            private string _columnsMajorDimension = "COLUMNS";


            private string _userEnteredValueInputOption = "USER_ENTERED";


            // "SpreadsheetId" is the name of the field in the gSheetNames.json file
            // this references information in the 'gSheetNames.json' file
            private string _spreadSheetId;


        #endregion CONSTANTS FOR SHEETS ------------------------------------------------------------





        #region WRITE TO SHEET ------------------------------------------------------------


            // STATUS [ August 3, 2019 ] : this works
            // Pass in your data as a list of a list (2-D lists are equivalent to the 2-D spreadsheet structure)
            // See: https://bit.ly/2MDoleW
            /// <example>
            ///     private static readonly GoogleSheetsConnector _gSC = new GoogleSheetsConnector();
            ///     _gSC.UpdateGoogleSheetRows(listOfLists,"FG_SP_MASTER_IMPORT","A3:DB1000","CoreCalculator");
            /// </example>
            /// <remarks>
            ///     View "FgSpMasterReportController" > "ScrapePitchersAndCreateList" for an example of this in practice
            /// </remarks>
            public string WriteGoogleSheetRows(IList<IList<object>> data, string sheetName, string range, string jsonGroupName)
            {
                _helpers.StartMethod();

                ConnectToGoogle();

                _spreadSheetId                       = SelectGoogleSheetToRead(jsonGroupName,"SpreadsheetId");

                ValueRange dataValueRange            = SetSheetDataValueRange(_rowsMajorDimension, sheetName, range, data);
                List<ValueRange> updateData          = CreateValueRangeList(dataValueRange);
                BatchUpdateValuesRequest requestBody = CreateBatchUpdateValuesRequest(_userEnteredValueInputOption, updateData);
                BatchUpdateRequest request           = CreateBatchUpdateRequest(requestBody, _spreadSheetId);
                BatchUpdateValuesResponse response   = CreateBatchUpdateValuesResponse(request);

                // PRINTERS
                // PrintRequestBodyData(requestBody);
                // PrintUpdateRangeDetails(sheetName, range, jsonGroupName, spreadsheetId);
                // LogSpreadsheetUpdateDetails(response);
                return JsonConvert.SerializeObject(response);
            }


            // STATUS [ August 3, 2019 ] : this works
            // See: non-async version of method for more details
            public async Task<string> WriteGoogleSheetRowsAsync(IList<IList<object>> data, string sheetName, string range, string jsonGroupName)
            {
                // _helpers.StartMethod();
                ConnectToGoogle();

                _spreadSheetId                       = SelectGoogleSheetToRead(jsonGroupName,"SpreadsheetId");

                ValueRange dataValueRange            = SetSheetDataValueRange(_rowsMajorDimension, sheetName, range, data);
                List<ValueRange> updateData          = CreateValueRangeList(dataValueRange);
                BatchUpdateValuesRequest requestBody = CreateBatchUpdateValuesRequest(_userEnteredValueInputOption, updateData);
                BatchUpdateRequest request           = CreateBatchUpdateRequest(requestBody, _spreadSheetId);
                BatchUpdateValuesResponse response   = await CreateBatchUpdateValuesResponseAsync(request);

                // PRINTERS
                // PrintRequestBodyData(requestBody);
                // PrintUpdateRangeDetails(sheetName, range, jsonGroupName, spreadsheetId);
                // LogSpreadsheetUpdateDetails(response);
                return JsonConvert.SerializeObject(response);
            }


            // STATUS [ August 3, 2019 ] : this works
            /// <example>
            ///     _gSC.WriteGoogleSheetColumns(listOfLists, "YAHOO_TRENDS","A1:Z1000","CoreCalculator");
            /// </example>
            public string WriteGoogleSheetColumns(IList<IList<object>> data, string sheetName, string range, string jsonGroupName)
            {
                // _helpers.StartMethod();

                ConnectToGoogle();

                _spreadSheetId                       = SelectGoogleSheetToRead(jsonGroupName,"SpreadsheetId");

                ValueRange dataValueRange            = SetSheetDataValueRange(_columnsMajorDimension, sheetName, range, data);
                List<ValueRange> updateData          = CreateValueRangeList(dataValueRange);
                BatchUpdateValuesRequest requestBody = CreateBatchUpdateValuesRequest(_userEnteredValueInputOption, updateData);
                BatchUpdateRequest request           = CreateBatchUpdateRequest(requestBody, _spreadSheetId);
                BatchUpdateValuesResponse response   = CreateBatchUpdateValuesResponse(request);

                // PRINTERS
                // PrintRequestBodyData(requestBody);
                // PrintUpdateRangeDetails(sheetName, range, jsonGroupName, spreadsheetId);
                // LogSpreadsheetUpdateDetails(response);
                return JsonConvert.SerializeObject(response);
            }


            // STATUS [ August 3, 2019 ] : this works
            /// <example>
            ///     await _gSC.WriteGoogleSheetColumnsAsync(listOfLists, "YAHOO_TRENDS","A1:Z1000","CoreCalculator");
            /// </example>
            public async Task<string> WriteGoogleSheetColumnsAsync(IList<IList<object>> data, string sheetName, string range, string jsonGroupName)
            {
                // _helpers.StartMethod();
                ConnectToGoogle();

                _spreadSheetId                       = SelectGoogleSheetToRead(jsonGroupName,"SpreadsheetId");

                ValueRange dataValueRange            = SetSheetDataValueRange(_columnsMajorDimension, sheetName, range, data);
                List<ValueRange> updateData          = CreateValueRangeList(dataValueRange);
                BatchUpdateValuesRequest requestBody = CreateBatchUpdateValuesRequest(_userEnteredValueInputOption, updateData);
                BatchUpdateRequest request           = CreateBatchUpdateRequest(requestBody, _spreadSheetId);
                BatchUpdateValuesResponse response   = await CreateBatchUpdateValuesResponseAsync(request);

                // PRINTERS
                // PrintRequestBodyData(requestBody);
                // PrintUpdateRangeDetails(sheetName, range, jsonGroupName, spreadsheetId);
                // LogSpreadsheetUpdateDetails(response);
                return JsonConvert.SerializeObject(response);
            }


        #endregion WRITE TO SHEET ------------------------------------------------------------





        #region READ SHEET ------------------------------------------------------------


            // STATUS [ August 3, 2019 ] : this should work but haven't tested
            // Examples:
            /*
                Example 1:
                string sfbbMapDocName = "SfbbPlayerIdMap";
                _gSC.ReadDataFromSheetRange(sfbbMapDocName,"SFBB_PLAYER_ID_MAP","A1:A2284");

                Example 2:
                var documentName = "wPDI";
                var sheetName = "wPDI";
                var range = "A4:MN10004";
                var allRows = _googleSheetsConnector.ReadDataFromSheetRange(documentName, sheetName, range);
            */
            public IList<IList<object>> ReadDataFromSheetRange(string documentName, string sheetName, string range)
            {
                _helpers.StartMethod();
                _spreadSheetId = SelectGoogleSheetToRead(documentName, "SpreadsheetId");
                range          = $"{sheetName}!{range}";

                SheetsService service = ConnectToGoogle();
                GetRequest request    = service.Spreadsheets.Values.Get(_spreadSheetId, range);
                ValueRange valueRange = request.Execute();

                IList<IList<object>> allRows = CreateListOfAllRowsInSheet(valueRange);
                // PrintRowInfo(allRows);
                _helpers.CompleteMethod();
                return allRows;
            }


            // STATUS [ August 3, 2019 ] : this works
            // Example:
            // 1) var documentName = "SfbbPlayerIdMap";
            // 2) var spreadSheetId = SelectGoogleSheetToRead(documentName, "SpreadsheetId");
            private string SelectGoogleSheetToRead(string jsonGroupName, string jsonItemName)
            {
                ConnectToGoogle();

                using (FileStream stream = new FileStream("Configuration/gSheetNames.json",FileMode.Open, FileAccess.Read))
                {
                    using(var reader = new StreamReader(stream))
                    {
                        string content     = reader.ReadToEnd();
                        JObject json       = ParseJsonFromConfigurationFile(content);
                        JToken jsonGroup   = json[jsonGroupName];
                        JToken jTokenValue = jsonGroup[jsonItemName];
                        string returnValue = jTokenValue.ToString();

                        // PrintJsonInfoForSheetBeingRead(jsonGroup, jTokenValue, returnValue);
                        return returnValue;
                    }
                }
            }


            // STATUS [ August 3, 2019 ] : this works
            private JObject ParseJsonFromConfigurationFile(string content)
            {
                // json --> all the data from the json file; shows all first-level and second-level data / key-value pairs
                // json.Count --> the number of json groups in the file (i.e., the number of first-level json items in the file)
                // you can't index on this
                // foreach loop
                // Keys are the first-level names
                // Values are all second-level key value pairs
                JObject json = JObject.Parse(content);
                return json;
            }


        #endregion READ SHEET ------------------------------------------------------------





        #region SHEET COMPONENTS ------------------------------------------------------------


            // STATUS [ August 3, 2019 ] : this works
            // ValueRange
            //  * "Data within a range of the spreadsheet."
            //  * The new values to apply to the spreadsheet
            //  * See: https://bit.ly/2Zu7fUh
            private ValueRange SetSheetDataValueRange(string majorDimension, string sheetName, string range, IList<IList<object>> data)
            {
                // [ 1 ] MajorDimension
                //  * Direction to write data; either "ROWS" (i.e., write horizontally) or "COLUMNS" (i.e., write vertically)
                //  * Defaults to "ROWS" if not explicitly set
                // [ 2 ] Range
                //  * The range the values cover, in A1 notation.
                //  * Example: "test_sheet!A2" or "test_sheet!A2:D1000"
                // [ 3 ] Values
                //  * System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
                //  * The data that was read or to be written to gSheet
                //  * An array of arrays:
                //      * Outer array is all the data
                //      * Each inner array representing a major dimension. Each item in inner array equates to one cell.
                ValueRange dataValueRange = new ValueRange
                {
                    MajorDimension = majorDimension,
                    Range          = $"{sheetName}!{range}",
                    Values         = data
                };
                return dataValueRange;
            }


            // STATUS [ August 3, 2019 ] : this works
            // ValueRange
            //  * "Data within a range of the spreadsheet."
            //  * The new values to apply to the spreadsheet
            //  * See: https://bit.ly/2Zu7fUh
            private List<ValueRange> CreateValueRangeList(ValueRange dataValueRange)
            {
                List<ValueRange> updateData = new List<ValueRange>
                {
                    // add "dataValueRange" to List<ValueRange> updateData
                    dataValueRange
                };
                return updateData;
            }


            // STATUS [ August 3, 2019 ] : this works
            // BatchUpdateValuesRequest
            //  * "The request for updating more than one range of values in a spreadsheet."
            //  * See: https://bit.ly/3364meV
            private BatchUpdateValuesRequest CreateBatchUpdateValuesRequest(string valueInputOption, IList<ValueRange> updateData)
            {
                // ValueInputOption
                //  * "How the input data should be interpreted."
                // Data
                //  * "The new values to apply to the spreadsheet"
                //  * Type is System.Collections.Generic.List`1[Google.Apis.Sheets.v4.Data.ValueRange]
                BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest
                {
                    ValueInputOption = valueInputOption,
                    Data             = updateData
                };
                return requestBody;
            }


            // STATUS [ August 3, 2019 ] : this works
            // BatchUpdateRequest
            //  * Sets values in 1+ ranges of spreadsheet
            //  * Caller must specify sheet ID, a valueInputOption, and 1+ ValueRanges
            //  * Type is Google.Apis.Sheets.v4.SpreadsheetsResource+ValuesResource+BatchUpdateRequest
            //  * See: https://bit.ly/2KBkBbt
            private BatchUpdateRequest CreateBatchUpdateRequest(BatchUpdateValuesRequest requestBody, string spreadSheetId)
            {
                BatchUpdateRequest request = _sheetsService.Spreadsheets.Values.BatchUpdate(
                    requestBody,
                    spreadSheetId
                );
                return request;
            }


            // STATUS [ August 3, 2019 ] : this works
            // BatchUpdateValuesResponse
            //  * "The response when updating a range of values in a spreadsheet"
            //  * Type is Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
            //  * To do this async --> Data.BatchUpdateValuesResponse response = await request.ExecuteAsync();
            //  * See: https://bit.ly/2ZG0VJq
            private BatchUpdateValuesResponse CreateBatchUpdateValuesResponse(BatchUpdateRequest request)
            {
                BatchUpdateValuesResponse response = request.Execute();
                return response;
            }


            // STATUS [ August 3, 2019 ] : this works
            // BatchUpdateValuesResponse
            //  * "The response when updating a range of values in a spreadsheet"
            //  * Type is Google.Apis.Sheets.v4.Data.BatchUpdateValuesResponse
            //  * To do this async --> Data.BatchUpdateValuesResponse response = await request.ExecuteAsync();
            //  * See: https://bit.ly/2ZG0VJq
            private async Task<BatchUpdateValuesResponse> CreateBatchUpdateValuesResponseAsync(BatchUpdateRequest request)
            {
                BatchUpdateValuesResponse response = await request.ExecuteAsync();
                return response;
            }


            // STATUS [ August 3, 2019 ] : this works
            // allRows:
            //  * Equivalent to all rows with data
            //  * Each row is also a list itself made up of the actual data within the cells of the row
            //  * allRows.Count = the number of rows with data
            //  * e.g., allRows[0].Count is the number of cells in the first row with data in the cells
            private IList<IList<object>> CreateListOfAllRowsInSheet(ValueRange valueRange)
            {
                IList<IList<object>> allRows = valueRange.Values;
                return allRows;
            }


        #endregion SHEET COMPONENTS ------------------------------------------------------------





        #region SHEET HEADERS ------------------------------------------------------------


            private List<object> CreateListOfSheetHeaders(string[] headerStrings)
            {
                List<object> headers = new List<object>();
                for(var i = 0; i < headerStrings.Length; i++)
                {
                    headers.Add(headerStrings);
                }
                return headers;
            }


            // STATUS [ August 3, 2019 ] : this works
            // var listFromDatabase = GetMany(season: 2019, minInningsPitched: 100);
            // var firstPitcher = listFromDatabase[0];
            // var listOfProperties = _googleSheetsConnector.GetModelProperties(firstPitcher);
            public List<object> GetModelProperties<T>(T item)
            {
                List<object> propertyNames = new List<object>();
                var props = item.GetType().GetProperties();
                foreach(var prop in props)
                {
                    propertyNames.Add(prop.Name);
                }
                return propertyNames;
            }


        #endregion SHEET HEADERS ------------------------------------------------------------



            public void SetGradientConditionalFormat()
            {
                SheetsService service = ConnectToGoogle();
                ConditionalFormatRule conditionalFormatRule = new ConditionalFormatRule();

                GradientRule gradientRule = new GradientRule()
                {
                    Maxpoint = new InterpolationPoint()
                    {
                        Color = SelectColor(ColorEnum.Green),
                        Type = "MAX",
                    },

                    Minpoint = new InterpolationPoint()
                    {
                        Color = SelectColor(ColorEnum.Red),
                        Type = "MIN"
                    }
                };

                conditionalFormatRule.GradientRule = gradientRule;

                int sheetGId = SelectGIdOfSheetToFormat("wPDI", service);

                GridRange gridRange = new GridRange()
                {
                    SheetId = sheetGId,
                    StartRowIndex = 5,
                    EndRowIndex = 50,
                    StartColumnIndex = 8,
                    EndColumnIndex = 9,
                };

                C.WriteLine($"gridRange: {gridRange.SheetId}");

                var listOfRanges = conditionalFormatRule.Ranges;
                foreach(var range in listOfRanges)
                {
                    C.WriteLine($"range: {range}");
                }

                conditionalFormatRule.Ranges.Add(gridRange);
                foreach(var range in listOfRanges)
                {
                    C.WriteLine($"range: {range}");
                }
            }





        #region FORMAT SHEET - PRIMARY METHODS -----------------------------------------------------------


            /* PRIMARY METHOD */
            // STATUS [ August 3, 2019 ] : this works
            // Example: _googleSheetsConnector.FormatSheet("wPDI", 0, 41, 3, 4);
            public void FormatSheet(string jsonGroupName, int startColumnIndex, int endColumnIndex, int startRowIndex, int endRowIndex, string fontFamily = "Arial", bool? isCellBolded=false, int fontSize=10, ColorEnum backgroundColor=ColorEnum.White, ColorEnum fontColorEnum=ColorEnum.Black)
            {
                SheetsService service = ConnectToGoogle();

                // STEP 1
                int sheetGId          = SelectGIdOfSheetToFormat(jsonGroupName, service);

                // STEP 2
                CellFormat cellFormat = SetCellFormat(
                    backgroundColor,
                    fontFamily,
                    isCellBolded,
                    fontSize,
                    fontColorEnum
                );

                // STEP 3
                Request updateCellsRequest = CreateFormatRequest(
                    sheetGId,
                    cellFormat,
                    startColumnIndex,
                    endColumnIndex,
                    startRowIndex,
                    endRowIndex
                );

                // STEP 4
                ExecuteFormatChange(updateCellsRequest, service);
            }


            /* PRIMARY METHOD - HEADERS */
            // STATUS [ August 3, 2019 ] : this works
            // _googleSheetsConnector.FormatHeaderRow("wPDI", 4, 0, 41);
            public void FormatHeaderRow(string jsonGroupName, int headerRowNumber, int startColumnIndex, int endColumnIndex)
            {
                SheetsService service = ConnectToGoogle();
                int sheetGId          = SelectGIdOfSheetToFormat(jsonGroupName, service);

                CellFormat cellFormat = SetCellFormat
                (
                    ColorEnum.Black, "Arial", true, 10, ColorEnum.White
                );

                headerRowNumber -= 1;

                Request updateCellsRequest = CreateFormatRequest(
                    sheetGId,
                    cellFormat,
                    startColumnIndex,
                    endColumnIndex,
                    headerRowNumber,
                    headerRowNumber + 1
                );
                ExecuteFormatChange(updateCellsRequest, service);
            }


        #endregion FORMAT SHEET - PRIMARY METHODS ------------------------------------------------------------





        #region FORMAT SHEET - STEPS -----------------------------------------------------------

            // STATUS [ August 3, 2019 ] : this works
            // STEP 1: Get the GId of the sheet that you want format
            private int SelectGIdOfSheetToFormat(string jsonGroupName, SheetsService service)
            {
                _spreadSheetId          = SelectGoogleSheetToRead(jsonGroupName, "SpreadsheetId");
                Spreadsheet spreadSheet = service.Spreadsheets.Get(_spreadSheetId).Execute();
                Sheet thisSheet         = spreadSheet.Sheets.Where(s => s.Properties.Title == jsonGroupName).FirstOrDefault();
                int sheetGId            = (int)thisSheet.Properties.SheetId;
                return sheetGId;
            }


            // STATUS [ August 3, 2019 ] : this works
            // STEP 2: set the format that you want to apply to cells in the sheet
            private CellFormat SetCellFormat(ColorEnum backgroundColor, string fontFamily, bool? isCellBolded, int fontSize, ColorEnum fontColorEnum)
            {
                CellFormat cellFormat = new CellFormat()
                {
                    BackgroundColor = SelectColor(backgroundColor),
                    TextFormat = SetCellTextFormat(
                        fontFamily,
                        isCellBolded,
                        fontSize,
                        fontColorEnum
                    ),
                };
                return cellFormat;
            }


            // STATUS [ August 3, 2019 ] : this works
            // STEP 2 helper
            private TextFormat SetCellTextFormat(string fontFamily, bool? isCellBolded, int fontSize, ColorEnum colorEnum)
            {
                var TextFormat = new TextFormat()
                {
                    Bold       = isCellBolded,
                    FontFamily = fontFamily,
                    FontSize   = fontSize,
                    ForegroundColor = SelectColor(colorEnum)
                };
                return TextFormat;
            }


            // STATUS [ August 3, 2019 ] : this works
            // STEP 3: set the start and end indexes for the rows and columns to be formatted
            private Request CreateFormatRequest(int sheetId, CellFormat cellFormat, int startColumnIndex, int endColumnIndex, int startRowIndex, int endRowIndex)
            {
                C.WriteLine($"create format request: {sheetId}");
                var updateCellsRequest = new Request()
                {
                    RepeatCell = new RepeatCellRequest()
                    {
                        Range = SetRangeToFormat(
                            sheetId,
                            startColumnIndex,
                            endColumnIndex,
                            startRowIndex,
                            endRowIndex
                        ),

                        Cell = new CellData()
                        {
                            UserEnteredFormat = cellFormat
                        },
                        Fields = "UserEnteredFormat(BackgroundColor,TextFormat)"
                    }
                };
                return updateCellsRequest;
            }


            // STATUS [ August 3, 2019 ] : this works
            // STEP 3 helper
            private GridRange SetRangeToFormat(int sheetId, int startColumnIndex, int endColumnIndex, int startRowIndex, int endRowIndex)
            {
                var Range = new GridRange()
                {
                    SheetId          = sheetId,
                    StartColumnIndex = startColumnIndex,
                    EndColumnIndex   = endColumnIndex,
                    StartRowIndex    = startRowIndex,
                    EndRowIndex      = endRowIndex
                };
                return Range;
            }


            // STATUS [ August 3, 2019 ] : this works
            // STEP 4: execute format changes in sheet
            private void ExecuteFormatChange(Request updateCellsRequest, SheetsService service)
            {
                BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest
                {
                    Requests = new List<Request>
                    {
                        updateCellsRequest
                    }
                };

                SpreadsheetsResource.BatchUpdateRequest batchUpdateRequest = service.Spreadsheets.BatchUpdate(
                    batchUpdateSpreadsheetRequest,
                    _spreadSheetId
                );
                batchUpdateRequest.Execute();
            }


            // STATUS [ August 3, 2019 ] : this works
            // Enum to set either text or cell color
            public enum ColorEnum
            {
                White,
                Black,
                Blue,
                Red,
                Green,
                Yellow,
                Orange,
                Purple,
            }


            // STATUS [ August 3, 2019 ] : this works
            // Switch that works with ColorEnum to set either text or cell color
            public Color SelectColor(ColorEnum colorEnum)
            {
                var color = new Color();
                switch(colorEnum)
                {
                    case ColorEnum.White:
                        color.Blue  = 1;
                        color.Green = 1;
                        color.Red   = 1;
                        return color;

                    case ColorEnum.Black:
                        color.Blue  = 0;
                        color.Green = 0;
                        color.Red   = 0;
                        return color;

                    case ColorEnum.Blue:
                        color.Blue  = 1;
                        color.Green = 0;
                        color.Red   = 0;
                        return color;

                    case ColorEnum.Red:
                        color.Blue  = 0;
                        color.Green = 0;
                        color.Red   = 1;
                        return color;

                    case ColorEnum.Green:
                        color.Blue  = 0;
                        color.Green = 1;
                        color.Red   = 0;
                        return color;

                    case ColorEnum.Yellow:
                        color.Blue  = 0;
                        color.Green = 1;
                        color.Red   = (float) 0.5;
                        return color;

                    case ColorEnum.Orange:
                        color.Blue  = 0;
                        color.Green = (float) 0.5;
                        color.Red   = 1;
                        return color;

                    case ColorEnum.Purple:
                        color.Blue  = 1;
                        color.Green = 0;
                        color.Red   = 1;
                        return color;
                }
                throw new Exception("ColorEnum not found");
            }

        #endregion FORMAT SHEET - STEPS -----------------------------------------------------------






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


            public void PrintJsonInfoForSheetBeingRead(JToken jsonGroup, JToken jTokenValue, string returnValue )
            {
                C.WriteLine($"\njsonGroup: {jsonGroup}");
                C.WriteLine($"jTokenValue: {jTokenValue}");
                C.WriteLine($"returnValue: {returnValue}\n");
            }

            public void PrintDetailsForSheetBeingRead(string jsonGroupName, string jsonItemName)
            {
                C.WriteLine($"\njSonGroupName: {jsonGroupName}\t jSonItemName: {jsonItemName}\n");
            }

            public void PrintRowInfo(IList<IList<object>> allRows)
            {
                if (allRows != null && allRows.Count > 0)
                {
                    int rowCounter = 1;
                    foreach (var row in allRows)
                    {
                        if(row.Count == 0)
                        {
                            C.WriteLine($"No data in row {rowCounter}.");
                        }
                        else
                        {
                            foreach(var cell in row)
                            {
                                C.WriteLine($"{cell}");
                            }
                        }
                        rowCounter++;
                    }
                }
                else { C.WriteLine("No data found."); }
            }

            public void PrintDetailsForRangeBeingRead(string documentName, string sheetName, string spreadSheetId)
            {
                C.WriteLine($"\nSpreadsheet Name: {documentName}");
                C.WriteLine($"Sheet Name:         {sheetName}");
                C.WriteLine($"Spreadsheet Id:     {spreadSheetId}\n");
            }

            public void LogSpreadsheetUpdateDetails(BatchUpdateValuesResponse response)
            {
                C.WriteLine();
                _helpers.Spotlight("UPDATE OUTCOMES");
                C.WriteLine("-------------------------------------------------------");
                C.WriteLine($"SPREADSHEET ID:         | {response.SpreadsheetId}");
                C.WriteLine($"# of SHEETS updated:    | {response.TotalUpdatedSheets}");
                C.WriteLine($"# of COLUMNS updated:   | {response.TotalUpdatedColumns}");
                C.WriteLine($"# of ROWS updated:      | {response.TotalUpdatedRows}");
                C.WriteLine($"# of CELLS updated:     | {response.TotalUpdatedCells}");
                C.WriteLine($"response.ETag:          | {response.ETag}");
                C.WriteLine("-------------------------------------------------------");
                C.WriteLine();
            }

            public void PrintUpdateRangeDetails(string sheetName, string range, string jsonGroupName, string spreadsheetId)
            {
                C.WriteLine();
                _helpers.Spotlight("UPDATE TARGET");
                C.WriteLine($"{_googleSheetsUrlBase}{spreadsheetId}/{sheetName}{range}");
                C.WriteLine("-------------------------------------------------------");
                C.WriteLine($"JSON GROUP NAME:    | {jsonGroupName}");
                C.WriteLine($"SPREADSHEET ID:     | {spreadsheetId}");
                C.WriteLine($"SHEET NAME:         | {sheetName}");
                C.WriteLine($"RANGE:              | {range}");
                C.WriteLine("-------------------------------------------------------");
                C.WriteLine();
            }

            public void PrintUpdateRangeDetails(string sheetName, string range, string jsonGroupName)
            {
                C.WriteLine();
                _helpers.Spotlight("UPDATE TARGET");
                C.WriteLine("-------------------------------------------------------");
                C.WriteLine($"JSON GROUP NAME:    | {jsonGroupName}");
                C.WriteLine($"SHEET NAME:         | {sheetName}");
                C.WriteLine($"RANGE:              | {range}");
                C.WriteLine("-------------------------------------------------------");
                C.WriteLine();
            }

            public void PrintRequestBodyData(BatchUpdateValuesRequest requestBody)
            {
                // Count = 1;
                IList<ValueRange> requestBodyData = requestBody.Data;

                C.WriteLine();
                _helpers.Spotlight("DATA TO BE ADDED");
                C.WriteLine("-------------------------------------------------------");

                foreach(ValueRange vR in requestBodyData)
                {
                    // listOfLists: System.Collections.Generic.List`1[System.Collections.Generic.IList`1[System.Object]]
                    // count = the # of lists within the listOfLists
                    IList<IList<object>> listOfLists = vR.Values;

                    int listCount = 1;
                    // list: System.Collections.Generic.List`1[System.Object]
                    foreach(IList<object> list in listOfLists)
                    {
                        C.WriteLine($"LIST: {listCount}");

                        int dataCounter = 1;
                        foreach(object data in list)
                        {
                            C.WriteLine($"{dataCounter}. {data}");
                            dataCounter++;
                        }
                        C.WriteLine();
                        listCount++;
                    }
                }
                C.WriteLine($"-------------------------------------------------------\n");
            }

            public void PrintGoogleCredentialDetails(GoogleCredential credential)
            {
                C.WriteLine();
                _helpers.Spotlight("GOOGLE CREDENTIAL INFO");
                C.WriteLine("-------------------------------------------------------");

                // a bunch of information about the credential object
                JObject credentialJObject = JObject.Parse(credential.ToJson());
                C.WriteLine($"credentialJObject: {credentialJObject}");

                C.WriteLine($"-------------------------------------------------------\n");
            }

            public void PrintGoogleSheetServiceInitializerDetails(SheetsService sheetsService)
            {
                _helpers.Spotlight($"\nSHEET SERVICE INITIALIZER INFO");
                C.WriteLine("-------------------------------------------------------");

                // a bunch of information about the initializer object
                JObject initializerJObject = JObject.Parse(sheetsService.HttpClientInitializer.ToJson());

                C.WriteLine($"initializerJObject: {initializerJObject}");
                C.WriteLine($"-------------------------------------------------------\n");
            }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
