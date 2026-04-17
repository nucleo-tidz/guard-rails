namespace infrastructure.Agents
{
    using application.Services.Interfaces;

    using infrastructure.Agents.Adaptors;
    using infrastructure.Agents.HistoryProvider;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.Options;

    internal class NucleotidzAgent(IChatClient chatClient,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        IEmbedService embedService, ITextSearchAdapter textSearchAdapter) : INucleotidzAgent
    {
        private AIAgent Create(string conversationId, string UserId) =>
              chatClient.AsAIAgent(new ChatClientAgentOptions
              {
                  ChatOptions = new ChatOptions()
                  {
                      Instructions = "You are a  Shiptech Assistant helping users with container shipping services. Provide information about our shipping solutions, help users find the best container options for their cargo needs, answer questions about shipping routes, vessel schedules, port information, and booking procedures. Guide users through the container selection process based on cargo type, weight, dimensions, and shipping requirements.",
                      ToolMode = ChatToolMode.Auto,
                  },
                  Description = "A  shiptech company assistant",
                  AIContextProviders = [
                    new TextSearchProvider(textSearchAdapter.SearchAdapter, new()
                       {
                           SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
                           RecentMessageMemoryLimit = 5,
                           StateKey = "document"
                       })],
                  ChatHistoryProvider = new RedisChatHistoryProvider(session => new RedisChatHistoryProvider.State(conversationId, UserId), summarizingChatReducer: new SummarizingChatReducer(chatClient, 2, 3)),
              });
        public async Task<string> Start(string conversationId, string UserId, string message)
        {
            var agent = Create(conversationId, UserId);
            var session = await agent.CreateSessionAsync();
            return (await agent.RunAsync(message, session)).Text;
        }
    }
}
