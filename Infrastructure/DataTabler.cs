// RESOURCES: // https://msdn.microsoft.com/en-us/library/system.data.datatable(v=vs.110).aspx

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using BaseballScraper.Models;


#pragma warning disable CC0091, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006, MA0016
namespace BaseballScraper.Infrastructure
{
    public class DataTabler
    {
        private readonly Helpers _h = new Helpers();

        public DataTabler() {}



        #region DATA TABLE ------------------------------------------------------------


        // STATUS [ June 26, 2019 ]: this works
        /// <summary>
        ///     Create a new data table
        /// </summary>
        /// <param name="tableName">todo: describe tableName parameter on CreateEmptyDataTable</param>
        /// <returns>
        ///     A new data table based with no columns or rows
        /// </returns>
        /// <example>
        ///     var dataTable = CreateEmptyDataTable("TABLE TITLE");
        /// </example>
        public static DataTable CreateEmptyDataTable (string tableName)
        {
            return new DataTable (tableName);
        }


        // STATUS [ June 26, 2019 ]: this works
        /// <summary>
        ///     Create a new data table; add columns / headers to table of provided strings
        /// </summary>
        /// <param name="tableName">todo: describe tableName parameter on CreateDataTableWithCustomHeaders</param>
        /// <param name="tableHeaders">todo: describe tableHeaders parameter on CreateDataTableWithCustomHeaders</param>
        /// <returns>
        ///     A new data table with headers of your choosing
        /// </returns>
        /// <example>
        ///     string[] tableHeaders = { "Name", "Pitches", "CswPitches", "CSW%" };
        ///     var dataTable = _dataTabler.CreateDataTableWithCustomHeaders("CSW PITCHERS", tableHeaders);
        /// </example>
        public DataTable CreateDataTableWithCustomHeaders(string tableName, string[] tableHeaders)
        {
            DataTable newDataTable = new DataTable (tableName);
            SetDataTableColumnHeaders(tableHeaders, newDataTable);
            return newDataTable;
        }



            // STATUS [ June 26, 2019 ]: NOT SURE IF THIS WORKS
            // / <summary>
            // /     Create a new data table; add columns / headers to table of model property names
            // / </summary>
            // / <returns>
            // /     A new data table based on the model of your choosing
            // / </returns>
            // public DataTable CreateDataTableForModelInstance(string tableName, Object obj)
            // {
            //     DataTable newDataTable = new DataTable (tableName);

            //     Type objectType = obj.GetType();
            //     PropertyInfo [] modelProperties = objectType.GetProperties();

            //     List<string> modelPropertiesStringsList = new List<string>();
            //     foreach(var property in modelProperties)
            //     {
            //         modelPropertiesStringsList.Add(property.Name);
            //     }
            //     SetDataTableColumnHeaders(modelPropertiesStringsList, newDataTable);
            //     return newDataTable;
            // }


        #endregion DATA TABLE ------------------------------------------------------------





        #region DATA TABLE COLUMNS & HEADERS ------------------------------------------------------------


        // STATUS [ June 26, 2019 ]: this works
        /// <summary>
        ///     Given a data table, set the tables headers with the values given in a list
        /// </summary>
        /// <param name="list">
        ///     A list of strings that you want to be the headers
        /// </param>
        /// <param name="dataTable">
        ///     The target data table that you want to set the headers for
        /// </param>
        public static void SetDataTableColumnHeaders (List<string> list, DataTable dataTable)
        {
            foreach (string columnHeader in list)
            {
                DataColumn newDataTableColumn = new DataColumn (columnName: columnHeader, dataType: typeof (string));
                dataTable.Columns.Add (newDataTableColumn);
            }
        }


        // STATUS [ June 26, 2019 ]: this works
        /// <summary>
        ///     Set the column headers of a given table
        /// </summary>
        /// <param name="dataTable">
        ///     The target data table that you want to insert columns and headers into
        /// </param>
        /// <param name="tableHeaders">
        ///     Something like:
        ///         * string[] tableHeaders = { "Name", "Pitches", "CswPitches", "CSW%" };
        /// </param>
        /// <example>
        ///     var dataTable = CreateEmptyDataTable("TABLE TITLE");
        ///     string[] tableHeaders = { "Name", "Pitches", "CswPitches", "CSW%" };
        ///     SetDataTableColumnHeaders(tableHeaders, dataTable);
        /// </example>
        public static void SetDataTableColumnHeaders (string[] tableHeaders, DataTable dataTable)
        {
            foreach (string columnHeader in tableHeaders)
            {
                DataColumn newDataTableColumn = new DataColumn (columnName: columnHeader, dataType: typeof (string));
                dataTable.Columns.Add (newDataTableColumn);
            }
        }


