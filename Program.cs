using System;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Controllers;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BaseballScraper
{
    public class Program
    {
        private static String Start               = "STARTED";
        private static String Complete            = "COMPLETED";
        private static string _twitterConsumerKey = null;
        public static IConfiguration _configuration;
        public static IOptions<TwitterConfiguration> _twConfig;

        public String ConsumerKey;

        private static readonly TwitterConfiguration _twitterConfiguration;


        public static void Main (string[] args)
        {
            Start.ThisMethod();
            Console.WriteLine ($"Version: {Environment.Version}");


            // var linqToTwitter = new LinqToTwitterController(_twitterConfiguration);

            // try {
            //     linqToTwitter.TwitterStringSearch("Anthony Rizzo").Wait();
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"Exception is {ex}");
            // }

            CreateWebHostBuilder(args).Build().Run();
            // BuildWebHost(args).Run();
        }

        // public static IWebHost BuildWebHost (string[] args) =>
        //     WebHost.CreateDefaultBuilder (args)
        //     .UseStartup<Startup> ()
        //     .Build();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => 
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        public static void StartStop()
        {
            Start.ThisMethod();
            Complete.ThisMethod();
        }
    }
}