using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Payroll.Logic.Quartz.ListenerManagers
{
    public class TriggerListener : ITriggerListener
    {
        private readonly ILogger<TriggerListener> _logger;
        public TriggerListener(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<TriggerListener>>();
        }

        public string Name => "Test Trigger Listener";

        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Trigger completed : {trigger.Key.Name}");
            return Task.CompletedTask;
        }

        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Trigger fired : {trigger.Key.Name}");
            return Task.CompletedTask;
        }

        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Trigger misfired : {trigger.Key.Name}");
            return Task.CompletedTask;
        }

        public async Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(false);
        }
    }
}
