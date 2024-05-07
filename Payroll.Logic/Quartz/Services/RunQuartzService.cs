using Payroll.Logic.Quartz.MachineName;
using Quartz;

namespace Payroll.Logic.Quartz.Services
{
    public static class RunQuartzService
    {
        public static Task AddQuartzMachineName(IServiceCollectionQuartzConfigurator q)
        {
            JobKey jobKey = new("AddQuartzMachineName", "AddQuartzMachineName");
            _ = q.AddJob<AddQuartzMachineName>(jobKey, j => j
                  .WithDescription("Tạo key để tránh bị dupplicate job")
            );
            _ = q.AddTrigger(t => t
                  .WithIdentity("AddQuartzMachineName")
                  .ForJob(jobKey)
                  .WithCronSchedule("0 0 0 * * ?")
                  .StartNow()
                  .WithDescription("Tạo key để tránh bị dupplicate trigger")
            );
            return Task.CompletedTask;
        }

        //public static Task SyncPartnerFmSchedulerJob(IServiceCollectionQuartzConfigurator q)
        //{
        //    JobKey job = new("SyncPartnerFmSchedulerJob", "SyncPartnerFmSchedulerJob");
        //    _ = q.AddJob<SyncPartnerFmSchedulerJob>(job, j => j
        //          .WithDescription("Đồng bộ dữ liệu dịch vụ của FM và tính giá trị Alpha")
        //    );
        //    _ = q.AddTrigger(t => t
        //          .WithIdentity(JobGroupKey.SyncPartnerFmSchedulerJob)
        //          .ForJob(job)
        //          .WithCronSchedule("0 5 0 * * ?") // Fire job -> Lúc 00h5 AM mỗi ngày                          
        //          .StartNow()
        //          .WithDescription("Đồng bộ dữ liệu dịch vụ của FM và tính giá trị Alpha")
        //    );
        //    return Task.CompletedTask;
        //}
    }
}