        // STATUS [ June 26, 2019 ]: this works
        // _dataTabler.AddEmptyColumnsToDataTable(dataTable, 5);
        // 'columnsToAdd' is the number of columns you want to add
        //      * they're instantiated with generic column names (e.g., COL 1)
        public static void AddEmptyColumnsToDataTable (DataTable dataTable, int columnsToAdd)
        {
            DataColumn column;
            for(int i = 1; i <= columnsToAdd; i++)
            {
                column = new DataColumn
                {
                    ColumnName = $"COL {i.ToString(CultureInfo.InvariantCulture)}",
                };
                dataTable.Columns.Add(column);
            }
        }


        #endregion DATA TABLE COLUMNS &  HEADERS ------------------------------------------------------------





        #region DATA TABLE ROWS ------------------------------------------------------------


        // STATUS [ June 26, 2019 ]: this works
        /// <summary>
        ///     Insert a row of data within a data table; desired cell value is passed in as a parameter
        /// </summary>
        /// <param name="dataTable">
        ///     The target data table that you want to insert a row into
        /// </param>
        /// <param name="columnHeader">
        ///     The header (string) of the column you want to insert data into
        /// </param>
        /// <param name="cellValue">
        ///     The value that you want to insert into the cell of the column
        /// </param>
        public void InsertDataRowIntoTable(DataTable dataTable, string columnHeader, string cellValue)
        {
            DataRow row = dataTable.NewRow();
            row [columnHeader] = cellValue;
            dataTable.Rows.Add(row);
        }


        // STATUS [ June 26, 2019 ]: this works
        /// <summary>
        ///     Insert a row of data within a data table; desired row/cell values are set through an object passed into method
        /// </summary>
        /// <param name="dataTable">
        ///     The target data table that you want to insert a row into
        /// </param>
        /// <param name="rowData">
        ///     Something like:
        ///         * Object[] pitcherData = { pitcher.PlayerName, pitcher.TotalPitches, pitcher.CswPitches, pitcher.CswPitchPercent };
        /// </param>
        /// <example>
        ///     var dataTable = CreateEmptyDataTable("TABLE TITLE");
        ///     string[] tableHeaders = { "Name", "Pitches", "CswPitches", "CSW%" };
        ///     Object[] pitcherData = { pitcher.PlayerName, pitcher.TotalPitches, pitcher.CswPitches, pitcher.CswPitchPercent };
        ///     InsertDataRowIntoTable(dataTable, pitcherData);
        /// </example>
        public void InsertDataRowIntoTable(DataTable dataTable, object[] rowData)
        {
            dataTable.Rows.Add (rowData);
        }


        #endregion DATA TABLE ROWS ------------------------------------------------------------





        #region NEEDED? ------------------------------------------------------------


        // OPTION 2
        // STATUS: this works but is it needed?
        // / <summary>
        // /     Given a data table, set the tables headers with a group of given data columns
        // / </summary>
        // / <param name="dataTableColumns">
        // /     A column including the column name / text
        // / </param>
        // / <param name="dataTable">
        // /     The target data table that you want to set the headers for
        // / </param>
        // public void SetDataTableColumnHeaders (DataColumn[] dataTableColumns, DataTable dataTable)
        // {
        //     dataTable.Columns.AddRange(dataTableColumns);
        //     // PrintTable(dataTable);
        // }



        // STATUS: this works
        // / <summary>
        // /     Set one data table column as the primary key column
        // / </summary>
        // / <param name="dataTable">
        // /     The target data table that you want to set the primary key column for
        // / </param>
        // / <param name="keyColumnName">
        // /     The column that you want to make the primary key column
        // / </param>
        // public void SetDataTablePrimaryKeyColumn(DataTable dataTable, string keyColumnName)
        // {
        //     dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[keyColumnName] };
        // }

        #endregion NEEDED? ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------


        // STATUS [ June 26, 2019 ]: this works
        /// <summary>
        ///     Print a data table in console
        /// </summary>
        /// <param name="dataTable">
        ///     The data table that you want to print in console
        /// </param>
        public void PrintTable (DataTable dataTable)
        {
            Console.WriteLine($"\n------------------------------------------------");
            Console.WriteLine($"COLUMNS: {dataTable.Columns.Count}\t ROWS: {dataTable.Rows.Count}");
            Console.WriteLine($"------------------------------------------------\n");

            foreach (DataColumn col in dataTable.Columns)
            {
                Console.Write("{0,-30}", col.ColumnName);
            }
            Console.WriteLine();

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (col.DataType.Equals (typeof (DateTime)))
                        Console.Write ("{0,-30:d}", row[col]);

                    else if (col.DataType.Equals (typeof (decimal)))
                        Console.Write ("{0,-30:C}", row[col]);

                    else
                        Console.Write ("{0,-30}", row[col]);
                        // Console.Write ("{0,-14}", row[col]);
                }
                Console.WriteLine ();
            }
            Console.WriteLine();
        }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}

