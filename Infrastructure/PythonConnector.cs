/// <summary> Create connection between .NET and Python; Get and Set variables </summary>

// https://docs.python-guide.org/dev/virtualenvs/
// https://docs.pipenv.org/en/latest/install/#installing-pipenv
// https://github.com/simplic/simplic-dlr
// https://medium.com/better-programming/running-python-script-from-c-and-working-with-the-results-843e68d230e5
// https://pythonnet.github.io
// https://stackoverflow.com/questions/42343676/how-to-add-modules-to-iron-python
// https://stackoverflow.com/questions/39187374/how-to-correctly-uninstall-numpy-on-macosx
// https://www.codeproject.com/Articles/602112/Scripting-NET-Applications-with-IronPython
// https://www.needfulsoftware.com/IronPython/IronPythonCS

// also check Evernote Python | Running Python | .main file

using System;
using System.Collections.Generic;
using IronPython.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Scripting.Hosting;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Infrastructure
{
    public class PythonConnector
    {
        private readonly Helpers _h = new Helpers();


        public PythonConnector(){}


        // See:
        //  * python_paths.md in Configuration folder
        //  * BRefLeagueBattingController.cs
        //  * BRefLeagueBattingController.py

        // Status [July 3, 2019]:
        //  * If all you want are simple variables, then this work
        //  * It breaks if you need to bring things in like numpy or pandas
        //  * Check all links / notes listed at top of this file
        //  * IronPython doesn't work with Python3 yet; Python 2.7 will be deprecated in 2020
        //  * Probably best to wait until IronPython is upgraded to support Python3 before doing any more on this


        #region CONNECT TO PYTHON ------------------------------------------------------------


            // STATUS: this works
            /// <summary>
            ///     Create a connection between .NET and a Python file; This must be run before any of the other methods will work
            /// </summary>
            /// <remarks>
            ///     This must be run before any of the other below methods
            /// </remarks>
            /// <returns>
            ///     A connection between .NET and Python
            /// </returns>
            public object ConnectToPythonFile(string fileName)
            {
                // Extract Python language engine from their grasp
                // Microsoft.Scripting.Hosting.ScriptEngine
                var engine = Python.CreateEngine();

                // Introduce Python namespace (scope)
                // Microsoft.Scripting.Hosting.ScriptScope
                var scope = engine.CreateScope();

                // Microsoft.Scripting.Hosting.ScriptSource
                ScriptSource source = engine.CreateScriptSourceFromFile(fileName);

                // any other functions must be called after this
                object connectionToPython = source.Execute(scope);

                return scope;
            }



            public object CreatePythonConnectionObject(string filePath)
            {
                ScriptEngine engine = CreatePythonScriptEngine();
                ICollection<string> paths  = GetListOfPythonFilePaths(engine);
                // PrintPaths(paths);

                SetPythonFilePaths(engine, paths);

                ScriptScope scope = CreatePythonScriptScope(engine);

                ScriptSource pathOfPythonFileToConnectTo = CreatePythonScriptSource(engine, filePath);

                // object connectionToPython = source.Execute(scope);
                object connectionToPython = CreatePythonConnectionObject(pathOfPythonFileToConnectTo, scope);

                return connectionToPython;
            }



            // Represents a language in Hosting API
            // Hosting API counterpart for Microsoft.Scripting.Hosting.ScriptEngine.LanguageContext
            public ScriptEngine CreatePythonScriptEngine()
            {
                // // Microsoft.Scripting.Hosting.ScriptEngine
                ScriptEngine engine = Python.CreateEngine();
                return engine;
            }


            public void SetPythonFilePaths(ScriptEngine engine, [FromQuery] ICollection<string> paths)
            {
                engine.SetSearchPaths(paths);
            }


            // A ScriptScope is a unit of execution for code
            // It consists of a global Scope which all code executes in
            // A ScriptScope can have an arbitrary initializer and arbitrary reloader
            // ScriptScope is not thread safe
            // Host should either lock when multiple threads could access the same module or should make a copy for each thread.
            // Hosting API counterpart for Microsoft.Scripting.Hosting.ScriptScope.Scope .
            public ScriptScope CreatePythonScriptScope(ScriptEngine engine)
            {
                // Microsoft.Scripting.Hosting.ScriptScope
                ScriptScope scope = engine.CreateScope();
                return scope;
            }



            // Hosting counterpart for Microsoft.Scripting.Hosting.ScriptSource.SourceUnit
            public ScriptSource CreatePythonScriptSource(ScriptEngine engine, string filePath)
            {
                // Microsoft.Scripting.Hosting.ScriptSource
                ScriptSource pathOfPythonFileToConnectTo = engine.CreateScriptSourceFromFile(filePath);
                return pathOfPythonFileToConnectTo;
            }



            public object CreatePythonConnectionObject(ScriptSource source, [FromQuery] ScriptScope scope)
            {
                dynamic connectionToPython = source.Execute(scope);
                // Console.WriteLine($"connectionToPython.GetType_2: {connectionToPython.GetType()}");
                return connectionToPython;
            }


            //.../Coding/Projects/BaseballScraper/bin/Debug/netcoreapp2.1/Lib
            // .../Coding/Projects/BaseballScraper/bin/Debug/netcoreapp2.1/DLLs
            public ICollection<string> GetListOfPythonFilePaths(ScriptEngine engine)
            {
                ICollection<string> paths  = engine.GetSearchPaths();
                return paths;
            }

        #endregion CONNECT TO PYTHON ------------------------------------------------------------





        #region GET DATA FROM PYTHON ------------------------------------------------------------


            // STATUS: this works
            // GET | ONE | VARIABLE VALUE
            /// <summary>
            ///     Retrieve a variable value from a Python scope / file
            /// </summary>
            /// <remarks>
            ///     This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
            /// </remarks>
            /// <param name="scope">
            ///     ScriptScope generated to connect to Python file
            /// </param>
            /// <param name="variableKey">
            ///     The key in the actual Python file that you want to retrieve the value for
            /// </param>
            /// <example>
            ///     var parameter = GetPythonVariableValue(scope, "parameter"); Where 'parameter' is the variable name in the Python file
            /// </example>
            /// <returns>
            ///     A value of a variable from a Python file
            /// </returns>
            public object GetPythonVariableValue(ScriptScope scope, string variableKey)
            {
                dynamic variableValue = scope.GetVariable<dynamic>(variableKey);
                Console.WriteLine($"KEY: {variableKey}  --> VALUE: {variableValue}");
                return variableValue;
            }


            // STATUS: this works
            // GET | ALL | VARIABLE NAMES
            /// <summary>
            ///     Retrieve all variable names from a Python scope / file
            /// </summary>
            /// <remarks>
            ///     This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
            /// </remarks>
            /// <param name="scope">
            ///     ScriptScope generated to connect to Python file
            /// </param>
            /// <returns>
            ///     A list of the names of each variable within a Python scope / file
            /// </returns>
            public IEnumerable<string> GetPythonVariableNames(ScriptScope scope)
            {
                IEnumerable<string> pythonVariableNamesList = scope.GetVariableNames();
                PrintPythonVariableNames(pythonVariableNamesList);
                return pythonVariableNamesList;
            }


            // STATUS: this works
            // GET | ALL | VARIABLE KEYS AND VALUES
            /// <summary>
            ///     Retrieve variable keys(i.e., names) and values from a Python scope / file
            /// </summary>
            /// <remarks>
            ///     This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
            /// </remarks>
            /// <param name="scope">
            ///     ScriptScope generated to connect to Python file
            /// </param>
            /// <example>
            ///     GetPythonKeyValuePairs(scope);
            /// </example>
            /// <returns>
            ///     An IEnumerable of keys and values from a Python scope / file
            /// </returns>
            public IEnumerable<KeyValuePair<string, dynamic>> GetPythonKeyValuePairs(ScriptScope scope)
            {
                IEnumerable<KeyValuePair<string, dynamic>> pythonKeyValuePairs = scope.GetItems();
                PrintPythonKeyValuePairs(pythonKeyValuePairs);
                return pythonKeyValuePairs;
            }


        #endregion GET DATA FROM PYTHON ------------------------------------------------------------





        #region SET DATA IN PYTHON ------------------------------------------------------------


            // STATUS: this works
            // SET | ONE | VARIABLE KEY AND VALUE
            /// <summary>
            ///     Set variable key (i.e. name) and value in corresponding Python scope / file
            /// </summary>
            /// <remarks>
            ///     This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
            /// </remarks>
            /// <param name="scope">
            ///     ScriptScope generated to connect to Python file
            /// </param>
            /// <param name="variableKey">
            ///     The key in the actual Python file that you want to retrieve the value for
            /// </param>
            /// <param name="variableValue">
            ///     The value in the actual Python file that you want to retrieve
            /// </param>
            /// <example>
            ///     SetPythonVariableKeyAndValue(scope, "myObjectKey", "my object value");
            /// </example>
            public void SetPythonVariableKeyAndValue(ScriptScope scope, string variableKey, string variableValue)
            {
                // This will be the name of the variable in Python script, initialized with previously created .NET variable
                scope.SetVariable(variableKey, variableValue);
            }


            // STATUS: this works
            // SET | MANY | VARIABLE KEYS AND VALUES
            /// <summary>
            ///     Given a dictionary of keys and values, set K&Vs as Python variable K&Vs
            /// </summary>
            /// <remarks>
            ///     This must be run AFTER establishing connection to a Python file (i.e, run 'ConnectToPythonFile' method before running this method)
            ///     To print the dictionary in Python --> 'print('dictionaryName')
            /// </remarks>
            /// <param name="scope">
            ///     ScriptScope generated to connect to Python file
            /// </param>
            /// <param name="keysAndValuesDictionary">
            ///     A dictionary of keys and values where keys are strings and values are objects </param>
            /// <param name="dictionaryName">
            /// </param>
            /// <example>
            ///     var newDictionary = new Dictionary<string, object>
            ///     newDictionary.Add{ "itemId", 37}
            /// </example>
            public void SetPythonKeyValueDictionary(ScriptScope scope, Dictionary<string, object> keysAndValuesDictionary, string dictionaryName)
            {
                // set dictionary as Python variable
                scope.SetVariable(dictionaryName, keysAndValuesDictionary);

                // Set each individual key value pair as a Python variable
                foreach(var dictionaryEntry in keysAndValuesDictionary)
                {
                    scope.SetVariable(dictionaryEntry.Key, dictionaryEntry.Value);
                    PrintKeyAndValueForPair(dictionaryEntry);
                }
            }



            // STATUS: this works but is it needed?
            /// <summary> Create a dictionary of keys and values that will be set as Python keys and values </summary>
            /// <remarks> Note that keys and values are defined within this function; a dictionary is not passed in as a parameter / argument </remarks>
            /// <example> var dictionaryForPython = CreateDictionaryOfPythonKeysAndValuesToSet(); </example>
            /// <returns> A dictionary of keys and values where keys are strings and values are objects </returns>
            // public Dictionary<string, object> CreateDictionaryOfPythonKeysAndValuesToSet ()
            // {
            //     // Add some sample parameters. Notice that there is no need in specifically setting the object type, interpreter will do that part for us in the script properly with high probability
            //     var keysAndValuesDictionary = new Dictionary<string, object>
            //     {
            //         { "personId", 1},
            //         { "person", "Mookie Betts"}
            //     };
            //     return keysAndValuesDictionary;
            // }


        #endregion SET DATA IN PYTHON ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------


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


            public void PrintKeyAndValueForPair(KeyValuePair<string, object> dictionaryEntry)
            {
                Console.WriteLine($"KEY CHECK: {dictionaryEntry.Key}  VALUE CHECK: {dictionaryEntry.Value}");
            }



            public void PrintPythonVariableNames(IEnumerable<string> pythonVariableNamesList)
            {
                foreach(var varName in pythonVariableNamesList)
                {
                    Console.WriteLine(varName);
                }
            }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
