namespace infrastructure.Agents
{
    using System.Text.Json;

    using application.Services.Interfaces;

    using infrastructure.Agents.Factory;
    using infrastructure.Agents.TokenManager;
    using infrastructure.Session;

    using Microsoft.Agents.AI;

    internal class NucleotidzAgent(IAgentFactory agentFactory, ISessionProvider sessionProvider, ITokenLimiter tokenLimiter) : INucleotidzAgent
    {
        public async Task<string> Start(string message)
        {
            var agent = agentFactory.Create();
            var session = await sessionProvider.Provide(agent);
            var response = await agent.RunAsync(message, session);
            await tokenLimiter.ConsumeAsync(response.Usage.TotalTokenCount.Value);
            JsonElement serializedSession = await agent.SerializeSessionAsync(session);
            await sessionProvider.SaveSession(serializedSession);
            return response.Text;
        }
    }
}
