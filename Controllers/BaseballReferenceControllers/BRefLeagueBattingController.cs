using System;
using System.Collections.Generic;
using System.IO;
using BaseballScraper.Infrastructure;
using IronPython.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Scripting.Hosting;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.BaseballReference
{
    // installing Pandas from the command line: python3 -m pip install --upgrade pandas
    // python -m pip install -U black


    // Find python paths:
        // python -m site --user-base
        // python -m site --user-base/bin


    // STATUS: none of this works


    [Route("api/baseballreference/[controller]")]
    [ApiController]
    public class BRefLeagueBattingController: ControllerBase
    {

        private readonly Helpers _h           = new Helpers();

        private readonly PythonConnector _pythonConnector;

        // may need to use full path and not relative path for this
        private readonly string _leagueBatting = "Controllers/BaseballReferenceControllers/BRefLeagueBattingController.py";


        public BRefLeagueBattingController (PythonConnector pythonConnector)
        {
            _pythonConnector = pythonConnector;
        }



        /*
            https://127.0.0.1:5001/api/baseballreference/BRefLeagueBatting/testA
        */
        [Route("testA")]
        public void BRefTestingA()
        {
            _h.StartMethod();
            // var variableNames = GetAllVariableNamesFromBrefPythonFile(_leagueBatting);

            // var variableNames = GetAllVariableNamesFromBrefPythonFile(_leagueBatting);

            // var host = new DlrHost<IronPythonLanguage>(new IronPythonLanguage());


            // host.ScriptEngine.SetSearchPaths(paths);
            // host.DefaultScope.Execute("print 'Hello World'");


            // var variable = GetOneVariableFromBrefPythonFile(_leagueBatting, "place_holder");

            // var sScope = CreateScopeForBrefPythonFile(_leagueBatting);

            var standings_current = new List<object>();

            var stringer = PatchParameter();



        }

        public string PatchParameter()
        {
            var engine = Python.CreateEngine(); // Extract Python language engine from their grasp
            var scope = engine.CreateScope(); // Introduce Python namespace (scope)

            var paths = _pythonConnector.GetListOfPythonFilePaths(engine);
            AddPythonPaths(paths);

            engine.SetSearchPaths(paths);

            // var pandas = engine.ImportModule("pandas");

            var response = "RESPONSE!";
            var place_holder = "place_holder";
            var standings_current = "standings_current";
            var variableA = "";

            var d = new Dictionary<string, object>
            {
                // { "response", response},
                { "place_holder", place_holder},
                { "variableA", variableA},
                { "standings_current", standings_current }
            }; // Add some sample parameters. Notice that there is no need in specifically setting the object type, interpreter will do that part for us in the script properly with high probability

            scope.SetVariable("params", d); // This will be the name of the dictionary in python script, initialized with previously created .NET Dictionary
            ScriptSource source = engine.CreateScriptSourceFromFile(_leagueBatting); // Load the script
            // dynamic result = source.Execute(scope);
            source.Execute(scope);
            place_holder = scope.GetVariable<string>("place_holder"); // To get the finally set variable 'parameter' from the python script
            return place_holder;
        }




        // STATUS [ July 1, 2019 ]: this works
        // ScriptScope scope = CreateScopeForBrefPythonFile(_leagueBatting);
        public ScriptScope CreateScopeForBrefPythonFile(string fileName)
        {


            ScriptEngine engine = _pythonConnector.CreatePythonScriptEngine();

            ScriptScope scope = _pythonConnector.CreatePythonScriptScope(engine);

            var paths = _pythonConnector.GetListOfPythonFilePaths(engine);
            AddPythonPaths(paths);

            engine.SetSearchPaths(paths);

            ScriptScope requests = engine.ImportModule("requests");

            // ScriptScope pandas = engine.ImportModule("pandas");

            // var moduleFileNames = engine.GetModuleFilenames();
            // foreach(var name in moduleFileNames)
            // {
            //     Console.WriteLine($"name: {name}");
            // }

            ScriptSource source = _pythonConnector.CreatePythonScriptSource(engine, fileName);
            Console.WriteLine("I");



            var compiled = source.Compile();
            Console.WriteLine("J");
            Console.WriteLine($"compiled: {compiled}");
            // _h.Dig(compiled);
            // var result = compiled.Execute(scope);

            // Console.WriteLine($"result: {result}");

            Console.WriteLine($"source.GetCode(): {source.GetCode()}\n");
            // Console.WriteLine($"source.GetCodeLine: {source.GetCodeLine(125)}");

            var standings_current = source.GetCodeLine(131);
            Console.WriteLine($"standings_current: \n {standings_current}\n");

            var connectionToPython = source.Execute(scope);
            // source.Execute(scope);
            // Console.WriteLine($"connectionToPython: {connectionToPython.GetType()}");

            var outputStream = new MemoryStream();
            var outputStreamWriter = new StreamWriter(outputStream);
            engine.Runtime.IO.SetOutput(outputStream, outputStreamWriter);
            // _h.Dig(outputStreamWriter);

            var outStream = engine.Runtime.IO.OutputStream;
            Console.WriteLine($"outStream: {outStream}");

            var streamReader = new StreamReader(outStream);
            string line = String.Empty;
            while((line = streamReader.ReadLine()) != null)
            {
                Console.WriteLine($"line: {line}");
            }

            Console.WriteLine("K");




            return scope;
        }


        // STATUS [ July 1, 2019 ]: this works
        // var variable = GetOneVariableFromBrefPythonFile(_leagueBatting, "newObject");
        public object GetOneVariableFromBrefPythonFile(string fileName, string variableKey)
        {
            ScriptScope scope = CreateScopeForBrefPythonFile(fileName);
            var variableValue = scope.GetVariable<string>(variableKey);
            Console.WriteLine($"variableValue: {variableValue} of type: {variableValue.GetType()}");
            return variableValue;
        }


        // STATUS [ July 1, 2019 ]: this works
        // var variableNames = GetAllVariableNamesFromBrefPythonFile(_leagueBatting);
        public IEnumerable<string> GetAllVariableNamesFromBrefPythonFile(string fileName)
        {
            ScriptScope scope = CreateScopeForBrefPythonFile(fileName);
            var variableNames = scope.GetVariableNames();
            foreach(var name in variableNames)
            {
                Console.WriteLine($"vNAME: {name}");
            }
            _pythonConnector.PrintPythonVariableNames(variableNames);
            return variableNames;
        }


        // note: see 'python_paths.md' for other paths that should be added here
        // not all paths should be pushed to github; 'python_paths.md' is in Configuration folder
        public void AddPythonPaths(ICollection<string> paths)
        {
            paths.Add("./miniconda2/lib/python2.7/site-packages");

        }



        #region PRINTING PRESS ------------------------------------------------------------


            public void PrintPaths(ICollection<string> paths)
            {
                foreach(var path in paths)
                {
                    Console.WriteLine($"path: {path}");
                }
            }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
