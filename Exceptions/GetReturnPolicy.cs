//using Polly.Extensions.Http;
//using Polly;

//namespace StocksApi.Exceptions
//{
//    public class GetReturnPolicy 
//    {
//        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
//        {
//            return HttpPolicyExtensions.HandleTransientHttpError()
//                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
//                (result, timeSpan, retryCount, context) =>
//                {
//                    Console.WriteLine($"Request failed. Retrying attempt {retryCount} in {timeSpan.TotalSeconds} seconds...");
//                });
//        }

//        private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
//        {
//            return Policy.TimeoutAsync<HttpResponseMessage>(60); // Set timeout per request
//        }

//    }
//}
