using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GeographicalPointsProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!System.IO.File.Exists(@"bin\Debug\netcoreapp2.1\nlog.config"))
            {
                System.IO.FileStream f = System.IO.File.Create(@"bin\Debug\netcoreapp2.1\nlog.config");
                f.Close();
            }
                

            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "NLog: catch setup errors");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true")
                .UseKestrel(c => c.AddServerHeader = false)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .CaptureStartupErrors(true)
                .UseNLog();
    }
}
