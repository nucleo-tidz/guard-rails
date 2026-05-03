namespace infrastructure.Agents.TokenManager
{
    using System.Text.Json;
    using System.Threading.Tasks;

    using model;

    using StackExchange.Redis;

    internal class TokenLimiter(ISharedContext sharedContext, IConnectionMultiplexer connectionMultiplexer) : ITokenLimiter
    {
        public async Task ConsumeAsync(long tokens)
        {
            var db = connectionMultiplexer.GetDatabase();
            var config = await db.StringGetAsync(GetUserConfigKey());
            TokenLimitModel tokenLimitModel = JsonSerializer.Deserialize<TokenLimitModel>(config.ToString());
            string redisKey = GetLimitKey();
            long current = await db.StringIncrementAsync(redisKey, tokens);
            if (current == tokens)
            {
                await db.KeyExpireAsync(redisKey, tokenLimitModel.window);
            }
        }
        public async Task<bool> IsAllowed()
        {
            var db = connectionMultiplexer.GetDatabase();

            var config = await db.StringGetAsync(GetUserConfigKey());
            if (config.IsNullOrEmpty)
            {
                return true;
            }
            var consumedraw = await db.StringGetAsync(GetLimitKey());
            long consumed = consumedraw.HasValue ? (long)consumedraw : 0;
            TokenLimitModel tokenLimitModel = JsonSerializer.Deserialize<TokenLimitModel>(config.ToString());
            if (consumed > tokenLimitModel.limit)
                return false;
            else
                return true;
        }

        private string GetUserConfigKey() => $"TokenLimiter:UserConfig:{sharedContext.User}";
        private string GetLimitKey() => $"TokenLimiter:FixedWindow:{sharedContext.User}";
    }
}
