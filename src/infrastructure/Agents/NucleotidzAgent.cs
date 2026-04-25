namespace infrastructure.Agents
{
    using application.Services.Interfaces;
    using infrastructure.Agents.Factory;
    using Microsoft.Agents.AI;
    using model;
    using StackExchange.Redis;
    using System.Text.Json;

    internal class NucleotidzAgent(IAgentFactory agentFactory, ISharedContext sharedContext, IConnectionMultiplexer redis) : INucleotidzAgent
    {
        public async Task<string> Start(string message)
        {
            var agent = agentFactory.Create();
            var session = await CreateSession(agent);
            var response = await agent.RunAsync(message, session);
            JsonElement serializedSession = await agent.SerializeSessionAsync(session);
                await SaveSession(serializedSession);
            return response.Text;
        }

        public async Task<AgentSession> CreateSession(AIAgent agent)
        {
            var db = redis.GetDatabase();
            string rawSeesion = db.StringGet($"sessionkey:{sharedContext.User}-{sharedContext.ThreadId}");
            if (rawSeesion is not null)
            {
                return await agent.DeserializeSessionAsync(JsonElement.Parse(rawSeesion));
            }
            return await agent.CreateSessionAsync();
        }

        public async Task SaveSession(JsonElement jsonElement)
        {
            var db = redis.GetDatabase();

            await db.StringSetAsync($"sessionkey:{sharedContext.User}-{sharedContext.ThreadId}", jsonElement.GetRawText());
        }
    }
}
