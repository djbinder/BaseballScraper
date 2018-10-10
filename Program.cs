using System;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Controllers;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using BaseballScraper.Controllers.MlbDataApiControllers;

namespace BaseballScraper
{
    #pragma warning disable CS0414
    public class Program
    {


        public static void Main (string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("***** ---> READY TO ROLL <--- *****");
            Console.WriteLine ($"Version: {Environment.Version}");
            Console.WriteLine();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "False")
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                    logging.SetMinimumLevel(LogLevel.Warning)
                );
    }
}

