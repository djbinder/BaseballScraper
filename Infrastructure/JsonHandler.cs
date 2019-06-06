using System.IO;

namespace BaseballScraper.Infrastructure
{
    public class JsonHandler
    {
        private readonly Helpers _h = new Helpers();


        public void JsonReaderTester()
        {
            ReadJsonFile();
        }


        public void ReadJsonFile()
        {
            _h.StartMethod();

            using (FileStream stream = new FileStream("Configuration/googleCredentials.json",FileMode.Open, FileAccess.Read))
            {
                // _h.stream.Dig();
            }





        }

    }
}
