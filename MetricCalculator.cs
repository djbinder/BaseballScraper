using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006
namespace BaseballScraper
{
    public class MetricCalculator
    {

        private readonly Helpers _helpers;
        private readonly CsvHandler _csvHandler;
        // private readonly


        public MetricCalculator(Helpers helpers, CsvHandler csvHandler)
        {
            _helpers = helpers;
            _csvHandler = csvHandler;
        }

        public MetricCalculator() {}


        /*
            https://127.0.0.1:5001/metric_calculator
        */
        // [HttpGet("metric_calculator")]
        // public async Task TestController()
        // {
        //     _helpers.StartMethod();
        //     // await _csvHandler.ClickLinkToDownloadCsvFile();
        // }



        public void CalculateWeightedPlateDisciplineIndexPitchers()
        {

        }





        #region START ------------------------------------------------------------
        #endregion START ------------------------------------------------------------
    }
}
