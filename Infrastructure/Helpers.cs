using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE0063, IDE0067, IDE1006, MA0016
namespace BaseballScraper.Infrastructure
{
    public class Helpers
    {
        public Helpers() {}

        private static string currentTime = DateTime.Now.ToShortTimeString();




        #region LOGGERS ------------------------------------------------------------


            public void Intro(object obj, string str)
            {
                if (obj is null)
                    throw new ArgumentNullException(nameof(obj));
                

                if (string.IsNullOrEmpty(str))
                    throw new ArgumentException("message", nameof(str));


                Console.ForegroundColor = ConsoleColor.Green;
                StackFrame frame        = new StackFrame(1, fNeedFileInfo: true);
                var lineNumber          = frame.GetFileLineNumber();
                Console.WriteLine($"\n{str.ToUpper(CultureInfo.InvariantCulture)} --> {obj} --> [@ Line# {lineNumber}]\n");
                Console.ResetColor();
            }

            public void TypeAndIntro(object o, string x)
            {
                Intro(o, x);
                Console.WriteLine($"Type for {x} --> {o.GetType()}");
            }


            // public void PrintKeysAndValues(Object obj)
            // {
            //     foreach(PropertyInfo property in obj.GetType().GetProperties())
            //     {
            //         var propertyValue = property.GetValue(obj, null).ToString();
            //         Console.WriteLine($"{property.Name} --> {propertyValue}");
            //     }
            // }


