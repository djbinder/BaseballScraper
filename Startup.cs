using System;
using BaseballScraper.Controllers;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration.UserSecrets;
using YahooFantasyWrapper;
using YahooFantasyWrapper.Client;
using YahooFantasyWrapper.Configuration;
using Microsoft.AspNetCore.Http;


// using BaseballScraper.Services.Security;

namespace BaseballScraper
{
    public class Startup
    {
        private static String Start              = "STARTED";
        private static String Complete           = "COMPLETED";
        private string _twitterConsumerKey       = null;
        private string _twitterConsumerSecret    = null;
        private string _twitterAccessToken       = null;
        private string _twitterAccessTokenSecret = null;
        private string _secretString             = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup (IConfiguration configuration, IHostingEnvironment env)
        {
            Start.ThisMethod();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"Configuration/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddJsonFile("secrets.json", optional:false, reloadOnChange:true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
                builder.AddUserSecrets<TwitterConfiguration>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            Start.ThisMethod();


            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);

            services.Configure<YahooFantasyWrapper.Configuration.YahooConfiguration>(Configuration.GetSection("YahooConfiguration"));

            services.Configure<TwitterConfiguration>(Configuration.GetSection("TwitterConfiguration"));

            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<TwitterConfiguration>>().Value);


            _twitterConsumerKey = Configuration["TwitterConfiguration:ConsumerKey"];
            // _twitterConsumerSecret = Configuration["TwitterConfiguration:ConsumerSecret"];
            // _twitterAccessToken       = Configuration["TwitterConfiguration:AccessToken"];
            // _twitterAccessTokenSecret = Configuration["TwitterConfiguration:AccessTokenSecret"];

            _twitterConsumerKey.Intro("twitter consumer key");
            // _twitterConsumerSecret.Intro("twitter consumer key");
            // _twitterAccessToken.Intro("token");
            // _twitterAccessTokenSecret.Intro("token secret");


            services.AddSession ();

            // services.AddDbContext<MovieContext> (options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));

            Complete.ThisMethod();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptionsMonitor<TwitterConfiguration> twitterConfigMonitor)
        {
            Start.ThisMethod();

            loggerFactory.AddConsole ();
            var result = string.IsNullOrEmpty(_twitterConsumerKey) ? "Null" : "Not Null";
            result.Intro("result");

            // app.Run(async (context) =>
            // {
            //     await context.Response.WriteAsync($"Secret is {result}");
            // });

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment ())
            {
                app.UseDeveloperExceptionPage ();
            }
            else
            {
                app.UseHsts ();
            }

            twitterConfigMonitor.OnChange(
                vals => 
                {
                    loggerFactory
                        .CreateLogger<IOptionsMonitor<TwitterConfiguration>>()
                        .LogDebug($"Config changed: {string.Join(", ", vals)}");
                }
            );
            app.UseDefaultFiles();
            app.UseStaticFiles ();
            app.UseSession ();
            app.UseHttpsRedirection ();
            app.UseMvc ();
        }
    }
}
