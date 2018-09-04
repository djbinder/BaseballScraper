/// <summary> Create connection between .NET and Python; Get and Set variables </summary>
/// <list> Resources used
///     <item> https://medium.com/emoney-engineering/running-python-script-from-c-and-working-with-the-results-843e68d230e5 </item>
/// </list>


using System;
using System.Collections.Generic;
using IronPython.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Scripting.Hosting;

namespace BaseballScraper.Controllers
{
    #pragma warning disable CS0414
    public class PythonController: Controller
    {
        private Constants _c = new Constants();

        [HttpGet]
        [Route("python/start")]

        public void ViewPythonHome()
        {
            _c.Start.ThisMethod();

            var scope = ConnectToPythonFile("HelloWorld.py");
        }

        /// <summary> Create a connection between .NET and a Python file; This must be run before any of the other methods will work </summary>
        /// <remarks> This must be run before any of the other below methods </remarks>
        /// <returns> A connection between .NET and Python </returns>
        public dynamic ConnectToPythonFile(string fileName)
        {
            // Extract Python language engine from their grasp
            // ENGINE type --> Microsoft.Scripting.Hosting.ScriptEngine
            var engine = Python.CreateEngine();

            // Introduce Python namespace (scope)
            // SCOPE type --> Microsoft.Scripting.Hosting.ScriptScope
            var scope = engine.CreateScope();

            // SOURCE type --> Microsoft.Scripting.Hosting.ScriptSource
            ScriptSource source = engine.CreateScriptSourceFromFile(fileName);
            // ScriptSource source = engine.CreateScriptSourceFromFile("HelloWorld.py");

            // any other functions must be called after this
            object connectionToPython = source.Execute(scope);

            // for testing purposes
            GetPythonVariableValue(scope, "x2");

            return scope;
        }

        // STATUS: this works
        // SET | ONE | VARIABLE KEY AND VALUE
        /// <summary> Set variable key (i.e. name) and value in corresponding Python scope / file </summary>
        /// <remarks> This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
        /// <param name="scope"> ScriptScope generated to connect to Python file </param>
        /// <param name="variableKey"> The key in the actual Python file that you want to retrieve the value for </param>
        /// <param name="variableValue"> The value in the actual Python file that you want to retrieve </param>
        /// <example> SetPythonVariableKeyAndValue(scope, "myObjectKey", "my object value"); </example>
        public void SetPythonVariableKeyAndValue(ScriptScope scope, string variableKey, string variableValue)
        {
            // This will be the name of the variable in Python script, initialized with previously created .NET variable
            scope.SetVariable(variableKey, variableValue);
        }

        // STATUS: this works
        /// <summary> Create a dictionary of keys and values that will be set as Python keys and values </summary>
        /// <remarks> Note that keys and values are defined within this function; a dictionary is not passed in as a parameter / argument </remarks>
        /// <example> var dictionaryForPython = CreateDictionaryOfPythonKeysAndValuesToSet(); </example>
        /// <returns> A dictionary of keys and values where keys are strings and values are objects </returns>
        public Dictionary<string, object> CreateDictionaryOfPythonKeysAndValuesToSet ()
        {
            // Add some sample parameters. Notice that there is no need in specifically setting the object type, interpreter will do that part for us in the script properly with high probability
            var keysAndValuesDictionary = new Dictionary<string, object>
            {
                { "personId", 1},
                { "person", "Mookie Betts"}
            };
            return keysAndValuesDictionary;
        }

        // STATUS: this works
        // SET | MANY | VARIABLE KEYS AND VALUES
        /// <summary> Given a dictionary of keys and values, set those keys and values as Python variable keys and value </summary>
        /// <remarks> This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
        /// <param name="scope"> ScriptScope generated to connect to Python file </param>
        /// <param name="keysAndValuesDictionary"> A dictionary of keys and values where keys are strings and values are objects </param>
        /// <param name="dictionaryName"></param>
        /// <example>
        ///     Step 1: Create dictionary --> var newDictionary = new Dictionary<string, object>
        ///     Step 2: Add variable to dictionary --> newDictionary.Add{ "itemId", 37}
        ///     <remarks> to print the dictionary in Python --> 'print('dictionaryName') </remarks>
        /// </example>
        public void SetPythonKeyValueDictionary(ScriptScope scope, Dictionary<string, object> keysAndValuesDictionary, string dictionaryName)
        {
            _c.Start.ThisMethod();

            // this sets the entire dictionary as a Python variable
            scope.SetVariable(dictionaryName, keysAndValuesDictionary);

            // this sets each individual key value pair as a Python variable
            foreach(var dictionaryEntry in keysAndValuesDictionary)
            {
                scope.SetVariable(dictionaryEntry.Key, dictionaryEntry.Value);
                Console.WriteLine($"KEY CHECK: {dictionaryEntry.Key}  VALUE CHECK: {dictionaryEntry.Value}");
            }
        }

