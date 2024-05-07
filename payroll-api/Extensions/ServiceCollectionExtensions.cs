using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Payroll.Core.Kafka;
using payroll_api.Common.Settings;
using Quartz.AspNetCore;
using Quartz;
using StackExchange.Redis;
using System.Reflection;
using System.Text;
using Payroll.Logic.Quartz.Services.Interface;
using Payroll.Logic.Quartz.Services;
using System.Net;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.Extensions.Primitives;
using Payroll.SharedModel.Wrapper;
using payroll_api.HealthChecks;
using Payroll.Core.Constants;
using Payroll.Logic.Services.Auth;
using Payroll.Data.Repository.Auth;
using Amazon.Auth.AccessControlPolicy;
using payroll_api.Infracstructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace payroll_api.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddHealthCheck(this IServiceCollection services, AppSettings _appSettings, LoggerKafkaConfig loggerKafka)
        {
            _ = services.AddHealthChecks()
                    //.AddRedis(_appSettings.RedisSessionConfig)
                    .AddCheck("mongodb", new MongoDbHealthCheck(_appSettings.MongoDBSettings.MongoDbConnectionString ?? ""));

            return services;
        }

        internal static IServiceCollection AddConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddOptions();

            _ = services.Configure<KestrelServerOptions>(options =>
            {
                _ = options.Limits.MaxRequestBodySize = 100 * 1024 * 1024; //bytes => 100MB
                _ = options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
                _ = options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
            });

            _ = services.Configure<FormOptions>(x =>
            {
                _ = x.MultipartBodyLengthLimit = 100 * 1024 * 1024; //bytes => 100MB
            });

            //service set header security and setup in proxy nginx and apache to load  balancing
            _ = services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            return services;
        }

        internal static IServiceCollection AddRedisCache(this IServiceCollection services, AppSettings _appSettings)
        {
            //setup redis
            if (!string.IsNullOrEmpty(_appSettings.RedisSessionConfig))
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(_appSettings.RedisSessionConfig);

                if (redis.IsConnected)
                {
                    _ = services.AddDataProtection().PersistKeysToStackExchangeRedis(redis, "QH-SiteName-DataProtectionKeys-API");//share key with multiple web application instance
                }

                _ = services.AddStackExchangeRedisCache(options =>
                {
                    _ = options.Configuration = _appSettings.RedisSessionConfig;
                    _ = options.InstanceName = "PAYROLL-API-";
                });
            }
            else
            {
                _ = services.AddDistributedMemoryCache();
            }

            return services;
        }

        internal static IServiceCollection AddDatabase(this IServiceCollection services, AppSettings _appSettings)
        {
            // Create MongoDB client
            MongoClient mongoClient = new(_appSettings.MongoDBSettings.MongoDbConnectionString);

            // Get or create the MongoDB database instance
            IMongoDatabase database = mongoClient.GetDatabase(_appSettings.MongoDBSettings.DatabaseName);

            // Register MongoClient and IMongoDatabase instances in DI container
            services.AddSingleton(mongoClient);
            services.AddSingleton(database);

            return services;
        }

        internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AppSettings config)
        {
            byte[] key = Encoding.ASCII.GetBytes(config.Jwt.Key);
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ValidateIssuer = true,
                        ValidIssuer = config.Jwt.Issuer,

                        ValidateAudience = true,
                        ValidAudience = config.Jwt.Issuer,
                    };

                    bearer.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            StringValues accessToken = context.Request.Query["access_token"];

                            /// If the request is for our hub...
                            ///var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                c.Response.ContentType = "application/json";
                                string result = JsonConvert.SerializeObject(Result.Fail("The Token is expired.", (int)HttpStatusCode.Unauthorized));
                                return c.Response.WriteAsync(result);
                            }
                            else
                            {
#if DEBUG
                                c.NoResult();
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "text/plain";
                                return c.Response.WriteAsync(c.Exception.ToString());
#else
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail("An unhandled error has occurred."));
                                return c.Response.WriteAsync(result);
#endif
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                string result = JsonConvert.SerializeObject(Result.Fail("You are not Authorized.", (int)HttpStatusCode.Unauthorized));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            string result = JsonConvert.SerializeObject(Result.Fail("You are not authorized to access this resource.", (int)HttpStatusCode.Forbidden));
                            return context.Response.WriteAsync(result);
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                          .RequireAuthenticatedUser()
                          .Build();

                options.AddPolicy(AuthorizeConst.POLICY_INTERNAL, policy => policy.RequireRole(
                       AuthorizeConst.ROLE_ADMIN
                    ));
                options.AddPolicy(AuthorizeConst.POLICY_EXTERNAL, policy => policy.RequireRole(
                        AuthorizeConst.ROLE_ADMIN,
                        AuthorizeConst.ROLE_MANAGER,
                        AuthorizeConst.ROLE_USER
                    ));
            });

            return services;
        }

        internal static void RegisterSwagger(this IServiceCollection services)
        {
            _ = services.AddSwaggerGen(c =>
            {
                Uri url = new(uriString: "https://opensource.org/licenses/MIT");
                //Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".xml";
                string commentsFile = Path.Combine(baseDirectory, commentsFileName);
                if (File.Exists(commentsFile))
                {
                    c.IncludeXmlComments(commentsFile);
                }

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API PAYROLL v1",
                    Version = "v1"
                });
                c.SwaggerDoc("ex", new OpenApiInfo
                {
                    Title = "API PAYROLL FOR PARTNER",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                 });
            });
        }

        internal static IServiceCollection AddAutoMappers(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MapperProfile));
            return services;
        }

        internal static IServiceCollection AddQuartzs(this IServiceCollection services)
        {
            _ = services.AddQuartz(config =>
            {
                config.UseMicrosoftDependencyInjectionJobFactory();
                config.UseDefaultThreadPool(options =>
                {
                    options.MaxConcurrency = 20;
                });
                config.UseInMemoryStore();
                //_ = RunQuartzService.AddQuartzMachineName(config);

                #region Sync Partner Data - Đồng bộ dữ liệu các dịch vụ
                //_ = RunQuartzService.SyncPartnerFmSchedulerJob(config);
                #endregion
            });
            services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });
            services.AddTransient<IQuartzService, QuartzService>();

            return services;
        }

        internal static IServiceCollection AddLogicServices(this IServiceCollection services)
        {
            #region 
            services.AddScoped<IAuthService, AuthService>();
            #endregion

            return services;
        }

        internal static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            #region 
            services.AddTransient<IAuthRepository, AuthRepository>();
            #endregion

            return services;
        }
    }
}
