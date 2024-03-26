using DotNetService.Exceptions;
using Microsoft.AspNetCore.Server.IIS;

namespace DotNetService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
            .Build()
            .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddCommandLine(args)
                .Build();

            double sentryTraceSampleRate = double.Parse(config["Sentry:TracesSampleRate"] ?? "1");
            //get dsn value
            string dsn = config["Sentry:Dsn"] ?? "";
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry(o =>
                    {
                        o.Dsn = dsn;
                        o.TracesSampleRate = sentryTraceSampleRate;
                        
                        o.AddExceptionFilterForType<Microsoft.AspNetCore.Http.BadHttpRequestException>();
                        o.AddExceptionFilterForType<UnauthenticatedException>();
                        o.AddExceptionFilterForType<ValidationException>();
                        o.AddExceptionFilterForType<DataNotFoundException>();
                        o.AddExceptionFilterForType<UnprocessableEntityException>();
                    });
                    webBuilder.UseStartup<Startup>();
                });

            return hostBuilder;
        }
    }
}
