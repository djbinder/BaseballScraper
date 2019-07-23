using System;
using System.Diagnostics;
using System.Text;
using BaseballScraper.Controllers.MlbDataApiControllers;
using BaseballScraper.Controllers.PlayerControllers;
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
using static BaseballScraper.Controllers.PlayerControllers.PlayerBaseController;
using static BaseballScraper.EndPoints.BaseballSavantUriEndPoints;
using static BaseballScraper.Infrastructure.Helpers;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper
{
    public class Startup
    {
        private readonly Helpers _h = new Helpers();

        public IConfiguration Configuration { get; set; }

        private string _gmail1;
        private string _gmail1PasswordAppAccess;


        public Startup (IHostingEnvironment env, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"Configuration/appsettings.json"                       ,optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/appsettings.{env.EnvironmentName}.json" ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/googleCredentials.json"                  ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/gSheetNames.json"                        ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/mongoDbConfiguration.json"               ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/theGameIsTheGameConfig.json"             ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/yahooConfig.json"                        ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/airtableConfiguration.json"              ,optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

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

                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                services.Configure<CookiePolicyOptions> (options =>
                {
                    options.CheckConsentNeeded    = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });


                services.AddMvc ()
                    .SetCompatibilityVersion (CompatibilityVersion.Version_2_1)
                    .AddSessionStateTempDataProvider()
                    .AddControllersAsServices()
                    .AddRazorPagesOptions(options =>
                    {
                        // this sets the apps default (i.e., index, home) route to be /Pages/Dashboard
                        options.Conventions.AddPageRoute("/DashboardPage","");
                    });


                services.AddSession (options =>
                {
                    options.IdleTimeout        = TimeSpan.FromDays(1);
                    options.Cookie.HttpOnly    = true;
                    options.Cookie.IsEssential = true;
                });


                services.AddDistributedMemoryCache();
                services.AddOptions(); // Required to use the Options<T> pattern

            #endregion .NET CORE CONFIGURATION


            services.AddDbContext<BaseballScraperContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DBInfo2:ConnectionString")));


            /* CONFIGURE INDIVIDUAL SERVICES */
            ConfigureAirtableServices(services);
            ConfigureGoogleSheetsServices(services);
            ConfigureMongoDbServices(services);
            ConfigureSqlDbServices(services);
            ConfigureTwitterServices(services);
            ConfigureYahooServices(services);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<ApiInfrastructure>();
            services.AddSingleton<BaseballSavantUriEndPoints>();
            services.AddSingleton<BaseballSavantHitterEndPoints>();
            services.AddSingleton<BaseballSavantPitcherEndPoints>();
            services.AddSingleton<CsvHandler>();
            services.AddSingleton<DataTabler>();
            services.AddSingleton<EmailHelper>();
            services.AddSingleton<FanGraphsUriEndPoints>();
            services.AddSingleton<Helpers>();
            services.AddSingleton<MlbDataApiEndPoints>();
            services.AddSingleton<MlbDataSeasonHittingStatsController>();
            services.AddSingleton<PlayerBaseController>();
            services.AddSingleton<PlayerBaseFromGoogleSheet>();
            services.AddSingleton<PostmanMethods>();
            services.AddSingleton<PythonConnector>();
            services.AddSingleton<RdotNetConnector>();
            services.AddSingleton<YahooApiEndPoints>();


            _gmail1 = Configuration["Gmail1"];
            _gmail1PasswordAppAccess = Configuration["Gmail1_Password_App_Access"];

            // #region SQL DB CONFIGURATION
            //     services.Configure<BaseballScraperContext>(Configuration);
            //     services.Configure<BaseballScraperContext>(config =>
            //     {
            //         config.ConnectionString = Configuration["DBInfo:ConnectionString"];
            //         config.Name             = Configuration["DBInfo:Name"];
            //     });
            //     services.AddDbContext<BaseballScraperContext>(options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));
            // #endregion SQL DB CONFIGURATION



            // #region YAHOO CONFIGURATION
            //     // this connects to secrets
            //     services.Configure<YahooConfiguration>(Configuration.GetSection("YahooConfiguration"));
            //     services.Configure<YahooConfiguration>(config =>
            //     {
            //         config.Name            = Configuration["YahooConfiguration:Name"];
            //         config.AppId           = Configuration["YahooConfiguration:AppId"];
            //         config.ClientId        = Configuration["YahooConfiguration:ClientId"];
            //         config.ClientSecret    = Configuration["YahooConfiguration:ClientSecret"];
            //         config.Base64Encoding  = Configuration["YahooConfiguration:Base64Encoding"];
            //         config.ClientPublic    = Configuration["YahooConfiguration:ClientPublic"];
            //         config.RedirectUri     = Configuration["YahooConfiguration:RedirectUri"];
            //         config.RefreshToken    = Configuration["YahooConfiguration:RefreshToken"];
            //         config.XOAuthYahooGuid = Configuration["YahooConfiguration:XOAuthYahooGuid"];
            //         config.ExpiresIn       = int.Parse(Configuration["YahooConfiguration:ExpiresIn"]);
            //     });

            //     services.Configure<TheGameIsTheGameConfiguration>(config =>
            //     {
            //         config.LeagueKey = Configuration["TheGameIsTheGame:2019Season:LeagueKey"];
            //     });

            //     services.AddTransient<Controllers.YahooControllers.YahooAuthController>();
            // #endregion YAHOO CONFIGURATION


            // #region TWITTER CONFIGURATION
            //     services.Configure<TwitterConfiguration>(Configuration.GetSection("TwitterConfiguration"));
            //     services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<TwitterConfiguration>>().Value);

            //     services.Configure<TwitterConfiguration>(config =>
            //     {
            //         config.AccessToken       = Configuration["TwitterConfiguration:AccessToken"];
            //         config.AccessTokenSecret = Configuration["TwitterConfiguration:AccessTokenSecret"];
            //         config.ConsumerKey       = Configuration["TwitterConfiguration:ConsumerKey"];
            //         config.ConsumerSecret    = Configuration["TwitterConfiguration:ConsumerSecret"];
            //     });
            // #endregion TWITTER CONFIGURATION


            // #region AIR TABLE CONFIGURATION
                // services.Configure<AirtableConfiguration>(Configuration.GetSection("AirtableConfiguration"));
                // services.Configure<AirtableConfiguration>(config =>
                // {
                //     config.ApiKey = Configuration["AirtableConfiguration:ApiKey"];
                // });

                // var airtableKey = new SymmetricSecurityKey(
                //     Encoding.UTF8.GetBytes(Configuration["AirtableConfiguration:ApiKey"])
                // );

                // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //     .AddJwtBearer(options =>
                //     {
                //         options.TokenValidationParameters = new TokenValidationParameters
                //         {
                //             IssuerSigningKey    = airtableKey,
                //             RequireSignedTokens = true,
                //         };
                //         options.IncludeErrorDetails = true;
                //     });
            // #endregion AIR TABLE CONFIGURATION


            // #region MONGO DB CONFIGURATION

            //     // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-2.2&tabs=visual-studio-mac
            //     // See: mongoDbConfiguration.json, MongoDbServicer.cs and MongoDbConfiguration.cs

            //     // The configuration instance in mongoDbConfiguration.json file to which MongoDbConfiguration binds to is registered in the Dependency Injection (DI) container
            //         // E.g., a MongoDbConfiguration object's ConnectionString property is populated with the MongoDbConfiguration:ConnectionString property in mongoDbConfiguration.json
            //     services.Configure<MongoDbConfiguration>(
            //         options =>
            //         {
            //             options.ConnectionString = Configuration.GetSection("MongoDbConfiguration:ConnectionString").Value;
            //             options.DatabaseName = Configuration.GetSection("MongoDbConfiguration:DatabaseName").Value;
            //             options.TweetsCollectionName = Configuration.GetSection("MongoDbConfiguration:TweetsCollectionName").Value;
            //         });

            //     // IMongoDbConfiguration interface is registered in DI with a singleton service lifetime
            //     // When injected, the interface instance resolves to a MongoDbConfiguration object
            //     services.AddSingleton<IMongoDbConfiguration>(sp =>          sp.GetRequiredService<IOptions<MongoDbConfiguration>>().Value);

            //     // MongoDbServicer class is registered with DI to support constructor injection in consuming classes
            //     // The singleton service lifetime is most appropriate because MongoDbServicer takes a direct dependency on MongoClient
            //     // Per official Mongo guidelines, MongoClient should be registered in DI with a singleton service
            //     services.AddSingleton<MongoDbServicer>();

            // #endregion MONGO DB CONFIGURATION



            // #region GOOGLE SHEETS CONFIGURATION

            //     // See: gSheetNames.json configuration file, GoogleSheetsConnector.cs, GoogleSheetConfiguration.cs
            //     // See: https://dotnetcoretutorials.com/2018/03/20/cannot-consume-scoped-service-from-singleton-a-lesson-in-asp-net-core-di-scopes/

            //     string sfbbPlayerIdMap       = "SfbbPlayerIdMap";
            //     string crunchtimePlayerIdMap = "CrunchtimePlayerIdMap";
            //     string spCustomAnalysisA     = "SpCustomAnalysisA";
            //     string yahooTrends           = "YahooTrends";
            //     string fgSpMasterImport      = "FgSpMasterImport";

            //     // these are equal to the json group name in gSheetNames.json configuration file
            //     services.Configure<GoogleSheetConfiguration>(sfbbPlayerIdMap, Configuration.GetSection(sfbbPlayerIdMap));
            //     services.Configure<GoogleSheetConfiguration>(crunchtimePlayerIdMap, Configuration.GetSection(crunchtimePlayerIdMap));
            //     services.Configure<GoogleSheetConfiguration>(spCustomAnalysisA, Configuration.GetSection(spCustomAnalysisA));
            //     services.Configure<GoogleSheetConfiguration>(yahooTrends, Configuration.GetSection(yahooTrends));
            //     services.Configure<GoogleSheetConfiguration>(fgSpMasterImport, Configuration.GetSection(fgSpMasterImport));

            //     services.AddTransient<GoogleSheetConfiguration>();
            //     services.AddTransient<GoogleSheetsConnector>();

            // #endregion GOOGLE SHEETS CONFIGURATION




            // Connects to Diagnoser Option 2 in 'Configure' section below
            // https://andrewlock.net/understanding-your-middleware-pipeline-with-the-middleware-analysis-package/
            services.AddMiddlewareAnalysis();

            // example of how to console config items
            // Console.WriteLine(Configuration["YahooConfiguration:Name"]);
            // Console.WriteLine(Configuration["TheGameIsTheGame:2018Season:LeagueKey"]);
        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptionsMonitor<TwitterConfiguration> twitterConfigMonitor, DiagnosticListener diagnosticListener, DiagnosticListener fullDiagnosticListener)
        {

            // var result = string.IsNullOrEmpty(_gmail1) ? "Null" : "Not Null";
            // app.Run(async(context) =>
            // {
            //     await context.Response.WriteAsync($"From Startup.Configure: Secret is {result}");
            // });

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
                // adds 'Exception Handling' middleware in non-dev environments
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

            // DIAGNOSER: Option 1: these connect to diagnostic helpers
            // https://andrewlock.net/logging-using-diagnosticsource-in-asp-net-core/
            // Listen for middleware events and log them to the console.
            var listener = new MiddlewareDiagnoserListener();
                diagnosticListener.SubscribeWithAdapter(listener);
                // uncomment this to log middleware info for each request
                // app.UseMiddleware<MiddlewareDiagnoser>();

            // DIAGNOSER: Option 2: these connect to diagnostic helpers
            // https://andrewlock.net/understanding-your-middleware-pipeline-with-the-middleware-analysis-package/
            var fullListener = new FullDiagnosticListener();
                // uncomment this to log middleware info for each request
                // fullDiagnosticListener.SubscribeWithAdapter(fullListener);


            app.UseHttpsRedirection ();
            app.UseDefaultFiles();
            app.UseStaticFiles ();
            app.UseSession ();
            app.UseCookiePolicy ();
            app.UseAuthentication();
            app.UseMvc();
        }



        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //
        //
        /* CONFIGURE INDIVIDUAL SERVICES */
        //
        //
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------




        #region CONFIGURE AIRTABLE ------------------------------------------------------------

            public void ConfigureAirtableServices(IServiceCollection services)
            {
                #region AIR TABLE KEY
                    services.Configure<AirtableConfiguration>(Configuration.GetSection("AirtableConfiguration"));
                    services.Configure<AirtableConfiguration>(config =>
                    {
                        config.ApiKey = Configuration["AirtableConfiguration:ApiKey"];
                    });

                    var airTableKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["AirtableConfiguration:ApiKey"])
                    );

                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                IssuerSigningKey    = airTableKey,
                                RequireSignedTokens = true,
                            };
                            options.IncludeErrorDetails = true;
                        });
                #endregion AIR TABLE KEY


                #region AIR TABLE - TABLES CONFIGURATION

                    string spRankings = "SpRankings";
                    string authors    = "Authors";
                    string websites   = "Websites";

                    services.Configure<AirtableConfiguration>(spRankings, Configuration.GetSection(spRankings));
                    services.Configure<AirtableConfiguration>(authors,    Configuration.GetSection(authors));
                    services.Configure<AirtableConfiguration>(websites,   Configuration.GetSection(websites));

                    services.AddTransient<AirtableConfiguration>();
                    services.AddTransient<AirtableManager>();

                #endregion AIR TABLE - TABLES CONFIGURATION
            }

        #endregion CONFIGURE AIRTABLE ------------------------------------------------------------




        #region CONFIGURE MONGO DB ------------------------------------------------------------

            public void ConfigureMongoDbServices(IServiceCollection services)
            {
                // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-2.2&tabs=visual-studio-mac
                // See: mongoDbConfiguration.json, MongoDbServicer.cs and MongoDbConfiguration.cs

                // The configuration instance in mongoDbConfiguration.json file to which MongoDbConfiguration binds to is registered in the Dependency Injection (DI) container
                    // E.g., a MongoDbConfiguration object's ConnectionString property is populated with the MongoDbConfiguration:ConnectionString property in mongoDbConfiguration.json
                services.Configure<MongoDbConfiguration>(
                    options =>
                    {
                        options.ConnectionString =     Configuration.GetSection("MongoDbConfiguration:ConnectionString").Value;
                        options.DatabaseName =         Configuration.GetSection("MongoDbConfiguration:DatabaseName").Value;
                        options.TweetsCollectionName = Configuration.GetSection("MongoDbConfiguration:TweetsCollectionName").Value;
                    });

                // IMongoDbConfiguration interface is registered in DI with a singleton service lifetime
                // When injected, the interface instance resolves to a MongoDbConfiguration object
                services.AddSingleton<IMongoDbConfiguration>(sp =>          sp.GetRequiredService<IOptions<MongoDbConfiguration>>().Value);

                // MongoDbServicer class is registered with DI to support constructor injection in consuming classes
                // The singleton service lifetime is most appropriate because MongoDbServicer takes a direct dependency on MongoClient
                // Per official Mongo guidelines, MongoClient should be registered in DI with a singleton service
                services.AddSingleton<MongoDbServicer>();
            }

        #endregion CONFIGURE MONGO DB ------------------------------------------------------------




        #region CONFIGURE GOOGLE SHEETS ------------------------------------------------------------

            public void ConfigureGoogleSheetsServices(IServiceCollection services)
            {
                // See: gSheetNames.json configuration file, GoogleSheetsConnector.cs, GoogleSheetConfiguration.cs
                // See: https://dotnetcoretutorials.com/2018/03/20/cannot-consume-scoped-service-from-singleton-a-lesson-in-asp-net-core-di-scopes/

                string sfbbPlayerIdMap       = "SfbbPlayerIdMap";
                string crunchtimePlayerIdMap = "CrunchtimePlayerIdMap";
                string spCustomAnalysisA     = "SpCustomAnalysisA";
                string yahooTrends           = "YahooTrends";
                string fgSpMasterImport      = "FgSpMasterImport";

                // these are equal to the json group name in gSheetNames.json configuration file
                services.Configure<GoogleSheetConfiguration>(sfbbPlayerIdMap,       Configuration.GetSection(sfbbPlayerIdMap));
                services.Configure<GoogleSheetConfiguration>(crunchtimePlayerIdMap, Configuration.GetSection(crunchtimePlayerIdMap));
                services.Configure<GoogleSheetConfiguration>(spCustomAnalysisA,     Configuration.GetSection(spCustomAnalysisA));
                services.Configure<GoogleSheetConfiguration>(yahooTrends,           Configuration.GetSection(yahooTrends));
                services.Configure<GoogleSheetConfiguration>(fgSpMasterImport,      Configuration.GetSection(fgSpMasterImport));

                services.AddTransient<GoogleSheetConfiguration>();
                services.AddTransient<GoogleSheetsConnector>();
            }

        #endregion CONFIGURE GOOGLE SHEETS ------------------------------------------------------------




        #region CONFIGURE TWITTER SERVICES  ------------------------------------------------------------

            public void ConfigureTwitterServices(IServiceCollection services)
            {
                services.Configure<TwitterConfiguration>(Configuration.GetSection("TwitterConfiguration"));
                services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<TwitterConfiguration>>().Value);

                services.Configure<TwitterConfiguration>(config =>
                {
                    config.AccessToken       = Configuration["TwitterConfiguration:AccessToken"];
                    config.AccessTokenSecret = Configuration["TwitterConfiguration:AccessTokenSecret"];
                    config.ConsumerKey       = Configuration["TwitterConfiguration:ConsumerKey"];
                    config.ConsumerSecret    = Configuration["TwitterConfiguration:ConsumerSecret"];
                });
            }

        #endregion CONFIGURE TWITTER SERVICES  ------------------------------------------------------------




        #region CONFIGURE YAHOO SERVICES  ------------------------------------------------------------

            public void ConfigureYahooServices(IServiceCollection services)
            {
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
                    config.ExpiresIn       = int.Parse(Configuration["YahooConfiguration:ExpiresIn"]);
                });

                services.Configure<TheGameIsTheGameConfiguration>(config =>
                {
                    config.LeagueKey = Configuration["TheGameIsTheGame:2019Season:LeagueKey"];
                });

                services.AddTransient<Controllers.YahooControllers.YahooAuthController>();

            }

        #endregion CONFIGURE YAHOO SERVICES  ------------------------------------------------------------




        #region CONFIGURE SQL DB SERVICES  ------------------------------------------------------------

            public void ConfigureSqlDbServices(IServiceCollection services)
            {


                services.Configure<BaseballScraperContext>(Configuration);
                services.Configure<BaseballScraperContext>(config =>
                {
                    config.ConnectionString = Configuration["DBInfo:ConnectionString"];
                    config.Name             = Configuration["DBInfo:Name"];
                    config.SqlName = Configuration["DBInfo2:Name"];
                    config.SqlConnectionString = Configuration["DBInfo2:ConnectionString"];
                });

                services.AddDbContext<BaseballScraperContext>(options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));

                Console.WriteLine(Configuration["DBInfo2:Name"]);
                Console.WriteLine(Configuration["DBInfo2:ConnectionString"]);
            }

        #endregion CONFIGURE SQL DB SERVICES  ------------------------------------------------------------




        #region CONFIGURE TWITTER SERVICES  ------------------------------------------------------------
            public void ConfigureXServices(IServiceCollection services)
            {

            }
        #endregion CONFIGURE  ------------------------------------------------------------

    }
}
