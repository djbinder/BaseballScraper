// IMPORTANT# some things needed to get csharp and R to work together
// MacBook - Pro : BaseballScraper DanBinder$ export LD_LIBRARY_PATH = / Library / Frameworks / R.framework / Libraries /: $LD_LIBRARY_PATH
//  MacBook-Pro:BaseballScraper DanBinder$ export PATH=/Library/Frameworks/R.framework/Libraries/:$PATH
//  had to switch R_Home path --> export R_HOME=/Library/Frameworks/R.framework/Resources

using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RDotNet;

namespace BaseballScraper.Controllers.RController
{
    [Route("r")]
    public class RdotNetController: Controller
    {
        private Constants _c    = new Constants();
        public  REngine _engine = REngine.GetInstance();


        [HttpGet]
        [Route("")]
        public IActionResult CreatePitcherWinsVector()
        {
            REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance();
            _c.Start.ThisMethod();
            NumericVector pitcherWins = engine.CreateNumericVector(new double[] { 8 ,21, 15, 21, 21, 22, 14 });
            Console.WriteLine($"PITCHER WINS {pitcherWins}");

            return Content($"PITCHER WINS: {pitcherWins}");
        }



        public void RExample1 ()
        {

            _c.Start.ThisMethod ();


            REngine.SetEnvironmentVariables ();
            // There are several options to initialize the engine, but by default the following suffice:
            REngine engine = REngine.GetInstance ();
            engine.Intro ("engine");

            // // .NET Framework array to R vector.
            NumericVector group1 = engine.CreateNumericVector (new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
            engine.SetSymbol ("group1", group1);
            // Direct parsing from R script.
            NumericVector group2 = engine.Evaluate ("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric ();

            // Test difference of mean and get the P-value.
            GenericVector testResult = engine.Evaluate ("t.test(group1, group2)").AsList ();
            double        p          = testResult["p.value"].AsNumeric ().First ();

            Console.WriteLine ("Group1: [{0}]", string.Join (", ", group1));
            Console.WriteLine ("Group2: [{0}]", string.Join (", ", group2));
            Console.WriteLine ("P-value = {0:0.000}", p);

            // you should always dispose of the REngine properly.
            // After disposing of the engine, you cannot reinitialize nor reuse it
            engine.Dispose ();
        }

        // this is interesting
        public static void CalculateOnCommandLine ()
        {
            string result;
            string input;
            REngine engine;

            //init the R engine
            REngine.SetEnvironmentVariables ();
            engine = REngine.GetInstance ();
            engine.Initialize ();

            //input
            Console.WriteLine ("Please enter the calculation");
            input = Console.ReadLine ();

            //calculate
            CharacterVector vector = engine.Evaluate (input).AsCharacter ();
                            result = vector[0];

            //clean up
            engine.Dispose ();

            //output
            Console.WriteLine ("");
            Console.WriteLine ("Result: '{0}'", result);
            Console.WriteLine ("Press any key to exit");
            Console.ReadKey ();
        }
    }
}