        // STATUS: this works
        // GET | ONE | VARIABLE VALUE
        /// <summary> Retrieve a variable value from a Python scope / file </summary>
        /// <remarks> This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
        /// <param name="scope"> ScriptScope generated to connect to Python file </param>
        /// <param name="variableKey"> The key in the actual Python file that you want to retrieve the value for </param>
        /// <example> var parameter = GetPythonVariableValue(scope, "parameter"); Where 'parameter' is the variable name in the Python file </example>
        /// <returns> A value of a variable from a Python file </returns>
        public object GetPythonVariableValue(ScriptScope scope, string variableKey)
        {
            var variableValue = scope.GetVariable<dynamic>(variableKey);

            Console.WriteLine($"KEY: {variableKey}  --> VALUE: {variableValue}");

            return variableValue;
        }

        // STATUS: this works
        // GET | ALL | VARIABLE NAMES
        /// <summary> Retrieve all variable names from a Python scope / file </summary>
        /// <remarks> This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
        /// <param name="scope"> ScriptScope generated to connect to Python file </param>
        /// <example> ListPythonVariableNames(scope); </example>
        /// <returns> A list of the names of each variable within a Python scope / file
        public IEnumerable<string> GetPythonVariableNames(ScriptScope scope)
        {
            IEnumerable<string> pythonVariableNamesList = scope.GetVariableNames();
            foreach(var varName in pythonVariableNamesList)
            {
                Console.WriteLine(varName);
            }
            return pythonVariableNamesList;
        }

        // STATUS: this works
        // GET | ALL | VARIABLE KEYS AND VALUES
        /// <summary> Retrieve variable keys(i.e., names) and values from a Python scope / file </summary>
        /// <remarks> This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
        /// <param name="scope"> ScriptScope generated to connect to Python file </param>
        /// <example> GetPythonKeyValuePairs(scope); </example>
        /// <returns> An IEnumerable of keys and values from a Python scope / file </returns>
        public IEnumerable<KeyValuePair<string, dynamic>> GetPythonKeyValuePairs(ScriptScope scope)
        {
            IEnumerable<KeyValuePair<string, dynamic>> pythonKeyValuePairs = scope.GetItems();

            PrintPythonKeyValuePairs(pythonKeyValuePairs);

            return pythonKeyValuePairs;
        }

        // OPTION 1
        // STATUS: this works
        // PRINT | ALL | VARIABLE KEYS AND VALUES
        /// <summary> Print the keys and values from a given IEnumerable </summary>
        /// <examples> PrintPythonKeyValuePairs(pythonKeyValuePairs); </example>
        /// <param name="pythonKeyValuePairs"> An IEnumerable containing variable keys and values</param>
        public void PrintPythonKeyValuePairs(IEnumerable<KeyValuePair<string, dynamic>> pythonKeyValuePairs)
        {
            int kvpNumber = 1;
            foreach(var kvp in pythonKeyValuePairs)
            {
                Console.WriteLine(kvpNumber);
                Console.WriteLine($"KEY: {kvp.Key}  VALUE: {kvp.Value}");
                Console.WriteLine();
                kvpNumber++;
            }
        }

        // OPTION 2
        // STATUS: ?????
        // PRINT | ALL | VARIABLE KEYS AND VALUES
        /// <summary> Print the keys and values from a given Dictionary </summary>
        /// <examples> PrintPythonKeyValuePairs(pythonKeyValuePairs); </example>
        /// <param name="pythonKeyValuePairs"> A Dictionary containing variable keys and values</param>
        ///
        public void PrintPythonKeyValuePairs(Dictionary<string, object> pythonKeyValuePairs)
        {
            int kvpNumber = 1;
            foreach(var kvp in pythonKeyValuePairs)
            {
                Console.WriteLine(kvpNumber);
                Console.WriteLine($"KEY: {kvp.Key}  VALUE: {kvp.Value}");
                Console.WriteLine();
                kvpNumber++;
            }
        }
    }
}