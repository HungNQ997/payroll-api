using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Payroll.Logic.Quartz.ListenerManagers
{
    public class JobListener : IJobListener
    {
        private readonly ILogger<JobListener> _logger;
        public JobListener(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<JobListener>>();
        }
        public string Name => "Test job listener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job vetoed : {context.JobDetail.Key.Name}");
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job is to be executed : {context.JobDetail.Key.Name}");
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning($"---------QRTZ------------Job executed : {context.JobDetail.Key.Name}");
            return Task.CompletedTask;
        }
    }
}
