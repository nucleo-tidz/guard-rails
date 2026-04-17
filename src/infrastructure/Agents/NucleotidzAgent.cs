namespace infrastructure.Agents
{
    using application.Services.Interfaces;
    using application.Dtos;

    using infrastructure.Agents.Adaptors;
    using infrastructure.Agents.HistoryProvider;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    internal class NucleotidzAgent(IChatClient chatClient,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IEmbedService embedService, ITextSearchAdapter textSearchAdapter) : INucleotidzAgent
    {
        private AIAgent Create(string conversationId, string UserId) =>
              chatClient.AsAIAgent(new ChatClientAgentOptions
              {
                  ChatOptions = new ChatOptions()
                  {
                      Instructions = "You are a shiptech Agent providing user information on shiptech company, you can also guide use which container is best to book based on user's need",
                      ToolMode = ChatToolMode.Auto,
                  },
                  Description = "A shiptech company assistant",
                  AIContextProviders = [
                    new TextSearchProvider(textSearchAdapter.SearchAdapter, new()
                       {
                           SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
                           RecentMessageMemoryLimit = 5,
                           StateKey = "document",
                       })],
                  ChatHistoryProvider = new RedisChatHistoryProvider(session => new RedisChatHistoryProvider.State(conversationId, UserId), summarizingChatReducer: new SummarizingChatReducer(chatClient, 2, 3)),
              }).AsBuilder().Use(GuardrailMiddleware,null).Build();

        public async Task<string> Start(string conversationId, string UserId, string message)
        {
            var agent = Create(conversationId, UserId);
            var session = await agent.CreateSessionAsync();
            var response = await agent.RunAsync(message, session);
            return response.Text;

        }

        async Task<AgentResponse> GuardrailMiddleware(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken)
        {
           
            var response = await innerAgent.RunAsync(messages, session, options, cancellationToken);
            var data = textSearchAdapter._context;
            return response;
        }
    }
}
