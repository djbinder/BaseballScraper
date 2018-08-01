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
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController: ControllerBase
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        public interface IValues { void Method(); }
        private readonly TwitterConfiguration _twitterConfiguration;
        public ValuesController(TwitterConfiguration optionsValue)
        {
            _twitterConfiguration = optionsValue;
        }

        // THIS WORKS
        // go to this link:
            // https://127.0.0.1:5001/api/Values
        [HttpGet]
        public string[] Get ()
        {
            _twitterConfiguration.Dig();
            string _twitterConsumerKey       = _twitterConfiguration.ConsumerKey;
            string _twitterConsumerSecret    = _twitterConfiguration.ConsumerSecret;
            string _twitterAccessToken       = _twitterConfiguration.AccessToken;
            string _twitterAccessTokenSecret = _twitterConfiguration.AccessTokenSecret;

            Console.WriteLine(".....CONSUMER KEY.....");
            Console.WriteLine(_twitterConfiguration.ConsumerKey);
            Console.WriteLine();
            Console.WriteLine(".....CONSUMER SECRET.....");
            Console.WriteLine(_twitterConfiguration.ConsumerSecret);
            Console.WriteLine();
            Console.WriteLine(".....ACCESS LEVEL.....");
            Console.WriteLine(_twitterConfiguration.AccessLevel);
            Console.WriteLine();
            Console.WriteLine(".....ACCESS TOKEN.....");
            Console.WriteLine(_twitterConfiguration.AccessToken);
            Console.WriteLine();
            Console.WriteLine(".....ACCESS TOKEN SECRET.....");
            Console.WriteLine(_twitterConfiguration.AccessTokenSecret);
            Console.WriteLine();

            return new string [] { _twitterConsumerKey, _twitterConsumerSecret, _twitterAccessToken, _twitterAccessTokenSecret};

            // return $@"
            //     IOptions         <>  : {_twitterConfiguration.ConsumerKey}
            //     IOptionsSnapshot<>   : {_snapshot.ConsumerKey}
            //     Are              same: {_twitterConfiguration == _snapshot}";
        }


        public async Task GetTwitterInfo(string[] args, CancellationToken token)
        // public ActionResult<IEnumerable<string>> Get()
        {
            string baseAddress = "https://127.0.0.1:5001/api/Values";
            using(var xhttpClient = new HttpClient())
            {
                string               json     = await xhttpClient.GetStringAsync(baseAddress);
                TwitterConfiguration instance = JsonConvert.DeserializeObject<TwitterConfiguration>(json);
            }
            return;
        }

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
