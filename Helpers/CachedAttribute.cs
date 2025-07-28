//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using SupplyChain.IServiceContracts;
//using SupplyChain.IServiceContracts;
//using System.Text;

//namespace SupplyChain.Helpers
//{
//    public class CachedAttribute : Attribute, IAsyncActionFilter
//    {
//        private readonly int _timeToLiveSeconds;
//        public CachedAttribute(int timeToLiveSeconds)
//        {
//            _timeToLiveSeconds = timeToLiveSeconds;
//        }

//        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
//            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
//            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);
//            if (!string.IsNullOrEmpty(cachedResponse))
//            {
//                var contentResult = new ContentResult
//                {
//                    Content = cachedResponse,
//                    ContentType = "application/json",
//                    StatusCode = 200
//                };
//                context.Result = contentResult;
//                return;
//            }
//            var executedcontext = await next();
//            if (executedcontext.Result is OkObjectResult okObjectResult)
//            {
//                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
//            }

//        }

//        private string GenerateCacheKeyFromRequest(HttpRequest request)
//        {
//            var keyBuilder = new StringBuilder();
//            keyBuilder.Append($"{request.Path}");
//            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
//            {
//                keyBuilder.Append($"|{key}-{value}");
//            }
//            return keyBuilder.ToString();
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SupplyChain.IServiceContracts;
using System.Text;
using Microsoft.Extensions.Logging;

namespace SupplyChain.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CachedAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            var logger = context.HttpContext.RequestServices.GetService<ILogger<CachedAttribute>>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            string? cachedResponse = null;

            try
            {
                cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, $"[Cache] Redis unavailable while fetching cache for key: {cacheKey}. Continuing without cache.");
            }

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                context.Result = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                return;
            }

            var executedContext = await next();

            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                try
                {
                    await cacheService.CacheResponseAsync(
                        cacheKey,
                        okObjectResult.Value,
                        TimeSpan.FromSeconds(_timeToLiveSeconds)
                    );
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, $"[Cache] Redis unavailable while saving cache for key: {cacheKey}. Skipping cache write.");
                }
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}

