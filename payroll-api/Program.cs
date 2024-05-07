using Payroll.Core.Kafka;
using payroll_api;
using Serilog;
using Serilog.Events;

namespace payroll.api
{
    public class Program
    {
        private static readonly Action<IConfigurationBuilder> BuildConfiguration =
         builder => builder
       .SetBasePath(System.IO.Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
       .AddEnvironmentVariables();

        public static async Task Main(string[] args)
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
            string file = "appsettings.json";
            if (!string.IsNullOrEmpty(env) &&
                !env.Equals("Production"))
            {
                file = $"appsettings.{env}.json";
            }
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(file, optional: true).Build();

            LoggerKafkaConfig _configLogger = new();
            configuration.GetSection("Serilog").Bind(_configLogger);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("QH", LogEventLevel.Information)
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                //.WriteTo.Kafka(
                //    batchSizeLimit: _configLogger.BatchSizeLimit,
                //    period: _configLogger.Period,
                //    bootstrapServers: _configLogger.Brokers,
                //    topic: _configLogger.Topic
                //).WriteTo.Console(
                //    theme: AnsiConsoleTheme.Code,
                //    outputTemplate: "[{Timestamp:dd-MM-yyyy HH:mm:ss.fff}] [{EventType:x8} {Level:u3}] [{SourceContext}] [ ThreadId:{ThreadId}] {NewLine} {Message:lj} {NewLine} {Exception} {NewLine}"
                // )
                .CreateLogger();

            try
            {
                IHost host = CreateHostBuilder(args).Build();
                Log.Information("Starting ASP net core 5.0 web host");
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }



        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            BuildConfiguration(builder);
            IConfiguration Configuration = builder.Build();
            string domain = Configuration["Host:Domain"]?.ToString() ?? "";

            return Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                _ = webBuilder.UseStartup<Startup>();

                if (!string.IsNullOrEmpty(domain))
                {
                    _ = webBuilder.UseUrls(domain);
                }
            });
        }
    }
}
