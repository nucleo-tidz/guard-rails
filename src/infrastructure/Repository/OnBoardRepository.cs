namespace infrastructure.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using application.Services.Interfaces;

    using model;

    using StackExchange.Redis;

    public class OnBoardRepository(IConnectionMultiplexer connectionMultiplexer) : IOnBoardRepository
    {
        public async Task<bool> IsOnboarded(string user)
        {
            var db = connectionMultiplexer.GetDatabase();
            return await db.KeyExistsAsync($"TokenLimiter:UserConfig:{user}");

        }

        public async Task OnBoardAsync(TokenLimitModel model)
        {
            var db = connectionMultiplexer.GetDatabase();
            string json = System.Text.Json.JsonSerializer.Serialize(model);

            await db.StringSetAsync($"TokenLimiter:UserConfig:{model.user}", json);
        }
    }
}
