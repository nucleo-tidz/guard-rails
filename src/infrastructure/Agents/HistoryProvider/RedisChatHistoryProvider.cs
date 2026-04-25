namespace infrastructure.Agents.HistoryProvider
{
    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using model;
    using StackExchange.Redis;
    using System;
    using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

    public sealed class RedisChatHistoryProvider : ChatHistoryProvider
    {
        private readonly ProviderSessionState<string> _sessionState;
        private readonly IDatabase _db;
        private readonly SummarizingChatReducer? _summarizingChatReducer;

        public RedisChatHistoryProvider(
            IConnectionMultiplexer redis,
            ISharedContext sharedContext,
            SummarizingChatReducer? summarizingChatReducer = null)
        {
           
            string statekey = $"conversation:{sharedContext.ThreadId}:user:{sharedContext.User}";
            this._sessionState = new ProviderSessionState<string>(s => statekey, statekey);
            _db = redis.GetDatabase();
            _summarizingChatReducer = summarizingChatReducer;
        }

        protected override ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(InvokingContext context, CancellationToken cancellationToken = default)
        {
            var stateKey = this._sessionState.GetOrInitializeState(context.Session);
            string messages = _db.StringGet(stateKey);
            if (messages == null)
                return new(new List<ChatMessage>());
            return new((System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ChatMessage>>(messages)));
        }

        protected override async ValueTask StoreChatHistoryAsync(InvokedContext context, CancellationToken cancellationToken = default)
        {
            var stateKey = this._sessionState.GetOrInitializeState(context.Session);
            string messages = _db.StringGet(stateKey);
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
                    await _db.StringSetAsync(stateKey, System.Text.Json.JsonSerializer.Serialize(reducedmessage));
                }
                else
                {
                    await _db.StringSetAsync(stateKey, System.Text.Json.JsonSerializer.Serialize(storedMessage));
                }
            }
            else
            {
                await _db.StringSetAsync(stateKey, System.Text.Json.JsonSerializer.Serialize(storedMessage));
            }
        }
    }
}
