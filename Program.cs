using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using C = System.Console;
using E = System.Environment;


#pragma warning disable CS0414, CS0219, IDE0051, IDE0059, CS1591, IDE0044
namespace BaseballScraper
{
    public class Program
    {
        public static void Main (string[] args)
        {
            PrintEnvironmentInfo();
            CheckEnvironmentVersion();

            C.WriteLine($"\n***** ---> READY TO ROLL <--- *****");

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "False")
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                    logging.SetMinimumLevel(LogLevel.Warning)
                            .AddConsole()
                            .AddDebug()
                );



        private static void PrintEnvironmentInfo()
        {
            C.ForegroundColor = ConsoleColor.Blue;
            C.WriteLine($"\n[ CURRENT ENVIRONMENT INFO ]");
            C.WriteLine ($"Version                           : {E.Version}");
            C.WriteLine ($"System Directory                  : {E.SystemDirectory}");
            C.WriteLine ($"Current Directory                 : {E.CurrentDirectory}");
            C.WriteLine ($"Command Line                      : {E.CommandLine}");
            C.WriteLine ($"Current Managed Thread Id         : {E.CurrentManagedThreadId}");
            C.WriteLine ($"Exit Code                         : {E.ExitCode}");
            C.WriteLine ($"Machine Name                      : {E.MachineName}");
            C.WriteLine ($"OS Version                        : {E.OSVersion}");
            C.WriteLine ($"Processor Count                   : {E.ProcessorCount}");
            C.WriteLine ($"Stack Trace                       : {E.StackTrace}");
            C.WriteLine ($"System Page Size                  : {E.SystemPageSize}");
            C.WriteLine ($"Working Set                       : {E.WorkingSet}");
            C.WriteLine ($"User Domain Name                  : {E.UserDomainName}\n");
            C.ResetColor();
        }


        private static void CheckEnvironmentVersion()
        {
            if(string.Equals(Environment.Version.ToString(), "Production", StringComparison.Ordinal))
            {
                C.ForegroundColor = ConsoleColor.Red;
                C.WriteLine($"\n***************************************************");
                C.WriteLine("SWITCH TO DEV ENVIRONMENT");
                C.WriteLine($"***************************************************\n");
                C.ResetColor();
            }
        }
    }
}




            // C.ForegroundColor = ConsoleColor.Blue;
            // C.WriteLine($"\n***** ---> READY TO ROLL <--- *****");
            // C.WriteLine ($"Version                           : {Environment.Version}");
            // C.WriteLine ($"System Directory                  : {Environment.SystemDirectory}");
            // C.WriteLine ($"Current Directory                 : {Environment.CurrentDirectory}");
            // C.WriteLine ($"Command Line                      : {Environment.CommandLine}");
            // C.WriteLine ($"Current Managed Thread Id         : {Environment.CurrentManagedThreadId}");
            // C.WriteLine ($"Machine Name                      : {Environment.MachineName}");
            // C.WriteLine ($"OS Version                        : {Environment.OSVersion}");
            // C.WriteLine ($"Processor Count                   : {Environment.ProcessorCount}");
            // C.WriteLine ($"User Domain Name                  : {Environment.UserDomainName}\n");
            // C.ResetColor();

            // if(string.Equals(Environment.Version.ToString(), "Production", StringComparison.Ordinal))
            // {
            //     C.ForegroundColor = ConsoleColor.Red;
            //     C.WriteLine($"\n***************************************************");
            //     C.WriteLine("SWITCH TO DEV ENVIRONMENT");
            //     C.WriteLine($"***************************************************\n");
            //     C.ResetColor();
            // }