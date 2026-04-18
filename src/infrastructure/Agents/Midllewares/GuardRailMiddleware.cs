namespace infrastructure.Agents.Midllewares
{
    using System.Collections.Generic;

    using infrastructure.Agents.Guardrails;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    using model;

    internal class GuardRailMiddleware(   
        IGroundnessDetector groundnessDetector,
        IJailBreakDetector jailBreakDetector,
        IPersonalCaetgoryDetector personalCaetgoryDetector,
        ISharedContext _sharedContext
        ) : IGuardRailMiddleware
    {
        public async Task<AgentResponse> GroudnessDetection(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken)
        {

            var response = await innerAgent.RunAsync(messages, session, options, cancellationToken);
            var data = _sharedContext.ragContexts;
            var grounded = await groundnessDetector.DetectGroundness(
                 messages.FirstOrDefault(m => m.Role == ChatRole.User).Text,
                 response.Text,
                 data.Select(_ => _.Text).ToList());
            if (grounded.UngroundedPercentage > 0.30)
            {
                var warningMessage = $"{response.Text}\n\n⚠️ **Disclaimer**: This response may contain information not based on verified facts . Please verify critical information from official sources.";
                response = new Microsoft.Agents.AI.AgentResponse(new ChatMessage(ChatRole.Assistant, warningMessage));
            }
            return response;
        }

        public async Task<AgentResponse> JailBreakDetection(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken)
        {
            var userInput = messages.FirstOrDefault(m => m.Role == ChatRole.User).Text;
            var response = await jailBreakDetector.DetectJailBreak(userInput, _sharedContext.ragContexts.Select(_ => _.Text).ToList());
            if (response.DocumentsAnalysis.Any(_ => _.AttackDetected) || response.UserPromptAnalysis.AttackDetected)
            {
                return new AgentResponse(new ChatMessage(ChatRole.Assistant, "⚠️ **Alert**: Potential Jailbreak Attempt Detected. The user's input may be trying to bypass security measures. Please review the conversation for potential risks."));
            }
            return await innerAgent.RunAsync(messages, session, options, cancellationToken);
        }
        public async Task<AgentResponse> PersonalCategoryDetection(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken)
        {
            var userInput = messages.FirstOrDefault(m => m.Role == ChatRole.User).Text;

            var response = await personalCaetgoryDetector.DetectPII(userInput, "presonaldata");
            if (response?.customCategoryAnalysis?.detected == true)
            {
                return new AgentResponse(new ChatMessage(ChatRole.Assistant, "⚠️ **Alert**: Personal data detcted in the prompt "));
            }
            return await innerAgent.RunAsync(messages, session, options, cancellationToken);
        }
    }
}
