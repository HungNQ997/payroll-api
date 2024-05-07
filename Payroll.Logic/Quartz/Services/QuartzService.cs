using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Payroll.Logic.Quartz.JobModel;
using Payroll.Logic.Quartz.Services.Interface;
using Payroll.Logic.Quartz.Type;
using Quartz;

namespace Payroll.Logic.Quartz.Services
{
    public class QuartzService : IQuartzService
    {
        private readonly ILogger<QuartzService> _logger;
        private readonly IScheduler _scheduler;

        public QuartzService(ILogger<QuartzService> logger, ISchedulerFactory schedulerFactory)
        {
            _logger = logger;
            _scheduler = schedulerFactory.GetScheduler().Result;
        }

        public async Task CreateOrUpdateFor<T>(JobCreateOrUpdateRequest request) where T : IJob
        {
            _logger.LogInformation($"{nameof(QuartzService)} -- {nameof(CreateOrUpdateFor)} -- request JobCreateOrUpdateRequest: {JsonConvert.SerializeObject(request)}");
            request.Name = request.Name.Trim();
            request.ContractChild = request.ContractChild.Trim();
            request.Group = request.Group.Trim();

            JobKey key = new JobKey(request.Name, request.Group);
            if (await _scheduler.CheckExists(key))
            {
                _ = !request.IsUpdate ? throw new Exception("CreateOrUpdateFor()") : await _scheduler.DeleteJob(key);
            }

            JobDataMap dataMap = new JobDataMap();
            dataMap.Put("Id", request.Id);
            dataMap.Put("Name", request.Name);
            dataMap.Put("ContractChild", request.ContractChild);
            dataMap.Put("Group", request.Group);
            dataMap.Put("DateTime", request.StartTime);

            IJobDetail job = JobBuilder.Create<T>()
                .StoreDurably(false)
                .RequestRecovery()
                .WithDescription(request.Description ?? string.Empty)
                .WithIdentity(request.Name, request.Group)
                .UsingJobData(dataMap)
                .Build();

            TriggerBuilder builder = TriggerBuilder.Create()
                .WithIdentity(request.Name, request.Group)
                .StartNow()
                .ForJob(job);
            if (request.TriggerType == (int)TriggerType.Simple)
            {
                _ = builder.WithSimpleSchedule(simple =>
                {
                    if (request.IntervalType == (int)IntervalType.Second)
                    {
                        _ = simple.WithIntervalInSeconds(request.Interval);
                    }
                    if (request.IntervalType == (int)IntervalType.Minute)
                    {
                        _ = simple.WithIntervalInMinutes(request.Interval);
                    }
                    if (request.IntervalType == (int)IntervalType.Hour)
                    {
                        _ = simple.WithIntervalInHours(request.Interval);
                    }
                    if (request.IntervalType == (int)IntervalType.Day)
                    {
                        _ = simple.WithIntervalInHours(request.Interval * 24);
                    }

                    if (request.RepeatCount > -1)
                    {
                        _ = simple.WithRepeatCount(request.RepeatCount);
                    }
                    else
                    {
                        _ = simple.RepeatForever();
                    }
                    _ = simple.WithMisfireHandlingInstructionFireNow();
                });
            }
            else
            {
                _ = builder.WithCronSchedule(request.Cron, cron => _ = cron.WithMisfireHandlingInstructionFireAndProceed());
            }

            ITrigger trigger = builder.Build();

            _ = await _scheduler.ScheduleJob(job, trigger);
        }

        public async Task ReCreateOrUpdateFor<T>(JobCreateOrUpdateRequest request) where T : IJob
        {
            _logger.LogInformation($"{nameof(QuartzService)} -- {nameof(CreateOrUpdateFor)} -- request JobCreateOrUpdateRequest: {JsonConvert.SerializeObject(request)}");
            request.Name = request.Name.Trim();
            request.Group = request.Group.Trim();

            JobKey key = new JobKey(request.Name, request.Group);
            if (await _scheduler.CheckExists(key))
            {
                _ = !request.IsUpdate ? throw new Exception("CreateOrUpdateFor()") : await _scheduler.DeleteJob(key);
            }

            JobDataMap dataMap = new JobDataMap();
            dataMap.Put("Id", request.Id);
            dataMap.Put("Name", request.Name);
            dataMap.Put("Group", request.Group);
            dataMap.Put("DateTime", request.StartTime);

            IJobDetail job = JobBuilder.Create<T>()
                .StoreDurably(false)
                .RequestRecovery()
                .WithDescription(request.Description ?? string.Empty)
                .WithIdentity(request.Name, request.Group)
                .UsingJobData(dataMap)
                .Build();

            TriggerBuilder builder = TriggerBuilder.Create()
                .WithIdentity(request.Name, request.Group)
                .StartAt(request.StartTime.Value)
                .ForJob(job);
            if (request.TriggerType == (int)TriggerType.Simple)
            {
                _ = builder.WithSimpleSchedule(simple =>
                {
                    if (request.IntervalType == (int)IntervalType.Second)
                    {
                        _ = simple.WithIntervalInSeconds(request.Interval);
                    }
                    if (request.IntervalType == (int)IntervalType.Minute)
                    {
                        _ = simple.WithIntervalInMinutes(request.Interval);
                    }
                    if (request.IntervalType == (int)IntervalType.Hour)
                    {
                        _ = simple.WithIntervalInHours(request.Interval);
                    }
                    if (request.IntervalType == (int)IntervalType.Day)
                    {
                        _ = simple.WithIntervalInHours(request.Interval * 24);
                    }

                    if (request.RepeatCount > -1)
                    {
                        _ = simple.WithRepeatCount(request.RepeatCount);
                    }
                    else
                    {
                        _ = simple.RepeatForever();
                    }
                    _ = simple.WithMisfireHandlingInstructionFireNow();
                });
            }
            else
            {
                _ = builder.WithCronSchedule(request.Cron, cron => _ = cron.WithMisfireHandlingInstructionFireAndProceed());
            }

            ITrigger trigger = builder.Build();

            _ = await _scheduler.ScheduleJob(job, trigger);
        }

        public async Task<bool> Delete(string name, string group)
        {
            return await _scheduler.DeleteJob(new JobKey(name, group));
        }

        public async Task Pause(string name, string group)
        {
            await _scheduler.PauseJob(new JobKey(name, group));
        }

        public async Task Resume(string name, string group)
        {
            await _scheduler.ResumeJob(new JobKey(name, group));
        }

        public async Task Trigger(string name, string group)
        {
            await _scheduler.TriggerJob(new JobKey(name, group));
        }

        public async Task<List<string>> Log(string name, string group)
        {
            JobKey key = new JobKey(name.Trim(), group.Trim());
            IJobDetail job = await _scheduler.GetJobDetail(key);
            return job.JobDataMap.GetLogList();
        }
    }
}
