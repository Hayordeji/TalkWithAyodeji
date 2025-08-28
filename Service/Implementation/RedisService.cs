using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Service.Implementation
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;
        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<T?> GetData<T>(string key)
        {
            try
            {
                var data = await _cache.GetStringAsync(key);

                if (data is null)
                {
                    return default(T);
                }

                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task SetData<T>(string key, T value)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                var data = JsonConvert.SerializeObject(value);
                return _cache.SetStringAsync(key, data, options);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
