using System;
using System.Collections.Generic;
using System.IO;
using BaseballScraper.Infrastructure;
using IronPython.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Scripting.Hosting;


#pragma warning disable CC0091, CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.BaseballReference
{
    [Route("api/baseballreference/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BRefLeagueBattingController: ControllerBase
    {

        private readonly Helpers         _helpers;
        private readonly PythonConnector _pythonConnector;

        // may need to use full path and not relative path for this
        private readonly string _leagueBatting = "Controllers/BaseballReferenceControllers/BRefLeagueBattingController.py";


        public BRefLeagueBattingController (Helpers helpers, PythonConnector pythonConnector)
        {
            _helpers         = helpers;
            _pythonConnector = pythonConnector;
        }



        // See:
        // * python_paths.md in Configuration folder
        // * PythonConnector.cs
        // * BRefLeagueBattingController.py

        // Status [July 3, 2019]:
        // * If all you want are simple variables, then this works
        // * It breaks if you need to bring things in like numpy or pandas
        // * Check all links / notes listed at top of PythonConnector.cs file
        // * IronPython doesn't work with Python3 yet; Python 2.7 will be deprecated in 2020
        // * Probably best to wait until IronPython is upgraded to support Python3 before doing any more on this
        // * https://github.com/IronLanguages/ironpython3


        /*
            https://127.0.0.1:5001/api/baseballreference/BRefLeagueBatting/testA
        */
        [Route("testA")]
        public void BRefTestingA()
        {
            _helpers.StartMethod();
            ScriptScope scope = CreateScopeForBrefPythonFile(_leagueBatting);
        }



        // STATUS [ July 1, 2019 ]: this works
        // ScriptScope scope = CreateScopeForBrefPythonFile(_leagueBatting);
        public ScriptScope CreateScopeForBrefPythonFile(string fileName)
        {
            ScriptEngine engine = _pythonConnector.CreatePythonScriptEngine();

            ScriptScope scope = _pythonConnector.CreatePythonScriptScope(engine);

            ICollection<string> paths = _pythonConnector.GetListOfPythonFilePaths(engine);
            AddPythonPaths(paths);

            engine.SetSearchPaths(paths);

            ScriptSource source = _pythonConnector.CreatePythonScriptSource(engine, fileName);

            CompiledCode compiled = source.Compile();

            dynamic connectionToPython = source.Execute(scope);

            return scope;
        }


        // STATUS [ July 1, 2019 ]: this works
        // var variable = GetOneVariableFromBrefPythonFile(_leagueBatting, "newObject");
        public object GetOneVariableFromBrefPythonFile(string fileName, string variableKey)
        {
            ScriptScope scope = CreateScopeForBrefPythonFile(fileName);
            string variableValue = scope.GetVariable<string>(variableKey);
            Console.WriteLine($"variableValue: {variableValue} of type: {variableValue.GetType()}");
            return variableValue;
        }


        // STATUS [ July 1, 2019 ]: this works
        // var variableNames = GetAllVariableNamesFromBrefPythonFile(_leagueBatting);
        public IEnumerable<string> GetAllVariableNamesFromBrefPythonFile(string fileName)
        {
            ScriptScope scope = CreateScopeForBrefPythonFile(fileName);
            IEnumerable<string> variableNames = scope.GetVariableNames();
            foreach(string name in variableNames)
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
                foreach(string path in paths)
                {
                    Console.WriteLine($"path: {path}");
                }
            }


        #endregion PRINTING PRESS ------------------------------------------------------------
    }
}
