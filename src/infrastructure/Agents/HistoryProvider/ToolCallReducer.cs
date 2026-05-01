namespace infrastructure.Agents.HistoryProvider
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.AI;

    public class ToolCallReducer : IChatReducer
    {
        public Task<IEnumerable<ChatMessage>> ReduceAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken)
        {
            List<ChatMessage> toKeep = [];
            foreach (var message in messages)
            {
                if (message.Role == ChatRole.Tool)
                {
                    continue; 
                }
                if (message.Role == ChatRole.Assistant && message.Contents.Any(x => x is FunctionCallContent))
                {
                    continue; 
                }
                toKeep.Add(message);
            }
            return Task.FromResult(toKeep.AsEnumerable());
        }
    }
}
