// Resources: https://rawgit.com/evelinag/Projects/master/RDotNetOnMac/output/RDotNetOnMac.html

using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RDotNet;

namespace BaseballScraper.Infrastructure
{
    public class RdotNetConnector
    {
        private Constants _c = new Constants();

        // NOTE: three steps needed to get csharp and R to work together; enter these in terminal in succession
        // (1)      export LD_LIBRARY_PATH=/Library/Frameworks/R.framework/Libraries/:$LD_LIBRARY_PATH
        // (2)      export PATH=/Library/Frameworks/R.framework/Libraries/:$PATH
        // (3)      export R_HOME=/Library/Frameworks/R.framework/Resources


        // /Library/Frameworks/R.framework/Versions/3.4/Resources/library

        // STATUS: this works
        /// <summary> Creates a new engine that drives other R functions </summary>
        /// <returns> A new R Engine </returns>
        public REngine CreateNewREngine()
        {
            _c.Start.ThisMethod();

            REngine.SetEnvironmentVariables ();

            REngine engine = REngine.GetInstance ();

            return engine;
        }



        public void RPractice()
        {
            REngine engine = CreateNewREngine();

            NumericVector preProcessedValue = null;
            DataFrame     predictedData     = null;

            string startDate = "2001-01-01";
            string days      = "2";
            string interval  = "3";

            double[] preProcessedList = new[] { 1.1, 7.3, 4.5, 7.4, 11.23, 985.44 };
                   preProcessedValue  = engine.CreateNumericVector(preProcessedList);

            int evalNumber = 1;

            engine.SetSymbol("preProcessedValue", preProcessedValue);
            engine.Evaluate("library(baseballr)");
            engine.Evaluate("library(forecast)");

            // returns --> "2001-01-03"
            // type --> RDotNet.SymbolicExpression
            Console.WriteLine(evalNumber);
                engine.Evaluate("startDate <- as.Date('" + startDate + "') + " + days);
                var eval = engine.Evaluate("startDate <- as.Date('" + startDate + "') + " + days);
                    Console.WriteLine(eval);
                    Console.WriteLine();
                    evalNumber++;

            Console.WriteLine(evalNumber);
                engine.Evaluate("size = length(seq(from=as.Date('" + startDate + "'), by='" + "day" + "', to=as.Date(startDate)))");
                eval = engine.Evaluate("size = length(seq(from=as.Date('" + startDate + "'), by='" + "day" + "', to=as.Date(startDate)))");
                    Console.WriteLine(eval);
                    Console.WriteLine();
                    evalNumber++;

            Console.WriteLine(evalNumber);
                engine.Evaluate("startDate <- as.POSIXct('" + startDate + "')");
                eval = engine.Evaluate("startDate <- as.POSIXct('" + startDate + "')");
                    Console.WriteLine(eval);
                    Console.WriteLine();
                    evalNumber++;

            Console.WriteLine(evalNumber);
                engine.Evaluate("endDate <- startDate + as.difftime(size, units='" + "days" + "')");
                eval = engine.Evaluate("endDate <- startDate + as.difftime(size, units='" + "days" + "')");
                    Console.WriteLine(eval);
                    Console.WriteLine();
                    evalNumber++;

            Console.WriteLine(evalNumber);
                engine.Evaluate("predictDate = seq(from=startDate, by=" + interval + "*60, to=endDate)");
                eval = engine.Evaluate("predictDate = seq(from=startDate, by=" + interval + "*60, to=endDate)");
                    Console.WriteLine(eval);
                    Console.WriteLine();
                    evalNumber++;

            Console.WriteLine(evalNumber);
                engine.Evaluate("freq <- ts(preProcessedValue, frequency = 20)");
                    evalNumber++;


            engine.Evaluate("forecastnavie <-snaive(freq, Datapoints)");
            // engine.Evaluate("predictValue = (forecastnavie$mean)");
            // engine.Evaluate("predictedData = cbind(predictValue, data.frame(predictDate))");
            // predictedData = engine.Evaluate("predictedData").AsDataFrame();

        }






        // STATUS: this works
        public void CalculatePitcherWinningPercentage()
        {
            REngine engine = CreateNewREngine();

            // PITCHER WINS --> each number represents number of wins for a single season
            // PITCHER WINS type --> RDotNet.NumericVector
            NumericVector pitcherWins = engine.CreateNumericVector(new double[] {8, 21, 15, 21, 21, 22, 14});
            engine.SetSymbol("pitcherWins", pitcherWins);

            // PITCHER LOSSES --> each number represents number of losses for a single season
            // PITCHER LOSSES type --> RDotNet.NumericVector
            NumericVector pitcherLosses = engine.CreateNumericVector(new double[] {5, 10, 12, 14, 17, 14, 19});
            engine.SetSymbol("pitcherLosses", pitcherLosses);

            // WINNING PERCENTAGE --> calculate the winning percentage for each single season
            NumericVector winningPercentage = engine.Evaluate("winningPercentage <- 100 * pitcherWins / (pitcherWins + pitcherLosses)").AsNumeric();
        }
    }
}
