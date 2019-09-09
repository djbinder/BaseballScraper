using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SharpPad;

#pragma warning disable CS0414, CS0219, IDE0051, IDE0059, CS1591, IDE0044
namespace BaseballScraper
{
    public class Program
    {
        public static void Main (string[] args)
        {
            Console.WriteLine($"\n***** ---> READY TO ROLL <--- *****");
            Console.WriteLine ($"Version: {Environment.Version}");

            if(Environment.Version.ToString() == "Production")
            {
                Console.WriteLine();
                Console.WriteLine("***************************************************");
                Console.WriteLine("SWITCH TO DEV ENVIRONMENT");
                Console.WriteLine("***************************************************");
                Console.WriteLine();
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "False")
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                    logging.SetMinimumLevel(LogLevel.Warning)
                            .AddConsole()
                            .AddDebug()
                );
    }
}
