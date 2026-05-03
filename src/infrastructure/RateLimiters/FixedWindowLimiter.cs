namespace infrastructure.RateLimiters
{
    using System;
    using System.Threading.Tasks;

    using StackExchange.Redis;

    public class FixedWindowLimiter(IConnectionMultiplexer connectionMultiplexer) : ILimiter
    {
        private readonly TimeSpan window= TimeSpan.FromMinutes(5);
        private readonly int threshold = 2;
        public Task<bool> IsRequestAllowedAsync(string key)
        {
            string redisKey = $"RateLimit:FixedWindow:{key}";
            var db = connectionMultiplexer.GetDatabase();
            long count = db.StringIncrement(redisKey);
            if(count == 1)
            {
                db.KeyExpire(redisKey, window);
            }
            else if(count > threshold)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
}
