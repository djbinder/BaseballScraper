#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006

using System;
using System.IO;
using System.Net;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.BaseballProspectusControllers
{
    [Route("api/baseballprospectus/[controller]")]
    [ApiController]
    public class BpSpController : ControllerBase
    {
        private static readonly Helpers _h = new Helpers();

        private readonly CsvHandler _csvH = new CsvHandler();

        private readonly string _bpSpDraLeaderboardLink = "https://legacy.baseballprospectus.com/sortable/extras/dra_runs.php";



        [Route("test")]
        public void BpSpTesting()
        {
            _h.StartMethod();
            GetBpSpDraLeaderboard();
        }






        public void GetBpSpDraLeaderboard()
        {
            // WebRequest request = WebRequest.Create(_bpSpDraLeaderboardLink);
            // request.Credentials = CredentialCache.DefaultCredentials;

            // HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // // Display the status.
            // Console.WriteLine (response.StatusDescription);
            // // Get the stream containing content returned by the server.
            // Stream dataStream = response.GetResponseStream ();
            // // Open the stream using a StreamReader for easy access.
            // StreamReader reader = new StreamReader (dataStream);

            // // Read the content.
            // string responseFromServer = reader.ReadToEnd ();

            // string[] files = System.IO.Directory.GetFiles(_bpSpDraLeaderboardLink,"*.csv");
            // Console.WriteLine(files.Length);

            // // Display the content.
            // // Console.WriteLine (responseFromServer);
            // // Cleanup the streams and the response.
            // reader.Close ();
            // dataStream.Close ();
            // response.Close ();



            // string csvLocation = "Baseball Prospectus DRA Run Values by Year.csv";
            // string csvLocationWithPercent = "Baseball%Prospectus%DRA%Run%Values%by%Year.csv";

            // string fullPath = $"{_bpSpDraLeaderboardLink}/{csvLocation}";
            // string fullPathWithPercent = $"{_bpSpDraLeaderboardLink}/{csvLocationWithPercent}";

            // Console.WriteLine(fullPath);
            // Console.WriteLine(fullPathWithPercent);

            // _csvH.DownloadCsvFromLink(fullPath, "BaseballData/02_Target_Write/BaseballProspectus_Target_Write/test_write1.csv");
        }



    }
}
