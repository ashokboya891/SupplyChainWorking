using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Interfaces;
using StackExchange.Redis;
using SupplyChain.IServiceContracts;

namespace SupplyChain.Services
{
    public class ReponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;
        public ReponseCacheService(IConnectionMultiplexer redis)
        {
            _database=redis.GetDatabase();

        }
        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if(response==null)
            {
                return;
            }
            var options=new JsonSerializerOptions
            {
                PropertyNamingPolicy=JsonNamingPolicy.CamelCase
            };
            var serialisedResponse=JsonSerializer.Serialize(response,options);
            await _database.StringSetAsync(cacheKey,serialisedResponse,timeToLive);
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var chachedResponse=await _database.StringGetAsync(cacheKey);
            if(chachedResponse.IsNullOrEmpty)
            {
                return null;
            }
            return chachedResponse;
        }
    }
}