namespace infrastructure.Agents.HistoryProvider
{
    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using StackExchange.Redis;
    using System;
    using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

    public sealed class RedisChatHistoryProvider : ChatHistoryProvider
    {

        private readonly IDatabase _db;
        private readonly SummarizingChatReducer? _summarizingChatReducer;

        public RedisChatHistoryProvider(
            IConnectionMultiplexer redis,
            SummarizingChatReducer? summarizingChatReducer = null)
        {
            _db = redis.GetDatabase();
            _summarizingChatReducer = summarizingChatReducer;
        }

        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(InvokingContext context, CancellationToken cancellationToken = default)
        {

            string messages = _db.StringGet(GetKey(context.Session));
            if (messages == null)
                return new(new List<ChatMessage>());
            return new((System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(messages)));
        }

        protected override async ValueTask StoreChatHistoryAsync(InvokedContext context, CancellationToken cancellationToken = default)
        {

            string messages = _db.StringGet(GetKey(context.Session));
            List<ChatMessage> storedMessage = messages is null ? new List<ChatMessage>() : (System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(messages)).ToList();

            IEnumerable<ChatMessage> reducedmessage = new List<ChatMessage>();
            var allNewMessages = context.RequestMessages
                 .Concat(context.ResponseMessages ?? []);
            storedMessage.AddRange(allNewMessages);
            if (_summarizingChatReducer is not null)
            {
                reducedmessage = await _summarizingChatReducer.ReduceAsync(storedMessage, cancellationToken);
                if (reducedmessage.Any() && reducedmessage.Count() < storedMessage.Count)
                {
                    await _db.StringSetAsync(GetKey(context.Session), System.Text.Json.JsonSerializer.Serialize(reducedmessage));
                }
                else
                {
                    await _db.StringSetAsync(GetKey(context.Session), System.Text.Json.JsonSerializer.Serialize(storedMessage));
                }
            }
            else
            {
                await _db.StringSetAsync(GetKey(context.Session), System.Text.Json.JsonSerializer.Serialize(storedMessage));
            }
        }

        private string GetKey(AgentSession session)
        {
            session.StateBag.TryGetValue("conversationId", out string conversationId);
            session.StateBag.TryGetValue("UserId", out string UserId);
            return $"conversation:{conversationId}:user:{UserId}";
        }
    }
}
