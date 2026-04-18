namespace infrastructure.Agents.HistoryProvider
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    using StackExchange.Redis;

    using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

    public sealed class RedisChatHistoryProvider : ChatHistoryProvider
    {
 
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        SummarizingChatReducer _summarizingChatReducer;

        public RedisChatHistoryProvider(
            SummarizingChatReducer summarizingChatReducer = null)
        {
     
            _redis = ConnectionMultiplexer.Connect("localhost:6379");
            _db = _redis.GetDatabase();
            _summarizingChatReducer = summarizingChatReducer;
        }
        
        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(InvokingContext context, CancellationToken cancellationToken = default)
        {
          
            string messages = _db.StringGet(GetKey(context.Session));
            if (messages == null)
                return new(new List<ChatMessage>());
            return new((System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(messages)).OrderBy(x => x.CreatedAt));
        }

        protected override async ValueTask StoreChatHistoryAsync(InvokedContext context, CancellationToken cancellationToken = default)
        {
           
            string messages = _db.StringGet(GetKey(context.Session));
            List<ChatMessage> storedMessage = messages is null ? new List<ChatMessage>() : (System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(messages)).OrderBy(x => x.CreatedAt).ToList();

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