            public void PrintJObjectItems(JObject JObjectToPrint)
            {
                var responseToJson = JObjectToPrint;
                foreach(var jsonItem in responseToJson)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"{jsonItem.Key.ToUpper(CultureInfo.InvariantCulture)}");
                    Console.ResetColor();
                    Console.WriteLine($"{jsonItem.Value}\n");
                }
            }


            /// <summary>
            ///     Serialize a given object to a JSON stream (i.e., take a given object and convert it to JSON ) and print to console
            /// </summary>
            /// <param name="obj">
            ///     An object; typically a JObject
            ///     Not certain how it deals with objects besides JObjects)
            /// </param>
            public void PrintJsonFromObject (object obj)
            {
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

                sR.Dispose();
                mS.Close();

                Console.WriteLine(Encoding.UTF8.GetString(json, 0, json.Length));
            }




            // STATUS: this works
            // PRINT | ALL | VARIABLE KEYS AND VALUES
            /// <summary>
            ///     Print the keys and values from a given IEnumerable
            /// </summary>
            /// <param name="keyValuePairs">
            ///     An IEnumerable containing variable keys and values
            /// </param>
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


            public void PrintKeyValuePairs(JObject obj)
            {
                // KEY VALUE PAIR --> KeyValuePair<string, JToken> recordObject
                foreach(var keyValuePair in obj)
                {
                    var key   = keyValuePair.Key;
                    var value = keyValuePair.Value;
                    Console.WriteLine($"Key: {keyValuePair.Key}    Value: {keyValuePair.Value}");
                }
            }


            public void PrintNameSpaceControllerNameMethodName(Type type)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                StackTrace stackTrace   = new StackTrace();
                StackFrame frame        = new StackFrame(1, fNeedFileInfo: true);

                string methodName;

                try
                {
                    methodName = stackTrace.GetFrame(2).GetMethod().Name;
                }

                catch(Exception ex)
                {
                    methodName = stackTrace.GetFrame(1).GetMethod().Name;
                    Console.WriteLine(ex.Message);
                }

                Console.WriteLine($"NAME_SPACE : {type.Namespace}");
                Console.WriteLine($"CONTROLLER : {type.Name}");
                Console.WriteLine($"METHOD     : {methodName} @ LINE: {frame.GetFileLineNumber()}");

                Console.ResetColor();
            }


        #endregion LOGGERS ------------------------------------------------------------





        #region GETTERS ------------------------------------------------------------

            // STATUS: //TO-DO: need to be able to pass a model in as a parameter; it's currently hardcoded into the function
            // / <summary> Given a model / class, get the properties of that model </summary>
            // / <returns> Model properties for a given class (e.g, FanGraphsPitcher) </returns>
            // public PropertyInfo[] GetModelProperties()
            // {
            //     TheGameIsTheGameCategories model = new TheGameIsTheGameCategories();

            //     Type         modelType          = model.GetType();
            //     PropertyInfo [] modelProperties = modelType.GetProperties();
            //     return modelProperties;
            // }

            // STATUS: //TO-DO: need to be able to pass a model in as a parameter to the GetModelProperties() function within the method
            // / <summary> Given a model / class, create a list(string) of the models property names (e.g, Wins) </summary>
            // / <returns> A list of property names </returns>
            // public List<string> CreateListOfModelProperties()
            // {
            //     PropertyInfo [] modelProperties           = GetModelProperties();
            //     List         <String> modelPropertiesList = new List<string>();

            //     int headerCount = 1;
            //     foreach(var prop in modelProperties)
            //     {
            //         // Console.WriteLine($"Header {headerCount}: {prop.Name}");
            //         modelPropertiesList.Add(prop.Name);
            //         headerCount++;
            //     }
            //     Console.WriteLine($"Final list item count: {modelPropertiesList.Count}");
            //     return modelPropertiesList;
            // }

        #endregion GETTERS ------------------------------------------------------------




        #region ITERATORS ------------------------------------------------------------


            public void IterateForEach(List<dynamic> list)
            {
                foreach(var listItem in list)
                {
                    Console.WriteLine(listItem);
                }
            }


            public void IterateForEach(IEnumerable<dynamic> list)
            {
                foreach(var listItem in list)
                {
                    Console.WriteLine(listItem);
                }
            }


        #endregion ITERATORS ------------------------------------------------------------





        #region MARKERS ------------------------------------------------------------


            public void Spotlight (string message)
            {
                string fullMessage = JsonConvert.SerializeObject(message, Formatting.Indented).ToUpper(CultureInfo.InvariantCulture);

                StackFrame frame      = new StackFrame(1, fNeedFileInfo: true);
                var        lineNumber = frame.GetFileLineNumber();

                using (StringWriter writer = new StringWriter())
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{fullMessage} @ Line#: {lineNumber}");
                    Console.Write(writer.ToString());
                    Console.ResetColor();
                }
            }


            public void Highlight (string message)
            {
                string fullMessage = JsonConvert.SerializeObject(message, Formatting.Indented).ToUpper(CultureInfo.InvariantCulture);

                using (StringWriter writer = new StringWriter())
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"{fullMessage}");
                    Console.Write(writer.ToString());
                    Console.ResetColor();
                }
            }

            public string GetMethodName()
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                StackTrace stackTrace = new StackTrace();

                var methodName = stackTrace.GetFrame(1).GetMethod().Name;

                StackFrame frame    = new StackFrame(1, fNeedFileInfo: true);
                var        method   = frame.GetMethod();
                var        fileName = frame.GetFileName();

                Type type = MethodBase.GetCurrentMethod().DeclaringType;
                var typeString = type.ToString();
                string fileNameTrimmed = Path.GetFileName(fileName);
                string methodDetails = $"{typeString} > {fileNameTrimmed}";
                Console.WriteLine($"frame: {frame}\t method: {method}\t fileName: {fileName}\t fileNameTrimmed: {fileNameTrimmed}");
                Console.ResetColor();
                return methodDetails;
            }


            // https://msdn.microsoft.com/en-us/library/system.io.path.getfilename(v=vs.110).aspx
            public void StartMethod()
            {
                Console.ForegroundColor = ConsoleColor.Blue;

                StackTrace stackTrace = new StackTrace();

                // var methodName = GetMethodName();
                var methodName = stackTrace.GetFrame(1).GetMethod().Name;

                StackFrame frame    = new StackFrame(1, fNeedFileInfo: true);
                var        method   = frame.GetMethod();
                var        fileName = frame.GetFileName();

                var lineNumber = frame.GetFileLineNumber();

                string fileNameTrimmed = Path.GetFileName(fileName);

                Console.WriteLine($"\n**********|\t{fileNameTrimmed} ---> START {methodName}  [Line: {lineNumber} @ {currentTime}]\t|**********\n");

                Console.ResetColor();
            }


            // If non-async method, set frameNumber to 1
            // If async method, set frameNumber to 3
            public void OpenMethod(int frameNumber)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                StackTrace stackTrace  = new StackTrace();
                string methodName      = stackTrace.GetFrame(frameNumber).GetMethod().Name;
                StackFrame frame       = new StackFrame(1, fNeedFileInfo: true);
                string fileName        = frame.GetFileName();
                int lineNumber         = frame.GetFileLineNumber();
                string fileNameTrimmed = Path.GetFileName(fileName);

                Console.WriteLine($"OPEN  [ Line {lineNumber} @ {currentTime} ] {fileNameTrimmed} > {methodName} [{frameNumber}]");
                Console.ResetColor();
            }



            // https://msdn.microsoft.com/en-us/library/system.io.path.getfilename(v=vs.110).aspx
            public void CompleteMethod()
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                StackTrace stackTrace = new StackTrace();

                var methodName = stackTrace.GetFrame(1).GetMethod().Name;

                StackFrame frame    = new StackFrame(1, fNeedFileInfo: true);
                // var        method   = frame.GetMethod();
                var        fileName = frame.GetFileName();

                var lineNumber = frame.GetFileLineNumber();

                string fileNameTrimmed = Path.GetFileName(fileName);

                Console.WriteLine($"\n**********|\t{fileNameTrimmed} ---> COMPLETED {methodName}  [Line: {lineNumber} @ {currentTime}]\t|**********\n");

                Console.ResetColor();
            }

            public void CloseMethod(int frameNumber)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                StackTrace stackTrace  = new StackTrace();
                string methodName      = stackTrace.GetFrame(frameNumber).GetMethod().Name;
                StackFrame frame       = new StackFrame(1, fNeedFileInfo: true);
                string fileName        = frame.GetFileName();
                int lineNumber         = frame.GetFileLineNumber();
                string fileNameTrimmed = Path.GetFileName(fileName);

                Console.WriteLine($"CLOSE [ Line {lineNumber} @ {currentTime} ] {fileNameTrimmed} > {methodName} [{frameNumber}]");
                Console.ResetColor();
            }


        #endregion MARKERS ------------------------------------------------------------





        #region PROBES ------------------------------------------------------------


            public void Dig<T>(T x)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                string json = JsonConvert.SerializeObject(x, Formatting.Indented);

                Console.WriteLine($"\n------------------------------------------------------------------");
                Console.WriteLine("BEGIN DIG");
                Console.WriteLine("------------------------------------------------------------------");
                Console.WriteLine(json);
                Console.WriteLine($"------------------------------------------------------------------\n");
                Console.ResetColor();
            }


            public void DigObj(object obj)
            {
                string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                Console.WriteLine($"\n------------------------------------------------------------------");
                Console.WriteLine("BEGIN DIG");
                Console.WriteLine("------------------------------------------------------------------");
                Console.WriteLine($"{obj} --------------------------- {json} --------------------------- {obj}");
                Console.WriteLine($"------------------------------------------------------------------\n");
                Console.WriteLine();
            }


            public void DigDeep<T>(T x)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                using (var writer = new StringWriter())
                {
                    ObjectDumper.Dumper.Dump(x, "Object Dumper", writer);
                    Console.Write(writer.ToString());
                }
                Console.WriteLine();
                Console.ResetColor();
            }


        #endregion PROBES ------------------------------------------------------------





        #region CONVERTERS ------------------------------------------------------------


            public string ConvertJTokenToString(JToken valueJToken)
            {
                string valueString = valueJToken.ToObject<string>();
                return valueString;
            }


            public int ConvertStringToInt(string valueString)
            {
                int valueInt = int.Parse(valueString, CultureInfo.InvariantCulture);
                return valueInt;
            }


            // HttpContext.Session.SetObjectAsJson("TheList", NewList);
            public void SetObjectAsJson (ISession session, string key, object value)
            {
                session.SetString (key, JsonConvert.SerializeObject (value));
            }


            //List<object> Retrieve = HttpContext.Session.GetObjectFromJson<List<object>>("TheList");
            public T GetObjectFromJson<T> (ISession session, string key)
            {
                var value = session.GetString (key);
                return value == null ? default : JsonConvert.DeserializeObject<T> (value);
            }


        #endregion CONVERTERS ------------------------------------------------------------





        #region ENUMERATORS ------------------------------------------------------------


            // STATUS: this works
            // example:
                // var dynamicRecords = csvReader.GetRecords<dynamic>();
                // EnumerateOverRecordsDynamic(dynamicRecords);
            public void EnumerateOverRecordsDynamic(IEnumerable<dynamic> records)
            {
                if (records is null)
                    throw new ArgumentNullException(nameof(records));

                // DYNAMIC RECORDS --> CsvHelper.CsvReader+<GetRecords>d__63`1[System.Object]
                // DYNAMIC RECORDS --> System.Collections.Generic.IEnumerable<dynamic> dynamicRecords
                // DYNAMIC RECORD type --> System.Dynamic.ExpandoObject
                foreach(var record in records)
                {
                    Console.WriteLine(record);
                }
            }


            public void EnumerateOverRecordsObject(IEnumerable<object> records)
            {
                if (records is null)
                    throw new ArgumentNullException(nameof(records));

                foreach (var record in records)
                {
                    Dig(record);
                }
                }


            public void EnumerateOverRecords(IEnumerable<object> records)
            {
                if (records is null)
                    throw new ArgumentNullException(nameof(records));

                IEnumerator<object> recordsEnumerator = records.GetEnumerator();
                while(recordsEnumerator.MoveNext())
                {
                    Console.WriteLine(recordsEnumerator.Current);
                }
            }


        #endregion ENUMERATORS ------------------------------------------------------------





        #region UTILS ------------------------------------------------------------


            // https://msdn.microsoft.com/en-us/library/system.consolekeyinfo(v=vs.110).aspx
            // public void ConsoleKey ()
            // {
            //     ConsoleKeyInfo key = Console.ReadKey();
            //     Console.WriteLine(key);
            //     Console.WriteLine($"\nCharacter Entered: {key.KeyChar}");
            //     Console.WriteLine("Special Keys: " + key.Modifiers);
            // }


            // / <summary> </summary>
            // / <param name="itemsToList"> e.g., string[] planet = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };</param>
            // public static void CreateNumberedList(string[] itemsToList)
            // {
            //     Console.Write(itemsToList.ToMarkdownNumberedList());
            // }
            // public static void CreateBulletedList(string[] itemsToList)
            // {
            //     Console.Write(itemsToList.ToMarkdownBulletedList());
            // }

            // public int CountRecords(IEnumerable<object> records)
            // {
            //     int count = 0;
            //     foreach(var record in records)
            //     {
            //         count++;
            //     }
            //     Console.WriteLine($"Retrieved {count} records from csv");
            //     return count;
            // }


        #endregion UTILS ------------------------------------------------------------





        #region DIAGNOSTICS ------------------------------------------------------------


            // DIAGNOSER: Option 1
            // https://andrewlock.net/logging-using-diagnosticsource-in-asp-net-core/
            public class MiddlewareDiagnoser
            {
                private readonly RequestDelegate _next;
                private readonly DiagnosticSource _diagnostics;

                public MiddlewareDiagnoser(RequestDelegate next, DiagnosticSource diagnosticSource)
                {
                    _next = next;
                    _diagnostics = diagnosticSource;
                }

                public async Task Invoke(HttpContext context)
                {
                    Console.WriteLine("Diagnostics > Invoke");
                    if (_diagnostics.IsEnabled("DiagnosticListenerExample.MiddlewareStarting"))
                    {
                        _diagnostics.Write("DiagnosticListenerExample.MiddlewareStarting",
                            new
                            {
                                httpContext = context,
                            });
                    }

                    await _next.Invoke(context).ConfigureAwait(false);
                }
            }

            // DIAGNOSER: Option 1
            // https://andrewlock.net/logging-using-diagnosticsource-in-asp-net-core/
            public class MiddlewareDiagnoserListener
            {
                [DiagnosticName("DiagnosticListenerExample.MiddlewareStarting")]
                public virtual void OnMiddlewareStarting(HttpContext httpContext)
                {
                    Console.WriteLine($"\n----------------------------------------------------------\nPATH >{httpContext.Request.Path}");
                    Console.WriteLine("----------------------------------------------------------\n");
                    Console.WriteLine($"Method: \n{httpContext.Request.Method}\n");
                    Console.WriteLine($"Query: \n{httpContext.Request.Query}\n");
                    Console.WriteLine($"Content Type \n{httpContext.Request.ContentType}");
                    Console.WriteLine("----------------------------------------------------------\n");
                }
            }


            // DIAGNOSER: Option 2
            // https://andrewlock.net/understanding-your-middleware-pipeline-with-the-middleware-analysis-package/
            public class FullDiagnosticListener
            {
                [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareStarting")]
                public virtual void OnMiddlewareStarting(HttpContext httpContext, string name)
                {
                    Console.WriteLine("MIDDLEWARE STARTING");
                    Console.WriteLine($"{name}; {httpContext.Request.Path}\n");
                }

                [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareException")]
                public virtual void OnMiddlewareException(Exception exception, string name)
                {
                    Console.WriteLine("MIDDLEWARE EXCEPTION");
                    Console.WriteLine($"{name}; {exception.Message}\n");
                }

                [DiagnosticName("Microsoft.AspNetCore.MiddlewareAnalysis.MiddlewareFinished")]
                public virtual void OnMiddlewareFinished(HttpContext httpContext, string name)
                {
                    Console.WriteLine("MIDDLEWARE FINISHED");
                    Console.WriteLine($"{name}; {httpContext.Response.StatusCode}\n");
                }
            }


        #endregion DIAGNOSTICS ------------------------------------------------------------

    }
}
