// reference: https://github.com/perevoznyk/excel-export/blob/master/README.md
// https://github.com/dotnetcore/NPOI/blob/master/samples/Npoi.Samples.CreateNewSpreadsheet/Program.cs

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using BaseballScraper.Models.FanGraphs;
using ExcelDataReader;
using Export.XLS;
using Npoi.Mapper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BaseballScraper.Infrastructure
{
    public class ExcelMapper
    {
        private Constants _c = new Constants();

        // STATUS: this works
        /// <summary> Needs to be run before most Excel methods </summary>
        private void RegisterProviderToStart()
        {
            System.Text.Encoding.RegisterProvider (System.Text.CodePagesEncodingProvider.Instance);
        }

        // STATUS: this works
        /// <summary> Needs to be run before most Excel methods </summary>
        private void SetThreadCurrentCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo ("en-US");
        }

        #region EXCEL WORKBOOK / DOCUMENT
            // STATUS: this works
            /// <summary> This creates and saves a new Excel (XLS) existingFile </summary>
            /// <type> XLSX </type>
            /// <param name="fileName"> What you want the existingFile to be named </param>
            /// <example> _eM.CreateNewExcelWorkbook("BaseballScraper"); </example>
            public void CreateNewExcelWorkbook(string fileName)
            {
                _c.Start.ThisMethod();
                RegisterProviderToStart();
                SetThreadCurrentCulture();

                ExcelDocument document = new ExcelDocument();

                // document.UserName = "dan";
                document.CodePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;

                string thisFilesName = $"{fileName}.xls";

                FileStream stream = new FileStream(fileName, FileMode.Create);

                document.Save(stream);
                stream.Close();
            }

            // OPTION 1
            // STATUS: this works
            /// <summary> Create and save a new Excel (xlsx) file </summary>
            /// <type> XLSX </type>
            /// <remarks> The new of the new Excel document and its first tab are defined within the method </remarks>
            public void CreateNewExcelWorkbook()
            {
                RegisterProviderToStart();
                SetThreadCurrentCulture();

                var    newFileName  = @"BaseballScraper.xlsx";
                string newSheetName = "SheetA";

                using (var fileStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write))
                {
                    IWorkbook newWorkbook = new XSSFWorkbook();
                    ISheet    newSheet    = newWorkbook.CreateSheet(newSheetName);
                    newWorkbook.Write(fileStream);
                }
            }

            // OPTION 2
            // TODO: the class/model type (FanGraphsPitcher) is defined within the method. This needs to be passed as an argument
            /// <summary> Create and save a new Excel (xlsx) file </summary>
            /// <type> XLSX </type>
            /// <param name="fileName"> The name of the new Excel document that you want to create </param>
            /// <param name="sheetName"> The name of the first tab / sheet of the new Excel document </param>
            /// <example> _eM.CreateNewExcelWorkbook("BaseballScraper", "FgPitchers"); </example>
            public void CreateNewExcelWorkbook(string fileName, string sheetName)
            {
                RegisterProviderToStart();
                SetThreadCurrentCulture();

                var mapper = new Mapper();

                string thisFilesName = $"{fileName}.xlsx";
                string newSheetName  = sheetName;

                FanGraphsPitcher newFanGraphsPitcher = new FanGraphsPitcher();

                mapper.Save(thisFilesName, new [] { newFanGraphsPitcher }, newSheetName );
            }

            // STATUS: this works
            /// <summary> This will format file names entered as string correctly to ensure the file name can be used by other methods; It will check if a the string entered as an argument to a method includes the '.xlsx' appended to the end of it; If it doesn't, it will add it; If it does, it will do nothing </summary>
            /// <param name="fileName"> The string, representing a file name, that needs to be checked </param>
            /// <returns> Properly formatted file name (as a string) that includes '.xlsx' appended to the file name </returns>
            private string ManageWorkbookNames(string fileName)
            {
                string targetWorkbook = "";

                if(fileName.Contains("xlsx"))
                {
                    Console.WriteLine("file name entered WITH xlsx appended");
                    targetWorkbook = fileName;
                }
                else
                {
                    Console.WriteLine("file name entered WITHOUT xlsx appended");
                    targetWorkbook = $"{fileName}.xlsx";
                }

                return targetWorkbook;
            }
        #endregion EXCEL WORKBOOK / DOCUMENT



        #region TABS/SHEETS
            // STATUS: this works
            /// <summary> Adds a new tab / sheet to an existing Excel file </summary>
            /// <type> XLSX </type>
            /// <param name="fileName"> The name of the file that you want to add a new tab / sheet too </param>
            /// <param name="sheetName"> The new tab / sheet's name </param>
            public void AddSheetToExistingExcelWorkbook(string fileName, string sheetName)
            {
                RegisterProviderToStart();
                SetThreadCurrentCulture();

                Stream newStream = new MemoryStream();

                string existingFileName = $@"{fileName}";

                using(var existingFile = new FileStream(existingFileName, FileMode.Open, FileAccess.Read))
                {
                    existingFile.CopyTo(newStream);
                    newStream.Seek(0, SeekOrigin.Begin);
                }

                var    updatedWorkbook = new XSSFWorkbook(newStream);
                ISheet newSheet        = updatedWorkbook.CreateSheet(sheetName) as XSSFSheet;

                var fileStream = new FileStream(existingFileName, FileMode.Create);
                updatedWorkbook.Write(fileStream);
                fileStream.Close();
            }

            // STATUS: this does NOT work
            // TODO: make this work?
            public void GetAllWorkbookSheets(string fileName)
            {
                _c.Start.ThisMethod();

                RegisterProviderToStart();
                SetThreadCurrentCulture();

                string existingFileName = $@"{fileName}";

                // STREAM type --> Filestream
                using(var stream = File.Open(existingFileName, FileMode.Open, FileAccess.Read))
                {
                    // READER type --> IExcelDataReader
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // RESULT type --> System.Data.DataSet
                        var result = reader.AsDataSet();

                        // RESULT TABLES type --> System.Data.DataTableCollection
                        var resultTables = result.Tables;

                        // RESULT TABLES COUNT --> the number of tabs / sheets in workbook
                        var resultTablesCount = resultTables.Count;

                        // TABLE --> each tab / sheet in the workbook
                        // TABLE type --> System.Data.DataTable
                        foreach(var table in resultTables)
                        {
                            Console.WriteLine(table);
                            // var isReading = reader.Read();
                            // Console.WriteLine($"Is Reading: {isReading}");

                            int numberOfColumnsInTable = reader.FieldCount;
                            Console.WriteLine($"Columns: {numberOfColumnsInTable}");

                            int numberOfRowsInTable = reader.RowCount;
                            Console.WriteLine($"Rows: {numberOfRowsInTable}");

                            Object headerFooter = reader.HeaderFooter;
                            Console.WriteLine($"HEADER FOOTER: {headerFooter}");
                            headerFooter.Dig();

                            Console.WriteLine();
                        }

                        // READER NAME --> djb
                        // READER NAME type --> string
                        var readerName = reader.Name;

                        // READER RESULTS COUNT --> the number of tabs / sheets in workbook
                        var readerResultsCount = reader.ResultsCount;
                    }
                }
            }
        #endregion TABS/SHEETS



        #region ROWS / RECORDS
            // STATUS: in progress
            // TODO: the class/model type (FGHitter) is defined within the method. This needs to be passed as an argument
            /// <summary> Add a new row / record to a sheet in an existing xlsx </summary>
            /// <param name="fileName"> The name of the file you are targeting </param>
            /// <param name="sheetName"> The name of the tab / sheet you are targeting </param>
            /// <example> _eM.AddRecordToSheet("BaseballScraper", "FgHitters"); </example>
            public void AddRecordToSheet(string fileName, string sheetName)
            {
                RegisterProviderToStart();
                SetThreadCurrentCulture();

                FGHitter newFGHitter = CreateHitterForTesting("Kenny Lofton");

                var mapper = new Mapper();

                string targetWorkbook = ManageWorkbookNames(fileName);

                // ARGUMENTS --> (1) string path  (2) IEnumerable<T> Objects  (3) string sheetName (4) bool overwrite
                    // (4A) true = create new workbook
                    // (4B) false = workbook already exists so update the existing workbook instead of creating a new one
                mapper.Save(targetWorkbook, new[] { newFGHitter }, sheetName, overwrite: false);
            }

            // STATUS: this works
            /// <summary> Retrieve all records of a given class / model from an existing tab / sheet within an existing workbook </summary>
            /// <remarks> Class / model type is defined within the method
            /// <param name="fileName"> The name of the file you are targeting </param>
            /// <param name="sheetName"> The name of the tab / sheet you are targeting </param>
            public void GetAllRecordsInSheet(string fileName, string sheetName)
            {
                var mapper = new Mapper (fileName);

                // ALL RECORDS --> IEnumerable<RowInfo<FGHitter>>
                // ALL RECORDS type --> IEnumerable<RowInfo<FGHitter>>
                var allRecords = mapper.Take<FGHitter> (sheetName);
                Extensions.TypeAndIntro(allRecords, "all records");

                List<FGHitter> fgHitters = new List<FGHitter>();

                // RECORD --> RowInfo<FGHitter>
                // RECORD type --> Npoi.Mapper.RowInfo`1[BaseballScraper.Models.FanGraphs.FGHitter]
                foreach(var record in allRecords)
                {
                    AddRecordsToList(fgHitters, record.Value);
                    PrintRecord(record);
                }
            }

            // STATUS: this works
            /// <summary> Add retrieved records / rows to a list </summary>
            public IList<T> AddRecordsToList<T>(IList<T> objectList, T t)
            {
                // _c.Start.ThisMethod();
                objectList.Add(t);
                Console.WriteLine($"Object #{objectList.Count()} has been added to list");
                return objectList;
            }

            // STATUS: this works
            /// <summary> Prints records to a list </summary>
            public void PrintRecord<T>(RowInfo<T> obj)
            {
                Console.WriteLine(obj);
                Console.WriteLine(obj.Value);
                // Console.WriteLine(obj.Value.FanGraphsName);
                Console.WriteLine();
            }
        #endregion ROWS / RECORDS


        #region MODIFY TAB/SHEET COLUMN(S)
            // OPTION 1
            // STATUS: this works
            /// <summary> Set the width of a given column </summary>
            /// <remarks> In option 1, you provide a letter; this is more intuitive than providing a number since Excel column headers are letters. A switch is used to convert the letter you provide to the right number so that the mapper understands it </remarks>
            /// <param name="document"> Excel document that the column is in </param>
            /// <param name="columnLetter"> The header letter (e.g, "A" or "AA" or "Z")</param>
            /// <param name="columnWidth"> The width that you want the column to be </param>
            private void SetColumnWidth(ExcelDocument document, string columnLetter, int columnWidth)
            {
                // document.ColumnWidth(0, 120);
                int columnNumber = ColumnHeaderLetterToNumber(columnLetter);
                document.ColumnWidth(columnNumber, columnWidth);
            }
            // OPTION 2
            // STATUS: this works
            private void SetColumnWidth(ExcelDocument document, int columnNumber, int columnWidth)
            {
                // document.ColumnWidth(0, 120);
                document.ColumnWidth(columnNumber, columnWidth);
            }
        #endregion MODIFY TAB/SHEET COLUMN(S)


        #region CELLS / VALUES
            // OPTION 1
            // STATUS: this works
            /// <summary> Set the contents of a cell </summary>
            /// <remarks> In option 1, you provide a letter; this is more intuitive than providing a number since Excel column headers are letters. A switch is used to convert the letter you provide to the right number so that the mapper understands it </remarks>
            /// <param name="document"> Excel document that the cell is in </param>
            /// <param name="rowNumber"> The row of the cell </param>
            /// <param name="columnLetter"> The column letter of the cell </param>
            /// <param name="cellValue"> What you want to add to the cell </param>
            private void SetCellValue(ExcelDocument document, int rowNumber, string columnLetter,  object cellValue)
            {
                int      columnNumber                   = ColumnHeaderLetterToNumber(columnLetter);
                document[rowNumber, columnNumber].Value = cellValue;
            }

            // OPTION 2
            // STATUS: this works
            /// <summary> Set the contents of a cell </summary>
            /// <remarks> In option 2, you provide a number to represent the target column </remarks>
            /// <param name="document"> Excel document that the cell is in </param>
            /// <param name="rowNumber"> The row of the cell </param>
            /// <param name="columnLetter"> The column number of the cell </param>
            /// <param name="cellValue"> What you want to add to the cell </param>
            private void SetCellValue(ExcelDocument document, int rowNumber, int columnNumber,  object cellValue)
            {
                document[rowNumber, columnNumber].Value = cellValue;
            }


            // OPTION 1
            // STATUS: // TODO: neither option accounts for bold, italic etc. that can be added as third parameter to new Font()
            public void SetFont(ExcelDocument document, int rowNumber, string columnLetter,  string fontName, int fontSize)
            {
                int      columnNumber                  = ColumnHeaderLetterToNumber(columnLetter);
                document[rowNumber, columnNumber].Font = new Font(fontName, fontSize);
            }
            // OPTION 2
            // STATUS: // TODO: neither option accounts for bold, italic etc. that can be added as third parameter to new Font()
            public void SetFont(ExcelDocument document, int rowNumber, int columnNumber, string fontName, int fontSize)
            {
                document[rowNumber, columnNumber].Font = new Font(fontName, fontSize);
            }

            // STATUS: this works
            /// <summary> Format a date cell </summary>
            public void FormatDate (ExcelDocument document, int rowNumber, int columnNumber)
            {
                document.Cell(rowNumber, columnNumber).Format = @"dd/mm/yyyy";
            }
        #endregion CELLS / VALUES


        #region HELPERS
            public FGHitter CreateHitterForTesting(string playerName)
            {
                FGHitter newFGHitter = new FGHitter
                {
                    FanGraphsName = playerName,
                    FanGraphsTeam = "Chicago Cubs",
                    GP            = "123",
                    PA            = "123",
                    HR            = "123",
                    R             = "123",
                    RBI           = "123",
                    SB            = "123",
                    BB_percent    = "23%",
                    K_percent     = "23%",
                    ISO           = ".321",
                    BABIP         = ".321",
                    AVG           = ".321",
                    OBP           = ".321",
                    SLG           = ".321",
                    wOBA          = ".321",
                    wRC_plus      = "789",
                    BsR           = "789",
                    Off           = "789",
                    Def           = "789",
                    WAR           = "6",
                };

                return newFGHitter;
            }

            // STATUS: this works
            /// <summary> Converts a given letter to it's corresponding number </summary>
            /// <remark> This makes it so you can enter a letter instead of a number when identifying an Excel column; basically it makes it more intuitive / natural - you don't have to figure out what column number each letter is </remark>
            /// <param name="letter"> A letter that corresponds to an Excel column </param>
            /// <returns> The column number of an Excel column </returns>
            internal int ColumnHeaderLetterToNumber(string letter)
            {
                _c.Start.ThisMethod();
                switch(letter)
                {
                    case "A": 
                        return 0;
                    case "B": 
                        return 1;
                    case "C": 
                        return 2;
                    case "D": 
                        return 3;
                    case "E": 
                        return 4;
                    case "F": 
                        return 5;
                    case "G": 
                        return 6;
                    case "H": 
                        return 7;
                    case "I": 
                        return 8;
                    case "J": 
                        return 9;
                    case "K": 
                        return 10;
                    case "L": 
                        return 11;
                    case "M": 
                        return 12;
                    case "N": 
                        return 13;
                    case "O": 
                        return 14;
                    case "P": 
                        return 15;
                    case "Q": 
                        return 16;
                    case "R": 
                        return 17 ;
                    case "S": 
                        return 18;
                    case "T": 
                        return 19;
                    case "U": 
                        return 20;
                    case "V": 
                        return 21;
                    case "W": 
                        return 22;
                    case "X": 
                        return 23;
                    case "Y": 
                        return 24;
                    case "Z": 
                        return 25;
                }
                throw new System.Exception("no column found for that letter; need to add more to switch");
            }
        #endregion HELPERS
    }
}