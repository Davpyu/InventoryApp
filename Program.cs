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

        public static IHostBuilder CreateHostBuilder(string[] args) {
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
                    });
                    webBuilder.UseStartup<Startup>();
                 });
            
            return hostBuilder;
        }
    }
}
