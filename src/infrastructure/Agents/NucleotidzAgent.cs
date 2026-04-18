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

    internal class NucleotidzAgent(IChatClient chatClient,
        [FromKeyedServices("nucleotidz")] AIAgent agent,
        ITextSearchAdapter textSearchAdapter,
        IGroundnessDetector groundnessDetector) : INucleotidzAgent
    {
        public async Task<string> Start(string conversationId, string UserId, string message)
        {
            var session = await agent.CreateSessionAsync();
            session.StateBag.SetValue("conversationId", conversationId);
            session.StateBag.SetValue("UserId", UserId);
            var response = await agent.RunAsync(message, session);
            return response.Text;
        }       
    }
}
