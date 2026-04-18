namespace infrastructure.Agents
{
    using application.Services.Interfaces;

    using infrastructure.Agents.Guardrails;
    using infrastructure.Agents.HistoryProvider;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.DependencyInjection;

    using model;

    using Pipelines.Sockets.Unofficial.Arenas;

    internal class NucleotidzAgent(IChatClient chatClient,
        [FromKeyedServices("nucleotidz")] AIAgent agent,
        ISharedContext sharedContext,
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) : INucleotidzAgent
    {
        public async Task<string> Start(string message)
        {
            var memoryProvider = new MemoryProviderBuilder(embeddingGenerator)
           .WithCollectionName("memory")
           .WithVectorDimensions(3072)
           .WithStorageScope(sharedContext.User, sharedContext.ThreadId)
           .Build();

            var session = await agent.CreateSessionAsync();
            session.StateBag.SetValue("conversationId", sharedContext.ThreadId);
            session.StateBag.SetValue("UserId", sharedContext.User);
            var response = await agent.RunAsync(message, session);
            return response.Text;
        }
    }
}
