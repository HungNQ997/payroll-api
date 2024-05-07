using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Payroll.Core.Constants;
using Payroll.Core.Infras;
using Quartz;

namespace Payroll.Logic.Quartz.MachineName
{
    [PersistJobDataAfterExecution]  // Sau khi thực thi, hãy cập nhật JobData
    [DisallowConcurrentExecution]   // không cho chạy đồng thời
    public sealed class AddQuartzMachineName : IDisposable, IJob
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<AddQuartzMachineName> _logger;

        public AddQuartzMachineName(IServiceProvider provider, ILogger<AddQuartzMachineName> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //Create cancellationToken
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            // Create a new scope
            using IServiceScope scope = _provider.CreateScope();
            ICacheManager cacheManager = scope.ServiceProvider.GetRequiredService<ICacheManager>();
            await cacheManager.ClearCache(CacheKeyConst.Quartz_MachineName, cancellationToken);
            await cacheManager.SetCache(CacheKeyConst.Quartz_MachineName, Environment.MachineName, 0);
            _logger.LogWarning("---------------Quartz-MachineName-------------" + Environment.MachineName);
        }

        public void Dispose()
        {
            _logger.LogWarning($"--------------Dispose job Quartz-MachineName {nameof(AddQuartzMachineName)}------------");
        }
    }
}
