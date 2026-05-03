namespace infrastructure.RateLimiters
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using StackExchange.Redis;

    internal class SlidingWindowLimiter(IConnectionMultiplexer connectionMultiplexer) : ILimiter
    {

        private readonly TimeSpan window = TimeSpan.FromMinutes(5);
        private readonly int threshold = 2;

        public Task<bool> IsRequestAllowedAsync(string key)
        {
            string redisKey = $"RateLimit:SlidingWindow:{key}";
            var db = connectionMultiplexer.GetDatabase();

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var windowStart = now - (long)window.TotalMilliseconds;

            db.SortedSetRemoveRangeByScore(redisKey, 0, windowStart);
            long count = db.SortedSetLength(redisKey);

            if (count >= threshold)
            {
                return Task.FromResult(false);
            }
            db.SortedSetAdd(redisKey, now, now);
            db.KeyExpire(redisKey, window.Add(TimeSpan.FromSeconds(1)));
            return Task.FromResult(true);
        }
    }
}
