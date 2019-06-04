using System;
using System.Text;
using BaseballScraper.Models.Configuration;
using BaseballScraper.EndPoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BaseballScraper.Models;
using BaseballScraper.Infrastructure;

namespace BaseballScraper
{
    #pragma warning disable CS0414, CS0219, IDE0051, IDE0059, CS1591, IDE0044
    public class Startup
    {
        // private string _connection = null;
        // private string _postGresDbConnectionString = null;

        private readonly Helpers _h = new Helpers();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup (IHostingEnvironment env)
        {
            // StartMethod();
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
                builder.AddUserSecrets<TheGameIsTheGameConfiguration>();
            }
            Configuration = builder.Build();
        }




        //  CONFIGURATION ---> Microsoft.Extensions.Configuration.ConfigurationRoot
        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions> (options => {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

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

            services.Configure<AirtableConfiguration>(Configuration.GetSection("AirtableConfiguration"));
            services.Configure<AirtableConfiguration>(config =>
            {
                config.ApiKey = Configuration["AirtableConfiguration:ApiKey"];
                Console.WriteLine(config.ApiKey);
            });

            var airtableKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["AirtableConfiguration:ApiKey"])
            );

            services.Configure<TheGameIsTheGameConfiguration>(Configuration);
            services.Configure<TheGameIsTheGameConfiguration>(config =>
            {
                config.LeagueKey = Configuration["TheGameIsTheGame:2018Season:LeagueKey"];
            });

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
            services.AddSingleton<YahooApiEndPoints>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddMvc ()
                .SetCompatibilityVersion (CompatibilityVersion.Version_2_1)
                .AddSessionStateTempDataProvider()
                .AddControllersAsServices()
                .AddRazorPagesOptions(options =>
                {
                    // this sets the apps default (i.e., index, home) route to be /Pages/Dashboard
                    options.Conventions.AddPageRoute("/Dashboard","");
                });

            // services.AddMvc().AddControllersAsServices();
            services.AddTransient<BaseballScraper.Controllers.YahooControllers.YahooAuthController>();
            services.AddSession ();

            services.Configure<BaseballScraperContext>(Configuration);
            services.Configure<BaseballScraperContext>(config =>
            {
                config.ConnectionString = Configuration["DBInfo:ConnectionString"];
                config.Name             = Configuration["DBInfo:Name"];
                Console.WriteLine($"config.ConnectionString: {config.ConnectionString}");
            });


            // Console.WriteLine($"Cstring: {Configuration["DBInfo:ConnectionString"]}");
            // Console.WriteLine($"Name: {Configuration["DBInfo:Name"]}");

            // services.Configure<PostGresDbConfiguration>(config =>
            // {
            //     config.ConnectionString = Configuration["DBInfo:ConnectionString"];
            //     config.Name             = Configuration["DBInfo:Name"];
            //     Console.WriteLine($"config.ConnectionString: {config.ConnectionString}");
            //     Console.WriteLine($"config.Name: {config.Name}");
            // });

            // Console.WriteLine($"Cstring: {Configuration["DBInfo:ConnectionString"]}");
            // Console.WriteLine($"Name: {Configuration["DBInfo:Name"]}");

            services.AddDbContext<BaseballScraperContext>(options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));


            // example of how to console config items
            // Console.WriteLine(Configuration["YahooConfiguration:Name"]);
            // Console.WriteLine(Configuration["TheGameIsTheGame:2018Season:LeagueKey"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptionsMonitor<TwitterConfiguration> twitterConfigMonitor)
        {
            // StartMethod();


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
            // app.UseMvc(routes =>
            // {
            //     routes.MapRoute(
            //         name    : "default",
            //         template: "{controller=Home}/{action=Index}/{id?}");
            // });
        }
    }
}
