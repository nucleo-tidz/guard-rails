namespace infrastructure.Agents.Midllewares
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using application.Services.Interfaces;

    using Microsoft.Agents.AI;
    using Microsoft.Extensions.AI;

    using model;
    using model.Enums;

    internal class ClassifierMiddleware(ISharedContext sharedContext, IQueryIntentClassifier queryIntentClassifier, ITextSearchAdapter searchAdapter) : IClassifierMiddleware
    {
        public async Task<AgentResponse> Classify(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken)
        {
            QueryIntent queryIntent = await queryIntentClassifier.ClassifyAsync(messages.FirstOrDefault(m => m.Role == ChatRole.User).Text);
            sharedContext.queryIntent = queryIntent;
            await searchAdapter.Search(messages.FirstOrDefault(m => m.Role == ChatRole.User).Text, cancellationToken);            
            return await innerAgent.RunAsync(messages, session, options, cancellationToken);
        }
    }
}
