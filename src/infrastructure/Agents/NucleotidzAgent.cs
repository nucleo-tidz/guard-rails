namespace infrastructure.Agents
{
    using application.Dtos;
    using application.Services.Interfaces;

    using infrastructure.Agents.Adaptors;
    using infrastructure.Agents.Guardrails;
    using infrastructure.Agents.HistoryProvider;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.SemanticKernel.Connectors.Redis;

    using StackExchange.Redis;

    internal class NucleotidzAgent(IChatClient chatClient,
        [FromKeyedServices("nucleotidz")] AIAgent agent,
        ITextSearchAdapter textSearchAdapter,
        IGroundnessDetector groundnessDetector,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : INucleotidzAgent
    {
        public async Task<string> Start(string conversationId, string UserId, string message)
        {
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            var vectorStore = new RedisVectorStore(redis.GetDatabase(), new RedisVectorStoreOptions { EmbeddingGenerator = embeddingGenerator });
            var builtAgent = agent.AsBuilder().UseAIContextProviders
                 (
                 new ChatHistoryMemoryProvider(vectorStore, collectionName: "memory", vectorDimensions: 3072,
                 session => new ChatHistoryMemoryProvider.State(storageScope: new() { UserId = UserId, SessionId = Guid.NewGuid().ToString() }, searchScope: new() { UserId = UserId }))
                 ).Build();
            var session = await builtAgent.CreateSessionAsync();
            session.StateBag.SetValue("conversationId", conversationId);
            session.StateBag.SetValue("UserId", UserId);
            var response = await builtAgent.RunAsync(message, session);
            return response.Text;
        }
    }
}
