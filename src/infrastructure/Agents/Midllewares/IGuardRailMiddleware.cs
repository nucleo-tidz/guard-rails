namespace infrastructure.Agents.Midllewares
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    public interface IGuardRailMiddleware
    {
        Task<AgentResponse> GroudnessDetection(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken);
        Task<AgentResponse> JailBreakDetection(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken);
        Task<AgentResponse> PersonalCategoryDetection(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken);
    }
}
