namespace Payroll.Core.Infras
{
    public interface ICacheManager
    {
        ValueTask<TOut> GetOrCreateAsync<TOut>(string key, double expiredMinutes, Func<ValueTask<TOut>> func) where TOut : class;
        ValueTask ClearCache(string key, CancellationToken cancellationToken);
        ValueTask ClearAllCache(string[] keys);
        Task SetCache(string key, string value, double expiredMinutes = 1);
        Task<string> GetCache(string key, CancellationToken cancellationToken);
    }
}
