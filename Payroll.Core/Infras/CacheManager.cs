using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Payroll.Core.Infras
{
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache _caching;
        private readonly ILogger<CacheManager> _logger;
        public CacheManager(IDistributedCache caching, ILogger<CacheManager> logger)
        {
            _caching = caching;
            _logger = logger;
        }

        public async ValueTask ClearAllCache(string[] keys)
        {
            try
            {
                foreach (string item in keys)
                {
                    await _caching.RemoveAsync(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CacheManager.ClearAllCache: MESSAGE={ex.Message}, STACKTRACE={ex.StackTrace}");
            }
        }

        public async ValueTask ClearCache(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await _caching.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CacheManager.ClearCache: MESSAGE={ex.Message}, STACKTRACE={ex.StackTrace}");
            }
        }
        /// <summary>
        /// Set value cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredMinutes">0 nếu ko set expired </param>
        public async Task SetCache(string key, string value, double expiredMinutes = 1)
        {
            try
            {
                DateTime endingCache = DateTime.Now.AddMinutes(expiredMinutes);
                DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
                if (expiredMinutes > 1)
                {
                    options.AbsoluteExpiration = endingCache;
                    await _caching.SetStringAsync(key, value, options);
                }
                else
                {
                    await _caching.SetStringAsync(key, value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CacheManager.SetCache: MESSAGE={ex.Message}, STACKTRACE={ex.StackTrace}");
            }
        }
        public async Task<string> GetCache(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return await _caching.GetStringAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CacheManager.GetCache: MESSAGE={ex.Message}, STACKTRACE={ex.StackTrace}");
                return string.Empty;
            }
        }

        public async ValueTask<TOut> GetOrCreateAsync<TOut>(string key, double expiredMinutes, Func<ValueTask<TOut>> func) where TOut : class
        {
            try
            {
                TOut result;
                string dataString = await _caching.GetStringAsync(key);
                if (!string.IsNullOrEmpty(dataString))
                {
                    result = JsonConvert.DeserializeObject<TOut>(dataString);
                    return result;
                }
                //set value
                TOut dataInput = await func.Invoke();
                string convertString = JsonConvert.SerializeObject(dataInput);
                if (JToken.Parse(convertString).JsonIsNullOrEmpty())
                {
                    return dataInput;
                }

                DateTime endingCache = DateTime.Now.AddMinutes(expiredMinutes);
                await _caching.SetStringAsync(key, convertString, new DistributedCacheEntryOptions() { AbsoluteExpiration = endingCache });

                return dataInput;
            }
            catch (Exception ex)
            {
                _logger.LogError($"CacheManager.GetOrCreateAsync: MESSAGE={ex.Message}, STACKTRACE={ex.StackTrace}");
                return null;
            }
        }
    }
    public static class JsonExts
    {
        public static bool JsonIsNullOrEmpty(this JToken value)
        {
            return value == null ||
                (value.Type == JTokenType.Array && !value.HasValues) ||
                (value.Type == JTokenType.Object && !value.HasValues) ||
                (value.Type == JTokenType.String && (string.IsNullOrEmpty(value.ToString()) || string.IsNullOrWhiteSpace(value.ToString()))) ||
                value.Type == JTokenType.Null;
        }
    }
}
