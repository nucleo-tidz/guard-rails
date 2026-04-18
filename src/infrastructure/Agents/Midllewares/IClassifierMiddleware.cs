namespace infrastructure.Agents.Midllewares
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    public interface IClassifierMiddleware
    {
        Task<AgentResponse> Classify(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken);
    }
}
