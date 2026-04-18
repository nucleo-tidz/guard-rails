namespace infrastructure.Agents.Midllewares
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using infrastructure.Agents.Adaptors;
    using infrastructure.Agents.Guardrails;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    internal class GuardRailMiddleware(ITextSearchAdapter textSearchAdapter, IGroundnessDetector groundnessDetector) : IGuardRailMiddleware
    {
        public async Task<AgentResponse> GuardrailMiddleware(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken)
        {

            var response = await innerAgent.RunAsync(messages, session, options, cancellationToken);
            var data = textSearchAdapter._context;
            var grounded = await groundnessDetector.DetectGroundness(
                 messages.LastOrDefault(m => m.Role == ChatRole.User).Text,
                 response.Text,
                 data.Select(_ => _.Text).ToList());

            if (grounded.UngroundedPercentage > 0.30)
            {
                var warningMessage = $"{response.Text}\n\n⚠️ **Disclaimer**: This response may contain information not based on verified facts . Please verify critical information from official sources.";
                response = new Microsoft.Agents.AI.AgentResponse(new ChatMessage(ChatRole.Assistant, warningMessage));
            }
            return response;
        }
    }
}
