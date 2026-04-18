namespace infrastructure.Agents.HistoryProvider
{
    using System;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    using model;

    using StackExchange.Redis;

    using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

    public sealed class RedisChatHistoryProvider : ChatHistoryProvider
    {
 
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        SummarizingChatReducer _summarizingChatReducer;
        ISharedContext _sharedContext;
        public RedisChatHistoryProvider(ISharedContext sharedContext,
            SummarizingChatReducer summarizingChatReducer = null)
        {
     
            _redis = ConnectionMultiplexer.Connect("localhost:6379");
            _db = _redis.GetDatabase();
            _summarizingChatReducer = summarizingChatReducer;
            _sharedContext = sharedContext;
        }
        
        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(InvokingContext context, CancellationToken cancellationToken = default)
        {
          
            string messages = _db.StringGet(GetKey());
            if (messages == null)
                return new(new List<ChatMessage>());
            return new((System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(messages)));
        }

        protected override async ValueTask StoreChatHistoryAsync(InvokedContext context, CancellationToken cancellationToken = default)
        {
           
            string messages = _db.StringGet(GetKey());
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
                    await _db.StringSetAsync(GetKey(), System.Text.Json.JsonSerializer.Serialize(reducedmessage));
                }
                else
                {
                    await _db.StringSetAsync(GetKey(), System.Text.Json.JsonSerializer.Serialize(storedMessage));
                }
            }
            else
            {
                await _db.StringSetAsync(GetKey(), System.Text.Json.JsonSerializer.Serialize(storedMessage));
            }
        }

        private string GetKey()
        {
            return $"conversation:{_sharedContext.ThreadId}:user:{_sharedContext.User}";
        }
    }
}
