using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

using BaseballScraper.Controllers;
using BaseballScraper.Mappers;
using BaseballScraper.Models;
using BaseballScraper.Scrapers;
using BaseballScraper.Services;
using BaseballScraper.Services.Security;
using HtmlAgilityPack;
using LinqToTwitter;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using Npoi.Mapper;

using NScrape;

using RDotNet;
using YahooFantasyWrapper.Client;

namespace BaseballScraper
{
    public class Program
    {
        private static String _start    = "STARTED";
        private static String _complete = "COMPLETED";
        public static string Start { get => _start; set => _start = value; }
        public static string Complete { get => _complete; set => _complete = value; }

        private static readonly AppIdentitySettings _identity;


        public static void Main (string[] args)
        {
            Start.ThisMethod();
            Console.WriteLine ($"Version: {Environment.Version}");

            // var npoiMapper             = new NpoiMapper ();
            // var htmlTableParser        = new HtmlTableParser ();
            // var angleSharpScraper      = new AngleSharpScraper ();
            // var htmlAgilityPackScraper = new HtmlAgilityPackScraper ();
            // var ironScraper            = new IronScraper ();
            // var linqToTwitter = new LinqToTwitter();


            var oAuth = new OAuthControllerX();
            oAuth.Intro("o auth");
            oAuth.Dig();
            try {
                oAuth.BeginAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception is {ex}");
            }

            // CreateWebHostBuilder (args).Build ().Run ();
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost (string[] args) => 
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup> ()
            .Build();



        public static void StartStop()
        {
            Start.ThisMethod();
            Complete.ThisMethod();
        }
    }
}