namespace infrastructure.Agents.Midllewares
{
    using application.Services.Interfaces;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    using model;
    using model.Enums;

    internal class ClassifierMiddleware(
        ISharedContext sharedContext,
        IQueryIntentClassifier queryIntentClassifier,
        IChatHistoryReader chatHistoryReader) : IClassifierMiddleware
    {
        private const int HistoryWindowSize = 6;

        public async Task<AgentResponse> Classify(
            IEnumerable<ChatMessage> messages,
            AgentSession? session,
            AgentRunOptions? options,
            AIAgent innerAgent,
            CancellationToken cancellationToken)
        {
            var userMessage = messages.FirstOrDefault(m => m.Role == ChatRole.User)?.Text ?? string.Empty;

            IEnumerable<ChatMessage> recentHistory = [];

            if (!string.IsNullOrEmpty(sharedContext.User) && !string.IsNullOrEmpty(sharedContext.ThreadId))
            {
                recentHistory = await chatHistoryReader.GetRecentMessagesAsync(HistoryWindowSize, cancellationToken);
            }

            sharedContext.queryIntent = await queryIntentClassifier.ClassifyAsync(userMessage, recentHistory, cancellationToken);

            return await innerAgent.RunAsync(messages, session, options, cancellationToken);
        }
    }
}
