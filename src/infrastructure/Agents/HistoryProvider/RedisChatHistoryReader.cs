namespace infrastructure.Agents.HistoryProvider
{
    using System.Text.Json;

    using application.Services.Interfaces;

    using Microsoft.Extensions.AI;

    using StackExchange.Redis;

    internal class RedisChatHistoryReader(IConnectionMultiplexer redis) : IChatHistoryReader
    {
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task<IEnumerable<ChatMessage>> GetRecentMessagesAsync(
            string userId,
            string conversationId,
            int count,
            CancellationToken ct = default)
        {
            var key = $"conversation:{conversationId}:user:{userId}";
            string? raw = await _db.StringGetAsync(key);

            if (raw is null)
                return [];

            var all = JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(raw) ?? [];
            return all
                    .Where(m => (m.Role == ChatRole.User || m.Role == ChatRole.Assistant) && m.AdditionalProperties is null)
                .TakeLast(count);
        }
    }
}
