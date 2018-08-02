// https://gist.githubusercontent.com/deanebarker/2b4520f290ece96be40436bc5c8915c5/raw/0cf6005f41ac27c46c9ce1f9bdbf8b5faeb62f8d/AirtableGetObjects.cs

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BaseballScraper.Controllers
{

    public class AirtableController: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";


        [Authorize]
        [HttpGet]
        [Route("DoTheQuery")]
        public IActionResult DoTheQuery()
        {
            Start.ThisMethod();
            QueryManagers();
            Complete.ThisMethod();

            return Content($"this is a test");
        }

        public static void QueryManagers ()
        {
            Start.ThisMethod();



            Complete.ThisMethod();

        }
    }
}



//     public static void CreateAirtable()
//     {
//         var airtable = new Airtable("myAccountId", "myApiKey");

//         airtable.AddConvertor("Last Name", "LastName", (value) => {
//             return $"{value} the Great!";
//         });

//         var people = airtable.GetObjects<Person>("myTableName");

//         airtable.OnHttpResponse += (sender, e) => { Console.WriteLine(e.HttpResponse); };

//         airtable.OnTypeConversionFailure += (sender, e) => {
//             if(e.FieldName == "Date") { e.FieldValue = DateTime.MinValue;}
//         };


//         airtable.OnBeforeAssignValues += (sender, e) => {
//             if(e.Values["FirstName"].ToString() == "Deane") { e.Cancel = true; }
//         };
//     }