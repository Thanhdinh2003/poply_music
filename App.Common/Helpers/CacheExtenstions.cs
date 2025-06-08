using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace App.Common.Helpers
{
    public static class CacheExtenstions
    {
        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key)
        {
            byte[] cacheData = await distributedCache.GetAsync(key);
            if (cacheData != null)
            {
                return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(cacheData))
                    ?? throw new ArgumentException();
            }

            return default(T);
        }

        public static async Task SetAsync<T>(this IDistributedCache distributedCache, string key, T value, int timeDurationInMinutes = 0)
        {
            byte[] byteValue = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
            await distributedCache.SetAsync(key, byteValue, new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(timeDurationInMinutes)));
        }

        public static async Task RemoveAsync(this IDistributedCache distributedCache, string key)
        {
            await distributedCache.RemoveAsync(key);
        }
    }
}
