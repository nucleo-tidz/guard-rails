namespace infrastructure.Agents.HistoryProvider
{
    using System.Text.Json;

    using application.Services.Interfaces;

    using Microsoft.Extensions.AI;
    using model;
    using StackExchange.Redis;

    internal class RedisChatHistoryReader(IConnectionMultiplexer redis, ISharedContext sharedContext) : IChatHistoryReader
    {
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task<IEnumerable<ChatMessage>> GetRecentMessagesAsync(int count, CancellationToken ct = default)
        {
            var key = sharedContext.StateKey;
            string? raw = await _db.StringGetAsync(key);

            if (raw is null)
                return [];

            var all = JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(raw) ?? [];
            return all
                    .Where(m => (m.Role == ChatRole.User || m.Role == ChatRole.Assistant) && !m.Text.StartsWith("##")  &&!string.IsNullOrEmpty( m.Text))
                .TakeLast(count);
        }
    }
}
