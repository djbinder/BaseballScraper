using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

using BaseballScraper.Models;
using BaseballScraper.Models.FanGraphs;
using Microsoft.AspNetCore.Mvc;
using Npoi.Mapper;

using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

using Export.XLS;



namespace BaseballScraper.Mappers
{
    public class NpoiMapper: Controller
    {
        private Constants _c = new Constants();

        // THIS WORKS
        public void CreateEmptyXlsx (string FileName, string SheetName, int StartStop)
        {
            _c.Start.ThisMethod ();
            if (StartStop == 1)
            {
                System.Text.Encoding.RegisterProvider (System.Text.CodePagesEncodingProvider.Instance);
                Extensions.Spotlight("A");
                // Npoi.Mapper.Mapper
                var    mapper       = new Mapper ();
                string NewFileName  = $"{FileName}.xlsx";
                string NewSheetName = SheetName;
                Dummy  dummy        = new Dummy ();

                Extensions.Spotlight("B");
                // ModifyFormatXLSX(mapper, NewSheetName);

                mapper.Save (NewFileName, new [] { dummy }, NewSheetName);
                Extensions.Spotlight("C");
                var dateCell = mapper.Workbook.GetSheetAt (0).GetRow (1).GetCell (1);
                dateCell.Intro ("date cell");
                Console.WriteLine (dateCell.GetType ());

                Extensions.Spotlight("D");
                string xD = dateCell.ToString ();
                xD.Intro ("xd");
                Console.WriteLine (xD.GetType ());
                Extensions.Spotlight("E");
            }
        }

        // THIS WORKS
        public void GetAllRecordsInSheet (string FileName, string SheetName)
        {
            System.Text.Encoding.RegisterProvider (System.Text.CodePagesEncodingProvider.Instance);

            var mapper = new Mapper (FileName);

            // e.g., Npoi.Mapper.Mapper+<Take>d__66`1[BaseballScraper.Models.FGHitter]
            var allRecords = mapper.Take<FGHitter> (SheetName);

            allRecords.Count ().Intro ("count of records");
            allRecords.Dig ();

            Console.WriteLine (allRecords);
        }


        // public string OneCellsData (this Mapper mapper, this Workbook workbook, int index, int row, int column)
        // {

        // }


        public void ModifyFormatXlsx (Mapper mapper, string sheetName)
        {
            var dateFormat   = "yyyy.MM.dd hh.mm.ss";
            var doubleFormat = "0%";
            // if (File.Exists(FileName)) File.Delete(FileName);

            mapper.UseFormat (typeof (DateTime), dateFormat);
            mapper.UseFormat (typeof (double), doubleFormat);
        }



        // THIS WORKS
        // add a new row to an existing sheet
        public void AddRowToExistingXlsx (FGHitter hitter, int StartStop)
        {
            if (StartStop == 1)
            {
                // _c.Start.ThisMethod ();

                System.Text.Encoding.RegisterProvider (System.Text.CodePagesEncodingProvider.Instance);

                FGHitter NewFGHitter = hitter;
                var      mapper      = new Mapper ();

                // excel doc to save too, then objects you want to save, then sheet name, 'overwrite: false' means it will save to an existing excel file vs creating a new file
                mapper.Save ("test.xlsx", new [] { NewFGHitter }, "FG_Hitters", overwrite : false);
            }
        }



        // THIS WORKS
        // https://github.com/dotnetcore/NPOI/blob/master/samples/Npoi.Samples.CreateNewSpreadsheet/Program.cs
        public void CreateEmptyXls (string FileName, string SheetName, int StartStop)
        {
            _c.Start.ThisMethod ();

            Thread.CurrentThread.CurrentCulture = new CultureInfo ("en-US");

            var NewFile = $"{FileName}.xls";

            using (var fs = new FileStream (NewFile, FileMode.Create, FileAccess.Write))
            {
                // WORKBOOK ---> NPOI.HSSF.UserModel.HSSFWorkbook
                IWorkbook workbook = new HSSFWorkbook ();

                // SHEET 1 ---> NPOI.HSSF.UserModel.HSSFSheet
                // ISheet sheet1 = workbook.CreateSheet (SheetName);
                // sheet1.Intro ("sheet 1");

                // MergeRowsInXls (workbook, sheet1);



                // var rowIndex = 0;

                // // ROW ---> NPOI.HSSF.UserModel.HSSFRow
                // IRow row = sheet1.CreateRow (rowIndex);
                //         row.Height = 30 * 80;

                // var cell = sheet1.CreateRow (1).CreateCell (1);
                //     cell.SetCellValue (1);
                // // var sheet2 = workbook.CreateSheet("sheet2");

                workbook.Write (fs);
            }
            _c.Complete.ThisMethod ();
        }



