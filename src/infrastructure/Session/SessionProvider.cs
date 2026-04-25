using Microsoft.Agents.AI;
using model;
using StackExchange.Redis;
using System.Text.Json;

namespace infrastructure.Session
{
    internal class SessionProvider(ISharedContext sharedContext, IConnectionMultiplexer redis) : ISessionProvider
    {
        string sessionKey()=> sharedContext.SessionKey;
        public async Task<AgentSession> Provide(AIAgent agent)
        {
            var db = redis.GetDatabase();
            string rawSeesion = db.StringGet(sessionKey());
            if (rawSeesion is not null)
            {
                return await agent.DeserializeSessionAsync(JsonElement.Parse(rawSeesion));
            }
            return await agent.CreateSessionAsync();
        }
        public async Task SaveSession(JsonElement jsonElement)
        {
            var db = redis.GetDatabase();
            await db.StringSetAsync(sessionKey(), jsonElement.GetRawText());
        }
    }
}
