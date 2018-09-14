// RESOURCES: // https://msdn.microsoft.com/en-us/library/system.data.datatable(v=vs.110).aspx

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using BaseballScraper.Models;

namespace BaseballScraper.Infrastructure
{
    public class DataTabler
    {
        private Helpers _h = new Helpers();


        #region MODEL HANDLERS ----------------------------------------

            // STATUS: //TODO: need to be able to pass a model in as a parameter; it's currently hardcoded into the function
            /// <summary> Given a model / class, get the properties of that model </summary>
            /// <returns> Model properties for a given class (e.g, FanGraphsPitcher) </returns>
            public PropertyInfo[] GetModelProperties()
            {
                TheGameIsTheGameCategories model = new TheGameIsTheGameCategories();

                Type         modelType          = model.GetType();
                PropertyInfo [] modelProperties = modelType.GetProperties();
                return modelProperties;
            }

            // OPTION 1
            // STATUS: //TODO: need to be able to pass a model in as a parameter to the GetModelProperties() function within the method
            /// <summary> Given a model / class, create a list(string) of the models property names (e.g, Wins) </summary>
            /// <returns> A list of property names </returns>
            public List<string> CreateListOfModelProperties()
            {
                PropertyInfo [] modelProperties           = GetModelProperties();
                List         <String> modelPropertiesList = new List<string>();

                int headerCount = 1;
                foreach(var prop in modelProperties)
                {
                    // Console.WriteLine($"Header {headerCount}: {prop.Name}");
                    modelPropertiesList.Add(prop.Name);
                    headerCount++;
                }
                Console.WriteLine($"Final list item count: {modelPropertiesList.Count}");
                return modelPropertiesList;
            }

            // OPTION 2
            // STATUS: this probably works but is it needed?
            public DataColumn[] CreateDataColumnsOfModelProperties()
            {
                PropertyInfo [] modelProperties  = GetModelProperties();
                DataColumn   [] dataTableColumns = {};

                var newDataTableColumn = new DataColumn ("H", typeof(Int32));
                dataTableColumns.Append(newDataTableColumn);

                newDataTableColumn = new DataColumn ("AB", typeof(Int32));
                dataTableColumns.Append(newDataTableColumn);

                return dataTableColumns;
            }

        #endregion MODEL HANDLERS ----------------------------------------


        #region DATA TABLE ----------------------------------------

            // STATUS: //TODO: need to be able to pass a model in as a parameter to the CreateListOfModelProperties() method
            /// <summary> Create a new data table </summary>
            /// <returns> A new data table based on the model of your choosing </returns>
            public DataTable CreateDataTable (string tableName)
            {
                DataTable newDataTable        = new DataTable (tableName);
                var       newDataTableColumns = CreateListOfModelProperties();

                SetDataTableColumnHeaders(newDataTableColumns, newDataTable);

                foreach(var column in newDataTableColumns)
                {
                    InsertDataRowIntoTable(newDataTable);
                    // InsertDataRowIntoTable(newDataTable, column , "100");
                }
                PrintTable(newDataTable);
                return newDataTable;
            }

        #endregion DATA TABLE ----------------------------------------


        #region COLUMNS ----------------------------------------

            // OPTION 1
            // STATUS: this works
            /// <summary> Given a data table, set the tables headers with the values given in a list </summary>
            /// <param name="list"> A list of strings that you want to be the headers </param>
            /// <param name="dataTable"> The target data table that you want to set the headers for </param>
            public void SetDataTableColumnHeaders (List<string> list, DataTable dataTable)
            {
                foreach (var columnHeader in list)
                {
                    DataColumn newDataTableColumn = new DataColumn (columnHeader, typeof (String));
                    dataTable.Columns.Add (newDataTableColumn);
                }
                PrintTable(dataTable);
            }

            // OPTION 2
            // STATUS: this works but is it needed?
            /// <summary> Given a data table, set the tables headers with a group of given data columns </summary>
            /// <param name="dataTableColumns"> A column including the column name / text </param>
            /// <param name="dataTable"> The target data table that you want to set the headers for </param>
            public void SetDataTableColumnHeaders (DataColumn[] dataTableColumns, DataTable dataTable)
            {
                dataTable.Columns.AddRange(dataTableColumns);
                PrintTable(dataTable);
            }

