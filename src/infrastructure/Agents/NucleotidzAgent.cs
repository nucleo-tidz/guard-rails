namespace infrastructure.Agents
{
    using application.Services.Interfaces;
    using infrastructure.Agents.Factory;
    using infrastructure.Session;
    using System.Text.Json;

    internal class NucleotidzAgent(IAgentFactory agentFactory, ISessionProvider sessionProvider) : INucleotidzAgent
    {
        public async Task<string> Start(string message)
        {
            var agent = agentFactory.Create();
            var session = await sessionProvider.Provide(agent);
            var response = await agent.RunAsync(message, session);
            JsonElement serializedSession = await agent.SerializeSessionAsync(session);
                await sessionProvider.SaveSession(serializedSession);
            return response.Text;
        }
    }
}
