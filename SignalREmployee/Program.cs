using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SignalREmployee
{
    public class Program
    {        
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Web host builder creation to start up the application
        /// </summary>
        /// <param name="args"> The host arguments</param>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        /// <summary>
        /// Build web host for application 
        /// </summary>
        /// <param name="args"> The host arguments</param>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((webHostBuilderContext, configurationbuilder) =>
                {
                    var environment = webHostBuilderContext.HostingEnvironment;

                    configurationbuilder
                            .AddJsonFile("appsettings.json", optional: true);

                    configurationbuilder.AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .Build();
    }
}
