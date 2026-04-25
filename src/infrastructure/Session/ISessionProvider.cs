using Microsoft.Agents.AI;
using System.Text.Json;

namespace infrastructure.Session
{
    public interface ISessionProvider
    {
        Task<AgentSession> Provide(AIAgent agent);
        Task SaveSession(JsonElement jsonElement);
    }
}
