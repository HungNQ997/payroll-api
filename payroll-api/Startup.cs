using Payroll.Core.Kafka;
using Payroll.Core.Logger;
using Payroll.Core.Settings;
using payroll_api.Common.Settings;
using payroll_api.Extensions;
using payroll_api.Helpers.Jwt;
using Serilog;

namespace payroll_api
{
    public class Startup
    {
        private const string _APP_SETTINGS_SECTION_KEY = "AppSettings";
        private readonly AppSettings _appSettings;
        private readonly LoggerKafkaConfig _configLoggerKafka;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _appSettings = new AppSettings();
            Configuration.GetSection(_APP_SETTINGS_SECTION_KEY).Bind(_appSettings);
            _configLoggerKafka = new LoggerKafkaConfig();
            Configuration.GetSection("Serilog").Bind(_configLoggerKafka);
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabase(_appSettings);
            services.AddHealthCheck(_appSettings, _configLoggerKafka);
            services.AddConfigureOptions(Configuration);

            #region --- Redis ---
            services.AddRedisCache(_appSettings);
            #endregion

            #region --- Implement Setting Instances ---
            services.AddSingleton(typeof(AppSettings), _appSettings);
            services.AddSingleton(typeof(MongoDBSettings), _appSettings.MongoDBSettings);
            services.AddSingleton(typeof(LoggerKafkaConfig), _configLoggerKafka);
            services.AddSingleton(typeof(IJwtHelper), typeof(JwtHelper));
            #endregion

            services.AddControllers();

            services.AddLogicServices();

            services.AddRepositories();

            services.AddAutoMappers();

            services.AddJwtAuthentication(_appSettings);

            services.RegisterSwagger();

            #region Quartz
            services.AddQuartzs();
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandling(_env);
            //app.UseAntiXssMiddleware();
            //app.ErrorHandlerMiddleware();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHeathChecks();
            app.UseCrystalQuartzs();
            app.ConfigureSwagger(_env);

            loggerFactory.AddSerilog();
            LogHelper.Logger = loggerFactory.CreateLogger("PAYROLL-API-LOGS");

            app.UseEndpoints();
        }
    }
}
