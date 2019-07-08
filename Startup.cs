using System;
using System.Diagnostics;
using System.Text;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using static BaseballScraper.Infrastructure.Helpers;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper
{

    public class Startup
    {
        // private string _connection = null;
        // private string _postGresDbConnectionString = null;

        private readonly Helpers _h = new Helpers();

        // private readonly IConfiguration _config;


        public IConfiguration Configuration { get; set; }

        // public Startup(IConfiguration configuration)
        // {
        //     _h.StartMethod();
        //     Configuration = configuration;
        // }

        // public Startup (IHostingEnvironment env)
        // public Startup (IHostingEnvironment env, IConfiguration config, IConfiguration configuration)
        public Startup (IHostingEnvironment env, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"Configuration/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/googleCredentials.json", optional:false, reloadOnChange:true)
                .AddJsonFile("Configuration/gSheetNames.json", optional:false, reloadOnChange:true)
                .AddJsonFile("Configuration/mongoDbConfiguration.json", optional:false, reloadOnChange:true)
                .AddJsonFile("Configuration/theGameIsTheGameConfig.json", optional:false, reloadOnChange:true)
                .AddJsonFile("Configuration/yahooConfig.json", optional:false, reloadOnChange:true)
                .AddEnvironmentVariables();
                // .AddJsonFile("secrets.json", optional:true, reloadOnChange:false)

            // _config = config;
            Configuration = configuration;

            // set all user secrets from appsettings.Development.json:
                // cat ./Configuration/appsettings.Development.json | dotnet user-secrets set
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
                builder.AddUserSecrets<TwitterConfiguration>();
                builder.AddUserSecrets<AirtableConfiguration>();
                builder.AddUserSecrets<YahooConfiguration>();
                builder.AddUserSecrets<TheGameIsTheGameConfiguration>();
            }
            Configuration = builder.Build();
        }




        // export ASPNETCORE_ENVIRONMENT="Development"
        // This method gets called by the runtime
        // Use this method to add services to the container
        public void ConfigureServices (IServiceCollection services)
        {

            #region .NET CORE CONFIGURATION

                services.Configure<CookiePolicyOptions> (options => {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

                services.AddOptions(); // Required to use the Options<T> pattern

                services.AddMvc ()
                    .SetCompatibilityVersion (CompatibilityVersion.Version_2_1)
                    .AddSessionStateTempDataProvider()
                    .AddControllersAsServices()
                    .AddRazorPagesOptions(options =>
                    {
                        // this sets the apps default (i.e., index, home) route to be /Pages/Dashboard
                        options.Conventions.AddPageRoute("/Dashboard","");
                    });

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                services.AddDistributedMemoryCache();
                services.AddSession (options =>
                {
                    options.IdleTimeout = TimeSpan.FromDays(1);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });

            #endregion .NET CORE CONFIGURATION



            #region YAHOO CONFIGURATION

                // this connects to secrets
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
                    // config.TokenType       = Configuration["YahooConfiguration:TokenType"];
                    // config.RequestUriBase  = Configuration["YahooConfiguration:RequestUriBase"];
                    // config.RequestAuthUri  = Configuration["YahooConfiguration:RequestAuthUri"];
                    // config.GetTokenBase    = Configuration["YahooConfiguration:GetTokenBase"];
                });

                services.AddSingleton<YahooApiEndPoints>();

                // services.Configure<TheGameIsTheGameConfiguration>(Configuration);
                // services.Configure<TheGameIsTheGameConfiguration>(config =>
                // {
                //     config.LeagueKey = Configuration["TheGameIsTheGame:2018Season:LeagueKey"];
                // });
                services.Configure<TheGameIsTheGameConfiguration>(config =>
                {
                    config.LeagueKey = Configuration["TheGameIsTheGame:2019Season:LeagueKey"];
                });

                services.AddTransient<Controllers.YahooControllers.YahooAuthController>();

            #endregion YAHOO CONFIGURATION



            #region TWITTER CONFIGURATION

                services.Configure<TwitterConfiguration>(Configuration.GetSection("TwitterConfiguration"));
                services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<TwitterConfiguration>>().Value);

                services.Configure<TwitterConfiguration>(config =>
                {
                    config.AccessToken       = Configuration["TwitterConfiguration:AccessToken"];
                    config.AccessTokenSecret = Configuration["TwitterConfiguration:AccessTokenSecret"];
                    config.ConsumerKey       = Configuration["TwitterConfiguration:ConsumerKey"];
                    config.ConsumerSecret    = Configuration["TwitterConfiguration:ConsumerSecret"];
                });

            #endregion TWITTER CONFIGURATION



            #region AIR TABLE CONFIGURATION

                services.Configure<AirtableConfiguration>(Configuration.GetSection("AirtableConfiguration"));
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

            #endregion AIR TABLE CONFIGURATION



            #region SQL DB CONFIGURATION

                services.Configure<BaseballScraperContext>(Configuration);
                services.Configure<BaseballScraperContext>(config =>
                {
                    config.ConnectionString = Configuration["DBInfo:ConnectionString"];
                    config.Name             = Configuration["DBInfo:Name"];
                });

                // Console.WriteLine($"DB Info Connection String: {Configuration["DBInfo:ConnectionString"]}");
                services.AddDbContext<BaseballScraperContext>(options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));

            #endregion SQL DB CONFIGURATION



            #region MONGO DB CONFIGURATION

                // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-2.2&tabs=visual-studio-mac
                // Configuration is set up in mongoDbConfiguration.json
                // these connect to 'MongoDbServicer.cs' and "MongoDbConfiguration' models

                // The configuration instance in mongoDbConfiguration.json file to which MongoDbConfiguration binds to  is registered in the Dependency Injection (DI) container
                    // E.g., a MongoDbConfiguration object's ConnectionString property is populated with the MongoDbConfiguration:ConnectionString property in mongoDbConfiguration.json
                services.Configure<MongoDbConfiguration>(
                    options =>
                    {
                        options.ConnectionString = Configuration.GetSection("MongoDbConfiguration:ConnectionString").Value;
                        options.DatabaseName = Configuration.GetSection("MongoDbConfiguration:DatabaseName").Value;
                        options.TweetsCollectionName = Configuration.GetSection("MongoDbConfiguration:TweetsCollectionName").Value;
                    });

                // Console.WriteLine($"Mongo Connection String: {Configuration["MongoDbConfiguration:ConnectionString"]}");
                // Console.WriteLine($"Mongo DatabaseName: {Configuration["MongoDbConfiguration:DatabaseName"]}");
                // Console.WriteLine($"Mongo Collection: {Configuration["MongoDbConfiguration:TweetsCollectionName"]}");

                // The IMongoDbConfiguration interface is registered in DI with a singleton service lifetime
                // When injected, the interface instance resolves to a MongoDbConfiguration object
                services.AddSingleton<IMongoDbConfiguration>(sp => sp.GetRequiredService<IOptions<MongoDbConfiguration>>().Value);

                // MongoDbServicer class is registered with DI to support constructor injection in consuming classes
                // The singleton service lifetime is most appropriate because MongoDbServicer takes a direct dependency on MongoClient
                // Per official Mongo guidelines, MongoClient should be registered in DI with a singleton service
                services.AddSingleton<MongoDbServicer>();

            #endregion MONGO DB CONFIGURATION


            // Connects to Diagnoser Option 2 in 'Configure' section below
            // https://andrewlock.net/understanding-your-middleware-pipeline-with-the-middleware-analysis-package/
            services.AddMiddlewareAnalysis();


            services.AddSingleton<DataTabler>();
            services.AddSingleton<PythonConnector>();
            services.AddSingleton<Helpers>();
            services.AddSingleton<GoogleSheetsConnector>();
            services.AddSingleton<FanGraphsUriEndPoints>();

            // example of how to console config items
            // Console.WriteLine(Configuration["YahooConfiguration:Name"]);
            // Console.WriteLine(Configuration["TheGameIsTheGame:2018Season:LeagueKey"]);
            // Console.WriteLine($"Y! Config Name: {Configuration["YahooConfiguration:Name"]}");
            // Console.WriteLine($"Y! Refresh TOken: {Configuration["YahooConfiguration:RefreshToken"]}");
            // Console.WriteLine($"TGITG League Key: {Configuration["TheGameIsTheGame:2019Season:LeagueKey"]}");
        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptionsMonitor<TwitterConfiguration> twitterConfigMonitor, DiagnosticListener diagnosticListener, DiagnosticListener fullDiagnosticListener)
        {
            // DIAGNOSER: Option 1
            // these connect to diagnostic helpers
            // https://andrewlock.net/logging-using-diagnosticsource-in-asp-net-core/
            // Listen for middleware events and log them to the console.
            var listener = new MiddlewareDiagnoserListener();
                diagnosticListener.SubscribeWithAdapter(listener);
                // uncomment this to log middleware info for each request
                // app.UseMiddleware<MiddlewareDiagnoser>();

            // DIAGNOSER: Option 2
            // these connect to diagnostic helpers
            // https://andrewlock.net/understanding-your-middleware-pipeline-with-the-middleware-analysis-package/
            var fullListener = new FullDiagnosticListener();
                // uncomment this to log middleware info for each request
                // fullDiagnosticListener.SubscribeWithAdapter(fullListener);


            HttpHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            if (env.IsDevelopment ())
            {
                loggerFactory.AddConsole ();
                loggerFactory.AddDebug();
                app.UseDeveloperExceptionPage ();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler ("/Error");
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

            app.UseHttpsRedirection ();
            app.UseDefaultFiles();
            app.UseStaticFiles ();
            app.UseSession ();
            app.UseCookiePolicy ();
            app.UseAuthentication();
            app.UseMvc();
            // app.UseMvcWithDefaultRoute();
            // app.UseMvc(routes =>
            // {
            //     routes.MapRoute(
            //         name    : "default",
            //         template: "{controller=Home}/{action=Index}/{id?}");
            // });
        }
    }
}