        // THIS WORKS
        public Object MergeRowsInXls (IWorkbook TargetWB, ISheet TargetS)
        {
            // _c.Start.ThisMethod ();

            // NPOI.HSSF.UserModel.HSSFWorkbook
            IWorkbook wb = TargetWB;

            // NPOI.HSSF.UserModel.HSSFWorksheet
            ISheet s = TargetS;

            // this is a count of the number of cells to combine
            int CellsDownToCombine = 20;
            // this is where to start combining; 0 equal the first row of the sheet
            int CellsDownToBeginCombining = 10;

            // this is a count of the number of cells to combine
            int CellsRightToCombine = 5;
            // this is where to start combining; 0 equals column A
            int CellsRightToBeginCombining = 0;

            var MergedRow = s.AddMergedRegion (new CellRangeAddress (
                CellsDownToBeginCombining,
                CellsDownToCombine,
                CellsRightToBeginCombining,
                CellsRightToCombine
            ));

            return MergedRow;
        }


        public void CreateEmptyWorkbook ()
        {
            // _c.Start.ThisMethod ();

            var newFile = @"newbook.core.xls";

            using (var fs = new FileStream (newFile, FileMode.Create, FileAccess.Write))
            {
                // WORKBOOK ---> NPOI.HSSF.UserModel.HSSFWorkbook
                IWorkbook workbook = new HSSFWorkbook ();

                // SHEET 1 ---> NPOI.HSSF.UserModel.HSSFSheet
                ISheet sheet1 = workbook.CreateSheet ("Sheet1");
                sheet1.AddMergedRegion (new CellRangeAddress (0, 0, 0, 10));

                var rowIndex = 0;

                // ROW ---> NPOI.HSSF.UserModel.HSSFRow
                IRow row        = sheet1.CreateRow (rowIndex);
                     row.Height = 30 * 80;

                var cell = sheet1.CreateRow (1).CreateCell (1);
                cell.SetCellValue (1);

                var CellRow = cell.RowIndex;

                var CellColumnIndex = cell.ColumnIndex;

                // FONT ---> NPOI.HSSF.UserModel.HSSFFont{[FONT]
                var font        = workbook.CreateFont ();
                    font.IsBold = true;
                    font.Color  = HSSFColor.DarkBlue.Index2;

                cell.CellStyle.SetFont (font);

                rowIndex++;

                // SHEET 2 [@ Line#: 98] ---> NPOI.HSSF.UserModel.HSSFSheet
                var sheet2 = workbook.CreateSheet ("Sheet2");

                var style1                     = workbook.CreateCellStyle ();
                    style1.FillForegroundColor = HSSFColor.Blue.Index2;
                    style1.FillPattern         = FillPattern.SolidForeground;

                var style2                     = workbook.CreateCellStyle ();
                    style2.FillForegroundColor = HSSFColor.Yellow.Index2;
                    style2.FillPattern         = FillPattern.SolidForeground;

                var cell2           = sheet2.CreateRow (0).CreateCell (0);
                    cell2.CellStyle = style1;
                cell2.SetCellValue (0);

                cell2           = sheet2.CreateRow (1).CreateCell (0);
                cell2.CellStyle = style2;
                cell2.SetCellValue (1);

                workbook.Write (fs);
            }
            // _c.Complete.ThisMethod ();
        }
    }
}



// FGHitter NewFGHitter = new FGHitter
// {

//     Name = "Javier Baez",
//     Team = "Chicago Cubs",
//     GP = "123",
//     PA = "123",
//     HR = "123",
//     R = "123",
//     RBI = "123",
//     SB = "123",
//     BB_percent = "23%",
//     K_percent = "23%",
//     ISO = ".321",
//     BABIP = ".321",
//     AVG = ".321",
//     OBP = ".321",
//     SLG = ".321",
//     wOBA = ".321",
//     wRC_plus = "789",
//     BsR = "789",
//     Off = "789",
//     Def = "789",
//     WAR = "6",
// };
