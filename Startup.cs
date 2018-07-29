using BaseballScraper.Client;
using BaseballScraper.Configuration;
using BaseballScraper.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// using BaseballScraper.Client;
// using BaseballScraper.Configuration;
// using BaseballScraper.Infrastructure;

namespace BaseballScraper
{
    public class Startup
    {
        private readonly IYahooAuthClient _client;
        private readonly IYahooFantasyClient _fantasy;


        public Startup (IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
        {
            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1);

            services.Configure<YahooConfiguration> ((IConfiguration) this.Configuration.GetSection ("YahooConfiguration"));

            //Add Services for BaseballScraper Package
            services.AddSingleton<IRequestFactory, RequestFactory> ();
            services.AddTransient<IYahooAuthClient, YahooAuthClient> ();
            services.AddSingleton<IYahooFantasyClient, YahooFantasyClient> ();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env)
        {
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
