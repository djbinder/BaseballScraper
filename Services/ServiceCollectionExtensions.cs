// // https://stackoverflow.com/questions/47294020/reading-appsettings-from-asp-net-core-webapi

// // using BaseballScraper.Services.Security.Entities;
// using System;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;

// namespace BaseballScraper.Services.Security.Extensions
// {
//     public static class ServiceCollectionExtensions
//     {
//         private static String Start    = "STARTED";
//         private static String Complete = "COMPLETED";
//         public static void AddIdentitySecurityService(this IServiceCollection services,
//             IConfiguration configuration)
//         {
//             Start.ThisMethod();

//             var settingsSection = configuration.GetSection("AppIdentitySettings");
//             var settings        = settingsSection.Get<AppIdentitySettings>();

//             // Inject AppIdentitySettings so that others can use too
//             services.Configure<AppIdentitySettings>(settingsSection);

//             Complete.ThisMethod();
//         }
//     }
// }