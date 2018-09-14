using System;
using System.Collections.Generic;
using BaseballScraper.Infrastructure;
using IronPython.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Scripting.Hosting;

namespace BaseballScraper.Controllers.BaseballReference
{
    // installing Pandas from the command line: python3 -m pip install --upgrade pandas
    // python -m pip install -U black


    // Find python paths:
        // python -m site --user-base
        // python -m site --user-base/bin


    // STATUS: none of this works

    #pragma warning disable CS0414
    [Route("python")]
    public class BaseballReferenceLeagueBatting: Controller
    {
        private readonly Helpers _h           = new Helpers();
        private readonly PythonConnector _pyC = new PythonConnector();
        private readonly string LeagueBatting = "Controllers/BaseballReference/BaseballReferenceLeagueBatting.py";



        [Route("start")]
        public void StartPython()
        {
            _h.StartMethod();

            var engine = _pyC.CreatePythonScriptEngine();
            var paths  = engine.GetSearchPaths();

            Console.WriteLine("PRE -----------");
            foreach(var x in paths) { Console.WriteLine(x); }

            List<string> pathList = GetListOfPythonFilePaths();

            foreach(var path in pathList)
            {
                paths.Add(path);
            }

            Console.WriteLine("POST -----------");
            foreach(var x in paths) { Console.WriteLine(x); }

            engine.SetSearchPaths(paths);

            var scope = engine.CreateScope();

            ScriptSource source = engine.CreateScriptSourceFromFile(LeagueBatting);

            object connectionToPython = source.Execute(scope);
        }

        [Route("modules")]
        public List<string> GetListOfPythonModules()
        {
            List<string> moduleList = new List<string>();
            moduleList.Add("os");
            moduleList.Add("requests");
            moduleList.Add("pandas");
            moduleList.Add("datetime");
            moduleList.Add("io");
            moduleList.Add("BeautifulSoup");

            return moduleList;
        }

        [Route("filepaths")]
        public List<string> GetListOfPythonFilePaths()
        {
            List<string> pathList = new List<string>();

            return pathList;
        }
    }

}