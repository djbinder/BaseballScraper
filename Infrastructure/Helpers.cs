using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using BaseballScraper.Models;
using MarkdownLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BaseballScraper.Infrastructure
{
    public class Helpers
    {
        private static string currentTime     = DateTime.Now.ToShortTimeString();
        public  string Start { get; set; }    = "START";
        public  string Complete { get; set; } = "COMPLETE";


        #region LOGGERS ------------------------------------------------------------

            public void Intro(Object obj, String str)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;

                string upperString = str.ToUpper();

                StackFrame frame = new StackFrame(1, true);

                var lineNumber = frame.GetFileLineNumber();

                Console.WriteLine($"// {upperString} --> {obj} --> [@ Line# {lineNumber}]");
                Console.ResetColor();
                Console.WriteLine();
            }

            // // shortcut console writer
            // public static void Intro(this object Object, string String)
            // {
            //     Console.WriteLine();
            //     Console.ForegroundColor = ConsoleColor.Green;

            //     string     upperString = String.ToUpper();
            //     StackFrame frame       = new StackFrame(1, true);

            //     var lineNumber = frame.GetFileLineNumber();

            //     Console.WriteLine($"// {upperString} --> {Object} --> [@ Line# {lineNumber}]");

            //     Console.ResetColor();
            //     Console.WriteLine();
            // }

            public void TypeAndIntro(Object o, string x)
            {
                o.Intro(x);
                o.GetType().Intro($"TYPE for {x}");
            }

            public void PrintKeysAndValues(Object obj)
            {
                foreach(PropertyInfo property in obj.GetType().GetProperties())
                {
                    var propertyValue = property.GetValue(obj, null).ToString();
                    Console.WriteLine($"{property.Name} --> {propertyValue}");
                }
            }

            public void PrintJObjectItems(JObject JObjectToPrint)
            {
                var responseToJson = JObjectToPrint;
                foreach(var jsonItem in responseToJson)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"{jsonItem.Key.ToUpper()}");
                    Console.ResetColor();
                    Console.WriteLine(jsonItem.Value);
                    Console.WriteLine();
                }
            }

            /// <summary> Serialize a given object to a JSON stream (i.e., take a given object and convert it to JSON ) and print to console </summary>
            /// <param name="obj"> An object; typically a JObject (not certain how it deals with objects besides JObjects) </param>
            public void PrintJsonFromObject (Object obj)
            {
                // _c.Start.ThisMethod();
                //Create a stream to serialize the object to.
                MemoryStream mS = new MemoryStream();

                var objType = obj.GetType();
                Console.WriteLine($"OBJECT TYPE BEING SERIALIZED IS: {objType}");

                // Serializer the given object to the stream
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(objType);

                serializer.WriteObject(mS, obj);
                byte[] json      = mS.ToArray();
                     mS.Position = 0;

                StreamReader sR = new StreamReader(mS);
                // this prints all object content in json format
                Console.WriteLine(sR.ReadToEnd());

                mS.Close();

                Console.WriteLine(Encoding.UTF8.GetString(json, 0, json.Length));
            }

            public void PrintTypes (Type type)
            {
                Console.WriteLine("IsArray: {0}", type.IsArray);
                Console.WriteLine("Name: {0}", type.Name);
                Console.WriteLine("IsSealed: {0}", type.IsSealed);
                Console.WriteLine("BaseType.Name: {0}", type.BaseType.Name);
                Console.WriteLine();
            }

            // STATUS: this works
            /// <summary> Print a data table in console </summary>
            /// <param name="dataTable"> The data table that you want to print in console </param>
            private void PrintDataTable (DataTable dataTable)
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

            // STATUS: this works
            // PRINT | ALL | VARIABLE KEYS AND VALUES
            /// <summary> Print the keys and values from a given IEnumerable </summary>
            /// <examples> PrintKeyValuePairs(pythonKeyValuePairs); </example>
            /// <param name="keyValuePairs"> An IEnumerable containing variable keys and values</param>
            public void PrintKeyValuePairs(IEnumerable<KeyValuePair<string, dynamic>> keyValuePairs)
            {
                int kvpNumber = 1;
                foreach(var kvp in keyValuePairs)
                {
                    Console.WriteLine(kvpNumber);
                    Console.WriteLine($"KEY: {kvp.Key}  VALUE: {kvp.Value}");
                    Console.WriteLine();
                    kvpNumber++;
                }
            }

        #endregion LOGGERS ------------------------------------------------------------



        #region GETTERS ------------------------------------------------------------

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

        #endregion GETTERS ------------------------------------------------------------





        #region MARKERS ------------------------------------------------------------

            // set color of console message
            // example ---> valueX.WriteColor(ConsoleColor.Red)
            // public static void Spotlight<T>(this T x, string Message)
            public void Spotlight (String message)
            {
                string fullMessage = JsonConvert.SerializeObject(message, Formatting.Indented).ToUpper();

                StackFrame frame      = new StackFrame(1, true);
                var        lineNumber = frame.GetFileLineNumber();
                // var lineNumber = GetCurrentLineNumber();

                using (var writer = new System.IO.StringWriter())
                {
                    // change text color
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{fullMessage} @ Line#: {lineNumber}");
                    Console.Write(writer.ToString());
                    Console.ResetColor();
                }
            }

            public void Highlight (String message)
            {
                string fullMessage = JsonConvert.SerializeObject(message, Formatting.Indented).ToUpper();

                using (var writer = new System.IO.StringWriter())
                {
                    // change text color
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{fullMessage}");
                    Console.Write(writer.ToString());
                    Console.ResetColor();
                }
            }

            // https://msdn.microsoft.com/en-us/library/system.io.path.getfilename(v=vs.110).aspx
            public void ThisMethod(String startOrComplete)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine();

                StackTrace stackTrace = new StackTrace();

                // var methodName = GetMethodName();
                var methodName = stackTrace.GetFrame(1).GetMethod().Name;

                StackFrame frame    = new StackFrame(1, true);
                var        method   = frame.GetMethod();
                var        fileName = frame.GetFileName();

                var lineNumber = frame.GetFileLineNumber();

                string fileNameTrimmed = Path.GetFileName(fileName);

                Console.WriteLine($"--------------->|     {fileNameTrimmed} ---> {methodName} {startOrComplete} [Line: {lineNumber} @ {currentTime}]     |<---------------");

                Console.ResetColor();
                Console.WriteLine();
            }

        #endregion MARKERS ------------------------------------------------------------



        #region PROBES ------------------------------------------------------------

            // retrieve high-level info about x
            public void Dig<T>(T x)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;

                string json = JsonConvert.SerializeObject(x, Formatting.Indented);

                Console.WriteLine($"{x} --------------------------- {json} --------------------------- {x}");
                Console.WriteLine();
                Console.ResetColor();
            }

            // retrieve detailed info about x
            public void DigDeep<T>(T x)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                using (var writer = new System.IO.StringWriter())
                {
                    ObjectDumper.Dumper.Dump(x, "Object Dumper", writer);
                    Console.Write(writer.ToString());
                }
                Console.WriteLine();
                Console.ResetColor();
            }

        #endregion PROBES ------------------------------------------------------------



        #region UTILS ------------------------------------------------------------

            // https://msdn.microsoft.com/en-us/library/system.consolekeyinfo(v=vs.110).aspx
            public void ConsoleKey ()
            {
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine(key);
                Console.WriteLine();
                Console.WriteLine("Character Entered: " + key.KeyChar);
                Console.WriteLine("Special Keys: " + key.Modifiers);
            }

            /// <summary> </summary>
            /// <param name="itemsToList"> e.g., string[] planet = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };</param>
            public static void CreateNumberedList(string[] itemsToList)
            {
                Console.Write(itemsToList.ToMarkdownNumberedList());
            }
            public static void CreateBulletedList(string[] itemsToList)
            {
                Console.Write(itemsToList.ToMarkdownBulletedList());
            }

        #endregion UTILS ------------------------------------------------------------


    }
}