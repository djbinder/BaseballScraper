// reference: https://github.com/perevoznyk/excel-export/blob/master/README.md



using System;
using System.Globalization;
using System.IO;
using System.Threading;
using BaseballScraper.Models.FanGraphs;
using Export.XLS;
using Microsoft.AspNetCore.Mvc;
using Npoi.Mapper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace BaseballScraper.Infrastructure
{
    public class ExcelMapper
    {
        private Constants _c = new Constants();

        // STATUS: this works
        private void RegisterProviderToStart()
        {
            System.Text.Encoding.RegisterProvider (System.Text.CodePagesEncodingProvider.Instance);
        }

        // STATUS: this works
        private void SetThreadCurrentCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo ("en-US");
        }

        #region EXCEL WORKBOOK / DOCUMENT
            // STATUS: this works
            /// <summary> This creates and saves a new Excel (XLS) existingFile </summary>
            /// <type> XLSX </type>
            /// <param name="fileName"> What you want the existingFile to be named </param>
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
        #endregion TABS/SHEETS


        // STATUS: in progress
        // TODO: the class/model type (FGHitter) is defined within the method. This needs to be passed as an argument
        public void AddRecordToSheet(string fileName, string sheetName)
        {
            RegisterProviderToStart();
            SetThreadCurrentCulture();

            FGHitter newFGHitter = new FGHitter
            {
                FanGraphsName = "David Bote",
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

            var mapper = new Mapper();

            string targetWorkbook = ManageWorkbookNames(fileName);

            // ARGUMENTS
                // (1) string path
                // (2) IEnumerable<T> Objects
                // (3) string sheetName
                // (4) bool overwrite
                    // true = create new workbook
                    // false = workbook already exists so update the existing workbook instead of creating a new onoe
            mapper.Save(targetWorkbook, new[] { newFGHitter }, sheetName, overwrite: false);
        }





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
        private void SetCellValue(ExcelDocument document, int rowNumber, int columnNumber,  object cellValue)
        {
            document[rowNumber, columnNumber].Value = cellValue;
        }


        // OPTION 1
        // STATUS: // TODO: neither option accounts for bold, itaclic etc. that can be added as third parameter to new Font()
        public void SetFont(ExcelDocument document, int rowNumber, string columnLetter,  string fontName, int fontSize)
        {
            int      columnNumber                  = ColumnHeaderLetterToNumber(columnLetter);
            document[rowNumber, columnNumber].Font = new Font(fontName, fontSize);
        }
        // OPTION 2
        public void SetFont(ExcelDocument document, int rowNumber, int columnNumber, string fontName, int fontSize)
        {
            document[rowNumber, columnNumber].Font = new Font(fontName, fontSize);
        }

        // STATUS: this works
        public void FormatDate (ExcelDocument document, int rowNumber, int columnNumber)
        {
            document.Cell(rowNumber, columnNumber).Format = @"dd/mm/yyyy";
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
    }
}