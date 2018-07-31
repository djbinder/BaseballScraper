using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YahooFantasyWrapper;
using YahooFantasyWrapper.Client;
using YahooFantasyWrapper.Configuration;


using BaseballScraper.Services.Security;

namespace BaseballScraper
{
    public class Startup
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        public IHostingEnvironment HostingEnvironment { get; private set; }
        public IConfiguration Configuration { get; private set; }


        public Startup (IConfiguration configuration, IHostingEnvironment env)
        {
            Start.ThisMethod();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            builder.AddEnvironmentVariables();

            builder.Intro("builder");

            this.HostingEnvironment = env;
            this.Configuration      = configuration;

            var AppIdentitySettings = Configuration["AppIdentitySettings"];
            // AppIdentitySettings.Intro("app identity settings");



        }

        // public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            Start.ThisMethod();

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);
            services.Configure<YahooFantasyWrapper.Configuration.YahooConfiguration>(Configuration.GetSection("YahooConfiguration"));
            // services.AddIdentitySecurityService(this.Configuration);
            var settingsSection = Configuration.GetSection("AppIdentitySettings");
            var settings        = settingsSection.Get<AppIdentitySettings>();

            // Inject AppIdentitySettings so that others can use too
            services.Configure<AppIdentitySettings>(settingsSection);



            Complete.ThisMethod();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env)
        {
            Start.ThisMethod();
            if (env.IsDevelopment ())
            {
                app.UseDeveloperExceptionPage ();
            }
            else
            {
                app.UseHsts ();
            }

            app.UseHttpsRedirection ();
            app.UseMvc ();
        }
    }
}
