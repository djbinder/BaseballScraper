using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace BaseballScraper
{
#pragma warning disable CS0414, CS0219, IDE0051, IDE0059, CS1591, IDE0044
    public class Program
    {
        public static void Main (string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("***** ---> READY TO ROLL <--- *****");
            Console.WriteLine ($"Version: {Environment.Version}");
            // Console.WriteLine();
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

