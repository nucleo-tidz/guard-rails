namespace infrastructure.Agents.HistoryProvider
{
    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    using StackExchange.Redis;

    using System.Text.Json;
    using System.Text.Json.Serialization;

    using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

    public sealed class RedisChatHistoryProvider : ChatHistoryProvider
    {
        private readonly ProviderSessionState<State> _sessionState;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        SummarizingChatReducer _summarizingChatReducer;

        public RedisChatHistoryProvider(
            Func<AgentSession?, State>? stateInitializer = null,
            SummarizingChatReducer summarizingChatReducer = null)
        {
            this._sessionState = new ProviderSessionState<State>(stateInitializer, this.GetType().Name);
            _redis = ConnectionMultiplexer.Connect("localhost:6379");
            _db = _redis.GetDatabase();
            _summarizingChatReducer = summarizingChatReducer;

        }


        
        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(InvokingContext context, CancellationToken cancellationToken = default)
        {
            var state = this._sessionState.GetOrInitializeState(context.Session);
            string messages = _db.StringGet(state.ConversationKey);
            if (messages == null)
                return new(new List<ChatMessage>());
            return new((System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(messages)).OrderBy(x => x.CreatedAt));
        }

        protected override async ValueTask StoreChatHistoryAsync(InvokedContext context, CancellationToken cancellationToken = default)
        {
            var state = this._sessionState.GetOrInitializeState(context.Session);
            string messages = _db.StringGet(state.ConversationKey);
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
                    await _db.StringSetAsync(state.ConversationKey, System.Text.Json.JsonSerializer.Serialize(reducedmessage));
                }
                else
                {
                    await _db.StringSetAsync(state.ConversationKey, System.Text.Json.JsonSerializer.Serialize(storedMessage));
                }
            }
            else
            {
                await _db.StringSetAsync(state.ConversationKey, System.Text.Json.JsonSerializer.Serialize(storedMessage));
            }
        }

        public sealed class State
        {
            public State(string conversationId, string userId )
            {
                ConversationId = conversationId;
                UserId = userId;
                ConversationKey= $":{conversationId}:{userId}";
            }

            public string ConversationId { get; }

            public string UserId { get; }

            public string ConversationKey { get; }
        }
    }
}
