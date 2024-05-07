using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Payroll.Logic.Quartz.ListenerManagers
{
    public class SchedulerListener : ISchedulerListener
    {
        private readonly ILogger<SchedulerListener> _logger;
        public SchedulerListener(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<SchedulerListener>>();
        }

        public Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job added : {jobDetail.Key.Name}");
            return Task.CompletedTask;
        }

        public Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job deleted : {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job interruped : {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job paused : {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------JobResumed : {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job scheduled  : {trigger.JobKey.Name} with trigger {trigger.Key.Name}");
            return Task.CompletedTask;
        }

        public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Jobs paused on group {jobGroup}");
            return Task.CompletedTask;
        }

        public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job resumed on group  {jobGroup}");
            return Task.CompletedTask;
        }

        public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job unscheduled  :  {triggerKey.Name}");
            return Task.CompletedTask;
        }

        public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------SchedulerError with msg {msg}");
            return Task.CompletedTask;
        }

        public Task SchedulerInStandbyMode(CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------SchedulerInStandbyMode");
            return Task.CompletedTask;
        }

        public Task SchedulerShutdown(CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------SchedulerShutdown");
            return Task.CompletedTask;
        }

        public Task SchedulerShuttingdown(CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------SchedulerShuttingdown");
            return Task.CompletedTask;
        }

        public Task SchedulerStarted(CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------SchedulerStarted");
            return Task.CompletedTask;
        }

        public Task SchedulerStarting(CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------SchedulerStarting");
            return Task.CompletedTask;
        }

        public Task SchedulingDataCleared(CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------SchedulingDataCleared");
            return Task.CompletedTask;
        }

        public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------TriggerFinalized : {trigger.Key.Name}");
            return Task.CompletedTask;
        }

        public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------TriggerPaused : {triggerKey.Name}");
            return Task.CompletedTask;
        }

        public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------TriggerResumed : {triggerKey.Name}");
            return Task.CompletedTask;
        }

        public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------TriggersPaused on group : {triggerGroup}");
            return Task.CompletedTask;
        }

        public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------TriggersResumed on group : {triggerGroup}");
            return Task.CompletedTask;
        }
    }
}
