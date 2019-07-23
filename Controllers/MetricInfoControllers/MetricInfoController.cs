using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using C = System.Console;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.MetricInfoControllers
{
    [Route("api/metrics/[controller]")]
    [ApiController]
    public class MetricInfoController : ControllerBase
    {

        private readonly Helpers _h;
        private readonly AirtableManager _atM;


        public MetricInfoController(Helpers h, AirtableManager atM)
        {
            _h = h;
            _atM = atM;
        }

        public MetricInfoController() {}


        /*
            https://127.0.0.1:5001/api/metrics/metricinfo/test
        */
        [HttpGet("test")]
        public void TestMetricInfoController()
        {
            _h.StartMethod();
        }

    }
}
