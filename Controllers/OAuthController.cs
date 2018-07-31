// https://github.com/JoeMayo/LinqToTwitter/wiki/Implementing-OAuth-for-ASP.NET-MVC

using System;
using System.Linq;
using System.Threading.Tasks;
using BaseballScraper.Models;
using LinqToTwitter;
using Microsoft.AspNetCore.Mvc;
using BaseballScraper.Services.Security;
using Microsoft.Extensions.Options;

namespace BaseballScraper.Controllers
{
    public class OAuthControllerX: Controller
    {
        private static String Start    = "STARTED";
        private static String Complete = "COMPLETED";
        private readonly AppIdentitySettings _identity;


        // public OAuthControllerX(IOptions<AppIdentitySettings> appSettingsAccessor)
        // {
        //     _identity = appSettingsAccessor.Value;
        // }

        public async Task BeginAsync()
        {
            Start.ThisMethod();
            //var auth = new MvcSignInAuthorizer

            var auth = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore()
                {
                    ConsumerKey    = _identity.TwConfig.ConsumerKey,
                    ConsumerSecret = _identity.TwConfig.ConsumerSecret
                }
            };

            Extensions.Spotlight("Mark 1");


            auth.Intro("auth");
            auth.Dig();

            await auth.AuthorizeAsync();

            Extensions.Spotlight("Mark 2");

            var twitterCtx = new TwitterContext(auth);

            var srch = 
                await
                (from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                    search.Query == "LINQ to Twitter"
                select search)
                .SingleOrDefaultAsync();

            Console.WriteLine("\nQuery: {0}\n", srch.SearchMetaData.Query);
            srch.Statuses.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.StatusID, entry.Source, entry.Text));

            Complete.ThisMethod();
        }



    }
}