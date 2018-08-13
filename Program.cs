using System;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Controllers;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace BaseballScraper
{
    #pragma warning disable CS0414
    public class Program
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";


        public static void Main (string[] args)
        {
            Start.ThisMethod();
            Console.WriteLine ($"Version: {Environment.Version}");

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => 
            WebHost.CreateDefaultBuilder(args)
                .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True")
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                    logging.SetMinimumLevel(LogLevel.Warning)
                );
    }
}




// public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
//     WebHost.CreateDefaultBuilder(args)
//         .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True")
//         .UseStartup<Startup>();