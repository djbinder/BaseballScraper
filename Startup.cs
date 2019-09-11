using BaseballScraper.Controllers.BaseballHQControllers;
using BaseballScraper.Controllers.CbsControllers;
using BaseballScraper.Controllers.EspnControllers;
using BaseballScraper.Controllers.MlbDataApiControllers;
using BaseballScraper.Controllers.PlayerControllers;
using BaseballScraper.Controllers.YahooControllers;
using BaseballScraper.EndPoints;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.ConfigurationModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using static BaseballScraper.Controllers.PlayerControllers.PlayerBaseController;
using static BaseballScraper.EndPoints.BaseballSavantUriEndPoints;
using static BaseballScraper.Infrastructure.Helpers;
using System;
using System.Diagnostics;
using System.Text;
using C = System.Console;
using BaseballScraper.Controllers.BrooksBaseballControllers;

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
                .AddJsonFile("Configuration/airtableConfiguration.json"              ,optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/appsettings.{env.EnvironmentName}.json" ,optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/appsettings.json"                       ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/googleCredentials.json"                  ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/gSheetNames.json"                        ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/mongoDbConfiguration.json"               ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/theGameIsTheGameConfig.json"             ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/yahooConfig.json"                        ,optional: false, reloadOnChange: true)
                .AddJsonFile("Configuration/twitterConfiguration.json"               ,optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = configuration;

            // set all user secrets from appsettings.Development.json:
                /*
                    cat ./Configuration/airtableConfiguration.json | dotnet user-secrets set
                    cat ./Configuration/appsettings.Development.json | dotnet user-secrets set
                    cat ./Configuration/appsettings.json | dotnet user-secrets set
                    cat ./Configuration/googleCredentials.json | dotnet user-secrets set
                    cat ./Configuration/gSheetNames.json | dotnet user-secrets set
                    cat ./Configuration/mongoDbConfiguration.json | dotnet user-secrets set
                    cat ./Configuration/theGameIsTheGameConfig.json | dotnet user-secrets set
                    cat ./Configuration/yahooConfig.json | dotnet user-secrets set
                    cat ./Configuration/twitterConfiguration.json | dotnet user-secrets set
                */
            // Other secrets commands:
            // * dotnet user-secrets list
            // * dotnet user-secrets clear
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();

                builder.AddUserSecrets<AirtableConfiguration>();
                builder.AddUserSecrets<BaseballHqCredentials>();
                builder.AddUserSecrets<GoogleSheetConfiguration>();
                builder.AddUserSecrets<MongoDbConfiguration>();
                builder.AddUserSecrets<PostGresDbConfiguration>();
                builder.AddUserSecrets<TheGameIsTheGameConfiguration>();
                builder.AddUserSecrets<TwitterConfiguration>();
                builder.AddUserSecrets<YahooConfiguration>();
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
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddSessionStateTempDataProvider()
                    .AddControllersAsServices();

                services.AddSession (options =>
                {
                    options.IdleTimeout        = TimeSpan.FromDays(1);
                    options.Cookie.HttpOnly    = true;
                    options.Cookie.IsEssential = true;
                });


                services.AddDistributedMemoryCache();
                services.AddOptions(); // Required to use the Options<T> pattern

                AddSwagger_ConfigureServices(services);


            #endregion .NET CORE CONFIGURATION


            /* CONFIGURE INDIVIDUAL SERVICES */
            ConfigureAirtableServices(services);
            ConfigureBaseballHqCredentials(services);
            ConfigureGoogleSheetsServices(services);
            ConfigureMongoDbServices(services);
            ConfigureSqlDbServices(services);
            ConfigureTwitterServices(services);
            ConfigureYahooServices(services);

            // Singletons:
            // * A singleton is one instance per application
            // * No matter how many times we refresh the page, it’s never going to create a new instance
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ApiInfrastructure>();
            services.AddSingleton<BaseballSavantUriEndPoints>();
            services.AddSingleton<BaseballSavantHitterEndPoints>();
            services.AddSingleton<BaseballSavantPitcherEndPoints>();
            services.AddSingleton<BrooksBaseballEndPoints>();
            services.AddSingleton<BrooksBaseballUtilitiesController>();
            services.AddSingleton<CsvHandler>();
            services.AddSingleton<DataTabler>();
            services.AddSingleton<EmailHelper>();
            services.AddSingleton<FanGraphsUriEndPoints>();
            services.AddSingleton<Helpers>();
            services.AddSingleton<MlbDataApiEndPoints>();
            services.AddSingleton<MlbDataSeasonHittingStatsController>();
            services.AddSingleton<PostmanMethods>();
            services.AddSingleton<ProjectDirectoryEndPoints>();
            services.AddSingleton<PythonConnector>();
            services.AddSingleton<RdotNetConnector>();
            services.AddSingleton<YahooApiEndPoints>();

            // services.AddTransient<PlayerBaseFromGoogleSheet>();

            // test
            // Scoped:
            // * A new instance will be created essentially per page load

            services.AddScoped<PlayerBaseController>();
            services.AddScoped<CbsTransactionTrendsController>();
            services.AddScoped<EspnTransactionTrendsController>();
            services.AddScoped<YahooTransactionTrendsController>();

            // * See: https://blog.zhaytam.com/2019/03/14/generic-repository-pattern-csharp/
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));

            _gmail1 = Configuration["Gmail1"];
            _gmail1PasswordAppAccess = Configuration["Gmail1_Password_App_Access"];

            // Connects to Diagnoser Option 2 in 'Configure' section below
            // https://andrewlock.net/understanding-your-middleware-pipeline-with-the-middleware-analysis-package/
            services.AddMiddlewareAnalysis();

            // example of how to console config items
            // C.WriteLine(Configuration["YahooConfiguration:Name"]);
            // C.WriteLine(Configuration["TheGameIsTheGame:2018Season:LeagueKey"]);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, DiagnosticListener diagnosticListener, DiagnosticListener fullDiagnosticListener)
        {
            HttpHelper.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Baseball Scraper API V1");
            });

            if (env.IsDevelopment ())
            {
                app.UseDeveloperExceptionPage ();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // adds 'Exception Handling' middleware in non-dev environments
                app.UseExceptionHandler ("/Error");
                app.UseHsts ();
            }

            AddListeners(diagnosticListener, fullDiagnosticListener);

            app.UseHttpsRedirection ();
            app.UseDefaultFiles();
            app.UseStaticFiles ();
            app.UseSession ();
            app.UseCookiePolicy ();
            app.UseAuthentication();
            app.UseMvc();
        }

        public void AddListeners(DiagnosticListener diagnosticListener, DiagnosticListener fullDiagnosticListener)
        {
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
        }







    //----------------------------------------------------------------------------
    /* CONFIGURE INDIVIDUAL SERVICES                                            */
    //----------------------------------------------------------------------------



    #region .NET SERVICES ------------------------------------------------------------


        /* ----- SWAGGER ----- */
        // Register the Swagger generator, defining 1 or more Swagger documents
        // * See: .NET Tutorial: https://bit.ly/2OWOF6K
        // * See: https://bit.ly/2KyPY7p
        // * See: https://github.com/swagger-api/swagger-ui
        // * Swagger is ignored when this is added to Controller:
        // * [ApiExplorerSettings(IgnoreApi = true)]
        public void AddSwagger_ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Baseball Scraper", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                // * XML comments will show up Swagger
                c.IncludeXmlComments("App.xml");
            });

        }


    #endregion .NET SERVICES ------------------------------------------------------------





    #region CONFIGURE AIRTABLE ------------------------------------------------------------

        public void ConfigureAirtableServices(IServiceCollection services)
        {
            string airtableConfiguration = "AirtableConfiguration";

            #region AIR TABLE KEY
                services.Configure<AirtableConfiguration>(Configuration.GetSection("AirtableConfiguration"));
                services.Configure<AirtableConfiguration>(config =>
                {
                    config.ApiKey = Configuration["AirtableConfiguration:ApiKey"];
                });

                // Console.WriteLine(Configuration["AirtableConfiguration:ApiKey"]);

                var airTableKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["AirtableConfiguration:ApiKey"])
                );
                // Console.WriteLine(Configuration["AirtableConfiguration:ApiKey"]);

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

            /* Configure individual table */
            string spRankings = "SpRankings";
            string authors    = "Authors";
            string websites   = "Websites";
            string baScConfig = "BaseballScraperConfig";

            services.Configure<AirtableConfiguration>(spRankings, Configuration.GetSection(spRankings));
            services.Configure<AirtableConfiguration>(authors,    Configuration.GetSection(authors));
            services.Configure<AirtableConfiguration>(websites,   Configuration.GetSection(websites));
            services.Configure<AirtableConfiguration>(baScConfig, Configuration.GetSection($"{airtableConfiguration}:{baScConfig}"));

            services.AddTransient<AirtableConfiguration>();
            services.AddTransient<AirtableManager>();
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





    #region CREDENTIALS  ------------------------------------------------------------

        public void ConfigureBaseballHqCredentials(IServiceCollection services)
        {
            services.Configure<BaseballHqCredentials>(
                config =>
                {
                    config.UserName  = Configuration.GetSection("BaseballHqCredentials:UserName").Value;
                    config.Password  = Configuration.GetSection("BaseballHqCredentials:Password").Value;
                    config.LoginLink = Configuration.GetSection("BaseballHqCredentials:LoginLink").Value;
                });

                services.AddSingleton<IBaseballHqCredentials>(hq => hq.GetRequiredService<IOptions<BaseballHqCredentials>>().Value);

            services.AddScoped<BaseballHqUtilitiesController>();
            services.AddScoped<BaseballHqHitterController>();
        }

    #endregion CREDENTIALS  ------------------------------------------------------------





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
                config.RefreshToken    = Configuration["YahooConfiguration:AccessToken"];
                config.RefreshToken    = Configuration["YahooConfiguration:RefreshToken"];
                config.XOAuthYahooGuid = Configuration["YahooConfiguration:XOAuthYahooGuid"];
                config.ExpiresIn       = int.Parse(Configuration["YahooConfiguration:ExpiresIn"]);
            });

            // C.WriteLine(Configuration["YahooConfiguration:Name"]);
            // C.WriteLine(Configuration["YahooConfiguration:XOAuthYahooGuid"]);

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
                config.ConnectionString     = Configuration["DBInfo:ConnectionString"];
                config.Name                 = Configuration["DBInfo:Name"];
                config.SqlName              = Configuration["DBInfo2:Name"];
                config.SqlConnectionString  = Configuration["DBInfo2:ConnectionString"];
            });

            services.AddDbContext<BaseballScraperContext>(options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));

            services.AddDbContext<BaseballScraperContext>(options =>
                options.UseSqlServer(Configuration["DBInfo2:ConnectionString"]));
            // C.WriteLine($"DBInfo2 Name: {Configuration["DBInfo2:Name"]}");
            // C.WriteLine($"DBInfo2 Connection String: {Configuration["DBInfo2:ConnectionString"]}");
        }

    #endregion CONFIGURE SQL DB SERVICES  ------------------------------------------------------------





    #region CONFIGURE TWITTER SERVICES  ------------------------------------------------------------

        public void ConfigureXServices(IServiceCollection services)
        {

        }

    #endregion CONFIGURE  ------------------------------------------------------------

    }
}