            // STATUS: this works
            /// <summary> Set one data table column as the primary key column </summary>
            /// <param name="dataTable"> The target data table that you want to set the primary key column for </param>
            /// <param name="keyColumnName"> The column that you want to make the primary key column </param>
            public void SetDataTablePrimaryKeyColumn(DataTable dataTable, string keyColumnName)
            {
                dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[keyColumnName] };
            }

        #endregion COLUMNS ----------------------------------------



        #region ROWS ----------------------------------------

            // OPTION 1
            // STATUS: this works
            /// <summary> Insert a row of data within a data table; desired cell value is passed in as a parameter </summary>
            /// <param name="dataTable"> The target data table that you want to insert a row into </param>
            /// <param name="dataTableColumn"> The target column that you want to insert data into </param>
            /// <param name="cellValue"> The value that you want to insert into the cell of the column </param>
            public void InsertDataRowIntoTable(DataTable dataTable, string dataTableColumn, string cellValue)
            {
                _h.StartMethod();

                DataRow row               = dataTable.NewRow();
                row     [dataTableColumn] = cellValue;
                dataTable.Rows.Add(row);
            }

            // OPTION 2
            // STATUS: this works but is not optimal
            /// <summary> Insert a row of data within a data table; desired cell values are set within the method </summary>
            /// <param name="dataTable"> The target data table that you want to insert a row into </param>
            public void InsertDataRowIntoTable(DataTable dataTable)
            {
                _h.StartMethod();

                DataRow row     = dataTable.NewRow();
                row     ["H"]   = "100";
                row     ["AB"]  = "200";
                row     ["R"]   = "200";
                row     ["Hr"]  = "80";
                row     ["Rbi"] = "600";
                row     ["Sb"]  = "10";
                row     ["Bb"]  = "179";
                dataTable.Rows.Add(row);
            }


            // OPTION 3
            // STATUS: not sure if this works
            /// <summary> Insert a row of data within a data table; desired row/cell values are set within a group of objects </summary>
            /// <param name="dataTable"> The target data table that you want to insert a row into </param>
            /// <param name="rowNumber"> ? Not sure this is needed </param>
            public void InsertDataRowIntoTable(DataTable dataTable, int rowNumber)
            {
                Object[] rows = {
                    new Object[] { 1, "O0001", "Mountain Bike", 1419.5, 36 },
                    new Object[] { 2, "O0001", "Road Bike", 1233.6, 16 },
                    new Object[] { 3, "O0001", "Touring Bike", 1653.3, 32 },
                    new Object[] { 4, "O0002", "Mountain Bike", 1419.5, 24 },
                    new Object[] { 5, "O0002", "Road Bike", 1233.6, 12 },
                    new Object[] { 6, "O0003", "Mountain Bike", 1419.5, 48 },
                    new Object[] { 7, "O0003", "Touring Bike", 1653.3, 8 },
            };

                foreach (Object[] row in rows)
                {
                    dataTable.Rows.Add (row);
                }
            }

        #endregion ROWS ----------------------------------------



        #region HELPERS ----------------------------------------
            // STATUS: this works
            /// <summary> Print a data table in console </summary>
            /// <param name="dataTable"> The data table that you want to print in console </param>
            private void PrintTable (DataTable dataTable)
            {
                foreach (DataColumn col in dataTable.Columns)
                {
                    Console.Write ("{0,-14}", col.ColumnName);
                }
                Console.WriteLine ();

                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        if (col.DataType.Equals (typeof (DateTime)))
                            Console.Write ("{0,-14:d}", row[col]);

                        else if (col.DataType.Equals (typeof (Decimal)))
                            Console.Write ("{0,-14:C}", row[col]);

                        else
                            Console.Write ("{0,-14}", row[col]);
                    }
                    Console.WriteLine ();
                }
                Console.WriteLine ();
            }
        #endregion HELPERS ----------------------------------------
    }
}