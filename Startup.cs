﻿using System;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace BaseballScraper
{
#pragma warning disable CS0414
    public class Startup
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup (IHostingEnvironment env)
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
                builder.AddUserSecrets<AirtableConfiguration>();
                builder.AddUserSecrets<YahooConfiguration>();
            }

            Configuration = builder.Build();
        }

        //  CONFIGURATION ---> Microsoft.Extensions.Configuration.ConfigurationRoot
        public IConfiguration Configuration { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            Start.ThisMethod();

            // Required to use the Options<T> pattern
            services.AddOptions();

            services.Configure<YahooConfiguration>(Configuration.GetSection("YahooConfiguration"));
            services.Configure<YahooConfiguration>(config =>
            {
                config.Name            = Configuration["YahooConfiguration:Name"];
                config.AppId           = Configuration["YahooConfiguration:AppId"];
                config.ClientId        = Configuration["YahooConfiguration:ClientId"];
                config.ClientSecret    = Configuration["YahooConfiguration:ClientSecret"];
                config.Base64Encoding  = Configuration["YahooConfiguration:Base64Encoding"];
                config.ClientPublic    = Configuration["YahooConfiguration:ClientPublic"];
                config.RedirectUri     = Configuration["YahooConfiguration:RedirectUri"];
                config.RefreshToken    = Configuration["YahooConfiguration:RefreshToken"];
                config.XOAuthYahooGuid = Configuration["YahooConfiguration:XOAuthYahooGuid"];
                config.ExpiresIn       = Int32.Parse(Configuration["YahooConfiguration:ExpiresIn"]);
                config.TokenType       = Configuration["YahooConfiguration:TokenType"];
                config.RequestUriBase  = Configuration["YahooConfiguration:RequestUriBase"];
                config.RequestAuthUri  = Configuration["YahooConfiguration:RequestAuthUri"];
                config.GetTokenBase    = Configuration["YahooConfiguration:GetTokenBase"];
            });


            // TWITTER CONFIGURATION
            services.Configure<TwitterConfiguration>(Configuration.GetSection("TwitterConfiguration"));
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<TwitterConfiguration>>().Value);

            services.Configure<TwitterConfiguration>(config =>
            {
                config.AccessToken       = Configuration["TwitterConfiguration:AccessToken"];
                config.AccessTokenSecret = Configuration["TwitterConfiguration:AccessTokenSecret"];
                config.ConsumerKey       = Configuration["TwitterConfiguration:ConsumerKey"];
                config.ConsumerSecret    = Configuration["TwitterConfiguration:ConsumerSecret"];
            });


            // AIRTABLE CONFIGURATION
            // adds airtable from configuration
            services.Configure<AirtableConfiguration>(Configuration);
            services.Configure<AirtableConfiguration>(config =>
            {
                config.ApiKey = Configuration["AirtableConfiguration:ApiKey"];
            });

            var airtableKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["AirtableConfiguration:ApiKey"])
            );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            IssuerSigningKey    = airtableKey,
                            RequireSignedTokens = true,
                        };

                        options.IncludeErrorDetails = true;
                    });

            // services.AddDbContext<MovieContext> (options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);
            services.AddSession ();
            Complete.ThisMethod();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptionsMonitor<TwitterConfiguration> twitterConfigMonitor)
        {
            Start.ThisMethod();

            if (env.IsDevelopment ())
            {
                loggerFactory.AddConsole ();
                loggerFactory.AddDebug();
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
            app.UseAuthentication();
            app.UseMvc ();
        }
    }
}
