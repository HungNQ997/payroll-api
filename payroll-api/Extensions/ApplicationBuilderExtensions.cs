using CrystalQuartz.Application;
using CrystalQuartz.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using payroll.api;
using Payroll.Logic.Quartz.ListenerManagers;
using Payroll.SharedModel.Responses.HealthChecks;
using Quartz;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace payroll_api.Extensions
{
    internal static class ApplicationBuilderExtensions
    {
        internal static IApplicationBuilder UseHeathChecks(this IApplicationBuilder app)
        {
            return app.UseHealthChecks("/healthchecks", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    HealthCheckResponse response = new()
                    {
                        Status = report.Status.ToString(),
                        HealthChecks = report.Entries.Select(x => new IndividualHealthCheckResponse
                        {
                            Components = x.Key,
                            Status = x.Value.Status.ToString(),
                            Description = x.Value.Description
                        }),
                        HealthCheckDuration = report.TotalDuration
                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                },
            });
        }

        internal static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
        {
            return app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllerRoute(
                   name: "MyPartnerMiddlewareArea",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                _ = endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                _ = endpoints.MapFallbackToFile("index.html");
            });
        }

        internal static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Local"))
            {
                _ = app.UseDeveloperExceptionPage();
            }

            return app;
        }

        internal static IApplicationBuilder UseCrystalQuartzs(
          this IApplicationBuilder app)
        {
            const string pathQuartz = "/quartz";
            _ = app.Use(async (context, next) =>
            {
                string path = context.Request.Path;
                if (!path.Contains(pathQuartz))
                {
                    await next.Invoke();
                }
                else
                {
                    string access_token = context.Request.Query["access_token"];
                    string pathFile = context.Request.Query["path"];
                    // Kiểm tra xem access_token có giá trị không
                    if (!string.IsNullOrEmpty(access_token))
                    {
                        // Kiểm tra xem access_token có đúng định dạng JWT
                        if (IsValidJwt(access_token))
                        {
                            // Xác thực access_token thành công, tiếp tục thực hiện các logic trong CrystalQuartz
                            await next.Invoke();
                        }
                        else
                        {
                            // Nếu access_token không đúng định dạng JWT, trả về lỗi Unauthorized
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            await context.Response.WriteAsync("Unauthorized");
                            return;
                        }
                    }
                    else if (!string.IsNullOrEmpty(pathFile))
                    {
                        // Nếu không có access_token nhưng có pathFile, tiếp tục thực hiện các logic trong CrystalQuartz
                        await next.Invoke();
                    }
                    else
                    {
                        // Nếu không có access_token và không có pathFile, trả về lỗi Unauthorized
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                }
            });

            app.UseCrystalQuartz(() => CreateScheduler(app.ApplicationServices.GetRequiredService<ISchedulerFactory>(), app.ApplicationServices), new CrystalQuartzOptions() { Path = pathQuartz }, null);

            static IScheduler CreateScheduler(ISchedulerFactory schedulerFactory, IServiceProvider serviceDescriptors)
            {
                IScheduler scheduler = schedulerFactory.GetScheduler().Result;

                scheduler.ListenerManager.AddTriggerListener(new TriggerListener(serviceDescriptors));
                scheduler.ListenerManager.AddJobListener(new JobListener(serviceDescriptors));
                scheduler.ListenerManager.AddSchedulerListener(new SchedulerListener(serviceDescriptors));
                _ = scheduler.Start();

                return scheduler;
            }
            return app;
        }

        internal static void ConfigureSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            _ = app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
                if (env.IsDevelopment() || env.IsStaging())
                {
                    c.PreSerializeFilters.Add((swagger, httpRq) =>
                    {
                        //Setup filter authorization user or session or anything, etc...
                        string prefixSv = string.Empty;
                        if (env.IsDevelopment())
                        {
                            prefixSv = "dev";
                        }

                        if (env.IsStaging())
                        {
                            prefixSv = "stag";
                        }

                        //if (env.IsDevelopment() || env.IsStaging())
                        //{
                        //    swagger.Servers = new List<OpenApiServer>()
                        //    {
                        //        new OpenApiServer()
                        //        {
                        //            Url=$"{httpRq.Scheme}://ftmsapi-{prefixSv}.fpt.net/ll"
                        //        }
                        //    };
                        //}
                    });
                }
            });
            _ = app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name + " API PAYROLL");
                options.SwaggerEndpoint("/swagger/ex/swagger.json", typeof(Program).Assembly.GetName().Name + " API PAYROLL FOR PARTNER");

                options.DisplayRequestDuration();
                options.EnableFilter();
                options.ShowExtensions();
                options.ShowCommonExtensions();
                options.EnableDeepLinking();
            });
        }


        // Phương thức kiểm tra xem access_token có đúng định dạng JWT hay không
        private static bool IsValidJwt(string token)
        {
            try
            {
                // Thực hiện việc parse JWT để kiểm tra tính hợp lệ
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                // Nếu không có lỗi khi parse JWT, coi token là hợp lệ
                return true;
            }
            catch (Exception)
            {
                // Nếu có lỗi khi parse JWT, coi token không hợp lệ
                return false;
            }
        }
    }
}
