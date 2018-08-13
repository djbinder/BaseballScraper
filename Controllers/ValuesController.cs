using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BaseballScraper.Controllers
{
    #pragma warning disable CS0414
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController: ControllerBase
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";


        // THIS WORKS
        // go to this link:
            // https://127.0.0.1:5001/api/Values
        // [HttpGet]
        // public string[] Get ()
        // {

        // }


        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
